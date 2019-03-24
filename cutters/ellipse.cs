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
//class Fiber;

/// An Ellipse. 
public class Ellipse
{
		/// dummy constructor
		public Ellipse()
		{
		}
		/// create an Ellipse with centerpoint center, X-axis a, Y-axis b, and offset distance offset.

		//********   Ellipse ********************** */
		public Ellipse(Point centerin, double ain, double bin, double offsetin)
		{
			center.CopyFrom(centerin);
			a = ain;
			b = bin;
			Debug.Assert(b > 0.0);
			eccen = a / b;
			offset = offsetin;
			target = new Point(0, 0, 0);
		}

		/// return a point on the ellipse at given EllipsePosition
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual Point ePoint(const EllipsePosition& pos) const
		public virtual Point ePoint(EllipsePosition pos)
		{
			// (s, t) where:  s^2 + t^2 = 1
			// a and b are the orthogonal axes of the ellipse
			// point of ellipse is:  center + a s + b t               s=cos(theta) t=sin(theta)
			// tangent at point is:  -a t + b s
			// normal at point is:    b s + a t
			// point on offset-ellipse:  point on ellipse + offset*normal
			Point p = new Point(center);
			p.x += a * pos.s; // a is in X-direction
			p.y += b * pos.t; // b is in Y-direction
			return new ocl.Point(p);
		}

		/// return a point on the offset-ellipse at given EllipsePosition
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual Point oePoint(const EllipsePosition& pos) const
		public virtual Point oePoint(EllipsePosition pos)
		{
			return ePoint(pos) + offset * normal(pos); // offset-point  = ellipse-point + offset*normal
		}

		/// return a normalized normal vector of the ellipse at the given EllipsePosition
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual Point normal(const EllipsePosition& pos) const
		public virtual Point normal(EllipsePosition pos)
		{
			Debug.Assert(pos.isValid());
			Point n = new Point(b * pos.s, a * pos.t, 0);
			n.normalize();
			return new ocl.Point(n);
		}

		/// offset-ellipse Brent solver

		/// offfset-ellipse solver using Brent's method
		/// find the EllipsePosition that makes the offset-ellipse point be at p
		/// this is a zero of Ellipse::error()
		/// returns number of iterations
		///
		/// called (only?) by BullCutter::singleEdgeDropCanonical()   
		public int solver_brent()
		{
			int iters = 1;
			EllipsePosition apos = new EllipsePosition(); // Brent's method requires bracketing the root in [apos.diangle, bpos.diangle]
			EllipsePosition bpos = new EllipsePosition();
			apos.setDiangle(0.0);
			Debug.Assert(apos.isValid());
			bpos.setDiangle(3.0);
			Debug.Assert(bpos.isValid());
			if (Math.Abs(error(apos)) < DefineConstants.OE_ERROR_TOLERANCE)
			{ // if we are lucky apos is the solution
				EllipsePosition1.CopyFrom(apos); // and we do not need to search further.
				find_EllipsePosition2();
				return iters;
			}
			else if (Math.Abs(error(bpos)) < DefineConstants.OE_ERROR_TOLERANCE)
			{ // or bpos might be the solution?
				EllipsePosition1.CopyFrom(bpos);
				find_EllipsePosition2();
				return iters;
			}
			// neither apos nor bpos is the solution
			// but root is now bracketed, so we can use brent_zero
			Debug.Assert(error(apos) * error(bpos) < 0.0);
			// this looks for the diangle that makes the offset-ellipse point y-coordinate zero
			double dia_sln = ocl.GlobalMembers.brent_zero(apos.diangle, bpos.diangle, 3E-16, DefineConstants.OE_ERROR_TOLERANCE, this); // brent_zero.hpp
			EllipsePosition1.setDiangle(dia_sln);
			Debug.Assert(EllipsePosition1.isValid());
			// because we only work with the y-coordinate of the offset-ellipse-point, there are two symmetric solutions
			find_EllipsePosition2();
			return iters;
		}

