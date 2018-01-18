using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Microsoft.AppCenter.Distribute;
using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Push;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using FaceOff.Helpers;

namespace FaceOff
{
	public class App : Application
	{
        public static string APPCENTER_INSTALLID;
		public static bool IsBounceButtonAnimationInProgress;

		readonly PicturePage _picturePage = new PicturePage();

		public App()
		{
            var mainNavPage = new NavigationPage(_picturePage);
            mainNavPage.BarBackgroundColor = Color.Green;//Color.FromHex("#188FA7");

            MainPage = mainNavPage;

            AppCenter.LogLevel = LogLevel.Verbose;
            Distribute.ReleaseAvailable = OnReleaseAvailable;
            Distribute.SetEnabledAsync(true);
            Analytics.SetEnabledAsync(true);

            AppCenter.Start($"android={Keys.AndroidAppCenterKey}" +
                            $"ios={Keys.iOSAppCenterKey}",
                            typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));

            Push.PushNotificationReceived += async (sender, e) => {

                // Add the notification message and title to the message
                var summary = $"Push notification received:" +
                                    $"\n\tNotification title: {e.Title}" +
                                    $"\n\tMessage: {e.Message}";

                // If there is custom data associated with the notification,
                // print the entries
                if (e.CustomData != null)
                {
                    summary += "\n\tCustom data:\n";
                    foreach (var key in e.CustomData.Keys)
                    {
                        summary += $"\t\t{key} : {e.CustomData[key]}\n";
                    }
                }

                await Current.MainPage.DisplayAlert($"Notification - {e.Title}", e.Message, "OK");

            };

            Settings.Username = "mahdi";

		}

        static bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            // Look at releaseDetails public properties to get version information, release notes text or release notes URL
            string versionName = releaseDetails.ShortVersion;
            string versionCodeOrBuildNumber = releaseDetails.Version;
            string releaseNotes = releaseDetails.ReleaseNotes;
            Uri releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

            // custom dialog
            var title = "Version " + versionName + " available!";
            Task answer;

            // On mandatory update, user cannot postpone
            //if (releaseDetails.MandatoryUpdate)
            //{
            answer = App.Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install");
            //}
            //else
            //{
            //  answer = App.Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install", "Maybe tomorrow...");
            //}
            answer.ContinueWith((task) =>
            {
                // If mandatory or if answer was positive
                //if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                //{
                // Notify SDK that user selected update
                Distribute.NotifyUpdateAction(UpdateAction.Update);
                //}
                //else
                //{
                //  // Notify SDK that user selected postpone (for 1 day)
                //  // Note that this method call is ignored by the SDK if the update is mandatory
                //  Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                //}
            });

            // Return true if you are using your own dialog, false otherwise
            return true;
        }

		protected async override void OnStart()
		{
            // Handle when your app starts
            var id = await AppCenter.GetInstallIdAsync();
            APPCENTER_INSTALLID = id.ToString();

            var customprops = new CustomProperties();
            customprops.Set("username", "mahdi");

            customprops.Set("installID", APPCENTER_INSTALLID);

            AppCenter.SetCustomProperties(customprops);

            AnalyticsHelper.TrackEvent("App Started");

		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
#if DEBUG
		public void UseDefaultImageForPhoto1()
		{
			_picturePage.SetPhotoImage1("Happy");
		}

		public void UseDefaultImageForPhoto2()
		{
			_picturePage.SetPhotoImage2("Happy");
		}
#endif
	}
}

