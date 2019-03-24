using System;
using System.Collections.Generic;
using System.Diagnostics;

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
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <boost/progress.hpp>

#if _OPENMP
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//	#include <omp.h>
#endif

/*  $Id$
 * 
 *  Copyright (c) 2010 Anders Wallin (anders.e.e.wallin "at" gmail.com).
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

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class STLSurf;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Triangle;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class MillingCutter;

///
/// BatchPushCutter takes a MillingCutter, an STLSurf, and many Fibers
/// and pushes the cutter along the fibers into contact with the surface.
/// When this runs the Fibers will be updated with the correct interval data.
/// This is then used to build a weave and extract a waterline.
public class BatchPushCutter : Operation
{

		//********   ********************** */

		public BatchPushCutter()
		{
			fibers = new List<Fiber>();
			nCalls = 0;
			nthreads = 1;
#if _OPENMP
			nthreads = omp_get_num_procs(); // figure out how many cores we have
#endif
			cutter = null;
			bucketSize = 1;
			root = new KDTree<Triangle>();
		}

		public override void Dispose()
		{
			if (fibers != null)
			{
				fibers.Dispose();
			}
			if (root != null)
			{
				root.Dispose();
			}
			base.Dispose();
		}

		/// set the STL-surface and build kd-tree
		public new void setSTL(STLSurf s)
		{
			surf = s;
			// std::cout << "BPC::setSTL() Building kd-tree... bucketSize=" << bucketSize << "..";
			root.setBucketSize((int)bucketSize);
			if (x_direction)
			{
				root.setYZDimensions(); // we search for triangles in the XY plane, don't care about Z-coordinate
			}
			else if (y_direction)
			{
				root.setXZDimensions();
			}
			else
			{
				std::cerr << "ERROR: setXDirection() or setYDirection() must be called before setSTL() \n";
				Debug.Assert(0);
			}
			// std::cout << "BPC::setSTL() root->build()...";
			root.build(s.tris);
			// std::cout << "done.\n";
		}

		/// set this bpc to be x-direction
		public new void setXDirection()
		{
			x_direction = true;
			y_direction = false;
		}
		/// set this bpc to be Y-direction
		public new void setYDirection()
		{
			x_direction = false;
			y_direction = true;
		}
		/// append to list of Fibers to evaluate
		public new void appendFiber(Fiber f)
		{
			fibers.Add(f);
		}


		/// run push-cutter
		public override void run()
		{
			this.pushCutter3();
		}
		//void run() {this->pushCutter1();}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: ClassicVector<Fiber>* getFibers() const
		public new List<Fiber> getFibers()
		{
			return fibers;
		}
		public new void reset()
		{
			fibers.Clear();
		}

		/// 1st version of algorithm

		/// very simple batch push-cutter
		/// each fiber is tested against all triangles of surface
		protected void pushCutter1()
		{
			// std::cout << "BatchPushCutter1 with " << fibers->size() <<
			//           " fibers and " << surf->tris.size() << " triangles..." << std::endl;
			nCalls = 0;
			boost::progress_display show_progress = new boost::progress_display(fibers.Count);
			foreach (Fiber f in * fibers)
			{
				foreach (Triangle t in surf.tris)
				{ // test against all triangles in s
					Interval i = new Interval();
					cutter.pushCutter(f,i,t);
					f.addInterval(i);
					++nCalls;
				}
				++show_progress;
			}
			// std::cout << "BatchPushCutter done." << std::endl;
			return;
		}

		/// 2nd version of algorithm

		/// push-cutter which uses KDNode2 kd-tree search to find triangles 
		/// overlapping with the cutter.
		protected void pushCutter2()
		{
			// std::cout << "BatchPushCutter2 with " << fibers->size() <<
			//           " fibers and " << surf->tris.size() << " triangles..." << std::endl;
			nCalls = 0;
			LinkedList<Triangle> overlap_triangles;
			boost::progress_display show_progress = new boost::progress_display(fibers.Count);
			foreach (Fiber f in * fibers)
			{
				CLPoint cl = new CLPoint();
				if (x_direction)
				{
					cl.x = 0;
					cl.y = f.p1.y;
					cl.z = f.p1.z;
				}
				else if (y_direction)
				{
					cl.x = f.p1.x;
					cl.y = 0;
					cl.z = f.p1.z;
				}
				else
				{
					Debug.Assert(0);
				}
				overlap_triangles = root.search_cutter_overlap(cutter, cl);
				Debug.Assert(overlap_triangles.Count <= surf.size()); // can't possibly find more triangles than in the STLSurf
				foreach (Triangle t in * overlap_triangles)
				{
					//if ( bb->overlaps( t.bb ) ) {
						Interval i = new Interval();
						cutter.pushCutter(f,i,t);
						f.addInterval(i);
						++nCalls;
					//}
				}
				delete(overlap_triangles);
				++show_progress;
			}
			// std::cout << "BatchPushCutter2 done." << std::endl;
			return;
		}

		/// 3rd version of algorithm

		/// use kd-tree search to find overlapping triangles
		/// use OpenMP for multi-threading
		protected void pushCutter3()
		{
			// std::cout << "BatchPushCutter3 with " << fibers->size() <<
			//           " fibers and " << surf->tris.size() << " triangles." << std::endl;
			// std::cout << " cutter = " << cutter->str() << "\n";
			nCalls = 0;
			boost::progress_display show_progress = new boost::progress_display(fibers.Count);
#if _OPENMP
			Console.Write("OpenMP is enabled");
			omp_set_num_threads(nthreads);
			//omp_set_nested(1);
#endif
			LinkedList<Triangle>.Enumerator it; // for looping over found triabgles
			LinkedList<Triangle>.Enumerator it_end;
			Interval i;
			LinkedList<Triangle> tris;
			List<Fiber> fiberr = fibers;
#if _WIN32 // OpenMP version 2 of VS2013 OpenMP need signed loop variable
			int n; // loop variable
			int Nmax = fibers.Count; // the number of fibers to process
#else
			uint n; // loop variable
			uint Nmax = (uint)fibers.Count; // the number of fibers to process
#endif
			uint calls = 0;

		//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
		//    #pragma omp parallel for schedule(dynamic) shared(calls, fiberr) private(n,i,tris,it,it_end)
			//#pragma omp parallel for shared( calls, fiberr) private(n,i,tris,it,it_end)
			for (n = 0; n < Nmax; ++n)
			{ // loop through all fibers
#if _OPENMP
				if (n == 0)
				{ // first iteration
					if (omp_get_thread_num() == 0)
					{
						Console.Write("Number of OpenMP threads = ");
						Console.Write(omp_get_num_threads());
						Console.Write("\n");
					}
				}
#endif
				CLPoint cl = new CLPoint(); // cl-point on the fiber
				if (x_direction)
				{
					cl.x = 0;
					cl.y = fiberr[n].p1.y;
					cl.z = fiberr[n].p1.z;
				}
				else if (y_direction)
				{
					cl.x = fiberr[n].p1.x;
					cl.y = 0;
					cl.z = fiberr[n].p1.z;
				}
				tris = root.search_cutter_overlap(cutter, cl);
				it_end = tris.end();
				for (it = tris.GetEnumerator() ; it != it_end ; ++it)
				{ // loop through the found overlapping triangles
					//if ( bb->overlaps( it->bb ) ) {
						// todo: optimization where method-calls are skipped if triangle bbox already in the fiber
						i = new Interval();
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						cutter.pushCutter(fiberr[n], i,it);
						fiberr[n].addInterval(i);
						++calls;
						if (i != null)
						{
							i.Dispose();
						}
					//}
				}
				delete(tris);
				++show_progress;
			} // OpenMP parallel region ends here

			this.nCalls = (int)calls;
			// std::cout << "\nBatchPushCutter3 done." << std::endl;
			return;
		}

		/// pointer to list of Fibers
		protected List<Fiber> fibers;

	// DATA
		/// true if this we have only x-direction fibers
		protected bool x_direction;
		/// true if we have y-direction fibers
		protected bool y_direction;
}

} // end namespace



// end file batchpushcutter.cpp
