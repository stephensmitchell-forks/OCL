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
//#include <boost/graph/adjacency_list.hpp>
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <boost/foreach.hpp>

// bundled BGL properties, see: http://www.boost.org/doc/libs/1_44_0/libs/graph/doc/bundles.html

// dcel notes from http://www.holmes3d.net/graphics/dcel/

// vertex (boost::out_edges)
//  -leaving pointer to HalfEdge that has this vertex as origin
//   if many HalfEdges have this vertex as origin, choose one arbitrarily

// HalfEdge
//  - origin pointer to vertex (boost::source)
//  - face to the left of halfedge
//  - twin pointer to HalfEdge (on the right of this edge)
//  - next pointer to HalfEdge
//     this edge starts from h->twin->origin and ends at next vertex in h->face
//     traveling ccw around boundary
//     (allows face traverse, follow h->next until we arrive back at h)

// Face
//  - edge pointer to HalfEdge
//    this edge has this Face object as face
//    half-edge can be any one on the boundary of face
// special "infinite face", face on "outside" of boundary
// may or may not store edge pointer


/// HEDIGraph is a A half-edge diagram class.
/// Templated on Vertex/Edge/Face property classes which allow
/// attaching information to vertices/edges/faces that is 
/// required for a particular algorithm.
/// 
/// Inherits from boost::adjacency_list
/// minor additions allow storing face-properties.
///
/// the hedi namespace contains functions for manipulating HEDIGraphs
///
/// For a general description of the half-edge data structure see e.g.:
///  - http://www.holmes3d.net/graphics/dcel/
///  - http://openmesh.org/index.php?id=228

namespace ocl
{

namespace hedi
{

//C++ TO C# CONVERTER TODO TASK: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template <class TOutEdgeList, class TVertexList, class TDirected, class TVertexProperties, class TEdgeProperties, class TFaceProperties, class TGraphProperties, class TEdgeList >
public class HEDIGraph <TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TFaceProperties, TGraphProperties, TEdgeList >
{



		public TFaceProperties this[uint f]
		{
			get
			{
				return faces[f];
			}
			set
			{
				faces[f] = value;
			}
		}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: inline const TFaceProperties& operator [](uint f) const
		public TFaceProperties this[uint f]
		{
			get
			{
				return faces[f];
			}
			set
			{
				faces[f] = value;
			}
		}

		public TEdgeProperties this[boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e]
		{
			get
			{
				return g[e];
			}
			set
			{
				g[e] = value;
			}
		}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: inline const TEdgeProperties& operator [](typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>::edge_descriptor e) const
		public TEdgeProperties this[boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e]
		{
			get
			{
				return g[e];
			}
			set
			{
				g[e] = value;
			}
		}

