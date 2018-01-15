using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.AppCenter.Analytics;

namespace FaceOff.Helpers
{
    public static class AnalyticsHelper
    {
        enum _pathType { Windows, Linux };

        public static Dictionary<string, string> userDetails = new Dictionary<string, string>{
            {"Username",Settings.Username.ToLower()},
        };

        public static void TrackEvent(string eventName)
        {
            Analytics.TrackEvent(eventName, userDetails);
        }

        public static void TrackEvent(string eventName, Dictionary<string, string> properties, Dictionary<string, double> measurements = null)
        {
            if(!properties.ContainsKey("Username"))
                properties.Add("Username", Settings.Username.ToLower());
            
            Analytics.TrackEvent(eventName, properties);
        }

        /// <summary>
        /// Reports a caught exception to AppCenter
        /// </summary>
        public static void Report(Exception exception, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerMembername = "")
        {
            userDetails.Add("filepath", filePath);
            userDetails.Add("lineNumber", lineNumber.ToString());
            userDetails.Add("callerMemberName", callerMembername);

            TrackEvent($"Caught Exception - {exception.Message}", userDetails);
        }

    }
}
