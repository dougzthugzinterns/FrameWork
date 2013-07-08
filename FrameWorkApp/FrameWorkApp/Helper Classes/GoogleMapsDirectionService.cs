using System;
using MonoTouch.CoreLocation;
using System.Net;
using Google.Maps;
using System.Collections.Generic;

namespace FrameWorkApp
{
	public class GoogleMapsDirectionService
	{
		List<CLLocationCoordinate2D> coordinatesToPlotPathWith;
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
					Console.WriteLine ("StatusNode OK");
					return true;
				} else{
					Console.WriteLine ("StatusNode not OK");
					return false;
				}
			}
			return false;
		}

		public List<Google.Maps.Polyline> getPathsFromSingleCallout(System.Xml.XmlDocument xmlString){
			List<Google.Maps.Polyline> listofPolylinesObtainedFromSingleCallout = new List<Google.Maps.Polyline> ();
			var legs = xmlString.SelectNodes ("DirectionsResponse/route/leg");
			foreach (System.Xml.XmlNode leg in legs) {
				//int stepCount = 1;
				var stepNodes = leg.SelectNodes ("step");
				foreach (System.Xml.XmlNode stepNode in stepNodes) {
					var encodedPolylinePoints = stepNode.SelectSingleNode ("polyline/points").InnerText;
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
			List<CLLocationCoordinate2D> poly = new List<CLLocationCoordinate2D>();
			char[] polylinechars = encodedPoints.ToCharArray();
			int index = 0;

			int currentLat = 0;
			int currentLng = 0;
			int next5bits;
			int sum;
			int shifter;
			try
			{
				while (index < polylinechars.Length)
				{
					// calculate next latitude
					sum = 0;
					shifter = 0;
					do
					{
						next5bits = (int)polylinechars[index++] - 63;
						sum |= (next5bits & 31) << shifter;
						shifter += 5;
					} while (next5bits >= 32 && index < polylinechars.Length);

					if (index >= polylinechars.Length)
						break;

					currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

					//calculate next longitude
					sum = 0;
					shifter = 0;
					do
					{
						next5bits = (int)polylinechars[index++] - 63;
						sum |= (next5bits & 31) << shifter;
						shifter += 5;
					} while (next5bits >= 32 && index < polylinechars.Length);

					if (index >= polylinechars.Length && next5bits >= 32)
						break;

					currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
					CLLocationCoordinate2D p = new CLLocationCoordinate2D();
					p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
					p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
					poly.Add(p);
				} 
			}
			catch (Exception ex)
			{
				Console.WriteLine (ex.ToString ());
			}
			return poly;
		}

		public  List<Google.Maps.Polyline> performGoogleDirectionServiceApiCallout(){

			int numberOfCallouts = 0;
			foreach (CLLocationCoordinate2D coord in coordinatesToPlotPathWith) {
				//Console.WriteLine ("Coordinates while turning "+coord.Latitude.ToString() + ","+coord.Longitude.ToString());
			}
			//CLLocationCoordinate2D[][] calloutArrayOfCoordinates = new CLLocationCoordinate2D[][] { };
			List<Google.Maps.Polyline> allPolylinesToShowOnMap = new List<Google.Maps.Polyline> ();
			var requestUrl = "http://maps.google.com/maps/api/directions/xml?origin=";
			if (coordinatesToPlotPathWith.Count < 2) {
				return new List<Google.Maps.Polyline>();
			}
			try
			{

				for(int i = 0; i <coordinatesToPlotPathWith.Count; i = i +9){
					if(i+1 < coordinatesToPlotPathWith.Count){
						String originLatitude= coordinatesToPlotPathWith[i].Latitude.ToString();
						String originLongitude = coordinatesToPlotPathWith[i].Longitude.ToString();
						String wayPointsString = "";
						int lastPointinBlock = i;
						for(int j = i+1; j< i+9; j++){
							if(j+1 < coordinatesToPlotPathWith.Count){
								wayPointsString += coordinatesToPlotPathWith[j].Latitude + "," + coordinatesToPlotPathWith[j].Longitude + "|";
								lastPointinBlock = j;
							}
						}
						wayPointsString = wayPointsString.Substring(0, wayPointsString.Length - 1); //get rid of last "|"
						String destinationLatitude = coordinatesToPlotPathWith[lastPointinBlock+1].Latitude.ToString();
						String destinationLongitude = coordinatesToPlotPathWith[lastPointinBlock+1].Longitude.ToString();
						var client = new WebClient();
						String requestURLwithParameters = requestUrl + coordinatesToPlotPathWith[i].Latitude + "," + coordinatesToPlotPathWith[i].Longitude + "&destination=" + coordinatesToPlotPathWith[i+1].Latitude + "," + coordinatesToPlotPathWith[i+1].Longitude;
						requestURLwithParameters += (wayPointsString.Length > 0)? "&waypoints="+wayPointsString : "";
						requestURLwithParameters += "&sensor=true&units=metric";
						Console.WriteLine("request URL"+requestURLwithParameters);

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
