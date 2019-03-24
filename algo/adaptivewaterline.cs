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

#if _OPENMP
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//	#include <omp.h>
#endif

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
//class Span;

/// \brief a Waterline toolpath follows the shape of the model at a constant z-height in the xy-plane

public class AdaptiveWaterline : Waterline
{
		/// create an empty Waterline object

		//********   ********************** */

		public AdaptiveWaterline()
		{
			subOp.Clear();
			subOp.Add(new FiberPushCutter());
			subOp.Add(new FiberPushCutter());
			subOp[0].setXDirection();
			subOp[1].setYDirection();
			nthreads = 1;
#if _OPENMP
			nthreads = omp_get_num_procs();
			//omp_set_dynamic(0);
			omp_set_nested(1);
#endif
			sampling = 1.0;
			min_sampling = 0.1;
			cosLimit = 0.999;
		}

		public override void Dispose()
		{
			Console.Write("~AdaptiveWaterline(): subOp.size()= ");
			Console.Write(subOp.Count);
			Console.Write("\n");
			subOp.Clear();
			base.Dispose();
		}

		/// set the minimum sampling interval
		public void setMinSampling(double s)
		{
			min_sampling = s;
		}
		/// set the cosine limit for the flat() predicate
		public void setCosLimit(double lim)
		{
			cosLimit = lim;
		}

		/// run the Waterline algorithm. setSTL, setCutter, setSampling, and setZ must
		/// be called before a call to run()
		public new void run()
		{
			adaptive_sampling_run();
			weave_process(); // in base-class Waterline
		}

		public new void run2()
		{
			adaptive_sampling_run();
			weave_process2(); // in base-class Waterline
		}

		public override void setSampling(double s)
		{
			sampling = s;
			min_sampling = sampling / 10.0; // default to this when setMinSampling is not called
		}


		/// adaptive waterline algorithm
		protected void adaptive_sampling_run()
		{
			minx = surf.bb.minpt.x - 2 * cutter.getRadius();
			maxx = surf.bb.maxpt.x + 2 * cutter.getRadius();
			miny = surf.bb.minpt.y - 2 * cutter.getRadius();
			maxy = surf.bb.maxpt.y + 2 * cutter.getRadius();
			Line line = new Line(new Point(minx, miny, zh), new Point(maxx, maxy, zh));
			Span linespan = new LineSpan(line);

#if _WIN32 // OpenMP task not supported with the version 2 of VS2013 OpenMP
		//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
		//    #pragma omp parallel sections
			{
		//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
		//        #pragma omp section // Replace OMP Task by Parallel sections
				{ // first child
#else
		//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
		//#pragma omp parallel
			{
		//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
		//#pragma omp single nowait
				{ // initial root task
		//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
		//#pragma omp task
					{ // first child task
#endif // _WIN32
						xfibers.Clear();
						Point xstart_p1 = new Point(minx, linespan.getPoint(0.0).y, zh);
						Point xstart_p2 = new Point(maxx, linespan.getPoint(0.0).y, zh);
						Point xstop_p1 = new Point(minx, linespan.getPoint(1.0).y, zh);
						Point xstop_p2 = new Point(maxx, linespan.getPoint(1.0).y, zh);
						Fiber xstart_f = new Fiber(xstart_p1, xstart_p2);
						Fiber xstop_f = new Fiber(xstop_p1, xstop_p2);
						subOp[0].run(xstart_f);
						subOp[0].run(xstop_f);
						xfibers.Add(xstart_f);
						Console.Write(" XFiber adaptive sample \n");
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: xfiber_adaptive_sample(linespan, 0.0, 1.0, xstart_f, xstop_f);
						xfiber_adaptive_sample(linespan, 0.0, 1.0, new ocl.Fiber(xstart_f), new ocl.Fiber(xstop_f));
#if _WIN32 // OpenMP task not supported with the version 2 of VS2013 OpenMP
				}
		//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
		//        #pragma omp section
				{ // second child
#else
				}
		//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
		//#pragma omp task
				{ // second child task
#endif // _WIN32
						yfibers.Clear();
						Point ystart_p1 = new Point(linespan.getPoint(0.0).x, miny, zh);
						Point ystart_p2 = new Point(linespan.getPoint(0.0).x, maxy, zh);
						Point ystop_p1 = new Point(linespan.getPoint(1.0).x, miny, zh);
						Point ystop_p2 = new Point(linespan.getPoint(1.0).x, maxy, zh);
						Fiber ystart_f = new Fiber(ystart_p1, ystart_p2);
						Fiber ystop_f = new Fiber(ystop_p1, ystop_p2);
						subOp[1].run(ystart_f);
						subOp[1].run(ystop_f);
						yfibers.Add(ystart_f);
						Console.Write(" YFiber adaptive sample \n");
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: yfiber_adaptive_sample(linespan, 0.0, 1.0, ystart_f, ystop_f);
						yfiber_adaptive_sample(linespan, 0.0, 1.0, new ocl.Fiber(ystart_f), new ocl.Fiber(ystop_f));
#if _WIN32 // OpenMP task not supported with the version 2 of VS2013 OpenMP
				}
			} // end omp parallel
#else
					}
				}
			} // end omp parallel
#endif // _WIN32

			if (line != null)
			{
				line.Dispose();
			}
			if (linespan != null)
			{
				linespan.Dispose();
			}

		}

