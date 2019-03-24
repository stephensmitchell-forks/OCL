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

public class SmartWeave : Weave
{
		public SmartWeave()
		{
		}
		public override void Dispose()
		{
			base.Dispose();
		}

		// this is the new smarter build() which uses less RAM
		public override void build()
		{
			Console.Write(" SimpleWeave::build()... \n");

			// this adds all CL-vertices from x-intervals
			// it also populates the xi.intersections_fibers set of intersecting y-fibers
			// also add the first-crossing vertex and the last-crossing vertex

			//std::cout << " build2() add_vertices_x() ... " << std::flush ;
			add_vertices_x();
			//std::cout << " done.\n" << std::flush ;
			// the same for y-intervals, add all CL-points, and intersections to the set.
			//std::cout << " build2() add_vertices_y() ... " << std::flush ;
			add_vertices_y();
			//std::cout << " done.\n" << std::flush ;

			//std::cout << " build2() looping over xfibers ... " << std::flush ;
			foreach (Fiber xf in xfibers)
			{
				List<Interval>.Enumerator xi;
				for (xi = xf.ints.GetEnumerator(); xi < xf.ints.end();)
				{
					SortedSet<List<Fiber>.Enumerator>.Enumerator current;
					SortedSet<List<Fiber>.Enumerator>.Enumerator prev;
					if (xi.intersections_fibers.size() > 1)
					{
						current = xi.intersections_fibers.begin();
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: prev = current++;
						prev.CopyFrom(current++);
						while (current.MoveNext())
						{
							// for each x-interval, loop through the intersecting y-fibers
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							if ((current.Current - prev) > 1)
							{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								List<Interval>.Enumerator yi = find_interval_crossing_x(xf, (prev + 1));
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								add_vertex(xf, (prev + 1), new List<Interval>.Enumerator(xi), new List<Interval>.Enumerator(yi), VertexType.FULLINT);
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								if ((current.Current - prev) > 2)
								{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: yi = find_interval_crossing_x(xf, *(current.Current - 1));
									yi.CopyFrom(find_interval_crossing_x(xf, (current.Current - 1)));
									add_vertex(xf, (current.Current - 1), new List<Interval>.Enumerator(xi), new List<Interval>.Enumerator(yi), VertexType.FULLINT);
								}
							}
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: prev = current;
							prev.CopyFrom(current);
						}
					}
				}
			}
			//std::cout << " done.\n" << std::flush ;

			//std::cout << " build2() looping over yfibers ... " << std::flush ;
			//std::cout << yfibers.size() << " fibers to loop over \n"<< std::flush ;
			//int ny = 0;
			foreach (Fiber yf in yfibers)
			{
				//std::cout << " fiber nr: " << ny++ << " has " << yf.ints.size() << " intervals\n" << std::flush ;
				List<Interval>.Enumerator yi;
				//int ny_int=0;
				for (yi = yf.ints.GetEnumerator(); yi < yf.ints.end();)
				{
					//std::cout << "  interval nr: " << ny_int++ << " has yi->intersections_fibers.size()= " << yi->intersections_fibers.size() << "\n" << std::flush;
					SortedSet<List<Fiber>.Enumerator>.Enumerator current;
					SortedSet<List<Fiber>.Enumerator>.Enumerator prev;
					if (yi.intersections_fibers.size() > 1)
					{
						current = yi.intersections_fibers.begin();
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: prev = current++;
						prev.CopyFrom(current++);
						while (current.MoveNext())
						{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							if ((current.Current - prev) > 1)
							{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								List<Interval>.Enumerator xi = find_interval_crossing_y((prev + 1), yf);
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								add_vertex((prev + 1), yf, new List<Interval>.Enumerator(xi), new List<Interval>.Enumerator(yi), VertexType.FULLINT);
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								if ((current.Current - prev) > 2)
								{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: xi = find_interval_crossing_y(*(current.Current - 1), yf);
									xi.CopyFrom(find_interval_crossing_y((current.Current - 1), yf));
									add_vertex((current.Current - 1), yf, new List<Interval>.Enumerator(xi), new List<Interval>.Enumerator(yi), VertexType.FULLINT);
								}
							}
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: prev = current;
							prev.CopyFrom(current);
						}
					}
				}
			}
			//std::cout << " done.\n" << std::flush ;

//C++ TO C# CONVERTER TODO TASK: The cout 'flush' manipulator is not converted by C++ to C# Converter:
//ORIGINAL LINE: std::cout << " SmartWeave::build() add_all_edges()... " << std::flush;
			Console.Write(" SmartWeave::build() add_all_edges()... ");
			add_all_edges();
//C++ TO C# CONVERTER TODO TASK: The cout 'flush' manipulator is not converted by C++ to C# Converter:
//ORIGINAL LINE: std::cout << " done.\n" << std::flush;
			Console.Write(" done.\n");
		}

