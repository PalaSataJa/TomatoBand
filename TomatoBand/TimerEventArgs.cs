using System;
using Microsoft.Band;

namespace TomatoBand
{
	public class TimerEventArgs : EventArgs
	{
		public IBandClient BandClient { get; set; }
	}
}