		/// x-direction adaptive sampling
		protected void xfiber_adaptive_sample(Span span, double start_t, double stop_t, Fiber start_f, Fiber stop_f)
		{
			double mid_t = start_t + (stop_t - start_t) / 2.0; // mid point sample
			Debug.Assert(mid_t > start_t);
			Debug.Assert(mid_t < stop_t);
			//std::cout << "xfiber sample= ( " << start_t << " , " << stop_t << " ) \n";
			Point mid_p1 = new Point(minx, span.getPoint(mid_t).y, zh);
			Point mid_p2 = new Point(maxx, span.getPoint(mid_t).y, zh);
			Fiber mid_f = new Fiber(mid_p1, mid_p2);
			subOp[0].run(mid_f);
			double fw_step = Math.Abs(start_f.p1.y - stop_f.p1.y);
			if (fw_step > sampling)
			{ // above minimum step-forward, need to sample more
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: xfiber_adaptive_sample(span, start_t, mid_t, start_f, mid_f);
				xfiber_adaptive_sample(span, start_t, mid_t, new ocl.Fiber(start_f), new ocl.Fiber(mid_f));
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: xfiber_adaptive_sample(span, mid_t, stop_t, mid_f, stop_f);
				xfiber_adaptive_sample(span, mid_t, stop_t, new ocl.Fiber(mid_f), new ocl.Fiber(stop_f));
			}
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: else if (!flat(start_f,mid_f,stop_f))
			else if (!flat(new ocl.Fiber(start_f), new ocl.Fiber(mid_f), new ocl.Fiber(stop_f)))
			{
				if (fw_step > min_sampling)
				{ // not a flat segment, and we have not reached maximum sampling
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: xfiber_adaptive_sample(span, start_t, mid_t, start_f, mid_f);
					xfiber_adaptive_sample(span, start_t, mid_t, new ocl.Fiber(start_f), new ocl.Fiber(mid_f));
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: xfiber_adaptive_sample(span, mid_t, stop_t, mid_f, stop_f);
					xfiber_adaptive_sample(span, mid_t, stop_t, new ocl.Fiber(mid_f), new ocl.Fiber(stop_f));
				}
			}
			else
			{
				xfibers.Add(stop_f);
			}
		}