		/// print out the found solutions
		public void print_solutions()
		{
			Console.Write("1st: (s, t)= ");
			Console.Write(EllipsePosition1);
			Console.Write(" oePoint()= ");
			Console.Write(oePoint(EllipsePosition1));
			Console.Write(" e=");
			Console.Write(error(EllipsePosition1));
			Console.Write("\n");
			Console.Write("2nd: (s, t)= ");
			Console.Write(EllipsePosition2);
			Console.Write(" oePoint()= ");
			Console.Write(oePoint(EllipsePosition2));
			Console.Write(" e=");
			Console.Write(error(EllipsePosition2));
			Console.Write("\n");
		}

		/// given one EllipsePosition solution, find the other.
		// #define DEBUG_SOLVER
		//
		// given a known EllipsePosition1, look for the other symmetric
		// solution EllipsePosition2
		public bool find_EllipsePosition2()
		{ // a horrible horrible function... :(
			Debug.Assert(EllipsePosition1.isValid());
			double err1 = Math.Abs(this.error(this.EllipsePosition1));
			this.EllipsePosition2.s = this.EllipsePosition1.s; // plus
			this.EllipsePosition2.t = -this.EllipsePosition1.t; // minus
			if (Math.Abs(this.error(this.EllipsePosition2)) < err1 + DefineConstants.OE_ERROR_TOLERANCE)
			{
				if ((Math.Abs(this.EllipsePosition2.s - this.EllipsePosition1.s) > 1E-8) || (Math.Abs(this.EllipsePosition2.t - this.EllipsePosition1.t) > 1E-8))
				{
					#if DEBUG_SOLVER
						Console.Write("2nd: (s, t)= ");
						Console.Write(this.EllipsePosition2);
						Console.Write(" oePoint()= ");
						Console.Write(this.oePoint(this.EllipsePosition2));
						Console.Write(" e=");
						Console.Write(this.error(this.EllipsePosition2));
						Console.Write("\n");
					#endif
					return true;
				}
			}

			this.EllipsePosition2.s = -this.EllipsePosition1.s;
			this.EllipsePosition2.t = this.EllipsePosition1.t;
			if (Math.Abs(this.error(this.EllipsePosition2)) < err1 + DefineConstants.OE_ERROR_TOLERANCE)
			{
				if ((Math.Abs(this.EllipsePosition2.s - this.EllipsePosition1.s) > 1E-8) || (Math.Abs(this.EllipsePosition2.t - this.EllipsePosition1.t) > 1E-8))
				{
					#if DEBUG_SOLVER
						Console.Write("2nd: (s, t)= ");
						Console.Write(this.EllipsePosition2);
						Console.Write(" oePoint()= ");
						Console.Write(this.oePoint(this.EllipsePosition2));
						Console.Write(" e=");
						Console.Write(this.error(this.EllipsePosition2));
						Console.Write("\n");
					#endif
					return true;
				}
			}

			this.EllipsePosition2.s = -this.EllipsePosition1.s;
			this.EllipsePosition2.t = -this.EllipsePosition1.t;
			if (Math.Abs(this.error(this.EllipsePosition2)) < err1 + DefineConstants.OE_ERROR_TOLERANCE)
			{
				if ((Math.Abs(this.EllipsePosition2.s - this.EllipsePosition1.s) > 1E-8) || (Math.Abs(this.EllipsePosition2.t - this.EllipsePosition1.t) > 1E-8))
				{
					#if DEBUG_SOLVER
						Console.Write("2nd: (s, t)= ");
						Console.Write(this.EllipsePosition2);
						Console.Write(" oePoint()= ");
						Console.Write(this.oePoint(this.EllipsePosition2));
						Console.Write(" e=");
						Console.Write(this.error(this.EllipsePosition2));
						Console.Write("\n");
					#endif
					return true;
				}
			}

			// last desperate attempt is identical solutions
			this.EllipsePosition2.s = this.EllipsePosition1.s;
			this.EllipsePosition2.t = this.EllipsePosition1.t;
			if (Math.Abs(this.error(this.EllipsePosition2)) < err1 + DefineConstants.OE_ERROR_TOLERANCE)
			{
				// DON'T require solutions to differ
				// if ( (fabs(this->EllipsePosition2.s - this->EllipsePosition1.s) > 1E-8) || (fabs(this->EllipsePosition2.t - this->EllipsePosition1.t) > 1E-8) ) {
					#if DEBUG_SOLVER
						Console.Write("2nd: (s, t)= ");
						Console.Write(this.EllipsePosition2);
						Console.Write(" oePoint()= ");
						Console.Write(this.oePoint(this.EllipsePosition2));
						Console.Write(" e=");
						Console.Write(this.error(this.EllipsePosition2));
						Console.Write("\n");
					#endif
					//std::cout << " find_EllipsePosition2() desperate-mode\n";
					return true;
				//}
			}
			Console.Write("Ellipse::find_EllipsePosition2 cannot find EllipsePosition2! \n");
			Console.Write("ellipse= ");
			Console.Write(this);
			Console.Write("\n");
			print_solutions();
			Debug.Assert(0); // serious error if we get here!

			return false;
		}

