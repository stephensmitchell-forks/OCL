using System;
using System.Collections.Generic;
using System.Diagnostics;

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


///
/// \brief run push-cutter on a single input fiber

public class FiberPushCutter : Operation
{

		//********   ********************** */

		public FiberPushCutter()
		{
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
			Console.Write("BPC::setSTL() Building kd-tree... bucketSize=");
			Console.Write(bucketSize);
			Console.Write("..");
			root.setBucketSize((int)bucketSize);
			if (x_direction)
			{
				root.setYZDimensions();
			}
			else if (y_direction)
			{
				root.setXZDimensions();
			}
			else
			{
				Console.Write("ERROR: setXDirection() or setYDirection() must be called before setSTL()");
				Debug.Assert(false);
			}
			Console.Write("BPC::setSTL() root->build()");
			root.build(s.tris);
			Console.Write(" done.\n");
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
		/// run() is an error.
		public override void run()
		{
			Debug.Assert(false);
		}
		public new void run(Fiber f)
		{
			pushCutter2(f);
		}

		/// input fiber is tested against all triangles of surface
		protected void pushCutter1(Fiber f)
		{
			nCalls = 0;
			foreach (Triangle t in surf.tris)
			{ // test against all triangles in s
				Interval i = new Interval();
				cutter.pushCutter(f,i,t);
				f.addInterval(i);
				++nCalls;
			}
		}

		/// use kd-tree search to find overlapping triangles
		protected void pushCutter2(Fiber f)
		{
			LinkedList<Triangle>.Enumerator it; // for looping over found triangles
			LinkedList<Triangle>.Enumerator it_end;
			Interval i;
			LinkedList<Triangle> tris;
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
			tris = root.search_cutter_overlap(cutter, cl);
			it_end = tris.end();
			for (it = tris.GetEnumerator() ; it != it_end ; ++it)
			{
				i = new Interval();
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
				cutter.pushCutter(f, i, it.Current);
				f.addInterval(i);
				++nCalls;
				if (i != null)
				{
					i.Dispose();
				}
			}
            /*
			delete(tris);
            */
		}

	// DATA
		/// true if this we have only x-direction fibers
		protected bool x_direction;
		/// true if we have y-direction fibers
		protected bool y_direction;
}

} // end namespace



// end file fiberpushcutter.cpp
