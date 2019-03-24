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

//#include <pair>

//#include "weave_typedef.hpp"

namespace ocl
{

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Fiber;

/// interval for use by fiber and weave
/// a parameter interval [upper, lower]
public class Interval : System.IDisposable
{
		public Interval()
		{
			lower = 0.0;
			upper = 0.0;
			lower_cc = new CCPoint();
			upper_cc = new CCPoint();
			in_weave = false;
		}

		/// create and interval [l,u]  (is this ever called??)
		public Interval(double l, double u)
		{
			Debug.Assert(l <= u);
			lower = l;
			upper = u;
			in_weave = false;
		}

		public virtual void Dispose()
		{
		}

		/// update upper with t, and corresponding cc-point p
		public void updateUpper(double t, CCPoint p)
		{
			if (upper_cc.type == CCType.NONE)
			{
				upper = t;
				lower = t;
				CCPoint tmp = new CCPoint(p);
				upper_cc.CopyFrom(tmp);
				lower_cc.CopyFrom(tmp);
				if (tmp != null)
				{
					tmp.Dispose();
				}
			}
			if (t > upper)
			{
				upper = t;
				CCPoint tmp = new CCPoint(p);
				upper_cc.CopyFrom(tmp);
				if (tmp != null)
				{
					tmp.Dispose();
				}
			}
		}

		/// update lower with t, and corresponding cc-point p
		public void updateLower(double t, CCPoint p)
		{
			if (lower_cc.type == CCType.NONE)
			{
				lower = t;
				upper = t;
				CCPoint tmp = new CCPoint(p);
				lower_cc.CopyFrom(tmp);
				upper_cc.CopyFrom(tmp);
				if (tmp != null)
				{
					tmp.Dispose();
				}
			}
			if (t < lower)
			{
				lower = t;
				CCPoint tmp = new CCPoint(p);
				lower_cc.CopyFrom(tmp);
				if (tmp != null)
				{
					tmp.Dispose();
				}
			}
		}

		/// call both updateUpper() and updateLower() with the given (t,p) pair
		public void update(double t, CCPoint p)
		{
			this.updateUpper(t, p);
			this.updateLower(t, p);
		}

		/// update interval with t_cl and cc_tmp if cc_tmp is in the p1-p2 edge and condition==true
		public bool update_ifCCinEdgeAndTrue(double t_cl, CCPoint cc_tmp, Point p1, Point p2, bool condition)
		{
			//if ( cc_tmp.isInsidePoints(p1,p2)  && condition ) {
			if (cc_tmp.isInside(p1, p2) && condition)
			{
				update(t_cl, cc_tmp); // both updateUpper(t,p) and updateLower(t,p)
				return true;
			}
			else
			{
				return false;
			}
		}

		/// return true if Interval i is outside *this

		// return true if *this is completely non-overlapping, or outside of i.
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool outside(const Interval& i) const
		public bool outside(Interval i)
		{
			if (this.lower > i.upper)
			{
				return true;
			}
			else if (this.upper < i.lower)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// return true if Interval i is inside *this

		// return true if *this is contained within i
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool inside(const Interval& i) const
		public bool inside(Interval i)
		{
			if ((this.lower > i.lower) && (this.upper < i.upper))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// return true if the interval is empty
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool empty() const
		public bool empty()
		{
			if (lower == 0.0 && upper == 0.0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
        /*
		/// string repr
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const
		public string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << "I [" << lower << " , " << upper << " ]";
			return o.str();
		}
        */

		public CCPoint upper_cc = new CCPoint(); ///< cutter contact points at upper and lower are stored in upper_cc and lower_cc
		public CCPoint lower_cc = new CCPoint(); ///< cutter contact point corresponding to lower
		public double upper; ///< the upper t-value
		public double lower; ///< the lower t-value
		public bool in_weave; ///< flag for use by Weave::build()
		public SortedSet<List<Fiber>.Enumerator> intersections_fibers = new SortedSet<List<Fiber>.Enumerator>(); ///< fibers

		/// intersections with other intervals are stored in this set of
		/// VertexPairs of type std::pair<VertexDescriptor, double>


		/// compare based on pair.second, the coordinate of the intersection
		public class VertexPairCompare
		{
			/// comparison operator
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool operator ()(const VertexPair& lhs, const VertexPair& rhs) const
			public static bool functorMethod(VertexPair lhs, VertexPair rhs)
			{
				return lhs.second < rhs.second;
			}
		}

		/// intersections stored in this set (for rapid finding of neighbors etc)

		// this is the same type as ocl::weave::VertexPairIterator, but redefined here anywhere


		public SortedSet< Tuple< boost::adjacency_list_traits<boost::listS, boost::listS, boost::bidirectionalS, boost::listS >.vertex_descriptor, double >, VertexPairCompare > intersections2 = new SortedSet< Tuple< boost::adjacency_list_traits<boost::listS, boost::listS, boost::bidirectionalS, boost::listS >.vertex_descriptor, double >, VertexPairCompare >();
}

} // end namespace
// end file interval.hpp


// end file interval.cpp
