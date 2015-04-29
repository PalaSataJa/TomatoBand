using System;
using Windows.UI.Xaml;
using Microsoft.Band;

namespace TomatoBand
{
	public class Timer
	{
		public delegate void TimerHandler(object sender, TimerEventArgs args);

		private readonly IBandClient _bandClient;
		private readonly int _breakDuration;
		private readonly int _largeBreakDuration;
		private readonly DispatcherTimer _timer;
		private readonly int _tomatoDuration;
		private Phase _currentPhase;
		private int _elapsed;
		private int _tomatoInAGroup;

		public Timer(int tomatoDuration, int breakDuration, int largeBreakDuration, IBandClient bandClient)
		{
			this._tomatoDuration = tomatoDuration;
			this._breakDuration = breakDuration;
			this._largeBreakDuration = largeBreakDuration;
			_bandClient = bandClient;

			_timer = new DispatcherTimer();
			_timer.Tick += timer_Tick;
			_timer.Interval = new TimeSpan(0, 0, 10);
		}

		public event TimerHandler OnTomatoFinished;
		public event TimerHandler OnBreakFinished;
		public event TimerHandler OnGroupComplete;

		private bool TryCallEvent(TimerHandler eventHandler)
		{
			if (eventHandler != null)
			{
				eventHandler(this, new TimerEventArgs {BandClient = _bandClient});
				return true;
			}
			return false;
		}

		private void timer_Tick(object sender, object e)
		{
			_elapsed++;
			switch (_currentPhase)
			{
				case Phase.Tomato:
					if (_elapsed == _tomatoDuration)
					{
						_elapsed = 0;
						_tomatoInAGroup++;
						if (_tomatoInAGroup == 4)
						{
							_currentPhase = Phase.LongBreak;
							;
						}
						else
						{
							_currentPhase = Phase.Break;
						}
						TryCallEvent(OnTomatoFinished);
					}
					break;
				case Phase.Break:
					if (_elapsed == _breakDuration)
					{
						_elapsed = 0;
						_currentPhase = Phase.Tomato;
						TryCallEvent(OnBreakFinished);
					}
					break;
				case Phase.LongBreak:
					if (_elapsed == _largeBreakDuration)
					{
						_elapsed = 0;
						_currentPhase = Phase.Tomato;
						TryCallEvent(OnGroupComplete);
					}
					break;
			}
		}

		public void Start()
		{
			_currentPhase = Phase.Tomato;
			_timer.Start();
		}
	}
}