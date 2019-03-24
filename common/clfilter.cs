using System.Collections.Generic;

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
/// \brief CL point filter virtual base class
///
public abstract class CLFilter : System.IDisposable
{
		/// constructor
		public CLFilter()
		{
		}
		public virtual void Dispose()
		{
		}

		/// add CLPoint
		public abstract void addCLPoint(CLPoint p);
		/// set the tolerance value
		public abstract void setTolerance(double tol);

		/// run filter
		public abstract void run();

		/// the list of CL-points to be processed
		public LinkedList<CLPoint> clpoints = new LinkedList<CLPoint>();
		/// tolerance
		public double tol;
}

} // end namespace
// end file clfilter.h
