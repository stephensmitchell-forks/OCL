using System;
using System.Diagnostics;

namespace ocl.weave
{
	public static class GlobalMembers
	{
	public static int VertexProps.count = 0;
	}
}

public static class GlobalMembers
{
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



	#if _MSC_VER
	//C++ TO C# CONVERTER TODO TASK: #define macros defined in multiple preprocessor conditionals can only be replaced within the scope of the preprocessor conditional:
	//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
	//ORIGINAL LINE: #define isnan(x) _isnan(x)
		#define isnan // VC++ uses _isnan() instead of isnan()
	#elif WIN32
		public static bool _isnan(double x)
		{
			return false;
		}
	#endif
}

namespace ocl
{
	public static class GlobalMembers
	{

	///
	/// \brief Numeric is a collection of functions for dealing
	/// with the joys of floating-point arithmetic.
	///

	/// return 1 of x>0, return -1 if x<0.
	public static double sign(double x)
	{
		if (x < 0.0)
		{
			return -1;
		}
		else
		{
			return 1;
		}
	}

	/// return x*x
	public static double square(double x)
	{
		return x * x;
	}

	/// return true if x is negative
	public static bool isNegative(double x)
	{
		if (x < 0.0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// return true if x is negative
	public static bool isPositive(double x)
	{
		if (x > 0.0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// return true if x is zero, to within tolerance 
	public static bool isZero_tol(double x)
	{
		if (Math.Abs(x) < DefineConstants.TOLERANCE)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

/// return machine epsilon

	/// returns machine-epsilon
	/// eps is such that 1 < 1 + eps
	/// but 1 == 1 + eps/2 
	public static double eps()
	{
		double r;
		r = 1.0;
		while (1.0 < (1.0 + r))
		{
			r = r / 2.0;
		}
		return (2.0 * r);
	}

	public static double epsD(double x)
	{
		double r;
		r = 1000.0;
		while (x < (x + r))
		{
			r = r / 2.0;
		}
		return (2.0 * r);
	}

	public static float epsF(float x)
	{
		float r;
		r = 1000.0F;
		while (x < (x + r))
		{
			r = r / (float)2.0;
		}
		return ((float)2.0 * r);
	}

	/// assertion with error message
	public static void assert_msg(bool assertion, string message)
	{
		if (!assertion)
		{
			Console.Write(message);
			Debug.Assert(assertion);
		}
	}

/// solve system Ax = y by inverting A
/// x = Ainv * y
/// returns false if det(A)==0, i.e. no solution found

	/// solves 2x2 matrix system Ax=y, solution is x = Ainv * y
	///  [ a  b ] [u] = [ e ]
	///  [ c  d ] [v] = [ f ]

	public static bool two_by_two_solver(double a, double b, double c, double d, double e, double f, ref double u, ref double v)
	{
		//  [ a  b ] [u] = [ e ]
		//  [ c  d ] [v] = [ f ]
		// matrix inverse is
		//          [ d  -b ]
		//  1/det * [ -c  a ]
		//  so
		//  [u]              [ d  -b ] [ e ]
		//  [v]  =  1/det *  [ -c  a ] [ f ]
		double det = a * d - c * b;
		if (isZero_tol(det))
		{
			return false;
		}
		u = (1.0 / det) * (d * e - b * f);
		v = (1.0 / det) * (-c * e + a * f);
		return true;
	}

/// find an intersection point in the XY-plane between two lines
/// first line:   p1 + v*(p2-p1)
/// second line:  p3 + t*(p4-p3)
/// sets (v,t) to the intersection point and returns true if an intersection was found 

	/// returns intersection in XY plane btw. lines p1,p2 and p3,p4
	/// line1 is   p1 + v * (p2-p1)
	/// line2 is   p3 + t * (p4-p3)
	public static bool xy_line_line_intersection(Point p1, Point p2, ref double v, Point p3, Point p4, ref double t)
	{
		// p1 + v*(p2-p1) = p3 + t*(p4-p3)
		// =>
		// [ (p2-p1).x  -(p4-p3).x ] [ v ]  = [ (p3-p1).x ]
		// [ (p2-p1).y  -(p4-p3).y ] [ t ]  = [ (p3-p1).y ]
		return two_by_two_solver((p2 - p1).x, -(p4 - p3).x, (p2 - p1).y, -(p4 - p3).y, (p3 - p1).x, (p3 - p1).y, ref v, ref t);
	}

	/// convert the direction (x,y) into a diangle
	public static double xyVectorToDiangle(double x, double y)
	{
		double diangle;
		if (y >= 0)
		{
			diangle = (x >= 0 ? y / (x + y) : 1 - x / (-x + y));
		}
		else
		{
			diangle = (x < 0 ? 2 - y / (-x - y) : 3 + x / (x - y));
		}
	#if _WIN32
		if ((boost::math.isnan)(diangle) != null)
		{ // Use the Boost version
	#else
		if (double.IsNaN(diangle))
		{ // Use the std version
	#endif
			Console.Write("numeric::xyVectorToDiangle() error (x,y)= (");
			Console.Write(x);
			Console.Write(" , ");
			Console.Write(y);
			Console.Write(" ) and diangle=");
			Console.Write(diangle);
			Console.Write("\n");
			Debug.Assert(false);
		}
		return diangle;
	}

// scalar*Point
/*
	/// scalar multiplication   scalar*Point
	public static Point operator * (double a, Point p)
	{
		return new Point(p) *= a;
	}
    */

		internal static string str_for_Ttc;

		/// consider renaming?
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'str', so pointers on this parameter are left unchanged:
		public static string Ttc(char * str)
		{
			// convert a wchar_t* string into a char* string
			str_for_Ttc = "";
			while (*str)
			{
				str_for_Ttc.push_back((char) * str++);
			}
			return str_for_Ttc;
		}
	public static double brent_zero<ErrObj>(double a, double b, double eps, double t, ErrObj ell)
	{
		// f must have unequal sign at a and b, i.e.
		// f(a)*f(b) < 0
		// returns the location of a root c where f(c)=0 
		// to within 6*eps*abs(c)+2*t tolerance 
		double c;
		double d;
		double e;
		double fa; // function values
		double fb;
		double fc;
		double m;
		double p;
		double q;
		double r;
		double s;
		double tol;
		fa = ell.error(a); // f(a);
		fb = ell.error(b); // f(b);
		if (fa * fb >= 0.0)
		{ // check for opposite signs
			Console.Write(" brent_zero() called with invalid interval [a,b] !\n");
			Debug.Assert(false);
		}
		c = a; // set c sln equal to a sln
		fc = fa;
		e = b - a; // interval width
		d = e; // interval width
		while (true)
		{
			if (Math.Abs(fc) < Math.Abs(fb))
			{ // sln at c is better than at b
				a = b; // a is the old solution
				b = c; // b is the best root so far
				c = a;
				fa = fb;
				fb = fc; // swap so that fb is the best solution
				fc = fa;
			}
			tol = 2.0 * eps * Math.Abs(b) + t;
			m = 0.5 * (c - b); // half of step from c to b sln
			if ((Math.Abs(m) <= tol) || (fb == 0.0)) // end-condition for the infinite loop
			{
				break; // either within tolerance, or found exact zero fb
			}

			if ((Math.Abs(e) < tol) || (Math.Abs(fa) <= Math.Abs(fb)))
			{
				// step from c->b was small, or fa is a better solution
				e = m;
				d = e; // bisection?
			}
			else
			{
				s = fb / fa;
				if (a == c)
				{
					p = 2.0 * m * s;
					q = 1.0 - s;
				}
				else
				{
					q = fa / fc;
					r = fb / fc;
					p = s * (2.0 * m * a * (q - r) - (b - a) * (r - 1.0));
					q = (q - 1.0) * (r - 1.0) * (s - 1.0);
				}

				if (p > 0.0)
				{
					q = -q;
				}
				else
				{
					p = -p; // make p negative
				}

				s = e;
				e = d;
				if ((2.0 * p < (3.0 * m * q - Math.Abs(tol * q))) && (p < Math.Abs(0.5 * s * q)))
				{
					d = p / q;
				}
				else
				{
					e = m;
					d = e;
				}
			}
			a = b; // store the old b-solution in a
			fa = fb;
			if (Math.Abs(d) > tol) // if d is "large"
			{
				b = b + d; // push the root by d
			}
			else if (m > 0.0)
			{
				b = b + tol; // otherwise, push root by tol in direction of m
			}
			else
			{
				b = b - tol;
			}
			// std::cout << " brent_zero b=" << b << "\n";

			fb = ell.error(b); // f(b);

			if (((fb > 0.0) && (fc > 0.0)) || ((fb <= 0.0) && (fc <= 0.0)))
			{
				// fb and fc have the same sign
				c = a; // so change c to a
				fc = fa;
				e = b - a; // interval width
				d = e;
			}
		} // end iteration-loop
		return b;
	}
	}
}

namespace ocl.tsp
{
	public static class GlobalMembers
	{
	public static void connectAllEuclidean< VertexListGraph, PointContainer, WeightMap, VertexIndexMap>(VertexListGraph g, PointContainer points, WeightMap wmap, VertexIndexMap vmap) // Property maps passed by value
	{
		graph_traits<VertexListGraph>.edge_descriptor e = new graph_traits<VertexListGraph>.edge_descriptor();
		bool inserted;
		Tuple<typename graph_traits<VertexListGraph>.vertex_iterator, typename graph_traits<VertexListGraph>.vertex_iterator> verts = new Tuple<typename graph_traits<VertexListGraph>.vertex_iterator, typename graph_traits<VertexListGraph>.vertex_iterator>(vertices(g).Item1, vertices(g).Item2);
		for (typename src = verts.Item1; src != verts.Item2; src++)
		{
			for (typename dest = src; dest != verts.Item2; dest++)
			{
				if (dest != src)
				{
					double weight = Math.Sqrt(Math.Pow((double)(points[vmap[*src]].x - points[vmap[*dest]].x), 2.0) + Math.Pow((double)(points[vmap[*dest]].y - points[vmap[*src]].y), 2.0));
					boost::tie(e, inserted) = add_edge(*src, *dest, g);
					wmap[e] = weight; // passed by value??
				}
			}
		}
	}
	}
}