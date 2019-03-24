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


///
/// \brief A finite line segment in 3D space specified by its end points (p1, p2)
///

public class Line : System.IDisposable
{
		public Line()
		{
		}
		/// create a line from p1 to p2
		public Line(Point p1in, Point p2in)
		{
			p1.CopyFrom(p1in);
			p2.CopyFrom(p2in);
		}

		/// create a copy of line l.
		public Line(Line l)
		{
			p1.CopyFrom(l.p1);
			p2.CopyFrom(l.p2);
		}

		public virtual void Dispose()
		{
		}

		/// text output
		public static std::ostream operator << (std::ostream stream, Line l)
		{
		  stream << "(" << l.p1 << ", " << l.p2 << ")";
		  return stream;
		}

		/// start point
		public Point p1 = new Point();
		/// end point
		public Point p2 = new Point();

		/// return the length of the line-segment in the xy-plane
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double length2d()const
		public double length2d()
		{
			return (p2 - p1).xyNorm();
		}

		/// return a Point on the Line at parameter value t [0,1]
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point getPoint(double t) const
		public Point getPoint(double t)
		{
			return (p2 - p1) * t + p1;
		}

		/// return the point on the Line which is closest to Point p.
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point Near(const Point& p) const
		public Point Near(Point p)
		{
			// returns the near point from a line on the extended line
			Point v = p2 - p1;
			v.normalize();
			double dp = (p - p1).dot(v);
			return p1 + (v * dp);
		}
}

} // end namespace
// end file line.h


// end file line.cpp
