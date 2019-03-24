using System;

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

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)

namespace ocl
{

///
/// \brief a finite arc segment in 3D space specified by its end points (p1, p2)
///
public class Arc : System.IDisposable
{
		/// 2D length of the segment in the xy-plane
		private double length; // 2d length
		/// radius of the arc
		private double radius;

		public Arc()
		{
		}
		/// create an arc from point p1 to point p2 with center c and direction dir.
		/// direction is true for anti-clockwise arcs.
		public Arc(Point p1in, Point p2in, Point cin, bool dirin)
		{
			p1.CopyFrom(p1in);
			p2.CopyFrom(p2in);
			c.CopyFrom(cin);
			dir = dirin;
			setProperties();
		}

		/// copy constructor
		public Arc(Arc a)
		{
			p1.CopyFrom(a.p1);
			p2.CopyFrom(a.p2);
			c.CopyFrom(a.c);
			dir = a.dir;
			setProperties();
		}

		public virtual void Dispose()
		{
		}

		/// text output
		public static std::ostream operator << (std::ostream stream, Arc a)
		{
		  stream << "(" << a.p1 << ", " << a.p2 << ", " << a.c << ", " << a.dir << ")";
		  return stream;
		}

		/// start point
		public Point p1 = new Point();
		/// end point
		public Point p2 = new Point();
		/// centre point
		public Point c = new Point();
		/// direction true for anti-clockwise
		public bool dir;
		/// return the length of the arc
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double length2d()const
		public double length2d()
		{
			return length;
		}
		/// return a point along the arc at parameter value t [0,1]
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point getPoint(double t)const
		public Point getPoint(double t)
		{
				/// returns a point which is 0-1 along span
				if (Math.Abs(t) < 0.00000000000001)
				{
				return new ocl.Point(p1);
				}
				if (Math.Abs(t - 1.0) < 0.00000000000001)
				{
				return new ocl.Point(p2);
				}

				double d = t * length;
				if (!dir)
				{
					d = -d;
				}
				Point v = p1 - c;
				v.xyRotate(d / radius);
				return v + c;
		}

		/// returns the absolute included angle (in radians) between 
		/// two vectors v1 and v2 in the direction of dir ( true=acw  false=cw)
		public double xyIncludedAngle(Point v1, Point v2, bool dir = true)
		{
			// returns the absolute included angle between 2 vectors in
			// the direction of dir ( true=acw  false=cw )
			int d = dir ? 1 : (-1);
			double inc_ang = v1.dot(v2);
			if (inc_ang > 1.0 - 1.0e-10)
			{
				return 0;
			}
			if (inc_ang < -1.0 + 1.0e-10)
			{
				inc_ang = DefineConstants.PI;
			}
			else
			{ // dot product,   v1 . v2  =  cos(alfa)
				if (inc_ang > 1.0)
				{
					inc_ang = 1.0;
				}
				inc_ang = Math.Acos(inc_ang); // 0 to pi radians

				double x = v1.x * v2.y - v1.y * v2.x;
				if (d * x < 0)
				{
					inc_ang = 2 * DefineConstants.PI - inc_ang; // cp
				}
			}
			return d * inc_ang;
		}

		/// set arc-properties
		private void setProperties()
		{
			// arc properties
			Point vs = (p1 - c).xyPerp();
			Point ve = (p2 - c).xyPerp();
			radius = vs.xyNorm();
			vs.normalize();
			ve.normalize();
			length = Math.Abs(xyIncludedAngle(vs, ve, dir)) * radius;
		}
}

} // end namespace
// end file arc.h


// end file arc.cpp
