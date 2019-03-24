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

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)

namespace ocl
{

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Point;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class CLPoint;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Triangle;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class MillingCutter;

/// \brief KDTree spread, a measure of how spread-out a list of triangles are.
///
/// simple struct-like class for storing the "spread" or maximum 
/// extent of a list of triangles. Used by the kd-tree algorithm.
public class Spread
{
		/// constructor
		public Spread(int dim, double v, double s)
		{
			d = dim;
			val = v;
			start = s;
		}
		/// dimension
		public int d;
		/// spread-value
		public double val;
		/// minimum or start value
		public double start;
		/// comparison of Spread objects. Used for finding the largest spread
		/// along which the next partition/cut is made.
		public static bool spread_compare(Spread x, Spread y)
		{
			if (x.val > y.val)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
}

/// a kd-tree for storing triangles and fast searching for triangles
/// that overlap the cutter
//C++ TO C# CONVERTER TODO TASK: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template <class BBObj>
public class KDTree <BBObj> : System.IDisposable
{
		public KDTree()
		{
		}
		public virtual void Dispose()
		{
			// std::cout << " ~KDTree()" << std::endl;
		}
		/// set the bucket-size 
		public void setBucketSize(int b)
		{
			//std::cout << "KDTree::setBucketSize = " << b << "\n"; 
			bucketSize = (uint)b;
		}
		/// set the search dimension to the XY-plane
		public void setXYDimensions()
		{
			//std::cout << "KDTree::setXYDimensions()\n"; 
			dimensions.Clear();
			dimensions.Add(0); // x
			dimensions.Add(1); // x
			dimensions.Add(2); // y
			dimensions.Add(3); // y
		} // for drop-cutter search in XY plane
		/// set search-plane to YZ
		public void setYZDimensions()
		{ // for X-fibers
			//std::cout << "KDTree::setYZDimensions()\n"; 
			dimensions.Clear();
			dimensions.Add(2); // y
			dimensions.Add(3); // y
			dimensions.Add(4); // z
			dimensions.Add(5); // z
		} // for X-fibers
		/// set search plane to XZ
		public void setXZDimensions()
		{ // for Y-fibers
			//std::cout << "KDTree::setXZDimensions()\n";
			dimensions.Clear();
			dimensions.Add(0); // x
			dimensions.Add(1); // x
			dimensions.Add(4); // z
			dimensions.Add(5); // z
		} // for Y-fibers
		/// build the kd-tree based on a list of input objects
		public void build(LinkedList<BBObj> list)
		{
			//std::cout << "KDTree::build() list.size()= " << list.size() << " \n";
			root = build_node(list, 0, null);
		}
		/// search for overlap with input Bbox bb, return found objects
		public LinkedList<BBObj> search(Bbox bb)
		{
			Debug.Assert(dimensions.Count > 0);
			LinkedList<BBObj> tris = new LinkedList<BBObj>();
			this.search_node(tris, bb, root);
			return tris;
		}
		/// search for overlap with a MillingCutter c positioned at cl, return found objects
		public LinkedList<BBObj> search_cutter_overlap(MillingCutter c, CLPoint cl)
		{
			double r = c.getRadius();
			// build a bounding-box at the current CL
			Bbox bb = new Bbox(cl.x - r, cl.x + r, cl.y - r, cl.y + r, cl.z, cl.z + c.getLength());
			return this.search(bb);
		}
		/// string repr
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const;
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//		string str();

		/// build and return a KDNode containing list *tris at depth dep.
		protected KDNode<BBObj> build_node(LinkedList<BBObj> tris, int dep, KDNode<BBObj> par)
		{ // parent node
			//std::cout << "KDNode::build_node list.size()=" << tris->size() << "\n";

			if (tris.Count == 0)
			{ //this is a fatal error.
				Console.Write("ERROR: KDTree::build_node() called with tris->size()==0 ! \n");
				Debug.Assert(0);
				return null;
			}
			Spread spr = calc_spread(tris); // calculate spread in order to know how to cut
			double cutvalue = spr.start + spr.val / 2; // cut in the middle
			//std::cout << " cutvalue= " << cutvalue << "\n";
			if ((tris.Count <= bucketSize) || isZero_tol(spr.val))
			{ // then return a bucket/leaf node
				//std::cout << "KDNode::build_node BUCKET list.size()=" << tris->size() << "\n";
				KDNode<BBObj> bucket; //  dim   cutv   parent   hi    lo   triangles depth
				bucket = new KDNode<BBObj>(spr.d, cutvalue, par, null, null, tris, dep);
				Debug.Assert(bucket.isLeaf);
				spr = null;
				return bucket; // this is the leaf/end of the recursion-tree
			}
			// build lists of triangles for hi and lo child nodes
			LinkedList<BBObj> lolist = new LinkedList<BBObj>();
			LinkedList<BBObj> hilist = new LinkedList<BBObj>();
			foreach (BBObj t in * tris)
			{ // loop through each triangle and put it in either lolist or hilist
				if (t.bb[spr.d] > cutvalue)
				{
					hilist.AddLast(t);
				}
				else
				{
					lolist.AddLast(t);
				}
			}

			/*
			if (hilist->empty() || lolist->empty()) {// an error ??
			    std::cout << "kdtree: hilist.size()==0! or lolist.size()==0! \n";
			    std::cout << "kdtree: tris->size()= " << tris->size()<< "\n";
			    std::cout << "kdtree: hilist.size()= " << hilist->size()<< "\n";
			    std::cout << "kdtree: lolist.size()= " << lolist->size()<< "\n";
			    BOOST_FOREACH(BBObj t, *tris) {
			        std::cout << t << "\n";
			        std::cout << t.bb << "\n";
			    }
			    std::cout << "kdtree: spr->d= " << spr->d << "\n";
			    std::cout << "kdtree: cutvalue= " << cutvalue << "\n";
			    assert(0);
			}*/


			// create the current node  dim     value    parent  hi   lo   trilist  depth
			KDNode<BBObj> node = new KDNode<BBObj>(spr.d, cutvalue, par, null, null, null, dep);
			// create the child-nodes through recursion
			//                    list    depth   parent
			if (hilist.Count > 0)
			{
				node.hi = build_node(hilist, dep + 1, node);
			}
			//else
				//std::cout << "hilist empty!\n";

			if (lolist.Count > 0)
			{
				node.lo = build_node(lolist, dep + 1, node);
			}
			else
			{
				//std::cout << "lolist empty!\n";
			}

			lolist.Clear();
			hilist.Clear();
			spr = null;
			lolist = null;
			hilist = null;

			return node; // return a new node
		}

