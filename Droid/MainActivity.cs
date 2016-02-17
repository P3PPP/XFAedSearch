using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using Xamarin.Forms;

namespace XFAedSearch.Droid
{
	[Activity(Label = "@string/AppName", Icon = "@drawable/icon", MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
			TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;  

			App.applicationVersion = PackageManager.GetPackageInfo(PackageName, 0).VersionName;;
			global::Xamarin.Forms.Forms.Init(this, bundle);
			global::Xamarin.FormsMaps.Init(this, bundle);
			NControl.Droid.NControlViewRenderer.Init();

			DisplayCrashReport();
			LoadApplication(new App());
		}

		private static void TaskSchedulerOnUnobservedTaskException(object sender,
			UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
		{
			var newExc = new Exception("TaskSchedulerOnUnobservedTaskException",
				unobservedTaskExceptionEventArgs.Exception);
			LogUnhandledException(newExc);
		}  

		private static void CurrentDomainOnUnhandledException(object sender,
			UnhandledExceptionEventArgs unhandledExceptionEventArgs)
		{
			var newExc = new Exception("CurrentDomainOnUnhandledException",
				unhandledExceptionEventArgs.ExceptionObject as Exception);
			LogUnhandledException(newExc);
		}  

		internal static void LogUnhandledException(Exception exception)
		{
			try
			{
				const string errorFileName = "Fatal.log";
				var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
				var errorFilePath = Path.Combine(libraryPath, errorFileName);  
				var errorMessage = String.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}",
					DateTime.Now, exception.ToString());
				File.WriteAllText(errorFilePath, errorMessage);  

				System.Diagnostics.Debug.WriteLine("Crash Report", errorMessage);
			}
			catch
			{
				// 何もできない
			}
		}

		[Conditional("DEBUG")]
		private void DisplayCrashReport()
		{
			const string errorFilename = "Fatal.log";
			var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			var errorFilePath = Path.Combine(libraryPath, errorFilename);

			if (!File.Exists(errorFilePath))
			{
				return; 
			}

			var errorText = File.ReadAllText(errorFilePath);
			new AlertDialog.Builder(this)
				.SetPositiveButton("Clear", (sender, args) =>
				{
					File.Delete(errorFilePath);
				})
				.SetNegativeButton("Close", (sender, args) =>
				{
					// User pressed Close.
				})
				.SetMessage(errorText)
				.SetTitle("Crash Report")
				.Show();
		} 
	}
}

