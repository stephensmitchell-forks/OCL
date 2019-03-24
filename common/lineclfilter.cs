using System.Collections.Generic;

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

/// LineCLFilter takes a sequence of cutter-location (CL) points
/// as input and produces another sequence as output.
///
/// The number of CL-points is reduced by finding co-linear points, 
/// to within a set tolerance, and deleting redundant ones.
///
public class LineCLFilter : CLFilter
{
		public LineCLFilter()
		{
			clpoints.Clear();
		}

		public new void Dispose()
		{
			base.Dispose();
		}
		public override void addCLPoint(CLPoint p)
		{
			clpoints.AddLast(p);
		}

		public override void setTolerance(double tolerance)
		{
			tol = tolerance;
		}

		public override void run()
		{
			int n = clpoints.Count;
			if (n < 2)
			{
				return; // can't filter lists of length 0, 1, or 2
			}

			LinkedList<CLPoint> new_list = new LinkedList<CLPoint>();

			LinkedList<CLPoint>.Enumerator p0 = clpoints.GetEnumerator();
			LinkedList<CLPoint>.Enumerator p1 = clpoints.GetEnumerator();
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
			p1++;
			LinkedList<CLPoint>.Enumerator p2 = new ClassicLinkedListIterator(p1);
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
			p2++;
			LinkedList<CLPoint>.Enumerator p_last_good = new ClassicLinkedListIterator(p1);

//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
			new_list.AddLast(p0);

			bool even_number = true;

			while (p2.MoveNext())
			{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
				Point p = p1.closestPoint(p0, p2.Current);
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
				if ((p - p1).norm() < tol)
				{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: p_last_good = p2;
					p_last_good.CopyFrom(p2);
					if (even_number)
					{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
						p1++;
					}
					even_number = !even_number;
				}
				else
				{
//C++ TO C# CONVERTER TODO TASK: Iterators are only converted within the context of 'while' and 'for' loops:
					new_list.AddLast(p_last_good);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: p0 = p_last_good;
					p0.CopyFrom(p_last_good);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: p1 = p2;
					p1.CopyFrom(p2);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: p_last_good = p1;
					p_last_good.CopyFrom(p1);
				}
			}
			new_list.AddLast(clpoints.Last.Value);
			clpoints = new LinkedList<CLPoint>(new_list);
			return;
		}
}



} // end namespace
// end file lineclfilter.h


// end file lineclfilter.cpp
