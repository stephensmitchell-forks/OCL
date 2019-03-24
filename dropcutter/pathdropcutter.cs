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



//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)

namespace ocl
{

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class MillingCutter;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class STLSurf;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Triangle;
//class KDNode;

///
/// \brief path drop cutter finish Path generation
public class PathDropCutter : Operation
{
		/// construct an empty PathDropCutter object

		//********   ********************** */

		public PathDropCutter()
		{
			cutter = null;
			surf = null;
			path = null;
			minimumZ = 0.0;
			subOp.Clear();
			subOp.Add(new BatchDropCutter()); // we delegate to BatchDropCutter, who does the heavy lifting
			sampling = 0.1;
		}

		public override void Dispose()
		{
			subOp.Clear();
			base.Dispose();
		}

		/// set the Path to follow and sample
		public void setPath(Path p)
		{
			path = p;
			// ((BatchDropCutter*)(subOp[0]))->clearCLPoints();
			subOp[0].clearCLPoints();
		}

		/// set the minimum z-value, or "floor" for drop-cutter
		public void setZ(double z)
		{
			minimumZ = z;
		}
		/// return Z
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double getZ() const
		public double getZ()
		{
			return minimumZ;
		}
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: ClassicVector<CLPoint> getPoints() const
		public List<CLPoint> getPoints()
		{
			return new List<CLPoint>(clpoints);
		}
		/// run drop-cutter on the whole Path
		public override void run()
		{
			uniform_sampling_run();

		}

		/// the path to follow
		protected Path path;
		/// the lowest z height, used when no triangles are touched, default is minimumZ = 0.0
		protected double minimumZ;
		/// list of CL-points
		protected List<CLPoint> clpoints = new List<CLPoint>();
		/// the algorithm
		private void uniform_sampling_run()
		{
			clpoints.Clear();
			foreach (Span span in path.span_list)
			{ // loop through the spans calling run() on each
				this.sample_span(span); // append points to bdc
			}
			subOp[0].run();
			clpoints = new List<CLPoint>(subOp[0].getCLPoints());
		}

		/// sample the span unfirormly with tolerance sampling

		// this samples the Span and pushes the corresponding sampled points to bdc
		private void sample_span(Span span)
		{
			Debug.Assert(sampling > 0.0);
			uint num_steps = (uint)(span.length2d() / sampling + 1);
			for (uint i = 0; i <= num_steps; i++)
			{
				double fraction = (double)i / num_steps;
				Point ptmp = span.getPoint(fraction);
				CLPoint p = new CLPoint(ptmp.x, ptmp.y, ptmp.z);
				p.z = minimumZ;
				subOp[0].appendPoint(p);
				if (p != null)
				{
					p.Dispose();
				}
			}
		}
}

} // end namespace
// end file pathdropcutter.h


// end file pathdropcutter.cpp
