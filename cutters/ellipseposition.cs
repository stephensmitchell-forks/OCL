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

// uncomment to disable assert() calls
// #define NDEBUG

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
//class Ellipse;
///
/// \brief EllipsePosition defines a position in (s,t) coordinates on a unit-circle.
/// The (s,t) pair is used to locate points on an ellipse.
/// 
/// s^2 + t^2 = 1 should be true at all times.
public class EllipsePosition
{
		/// create an EllipsePosition

		//********   EllipsePosition ********************** */
		public EllipsePosition()
		{
			diangle = 0.0;
			setD();
		}

		/// create EllipsePosition at (s,t)
		public EllipsePosition(double sin, double tin)
		{
			s = sin;
			t = tin;
		}
		/// set (s,t) pair to the position corresponding to diangle
		public void setDiangle(double dia)
		{
			Debug.Assert(!double.IsNaN(dia));
			diangle = dia;
			setD();
		}

		/// set rhs EllipsePosition (s,t) values equal to lhs EllipsePosition
//C++ TO C# CONVERTER NOTE: This 'CopyFrom' method was converted from the original copy assignment operator:
//ORIGINAL LINE: EllipsePosition& operator =(const EllipsePosition &pos)
		public EllipsePosition CopyFrom(EllipsePosition pos)
		{
			s = pos.s;
			t = pos.t;
			diangle = pos.diangle;
			return this;
		}

		/// return true if (s,t) is valid, i.e. lies on the unit circle
		/// checks s^2 + t^2 == 1  (to within tolerance) 

		// check that s and t values are OK
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool isValid() const
		public bool isValid()
		{
			if (GlobalMembers.isZero_tol(ocl.GlobalMembers.square(s) + ocl.GlobalMembers.square(t) - 1.0))
			{
				return true;
			}
			else
			{
				Console.Write(" EllipsePosition=");
				Console.Write(this);
				Console.Write("\n");
				Console.Write(" square(s) + square(t) - 1.0 = ");
				Console.Write(ocl.GlobalMembers.square(s) + ocl.GlobalMembers.square(t) - 1.0);
				Console.Write(" !!\n");
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
			o << this;
			return o.str();
		}
        
		/// string repr
		public static std::ostream operator << (std::ostream stream, EllipsePosition pos)
		{
			stream << "(" << pos.s << " ," << pos.t << ")";
			return stream;
		}
        */
	// DATA
		/// s-parameter in [-1, 1]
		public double s;
		/// t-parameter in [-1, 1]
		public double t;
		/// diamond angle parameter in [0,4] (modulo 4)
		/// this models an angle [0,2pi] and maps 
		/// from the angle to an (s,t) pair using setD()
		public double diangle;

		/// set (s,t) pair to match diangle
		private void setD()
		{
			// set (s,t) to angle corresponding to diangle
			// see: http://www.freesteel.co.uk/wpblog/2009/06/encoding-2d-angles-without-trigonometry/
			// see: http://www.anderswallin.net/2010/07/radians-vs-diamondangle/
			// return P2( (a < 2 ? 1-a : a-3),
			//           (a < 3 ? ((a > 1) ? 2-a : a) : a-4)
			double d = diangle;
			Debug.Assert(!double.IsNaN(d));
			while (d > 4.0) // make d a diangle in [0,4]
			{
				d -= 4.0;
			}
			while (d < 0.0)
			{
				d += 4.0;
			}

			Debug.Assert(d >= 0.0 && d <= 4.0); // now we should be in [0,4]
			Point p = new Point((d < 2 ? 1 - d : d - 3), (d < 3 ? ((d > 1) ? 2 - d : d) : d - 4));

			// now we have a vector pointing in the right direction
			// but it is not normalized
			p.normalize();
			s = p.x;
			t = p.y;
			Debug.Assert(this.isValid());
		}
}

} // end namespace
// end file EllipsePosition.h

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)
//end file ellipseposition.cpp
