//  stlreader.cpp
// 
//  Written by Dan Heeks
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


namespace ocl
{

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class STLSurf;


/// \brief STL file reader, reads an STL file and calls addTriangle on the STLSurf
///

public class STLReader : System.IDisposable
{
		public STLReader()
		{
		}
		/// construct with file name and surface to fill
		public STLReader(string filepath, STLSurf surface)
		{
			read_from_file(filepath, surface);
		}

		/// destructor
		public virtual void Dispose()
		{
			//delete tris;
		}

		/// read STL-surface from file
		private void read_from_file(string filepath, STLSurf surface)
		{
			// read the stl file
			std::ifstream ifs = new std::ifstream(ocl.GlobalMembers.Ttc(filepath), ios.binary);
			if (ifs == null)
			{
				return;
			}

			const string solid_string = "aaaaa";
			ifs.read(solid_string, 5);
			if (ifs.eof())
			{
				return;
			}
			if (string.Compare(solid_string, "solid"))
			{
				// try binary file read

				// read the header
				string header = new string(new char[81]);
				header = header.Substring(0, 80);
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
				memcpy(header, solid_string, 5);
				ifs.read(header[5], 75);

				uint num_facets = 0;
				ifs.read((string)(num_facets), 4);

				for (uint i = 0; i < num_facets; i++)
				{
					float[] n = new float[3];
					float[][] x =
					{
						new float[3],
						new float[3],
						new float[3]
					};
					ifs.read((string)(n), 12);
					ifs.read((string)(x[0]), 36);
					short attr;
					ifs.read((string)(attr), 2);
					surface.addTriangle(new Triangle(new Point(x[0][0], x[0][1], x[0][2]), new Point(x[1][0], x[1][1], x[1][2]), new Point(x[2][0], x[2][1], x[2][2])));
				}
			}
			else
			{
				// "solid" already found
				const string str = "solid";
				ifs.getline(str[5], 1024);
				//char title[1024];
				//if(sscanf(str, "solid %s", title) == 1)
				//m_title.assign(Ctt(title));

				float[] n = new float[3];
				float[][] x =
				{
					new float[3],
					new float[3],
					new float[3]
				};
				const string five_chars = "aaaaa";

				int vertex = 0;

				while (!ifs.eof())
				{
					ifs.getline(str, 1024);

					int i = 0;
					int j = 0;
					for (; i < 5; i++, j++)
					{
						if (str[j] == 0)
						{
							break;
						}
						while (str[j] == ' ' || str[j] == '\t')
						{
							j++;
						}
						five_chars = StringFunctions.ChangeCharacter(five_chars, i, str[j]);
					}
					if (i == 5)
					{
						if (!string.Compare(five_chars, "verte"))
						{
#if WIN32
							sscanf(str, " vertex %f %f %f", (x[vertex][0]), (x[vertex][1]), (x[vertex][2]));
#else
							std::istringstream ss = new std::istringstream(str);
							ss.imbue(std::locale("C"));
							while (ss.peek() == ' ')
							{
								ss.seekg(1, ios_base.cur);
							}
							ss.seekg("vertex".size(), ios_base.cur);
							ss >> x[vertex][0] >> x[vertex][1] >> x[vertex][2];
#endif
							vertex++;
							if (vertex > 2)
							{
								vertex = 2;
							}
						}
						else if (!string.Compare(five_chars, "facet"))
						{
#if WIN32
							sscanf(str, " facet normal %f %f %f", (n[0]), (n[1]), (n[2]));
#else
							std::istringstream ss = new std::istringstream(str);
							ss.imbue(std::locale("C"));
							while (ss.peek() == ' ')
							{
								ss.seekg(1, ios_base.cur);
							}
							ss.seekg("facet normal".size(), ios_base.cur);
							ss >> n[0] >> n[1] >> n[2];
#endif
							vertex = 0;
						}
						else if (!string.Compare(five_chars, "endfa"))
						{
							if (vertex == 2)
							{
								surface.addTriangle(new Triangle(new Point(x[0][0], x[0][1], x[0][2]), new Point(x[1][0], x[1][1], x[1][2]), new Point(x[2][0], x[2][1], x[2][2])));
							}
						}
					}
				}
			}
		}
}

}



