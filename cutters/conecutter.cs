using System;
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

/// \brief Conical MillingCutter 
///
/// cone defined by diameter and the cone half-angle(in radians). sharp tip. 
/// 60 degrees or 90 degrees are common
public class ConeCutter : MillingCutter
{
		public ConeCutter()
		{
			Debug.Assert(false);
		}

		/// create a ConeCutter with specified maximum diameter and cone-angle
		/// for a 90-degree cone specify the half-angle  angle= pi/4
		public ConeCutter(double d, double a, double l = 10)
		{
			diameter = d;
			radius = d / 2.0;
			angle = a;
			Debug.Assert(angle > 0.0);
			length = radius / Math.Tan(angle) + l;
			center_height = radius / Math.Tan(angle);
			Debug.Assert(center_height > 0.0);
			xy_normal_length = radius;
			normal_length = 0.0;
		}

		/// offset of ConeCutter is BallConeCutter (should be Ball-Cone-Bull??)

		// ?? Ball-Cone-Bull ??
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: MillingCutter* offsetCutter(double d) const
		public new MillingCutter offsetCutter(double d)
		{
			return new BallConeCutter(2 * d, diameter + 2 * d, angle);
		}

		/// Cone facet-drop is special, since we can make contact with either the tip or the circular rim

		// because this checks for contact with both the tip and the circular edge it is hard to move to the base-class
		// we either hit the tip, when the slope of the plane is smaller than angle
		// or when the slope is steep, the circular edge between the cone and the cylindrical shaft
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool facetDrop(CLPoint &cl, const Triangle &t) const
		public new bool facetDrop(CLPoint cl, Triangle t)
		{
			bool result = false;
			Point normal = t.upNormal(); // facet surface normal
			if (GlobalMembers.isZero_tol(normal.z)) // vertical surface
			{
				return false; //can't drop against vertical surface
			}

			if ((GlobalMembers.isZero_tol(normal.x)) && (GlobalMembers.isZero_tol(normal.y)))
			{ // horizontal plane special case
				CCPoint cc_tmp = new CCPoint(cl.x, cl.y, t.p[0].z, CCType.FACET_TIP); // so any vertex is at the correct height
				return cl.liftZ_if_inFacet(cc_tmp.z, cc_tmp, t);
			}
			else
			{
				// define plane containing facet
				// a*x + b*y + c*z + d = 0, so
				// d = -a*x - b*y - c*z, where  (a,b,c) = surface normal
				double a = normal.x;
				double b = normal.y;
				double c = normal.z;
				double d = - normal.dot(t.p[0]);
				normal.xyNormalize(); // make xy length of normal == 1.0
				// cylindrical contact point case
				// find the xy-coordinates of the cc-point
				CCPoint cyl_cc_tmp = cl - radius * normal;
				cyl_cc_tmp.z = (1.0 / c) * (-d - a * cyl_cc_tmp.x - b * cyl_cc_tmp.y);
				double cyl_cl_z = cyl_cc_tmp.z - length; // tip positioned here
				cyl_cc_tmp.type = CCType.FACET_CYL;

				// tip contact with facet
				CCPoint tip_cc_tmp = new CCPoint(cl.x, cl.y, 0.0);
				tip_cc_tmp.z = (1.0 / c) * (-d - a * tip_cc_tmp.x - b * tip_cc_tmp.y);
				double tip_cl_z = tip_cc_tmp.z;
				tip_cc_tmp.type = CCType.FACET_TIP;

				result = result || cl.liftZ_if_inFacet(tip_cl_z, tip_cc_tmp, t);
				result = result || cl.liftZ_if_inFacet(cyl_cl_z, cyl_cc_tmp, t);
				return result;
			}
		}
        /*
		/// string repr
		public static std::ostream operator << (std::ostream stream, ConeCutter c)
		{
		  stream << "ConeCutter (d=" << c.diameter << ", angle=" << c.angle << ", L=" << c.length << ")";
		  return stream;
		}
        
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const
		public new string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << this;
			return o.str();
		}
        */