		protected void add_vertices_x()
		{
			List<Fiber>.Enumerator xf;
			for (xf = xfibers.GetEnumerator(); xf < xfibers.end();)
			{
				List<Interval>.Enumerator xi;
				for (xi = xf.ints.begin(); xi < xf.ints.end();)
				{
					// find first and last fiber crossing this interval
					List<Fiber>.Enumerator yf = yfibers.GetEnumerator();
					List<Interval>.Enumerator yi;

//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
					bool is_crossing = crossing_x(yf, ref yi, xi.Current, xf.Current);
					while ((yf < yfibers.end()) && !is_crossing)
					{ // first crossing
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						yf++;
						if (yf < yfibers.end())
						{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							is_crossing = crossing_x(yf, ref yi, xi.Current, xf.Current);
						}
					}

					if (yf < yfibers.end())
					{
						Point lower = new Point(xf.point(xi.lower));
						add_cl_vertex(lower, xi.Current, lower.x);
						Point upper = new Point(xf.point(xi.upper));
						add_cl_vertex(upper, xi.Current, upper.x);

//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						add_vertex(xf.Current, yf, new List<Interval>.Enumerator(xi), new List<Interval>.Enumerator(yi), VertexType.INTEGER); // the first crossing vertex
						xi.intersections_fibers.insert(yf);
						yi.intersections_fibers.insert(xf);

//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						is_crossing = crossing_x(yf, ref yi, xi.Current, xf.Current);
						while ((yf < yfibers.end()) && is_crossing)
						{ // last crossing
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							yf++;
							if (yf < yfibers.end())
							{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								is_crossing = crossing_x(yf, ref yi, xi.Current, xf.Current);
							}
						}
						add_vertex(xf.Current, (--yf), new List<Interval>.Enumerator(xi), new List<Interval>.Enumerator(yi), VertexType.INTEGER); // the last crossing vertex
						xi.intersections_fibers.insert(yf);
						yi.intersections_fibers.insert(xf);
					}
				} // end foreach x-interval
			} // end foreach x-fiber
		}

		protected void add_vertices_y()
		{
			List<Fiber>.Enumerator yf;
			for (yf = yfibers.GetEnumerator(); yf < yfibers.end();)
			{
				List<Interval>.Enumerator yi;
				for (yi = yf.ints.begin(); yi < yf.ints.end();)
				{
					List<Fiber>.Enumerator xf = xfibers.GetEnumerator();
					List<Interval>.Enumerator xi;

					// find first and last fiber crossing this interval
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
					bool is_crossing = crossing_y(xf, ref xi, yi.Current, yf.Current);
					while ((xf < xfibers.end()) && !is_crossing)
					{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						xf++;
						if (xf < xfibers.end())
						{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							is_crossing = crossing_y(xf, ref xi, yi.Current, yf.Current);
						}
					}

					if (xf < xfibers.end())
					{
						Point lower = new Point(yf.point(yi.lower));
						add_cl_vertex(lower, yi.Current, lower.y);
						Point upper = new Point(yf.point(yi.upper));
						add_cl_vertex(upper, yi.Current, upper.y);

//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						if (add_vertex(xf, yf.Current, new List<Interval>.Enumerator(xi), new List<Interval>.Enumerator(yi), VertexType.INTEGER))
						{ // add_vertex returns false if vertex already exists
							xi.intersections_fibers.insert(yf);
							yi.intersections_fibers.insert(xf);
						}

//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						bool is_crossing = crossing_y(xf, ref xi, yi.Current, yf.Current);
						while ((xf < xfibers.end()) && is_crossing)
						{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							xf++;
							if (xf < xfibers.end())
							{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
								is_crossing = crossing_y(xf, ref xi, yi.Current, yf.Current);
							}
						}
						if (add_vertex((--xf), yf.Current, new List<Interval>.Enumerator(xi), new List<Interval>.Enumerator(yi), VertexType.INTEGER))
						{
							xi.intersections_fibers.insert(yf);
							yi.intersections_fibers.insert(xf);
						}
					}
				} // end foreach x-interval
			} // end foreach x-fiber
		}


