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

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Triangle;

///
/// \brief a point or vector in 3D space specified by its coordinates (x, y, z)
///
public class Point : System.IDisposable
{
		/// create a point at (0,0,0)
		public Point()
		{
			x = 0;
			y = 0;
			z = 0;
		}

		/// create a point at (x,y,z)
		public Point(double xin, double yin, double zin)
		{
			x = xin;
			y = yin;
			z = zin;
		}

		/// create a point at (x,y,0)
		public Point(double xin, double yin)
		{
			x = xin;
			y = yin;
			z = 0.0;
		}

		/// create a point at p
		public Point(Point p)
		{
			x = p.x;
			y = p.y;
			z = p.z;
		}

		/// destructor. empty.
		public virtual void Dispose()
		{
		}

		/// dot product
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double dot(const Point &p) const
		public double dot(Point p)
		{
			return x * p.x + y * p.y + z * p.z;
		}

		/// cross product
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point cross(const Point &p) const
		public Point cross(Point p)
		{
			double xc = y * p.z - z * p.y;
			double yc = z * p.x - x * p.z;
			double zc = x * p.y - y * p.x;
			return new Point(xc, yc, zc);
		}

		/// norm of vector, or distance from (0,0,0) to *this

		//********     methods ********************** */


//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double norm() const
		public double norm()
		{
			return Math.Sqrt(ocl.GlobalMembers.square(x) + ocl.GlobalMembers.square(y) + ocl.GlobalMembers.square(z));
		}

		/// scales vector so that norm()==1.0
		public void normalize()
		{
			if (this.norm() != 0.0)
			{
				this *= (1 / this.norm());
			}
		}

		/// distance from Point to another Point p in the XY plane
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double xyDistance(const Point &p) const
		public double xyDistance(Point p)
		{
			return (this - p).xyNorm();
			//return sqrt(pow(x - p.x, 2) + pow((y - p.y), 2));
		}

		/// length of vector in the XY plane
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double xyNorm() const
		public double xyNorm()
		{
			return Math.Sqrt(ocl.GlobalMembers.square(x) + ocl.GlobalMembers.square(y));
		}

		/// normalize so that length xyNorm == 1.0
		public void xyNormalize()
		{
			if (this.xyNorm() != 0.0)
			{
				this *= (1 / this.xyNorm());
			}
		}

		/// return perpendicular in the xy plane, rotated 90 degree to the left
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point xyPerp() const
		public Point xyPerp()
		{
				return new Point(-y, x, z);
		}

		/// move *this along z-axis so it lies on p1-p2 line-segment 
		public void z_projectOntoEdge(Point p1, Point p2)
		{
			// edge is p1+t*(p2-p1)
			// now locate z-coord of *this on edge
			double t;
			if (Math.Abs(p2.x - p1.x) > Math.Abs(p2.y - p1.y))
			{
				t = (this.x - p1.x) / (p2.x - p1.x);
			}
			else
			{
				t = (this.y - p1.y) / (p2.y - p1.y);
			}
			this.z = p1.z + t * (p2.z - p1.z);
		}

		/// rotate point in the xy-plane by angle theta
		/// inputs are cos(theta) and sin(theta)
		public void xyRotate(double cosa, double sina)
		{ // rotate vector by angle
			double temp = -y * sina + x * cosa;
			y = x * sina + cosa * y;
			x = temp;
		}

		/// rotate point in xy-plane bu angle theta (radians or degrees??)
		public void xyRotate(double angle)
		{
			xyRotate(Math.Cos(angle), Math.Sin(angle));
		}

		/// rotate around x-axis
		public void xRotate(double theta)
		{
			matrixRotate(1, 0, 0, 0, Math.Cos(theta), -Math.Sin(theta), 0, Math.Sin(theta), Math.Cos(theta));
		}

		/// rotate around y-axis
		public void yRotate(double theta)
		{
			matrixRotate(Math.Cos(theta), 0, Math.Sin(theta), 0, 1, 0, -Math.Sin(theta), 0, Math.Cos(theta));
		}

		/// rotate around z-axis
		public void zRotate(double theta)
		{
			matrixRotate(Math.Cos(theta), -Math.Sin(theta), 0, Math.Sin(theta), Math.Cos(theta), 0, 0, 0, 1);
		}


		// http://en.wikipedia.org/wiki/Rotation_matrix
		public void matrixRotate(double a, double b, double c, double d, double e, double f, double g, double h, double i)
		{
			// multiply point with matrix
			double xr = a * x + b * y + c * z;
			double yr = d * x + e * y + f * z;
			double zr = g * x + h * y + i * z;
			x = xr;
			y = yr;
			z = zr;
		}

