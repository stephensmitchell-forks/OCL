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
/// \brief Cutter-Location (CL) point.
///
public class CLPoint : Point
{
		/// CLPoint at (0,0,0)

		/* ********************************************** CLPoint *************/

		public CLPoint() : base()
		{
			cc = new CCPoint();
		}

		/// CLPoint at (x,y,z)
		public CLPoint(double x, double y, double z) : base(x, y, z)
		{
			cc = new CCPoint();
		}

		/// CLPoint at (x,y,z) with CCPoint ccp
		public CLPoint(double x, double y, double z, CCPoint ccp) : base(x, y, z)
		{
			cc = new CCPoint(ccp);
		}

		/// copy constructor
		public CLPoint(CLPoint cl) : base(cl.x, cl.y, cl.z)
		{
			cc = new CCPoint(cl.cc);
		}

		/// cl-point at Point p
		public CLPoint(Point p) : base(p.x, p.y, p.z)
		{
			cc = new CCPoint();
		}

		public override void Dispose()
		{
		   if (cc != null)
		   {
			   cc.Dispose();
		   }
			base.Dispose();
		}

		/// Pointer to the corresponding CCPoint
		public CCPoint cc;
        /*
		/// string repr
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const
		public new string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << "CL(" << x << ", " << y << ", " << z << ") cc=" << cc;
			return o.str();
		}
        */
		/// if cc is in the edge p1-p2, test if clpoint needs to be lifted to z
		/// if so, set cc = cc_tmp and return true
		public bool liftZ_if_InsidePoints(double zin, CCPoint cc_tmp, Point p1, Point p2)
		{
			if (cc_tmp.isInside(p1, p2))
			{
				return this.liftZ(zin, cc_tmp);
			}
			return false;
		}

		/// if cc in in Triangle facet, test if clpoint needs to be lifted
		/// if so, set cc=cc_tmp and return true
		public bool liftZ_if_inFacet(double zin, CCPoint cc_tmp, Triangle t)
		{
			if (cc_tmp.isInside(t))
			{
				return this.liftZ(zin, cc_tmp);
			}
			return false;
		}

		/// if zin > z, lift CLPoint and update cc-point, and return true 
		public bool liftZ(double zin, CCPoint ccp)
		{
			if (zin > z)
			{
				z = zin;
				if (cc != null)
				{
					if (cc != null)
					{
						cc.Dispose();
					}
				}
				cc = new CCPoint(ccp);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// if zin > z, lift CLPoint and return true.
		public bool liftZ(double zin)
		{
			if (zin > z)
			{
				z = zin;
				return true;
			}
			else
			{
				return false;
			}
		}


		/// return true if cl-point above triangle
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool below(const Triangle& t) const
		public bool below(Triangle t)
		{
			if (z < t.bb.maxpt.z)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// return the CCPoint (for python)
		public CCPoint getCC()
		{
			return cc;
		}

		/// assignment
//C++ TO C# CONVERTER NOTE: This 'CopyFrom' method was converted from the original copy assignment operator:
//ORIGINAL LINE: CLPoint& operator =(const CLPoint &clp)
		public CLPoint CopyFrom(CLPoint clp)
		{
			if (this == clp) // check for self-assignment
			{
				return this;
			}
			x = clp.x;
			y = clp.y;
			z = clp.z;
			cc = new CCPoint((clp.cc));
			return this;
		}

		/// addition
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: const CLPoint operator +(const CLPoint &p) const
		public static CLPoint operator + (CLPoint ImpliedObject, CLPoint p)
		{
			return new CLPoint(ImpliedObject.x + p.x, ImpliedObject.y + p.y, ImpliedObject.z + p.z);
		}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: const CLPoint operator +(const Point &p) const
		public static new CLPoint operator + (CLPoint ImpliedObject, Point p)
		{
			return new CLPoint(ImpliedObject.x + p.x, ImpliedObject.y + p.y, ImpliedObject.z + p.z);
		}
}

} // end namespace
// end file clpoint.h


// end file clpoint.cpp
