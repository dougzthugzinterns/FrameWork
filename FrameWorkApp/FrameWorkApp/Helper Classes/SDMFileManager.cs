using System;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using System.Collections;
using System.IO;

namespace FrameWorkApp
{
	public class SDMFileManager
	{
		String libraryAppFolder;
		String currentTripEventFile;
		String currentTripDistanceFile;
		String tripLogFile;

		public SDMFileManager ()
		{
			//Set Paths for All Files
			var libraryCache= Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "..", "Library", "Caches");
			libraryAppFolder= Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "..", "Library", "SafeDrivingMate");

			currentTripEventFile=Path.Combine (libraryCache, "currentTripEvents_SafeDrivingMate.txt");
			tripLogFile=Path.Combine (libraryAppFolder, "tripHistory.txt");
			currentTripDistanceFile=Path.Combine (libraryCache, "currentTripDistance_SafeDrivingMate.txt");

			//Make Sure all files and folders already exist if not create them
			if (!Directory.Exists (libraryAppFolder)) {
				Directory.CreateDirectory (libraryAppFolder);
				File.Create (tripLogFile);
			}
			if (!File.Exists (currentTripDistanceFile)) {
				File.Create (currentTripDistanceFile);
			}
			if (!File.Exists (currentTripEventFile)) {
				File.Create (currentTripEventFile);
			}

			//Console.WriteLine ("Trip Log Path:"+ tripLogFile);
			//Console.WriteLine ("Current Trip Event File Path: " + currentTripEventFile);
			//Console.WriteLine ("Current Trip Distance File Path: " + currentTripDistanceFile);

		}

		//Trip Log Methods

		//Adds Trip to Trip Log
		public  void addDataToTripLogFile(Trip newTrip){
			FileStream currentTripFile_FileStream = File.Open (tripLogFile,FileMode.Append);
			StreamWriter currentTripFile_SteamWriter = new StreamWriter (currentTripFile_FileStream);
			newTrip.DateTime.ToString ("MM/dd/yyyy h:mmtt");
			currentTripFile_SteamWriter.WriteLine (newTrip.DateTime.ToString ("MM/dd/yyyy h:mmtt")+","+newTrip.NumberOfEvents);

			currentTripFile_SteamWriter.Close ();
			currentTripFile_FileStream.Close ();
			Console.WriteLine ("Trip History Updated");
		}

		//Reads Trip Log File back in returning a array of Trip objects.
		public  Trip[] readDataFromTripLogFile(){
			ArrayList temporaryArrayListForData = new ArrayList();

			foreach (String line in File.ReadLines (tripLogFile)) {
				String[] splitLine = line.Split (',');
				Trip newTrip = new Trip (DateTime.ParseExact(splitLine[0], "MM/dd/yyyy h:mmtt",null), int.Parse(splitLine[1]));
				temporaryArrayListForData.Add (newTrip);
			}

			return (Trip[])temporaryArrayListForData.ToArray(typeof(Trip));

		}

		//Clear Trip Log File
		public void clearTripLogFile(){
			File.WriteAllText (tripLogFile, "");
		}

		//Trip Event File Methods

		//Add Event Cooridnate to Current Trip Event File
		public  void addEventToTripEventFile(CLLocationCoordinate2D newCoordiante){
			FileStream currentTripFile_FileStream = File.Open (currentTripEventFile,FileMode.Append);
			StreamWriter currentTripFile_SteamWriter = new StreamWriter (currentTripFile_FileStream);

			currentTripFile_SteamWriter.WriteLine (newCoordiante.Latitude+","+newCoordiante.Longitude);

			currentTripFile_SteamWriter.Close ();
			currentTripFile_FileStream.Close ();
		}

		//Reads Current Trip Event File back in and returns a Array of CLLocationCordinates2D objects.
		public  CLLocationCoordinate2D[] readDataFromTripEventFile(){
			ArrayList temporaryArrayListForData = new ArrayList();

			foreach (String line in File.ReadLines (currentTripEventFile)) {
				String[] splitLine = line.Split (',');
				CLLocationCoordinate2D newCoordinate = new CLLocationCoordinate2D (Double.Parse(splitLine[0]), Double.Parse(splitLine[1]));
				temporaryArrayListForData.Add (newCoordinate);
			}
			return (CLLocationCoordinate2D[])temporaryArrayListForData.ToArray(typeof(CLLocationCoordinate2D));
		}

		//Clears Current Trip Event File
		public void clearCurrentTripEventFile(){
			File.WriteAllText (currentTripEventFile, "");
		}

		//Trip Distance File Methods

		//Adds a location to the Current ,ooTrip Distance File
		public  void addLocationToTripDistanceFile(CLLocationCoordinate2D newCoordiante){
			FileStream currentTripFile_FileStream = File.Open (currentTripDistanceFile,FileMode.Append);
			StreamWriter currentTripFile_SteamWriter = new StreamWriter (currentTripFile_FileStream);

			currentTripFile_SteamWriter.WriteLine (newCoordiante.Latitude+","+newCoordiante.Longitude);

			currentTripFile_SteamWriter.Close ();
			currentTripFile_FileStream.Close ();
		}

		//Reads Current Trip Distance File back in returns a Array of CLLocation objects.
		public  CLLocation[] readDataFromTripDistanceFile(){
			ArrayList temporaryArrayListForData = new ArrayList();

			foreach (String line in File.ReadLines (currentTripDistanceFile)) {
				String[] splitLine = line.Split (',');
				CLLocation newCoordinate = new CLLocation(Double.Parse(splitLine[0]), Double.Parse(splitLine[1]));
				temporaryArrayListForData.Add (newCoordinate);
			}
			return (CLLocation[])temporaryArrayListForData.ToArray(typeof(CLLocation));
		}

		//Clears Current Trip Distance File.
		public void clearCurrentTripDistanceFile(){
			File.WriteAllText (currentTripDistanceFile, "");
		}



	}
}

