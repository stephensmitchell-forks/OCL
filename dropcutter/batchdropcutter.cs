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

#if _OPENMP // this should really not be a check for Windows, but a check for OpenMP
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


///
/// BatchDropCutter takes a MillingCutter, an STLSurf, and a list of CLPoint's
/// and calls MillingCutter::dropCutter() for each CLPoint.
/// To find triangles overlapping the cutter a kd-tree data structure is used.
/// The list of CLPoint's will be updated with the correct z-height as well
/// as corresponding CCPoint's
/// Some versions of this algorithm use OpenMP for multi-threading.
public class BatchDropCutter : Operation
{

		//********   ********************** */

		public BatchDropCutter()
		{
			clpoints = new List<CLPoint>();
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
			clpoints.Clear();
			if (clpoints != null)
			{
				clpoints.Dispose();
			}
			if (root != null)
			{
				root.Dispose();
			}
			base.Dispose();
		}

		/// set the STL-surface and build kd-tree to enable optimized algorithm
		public new void setSTL(STLSurf s)
		{
			Console.Write("bdc::setSTL()\n");
			surf = s;
			root.setXYDimensions(); // we search for triangles in the XY plane, don't care about Z-coordinate
			root.setBucketSize((int)bucketSize);
			root.build(s.tris);
			Console.Write("bdc::setSTL() done.\n");
		}

		/// append to list of CL-points to evaluate
		public new void appendPoint(CLPoint p)
		{
			clpoints.Add(p);
		}

		/// run drop-cutter on all clpoints
		public override void run()
		{
			this.dropCutter5();
		}
	// getters and setters
		/// return a vector of CLPoints, the result of this operation
		public new List<CLPoint> getCLPoints()
		{
			return clpoints;
		}
		/// clears the vector of CLPoints
		public new void clearCLPoints()
		{
			clpoints.Clear();
		}

		/// unoptimized drop-cutter,  tests against all triangles of surface

		// drop cutter against all triangles in surface
		protected void dropCutter1()
		{
			Console.Write("dropCutterSTL1 ");
			Console.Write(clpoints.Count);
			Console.Write(" cl-points and ");
			Console.Write(surf.tris.Count);
			Console.Write(" triangles...");
			nCalls = 0;
			foreach (CLPoint cl in clpoints)
			{
				foreach (Triangle t in surf.tris)
				{ // test against all triangles in s
					cutter.dropCutter(cl,t);
					++nCalls;
				}
			}
			Console.Write("done.\n");
			return;
		}

		/// better, kd-tree optimized version      

		// first search for triangles under the cutter
		// then only drop cutter against found triangles
		protected void dropCutter2()
		{
			Console.Write("dropCutterSTL2 ");
			Console.Write(clpoints.Count);
			Console.Write(" cl-points and ");
			Console.Write(surf.tris.Count);
			Console.Write(" triangles.\n");
            /*
			std::cout.flush();
            */
			nCalls = 0;
			LinkedList<Triangle> triangles_under_cutter;
			foreach (CLPoint cl in clpoints)
			{ //loop through each CL-point
				triangles_under_cutter = root.search_cutter_overlap(cutter, cl);
				foreach (Triangle t in triangles_under_cutter)
				{
					cutter.dropCutter(cl,t);
					++nCalls;
				}
				triangles_under_cutter = null;
			}

			Console.Write("done. ");
			Console.Write(nCalls);
			Console.Write(" dropCutter() calls.\n");
            /*
			std::cout.flush();
            */
			return;
		}

		/// kd-tree and explicit overlap test      

		// compared to dropCutter2, add an additional explicit overlap-test before testing triangle
		protected void dropCutter3()
		{
			Console.Write("dropCutterSTL3 ");
			Console.Write(clpoints.Count);
			Console.Write(" cl-points and ");
			Console.Write(surf.tris.Count);
			Console.Write(" triangles.\n");
			nCalls = 0;
            /*
			boost::progress_display show_progress = new boost::progress_display(clpoints.Count);
            */
			LinkedList<Triangle> triangles_under_cutter;
			foreach (CLPoint cl in clpoints)
			{ //loop through each CL-point
				triangles_under_cutter = root.search_cutter_overlap(cutter, cl);
				foreach (Triangle t in triangles_under_cutter)
				{
					if (cutter.overlaps(cl,t))
					{
						if (cl.below(t))
						{
							cutter.dropCutter(cl,t);
							++nCalls;
						}
					}
				}
                /*
				++show_progress;
                */
				triangles_under_cutter = null;
			}

			Console.Write("done. ");
			Console.Write(nCalls);
			Console.Write(" dropCutter() calls.\n");
			return;
		}

		/// use OpenMP for multi-threading     

