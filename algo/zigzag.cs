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




namespace ocl
{

/// zgizag 2D operation
public class ZigZag : System.IDisposable
{
		public ZigZag()
		{
		}
		public virtual void Dispose()
		{
		}
		/// step over distance 
		public void setStepOver(double d)
		{
			stepOver = d;
		}
		/// set dir
		public void setDirection(Point d)
		{
			dir.CopyFrom(d);
		}
		public void setOrigin(Point d)
		{
			origin.CopyFrom(d);
		}

		/// run the algorithm
		public void run()
		{
			// calculate a reasonable maximum/minimum step-over dist
			Point perp = dir.xyPerp();
			perp.xyNormalize();
			Console.Write(" minpt = ");
			Console.Write(bb.minpt);
			Console.Write("\n");
			Console.Write(" maxpt = ");
			Console.Write(bb.maxpt);
			Console.Write("\n");
			Console.Write(" perp = ");
			Console.Write(perp);
			Console.Write("\n");
			double max_d = (bb.maxpt - origin).dot(perp);
			double min_d = (bb.minpt - origin).dot(perp);
			if (max_d < min_d)
			{
				double tmp = max_d;
				max_d = min_d;
				min_d = tmp;
			}
			//int n = min_d / stepOver; // some safety margin here... (required?)
			Console.Write(" max_d= ");
			Console.Write(max_d);
			Console.Write(" min_d= ");
			Console.Write(min_d);
			Console.Write("\n");

			List<double> distances = new List<double>();
			for (double d = min_d ; d <= max_d ; d += stepOver)
			{
				distances.Add(d);
				@out.Add(origin + d * perp);
				Debug.Assert(@out.Count < 500);
			}

		}

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: boost::python::list getOutput() const
		public boost::python.list getOutput()
		{
			boost::python.list o = new boost::python.list();
			foreach (Point p in @out)
			{
				o.append(p);
			}
			return new boost::python.list(o);
		}

		/// add an input CLPoint to this Operation
		public void addPoint(Point p)
		{
			pocket.Add(p);
			bb.addPoint(p);
		}
        /*
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: string str() const
		public string str()
		{
			std::ostringstream o = new std::ostringstream();
			o << "ZigZag: pocket.size()=" << pocket.Count << std::endl;
			return o.str();
		}
        */
		/// the step over
		protected double stepOver;
		/// direction 
		protected Point dir = new Point();
		/// origin
		protected Point origin = new Point();
		/// pocket
		protected List<Point> pocket = new List<Point>();
		protected List<Point> @out = new List<Point>();
		protected Bbox bb = new Bbox();
}

} // end namespace

