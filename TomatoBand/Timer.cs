using System;
using Windows.UI.Xaml;

namespace TomatoBand
{
	public class Timer
	{
		public delegate void TimerHandler(object sender, TimerEventArgs args);

		private readonly int _breakDuration;
		private readonly int _largeBreakDuration;
		private readonly DispatcherTimer _timer;
		private readonly int _tomatoDuration;
		private Phase _currentPhase;
		private int _elapsed;
		private int _tomatoCounter;

		public Timer(int tomatoDuration, int breakDuration, int largeBreakDuration)
		{
			_tomatoDuration = tomatoDuration * 60;
			_breakDuration = breakDuration * 60;
			_largeBreakDuration = largeBreakDuration * 60;

			_timer = new DispatcherTimer();
			_timer.Tick += timer_Tick;
			_timer.Interval = new TimeSpan(0, 0, 1);
		}

		public event TimerHandler OnTomatoFinished;
		public event TimerHandler OnBreakFinished;
		public event TimerHandler OnGroupComplete;
		public event TimerHandler OnTick;

		private bool TryCallEvent(TimerHandler eventHandler, TimerEventArgs args = null)
		{
			if (args == null)
			{
				args = new TimerEventArgs();
			}
			if (eventHandler != null)
			{
				eventHandler(this, args);
				return true;
			}
			return false;
		}

		private void timer_Tick(object sender, object e)
		{
			_elapsed++;
			TryCallEvent(OnTick, new TimerEventArgs { Elapsed = _elapsed });
			switch (_currentPhase)
			{
				case Phase.Tomato:
					if (_elapsed == _tomatoDuration)
					{
						_elapsed = 0;
						_tomatoCounter++;
						if (_tomatoCounter % 4==0)
						{
							_currentPhase = Phase.LongBreak;
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
			_tomatoCounter = 0;
			_currentPhase = Phase.Tomato;
			_timer.Start();
		}

		public void Stop()
		{
			_timer.Stop();
		}
	}
}