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

//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <boost/foreach.hpp>

//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)
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



//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <boost/foreach.hpp>


namespace ocl
{

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class MillingCutter;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class STLSurf;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Triangle;


///
/// \brief path drop cutter finish Path generation
public class AdaptivePathDropCutter : Operation
{
		/// construct an empty PathDropCutter object

		//********   ********************** */

		public AdaptivePathDropCutter()
		{
			// std::cout << " AdaptivePathDropCutter() " << std::endl;
			cutter = null;
			surf = null;
			path = null;
			minimumZ = 0.0;
			subOp.Clear();
			subOp.Add(new PointDropCutter()); // we delegate to PointDropCutter, who does the heavy lifting
			sampling = 0.1;
			min_sampling = 0.01;
			cosLimit = 0.999;
		}

		public override void Dispose()
		{
			// std::cout << " ~AdaptivePathDropCutter() " << std::endl;
			subOp.Clear();
			base.Dispose();
		}

		/// run drop-cutter on the whole Path
		public override void run()
		{
			adaptive_sampling_run();
		}

		/// set the minimum sapling interval
		public void setMinSampling(double s)
		{
			Debug.Assert(s > 0.0);
			//std::cout << " apdc::setMinSampling = " << s << "\n";
			min_sampling = s;
		}
		/// set the cosine limit for the flat() predicate
		public void setCosLimit(double lim)
		{
			cosLimit = lim;
		}
		public void setZ(double z)
		{
			minimumZ = z;
		}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double getZ() const
		public double getZ()
		{
			return minimumZ;
		}
		public void setPath(Path p)
		{
			path = p;

			subOp[0].clearCLPoints();
		}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: ClassicVector<CLPoint> getPoints() const
		public List<CLPoint> getPoints()
		{
			return new List<CLPoint>(clpoints);
		}

		/// run adaptive sample on the given Span between t-values of start_t and stop_t
		protected void adaptive_sample(Span span, double start_t, double stop_t, CLPoint start_cl, CLPoint stop_cl)
		{
			double mid_t = start_t + (stop_t - start_t) / 2.0; // mid point sample
			Debug.Assert(mid_t > start_t);
			Debug.Assert(mid_t < stop_t);
			CLPoint mid_cl = new CLPoint(span.getPoint(mid_t));
			//std::cout << " apdc sampling at " << mid_t << "\n";
			subOp[0].run(mid_cl);
			double fw_step = (stop_cl - start_cl).xyNorm();
			if ((fw_step > sampling) || ((!flat(start_cl, mid_cl, stop_cl)) && (fw_step > min_sampling)))
			{ // OR not flat, and not max sampling
				adaptive_sample(span, start_t, mid_t, new ocl.CLPoint(start_cl), new ocl.CLPoint(mid_cl));
				adaptive_sample(span, mid_t, stop_t, new ocl.CLPoint(mid_cl), new ocl.CLPoint(stop_cl));
			}
			else
			{
				clpoints.Add(stop_cl);
			}
		}

		/// flatness predicate for adaptive sampling
		protected bool flat(CLPoint start_cl, CLPoint mid_cl, CLPoint stop_cl)
		{
			CLPoint v1 = new CLPoint(mid_cl - start_cl);
			CLPoint v2 = new CLPoint(stop_cl - mid_cl);
			v1.normalize();
			v2.normalize();
			return (v1.dot(v2) > cosLimit);
		}

		/// run adaptive sampling
		protected void adaptive_sampling_run()
		{
			//std::cout << " apdc::adaptive_sampling_run()... ";

			clpoints.Clear();
			foreach (Span span in path.span_list)
			{ // this loop could run in parallel, since spans don't depend on eachother
				CLPoint start = new CLPoint(span.getPoint(0.0));
				CLPoint stop = new CLPoint(span.getPoint(1.0));
				subOp[0].run(start);
				subOp[0].run(stop);
				clpoints.Add(start);
				adaptive_sample(span, 0.0, 1.0, new ocl.CLPoint(start), new ocl.CLPoint(stop));
			}
			//std::cout << " DONE clpoints.size()=" << clpoints.size() << "\n";
		}

	// DATA
		/// the smallest sampling interval used when adaptively subdividing
		protected double min_sampling;
		/// the limit for dot-product used in flat()
		protected double cosLimit;
		protected Path path;
		protected double minimumZ;
		protected List<CLPoint> clpoints = new List<CLPoint>();
}

} // end namespace
// end file pathdropcutter.h



