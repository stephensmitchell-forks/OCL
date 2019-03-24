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



namespace ocl
{

/// a fiber is an infinite line in space along which the cutter can be pushed
/// into contact with a triangle. A Weave is built from many X-fibers and Y-fibers.
/// might be called a Dexel also in some papers/textbooks.
public class Fiber : System.IDisposable
{
		public Fiber()
		{
			ints.Clear();
		}
		/// create a Fiber between points p1 and p2
		public Fiber(Point p1in, Point p2in)
		{
			p1.CopyFrom(p1in);
			p2.CopyFrom(p2in);
			calcDir();
		}

		public virtual void Dispose()
		{
		}
		/// add an interval to this Fiber
		public void addInterval(Interval i)
		{
			if (i.empty())
			{
				return; // do nothing.
			}

			if (ints.Count == 0)
			{ // empty fiber case
				ints.Add(i);
				return;
			}
			else if (this.contains(i))
			{ // if fiber already contains i
				return; // do nothing
			}
			else if (this.missing(i))
			{ // if fiber doesn't contain i
				ints.Add(i);
				return;
			}
			else
			{
				// this is the messier general case with partial overlap
				List<Interval>.Enumerator itr;
				itr = ints.GetEnumerator();
				List<Interval> overlaps = new List<Interval>();
				while (itr.MoveNext())
				{ // loop through all intervals
					if (!(itr.outside(i)))
					{
						overlaps.push_backitr.Current; // add overlaps here
//C++ TO C# CONVERTER TODO TASK: There is no direct equivalent to the STL vector 'erase' method in C#:
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: itr = ints.erase(itr);
						itr.CopyFrom(ints.erase(itr)); // erase overlaps from ints
					}
					else
					{
					}
				}
				overlaps.Add(i);
				// now build a new interval from i and the overlaps
				Interval sumint = new Interval();
				foreach (Interval intr in overlaps)
				{
					sumint.updateLower(intr.lower, intr.lower_cc);
					sumint.updateUpper(intr.upper, intr.upper_cc);
				}
				ints.Add(sumint); // add the sum-interval to ints
				return;
			}
		}

		/// return true if Fiber already has interval i in it
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool contains(Interval& i) const
		public bool contains(Interval i)
		{
			foreach (Interval fi in ints)
			{
				if (i.inside(fi))
				{
					return true;
				}
			}
			return false;
		}

		/// return true if Interval i is completely missing (no overlaps) from Fiber
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool missing(Interval& i) const
		public bool missing(Interval i)
		{
			bool result = true;
			foreach (Interval fi in ints)
			{
				if (!i.outside(fi)) // all existing ints must be non-overlapping
				{
					result = false;
				}
			}
			return result;
		}

		/// t-value corresponding to Point p
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double tval(Point& p) const
		public double tval(Point p)
		{
			// fiber is  f = p1 + t * (p2-p1)
			// t = (f-p1).dot(p2-p1) / (p2-p1).dot(p2-p1)
			return (p - p1).dot(p2 - p1) / (p2 - p1).dot(p2 - p1);
		}

		/// Point corresponding to t-value
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point point(double t) const
		public Point point(double t)
		{
			Point p = p1 + t * (p2 - p1);
			return new ocl.Point(p);
		}

		/// print the intervals
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: void printInts() const
		public void printInts()
		{
			int n = 0;
			foreach (Interval i in ints)
			{
				Console.Write(n);
				Console.Write(": [ ");
				Console.Write(i.lower);
				Console.Write(" , ");
				Console.Write(i.upper);
				Console.Write(" ]");
				Console.Write("\n");
				++n;
			}
		}

		/// return true if the Fiber contains no intervals
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool empty() const
		public bool empty()
		{
			return ints.Count == 0;
		}
		/// return number of intervals
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint size() const
		public uint size()
		{
			return (uint)ints.Count;
		}

		/// return the upper cl-point of interval n
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point upperCLPoint(uint n) const
		public Point upperCLPoint(uint n)
		{
			return new ocl.Point(point(ints[n].upper));
		}
		/// return the lower cl-point of interval n
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point lowerCLPoint(uint n) const
		public Point lowerCLPoint(uint n)
		{
			return new ocl.Point(point(ints[n].lower));
		}

		/// string repr
		public static std::ostream operator << (std::ostream stream, Fiber f)
		{
		  stream << " fiber dir=" << f.dir << " and " << f.ints.Count << " intervals\n";
		  stream << " fiber.p1=" << f.p1 << " fiber.p2 " << f.p2;
		  return stream;
		}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool operator ==(const Fiber& other) const
		public static bool operator == (Fiber ImpliedObject, Fiber other)
		{
			if ((ImpliedObject.p1 == other.p1) && (ImpliedObject.p2 == other.p2))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

	// DATA
		public Point p1 = new Point(); ///< start point
		public Point p2 = new Point(); ///< end point
		public Point dir = new Point(); ///< direction vector (normalized)
		public List<Interval> ints = new List<Interval>(); ///< the intervals in this Fiber
		/// set the direction(tangent) vector
		protected void calcDir()
		{
			dir.CopyFrom(p2 - p1);
			Debug.Assert(dir.z == 0.0);
			dir.normalize();
		}
}

} // end namespace
// end file fiber.h


// end file fiber.cpp
