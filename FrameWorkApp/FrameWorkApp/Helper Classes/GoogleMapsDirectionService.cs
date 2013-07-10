using System;
using MonoTouch.CoreLocation;
using System.Net;
using Google.Maps;
using System.Collections.Generic;

namespace FrameWorkApp
{
	public class GoogleMapsDirectionService
	{
		private List<CLLocationCoordinate2D> coordinatesToPlotPathWith;
		const String requestUrl = "http://maps.google.com/maps/api/directions/xml";

		public GoogleMapsDirectionService (List<CLLocationCoordinate2D> coordsToPlotPath)
		{
			this.coordinatesToPlotPathWith = coordsToPlotPath;
		}
	
		public Boolean wasGoogleCalloutSuccessful(System.Xml.XmlDocument xmlString){
			var directionsResponseNode = xmlString.SelectSingleNode("DirectionsResponse");
			if (directionsResponseNode != null)
			{
				var statusNode = directionsResponseNode.SelectSingleNode("status");
				if (statusNode != null && statusNode.InnerText.Equals ("OK")) {
					Console.WriteLine ("StatusNode OK"+statusNode.InnerText);
					return true;
				} else{
					Console.WriteLine ("StatusNode not OK"+statusNode.InnerText);
					return false;
				}
			}
			return false;
		}

		public List<Google.Maps.Polyline> getPathsFromSingleCallout(System.Xml.XmlDocument xmlString){
			List<Google.Maps.Polyline> listofPolylinesObtainedFromSingleCallout = new List<Google.Maps.Polyline> ();
			var legs = xmlString.SelectNodes ("DirectionsResponse/route/leg");
			foreach (System.Xml.XmlNode singleLeg in legs) {
				//int stepCount = 1;
				var stepNodes = singleLeg.SelectNodes ("step");
				foreach (System.Xml.XmlNode singleStepNode in stepNodes) {
					var encodedPolylinePoints = singleStepNode.SelectSingleNode ("polyline/points").InnerText;
					Google.Maps.MutablePath currentMutablePath = new Google.Maps.MutablePath ();
					Polyline currentPolyline = new Polyline ();
					List<CLLocationCoordinate2D> decodedCoordinates = DecodePolylinePoints (encodedPolylinePoints);
					foreach (CLLocationCoordinate2D point in decodedCoordinates) {
						currentMutablePath.AddCoordinate (point);
					}
					currentPolyline.Path = currentMutablePath;
					listofPolylinesObtainedFromSingleCallout.Add (currentPolyline);
				}
			}
			return listofPolylinesObtainedFromSingleCallout;	
		}

		/*
		 * Source: https://groups.google.com/forum/embed/#!topic/google-maps-js-api-v3/0y0KoB-wWpw
		 * */
		private List<CLLocationCoordinate2D> DecodePolylinePoints(string encodedPoints) 
		{
			if (encodedPoints == null || encodedPoints == "") return null;
			List<CLLocationCoordinate2D> listOfPointsDecoded = new List<CLLocationCoordinate2D>();
			char[] polyLineChars = encodedPoints.ToCharArray();
			int index = 0;
			int currentLat = 0;
			int currentLng = 0;
			int next5Bits;
			int sum;
			int shifter;
			try
			{
				while (index < polyLineChars.Length)
				{
					// calculate next latitude
					sum = 0;
					shifter = 0;
					do
					{
						next5Bits = (int)polyLineChars[index++] - 63;
						sum |= (next5Bits & 31) << shifter;
						shifter += 5;
					} while (next5Bits >= 32 && index < polyLineChars.Length);

					if (index >= polyLineChars.Length)
						break;
					currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
					//calculate next longitude
					sum = 0;
					shifter = 0;
					do
					{
						next5Bits = (int)polyLineChars[index++] - 63;
						sum |= (next5Bits & 31) << shifter;
						shifter += 5;
					} while (next5Bits >= 32 && index < polyLineChars.Length);
					if (index >= polyLineChars.Length && next5Bits >= 32)
						break;
					currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
					CLLocationCoordinate2D p = new CLLocationCoordinate2D();
					p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
					p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
					listOfPointsDecoded.Add(p);
				} 
			}
			catch (Exception ex)
			{
				Console.WriteLine (ex.ToString ());
			}
			return listOfPointsDecoded;
		}