		/// distance from Point to infinite line through p1 and p2. In the XY plane.
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double xyDistanceToLine(const Point &p1, const Point &p2) const
		public double xyDistanceToLine(Point p1, Point p2)
		{
			// see for example
			// http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
			if ((p1.x == p2.x) && (p1.y == p2.y))
			{ // no line in xy plane
				Console.Write("point.cpp: xyDistanceToLine ERROR!: can't calculate distance from \n");
				Console.Write("point.cpp: xyDistanceToLine ERROR!: *this =");
				Console.Write(this);
				Console.Write(" to line through\n");
				Console.Write("point.cpp: xyDistanceToLine ERROR!: p1=");
				Console.Write(p1);
				Console.Write(" and \n");
				Console.Write("point.cpp: xyDistanceToLine ERROR!: p2=");
				Console.Write(p2);
				Console.Write("\n");
				Console.Write("point.cpp: xyDistanceToLine ERROR!: in the xy-plane\n");
				return -1;
			}
			else
			{
				Point v = new Point(p2.y - p1.y, -(p2.x - p1.x), 0);
				v.normalize();
				Point r = new Point(p1.x - x, p1.y - y, 0);
				return Math.Abs(v.dot(r));
			}
		}

		/// return closest Point to line through p1 and p2. in 3D.

		/// return Point on p1-p2 line which is closest in 3D to this.
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point closestPoint(const Point &p1, const Point &p2) const
		public Point closestPoint(Point p1, Point p2)
		{
			Point v = p2 - p1;
			Debug.Assert(v.norm() > 0.0);
			double u = (this - p1).dot(v) / v.dot(v); // u = (p3-p1) dot v / (v dot v)
			return p1 + u * v;
		}

		/// return closest Point to line through p1 and p2. Works in the XY plane.

		/// return Point on p1-p2 line which is closest in XY-plane to this
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point xyClosestPoint(const Point &p1, const Point &p2) const
		public Point xyClosestPoint(Point p1, Point p2)
		{
			// one explanation is here
			// http://local.wasp.uwa.edu.au/~pbourke/geometry/pointline/
			Point pt1 = new Point(p1); // this required because of "const" arguments above.
			Point pt2 = new Point(p2);
			Point v = pt2 - pt1;
			if (isZero_tol(v.xyNorm()))
			{ // if p1 and p2 do not make a line in the xy-plane
				Console.Write("point.cpp: xyClosestPoint ERROR!: can't calculate closest point from \n");
				Console.Write("point.cpp: xyClosestPoint ERROR!: *this =");
				Console.Write(this);
				Console.Write(" to line through\n");
				Console.Write("point.cpp: xyClosestPoint ERROR!: p1=");
				Console.Write(p1);
				Console.Write(" and \n");
				Console.Write("point.cpp: xyClosestPoint ERROR!: p2=");
				Console.Write(p2);
				Console.Write("\n");
				Console.Write("point.cpp: xyClosestPoint ERROR!: in the xy-plane\n");
				Debug.Assert(false);
				return new Point(0, 0, 0);
			}

			double u;
			// vector notation:
			// u = (p3-p1) dot v / (v dot v)
			u = (this.x - p1.x) * (v.x) + (this.y - p1.y) * (v.y);
			u = u / (v.x * v.x + v.y * v.y);
			// coordinates for closest point
			double x = p1.x + u * v.x;
			double y = p1.y + u * v.y;
			return new Point(x, y, 0);
		}