		// cone sliced with vertical plane results in a hyperbola as the intersection curve
		// find point where hyperbola and line slopes match
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: System.Tuple< double, double > singleEdgeDropCanonical(const Point& u1, const Point& u2) const
		protected new Tuple< double, double > singleEdgeDropCanonical(Point u1, Point u2)
		{
			double d = u1.y;
			double m = (u2.z - u1.z) / (u2.x - u1.x); // slope of edge
			// the outermost point on the cutter is at   xu = sqrt( R^2 - d^2 )
			double xu = Math.Sqrt(ocl.GlobalMembers.square(radius) - ocl.GlobalMembers.square(u1.y));
			Debug.Assert(xu <= radius);
			// max slope at xu is mu = (L/(R-R2)) * xu /(sqrt( xu^2 + d^2 ))
			double mu = (center_height / radius) * xu / Math.Sqrt(ocl.GlobalMembers.square(xu) + ocl.GlobalMembers.square(d));
			bool hyperbola_case = (Math.Abs(m) <= Math.Abs(mu));
			// find contact point where slopes match, there are two cases:
			// 1) if abs(m) <= abs(mu)  we contact the curve at xp = sign(m) * sqrt( R^2 m^2 d^2 / (h^2 - R^2 m^2) )
			// 2) if abs(m) > abs(mu) there is contact with the circular edge at +/- xu
			double ccu;
			if (hyperbola_case)
			{
				ccu = sign(m) * Math.Sqrt(ocl.GlobalMembers.square(radius) * ocl.GlobalMembers.square(m) * ocl.GlobalMembers.square(d) / (ocl.GlobalMembers.square(length) - ocl.GlobalMembers.square(radius) * ocl.GlobalMembers.square(m)));
			}
			else
			{
				ccu = sign(m) * xu;
			}
			Point cc_tmp = new Point(ccu, d, 0.0); // cc-point in the XY plane
			cc_tmp.z_projectOntoEdge(u1, u2);
			double cl_z;
			if (hyperbola_case)
			{ // 1) zc = zp - Lc + (R - sqrt(xp^2 + d^2)) / tan(beta2)
				cl_z = cc_tmp.z - center_height + (radius - Math.Sqrt(ocl.GlobalMembers.square(ccu) + ocl.GlobalMembers.square(d))) / Math.Tan(angle);
			}
			else
			{ // 2) zc = zp - Lc
				cl_z = cc_tmp.z - center_height; // case where we hit the edge of the cone
			}
			return new Tuple< double, double >(ccu, cl_z);
		}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool facetPush(const Fiber& fib, Interval& i, const Triangle& t) const
		protected new bool facetPush(Fiber fib, Interval i, Triangle t)
		{
			// push two objects: tip, and base-circle
			bool result = false;
			if (generalFacetPush(0, 0, 0, fib, i, t)) // TIP
			{
				result = true;
			}
						   //   normal_length, center_height, xy_normal_length, fiber, interval, triangle
			if (generalFacetPush(0, this.center_height, this.xy_normal_length, fib, i, t)) // BASE
			{
				result = true;
			}

			return result;
		}