		//crossing_x fiber
		protected bool crossing_x(Fiber yf, ref List<Interval>.Enumerator yi, Interval xi, Fiber xf)
		{
			//if the FIBER crosses the xi interval
			if ((yf.p1.x >= xf.point(xi.lower).x) && (yf.p1.x <= xf.point(xi.upper).x))
			{
				//for all the intervals of this y-fiber...
				List<Interval>.Enumerator it;
				for (it = yf.ints.GetEnumerator(); it < yf.ints.end();)
				{
					//find the first INTERVAL which crosses our xi interval
					if ((yf.point(it.lower).y <= xf.p1.y) && (yf.point(it.upper).y >= xf.p1.y))
					{
						//save the y-interval iterator
						//and return true because we found an interval
						yi = it;
						return true;
					}
				}
				//return false, there is no y-interval on this y-fiber crossing our xi interval
				return false;
			}
			//this y-fiber doesn't cross our xi interval
			else
			{
				return false;
			}
		}


		//crossing_y fiber
		protected bool crossing_y(Fiber xf, ref List<Interval>.Enumerator xi, Interval yi, Fiber yf)
		{
			if ((xf.p1.y >= yf.point(yi.lower).y) && (xf.p1.y <= yf.point(yi.upper).y))
			{
				List<Interval>.Enumerator it;
				for (it = xf.ints.GetEnumerator(); it < xf.ints.end();)
				{
					if ((xf.point(it.lower).x <= yf.p1.x) && (xf.point(it.upper).x >= yf.p1.x))
					{
						xi = it;
						return true;
					}
				}
				return false;
			}
			else
			{
				return false;
			}
		}


		//find_interval_crossing_x
		protected List<Interval>.Enumerator find_interval_crossing_x(Fiber xf, Fiber yf)
		{
			List<Interval>.Enumerator yi;
			List<Interval>.Enumerator xi;
			yi = yf.ints.GetEnumerator();
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
			while ((yi < yf.ints.end()) && !crossing_y(xf, ref xi, yi, yf))
			{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
				yi++;
			}
			return new List<Interval>.Enumerator(yi);
		}


		//crossing_y interval
		protected List<Interval>.Enumerator find_interval_crossing_y(Fiber xf, Fiber yf)
		{
			List<Interval>.Enumerator xi;
			List<Interval>.Enumerator yi;
			xi = xf.ints.GetEnumerator();
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
			while ((xi < xf.ints.end()) && !crossing_x(yf, ref yi, xi, xf))
			{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
				xi++;
			}
			return new List<Interval>.Enumerator(xi);
		}


		// add a new CL-vertex to Weave, also adding it to the interval intersection-set, and to clVertices
		protected Vertex add_cl_vertex(Point position, Interval ival, double ipos)
		{
			Vertex v = g.add_vertex();
			g[v].position = position;
			g[v].type = VertexType.CL;
			ival.intersections2.Add(VertexPair(v, ipos)); // ?? this makes Interval depend on the WeaveGraph type
			clVertexSet.Add(v);
			return new Vertex(v);
		}


