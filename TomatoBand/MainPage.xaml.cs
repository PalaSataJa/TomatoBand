// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band;
using Microsoft.Band.Notifications;

namespace TomatoBand
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private const string PairedMessage =
			"Once you click &quot;Start Tomatoes&quot; a timer will start and you Band will vibrate in indication.";

		private const string NotPairedMessage =
			"This sample requires a Microsoft Band paired to your phone. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";

		public MainPage()
		{
			InitializeComponent();

			NavigationCacheMode = NavigationCacheMode.Required;
		}

		/// <summary>
		/// Connect to Microsoft Band, create a Tile and send notifications.
		/// </summary>
		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Get the list of Microsoft Bands paired to the phone.
				IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
				if (pairedBands.Length < 1)
				{
					TextBlock.Text = NotPairedMessage;
					return;
				}

				// Connect to Microsoft Band.
				using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
				{
					// Send a notification.
					await
						bandClient.NotificationManager.VibrateAsync(VibrationType.NotificationTwoTone);

					int tomato, shortBreak, largeBreak;
					int.TryParse(TomatoBox.Text, out tomato);
					int.TryParse(BreakBox.Text, out shortBreak);
					int.TryParse(LargeBreakBox.Text, out largeBreak);
					var timer = new Timer(tomato, shortBreak, largeBreak, bandClient);
					timer.OnTomatoFinished += timer_OnTomatoFinished;
					timer.OnBreakFinished += timer_OnBreakFinished;
					timer.OnGroupComplete += timer_OnGroupComplete;
				}
			}
			catch (Exception ex)
			{
				TextBlock.Text = ex.ToString();
			}
		}

		private void timer_OnGroupComplete(object sender, TimerEventArgs args)
		{
			args.BandClient.NotificationManager.VibrateAsync(VibrationType.NotificationTimer);
		}

		private void timer_OnBreakFinished(object sender, TimerEventArgs args)
		{
			args.BandClient.NotificationManager.VibrateAsync(VibrationType.NotificationTwoTone);
		}

		private void timer_OnTomatoFinished(object sender, TimerEventArgs args)
		{
			args.BandClient.NotificationManager.VibrateAsync(VibrationType.RampUp);
		}


		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.
		/// This parameter is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// TODO: Prepare page for display here.

			// TODO: If your application contains multiple pages, ensure that you are
			// handling the hardware Back button by registering for the
			// Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
			// If you are using the NavigationHelper provided by some templates,
			// this event is handled for you.

			TextBlock.Text = PairedMessage;
		}
	}
}