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




namespace ocl
{

namespace weave
{

// Abstract base-class for weave-implementations. build() must be implemented in sub-class!
public abstract class Weave : System.IDisposable
{
		public Weave()
		{
		}
		public virtual void Dispose()
		{
		}
		/// add Fiber f to the graph
		/// each fiber should be either in the X or Y-direction
		/// FIXME: separate addXFiber and addYFiber methods?
		public void addFiber(Fiber f)
		{
			if (f.dir.xParallel() && !f.empty())
			{
				xfibers.Add(f);
			}
			else if (f.dir.yParallel() && !f.empty())
			{
				yfibers.Add(f);
			}
			else if (!f.empty())
			{
				Debug.Assert(0); // fiber must be either x or y
			}
		}

		/// from the list of fibers, build a graph
		public abstract void build();
		/// run planar_face_traversal to get the waterline loops

		// traverse the graph putting loops of vertices into the loops variable
		// this figure illustrates next-pointers: http://www.anderswallin.net/wp-content/uploads/2011/05/weave2_zoom.png
		public void face_traverse()
		{
			// std::cout << " traversing graph with " << clVertexSet.size() << " cl-points\n";
			while (clVertexSet.Count > 0)
			{ // while unprocessed cl-vertices remain
				List<Vertex> loop = new List<Vertex>(); // start on a new loop
				Vertex current = *(clVertexSet.GetEnumerator());
				Vertex first = new Vertex(current);

				do
				{ // traverse around the loop
					Debug.Assert(g[current].type == CL); // we only want cl-points in the loop
					loop.Add(current);
					clVertexSet.erase(current); // remove from set of unprocesser cl-verts
					List<Edge> outEdges = g.out_edges(current); // find the edge to follow
					//if (outEdges.size() != 1 )
					//    std::cout << " outEdges.size() = " << outEdges.size() << "\n";
					Debug.Assert(outEdges.Count == 1); // cl-points are always at ends of intervals, so they have only one out-edge
					Edge currentEdge = outEdges[0];
					do
					{ // following next, find a CL point
						current = g.target(currentEdge);
						currentEdge = g[currentEdge].next;
					} while (g[current].type != CL);
				} while (current != first); // end the loop when we arrive at the start

				loops.Add(loop); // add the processed loop to the master list of all loops
			}
		}

		/// return list of loops
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: ClassicVector< ClassicVector<Point>> getLoops() const
		public List< List<Point>> getLoops()
		{
			List< List<Point>> loop_list = new List< List<Point>>();
			foreach (List<Vertex> loop in loops)
			{
				List<Point> point_list = new List<Point>();
				foreach (Vertex v in loop)
				{
					point_list.Add(g[v].position);
				}
				loop_list.Add(point_list);
			}
			return new List< List<Point>>(loop_list);
		}

		/// string representation

		// string representation
		public string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << "Weave2\n";
			o << "  " << xfibers.Count << " X-fibers\n";
			o << "  " << yfibers.Count << " Y-fibers\n";
			return o.str();
		}


		// this can cause a build error when both face and vertex descriptors have the same type
		// i.e. unsigned int (?)
		// operator[] below "g[*itr].type" then looks for FaceProps.type which does not exist...
		public void printGraph()
		{
			Console.Write(" number of vertices: ");
			Console.Write(g.num_vertices());
			Console.Write("\n");
			Console.Write(" number of edges: ");
			Console.Write(g.num_edges());
			Console.Write("\n");

			int n = 0;
			int n_cl = 0;
			int n_internal = 0;
			foreach (Vertex v in g.vertices())
			{
				if (g[v].type == CL)
				{
					++n_cl;
				}
				else
				{
					++n_internal;
				}
				++n;
			}

			Console.Write(" counted ");
			Console.Write(n);
			Console.Write(" vertices\n");
			Console.Write("          CL-nodes: ");
			Console.Write(n_cl);
			Console.Write("\n");
			Console.Write("    internal-nodes: ");
			Console.Write(n_internal);
			Console.Write("\n");
		}

		protected WeaveGraph g = new WeaveGraph(); ///< the weave-graph
		protected List< List<Vertex>> loops = new List< List<Vertex>>(); ///< output: list of loops in this weave
		protected List<Fiber> xfibers = new List<Fiber>(); ///< the X-fibers
		protected List<Fiber> yfibers = new List<Fiber>(); ///< the Y-fibers
		protected SortedSet<Vertex> clVertexSet = new SortedSet<Vertex>(); ///< set of CL-points
}

} // end weave namespace

} // end ocl namespace
// end file weave.hpp



// end file weave.cpp
