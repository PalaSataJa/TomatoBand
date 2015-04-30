using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band;
using Microsoft.Band.Notifications;

namespace TomatoBand
{
	/// <summary>
	///   An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private const string PairedMessage =
			"Once you click &quot;Start Tomatoes&quot; a timer will start and you Band will vibrate in indication.";

		private const string NotPairedMessage =
			"This sample requires a Microsoft Band paired to your phone. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";

		private IBandClient _bandClient;
		private Timer _timer;

		public MainPage()
		{
			InitializeComponent();

			NavigationCacheMode = NavigationCacheMode.Required;
		}

		private async void StartButton_Click(object sender, RoutedEventArgs e)
		{
			int tomato, shortBreak, largeBreak;
			int.TryParse(TomatoBox.Text, out tomato);
			int.TryParse(BreakBox.Text, out shortBreak);
			int.TryParse(LargeBreakBox.Text, out largeBreak);
			_timer = new Timer(tomato, shortBreak, largeBreak);
			SubscribeTimer();
			_timer.Start();

			await
				_bandClient.NotificationManager.VibrateAsync(VibrationType.NotificationTwoTone);

			StartButton.IsEnabled = false;
			StopButton.IsEnabled = true;
		}

		private void SubscribeTimer()
		{
			_timer.OnTomatoFinished += timer_OnTomatoFinished;
			_timer.OnBreakFinished += timer_OnBreakFinished;
			_timer.OnGroupComplete += timer_OnGroupComplete;
			_timer.OnTick += timer_OnTick;
		}

		private async void StopButton_Click(object sender, RoutedEventArgs e)
		{
			_timer.Stop();
			StartButton.IsEnabled = true;
			StopButton.IsEnabled = false;
		}

		private void timer_OnTick(object sender, TimerEventArgs args)
		{
			SecondBlock.Text = string.Format("Elapsed {0:mm\\:ss}", TimeSpan.FromSeconds(args.Elapsed));
		}

		private void timer_OnGroupComplete(object sender, TimerEventArgs args)
		{
			_bandClient.NotificationManager.VibrateAsync(VibrationType.NotificationTimer);
			TextBlock.Text = "group";
		}

		private void timer_OnBreakFinished(object sender, TimerEventArgs args)
		{
			_bandClient.NotificationManager.VibrateAsync(VibrationType.NotificationTwoTone);
			TextBlock.Text = "break";
		}

		private void timer_OnTomatoFinished(object sender, TimerEventArgs args)
		{
			_bandClient.NotificationManager.VibrateAsync(VibrationType.RampUp);
			TextBlock.Text = "tomato";
		}


		/// <summary>
		///   Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">
		///   Event data that describes how this page was reached.
		///   This parameter is typically used to configure the page.
		/// </param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.NavigationMode != NavigationMode.New)
			{
				return;
			}
			IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
			IBandInfo band = pairedBands.FirstOrDefault();
			if (band == null)
			{
				TextBlock.Text = NotPairedMessage;
				return;
			}
			TextBlock.Text = PairedMessage;

			StopButton.IsEnabled = false;
			_bandClient = await BandClientManager.Instance.ConnectAsync(band);
		}
	}
}