		/// error function for the solver
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double error(EllipsePosition& pos) const
		public double error(EllipsePosition pos)
		{
			Point p1 = oePoint(pos);
			return p1.y;
		}

		/// error function for solver
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual double error(double diangle) const
		public virtual double error(double diangle)
		{
			EllipsePosition tmp = new EllipsePosition();
			tmp.setDiangle(diangle);
			return error(tmp);
		}

		/// calculate ellipse center

		/// given the two solutions EllipsePosition1 and EllipsePosition2 and the edge up1-up2
		/// locate the ellipse center correctly
		public Point calcEcenter(Point up1, Point up2, int sln)
		{
			Point cle = (sln == 1 ? oePoint1() : oePoint2());
			double xoffset = - cle.x;
			// x-coord on line is  x = up1.x + t*(up2.x-up1.x) = center.x+offset
			double t = (center.x + xoffset - up1.x) / (up2.x - up1.x);
			return up1 + t * (up2 - up1); // return a point on the line
		}

		/// set EllipsePosition_hi to either EllipsePosition1 or EllipsePosition2, depending on which
		/// has the center (given by calcEcenter() ) with higher z-coordinate  
		public void setEllipsePositionHi(Point u1, Point u2)
		{
			Point ecen1 = calcEcenter(u1, u2, 1);
			Point ecen2 = calcEcenter(u1, u2, 2);
			if (ecen1.z >= ecen2.z)
			{ // we want the higher center
				EllipsePosition_hi.CopyFrom(EllipsePosition1);
				center.CopyFrom(ecen1);
			}
			else
			{
				EllipsePosition_hi.CopyFrom(EllipsePosition2);
				center.CopyFrom(ecen2);
			}
		}

		/// once EllipsePosition_hi is set, return an ellipse-point at this position
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point ePointHi() const
		public Point ePointHi()
		{
			return new ocl.Point(ePoint(EllipsePosition_hi));
		}

		/// ellipse-point at EllipsePosition1
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point ePoint1() const
		public Point ePoint1()
		{
			return new ocl.Point(this.ePoint(EllipsePosition1));
		}

		/// ellipse-point at EllipsePosition2
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point ePoint2() const
		public Point ePoint2()
		{
			return new ocl.Point(this.ePoint(EllipsePosition2));
		}

		/// offset-ellipse-point at EllipsePosition1
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point oePoint1() const
		public Point oePoint1()
		{
			return new ocl.Point(this.oePoint(EllipsePosition1));
		}

		/// offset-ellipse-point at EllipsePosition2
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point oePoint2() const
		public Point oePoint2()
		{
			return new ocl.Point(this.oePoint(EllipsePosition2));
		}


		/// string repr

		/// Ellipse string output
		public static std::ostream operator << (std::ostream stream, Ellipse e)
		{
		  stream << "Ellipse: cen=" << e.center << " a=" << e.a << " b=" << e.b << " ofs=" << e.offset;
		  return stream;
		}

