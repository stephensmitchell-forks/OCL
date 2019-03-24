using System.Collections.Generic;

/*  $Id$
 * 
 *  Copyright (c) 2010-2011 Anders Wallin (anders.e.e.wallin "at" gmail.com).
 *  
 *  This file is part of OpenCAMlib 
 *  (see https://github.com/aewallin/opencamlib).
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 2.1 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *  
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
*/

//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <boost/foreach.hpp>

#if _OPENMP
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//	#include <omp.h>
#endif

/*  $Id$
 * 
 *  Copyright (c) 2010-2011 Anders Wallin (anders.e.e.wallin "at" gmail.com).
 *  
 *  This file is part of OpenCAMlib 
 *  (see https://github.com/aewallin/opencamlib).
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 2.1 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *  
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
*/



//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)


namespace ocl
{


/// \brief a Waterline toolpath follows the shape of the model at a constant z-height in the xy-plane

/// The Waterline object is used for generating waterline or z-slice toolpaths
/// from an STL-model. Waterline uses two BatchPushCutter sub-operations to find out where the CL-points are located
/// and a Weave to split and order the CL-points correctly into loops.
public class Waterline : Operation
{
		/// create an empty Waterline object

		//********   ********************** */

		public Waterline()
		{
			subOp.Clear();
			subOp.Add(new BatchPushCutter());
			subOp.Add(new BatchPushCutter());
			subOp[0].setXDirection();
			subOp[1].setYDirection();
			nthreads = 1;
#if _OPENMP
			nthreads = omp_get_num_procs();
			//omp_set_dynamic(0);
			omp_set_nested(1);
#endif

		}

		public override void Dispose()
		{
			// std::cout << "~Waterline(): subOp.size()= " << subOp.size() <<"\n";
			subOp.Clear();
			base.Dispose();
		}

		/// Set the z-coordinate for the waterline we generate
		public void setZ(double z)
		{
			zh = z;
		}
		/// run the Waterline algorithm. setSTL, setCutter, setSampling, and setZ must
		/// be called before a call to run()
		public override void run()
		{
			init_fibers();
			subOp[0].run(); // these two are independent, so could/should run in parallel
			subOp[1].run();

			xfibers = new List<Fiber>(subOp[0].getFibers());
			yfibers = new List<Fiber>(subOp[1].getFibers());

			weave_process();
		}


		// run the batchpuschutter sub-operations to get x- and y-fibers
		// pass the fibers to weave, and process the weave to get waterline-loops
		public virtual void run2()
		{
			init_fibers();
			subOp[0].run(); // these two are independent, so could/should run in parallel
			subOp[1].run();

			xfibers = new List<Fiber>(subOp[0].getFibers());
			yfibers = new List<Fiber>(subOp[1].getFibers());

			weave_process2();
		}

		/// returns a vector< vector< Point > > with the resulting waterline loops
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: ClassicVector< ClassicVector<Point>> getLoops() const
		public List< List<Point>> getLoops()
		{
			return new List< List<Point>>(loops);
		}
		public new void reset()
		{
			xfibers.Clear();
			yfibers.Clear();
			subOp[0].reset();
			subOp[1].reset();
		}

		/// from xfibers and yfibers, build the weave, run face-traverse, and write toolpaths to loops
		protected void weave_process()
		{
			// std::cout << "Weave...\n" << std::flush;
			weave.SimpleWeave weave = new weave.SimpleWeave();
			foreach (Fiber f in xfibers)
			{
				weave.addFiber(f);
			}
			foreach (Fiber f in yfibers)
			{
				weave.addFiber(f);
			}

			//std::cout << "Weave::build()..." << std::flush;
			weave.build();
			// std::cout << "done.\n";

			// std::cout << "Weave::face traverse()...";
			weave.face_traverse();
			// std::cout << "done.\n";

			// std::cout << "Weave::get_loops()...";
			loops = new List< List<Point>>(weave.getLoops());
			// std::cout << "done.\n";
		}

		protected void weave_process2()
		{
			// std::cout << "Weave...\n" << std::flush;
			weave.SmartWeave weave = new weave.SmartWeave();
			foreach (Fiber f in xfibers)
			{
				weave.addFiber(f);
			}
			foreach (Fiber f in yfibers)
			{
				weave.addFiber(f);
			}

			//std::cout << "Weave::build2()..." << std::flush;
			weave.build();
			// std::cout << "done.\n";

			// std::cout << "Weave::face traverse()...";
			weave.face_traverse();
			// std::cout << "done.\n";

			// std::cout << "Weave::get_loops()...";
			loops = new List< List<Point>>(weave.getLoops());
			// std::cout << "done.\n";
		}

		/// initialization of fibers
		protected void init_fibers()
		{
			// std::cout << " Waterline::init_fibers()\n";
			double minx = surf.bb.minpt.x - 2 *cutter.getRadius();
			double maxx = surf.bb.maxpt.x + 2 *cutter.getRadius();
			double miny = surf.bb.minpt.y - 2 *cutter.getRadius();
			double maxy = surf.bb.maxpt.y + 2 *cutter.getRadius();
			int Nx = (int)((maxx - minx) / sampling);
			int Ny = (int)((maxy - miny) / sampling);
			List<double> xvals = generate_range(minx, maxx, Nx);
			List<double> yvals = generate_range(miny, maxy, Ny);
			foreach (double y in yvals)
			{
				Point p1 = new Point(minx, y, zh);
				Point p2 = new Point(maxx, y, zh);
				Fiber f = new Fiber(p1, p2);
				subOp[0].appendFiber(f);
			}
			foreach (double x in xvals)
			{
				Point p1 = new Point(x, miny, zh);
				Point p2 = new Point(x, maxy, zh);
				Fiber f = new Fiber(p1, p2);
				subOp[1].appendFiber(f);
			}
		}

		/// x and y-coordinates for fiber generation

		// return a double-vector [ start , ... , end ] with N elements
		// for generating fibers.
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: ClassicVector<double> generate_range(double start, double end, int N) const
		protected List<double> generate_range(double start, double end, int N)
		{
			List<double> output = new List<double>();
			double d = (end - start) / (double)N;
			double v = start;
			for (int n = 0; n < (N + 1); ++n)
			{
				output.Add(v);
				v = v + d;
			}
			return new List<double>(output);
		}

	// DATA
		/// the z-height for this Waterline
		protected double zh;
		/// the results of this operation, a list of loops
		protected List< List<Point>> loops = new List< List<Point>>();

		/// x-fibers for this operation
		protected List<Fiber> xfibers = new List<Fiber>();
		/// y-fibers for this operation
		protected List<Fiber> yfibers = new List<Fiber>();

}



} // end namespace


// #include "weave.hpp"

// end file waterline.cpp
