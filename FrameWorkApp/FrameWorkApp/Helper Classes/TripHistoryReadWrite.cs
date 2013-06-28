using System;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using System.Collections;
using System.IO;

namespace FrameWorkApp
{
	public class TripHistoryReadWrite
	{
		String libraryCache;
		String filePath;

		public TripHistoryReadWrite (Boolean clearExistingFile)
		{
			libraryCache= Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "..", "Library", "Caches");
			filePath=Path.Combine (libraryCache, "currentTrip_SafeDrivingMate.txt");
			if (clearExistingFile) {
				File.WriteAllText (filePath, "");
			}
		}


		public  void addDataToTripHistoryFile(Trip newTrip){
			FileStream currentTripFile_FileStream = File.Open (filePath,FileMode.Append);
			StreamWriter currentTripFile_SteamWriter = new StreamWriter (currentTripFile_FileStream);
			newTrip.DateTime.ToString ("MM/dd/yyyy h:mmtt");
			currentTripFile_SteamWriter.WriteLine (newTrip.DateTime.ToString ("MM/dd/yyyy h:mmtt")+","+newTrip.NumberOfEvents);

			currentTripFile_SteamWriter.Close ();
			currentTripFile_FileStream.Close ();
		}

		public  Trip[] readDataFromTripHistoryFile(){
			ArrayList temporaryArrayListForData = new ArrayList();

			foreach (String line in File.ReadLines (filePath)) {
				String[] splitLine = line.Split (',');
				Trip newTrip = new Trip (DateTime.ParseExact(splitLine[0], "MM/dd/yyyy h:mmtt",null), int.Parse(splitLine[1]));
				temporaryArrayListForData.Add (newTrip);
			}

			return (Trip[])temporaryArrayListForData.ToArray(typeof(Trip));

		}
	}
}

