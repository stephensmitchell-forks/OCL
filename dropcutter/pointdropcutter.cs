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

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class STLSurf;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Triangle;

/// run drop-cutter on an STL-surface at a single input CLPoint
public class PointDropCutter : Operation
{

		//********   ********************** */

		public PointDropCutter()
		{
			nCalls = 0;
#if _OPENMP
			nthreads = omp_get_num_procs(); // figure out how many cores we have
#endif
			cutter = null;
			bucketSize = 1;
			root = new KDTree<Triangle>();
		}

		public override void Dispose()
		{
			//std::cout << " ~PointDropCutter() \n";
			if (root != null)
			{
				root.Dispose();
			}
			base.Dispose();
		}
		public new void setSTL(STLSurf s)
		{
			//std::cout << "PointDropCutter::setSTL()\n";
			surf = s;
			root.setXYDimensions(); // we search for triangles in the XY plane, don't care about Z-coordinate
			root.setBucketSize((int)bucketSize);
			root.build(s.tris);
		}

		public new void run(CLPoint clp)
		{
			//std::cout << "PointDropCutter::run() clp= " << clp << " dropped to ";
			pointDropCutter1(clp);
			//std::cout  << clp << " nCalls = " << nCalls <<"\n ";
		}

		public override void run()
		{
			Console.Write("ERROR: can't call run() on PointDropCutter()\n");
			Debug.Assert(false);
		}

		/// first simple implementation of this operation

		// use OpenMP to share work between threads
		protected void pointDropCutter1(CLPoint clp)
		{
			nCalls = 0;
			int calls = 0;
			LinkedList<Triangle> tris;
			//tris=new std::list<Triangle>();
			tris = root.search_cutter_overlap(cutter, clp);
			LinkedList<Triangle>.Enumerator it;
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
			for (it = tris.GetEnumerator(); it != tris.end() ; ++it)
			{ // loop over found triangles
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
				if (cutter.overlaps(clp,it))
				{ // cutter overlap triangle? check
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
					if (clp.below(it))
					{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						cutter.dropCutter(clp,it);
						++calls;
					}
				}
			}
            /*
			delete(tris);
            */
			nCalls = calls;
			return;
		}
}

} // end namespace




// end file pointdropcutter.cpp
