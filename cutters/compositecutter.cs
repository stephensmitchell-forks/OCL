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

/// \brief a CompositeCutter is composed one or more MillingCutters
/// the cutters are stored in a vector *cutter* and their axial offsets
/// from eachother in *zoffset*. The different cutters apply in different
/// radial regions. cutter[0] from r=0 to r=radius[0] after that 
/// cutter[1] from r=radius[0] to r=radius[1] and so on. 
public class CompositeCutter : MillingCutter
{
		/// create an empty CompositeCutter
		public CompositeCutter()
		{
			radiusvec = new List<double>();
			cutter = new List<MillingCutter>();
			radius = 0;
			diameter = 0;
		}

		/// add a MillingCutter to this CompositeCutter
		/// the cutter is valid from the previous radius out to the given radius
		/// and its axial offset is given by zoffset

		// add cutter c which is valid until radius=r and height=z with z-offset zoff
		public void addCutter(MillingCutter c, double r, double h, double zoff)
		{
			radiusvec.Add(r);
			heightvec.Add(h);
			cutter.Add(c);
			zoffset.Add(zoff);
			if (r > radius)
			{
				radius = r;
				diameter = 2 * r;
			}
			// update length also?
		}



//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: MillingCutter* offsetCutter(double d) const
		public new MillingCutter offsetCutter(double d)
		{
			Console.Write(" ERROR: not implemented.\n");
			Debug.Assert(false);
			return new CylCutter(); //FIXME!
		}

		/// CompositeCutter can not use the base-class facetDrop, instead we here
		/// call facetDrop() on each cutter in turn, and pick the valid CC/CL point 
		/// as the result for the CompositeCutter

		//********   facetDrop  ********************** */

		// call facetDrop on each cutter and pick a valid cc-point
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool facetDrop(CLPoint &cl, const Triangle &t) const
		public new bool facetDrop(CLPoint cl, Triangle t)
		{
			bool result = false;
			for (int n = 0; n < cutter.Count; ++n)
			{ // loop through cutters
				CLPoint cl_tmp = cl + new CLPoint(0, 0, zoffset[n]);
				CCPoint cc_tmp;
				if (cutter[n].facetDrop(cl_tmp, t))
				{
					Debug.Assert(cl_tmp.cc != null);
					if (ccValidRadius(n, cl_tmp))
					{ // cc-point is valid
						cc_tmp = new CCPoint(cl_tmp.cc);
						if (cl.liftZ(cl_tmp.z - zoffset[n]))
						{ // we need to lift the cutter
							cc_tmp.type = CCType.FACET;
							cl.cc = cc_tmp;
							result = true;
						}
						else
						{
							if (cc_tmp != null)
							{
								cc_tmp.Dispose();
							}
						}
					}
				}
			}
			return result;
		}

		/// call edgeDrop on each cutter and pick the correct (highest valid CL-point) result

		//********   edge **************************************************** */
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool edgeDrop(CLPoint &cl, const Triangle &t) const
		public new bool edgeDrop(CLPoint cl, Triangle t)
		{
			bool result = false;
			for (int n = 0; n < cutter.Count; ++n)
			{ // loop through cutters
				CLPoint cl_tmp = cl + new Point(0, 0, zoffset[n]);
				CCPoint cc_tmp;
				if (cutter[n].edgeDrop(cl_tmp, t))
				{ // drop sub-cutter against edge
					if (ccValidRadius(n, cl_tmp))
					{ // check if cc-point is valid
						cc_tmp = new CCPoint(cl_tmp.cc);
						if (cl.liftZ(cl_tmp.z - zoffset[n]))
						{ // we need to lift the cutter
							cc_tmp.type = CCType.EDGE;
							cl.cc = cc_tmp;
							result = true;
						}
						else
						{
							if (cc_tmp != null)
							{
								cc_tmp.Dispose();
							}
						}
					}
				}
			}
			return result;
		}
        /*
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const
		public new string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << "CompositeCutter with " << cutter.Count << " cutters:\n";
			for (uint n = 0; n < cutter.Count; ++n)
			{ // loop through cutters
				o << " " << (int)n << ":" << cutter[n].str() << "\n";
				o << "  radius[" << (int)n << "]=" << radiusvec[n] << "\n";
				o << "  height[" << (int)n << "]=" << heightvec[n] << "\n";
				o << "  zoffset[" << (int)n << "]=" << zoffset[n] << "\n";
			}
			return o.str();
		}
        */

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool vertexPush(const Fiber& f, Interval& i, const Triangle& t) const
		protected new bool vertexPush(Fiber f, Interval i, Triangle t)
		{
			bool result = false;
			List< Tuple<double, CCPoint>> contacts = new List< Tuple<double, CCPoint>>();
			for (int n = 0; n < cutter.Count; ++n)
			{
				Interval ci = new Interval();
				Fiber cf = new Fiber(f);
				cf.p1.z = f.p1.z + zoffset[n];
				cf.p2.z = f.p2.z + zoffset[n]; // raised/lowered fiber to push along
				if (cutter[n].vertexPush(cf,ci,t))
				{
					if (ccValidHeight(n, ci.upper_cc, f))
					{
						contacts.Add(new Tuple<double,CCPoint>(ci.upper, ci.upper_cc));
					}
					if (ccValidHeight(n, ci.lower_cc, f))
					{
						contacts.Add(new Tuple<double,CCPoint>(ci.lower, ci.lower_cc));
					}
				}
			}

			for (int n = 0; n < contacts.Count; ++n)
			{
				i.update(contacts[n].Item1, contacts[n].Item2);
				result = true;
			}
			return result;
		}


