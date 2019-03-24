using System;
using System.Collections.Generic;
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
/// \brief a Triangle defined by its three vertices
///
public class Triangle : System.IDisposable
{
		/// default constructor
		public Triangle()
		{
			p[0] = new Point(1, 0, 0);
			p[1] = new Point(0, 1, 0);
			p[2] = new Point(0, 0, 1);
			calcNormal();
			calcBB();
		}

		/// copy constructor
		public Triangle(Triangle t)
		{
			p[0] = t.p[0];
			p[1] = t.p[1];
			p[2] = t.p[2];
			calcNormal();
			calcBB();
		}

		/// destructor
		public virtual void Dispose()
		{
		}
		/// Create a triangle with the vertices p1, p2, and p3.
		public Triangle(Point p1, Point p2, Point p3)
		{
			p[0] = p1;
			p[1] = p2;
			p[2] = p3;
			calcNormal();
			calcBB();
		}

		/// return true if Triangle is sliced by a z-plane at z=zcut
		/// modify p1 and p2 so that they are intesections of the triangle edges
		/// and the plane. These vertices are used by CylCutter::edgePush()
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool zslice_verts(Point& p1, Point& p2, double zcut) const
		public bool zslice_verts(ref Point p1, ref Point p2, double zcut)
		{
			if ((zcut <= this.bb.minpt.z) || ((zcut >= this.bb.maxpt.z)))
			{
				return false; // no zslice
			}
			// find out how many vertices are below zcut
			List<Point> below = new List<Point>();
			List<Point> above = new List<Point>();
			for (int m = 0;m < 3;++m)
			{
				if (p[m].z <= zcut)
				{
					below.Add(p[m]);
				}
				else
				{
					above.Add(p[m]);
				}
			}
			if (!(below.Count == 1) && !(below.Count == 2))
			{
				Console.Write("triangle.cpp: zslice_verts() error while trying to z-slice\n");
				Console.Write(" triangle=");
				Console.Write(this);
				Console.Write("\n");
				Console.Write(" zcut=");
				Console.Write(zcut);
				Console.Write("\n");
				Console.Write(above.Count);
				Console.Write(" above points:\n");
				foreach (Point p in above)
				{
					Console.Write("   ");
					Console.Write(p);
					Console.Write("\n");
				}
				Console.Write(below.Count);
				Console.Write(" below points:\n");
				foreach (Point p in below)
				{
					Console.Write("   ");
					Console.Write(p);
					Console.Write("\n");
				}
			}
			Debug.Assert((below.Count == 1) || (below.Count == 2));

			if (below.Count == 2)
			{
				Debug.Assert(above.Count == 1);
				// find two new intersection points
				// edge is p1 + t*(p2-p1) = zcut
				// so t = zcut-p1 / (p2-p1)
				double t1 = (zcut - above[0].z) / (below[0].z - above[0].z); // div by zero?!
				double t2 = (zcut - above[0].z) / (below[1].z - above[0].z);
				p1 = above[0] + t1 * (below[0] - above[0]);
				p2 = above[0] + t2 * (below[1] - above[0]);
				return true;
			}
			else if (below.Count == 1)
			{
				Debug.Assert(above.Count == 2);
				// find intersection points and add two new triangles
				// t = (zcut -p1) / (p2-p1)
				double t1 = (zcut - above[0].z) / (below[0].z - above[0].z);
				double t2 = (zcut - above[1].z) / (below[0].z - above[1].z);
				p1 = above[0] + t1 * (below[0] - above[0]);
				p2 = above[1] + t2 * (below[0] - above[1]);
				return true;
			}
			else
			{
				Debug.Assert(false);
				return false;
			}

		}

		/// rotate triangle xrot radians around X-axis, yrot radians around Y-axis
		/// and zrot radians around Z-axis
		public void rotate(double xr, double yr, double zr)
		{
			for (int n = 0;n < 3;++n)
			{
				p[n].xRotate(xr);
				p[n].yRotate(yr);
				p[n].zRotate(zr);
			}
			calcNormal();
			calcBB();
		}
        /*
		/// Triangle string repr     
		public static std::ostream operator << (std::ostream stream, Triangle t)
		{
		  stream << "T: " << t.p[0] << " " << t.p[1] << " " << t.p[2] << "n=" << t.n;
		  return stream;
		}
        */
		/// the three vertex Points of the Triangle
		public Point[] p = Arrays.InitializeWithDefaultInstances<Point>(3); // vertices
		/// normal vector
		public Point n = new Point();
		/// return normal vector with positive z-coordinate 
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point upNormal() const
		public Point upNormal()
		{
			return (n.z < 0) ? -1.0 * n : n;
		}

		/// bounding-box 
		public Bbox bb = new Bbox();



		/// calculate and set Triangle normal

		/// calculate, normalize, and set the Triangle normal
		protected void calcNormal()
		{
			Point v1 = p[0] - p[1];
			Point v2 = p[0] - p[2];
			Point ntemp = v1.cross(v2); // the normal is in the direction of the cross product between the edge vectors
			ntemp.normalize(); // FIXME this might fail if norm()==0
			n = new Point(ntemp.x, ntemp.y, ntemp.z);
		}

		/// update bounding-box

		/// calculate bounding box values
		protected void calcBB()
		{
			bb.clear();
			bb.addTriangle(this);
		}
}

} // end namespace
// end file triangle.h

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)

// end file triangle.cpp
