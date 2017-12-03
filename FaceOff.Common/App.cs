﻿using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Microsoft.AppCenter.Distribute;
using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Push;

namespace FaceOff
{
	public class App : Application
	{
		public static bool IsBounceButtonAnimationInProgress;

		readonly PicturePage _picturePage = new PicturePage();

		public App()
		{
            var mainNavPage = new NavigationPage(_picturePage);
            mainNavPage.BarBackgroundColor = Color.FromHex("#1FAECE");

            MainPage = mainNavPage;

            AppCenter.LogLevel = LogLevel.Verbose;
            Distribute.ReleaseAvailable = OnReleaseAvailable;
            Distribute.SetEnabledAsync(true);
            Analytics.SetEnabledAsync(true);

            AppCenter.Start($"android={Keys.AndroidAppCenterKey}" +
                            $"ios={Keys.iOSAppCenterKey}",
                            typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
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

		protected override void OnStart()
		{
			// Handle when your app starts
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