		// push each cutter against facet
		//  if the cc-point is valid (at correct height), store interval data
		// push all interval data into the original interval
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool facetPush(const Fiber& f, Interval& i, const Triangle& t) const
		protected new bool facetPush(Fiber f, Interval i, Triangle t)
		{
			// run facetPush for each cutter, retain valid results, and return union of all
			bool result = false;
			List< Tuple<double, CCPoint>> contacts = new List< Tuple<double, CCPoint>>();
			for (int n = 0; n < cutter.Count; ++n)
			{
				Interval ci = new Interval();
				Fiber cf = new Fiber(f);
				cf.p1.z = f.p1.z + zoffset[n];
				cf.p2.z = f.p2.z + zoffset[n]; // raised/lowered fiber to push along
				if (cutter[n].facetPush(cf,ci,t))
				{
					if (ccValidHeight(n, ci.upper_cc, f))
					{
						contacts.Add(new Tuple<double,CCPoint>(ci.upper, ci.upper_cc));
					}
					if (ccValidHeight(n, ci.lower_cc, f))
					{
						contacts.Add(new Tuple<double,CCPoint>(ci.lower, ci.lower_cc));
					}
				}
			}

			for (int n = 0; n < contacts.Count; ++n)
			{
				i.update(contacts[n].Item1, contacts[n].Item2);
				result = true;
			}
			return result;
		}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool edgePush(const Fiber& f, Interval& i, const Triangle& t) const
		protected new bool edgePush(Fiber f, Interval i, Triangle t)
		{
			bool result = false;
			List< Tuple<double, CCPoint>> contacts = new List< Tuple<double, CCPoint>>();
			for (int n = 0; n < cutter.Count; ++n)
			{
				Interval ci = new Interval(); // interval for this cutter
				Fiber cf = new Fiber(f); // fiber for this cutter
				cf.p1.z = f.p1.z + zoffset[n];
				cf.p2.z = f.p2.z + zoffset[n]; // raised/lowered fiber to push along
				if (cutter[n].edgePush(cf,ci,t))
				{
					if (ccValidHeight(n, ci.upper_cc, f))
					{
						contacts.Add(new Tuple<double,CCPoint>(ci.upper, ci.upper_cc));
					}
					if (ccValidHeight(n, ci.lower_cc, f))
					{
						contacts.Add(new Tuple<double,CCPoint>(ci.lower, ci.lower_cc));
					}
				}
			}

			for (int n = 0; n < contacts.Count; ++n)
			{
				i.update(contacts[n].Item1, contacts[n].Item2);
				result = true;
			}

			return result;
		}

		/// convert input radius r to cutter index
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint radius_to_index(double r) const
		protected int radius_to_index(double r)
		{
			for (int n = 0; n < cutter.Count; ++n)
			{
				if (validRadius(n, r))
				{
					return n;
				}
			}
			Debug.Assert(false);
			return 0;
		}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint height_to_index(double h) const
		protected int height_to_index(double h)
		{
			for (int n = 0; n < cutter.Count; ++n)
			{
				if (validHeight(n, h))
				{
					return n;
				}
			}
			// return the last cutter if we get here...
			return (int)(cutter.Count - 1);
			Console.Write(" Error, height= ");
			Console.Write(h);
			Console.Write(" has no index \n");
			Debug.Assert(false);
			return 0;
		}

		/// return true if radius=r belongs to cutter n
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool validRadius(uint n, double r) const
		protected bool validRadius(int n, double r)
		{
			Debug.Assert(r >= 0.0);
			double lolimit;
			double hilimit;
			if (n == 0)
			{
				lolimit = -1E-6;
			}
			else
			{
				lolimit = radiusvec[n - 1] - 1E-6;
			}
			hilimit = radiusvec[n] + 1e-6; // FIXME: really ugly solution this one...
			if ((lolimit <= r))
			{
				if (r <= hilimit)
				{
					return true;
				}
			}
			return false;
		}


