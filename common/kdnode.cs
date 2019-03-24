using System.Collections.Generic;

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



namespace ocl
{

/// \brief K-D tree node. http://en.wikipedia.org/wiki/Kd-tree
///
/// A k-d tree is used for searching for triangles overlapping with the cutter.
///
//C++ TO C# CONVERTER TODO TASK: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template < class BBObj >
public class KDNode < BBObj > : System.IDisposable
{
		/// Create a node which partitions(cuts) along dimension d, at 
		/// cut value cv, with child-nodes hi_c and lo_c.
		/// If this is a bucket-node containing triangles, 
		/// they are in the list tris
		/// depth indicates the depth of the node in the tree
		public KDNode(int d, double cv, KDNode<BBObj> parentNode, KDNode<BBObj> hi_child, KDNode<BBObj> lo_child, LinkedList< BBObj > tlist, int nodeDepth) // depth of node
		{
			dim = d;
			cutval = cv;
			parent = parentNode;
			hi = hi_child;
			lo = lo_child;
			tris = new LinkedList<BBObj>();
			depth = nodeDepth;
			isLeaf = false;
			if (tlist != null)
			{
				isLeaf = true;
				foreach (BBObj bo in * tlist)
				{
					tris.AddLast(bo);
				}
			}
		}
		public virtual void Dispose()
		{
			// std::cout << " ~KDNode3()\n";
			if (hi != null)
			{
				if (hi != null)
				{
					hi.Dispose();
				}
			}
			if (lo != null)
			{
				if (lo != null)
				{
					lo.Dispose();
				}
			}
			if (tris != null)
			{
				if (tris != null)
				{
					tris.Dispose();
				}
			}
		}
        /*
		/// string repr
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const
		public string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << "KDNode d:" << dim << " cv:" << cutval;
			return o.str();
		}
        */
	// DATA
		/// level of node in tree 
		public int depth;
		/// dimension of cut
		public int dim;
		/// Cut value.
		/// Child node hi contains only triangles with a higher value than this.
		/// Child node lo contains triangles with lower values.
		public double cutval;
		/// parent-node
		public KDNode parent;
		/// Child-node hi.
		public KDNode hi;
		/// Child-node lo.
		public KDNode lo;
		/// A list of triangles, if this is a bucket-node (NULL for internal nodes)
		public LinkedList< BBObj > tris;
		/// flag to indicate leaf in the tree. Leafs or bucket-nodes contain triangles in the list tris.
		public bool isLeaf;
}


} // end namespace
// end file kdnode.h
