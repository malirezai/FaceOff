﻿using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace FaceOff.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        private const string UsernameKey = nameof(UsernameKey);

        public static string Username
        {
            get
            {
                return AppSettings.GetValueOrDefault(UsernameKey, "");
            }
            set
            {
                AppSettings.AddOrUpdateValue(UsernameKey, value);
            }
        }

    }
}