		// return true if height h belongs to cutter n
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool validHeight(uint n, double h) const
		protected bool validHeight(int n, double h)
		{
			double lolimit;
			double hilimit;
			if (n == 0)
			{
				lolimit = -1E-6;
			}
			else
			{
				lolimit = heightvec[n - 1] - 1E-6;
			}
			hilimit = heightvec[n] + 1e-6; // FIXME: really ugly solution this one...
			if ((lolimit <= h))
			{
				if (h <= hilimit)
				{
					return true;
				}
			}
			return false;
		}


		// this allows vertexDrop in the base-class to work as for other cutters
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double height(double r) const
		protected new double height(double r)
		{
			int idx = radius_to_index(r);
			return cutter[idx].height(r) + zoffset[idx];
		}


		// return the width of the cutter at height h. 
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double width(double h) const
		protected new double width(double h)
		{
			int idx = height_to_index(h);
			// std::cout << "CompositeCutter::width( " << h << " ) idx=" << idx << " zoffset= " << zoffset[idx] << "\n";
			// std::cout << " width  =  " << cutter[idx]->width( h - zoffset[idx] ) << "\n";
			return cutter[idx].width(h - zoffset[idx]);
		}

		/// return true if cl.cc is within the radial range of cutter n
		/// for cutter n the valid radial distance from cl is
		/// between radiusvec[n-1] and radiusvec[n]
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool ccValidRadius(uint n, CLPoint& cl) const
		protected bool ccValidRadius(int n, CLPoint cl)
		{
			if (cl.cc.type == CCType.NONE)
			{
				return false;
			}
			double d = cl.xyDistance(cl.cc);
			double lolimit;
			double hilimit;
			if (n == 0)
			{
				lolimit = - 1E-6;
			}
			else
			{
				lolimit = radiusvec[n - 1] - 1E-6;
			}
			hilimit = radiusvec[n] + 1e-6; // FIXME: really ugly solution this one...
			if (d < lolimit)
			{
				return false;
			}
			else if (d > hilimit)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool ccValidHeight(uint n, CCPoint& cc, const Fiber& f) const
		protected bool ccValidHeight(int n, CCPoint cc, Fiber f)
		{
			//if (  ((cc.z-f.p1.z) >= 0.0)  && (n == height_to_index(cc.z-f.p1.z)) )
			if (n == height_to_index(cc.z - f.p1.z))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// vector that holds the radiuses of the different cutters.
		/// cutter[0] is valid from r=0 to  r=radiusvec[0]
		/// cutter[1] is valid from r=radiusvec[0] to r=radiusvec[1]
		/// etc
		protected List<double> radiusvec = new List<double>(); // vector of radiuses
		/// vector for cutter heights
		/// cutter[0] is valid in h = [0, heihgtvec[0] ]
		/// cutter[1] is valid in h = [ heightvec[0], heihgtvec[1] ]
		protected List<double> heightvec = new List<double>(); // vector of heights
		/// vector of the axial offsets 
		protected List<double> zoffset = new List<double>(); // vector of z-offset values for the cutters
		/// vector of cutters in this CompositeCutter
		protected List<MillingCutter> cutter = new List<MillingCutter>(); // vector of pointers to cutters
}

/// a composite cutter for testing, consisting only of a cylindrical cutter
public class CompCylCutter : CompositeCutter
{
		public CompCylCutter()
		{
		}

		//********   actual Composite-cutters  ******************************* */


		//  only constructors required, drop-cutter and push-cutter calls handled by base-class

		// TESTING
		public CompCylCutter(double diam2, double clength)
		{
			MillingCutter shaft = new CylCutter(diam2, clength);
			addCutter(shaft, diam2 / 2.0, clength, 0.0);
			length = clength;
		}
}

/// for testing, a single ballcutter
public class CompBallCutter : CompositeCutter
{
		public CompBallCutter()
		{
		}

		// TESTING
		public CompBallCutter(double diam2, double clength)
		{
			MillingCutter shaft = new BallCutter(diam2, clength);
			addCutter(shaft, diam2 / 2.0, clength, 0.0);
			length = clength;
		}
}

/// \brief CompositeCutter with a cylindrical/flat central part of diameter diam1
/// and a conical outer part sloping at angle, with a max diameter diam2
public class CylConeCutter : CompositeCutter
{
		public CylConeCutter()
		{
		} // dummy, required(?) by python wrapper
		/// create cylconecutter
		public CylConeCutter(double diam1, double diam2, double angle)
		{
			MillingCutter cyl = new CylCutter(diam1, 1);
			MillingCutter cone = new ConeCutter(diam2, angle);
			MillingCutter shaft = new CylCutter(diam2, 20); // FIXME: dummy height

			double cone_offset = - (diam1 / 2) / Math.Tan(angle);
			double cyl_height = 0.0;
			double cone_height = (diam2 / 2.0) / Math.Tan(angle) + cone_offset;

			addCutter(cyl, diam1 / 2.0, cyl_height, 0.0);
			addCutter(cone, diam2 / 2.0, cone_height, cone_offset);
			addCutter(shaft, diam2 / 2.0, (diam2 / 2.0) / Math.Tan(angle) + 20, cone_height);
			length = cyl_height + cone_height + 10; // Arbitrary 10 here!
		}
}

/// \brief CompositeCutter with a spherical central part of diameter diam1
/// and a conical outer part sloping at angle, with a max diameter diam2
/// the cone is positioned so that the tangent of the cone matches the tangent of the sphere
public class BallConeCutter : CompositeCutter
{
		public BallConeCutter()
		{
		} // dummy, required(?) by python wrapper
		/// create ballconecutter
		public BallConeCutter(double diam1, double diam2, double angle)
		{
			MillingCutter c1 = new BallCutter(diam1, 1); // at offset zero
			MillingCutter c2 = new ConeCutter(diam2, angle);
			MillingCutter shaft = new CylCutter(diam2, 20); // FIXME: length

			double radius1 = diam1 / 2.0;
			double radius2 = diam2 / 2.0;
			double rcontact = radius1 * Math.Cos(angle);
			double height1 = radius1 - Math.Sqrt(ocl.GlobalMembers.square(radius1) - ocl.GlobalMembers.square(rcontact));
			double cone_offset = - ((rcontact) / Math.Tan(angle) - height1);
			double height2 = radius2 / Math.Tan(angle) + cone_offset;
			double shaft_offset = height2;

			// cutter, radivec, heightvec, zoffset
			addCutter(c1, rcontact, height1, 0.0);
			addCutter(c2, diam2 / 2.0, height2, cone_offset);
			addCutter(shaft, diam2 / 2.0, height2 + 20, shaft_offset);
			length = 30;
		}
}

/// \brief CompositeCutter with a toroidal central part of diameter diam1 
/// and corner radius radius1
/// The outer part is conical sloping at angle, with a max diameter diam2
/// the cone is positioned so that the tangent of the cone matches the tangent of the torus
public class BullConeCutter : CompositeCutter
{
		public BullConeCutter()
		{
		} // dummy, required(?) by python wrapper
		/// create bullconecutter
		public BullConeCutter(double diam1, double radius1, double diam2, double angle)
		{
			MillingCutter c1 = new BullCutter(diam1, radius1, 1.0); // at offset zero
			MillingCutter c2 = new ConeCutter(diam2, angle);
			MillingCutter shaft = new CylCutter(diam2, 20);

			double h1 = radius1 * Math.Sin(angle); // the contact point is this much down from the toroid-ring
			double rad = Math.Sqrt(ocl.GlobalMembers.square(radius1) - ocl.GlobalMembers.square(h1));
			double rcontact = (diam1 / 2.0) - radius1 + rad; // radius of the contact-ring
			double cone_offset = - (rcontact / Math.Tan(angle) - (radius1 - h1));
			double height1 = radius1 - h1;
			double height2 = (diam2 / 2.0) / Math.Tan(angle) + cone_offset;
			double shaft_offset = height2;

			addCutter(c1, rcontact, height1, 0.0);
			addCutter(c2, diam2 / 2.0, height2, cone_offset);
			addCutter(shaft, diam2 / 2.0, height2 + 20, shaft_offset);
			length = 30;
		}
}

/// \brief CompositeCutter with a conical central part with diam1/angle1 
/// and a conical outer part with diam2/angle2
/// we assume angle2 < angle1  and  diam2 > diam1.
public class ConeConeCutter : CompositeCutter
{
		public ConeConeCutter()
		{
		} // dummy, required(?) by python wrapper
		/// create cone-cone cutter with lower cone (diam1,angle1) and upper cone (diam2,angle2)
		/// we assume angle2 < angle1  and  diam2 > diam1.
		public ConeConeCutter(double diam1, double angle1, double diam2, double angle2)
		{
			MillingCutter c1 = new ConeCutter(diam1, angle1); // at offset zero
			MillingCutter c2 = new ConeCutter(diam2, angle2);
			MillingCutter shaft = new CylCutter(diam2, 20);

			double height1 = (diam1 / 2.0) / Math.Tan(angle1);
			double tmp = (diam1 / 2.0) / Math.Tan(angle2);
			double cone_offset = - (tmp - height1);
			double height2 = (diam2 / 2.0) / Math.Tan(angle2) + cone_offset;
			double shaft_offset = height2;

			addCutter(c1, diam1 / 2.0, height1, 0.0);
			addCutter(c2, diam2 / 2.0, height2, cone_offset);
			addCutter(shaft, diam2 / 2.0, height2 + 20, shaft_offset);
			length = 30;
		}
}


} // end namespace

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)

// end file compositecutter.cpp
