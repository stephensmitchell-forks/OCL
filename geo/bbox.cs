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

/// axis-aligned bounding-box
public class Bbox : System.IDisposable
{
		/// default constructor

		//************* axis-aligned Bounding-Box **************/

		public Bbox()
		{
			minpt = new Point(0, 0, 0);
			maxpt = new Point(0, 0, 0);
			initialized = false;
		}

		/// explicit constructor
		//              minx       maxx        miny       maxy       minz       maxz
		public Bbox(double b1, double b2, double b3, double b4, double b5, double b6)
		{
			minpt = new Point(b1, b3, b5);
			maxpt = new Point(b2, b4, b6);
			initialized = true;
		}

		public virtual void Dispose()
		{
		}


		/// index into maxpt and minpt returning a vector
		/// [minx maxx miny maxy minz maxz]

		// return the bounding box values as a vector:
		//  0    1    2    3    4    5
		// [minx maxx miny maxy minz maxz]
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double operator [](const uint idx) const
		public double this[uint idx]
		{
			get
			{
				switch (idx)
				{
					case 0:
						return minpt.x;
						break;
					case 1:
						return maxpt.x;
						break;
					case 2:
						return minpt.y;
						break;
					case 3:
						return maxpt.y;
						break;
					case 4:
						return minpt.z;
						break;
					case 5:
						return maxpt.z;
						break;
					default:
						Debug.Assert(0);
						break;
				}
				Debug.Assert(0);
				return -1;
			}
		}

		/// return true if Point p is inside this Bbox
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool isInside(Point& p) const
		public bool isInside(Point p)
		{
			Debug.Assert(initialized);
			if (p.x > maxpt.x)
			{
				return false;
			}
			else if (p.x < minpt.x)
			{
				return false;
			}
			else if (p.y > maxpt.y)
			{
				return false;
			}
			else if (p.y < minpt.y)
			{
				return false;
			}
			else if (p.z > maxpt.z)
			{
				return false;
			}
			else if (p.z < minpt.z)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// return true if *this overlaps Bbox b

		/// does this Bbox overlap with b?
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool overlaps(const Bbox& b) const
		public bool overlaps(Bbox b)
		{
			if ((this.maxpt.x < b.minpt.x) || (this.minpt.x > b.maxpt.x))
			{
				return false;
			}
			else if ((this.maxpt.y < b.minpt.y) || (this.minpt.y > b.maxpt.y))
			{
				return false;
			}
			else if ((this.maxpt.z < b.minpt.z) || (this.minpt.z > b.maxpt.z))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// reset the Bbox (sets initialized=false)
		public void clear()
		{
			initialized = false;
		}

		/// Add a Point to the Bbox.
		/// This enlarges the Bbox so that p is contained within it.
		public void addPoint(Point p)
		{
			if (!initialized)
			{
				maxpt.CopyFrom(p);
				minpt.CopyFrom(p);
				initialized = true;
			}
			else
			{
				if (p.x > maxpt.x)
				{
					maxpt.x = p.x;
				}
				if (p.x < minpt.x)
				{
					minpt.x = p.x;
				}

				if (p.y > maxpt.y)
				{
					maxpt.y = p.y;
				}
				if (p.y < minpt.y)
				{
					minpt.y = p.y;
				}

				if (p.z > maxpt.z)
				{
					maxpt.z = p.z;
				}
				if (p.z < minpt.z)
				{
					minpt.z = p.z;
				}
			}
		}

		/// Add each vertex of a Triangle to the Bbox.
		/// This enlarges the Bbox so that the Triangle is contained within it.
		/// Calls addPoint() for each vertex of the Triangle.

		/// add each vertex of the Triangle      
		public void addTriangle(Triangle t)
		{
			addPoint(t.p[0]);
			addPoint(t.p[1]);
			addPoint(t.p[2]);
			return;
		}

		public static std::ostream operator << (std::ostream stream, Bbox b)
		{
		  stream << " Bbox \n";
		  stream << " min= " << b.minpt << "\n";
		  stream << " max= " << b.maxpt << "\n";
		  return stream;
		}

//DATA
		/// the maximum point
		public Point maxpt = new Point();
		/// the minimum point
		public Point minpt = new Point();
		/// false until one Point or one Triangle has been added
		private bool initialized;
}

} // end namespace
// end file bbox.h


// end of file volume.cpp
