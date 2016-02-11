using System;
using Xamarin.Forms;
using XFAedSearch.Views;
using System.Reflection;

namespace XFAedSearch
{
	[Xamarin.Forms.Xaml.XamlCompilation (Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
	public partial class App : Application
	{
		private static Version assemblyVersion = typeof(App).Assembly.GetName().Version;
		public static string Version
		{
			get { return assemblyVersion.ToString();}
		}

		public App()
		{
			InitializeComponent();
			MainPage = new RootPage();
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
	}
}