		// use OpenMP to share work between threads
		protected void dropCutter4()
		{
			Console.Write("dropCutterSTL4 ");
			Console.Write(clpoints.Count);
			Console.Write(" cl-points and ");
			Console.Write(surf.tris.Count);
			Console.Write(" triangles.\n");
            /*
			boost::progress_display show_progress = new boost::progress_display(clpoints.Count);
            */
			nCalls = 0;
			int calls = 0;
			int ntris = 0;
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
			LinkedList<Triangle> tris = new LinkedList<Triangle>();
#if _WIN32 // OpenMP version 2 of VS2013 OpenMP need signed loop variable
			int n; // loop variable
			int Nmax = clpoints.Count;
#else
			int n; // loop variable
			uint Nmax = (uint)clpoints.Count;
#endif
			List<CLPoint> clref = clpoints;
			int nloop = 0;
#if _OPENMP
			omp_set_num_threads(nthreads); // the constructor sets number of threads right
										   // or the user can explicitly specify something else
#endif
			LinkedList<Triangle>.Enumerator it;
		//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
		//    #pragma omp parallel for shared( nloop, ntris, calls, clref) private(n,tris,it)
				for (n = 0;n < Nmax ;n++)
				{ // PARALLEL OpenMP loop!
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
					nloop++;
					tris = new LinkedList<Triangle>(root.search_cutter_overlap(cutter, clref[n]));
					// assert( tris->size() <= ntriangles ); // can't possibly find more triangles than in the STLSurf
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
					for (it = tris.GetEnumerator(); it != tris.end() ; ++it)
					{ // loop over found triangles
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						if (cutter.overlaps(clref[n],it))
						{ // cutter overlap triangle? check
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							if (clref[n].below(it))
							{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								cutter.vertexDrop(clref[n],it);
								++calls;
							}
						}
					}
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
					for (it = tris.GetEnumerator(); it != tris.end() ; ++it)
					{ // loop over found triangles
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						if (cutter.overlaps(clref[n],it))
						{ // cutter overlap triangle? check
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							if (clref[n].below(it))
							{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								cutter.facetDrop(clref[n],it);
							}
						}
					}
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
					for (it = tris.GetEnumerator(); it != tris.end() ; ++it)
					{ // loop over found triangles
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						if (cutter.overlaps(clref[n],it))
						{ // cutter overlap triangle? check
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							if (clref[n].below(it))
							{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								cutter.edgeDrop(clref[n],it);
							}
						}
					}
					ntris += tris.Count;

                /*
                delete(tris);
                ++show_progress;
                */
            } // end OpenMP PARALLEL for
            nCalls = calls;
			Console.Write(" ");
			Console.Write(nCalls);
			Console.Write(" dropCutter() calls.\n");
			return;
		}

		/// version 5 of the algorithm

		// use OpenMP to share work between threads
		protected void dropCutter5()
		{
			Console.Write("dropCutterSTL5 ");
			Console.Write(clpoints.Count);
			Console.Write(" cl-points and ");
			Console.Write(surf.tris.Count);
			Console.Write(" triangles.\n");
            /*
			boost::progress_display show_progress = new boost::progress_display(clpoints.Count);
            */
			nCalls = 0;
			int calls = 0;
			int ntris = 0;
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
			LinkedList<Triangle> tris = new LinkedList<Triangle>();
#if _WIN32 // OpenMP version 2 of VS2013 OpenMP need signed loop variable
			int Nmax = clpoints.Count;
			int n; // loop variable
#else
			uint Nmax = (uint)clpoints.Count;
			int n; // loop variable
#endif
			List<CLPoint> clref = clpoints;
			int nloop = 0;

#if _OPENMP
			omp_set_num_threads(nthreads); // the constructor sets number of threads right
										   // or the user can explicitly specify something else
#endif
			LinkedList<Triangle>.Enumerator it;
		//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
		//    #pragma omp parallel for schedule(dynamic) shared( nloop, ntris, calls, clref ) private(n,tris,it)
				for (n = 0;n < Nmax;++n)
				{ // PARALLEL OpenMP loop!
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
					nloop++;
					tris = new LinkedList<Triangle>(root.search_cutter_overlap(cutter, clref[n]));
					Debug.Assert(tris!=null);
					// assert( tris->size() <= ntriangles ); // can't possibly find more triangles than in the STLSurf
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
					for (it = tris.GetEnumerator(); it != tris.end() ; ++it)
					{ // loop over found triangles
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						if (cutter.overlaps(clref[n],it))
						{ // cutter overlap triangle? check
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							if (clref[n].below(it))
							{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								cutter.dropCutter(clref[n],it);
								++calls;
							}
						}
					}
					ntris += tris.Count;

                /*
                delete(tris);
                ++show_progress;
                */
            } // end OpenMP PARALLEL for
            nCalls = calls;
			Console.Write("\n ");
			Console.Write(nCalls);
			Console.Write(" dropCutter() calls.\n");
			return;
		}

	// DATA
		/// pointer to list of CL-points on which to run drop-cutter.
		protected List<CLPoint> clpoints;

}

} // end namespace



// end file batchdropcutter.cpp
