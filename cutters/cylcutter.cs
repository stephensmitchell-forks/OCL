using System;
using System.Diagnostics;

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


//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <boost/foreach.hpp>

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

///
/// \brief Cylindrical MillingCutter (flat-endmill)
///
/// defined by one parameter, the cutter diameter
public class CylCutter : MillingCutter
{
		public CylCutter()
		{
			// std::cout << " usage: CylCutter( double diameter, double length ) \n";
			Debug.Assert(false);
		}

		/// create CylCutter with diameter d and length l
		public CylCutter(double d, double l)
		{
			diameter = d;
			Debug.Assert(d > 0.0);
			radius = d / 2.0;
			length = l;
			Debug.Assert(l > 0.0);
			xy_normal_length = radius;
			normal_length = 0.0;
			center_height = 0.0;
		}

		/// offset of Cylinder is BullCutter
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: MillingCutter* offsetCutter(double d) const
		public new MillingCutter offsetCutter(double d)
		{
			return new BullCutter(diameter + 2 * d, d, length + d);
		}
        /*
		/// string repr
		public static std::ostream operator << (std::ostream stream, CylCutter c)
		{
		  stream << "CylCutter (d=" << c.diameter << ", L=" << c.length << ")";
		  return stream;
		}
        
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const
		public new string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << this;
			return o.str();
		}
        */

		// general purpose vertexPush, delegates to this->width(h) 
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool vertexPush(const Fiber& f, Interval& i, const Triangle& t) const
		protected new bool vertexPush(Fiber f, Interval i, Triangle t)
		{
			bool result = false;
			foreach (Point p in t.p)
			{
				if (this.singleVertexPush(f, i, p, CCType.VERTEX))
				{
					result = true;
				}
			}

			Point p1 = new Point();
			Point p2 = new Point();
			if (t.zslice_verts(ref p1, ref p2, f.p1.z))
			{
				p1.z = f.p1.z; // z-coord should be very close to f.p1.z, but set it exactly anyway.
				p2.z = f.p1.z;
				if (this.singleVertexPush(f, i, p1, CCType.VERTEX_CYL))
				{
					result = true;
				}
				if (this.singleVertexPush(f, i, p2, CCType.VERTEX_CYL))
				{
					result = true;
				}
			}

			return result;
		}


		// drop-cutter vertexDrop is handled by the base-class method in MillingCutter
		// drop-cutter facetDrop is handled by the base-class method in MillingCutter

		// we handle the edge-drop here.
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: System.Tuple< double, double > singleEdgeDropCanonical(const Point& u1, const Point& u2) const
		protected new Tuple< double, double > singleEdgeDropCanonical(Point u1, Point u2)
		{
			// along the x-axis the cc-point is at x-coord s or -s:
			double s = Math.Sqrt(ocl.GlobalMembers.square(radius) - ocl.GlobalMembers.square(u1.y));
			Point cc1 = new Point(s, u1.y, 0);
			Point cc2 = new Point(-s, u1.y, 0);
			cc1.z_projectOntoEdge(u1, u2);
			cc2.z_projectOntoEdge(u1, u2);
			// pick the higher one
			double cc_u;
			double cl_z;
			if (cc1.z > cc2.z)
			{
				cc_u = cc1.x;
				cl_z = cc1.z;
			}
			else
			{
				cc_u = cc2.x;
				cl_z = cc2.z;
			}
			return new Tuple< double, double >(cc_u, cl_z);
		}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double height(double r) const
		protected new double height(double r)
		{
			return (r <= radius) ? 0.0 : -1.0;
		}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double width(double h) const
		protected new double width(double h)
		{
			return radius;
		}
}

} // end namespace

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)

// end file cylcutter.cpp
