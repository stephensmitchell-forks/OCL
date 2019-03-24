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



namespace ocl
{

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Point;

/// \brief STL surface, essentially an unordered list of Triangle objects
///
/// STL surfaces consist of triangles. There is by definition no structure
/// or order among the triangles, i.e. they can be positioned or connected in arbitrary ways.
public class STLSurf : System.IDisposable
{
		/// Create an empty STL-surface
		public STLSurf()
		{
		}
		/// destructor
		public virtual void Dispose()
		{
		}
		/// add Triangle t to this surface
		public void addTriangle(Triangle t)
		{

			// some sanity-checking:
			Debug.Assert((t.p[0] - t.p[1]).norm() > 0.0);
			Debug.Assert((t.p[1] - t.p[2]).norm() > 0.0);
			Debug.Assert((t.p[2] - t.p[0]).norm() > 0.0);

			tris.AddLast(t);
			bb.addTriangle(t);
			return;
		}

		/// return number of triangles in surface
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint size() const
		public uint size()
		{
			return (uint)tris.Count;
		}

		/// call Triangle::rotate on all triangles
		public void rotate(double xr, double yr, double zr)
		{
			//std::cout << " before " << t << "\n";
			bb.clear();
			foreach (Triangle t in tris)
			{
				//std::cout << " before " << t << "\n";
				t.rotate(xr, yr, zr);
				//std::cout << " after " << t << "\n";
				//char c;
				//std::cin >> c;
				bb.addTriangle(t);
			}
		}

		/// list of Triangles in this surface
		public LinkedList<Triangle> tris = new LinkedList<Triangle>();
		/// bounding-box
		public Bbox bb = new Bbox();
		/// STLSurf string repr
		public static std::ostream operator << (std::ostream stream, STLSurf s)
		{
		  stream << "STLSurf(N=" << s.tris.Count << ")";
		  return stream;
		}
}

} // end namespace
// end file stlsurf.h


// end file stlsurf.cpp
