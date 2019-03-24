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

/// type of cc-point
public enum CCType
{
	NONE,
			 VERTEX,
			 VERTEX_CYL,
			 EDGE,
			 EDGE_HORIZ,
			 EDGE_SHAFT,
			 EDGE_HORIZ_CYL,
			 EDGE_HORIZ_TOR,
			 EDGE_BALL,
			 EDGE_POS,
			 EDGE_NEG,
			 EDGE_CYL,
			 EDGE_CONE,
			 EDGE_CONE_BASE,
			 FACET,
			 FACET_TIP,
			 FACET_CYL,
			 ERROR
}

///
/// \brief Cutter-Contact (CC) point. A Point with a CCType.
///
/// Cutter-Contact (CC) Point.
/// A Point which also contains the type of cutter-contact.
public class CCPoint : Point
{
		/// create a CCPoint at (0,0,0)

		/* ********************************************** CCPoint *************/

		public CCPoint() : base()
		{
			type = CCType.NONE;
		}

		/// create CCPoint at (x,y,z)
		public CCPoint(double x, double y, double z) : base(x, y, z)
		{
			type = CCType.NONE;
		}

		/// create CCPoint at (x,y,z) with type t
		public CCPoint(double x, double y, double z, CCType t) : base(x, y, z)
		{
			type = t;
		}

		/// create CCPoint at p with type t
		public CCPoint(Point p, CCType t) : base(p)
		{
			type = t;
		}

		/// create a CCPoint at Point p
		public CCPoint(Point p) : base(p)
		{
			type = CCType.NONE;
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		/// specifies the type of the Cutter Contact point. 
		public CCType type;
		/// assign coordinates of Point to this CCPoint. sets type=NONE
//C++ TO C# CONVERTER NOTE: This 'CopyFrom' method was converted from the original copy assignment operator:
//ORIGINAL LINE: CCPoint& operator =(const Point &p)
		public CCPoint CopyFrom(Point p)
		{
			x = p.x;
			y = p.y;
			z = p.z;
			type = CCType.NONE;
			return this;
		}
        /*
		/// string repr
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const
		public new string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << this;
			return o.str();
		}
        
		/// string repr
		public static std::ostream operator << (std::ostream stream, CCPoint p)
		{
		  stream << "CC(" << p.x << ", " << p.y << ", " << p.z << ", t=" << p.type << ")";
		  return stream;
		}
        */
}

} // end namespace
// end file ccpoint.h


// end file ccpoint.cpp
