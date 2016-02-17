using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace XFAedSearch.Droid.Helpers
{
	public static class Settings
	{
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region Setting Constants

		const string LatitudeKey = "Latitude";
		// 東京都庁の緯度、経度
		private static readonly double LatitudeDefault = 35.689521;

		const string LongitudeKey = "Longitude";
		// 東京都庁の緯度、経度
		private static readonly double LongitudeDefault = 139.691704;

		const string ZoomLevelKey = "ZoomLevel";
		private static readonly float ZoomLevelDefault = 16.5f;

		#endregion


		public static double Latitude
		{
			get { return AppSettings.GetValueOrDefault<double>(LatitudeKey, LatitudeDefault); }
			set { AppSettings.AddOrUpdateValue<double>(LatitudeKey, value); }
		}

		public static double Longitude
		{
			get { return AppSettings.GetValueOrDefault<double>(LongitudeKey, LongitudeDefault); }
			set { AppSettings.AddOrUpdateValue<double>(LongitudeKey, value); }
		}

		public static float ZoomLevel
		{
			get { return AppSettings.GetValueOrDefault<float>(ZoomLevelKey, ZoomLevelDefault); }
			set { AppSettings.AddOrUpdateValue<float>(ZoomLevelKey, value); }
		}
	}
}
