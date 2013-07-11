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
		String userFile;

		public SDMFileManager ()
		{
			//Set Paths for All Files
			var libraryCache = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "..", "Library", "Caches");
			libraryAppFolder = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "..", "Library", "SafeDrivingMate");

			currentTripEventFile = Path.Combine (libraryCache, "currentTripEvents_SafeDrivingMate.txt");
			tripLogFile = Path.Combine (libraryAppFolder, "tripHistory.txt");
			currentTripDistanceFile = Path.Combine (libraryCache, "currentTripDistance_SafeDrivingMate.txt");
			userFile = Path.Combine (libraryAppFolder, "userData.txt");

			//Make Sure all files and folders already exist if not create them
			if (!Directory.Exists (libraryAppFolder)) {
				Directory.CreateDirectory (libraryAppFolder);
				File.Create (tripLogFile).Close ();
			}
			if (!File.Exists (currentTripDistanceFile)) {
				File.Create (currentTripDistanceFile).Close ();
			}
			if (!File.Exists (currentTripEventFile)) {
				File.Create (currentTripEventFile).Close ();
			}
			if (!File.Exists (userFile)) {
				File.Create (userFile).Close ();
				File.WriteAllText (userFile, "0, 0, 0");
			}

			Console.WriteLine ("Trip Log Path:" + tripLogFile);
			Console.WriteLine ("Current Trip Event File Path: " + currentTripEventFile);
			Console.WriteLine ("Current Trip Distance File Path: " + currentTripDistanceFile);
		}

		//Overall Helper Methods
		//Returns True of Trip is still in progress, false if not.
		public Boolean currentTripInProgress ()
		{
			if (File.Exists (currentTripEventFile) && File.Exists (currentTripDistanceFile)) {
				if (File.ReadAllText (currentTripEventFile) != "" || File.ReadAllText (currentTripDistanceFile) != "") {
					return true;
				}
			}
			return false;
		}

		//Get Last Date of Any point being entered into a file
		public DateTime getDateOfLastPointEnteredInCurrentTrip ()
		{
			DateTime eventFileDateTime = File.GetLastWriteTime (currentTripEventFile);
			DateTime distanceFileDateTime = File.GetLastWriteTime (currentTripDistanceFile);

			if (eventFileDateTime.CompareTo (distanceFileDateTime) > 0) {
				return eventFileDateTime;
			} else {
				return distanceFileDateTime;
			}
		}

		//Trip Log Methods
		//Adds Trip to Trip Log
		public void addDataToTripLogFile (Trip newTrip)
		{
			FileStream currentTripFile_FileStream = File.Open (tripLogFile, FileMode.Append);
			StreamWriter currentTripFile_SteamWriter = new StreamWriter (currentTripFile_FileStream);
			newTrip.DateTime.ToString ("MM/dd/yyyy h:mmtt");
			currentTripFile_SteamWriter.WriteLine (newTrip.DateTime.ToString ("MM/dd/yyyy h:mmtt") + "," + newTrip.NumberOfEvents);

			currentTripFile_SteamWriter.Close ();
			currentTripFile_FileStream.Close ();
			Console.WriteLine ("Trip History Updated");
		}

		//Reads Trip Log File back in returning a array of Trip objects.
		public Trip[] readDataFromTripLogFile ()
		{
			ArrayList temporaryArrayListForData = new ArrayList ();

			foreach (String line in File.ReadLines (tripLogFile)) {
				String[] coordinatesSplitAtComma = line.Split (',');
				Trip newTrip = new Trip (DateTime.ParseExact (coordinatesSplitAtComma [0], "MM/dd/yyyy h:mmtt", null), int.Parse (coordinatesSplitAtComma [1]));
				temporaryArrayListForData.Add (newTrip);
			}
			return (Trip[])temporaryArrayListForData.ToArray (typeof(Trip));
		}

		//Clear Trip Log File
		public void clearTripLogFile ()
		{
			File.WriteAllText (tripLogFile, "");
		}

		//Trip Event File Methods
		//Add Event Cooridnate to Current Trip Event File
		public  void addEventToTripEventFile (Event newEvent)
		{
			FileStream currentTripFile_FileStream = File.Open (currentTripEventFile, FileMode.Append);
			StreamWriter currentTripFile_SteamWriter = new StreamWriter (currentTripFile_FileStream);

			currentTripFile_SteamWriter.WriteLine (newEvent.Location.Latitude + "," + newEvent.Location.Longitude + "," + newEvent.ColorNumber);

			currentTripFile_SteamWriter.Close ();
			currentTripFile_FileStream.Close ();
		}

		//Reads Current Trip Event File back in and returns a Array of CLLocationCordinates2D objects.
		public  Event[] readDataFromTripEventFile ()
		{
			ArrayList temporaryArrayListForCoordinateData = new ArrayList ();

			foreach (String line in File.ReadLines (currentTripEventFile)) {
				String[] coordinateDataSplitAtComma = line.Split (',');
				CLLocationCoordinate2D newCoordinate = new CLLocationCoordinate2D (Double.Parse (coordinateDataSplitAtComma [0]), Double.Parse (coordinateDataSplitAtComma [1]));
				Event newEvent = new Event (newCoordinate, int.Parse (coordinateDataSplitAtComma [2]));
				temporaryArrayListForCoordinateData.Add (newEvent);
			}
			return (Event[])temporaryArrayListForCoordinateData.ToArray (typeof(Event));
		}

		//Clears Current Trip Event File
		public void clearCurrentTripEventFile ()
		{
			File.WriteAllText (currentTripEventFile, "");
		}

		//Trip Distance File Methods
		//Adds a location to the Current ,ooTrip Distance File
		public  void addLocationToTripDistanceFile (CLLocationCoordinate2D newCoordiante)
		{
			FileStream currentTripFile_FileStream = File.Open (currentTripDistanceFile, FileMode.Append);
			StreamWriter currentTripFile_SteamWriter = new StreamWriter (currentTripFile_FileStream);

			currentTripFile_SteamWriter.WriteLine (newCoordiante.Latitude + "," + newCoordiante.Longitude);

			currentTripFile_SteamWriter.Close ();
			currentTripFile_FileStream.Close ();
		}

		//Reads Current Trip Distance File back in returns a Array of CLLocation objects.
		public  CLLocation[] readDataFromTripDistanceFile ()
		{
			ArrayList temporaryArrayListForData = new ArrayList ();

			foreach (String line in File.ReadLines (currentTripDistanceFile)) {
				String[] splitLine = line.Split (',');
				CLLocation newCoordinate = new CLLocation (Double.Parse (splitLine [0]), Double.Parse (splitLine [1]));
				temporaryArrayListForData.Add (newCoordinate);
			}
			return (CLLocation[])temporaryArrayListForData.ToArray (typeof(CLLocation));
		}

		//Clears Current Trip Distance File.
		public void clearCurrentTripDistanceFile ()
		{
			File.WriteAllText (currentTripDistanceFile, "");
		}

		//User Data File Methods
		public void updateUserFile (User updatedUser)
		{
			String dataToWrite = updatedUser.TotalDistance + "," + updatedUser.TotalNumberOfEvents + "," + updatedUser.TotalPoints;
			File.WriteAllText (userFile, dataToWrite);
		}

		public User readUserFile ()
		{
			String dataFromFile = File.ReadAllText (userFile);
			String[] splitDataFromFile = dataFromFile.Split (',');

			User userData = new User (Double.Parse(splitDataFromFile[0]), int.Parse(splitDataFromFile[1]), int.Parse(splitDataFromFile[2]));
			return userData;
		}

		public void clearUserFile ()
		{
			File.WriteAllText (userFile, "");
		}
	}
}