		public TVertexProperties this[boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v]
		{
			get
			{
				return g[v];
			}
			set
			{
				g[v] = value;
			}
		}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: inline const TVertexProperties& operator [](typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>::vertex_descriptor v) const
		public TVertexProperties this[boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v]
		{
			get
			{
				return g[v];
			}
			set
			{
				g[v] = value;
			}
		}

//DATA
		public List< TFaceProperties > faces = new List< TFaceProperties >();
		public boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList > g = new boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >();

public boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor null_vertex()
{
	return boost::graph_traits<typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.null_vertex();
}

/// add a blank vertex and return its descriptor
public boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor add_vertex()
{
	return boost::add_vertex(g);
}

/*
/// add a vertex with given properties, return vertex descriptor
template < class FVertexProperty>
typename boost::graph_traits< BGLGraph >::vertex_descriptor add_vertex(typedef const FVertexProperty& prop) {
    return boost::add_vertex( prop, g );
}*/

/// add an edge between vertices v1-v2
public boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor add_edge(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v1, boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v2)
{
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e = new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor();
	bool b;
	boost::tie(e, b) = boost::add_edge(v1, v2, g);
	return new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor(e);
}

/// add an edge with given properties between vertices v1-v2
//template < class EdgeProperty>
/*
typename boost::graph_traits< BGLGraph >::edge_descriptor add_edge( 
                                                       typename boost::graph_traits< BGLGraph >::vertex_descriptor v1, 
                                                       typename boost::graph_traits< BGLGraph >::vertex_descriptor v2, 
                                                       typename  TEdgeProperties prop
                                                       ) {
    typename boost::graph_traits< BGLGraph >::edge_descriptor e;
    bool b;
    boost::tie( e , b ) = boost::add_edge( v1, v2, prop, g);
    return e;
}*/

/// make e1 the twin of e2 (and vice versa)
public void twin_edges(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e1, boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e2)
{
	g[e1].twin = e2;
	g[e2].twin = e1;
}


/// add a face 
public uint add_face()
{
	TFaceProperties f_prop = new default(TFaceProperties);
	faces.Add(f_prop);
	uint index = (uint)(faces.Count - 1);
	faces[index].idx = index;
	return index;
}


/// return the target vertex of the given edge
public boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor target(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e)
{
	return boost::target(e, g);
}

/// return the source vertex of the given edge
public boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor source(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e)
{
	return boost::source(e, g);
}

/// return all vertices in a vector of vertex descriptors
public List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor> vertices()
{
	List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor> vv = new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor>();
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_iterator it_begin = new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_iterator();
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_iterator it_end = new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_iterator();
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_iterator itr = new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_iterator();
	boost::tie(it_begin, it_end) = boost::vertices(g);
	for (itr = it_begin ; itr != it_end ; ++itr)
	{
		vv.Add(*itr);
	}
	return new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor>(vv);
}

/// return all vertices adjecent to given vertex
public List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor> adjacent_vertices(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v)
{
	List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor> vv = new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor>();
	foreach (boost  in :graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor edge : out_edges(new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>(v)))
	{
		vv.Add(target(edge));
	}
	return new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor>(vv);
}

/// return all vertices of given face

public List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor> face_vertices(uint face_idx)
{
	List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor> verts = new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor>();
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor startedge = faces[face_idx].edge; // the edge where we start
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor start_target = boost::target(startedge, g);
	verts.Add(start_target);
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor current = g[startedge].next;
	do
	{
		boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor current_target = boost::target(current, g);
		verts.Add(current_target);
		current = g[current].next;
	} while (current != startedge);
	return new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor>(verts);
}

/// return edges of face f
public List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor> face_edges(uint f)
{
	//Edge start_edge = g[ f ].edge; // was cast: (Face)f
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor start_edge = faces[f].edge;
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor current_edge = start_edge;
	List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor> @out = new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor>();
	do
	{
		@out.Add(current_edge);
		current_edge = g[current_edge].next;
	} while (current_edge != start_edge);
	return new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor>(@out);
}


/// return degree of given vertex
public uint degree(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v)
{
	return boost::degree(v, g);
}

/// return number of vertices in graph
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint num_vertices() const
public uint num_vertices()
{
	return boost::num_vertices(g);
}

/// return out_edges of given vertex
public List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor> out_edges(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v)
{
	List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor> ev = new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor>();
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.out_edge_iterator it = new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.out_edge_iterator();
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.out_edge_iterator it_end = new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.out_edge_iterator();
	boost::tie(it, it_end) = boost::out_edges(v, g);
	for (; it != it_end ; ++it)
	{
		ev.Add(*it);
	}
	return new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor>(ev);
}

/// return all edges
public List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor> edges()
{
	List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor> ev = new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor>();
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_iterator it = new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_iterator();
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_iterator it_end = new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_iterator();
	boost::tie(it, it_end) = boost::edges(g);
	for (; it != it_end ; ++it)
	{
		ev.Add(*it);
	}
	return new List<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor>(ev);
}

/// return v1-v2 edge descriptor
public boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor edge(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v1, boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v2)
{
	Tuple<typename boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor, bool> result = boost::edge(v1, v2, g);
	return result.Item1;
}

/// return the previous edge. traverses all edges in face until previous found.
public boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor previous_edge(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e)
{
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor previous = g[e].next;
	while (g[previous].next != e)
	{
		previous = g[previous].next;
	}
	return new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor(previous);
}


/// return true if v1-v2 edge exists
public bool has_edge(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v1, boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v2)
{
//C++ TO C# CONVERTER TODO TASK: The typedef 'EdgeBool' was defined in multiple preprocessor conditionals and cannot be replaced in-line:
	EdgeBool result = boost::edge(v1, v2, g);
	return result.second;
}

/// return adjacent faces to the given vertex
public List<uint> adjacent_faces(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor q)
{
	SortedSet<uint> face_set = new SortedSet<uint>();
//C++ TO C# CONVERTER TODO TASK: The typedef 'HEOutEdgeItr' was defined in multiple preprocessor conditionals and cannot be replaced in-line:
	HEOutEdgeItr itr = new HEOutEdgeItr();
	HEOutEdgeItr itr_end = new HEOutEdgeItr();
	boost::tie(itr, itr_end) = boost::out_edges(q, g);
	for (; itr != itr_end ; ++itr)
	{
		face_set.Add(g[*itr].face);
	}
	List<uint> fv = new List<uint>();
	foreach (uint m in face_set)
	{
		fv.Add(m);
	}
	return new List<uint>(fv);
}

/// return number of faces in graph
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint num_faces() const
public uint num_faces()
{
	return (uint)faces.Count;
}

/// return number of edges in graph
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint num_edges() const
public uint num_edges()
{
	return boost::num_edges(g);
}

/// inserts given vertex into edge e, and into the twin edge e_twin
public void insert_vertex_in_edge(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v, boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e)
{
	// the vertex v is in the middle of edge e
	//                    face
	//                    e1   e2
	// previous-> source  -> v -> target -> next
	//            tw_trg  <- v <- tw_src <- tw_previous
	//                    te2  te1
	//                    twin_face

	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor twin = g[e].twin;
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor source = boost::source(e, g);
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor target = boost::target(e, g);
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor twin_source = boost::source(twin, g);
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor twin_target = boost::target(twin, g);
	Debug.Assert(source == twin_target);
	Debug.Assert(target == twin_source);

	uint face = g[e].face;
	uint twin_face = g[twin].face;
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor previous = previous_edge(new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>(e));
	Debug.Assert(g[previous].face == g[e].face);
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor twin_previous = previous_edge(new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor(twin));
	Debug.Assert(g[twin_previous].face == g[twin].face);

	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e1 = add_edge(new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor(source), new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>(v));
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e2 = add_edge(new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>(v), new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor(target));

	// preserve the left/right face link
	g[e1].face = face;
	g[e2].face = face;
	// next-pointers
	g[previous].next = e1;
	g[e1].next = e2;
	g[e2].next = g[e].next;


	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor te1 = add_edge(new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor(twin_source), new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>(v));
	boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor te2 = add_edge(new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>(v), new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor(twin_target));

	g[te1].face = twin_face;
	g[te2].face = twin_face;

	g[twin_previous].next = te1;
	g[te1].next = te2;
	g[te2].next = g[twin].next;

	// TWINNING (note indices 'cross', see ASCII art above)
	g[e1].twin = te2;
	g[te2].twin = e1;
	g[e2].twin = te1;
	g[te1].twin = e2;

	// update the faces (required here?)
	faces[face].edge = e1;
	faces[twin_face].edge = te1;

	// finally, remove the old edge
	boost::remove_edge(e, g);
	boost::remove_edge(twin, g);
}


/// inserts given vertex into edge e
/*
template <class BGLGraph>
void insert_vertex_in_half_edge(typename boost::graph_traits< BGLGraph >::vertex_descriptor  v, 
                           typename boost::graph_traits< BGLGraph >::edge_descriptor e, 
                           BGLGraph& g) {
    typedef typename boost::graph_traits< BGLGraph >::edge_descriptor    HEEdge;
    typedef typename boost::graph_traits< BGLGraph >::vertex_descriptor  HEVertex;
    // the vertex v is in the middle of edge e
    //                    face
    //                    e1   e2
    // previous-> source  -> v -> target -> next
    
    HEVertex source = boost::source( e , g );
    HEVertex target = boost::target( e , g);
    unsigned int face = g[e].face;
    HEEdge previous = previous_edge(e, g);
    assert( g[previous].face == g[e].face );
    HEEdge e1 = add_edge( source, v , g);
    HEEdge e2 = add_edge( v, target , g);
    // preserve the left/right face link
    g[e1].face = face;
    g[e2].face = face;
    // next-pointers
    g[previous].next = e1;
    g[e1].next = e2;
    g[e2].next = g[e].next;
    // update the faces (required here?)
    g.faces[face].edge = e1;
    // finally, remove the old edge
    boost::remove_edge( e   , g);
    // NOTE: twinning is not done here, since the twin edge is not split...
}

*/
		/// check that all edges belong to the correct face, TODO: template this to make it a useful check
		/*
		bool checkFaces() {
		    BOOST_FOREACH(FaceProps f, faces) {
		        BOOST_FOREACH( HEEdge e, face_edges(f.idx)) {
		            if ( g[e].face != f.idx )
		                return false;
		        }
		    }
		    return true;
		}*/

/// delete a vertex
public void delete_vertex(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v)
{
	clear_vertex(new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>(v));
	remove_vertex(new boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>(v));
}

/// clear given vertex. this removes all edges connecting to the vertex.
public void clear_vertex(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v)
{
	boost::clear_vertex(v, g);
}
/// remove given vertex
public void remove_vertex(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v)
{
	boost::remove_vertex(v, g);
}

public void remove_edge(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v1, boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.vertex_descriptor v2)
{
	boost::remove_edge(v1, v2, g);
}

public void remove_edge(boost::graph_traits< typename boost::adjacency_list< TOutEdgeList, TVertexList, TDirected, TVertexProperties, TEdgeProperties, TGraphProperties, TEdgeList >>.edge_descriptor e)
{
	boost::remove_edge(e, g);
}

} // end class definition


} // end hedi namespace

} // end ocl namespace
// end halfedgediagram.hpp
