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


/// Span type
public enum SpanType
{
	LineSpanType,
	ArcSpanType
}

/// \brief A finite curve which returns Point objects along its length.
///
/// location along span is based on a parameter t for which 0 <= t <= 1.0
public abstract class Span : System.IDisposable
{
		/// return type of span
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual SpanType type()const = 0;
		public abstract SpanType type();
		/// return the length of the span in the xy-plane
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual double length2d()const = 0;
		public abstract double length2d();
		/// return a point at parameter value 0 <= t <= 1.0
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual Point getPoint(double t) const = 0;
		public abstract Point getPoint(double t); // 0.0 to 1.0
		/// avoid gcc 4.7.1 delete-non-virtual-dtor error
		public virtual void Dispose()
		{
		}
}

/// Line Span
public class LineSpan : Span
{
		/// create a line span from Line l
		public LineSpan(Line l)
		{
			this.line = new ocl.Line(l);
		}
		/// the line
		public Line line = new Line();

		// Span's virtual functions
		/// return span type
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: SpanType type()const
		public override SpanType type()
		{
			return SpanType.LineSpanType;
		}
		/// return span length
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double length2d() const
		public override double length2d()
		{
			return line.length2d();
		}
		/// return point on span
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point getPoint(double t) const
		public override Point getPoint(double t)
		{
			return new ocl.Point(line.getPoint(t));
		}
}

/// circular Arc Span
public class ArcSpan : Span
{
		/// create span
		public ArcSpan(Arc a)
		{
			this.arc = new ocl.Arc(a);
		}
		/// arc
		public Arc arc = new Arc();

		// Span's virtual functions
		/// return type
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: SpanType type()const
		public override SpanType type()
		{
			return SpanType.ArcSpanType;
		}
		/// return length in xy-plane
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: double length2d()const
		public override double length2d()
		{
			return arc.length2d();
		}
		/// return a point on the span
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: Point getPoint(double t)const
		public override Point getPoint(double t)
		{
			return new ocl.Point(arc.getPoint(t));
		}
}

///
/// \brief A collection of Span objects
///
public class Path : System.IDisposable
{
		/// create empty path
		public Path()
		{
		}

		/// copy constructor
		public Path(Path p)
		{
		}

		/// destructor
		public virtual void Dispose()
		{
		}

		/// list of spans in this path
		public LinkedList<Span> span_list = new LinkedList<Span>();

		// FIXME: this looks wrong
		// should be only one append() that takes a Span
		/// append a Line to this path
		public void append(Line l)
		{
				span_list.AddLast(new LineSpan(l));
		}

		/// append an Arc to this path
		public void append(Arc a)
		{
				span_list.AddLast(new ArcSpan(a));
		}
}

} // end namespace
// end file path.h


// end file path.cpp
