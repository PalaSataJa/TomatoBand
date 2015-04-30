using System;
using System.Reflection;
using Microsoft.Band;

namespace TomatoBand
{
	public class TimerEventArgs : EventArgs
	{
		public int Elapsed { get; set; }
	}
}