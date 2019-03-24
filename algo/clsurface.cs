using System;
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




namespace ocl
{

namespace clsurf
{







//typedef CLSGraph::Edge CLSEdge;
//typedef CLSGraph::Vertex CLSVertex;
//typedef hedi::HEDIGraph::Face CLSFace;



public class CLSVertexProps
{
	public CLSVertexProps()
	{
		init();
	}
	/// construct vertex at position p with type t
	public CLSVertexProps(Point p)
	{
		position.CopyFrom(p);
		init();
	}
	public void init()
	{
		index = count;
		count++;
	}
// HE data
	public Point position = new Point(); ///< the position of the vertex
	public int index; ///< index of vertex
	public static int count = 0; ///< global vertex count
}


public class CLSEdgeProps
{
	public CLSEdgeProps()
	{
	}
	/// create edge with given next, twin, and face
	public CLSEdgeProps(CLSEdge n, CLSEdge t, CLSFace f)
	{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: next = n;
		next.CopyFrom(n);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: twin = t;
		twin.CopyFrom(t);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: face = f;
		face.CopyFrom(f);
	}
	public CLSEdge next = new CLSEdge(); ///< the next edge, counterclockwise, from this edge
	public CLSEdge twin = new CLSEdge(); ///< the twin edge
	public CLSFace face = new CLSFace(); ///< the face to which this edge belongs
}

/// properties of a face 
public class CLSFaceProps
{
	/// create face with given edge, generator, and type
	public CLSFaceProps()
	{
	}
	public CLSFaceProps(CLSEdge e)
	{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: edge = e;
		edge.CopyFrom(e);
	}

	public CLSFace idx = new CLSFace(); ///< face index
	public CLSEdge edge = new CLSEdge(); ///< one edge that bounds this face

}


// the cutter location surface graph



/// \brief cutter location surface.
///
/// 1) start with a square sized like the bounding-box of the surface
/// 2) recursively subdivide until we reach sampling
/// 3) run drop cutter to project the surface
/// 4) adaptively subdivide until min_sampling where required
/// 5) do something:
///    - constant step-over (propagating geodesic windows on square grid is easy?)
///    - slicing (?)
///    - classify into steep/flat
///    - use for identifying rest-machining areas?
///
///    geodesic papers: "Fast Exact and Approximate Geodesics on Meshes"
///    doi 10.1145/1073204.1073228
///     http://research.microsoft.com/en-us/um/people/hoppe/geodesics.pdf
///
///    Accurate Computation of Geodesic Distance Fields for Polygonal Curves on Triangle Meshes
///    http://www.graphics.rwth-aachen.de/uploads/media/bommes_07_VMV_01.pdf
///
public class CutterLocationSurface : Operation, System.IDisposable
{
		public CutterLocationSurface()
		{
			far = 1.0;
			init();
		}
		/// create diagram with given far-radius and number of bins
		public CutterLocationSurface(double f)
		{
			far = f;
			init();
		}
		public virtual void Dispose()
		{
		}

