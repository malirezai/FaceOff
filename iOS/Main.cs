using UIKit;

using Xamarin;

namespace FaceOff.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			//Insights.Initialize(MobileCenterConstants.InsightsApiKey);

			//Insights.HasPendingCrashReport += (sender, isStartupCrash) =>
			//{
			//	if (isStartupCrash)
			//	{
			//		Insights.PurgePendingCrashReports().Wait();
			//	}
			//};

			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}