		//add_vertex
		protected bool add_vertex(Fiber xf, Fiber yf, List<Interval>.Enumerator xi, List<Interval>.Enumerator yi, VertexType type)
		{
			//test if vertex exists
			foreach (List<Fiber>.Enumerator it_xf in yi.intersections_fibers)
			{
				if (it_xf == xf)
				{
					return false;
				}
			}
			Point v_position = new Point(yf.p1.x, xf.p1.y, xf.p1.z);
			Vertex v = g.add_vertex();
			g[v].position = v_position;
			g[v].type = type;
			g[v].xi = xi;
			g[v].yi = yi;
			xi.intersections2.insert(VertexPair(v, v_position.x));
			yi.intersections2.insert(VertexPair(v, v_position.y));
			return true;
		}


		//add_all_edges
		protected void add_all_edges()
		{
			List<Vertex> vertices = g.vertices();

			Console.Write("There are ");
			Console.Write(vertices.Count);
			Console.Write(" vertices.\n");
			foreach (Vertex vertex in vertices)
			{
				if ((g[vertex].type == VertexType.INTEGER) || (g[vertex].type == VertexType.FULLINT))
				{
					List<Vertex> adjacent_vertices = new List<Vertex>();
					List<Vertex>.Enumerator adj_itr;
					List<Edge> in_edges = new List<Edge>();
					List<Edge> out_edges = new List<Edge>();
					List<Edge>.Enumerator in_edge_itr;
					List<Edge>.Enumerator out_edge_itr;

					Vertex x_u = new Vertex();
					Vertex x_l = new Vertex();
					Vertex y_u = new Vertex();
					Vertex y_l = new Vertex();
					boost::tie(x_u, x_l) = find_neighbor_vertices(VertexPair(vertex, g[vertex].position.x), (g[vertex].xi), false);
					boost::tie(y_u, y_l) = find_neighbor_vertices(VertexPair(vertex, g[vertex].position.y), (g[vertex].yi), false);

					adjacent_vertices.Add(x_l);
					adjacent_vertices.Add(y_u);
					adjacent_vertices.Add(x_u);
					adjacent_vertices.Add(y_l);

					for (adj_itr = adjacent_vertices.GetEnumerator(); adj_itr < adjacent_vertices.end();)
					{
						Edge @in = new Edge();
						Edge @out = new Edge();
						if (g.has_edge(adj_itr.Current, vertex))
						{
							@in = g.edge(adj_itr.Current, vertex);
							@out = g.edge(vertex, adj_itr.Current);
							in_edges.Add(@in);
							out_edges.Add(@out);
						}
						else
						{
							@in = g.add_edge(adj_itr.Current, vertex);
							@out = g.add_edge(vertex, adj_itr.Current);
							in_edges.Add(@in);
							out_edges.Add(@out);
						}

						if (g[adj_itr.Current].type == VertexType.CL)
						{
							g[@in].prev = @out;
							g[@out].next = @in;
						}
					}

					for (in_edge_itr = in_edges.GetEnumerator(), out_edge_itr = out_edges.GetEnumerator(); in_edge_itr < in_edges.end(), out_edge_itr < out_edges.end(); in_edge_itr++, out_edge_itr++)
					{
						if (in_edge_itr == in_edges.GetEnumerator())
						{
							g[in_edge_itr.Current].next = *(out_edge_itr + 1);
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							g[out_edge_itr].prev = *(in_edges.end() - 1);
						}
						else if (in_edge_itr == (in_edges.end() - 1))
						{
							g[in_edge_itr.Current].next = *(out_edges.GetEnumerator());
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							g[out_edge_itr].prev = *(in_edge_itr - 1);
						}
						else
						{
							g[in_edge_itr.Current].next = *(out_edge_itr + 1);
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
							g[out_edge_itr].prev = *(in_edge_itr - 1);
						}
					}
				}
				/*else if( g[vertex].type == FULLINT ) {
				    std::vector<Vertex> adjacent_vertices;
				    Vertex x_u, x_l, y_u, y_l;
				    boost::tie( x_u, x_l ) = find_neighbor_vertices( VertexPair(vertex, g[vertex].position.x), *(g[vertex].xi), false );
				    boost::tie( y_u, y_l ) = find_neighbor_vertices( VertexPair(vertex, g[vertex].position.y), *(g[vertex].yi), false );
		
				    if( g[x_l].type == INT ) adjacent_vertices.push_back( x_l );
				    if( g[y_u].type == INT ) adjacent_vertices.push_back( y_u );
				    if( g[x_u].type == INT ) adjacent_vertices.push_back( x_u );
				    if( g[y_l].type == INT ) adjacent_vertices.push_back( y_l );
				
				    if( adjacent_vertices.size() == 1 ) {
				        Edge in, out;
				        std::vector<Vertex>::iterator adj_itr = adjacent_vertices.begin();
		
				        if( hedi::has_edge( *adj_itr, vertex, g ) ) {
				            in  = hedi::edge( *adj_itr, vertex, g );
				            out = hedi::edge( vertex, *adj_itr, g );
				        }
				        else {
				            in  = hedi::add_edge( *adj_itr, vertex, g );
				            out = hedi::add_edge( vertex, *adj_itr, g );
				        }
				        g[in].next = out;
				        g[out].prev = in;
				    }
				    else {
				        std::vector<Vertex>::iterator adj_itr;
				        std::vector<Edge> out_edges, in_edges;
				        std::vector<Edge>::iterator edge_itr;
		
				        for( adj_itr=adjacent_vertices.begin(); adj_itr<adjacent_vertices.end(); adj_itr++ ) {
				            if( !hedi::has_edge( *adj_itr, vertex, g ) ) {
				                in_edges.push_back( hedi::add_edge( *adj_itr, vertex, g ) );
				                out_edges.push_back( hedi::add_edge( vertex, *adj_itr, g ) );
				            }
				            else {
				                in_edges.push_back( hedi::edge( *adj_itr, vertex, g ) );
				                out_edges.push_back( hedi::edge( vertex, *adj_itr, g ) );
				            }
				        }
		
				        for( unsigned int i=0; i<in_edges.size(); i++ ) {
				            if( i == (in_edges.size() - 1) ) {
				                g[in_edges[i] ].next = out_edges[ 0 ];
				                g[out_edges[i]].prev =  in_edges[i-1];
				            }
				            else if( i == 0 ) {
				                g[in_edges[i] ].next = out_edges[i+1];
				                g[out_edges[i]].prev =  in_edges[in_edges.size()-1];
				            }
				            else {
				                g[in_edges[i] ].next = out_edges[i+1];
				                g[out_edges[i]].prev =  in_edges[i-1];
				            }
				        }
				    }
		
				}*/
			}
		}


