using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace XFAedSearch.Models
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

		const string RegionLatitudeKey = "RegionLatitude";
		// 東京都庁の緯度、経度
		private static readonly double RegionLatitudeDefault = 35.689521;

		const string RegionLongitudeKey = "RegionLongitude";
		// 東京都庁の緯度、経度
		private static readonly double RegionLongitudeDefault = 139.691704;

		const string RegionRadiusKey = "RegionRadius";
		private static readonly double RegionRadiusDefault = 250;

		#endregion


		public static double RegionLatitude
		{
			get { return AppSettings.GetValueOrDefault<double>(RegionLatitudeKey, RegionLatitudeDefault); }
			set { AppSettings.AddOrUpdateValue<double>(RegionLatitudeKey, value); }
		}

		public static double RegionLongitude
		{
			get { return AppSettings.GetValueOrDefault<double>(RegionLongitudeKey, RegionLongitudeDefault); }
			set { AppSettings.AddOrUpdateValue<double>(RegionLongitudeKey, value); }
		}

		public static double RegionRadius
		{
			get { return AppSettings.GetValueOrDefault<double>(RegionRadiusKey, RegionRadiusDefault); }
			set { AppSettings.AddOrUpdateValue<double>(RegionRadiusKey, value); }
		}
	}
}