		public void init()
		{
			// initialize cl-surf
			//
			//    b  e1   a
			//    e2      e4
			//    c   e3  d
			CLSVertex a = g.add_vertex(); // VertexProps( Point(far,far,0) ), g);
			g[a].position = new Point(far, far, 0);
			CLSVertex b = g.add_vertex(); // VertexProps( Point(-far,far,0) ), g);
			g[b].position = new Point(-far, far, 0);
			CLSVertex c = g.add_vertex(); // VertexProps( Point(-far,-far,0) ), g);
			g[c].position = new Point(-far, -far, 0);
			CLSVertex d = g.add_vertex(); // VertexProps( Point(far,-far,0) ), g);
			g[c].position = new Point(far, -far, 0);
			CLSFace f_outer = g.add_face();
			CLSFace f_inner = g.add_face();
			CLSEdge e1 = g.add_edge(a, b);
			CLSEdge e1t = g.add_edge(b, a);
			CLSEdge e2 = g.add_edge(b, c);
			CLSEdge e2t = g.add_edge(c, b);
			CLSEdge e3 = g.add_edge(c, d);
			CLSEdge e3t = g.add_edge(d, c);
			CLSEdge e4 = g.add_edge(d, a);
			CLSEdge e4t = g.add_edge(a, d);

			g[f_inner].edge = e1;
			g[f_outer].edge = e1t;
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: out_face = f_outer;
			out_face.CopyFrom(f_outer);
			// twin edges
			g.twin_edges(e1,e1t);
			g.twin_edges(e2,e2t);
			g.twin_edges(e3,e3t);
			g.twin_edges(e4,e4t);
			// inner face:
			g[e1].face = f_inner;
			g[e2].face = f_inner;
			g[e3].face = f_inner;
			g[e4].face = f_inner;
			// outer face:
			g[e1t].face = f_outer;
			g[e2t].face = f_outer;
			g[e3t].face = f_outer;
			g[e4t].face = f_outer;
			// next-pointers:
			g[e1].next = e2;
			g[e2].next = e3;
			g[e3].next = e4;
			g[e4].next = e1;
			// outer next-pointers:
			g[e1t].next = e4t;
			g[e4t].next = e3t;
			g[e3t].next = e2t;
			g[e2t].next = e1t;

			subdivide();

		}

		public void subdivide()
		{
			for (CLSFace f = 0; f < g.num_faces() ; ++f)
			{
				// subdivide each face
				if (f != out_face)
				{
					subdivide_face(new CLSFace(f));
				}
			}
		}

		public void subdivide_face(CLSFace f)
		{
			CLSEdgeVector f_edges = g.face_edges(f);
			Debug.Assert(f_edges.size() == 4);
			CLSVertex center = g.add_vertex();
			foreach (CLSEdge e in f_edges)
			{
				CLSVertex src = g.source(e);
				CLSVertex trg = g.target(e);
				// new vertex at mid-point of each edge
				Point mid = 0.5 * (g[src].position + g[trg].position);
				g[center].position += 0.25 * g[src].position; // average of four corners
				CLSVertex v = g.add_vertex();
				g[v].position = mid;
				g.insert_vertex_in_edge(v,e); // this also removes the old edges...
			}
			// now loop through edges again:
			f_edges = g.face_edges(f);
			Debug.Assert(f_edges.size() == 8);
			foreach (CLSEdge e in f_edges)
			{
				Console.Write(e);
				Console.Write("\n");
			}
		}

		public virtual void run()
		{
		}
		public void setMinSampling(double s)
		{
			min_sampling = s;
		}

	// PYTHON
		public boost::python.list getVertices()
		{
			boost::python.list plist = new boost::python.list();
			foreach (CLSVertex v in g.vertices())
			{
				plist.append(g[v].position);
			}
			return new boost::python.list(plist);
		}

		public boost::python.list getEdges()
		{
			boost::python.list edge_list = new boost::python.list();
			foreach (CLSEdge edge in g.edges())
			{ // loop through each edge
					boost::python.list point_list = new boost::python.list(); // the endpoints of each edge
					CLSVertex v1 = g.source(edge);
					CLSVertex v2 = g.target(edge);
					point_list.append(g[v1].position);
					point_list.append(g[v2].position);
					edge_list.append(point_list);
			}
			return new boost::python.list(edge_list);
		}
        /*
		/// string repr
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const
		public string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << "CutterLocationSurface (nVerts=" << g.num_vertices() << " , nEdges=" << g.num_edges() << "\n";
			return o.str();
		}
        */

	// DATA
		/// the half-edge diagram 
		protected CLSGraph g = new CLSGraph();
		protected double min_sampling;
		protected double far;
		protected CLSFace out_face = new CLSFace();
}


} // end clsurf namespace

} // end ocl namespace
// end clsurface.h
