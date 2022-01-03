using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace IOStreams
{

	public static class TestTasks
	{
		/// <summary>
		/// Parses Resourses\Planets.xlsx file and returns the planet data: 
		///   Jupiter     69911.00
		///   Saturn      58232.00
		///   Uranus      25362.00
		///    ...
		/// See Resourses\Planets.xlsx for details
		/// </summary>
		/// <param name="xlsxFileName">source file name</param>
		/// <returns>sequence of PlanetInfo</returns>
		public static IEnumerable<PlanetInfo> ReadPlanetInfoFromXlsx(string xlsxFileName)
		{
			var mainPackage = Package.Open(xlsxFileName, FileMode.OpenOrCreate);

			XElement xPlanets = XElement.Load(mainPackage.GetPart(new Uri("/xl/sharedStrings.xml", UriKind.Relative)).GetStream());
			XElement xRadius = XElement.Load(mainPackage.GetPart(new Uri("/xl/worksheets/sheet1.xml", UriKind.Relative)).GetStream());

			mainPackage.Close();

			var namePlanets = xPlanets.Elements().Elements().Select(x => x.Value).Take(8);
			var radiusResult = xRadius.Elements().Elements().Elements().Elements().Where(x => x.Value.ToString().Length > 3).Select(x => double.Parse(x.Value));		

			return namePlanets.Zip(radiusResult, (n, r) => new PlanetInfo() { Name = n, MeanRadius = r });
		}


		/// <summary>
		/// Calculates hash of stream using specifued algorithm
		/// </summary>
		/// <param name="stream">source stream</param>
		/// <param name="hashAlgorithmName">hash algorithm ("MD5","SHA1","SHA256" and other supported by .NET)</param>
		/// <returns></returns>
		public static string CalculateHash(this Stream stream, string hashAlgorithmName)
		{
            if (stream.Length == 0 || hashAlgorithmName == "unrecognized")
            {
                throw new ArgumentException();
            }

			using (var hashAlgoritm = HashAlgorithm.Create(hashAlgorithmName))
			{
				var b = hashAlgoritm.ComputeHash(stream);
				var sBuilder = new StringBuilder();
				stream.Position = 0;
				for (int i = 0; i < b.Length; i++)
				{
					sBuilder.Append(b[i].ToString("X2"));
				}

				return sBuilder.ToString();
			}
		}


		/// <summary>
		/// Returns decompressed strem from file. 
		/// </summary>
		/// <param name="fileName">source file</param>
		/// <param name="method">method used for compression (none, deflate, gzip)</param>
		/// <returns>output stream</returns>
		public static Stream DecompressStream(string fileName, DecompressionMethods method)
		{	

			using (FileStream originalStream = new FileStream(fileName, FileMode.Open))
            {
				int indexOfPoint = fileName.IndexOf('.');
				string newFileName = fileName.Substring(0, indexOfPoint) + method.ToString() + ".xlsx";
				if (method == DecompressionMethods.None)
                {
					int length = (int)originalStream.Length;
					var bytes = new byte[length];
					var x = originalStream.Read(bytes, 0, length);
					return new MemoryStream(bytes);
                }
				using (FileStream decompressedFileStream = File.Create(newFileName))
				{
					if (method == DecompressionMethods.Deflate)
                    {
						using (DeflateStream decompressionStream = new DeflateStream(originalStream, CompressionMode.Decompress))
						{
							decompressionStream.CopyTo(decompressedFileStream);							
						}
					}
                    else
                    {
						using (GZipStream decompressionStream = new GZipStream(originalStream, CompressionMode.Decompress))
						{
							decompressionStream.CopyTo(decompressedFileStream);							
						}
					}

					decompressedFileStream.Position = 0;
					int length = (int)decompressedFileStream.Length;
					var bytes = new byte[length];
					var x = decompressedFileStream.Read(bytes, 0, length);
					return new MemoryStream(bytes);
				}
			}
		}


		/// <summary>
		/// Reads file content econded with non Unicode encoding
		/// </summary>
		/// <param name="fileName">source file name</param>
		/// <param name="encoding">encoding name</param>
		/// <returns>Unicoded file content</returns>
		public static string ReadEncodedText(string fileName, string encoding)
		{
			using(StreamReader streamReader = new StreamReader(fileName, Encoding.GetEncoding(encoding)))
            {
				return streamReader.ReadToEnd();
            }
		}
	}


	public class PlanetInfo : IEquatable<PlanetInfo>
	{
		public string Name { get; set; }
		public double MeanRadius { get; set; }

		public override string ToString()
		{
			return string.Format("{0} {1}", Name, MeanRadius);
		}

		public bool Equals(PlanetInfo other)
		{
			return Name.Equals(other.Name)
				&& MeanRadius.Equals(other.MeanRadius);
		}
	}



}