		// cone is pushed along Fiber f into contact with edge p1-p2
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool generalEdgePush(const Fiber& f, Interval& i, const Point& p1, const Point& p2) const
		protected new bool generalEdgePush(Fiber f, Interval i, Point p1, Point p2)
		{
			bool result = false;

			if (GlobalMembers.isZero_tol(p2.z - p1.z)) // guard against horizontal edge
			{
				return result;
			}
			Debug.Assert((p2.z - p1.z) != 0.0);
			// idea: as the ITO-cone slides along the edge it will pierce a z-plane at the height of the fiber
			// the shaped of the pierced area is either a circle if the edge is steep
			// or a 'half-circle' + cone shape if the edge is shallow (ice-cream cone...)
			// we can now intersect this 2D shape with the fiber and get the CL-points.

			// this is where the ITO cone pierces the z-plane of the fiber
			// edge-line: p1+t*(p2-p1) = zheight
			// => t = (zheight - p1)/ (p2-p1)
			double t_tip = (f.p1.z - p1.z) / (p2.z - p1.z);
			Point p_tip = p1 + t_tip * (p2 - p1);
			Debug.Assert(GlobalMembers.isZero_tol(Math.Abs(p_tip.z - f.p1.z))); // p_tip should be in plane of fiber

			// this is where the ITO cone base exits the plane
			double t_base = (f.p1.z + center_height - p1.z) / (p2.z - p1.z);
			Point p_base = p1 + t_base * (p2 - p1);
			p_base.z = f.p1.z; // project to plane of fiber
			double L = (p_base - p_tip).xyNorm();

			//if ( L <= radius ){ // this is where the ITO-slice is a circle
				// find intersection points, if any, between the fiber and the circle
				// fiber is f.p1 - f.p2
				// circle is centered at p_base
				double d = p_base.xyDistanceToLine(f.p1, f.p2);
				if (d <= radius)
				{
					// we know there is an intersection point.
					// http://mathworld.wolfram.com/Circle-LineIntersection.html

					// subtract circle center, math is for circle centered at (0,0)
					double dx = f.p2.x - f.p1.x;
					double dy = f.p2.y - f.p1.y;
					double dr = Math.Sqrt(ocl.GlobalMembers.square(dx) + ocl.GlobalMembers.square(dy));
					double det = (f.p1.x - p_base.x) * (f.p2.y - p_base.y) - (f.p2.x - p_base.x) * (f.p1.y - p_base.y);

					// intersection given by:
					//  x = det*dy +/- sign(dy) * dx * sqrt( r^2 dr^2 - det^2 )   / dr^2
					//  y = -det*dx +/- abs(dy)  * sqrt( r^2 dr^2 - det^2 )   / dr^2

					double discr = ocl.GlobalMembers.square(radius) * ocl.GlobalMembers.square(dr) - ocl.GlobalMembers.square(det);
					if (discr >= 0.0)
					{
						if (discr == 0.0)
						{ // tangent case
							double x_tang = (det * dy) / ocl.GlobalMembers.square(dr);
							double y_tang = -(det * dx) / ocl.GlobalMembers.square(dr);
							Point p_tang = new Point(x_tang + p_base.x, y_tang + p_base.y); // translate back from (0,0) system!
							double t_tang = f.tval(p_tang);
							if (circle_CC(t_tang, p1, p2, f, i))
							{
								result = true;
							}
						}
						else
						{
							// two intersection points with the base-circle
							double x_pos = (det * dy + sign(dy) * dx * Math.Sqrt(discr)) / ocl.GlobalMembers.square(dr);
							double y_pos = (-det * dx + Math.Abs(dy) * Math.Sqrt(discr)) / ocl.GlobalMembers.square(dr);
							Point cl_pos = new Point(x_pos + p_base.x, y_pos + p_base.y);
							double t_pos = f.tval(cl_pos);
							// the same with "-" sign:
							double x_neg = (det * dy - sign(dy) * dx * Math.Sqrt(discr)) / ocl.GlobalMembers.square(dr);
							double y_neg = (-det * dx - Math.Abs(dy) * Math.Sqrt(discr)) / ocl.GlobalMembers.square(dr);
							Point cl_neg = new Point(x_neg + p_base.x, y_neg + p_base.y);
							double t_neg = f.tval(cl_neg);
							if (circle_CC(t_pos, p1, p2, f, i))
							{
								result = true;
							}

							if (circle_CC(t_neg, p1, p2, f, i))
							{
								result = true;
							}

						}
					}
				}

			//} // circle-case

			if (L > radius)
			{
				// ITO-slice is cone + "half-circle"
				// lines from p_tip to tangent points of the base-circle

				// this page has an analytic solution:
				// http://mathworld.wolfram.com/CircleTangentLine.html
				// this page has a geometric construction:
				// http://www.mathopenref.com/consttangents.html

				// circle p_base, radius
				// top    p_tip

				// tangent point at intersection of base-circle with this circle:
				Point p_mid = 0.5 * (p_base + p_tip);
				p_mid.z = f.p1.z;
				double r_tang = L / 2;

				// circle-circle intersection to find tangent-points
				// three cases: no intersection point
				//              one intersection point
				//              two intersection points

				//d is the distance between the circle centers
				Point pd = p_mid - p_base;
				pd.z = 0;
				double dist = pd.xyNorm(); //distance between the circles

				//Check for special cases which do not lead to solutions we want
				bool case1 = (GlobalMembers.isZero_tol(dist) && GlobalMembers.isZero_tol(Math.Abs(radius - r_tang)));
				bool case2 = (dist > (radius + r_tang)); //no solution. circles do not intersect
				bool case3 = (dist < Math.Abs(radius - r_tang)); //no solution. one circle is contained in the other
				bool case4 = (GlobalMembers.isZero_tol(dist - (radius + r_tang))); // tangent case
				if (case1 || case2 || case3 || case4)
				{

				}
				else
				{
					// here we know we have two solutions.

					//Determine the distance from point 0 to point 2
					// law of cosines (http://en.wikipedia.org/wiki/Law_of_cosines)
					// rt^2 = d^2 + r^2 -2*d*r*cos(gamma)
					// so cos(gamma) = (rt^2-d^2-r^2)/ -(2*d*r)
					// and the sought distance is
					// a = r*cos(gamma) = (-rt^2+d^2+r^2)/ (2*d)
					double a = (-ocl.GlobalMembers.square(r_tang) + ocl.GlobalMembers.square(radius) + ocl.GlobalMembers.square(dist)) / (2.0 * dist);
					Debug.Assert(a >= 0.0);

					Point v2 = p_base + (a / dist) * pd; // v2 is the point where the line through the circle intersection points crosses the line between the circle centers.
					//Determine the distance from v2 to either of the intersection points
					double h = Math.Sqrt(ocl.GlobalMembers.square(radius) - ocl.GlobalMembers.square(a));
					//Now determine the offsets of the intersection points from point 2
					Point ofs = new Point(-pd.y * (h / dist), pd.x * (h / dist));
					// now we know the tangent-points
					Point tang1 = v2 + ofs;
					Point tang2 = v2 - ofs;
					if (cone_CC(tang1, p_tip, p_base, p1, p2, f, i))
					{
						result = true;
					}
					if (cone_CC(tang2, p_tip, p_base, p1, p2, f, i))
					{
						result = true;
					}
				}
			} // end circle+cone case

			return result;
		}


