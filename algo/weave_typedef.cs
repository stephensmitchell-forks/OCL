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


namespace ocl
{

namespace weave
{


// we use the traits-class here so that EdgeProps can have Edge as a member

/// vertex type: CL-point, internal point, adjacent point
public enum VertexType
{
	CL,
	CL_DONE,
	ADJ,
	TWOADJ,
	int,
	FULLINT
}

/// vertex properties
public class VertexProps
{
	public VertexProps()
	{
		init();
	}
	/// construct vertex at position p with type t
	public VertexProps(Point p, VertexType t)
	{
		position.CopyFrom(p);
		type = t;
		init();
	}
	/// construct vertex at position p with type t
	public VertexProps(Point p, VertexType t, List<Interval>.Enumerator x, List<Interval>.Enumerator y)
	{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: this.xi = x;
		this.xi.CopyFrom(x);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: this.yi = y;
		this.yi.CopyFrom(y);
		position.CopyFrom(p);
		type = t;
		init();
	}

	public void init()
	{
		index = count;
		count++;
	}
	public VertexType type;
// HE data
	/// the position of the vertex
	public Point position = new Point();
	/// index of vertex
	public int index;
	/// global vertex count
	public static int count;

	// x interval
	public List<Interval>.Enumerator xi;
	// y interval
	public List<Interval>.Enumerator yi;

}

/// edge properties
public class EdgeProps
{
	public EdgeProps()
	{
	}
	/// the next edge, counterclockwise, from this edge
	public Edge next = new Edge();
	/// previous edge, to make Weave::build() faster, since we avoid calling hedi::previous_edge() 
	public Edge prev = new Edge();
	/// the twin edge
	public Edge twin = new Edge();
}


/// properties of a face in the weave
public class FaceProps
{
	/// create face with given edge, generator, and type
	public FaceProps(Edge e)
	{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: edge = e;
		edge.CopyFrom(e);
	}
	/// face index
	public Face idx = new Face();
	/// one edge that bounds this face
	public Edge edge = new Edge();
}



// the graph type for the weave


//typedef boost::graph_traits< WeaveGraph >::vertex_descriptor  Vertex;
//
// typedef boost::graph_traits< WeaveGraph >::edge_descriptor    Edge;
//typedef boost::graph_traits< WeaveGraph >::edge_iterator      EdgeItr;
//typedef boost::graph_traits< WeaveGraph >::out_edge_iterator  OutEdgeItr;
//typedef boost::graph_traits< WeaveGraph >::adjacency_iterator AdjacencyItr;

/// intersections between intervals are stored as a VertexPair
/// pair.first is a vertex descriptor of the weave graph
/// pair.second is the coordinate along the fiber of the intersection

/// compare based on pair.second, the coordinate of the intersection
public class VertexPairCompare
{
	/// comparison operator
//C++ TO C# CONVERTER TODO TASK: The typedef 'VertexPair' was defined in multiple preprocessor conditionals and cannot be replaced in-line:
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool operator ()(const VertexPair& lhs, const VertexPair& rhs) const
	public static bool functorMethod(VertexPair lhs, VertexPair rhs)
	{
		return lhs.second < rhs.second;
	}
}

/// intersections stored in this set (for rapid finding of neighbors etc)


} // end weave namespace

} // end ocl namespace
// end file weave_typedef.hpp