		// given a VertexPair and an Interval, in the Interval find the Vertex above and below the given vertex
		protected Tuple<Vertex,Vertex> find_neighbor_vertices(VertexPair v_pair, Interval ival, bool above_equality)
		{
			Interval.VertexPairIterator itr = ival.intersections2.lower_bound(v_pair); // returns first that is not less than argument (equal or greater)
			Debug.Assert(itr != ival.intersections2.end()); // we must find a lower_bound
			Interval.VertexPairIterator v_above = new Interval.VertexPairIterator();
			if (above_equality)
			{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: v_above = itr;
				v_above.CopyFrom(itr); // lower_bound returns one beyond the give key, i.e. what we want
			}
			else
			{
				v_above = ++itr;
				--itr;
			}
			Interval.VertexPairIterator v_below = --itr; // this is the vertex below the given vertex
			Tuple<Vertex,Vertex> @out = new Tuple<Vertex,Vertex>(null, null);
			@out.Item1 = v_above.first; // vertex above v (xu)
			@out.Item2 = v_below.first; // vertex below v (xl)
			return new Tuple<Vertex,Vertex>(@out.Item1, @out.Item2);
		}
}

} // end weave namespace

} // end ocl namespace
// end file smart_weave.hpp


// end file smart_weave.cpp