		/// set length of ellipse major axis
		public void setA(double ain)
		{
			a = ain;
		}
		/// set length of ellipse minor axis
		public void setB(double bin)
		{
			b = bin;
		}
		/// set the ellipse center
		public void setCenter(Point pin)
		{
			center.CopyFrom(pin);
		}
		/// set offset-ellipse offset distance
		public void setOffset(double ofs)
		{
			offset = ofs;
		}
		/// set/calculate the eccentricity
		public void setEccen()
		{
			eccen = a / b;
		}
		/// returns the z-coordinate of this->center
		public double getCenterZ()
		{
			return center.z;
		}

		/// eccentricity = a/b
		public double eccen;

		/// first EllipsePosition solution found by solver()
		protected EllipsePosition EllipsePosition1 = new EllipsePosition();
		/// second EllipsePosition solution found by solver()
		protected EllipsePosition EllipsePosition2 = new EllipsePosition();
		/// the higher EllipsePosition solution
		protected EllipsePosition EllipsePosition_hi = new EllipsePosition();

		/// the center point of the ellipse
		protected Point center = new Point();
		/// a-axis, in the X-direction
		protected double a;
		/// b-axis, in the Y-direction
		protected double b;
		/// offset
		protected double offset;
		/// the target Point for the error-function
		protected Point target = new Point();
}

/// an aligned ellipse, used by the edgePush function of BullCutter
public class AlignedEllipse : Ellipse
{
		public AlignedEllipse()
		{
		}
		/// create an aligned ellipse
		public AlignedEllipse(Point centerin, double ain, double bin, double offsetin, Point major, Point minor)
		{
			center.CopyFrom(centerin);
			a = ain; // major axis of ellipse
			b = bin; // minor axis of ellipse
			Debug.Assert(b > 0.0);
			eccen = a / b;
			offset = offsetin;
			major_dir.CopyFrom(major);
			minor_dir.CopyFrom(minor);
		}

		/// normal vector at given EllipsePosition
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point normal(const EllipsePosition& pos) const
		public new Point normal(EllipsePosition pos)
		{
			// normal at point is:    b s + a t
			Point n = pos.s * b * major_dir + pos.t * a * minor_dir;
			n.normalize();
			return new ocl.Point(n);
		}

		/// ellipse-point at given EllipsePosition
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point ePoint(const EllipsePosition& pos) const
		public new Point ePoint(EllipsePosition pos)
		{
			Point p = center + a * pos.s * major_dir + b * pos.t * minor_dir; // point of ellipse is:  center + a s + b t
			return new ocl.Point(p);
		}

		/// offset-ellipse point at given EllipsePosition
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point oePoint(const EllipsePosition& pos) const
		public new Point oePoint(EllipsePosition pos)
		{
			return ePoint(pos) + offset * normal(pos); // offset-point  = ellipse-point + offset*normal
		}

		/// error-function for the solver
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double error(double diangle) const
		public new double error(double diangle)
		{
			EllipsePosition tmp = new EllipsePosition();
			tmp.setDiangle(diangle);
			Point p = this.oePoint(tmp);
			Point errorVec = target - p;
			return errorVec.dot(error_dir);
		}

		/// aligned offset-ellipse solver. callsn Numeric::brent_solver()