		/// calculate the spread of list *tris                        
		protected Spread calc_spread(LinkedList<BBObj> tris)
		{
			List<double> maxval = new List<double>(6);
			List<double> minval = new List<double>(6);
			if (tris.Count == 0)
			{
				Console.Write(" ERROR, KDTree::calc_spread() called with tris->size()==0 ! \n");
				Debug.Assert(0);
				return null;
			}
			else
			{
				// find out the maximum spread
				//std::cout << "calc_spread()...\n";
				bool first = true;
				foreach (BBObj t in * tris)
				{ // check each triangle
					for (uint m = 0;m < dimensions.Count;++m)
					{
						// dimensions[m] is the dimensions we want to update
						// t.bb[ dimensions[m] ]   is the update value
						if (first)
						{
							maxval[dimensions[m]] = t.bb[dimensions[m]];
							minval[dimensions[m]] = t.bb[dimensions[m]];
							if (m == (dimensions.Count - 1))
							{
								first = false;
							}
						}
						else
						{
							if (maxval[dimensions[m]] < t.bb[dimensions[m]])
							{
								maxval[dimensions[m]] = t.bb[dimensions[m]];
							}
							if (minval[dimensions[m]] > t.bb[dimensions[m]])
							{
								minval[dimensions[m]] = t.bb[dimensions[m]];
							}
						}
					}
				}
				List<Spread> spreads = new List<Spread>(); // calculate the spread along each dimension
				for (uint m = 0;m < dimensions.Count;++m)
				{ // dim,  spread, start
					spreads.Add(new Spread(dimensions[m], maxval[dimensions[m]] - minval[dimensions[m]], minval[dimensions[m]]));
				} // priority-queue could also be used ??
				Debug.Assert(spreads.Count > 0);
				//std::cout << " spreads.size()=" << spreads.size() << "\n";
//C++ TO C# CONVERTER TODO TASK: The 'Compare' parameter of std::sort produces a boolean value, while the .NET Comparison parameter produces a tri-state result:
//ORIGINAL LINE: std::sort(spreads.begin(), spreads.end(), Spread::spread_compare);
				spreads.Sort(Spread.spread_compare); // sort the list
				Spread s = new Spread(*spreads[0]); // this is the one we want to return
				while (spreads.Count > 0)
				{
					spreads[spreads.Count - 1], spreads.RemoveAt(spreads.Count - 1) = null; // delete the others
				}
				//std::cout << "calc_spread() done\n";
				return s; // select the biggest spread and return
			} // end tris->size != 0
		} // end spread();


		/// search kd-tree starting at *node, looking for overlap with bb, and placing
		/// found objects in *tris
		protected void search_node(LinkedList<BBObj> tris, Bbox bb, KDNode<BBObj> node)
		{
			if (node.isLeaf)
			{ // we found a bucket node, so add all triangles and return.

				foreach (BBObj t in * (node.tris))
				{
						tris.AddLast(t);
				}
				//std::cout << " search_node Leaf bucket tris-size() = " << tris->size() << "\n";
				return; // end recursion
			}
			else if ((node.dim % 2) == 0)
			{ // cutting along a min-direction: 0, 2, 4
				// not a bucket node, so recursevily search hi/lo branches of KDNode
				uint maxdim = (uint)(node.dim + 1);
				if (node.cutval > bb[maxdim])
				{ // search only lo
					search_node(tris, bb, node.lo);
				}
				else
				{ // need to search both child nodes
					if (node.hi != null)
					{
						search_node(tris, bb, node.hi);
					}
					if (node.lo != null)
					{
						search_node(tris, bb, node.lo);
					}
				}
			}
			else
			{ // cutting along a max-dimension: 1,3,5
				uint mindim = (uint)(node.dim - 1);
				if (node.cutval < bb[mindim])
				{ // search only hi
					search_node(tris, bb, node.hi);
				}
				else
				{ // need to search both child nodes
					if (node.hi != null)
					{
						search_node(tris, bb, node.hi);
					}
					if (node.lo != null)
					{
						search_node(tris, bb, node.lo);
					}
				}
			}
			return; // Done. We get here after all the recursive calls above.
		} // end search_kdtree();
	// DATA
		/// bucket size of tree
		protected uint bucketSize;
		/// pointer to root KDNode
		protected KDNode<BBObj> root;
		/// the dimensions in this kd-tree
		protected List<int> dimensions = new List<int>();
}

} // end ocl namespace
// end file kdtree.hpp
