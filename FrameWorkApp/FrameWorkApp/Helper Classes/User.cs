using System;

namespace FrameWorkApp
{
	public class User
	{
		private double totalDistance = 0;
		private int totalNumberOfEvents = 0;
		private int totalPoints = 0;

		public User (double totalDistance, int totalNumberOfEvents, int totalPoints)
		{
			this.totalDistance = totalDistance;
			this.totalNumberOfEvents = totalNumberOfEvents;
			this.totalPoints = totalPoints;
		}

		public double TotalDistance {
			get { return totalDistance;}
			set { this.totalDistance = value;}
		}

		public int TotalNumberOfEvents {
			get { return totalNumberOfEvents;}
			set { this.totalNumberOfEvents = value;}
		}

		public int TotalPoints {
			get { return totalPoints;}
			set { this.totalPoints = value;}
		}

		public int updateData (double tripDistance, int tripNumberOfEvents)
		{
			totalDistance += tripDistance;
			totalNumberOfEvents += tripNumberOfEvents;

			totalPoints = Math.Max ((totalPoints + (int)tripDistance + ((-3) * tripNumberOfEvents)), 0);

			return (totalPoints + (int)tripDistance + ((-3) * tripNumberOfEvents));
		}
	}
}