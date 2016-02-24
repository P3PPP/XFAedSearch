
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace XFAedSearch.Droid
{
	[Activity(Label = "@string/AppName", MainLauncher = true, NoHistory = true, Theme = "@style/Theme.Splash", 
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			var intent = new Intent(this, typeof(MainActivity));
			StartActivity(intent);
			Finish();
		}
	}
}

