using System;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using System.Collections;
using System.IO;

namespace TripReadWrite
{
	public class TripCoordinateReadWrite
	{
		String libraryCache;
		String filePath;

		public TripCoordinateReadWrite ()
		{
			libraryCache= Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "..", "Library", "Caches");
			filePath=Path.Combine (libraryCache, "currentTrip_SafeDrivingMate.txt");
			File.WriteAllText(filePath, "");
		}



		public  void addDataToTripFile(CLLocationCoordinate2D newCoordiante){
			FileStream currentTripFile_FileStream = File.Open (filePath,FileMode.Append);
			StreamWriter currentTripFile_SteamWriter = new StreamWriter (currentTripFile_FileStream);

			Console.WriteLine (filePath);
			currentTripFile_SteamWriter.WriteLine (newCoordiante.Latitude+","+newCoordiante.Longitude);

			currentTripFile_SteamWriter.Close ();
			currentTripFile_FileStream.Close ();
		}

		public  CLLocationCoordinate2D[] readDataFromTripFile(){
			ArrayList temporaryArrayListForData = new ArrayList();

			foreach (String line in File.ReadLines (filePath)) {
				String[] splitLine = line.Split (',');
				CLLocationCoordinate2D newCoordinate = new CLLocationCoordinate2D (Double.Parse(splitLine[0]), Double.Parse(splitLine[1]));
				temporaryArrayListForData.Add (newCoordinate);
			}

			return (CLLocationCoordinate2D[])temporaryArrayListForData.ToArray(typeof(CLLocationCoordinate2D));

		}


	}
}