		/// y-direction adaptive sampling
		protected void yfiber_adaptive_sample(Span span, double start_t, double stop_t, Fiber start_f, Fiber stop_f)
		{
			double mid_t = start_t + (stop_t - start_t) / 2.0; // mid point sample
			Debug.Assert(mid_t > start_t);
			Debug.Assert(mid_t < stop_t);
			//std::cout << "yfiber sample= ( " << start_t << " , " << stop_t << " ) \n";
			Point mid_p1 = new Point(span.getPoint(mid_t).x, miny, zh);
			Point mid_p2 = new Point(span.getPoint(mid_t).x, maxy, zh);
			Fiber mid_f = new Fiber(mid_p1, mid_p2);
			subOp[1].run(mid_f);
			double fw_step = Math.Abs(start_f.p1.x - stop_f.p1.x);
			if (fw_step > sampling)
			{ // above minimum step-forward, need to sample more
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: yfiber_adaptive_sample(span, start_t, mid_t, start_f, mid_f);
				yfiber_adaptive_sample(span, start_t, mid_t, new ocl.Fiber(start_f), new ocl.Fiber(mid_f));
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: yfiber_adaptive_sample(span, mid_t, stop_t, mid_f, stop_f);
				yfiber_adaptive_sample(span, mid_t, stop_t, new ocl.Fiber(mid_f), new ocl.Fiber(stop_f));
			}
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: else if (!flat(start_f,mid_f,stop_f))
			else if (!flat(new ocl.Fiber(start_f), new ocl.Fiber(mid_f), new ocl.Fiber(stop_f)))
			{
				if (fw_step > min_sampling)
				{ // not a flat segment, and we have not reached maximum sampling
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: yfiber_adaptive_sample(span, start_t, mid_t, start_f, mid_f);
					yfiber_adaptive_sample(span, start_t, mid_t, new ocl.Fiber(start_f), new ocl.Fiber(mid_f));
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: yfiber_adaptive_sample(span, mid_t, stop_t, mid_f, stop_f);
					yfiber_adaptive_sample(span, mid_t, stop_t, new ocl.Fiber(mid_f), new ocl.Fiber(stop_f));
				}
			}
			else
			{
				yfibers.Add(stop_f);
			}
		}

		/// flatness predicate for fibers. Checks Fiber.size() and then calls flat() on cl-points

		// flat predicate to determine when we subdivide
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool flat(Fiber& start, Fiber& mid, Fiber& stop) const
		protected bool flat(Fiber start, Fiber mid, Fiber stop)
		{
			if (start.size() != stop.size()) // start, mid, and stop need to have same size()
			{
				return false;
			}
			else if (start.size() != mid.size())
			{
				return false;
			}
			else if (mid.size() != stop.size())
			{
				return false;
			}
			else
			{
				if (!start.empty())
				{ // all now have same size
					Debug.Assert(start.size() == stop.size() && start.size() == mid.size());
					for (uint n = 0;n < start.size();++n)
					{
						// now check for angles between cl-points (NOTE: cl-points might not belong to same loop?)
						// however, errors here only lead to dense sampling which is harmless (but slow)
						if ((!flat(start.upperCLPoint(n), mid.upperCLPoint(n), stop.upperCLPoint(n))))
						{
							return false;
						}
						else if (!flat(start.lowerCLPoint(n), mid.lowerCLPoint(n), stop.lowerCLPoint(n)))
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		/// flatness predicate for cl-points. checks for angle metween start-mid-stop
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: bool flat(Point start_cl, Point mid_cl, Point stop_cl) const
		protected bool flat(Point start_cl, Point mid_cl, Point stop_cl)
		{
			Point v1 = mid_cl - start_cl;
			Point v2 = stop_cl - mid_cl;
			v1.normalize();
			v2.normalize();
			double dotprod = v1.dot(v2);
			return (dotprod > cosLimit);
		}

	// DATA

		/// minimum x-coordinate
		protected double minx;
		/// maximum x-coordinate
		protected double maxx;
		/// minimum y-coordinate
		protected double miny;
		/// maximum y-coordinate
		protected double maxy;
		/// the minimum sampling interval when subdividing
		protected double min_sampling;
		/// the cosine limit value for cl-point flat(). In the constructor, cosLimit = 0.999 by default.
		protected double cosLimit;
}

} // end namespace




// end file adaptivewaterline.cpp