		/// returns true if point is right of line through p1 and p2 (works in the XY-plane)
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool isRight(const Point &p1, const Point &p2) const
		public bool isRight(Point p1, Point p2)
		{
			// is Point right of line through points p1 and p2 ?, in the XY plane.
			// this is an ugly way of doing a determinant
			// should be prettyfied sometime...
			/// \todo FIXME: what if p1==p2 ? (in the XY plane)
			double a1 = p2.x - p1.x;
			double a2 = p2.y - p1.y;
			double t1 = a2;
			double t2 = -a1;
			double b1 = x - p1.x;
			double b2 = y - p1.y;

			double t = t1 * b1 + t2 * b2;
			if (t > 0.00000000000001) /// \todo FIXME: hardcoded magic number...
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// returns true if Point *this is inside Triangle t 
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool isInside(const Triangle &t) const
		public bool isInside(Triangle t)
		{
			// point in triangle test
			// http://www.blackpawn.com/texts/pointinpoly/default.html

			Point v0 = t.p[2] - t.p[0];
			Point v1 = t.p[1] - t.p[0];
			Point v2 = this - t.p[0];

			double dot00 = v0.dot(v0);
			double dot01 = v0.dot(v1);
			double dot02 = v0.dot(v2);
			double dot11 = v1.dot(v1);
			double dot12 = v1.dot(v2);

			double invD = 1.0 / (dot00 * dot11 - dot01 * dot01);
			// barycentric coordinates
			double u = (dot11 * dot02 - dot01 * dot12) * invD;
			double v = (dot00 * dot12 - dot01 * dot02) * invD;

			// Check if point is in triangle
			return (u > 0.0) && (v > 0.0) && (u + v < 1.0);
		}

		/// return true if Point within line segment p1-p2
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool isInside(const Point& p1, const Point& p2) const
		public bool isInside(Point p1, Point p2)
		{
			// segment is p1 + t*(p2-p1)
			// p1 + t*(p2-p1) = p
			// p1*(p2-p1) + t * (p2-p1)*(p2-p1) = p*(p2-p1)
			// t = (p - p1 )*(p2-p1) / (p2-p1)*(p2-p1)
			double t = (this - p1).dot(p2 - p1) / (p2 - p1).dot(p2 - p1);
			if (t > 1.0)
			{
				return false;
			}
			else if (t < 0.0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}


		/// return true if the x and y components are both zero.
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool xParallel() const
		public bool xParallel()
		{
			if (isZero_tol(y) && isZero_tol(z))
			{
				return true;
			}
			return false;
		}

		/// return true if vector parallel to y-axis
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool yParallel() const
		public bool yParallel()
		{
			if (isZero_tol(x) && isZero_tol(z))
			{
				return true;
			}
			return false;
		}

		/// return true if vector parallel to z-axis
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool zParallel() const
		public bool zParallel()
		{
			if (x != 0.0)
			{
				return false;
			}
			else if (y != 0.0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// assignment

		/* **************** Operators ***************  
		 *  see
		 *  http://www.cs.caltech.edu/courses/cs11/material/cpp/donnie/cpp-ops.html
		*/

//C++ TO C# CONVERTER NOTE: This 'CopyFrom' method was converted from the original copy assignment operator:
//ORIGINAL LINE: Point& operator =(const Point &p)
		public Point CopyFrom(Point p)
		{
			if (this == p)
			{
				return this;
			}
			x = p.x;
			y = p.y;
			z = p.z;
			return this;
		}

		/// addition
//C++ TO C# CONVERTER TODO TASK: The += operator cannot be overloaded in C#:
		public static Point operator += (Point p)
		{
			x += p.x;
			y += p.y;
			z += p.z;
			return this;
		}

		/// subtraction
//C++ TO C# CONVERTER TODO TASK: The -= operator cannot be overloaded in C#:
		public static Point operator -= (Point p)
		{
			x -= p.x;
			y -= p.y;
			z -= p.z;
			return this;
		}

		/// addition
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: const Point operator +(const Point &p) const
		public static Point operator + (Point ImpliedObject, Point p)
		{
			return new Point(ImpliedObject) += p;
		}

		/// subtraction
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: const Point operator -(const Point &p) const
		public static Point operator - (Point ImpliedObject, Point p)
		{
			return new Point(ImpliedObject) -= p;
		}

		/// scalar multiplication

		// Point*scalar multiplication
//C++ TO C# CONVERTER TODO TASK: The *= operator cannot be overloaded in C#:
		public static Point operator *= (double a)
		{
			x *= a;
			y *= a;
			z *= a;
			return this;
		}

		/// Point * scalar
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: const Point operator *(const double &a) const
		public static Point operator * (Point ImpliedObject, double a)
		{
			return new Point(ImpliedObject) *= a;
		}

		/// equality
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool operator ==(const Point &p) const
		public static bool operator == (Point ImpliedObject, Point p)
		{
			return (ImpliedObject == p) || (ImpliedObject.x == p.x && ImpliedObject.y == p.y && ImpliedObject.z == p.z);
		}

		/// inequality
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool operator !=(const Point &p) const
		public static bool operator != (Point ImpliedObject, Point p)
		{
			return !(ImpliedObject == p);
		}
        /*
		/// string repr
		public static std::ostream operator << (std::ostream stream, Point p)
		{
		  stream << "(" << p.x << ", " << p.y << ", " << p.z << ")";
		  return stream;
		}
       
		/// string repr
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const
		public string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << this;
			return o.str();
		}
        */
		/// X coordinate
		public double x;
		/// Y coordinate
		public double y;
		/// Z coordinate
		public double z;
}

} // end namespace
// end file point.h

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)

// end file point.cpp