		public String createGoogleDirectionServiceCalloutString(int startingIndexOfCoordinatesToPlotPathWith)
		{
			String requestURLWithParameters = requestUrl;

			String originLatitude= coordinatesToPlotPathWith[startingIndexOfCoordinatesToPlotPathWith].Latitude.ToString();
			String originLongitude = coordinatesToPlotPathWith[startingIndexOfCoordinatesToPlotPathWith].Longitude.ToString();
			String wayPointsString = "";
			int lastPointinBlock = startingIndexOfCoordinatesToPlotPathWith;
			for(int j = startingIndexOfCoordinatesToPlotPathWith+1; j< startingIndexOfCoordinatesToPlotPathWith+9; j++){
				if(j+1 < coordinatesToPlotPathWith.Count){
					wayPointsString += coordinatesToPlotPathWith[j].Latitude + "," + coordinatesToPlotPathWith[j].Longitude + "|";
					lastPointinBlock = j;
				}
			}
			String destinationLatitude = coordinatesToPlotPathWith[lastPointinBlock+1].Latitude.ToString();
			String destinationLongitude = coordinatesToPlotPathWith[lastPointinBlock+1].Longitude.ToString();
			requestURLWithParameters += "?origin=" + originLatitude + "," + originLongitude + "&destination=" + destinationLatitude + "," + destinationLongitude;
			if(wayPointsString.Length >0){
				wayPointsString = wayPointsString.Substring(0, wayPointsString.Length - 1); //get rid of last "|"
				requestURLWithParameters += "&waypoints="+wayPointsString;
			}
			requestURLWithParameters += "&sensor=true&units=metric";
			Console.WriteLine("request URL"+requestURLWithParameters);
			
			return requestURLWithParameters;

		}

		public  List<Google.Maps.Polyline> performGoogleDirectionServiceApiCallout(){

			Console.WriteLine ("Number of coordinates" + coordinatesToPlotPathWith.Count);
			int numberOfCallouts = 0;
			List<Google.Maps.Polyline> allPolylinesToShowOnMap = new List<Google.Maps.Polyline> ();
			if (coordinatesToPlotPathWith.Count < 2) {
				return new List<Google.Maps.Polyline>();
			}
			try
			{
				for(int i = 0; i <coordinatesToPlotPathWith.Count; i = i +9){
					if(i+1 < coordinatesToPlotPathWith.Count){
						var client = new WebClient();
						String requestURLwithParameters = createGoogleDirectionServiceCalloutString(i);
						var calloutResultString = client.DownloadString(requestURLwithParameters);
						var calloutXMLDocument = new System.Xml.XmlDocument { InnerXml = calloutResultString };
						Boolean wasCalloutSuccessful = wasGoogleCalloutSuccessful(calloutXMLDocument);
						while(wasCalloutSuccessful == false){ // if call fails, keep calling
							//sleep 2 seconds ...
							System.Threading.Thread.Sleep(2000);
							calloutResultString = client.DownloadString(requestURLwithParameters);
							calloutXMLDocument = new System.Xml.XmlDocument { InnerXml = calloutResultString };
							wasCalloutSuccessful = wasGoogleCalloutSuccessful(calloutXMLDocument);
						}
						numberOfCallouts++;
						List<Polyline> listOfPolyLinesForThisCallout = getPathsFromSingleCallout(calloutXMLDocument);
						foreach(Polyline eachPolyLine in listOfPolyLinesForThisCallout){
							allPolylinesToShowOnMap.Add(eachPolyLine);
						}
					}
				}
				Console.WriteLine ("Number of coordinates" + coordinatesToPlotPathWith.Count);
				Console.WriteLine ("Number of callouts" + numberOfCallouts);
				return allPolylinesToShowOnMap;
			}
			catch(Exception e) {
				//do something
			}
			return null;
		}
	}
}
