using System;
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
//#include <boost/graph/adjacency_matrix.hpp>
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <boost/graph/simple_point.hpp>
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <boost/graph/metric_tsp_approx.hpp>

//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <boost/python.hpp>

namespace ocl
{

namespace tsp
{

// loosely based on metric tsp example:
// http://www.boost.org/doc/libs/1_46_1/libs/graph/test/metric_tsp_approx.cpp

//add edges to the graph (for each node connect it to all other nodes)
//C++ TO C# CONVERTER TODO TASK: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template< typename VertexListGraph, typename PointContainer, typename WeightMap, typename VertexIndexMap>

public class TSPSolver : System.IDisposable
{

	public TSPSolver()
	{
	}
	public virtual void Dispose()
	{
		if (g)
		{
			if (g != null)
			{
				g.Dispose();
			}
		}
	}
	public void run()
	{
		g = new boost::adjacency_matrix< boost::undirectedS, boost::no_property, boost::property< boost::edge_weight_t, double, boost::property< boost::edge_index_t, uint>>, boost::no_property >(points.Count);
		// connect all vertices
		boost::property_map<boost::adjacency_matrix< boost::undirectedS, boost::no_property, boost::property< boost::edge_weight_t, double, boost::property< boost::edge_index_t, uint>>, boost::no_property >, boost::edge_weight_t>.type weight_map = boost::get(boost::edge_weight, g[0]);
		tsp.GlobalMembers.connectAllEuclidean(g[0], points, new boost::property_map<boost::adjacency_matrix< boost::undirectedS, boost::no_property, boost::property< boost::edge_weight_t, double, boost::property< boost::edge_index_t, uint>>, boost::no_property >, boost::edge_weight_t>.type(weight_map), boost::get(boost::vertex_index, g[0]));
		length = 0.0;
		// Run the TSP approx, creating the visitor on the fly.
		boost::metric_tsp_approx(g[0], boost::make_tsp_tour_len_visitor(g[0], std::back_inserter(output), length, weight_map));
		//length = len;
		//std::cout << "Number of points: " << boost::num_vertices(*g) << std::endl;
		//std::cout << "Number of edges: " << boost::num_edges(*g) << std::endl;
		//std::cout << "Length of tour: " << len << std::endl;
		//std::cout << "vertices in tour: " << output.size() << std::endl;
		//std::cout << "Elapsed: " << t.elapsed() << std::endl;
	}
	public void addPoint(double x, double y)
	{
		boost::simple_point<double> pnt = new boost::simple_point<double>();
		pnt.x = x;
		pnt.y = y;
		points.Add(pnt);
	}
	public void reset()
	{
		output.Clear();
		points.Clear();
		if (g)
		{
			g = null;
		}
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: void printOutput() const
	public void printOutput()
	{
		int n = 0;
		foreach (boost  in :graph_traits<boost::adjacency_matrix< boost::undirectedS, boost::no_property, boost::property< boost::edge_weight_t, double, boost::property< boost::edge_index_t, uint>>, boost::no_property >>.vertex_descriptor v : output)
		{
			Console.Write(n++);
			Console.Write(" : ");
			Console.Write(v);
			Console.Write("\n");
		}
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double getLength() const
	public double getLength()
	{
		return length;
	}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: boost::python::list getOutput() const
	public boost::python.list getOutput()
	{
		boost::python.list plist = new boost::python.list();
		foreach (boost  in :graph_traits<boost::adjacency_matrix< boost::undirectedS, boost::no_property, boost::property< boost::edge_weight_t, double, boost::property< boost::edge_index_t, uint>>, boost::no_property >>.vertex_descriptor v : output)
		{
			plist.append(v);
		}
		return new boost::python.list(plist);
	}

	protected List< boost::graph_traits<boost::adjacency_matrix< boost::undirectedS, boost::no_property, boost::property< boost::edge_weight_t, double, boost::property< boost::edge_index_t, uint>>, boost::no_property >>.vertex_descriptor > output = new List< boost::graph_traits<boost::adjacency_matrix< boost::undirectedS, boost::no_property, boost::property< boost::edge_weight_t, double, boost::property< boost::edge_index_t, uint>>, boost::no_property >>.vertex_descriptor >();
	protected List< boost::simple_point<double>> points = new List< boost::simple_point<double>>();
	protected boost::adjacency_matrix< boost::undirectedS, boost::no_property, boost::property< boost::edge_weight_t, double, boost::property< boost::edge_index_t, uint>>, boost::no_property >[] g;
	protected double length;
}


} // end tsp namespace

} // end ocl namespace
