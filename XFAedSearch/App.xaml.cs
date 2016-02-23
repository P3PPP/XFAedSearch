using System;
using Xamarin.Forms;
using XFAedSearch.Views;
using XFAedSearch.ViewModels;

// 共通コードをPCLにして他プラットーフォームにinternalメンバを公開したい時に使う
//[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("XFAedSearch.Droid")]
//[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("XFAedSearch.iOS")]

namespace XFAedSearch
{
	[Xamarin.Forms.Xaml.XamlCompilation (Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
	public partial class App : Application
	{
		internal static string applicationVersion = "";
		public static string Version
		{
			get { return applicationVersion;}
		}

		public App()
		{
			InitializeComponent();

			var nearAedPageViewModel = new NearAedPageViewModel();
			var nearAedPage = new NearAedPage { BindingContext = nearAedPageViewModel };
			NavigationPage.SetHasNavigationBar(nearAedPage, false);
			MainPage = new NavigationPage(nearAedPage);
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

