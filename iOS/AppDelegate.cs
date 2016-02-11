using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace XFAedSearch.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
			TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

			DisplayCrashReport();

			global::Xamarin.Forms.Forms.Init();
			global::Xamarin.FormsMaps.Init();
			NControl.iOS.NControlViewRenderer.Init();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
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
				var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
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
		private static void DisplayCrashReport()
		{
			const string errorFilename = "Fatal.log";
			var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
			var errorFilePath = Path.Combine(libraryPath, errorFilename);

			if (!File.Exists(errorFilePath))
			{
				return;
			}

			var errorText = File.ReadAllText(errorFilePath);
			var alertView = new UIAlertView("Crash Report", errorText, null, "Close", "Clear") { UserInteractionEnabled = true };
			alertView.Clicked += (sender, args) =>
			{
				if (args.ButtonIndex != 0)
				{
					File.Delete(errorFilePath);
				}
			};
			alertView.Show();
		}
	}
}

