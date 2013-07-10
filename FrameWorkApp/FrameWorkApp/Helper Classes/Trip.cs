using System;
using MonoTouch.CoreLocation;

namespace FrameWorkApp
{
	public class Trip
	{
		private DateTime dateTime;
		private int numberOfEvents;


		public Trip (DateTime dateTime, int numberOfEvents)
		{
			this.dateTime = dateTime;
			this.numberOfEvents = numberOfEvents;
		}

		public DateTime DateTime{
			get { return dateTime; }
			set { this.dateTime = value; }
		}

		public int NumberOfEvents{
			get { return numberOfEvents; }
			set { numberOfEvents= value; }
		}

		public override string ToString ()
		{
			return DateTime.ToString ("MM/dd/yyyy h:mmtt")+" "+numberOfEvents;
		}

	}
}