		// t is a position along the fiber
		// p1-p2 is the edge
		// Interval& i is updated
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool circle_CC(double t, const Point& p1, const Point& p2, const Fiber& f, Interval& i) const
		protected bool circle_CC(double t, Point p1, Point p2, Fiber f, Interval i)
		{
			// cone base circle is center_height above fiber
			double t_cc = (f.p1.z + center_height - p1.z) / (p2.z - p1.z); // t-parameter of the cc-point, center_height above fiber
			CCPoint cc_tmp = p1 + t_cc * (p2 - p1); // cc-point on the edge
			cc_tmp.type = CCType.EDGE_CONE_BASE;
			return i.update_ifCCinEdgeAndTrue(t, cc_tmp, p1, p2, (true));
		}


		// test for intersection with the fiber and a tip-tang line
		// if there is an intersection in tip-tang, calculate the cc-point and update the interval
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool cone_CC(const Point& tang, const Point& tip, const Point& base, const Point& p1, const Point& p2, const Fiber& f, Interval& i) const
		protected bool cone_CC(Point tang, Point tip, Point @base, Point p1, Point p2, Fiber f, Interval i)
		{
			double u;
			double t;
			if (GlobalMembers.xy_line_line_intersection(f.p1, f.p2, u, tang, tip, t))
			{
				if ((t >= 0.0) && (t <= 1.0))
				{
					CCPoint cc_tmp = @base + t * (tip - @base);
					cc_tmp.z_projectOntoEdge(p1, p2);
					cc_tmp.type = CCType.EDGE_CONE;
					return i.update_ifCCinEdgeAndTrue(u, cc_tmp, p1, p2, (true));
				}
			}
			return false;
		}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double height(double r) const
		protected new double height(double r)
		{
			Debug.Assert(Math.Tan(angle) > 0.0); // guard against division by zero
			return r / Math.Tan(angle);
		}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double width(double h) const
		protected new double width(double h)
		{
			// grows from zero up to radius
			// above that (cutter shaft) return radius
			return (h < center_height) ? h * Math.Tan(angle) : radius;
		}

		/// the half-angle of the cone, in radians
		protected double angle;
}

} // end ocl namespace
// end conecutter.hpp

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)

// end file conecutter.cpp
