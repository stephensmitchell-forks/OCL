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



//C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
//ORIGINAL LINE: #define isnan(x) _isnan(x)

namespace ocl
{

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class STLSurf;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Triangle;
//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class MillingCutter;

/// \brief base-class for low-level cam algorithms
///
/// base-class for cam algorithms
public abstract class Operation : System.IDisposable
{
		public Operation()
		{
		}
		public virtual void Dispose()
		{
			//std::cout << "~Operation()\n";
		}
		/// set the STL-surface and build kd-tree
		public virtual void setSTL(STLSurf s)
		{
			surf = s;
			foreach (Operation op in subOp)
			{
				op.setSTL(s);
			}
		}
		/// set the MillingCutter to use
		public virtual void setCutter(MillingCutter c)
		{
			cutter = c;
			foreach (Operation op in subOp)
			{
				op.setCutter(cutter);
			}
		}
		/// set number of OpenMP threads. Defaults to OpenMP::omp_get_num_procs()
		public void setThreads(uint n)
		{
			nthreads = n;
			foreach (Operation op in subOp)
			{
				op.setThreads(nthreads);
			}
		}
		/// return number of OpenMP threads
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int getThreads() const
		public int getThreads()
		{
			return (int)nthreads;
		}
		/// return the kd-tree bucket-size
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int getBucketSize() const
		public int getBucketSize()
		{
			return (int)bucketSize;
		}
		/// set the kd-tree bucket-size
		public void setBucketSize(uint s)
		{
			bucketSize = s;
			foreach (Operation op in subOp)
			{
				op.setBucketSize(bucketSize);
			}
		}
		/// return number of low-level calls
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: int getCalls() const
		public int getCalls()
		{
			return nCalls;
		}

		/// set the sampling interval for this Operation and all sub-operations
		public virtual void setSampling(double s)
		{
			sampling = s;
			foreach (Operation op in subOp)
			{
				op.setSampling(sampling);
			}
		}
		/// return the sampling interval
		public virtual double getSampling()
		{
			return sampling;
		}

		/// run the algorithm
		public abstract void run();
		/// run algorithm on a single input CLPoint
		public virtual void run(CLPoint cl)
		{
			Debug.Assert(false);
		}
		/// run push-cutter type algorithm on input Fiber
		public virtual void run(Fiber f)
		{
			Debug.Assert(false);
		}

		public virtual void reset()
		{
		}

		/// return CL-points
		public virtual List<CLPoint> getCLPoints()
		{
			List<CLPoint> clv = new List<CLPoint>();
			return clv;
		}
		public virtual void clearCLPoints()
		{
		}

		/// add an input CLPoint to this Operation
		public virtual void appendPoint(CLPoint p)
		{
		}
		/// used by batchpushcutter
		public virtual void setXDirection()
		{
		}
		/// used by batchpushcutter
		public virtual void setYDirection()
		{
		}
		/// add a fiber input to a push-cutter type operation
		public virtual void appendFiber(Fiber f)
		{
		}
		/// return the result of a push-cutter type operation
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: virtual ClassicVector<Fiber>* getFibers() const
		public virtual List<Fiber> getFibers()
		{
			return null;
		}

		/// sampling interval
		protected double sampling;
		/// how many low-level calls were made
		protected int nCalls;
		/// size of bucket-node in KD-tree
		protected uint bucketSize;
		/// the MillingCutter used
		protected readonly MillingCutter[] cutter;
		/// the STLSurf which we test against.
		protected readonly STLSurf surf;
		/// root of a kd-tree
		protected KDTree<Triangle> root;
		/// number of threads to use
		protected uint nthreads;
		/// sub-operations, if any, of this operation
		protected List<Operation> subOp = new List<Operation>();
}

} // end namespace