		// used by BullCutter pushcutter edge-test
		public bool aligned_solver(Fiber f)
		{
			error_dir.CopyFrom(f.dir.xyPerp()); // now calls to error(diangle) will give the right error
			Debug.Assert(error_dir.xyNorm() > 0.0);
			target.CopyFrom(f.p1); // target is either x or y-coord of f.p1
			// find position(s) where ellipse tangent is parallel to fiber. Here error() will be minimized/maximized.
			// tangent at point is:  -a t + b s = -a*major_dir*t + b*minor_dir*s
			// -at ma.y + bs mi.y = 0   for X-fiber
			// s = sqrt(1-t^2)
			//  -a*ma.y * t + b*mi.y* sqrt(1-t^2) = 0
			//  =>  t^2 = b^2 / (a^2 + b^2)
			double t1 = 0.0;
			if (f.p1.y == f.p2.y)
			{
				t1 = Math.Sqrt(ocl.GlobalMembers.square(b * minor_dir.y) / (ocl.GlobalMembers.square(a * major_dir.y) + ocl.GlobalMembers.square(b * minor_dir.y)));
			}
			else if (f.p1.x == f.p2.x)
			{
				t1 = Math.Sqrt(ocl.GlobalMembers.square(b * minor_dir.x) / (ocl.GlobalMembers.square(a * major_dir.x) + ocl.GlobalMembers.square(b * minor_dir.x)));
			}
			else
			{
				Debug.Assert(0);
			}
			// bracket root
			EllipsePosition tmp = new EllipsePosition();
			EllipsePosition apos = new EllipsePosition();
			EllipsePosition bpos = new EllipsePosition();
			double s1 = Math.Sqrt(1.0 - ocl.GlobalMembers.square(t1));
			bool found_positive = false;
			bool found_negative = false;
			tmp.setDiangle(xyVectorToDiangle(s1,t1));
			if (error(tmp.diangle) > 0)
			{
				found_positive = true;
				apos.CopyFrom(tmp);
			}
			else if (error(tmp.diangle) < 0)
			{
				found_negative = true;
				bpos.CopyFrom(tmp);
			}
			tmp.setDiangle(xyVectorToDiangle(s1,-t1));
			if (error(tmp.diangle) > 0)
			{
				found_positive = true;
				apos.CopyFrom(tmp);
			}
			else if (error(tmp.diangle) < 0)
			{
				found_negative = true;
				bpos.CopyFrom(tmp);
			}
			tmp.setDiangle(xyVectorToDiangle(-s1,t1));
			if (error(tmp.diangle) > 0)
			{
				found_positive = true;
				apos.CopyFrom(tmp);
			}
			else if (error(tmp.diangle) < 0)
			{
				found_negative = true;
				bpos.CopyFrom(tmp);
			}
			tmp.setDiangle(xyVectorToDiangle(-s1,-t1));
			if (error(tmp.diangle) > 0)
			{
				found_positive = true;
				apos.CopyFrom(tmp);
			}
			else if (error(tmp.diangle) < 0)
			{
				found_negative = true;
				bpos.CopyFrom(tmp);
			}

			if (found_positive)
			{
				if (found_negative)
				{
					Debug.Assert(this.error(apos.diangle) * this.error(bpos.diangle) < 0.0); // root is now bracketed.
					double lolim = 0.0;
					double hilim = 0.0;
					if (apos.diangle > bpos.diangle)
					{
						lolim = bpos.diangle;
						hilim = apos.diangle;
					}
					else if (bpos.diangle > apos.diangle)
					{
						hilim = bpos.diangle;
						lolim = apos.diangle;
					}
					double dia_sln = ocl.GlobalMembers.brent_zero(lolim, hilim, 3E-16, DefineConstants.OE_ERROR_TOLERANCE, this);
					double dia_sln2 = ocl.GlobalMembers.brent_zero(hilim - 4.0, lolim, 3E-16, DefineConstants.OE_ERROR_TOLERANCE, this);

					EllipsePosition1.setDiangle(dia_sln);
					EllipsePosition2.setDiangle(dia_sln2);

					Debug.Assert(EllipsePosition1.isValid());
					Debug.Assert(EllipsePosition2.isValid());
					/*
					// FIXME. This assert fails in some cases (30sphere.stl z=0, for example)
					// FIXME. The allowed error should probably be in proportion to the difficulty of the case.
					
					if (!isZero_tol( error(EllipsePosition1.diangle) )) {
					    std::cout << "AlignedEllipse::aligned_solver() ERROR \n";
					    std::cout << "error(EllipsePosition1.diangle)= "<< error(EllipsePosition1.diangle) << " (expected zero)\n";
					
					}
					assert( isZero_tol( error(EllipsePosition1.diangle) ) );
					assert( isZero_tol( error(EllipsePosition2.diangle) ) );
					*/

					return true;
				}
			}

			return false;
		}

		/// direction of the major axis
		private Point major_dir = new Point();
		/// direction of the minor axis
		private Point minor_dir = new Point();
		/// the error-direction for error()
		private Point error_dir = new Point();
}

} // end namespace
// end file ellipse.h

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)

// end of file oellipse.cpp
