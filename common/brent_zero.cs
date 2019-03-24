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

/// Brent's root finding algorithm
/// http://en.wikipedia.org/wiki/Brent's_method
///
/// find a zero of function f in the interval [a,b]
/// a and b must bracket the root, i.e. f(a) must have different sign than f(b)
/// needs a pointer to an ErrObj which must provide a function
/// double ErrObj::error(double x) for which we try to find a zero
///
/// FIXME: describe tolerance parameters eps and t
//C++ TO C# CONVERTER TODO TASK: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template <class ErrObj>



} // end namespace
// end file brent_zero.hpp
