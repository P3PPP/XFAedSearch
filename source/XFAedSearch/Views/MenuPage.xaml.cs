using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace XFAedSearch.Views
{
	[Xamarin.Forms.Xaml.XamlCompilation (Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
		public List<MenuItem> MenuItems
		{
			get;
			private set;
		}

		public MenuItem SelectedItem
		{
			get;
			private set;
		}

		public MenuPage()
		{
			Title = "メニュー";
			InitializeComponent();
			MenuItems = new List<MenuItem> {
				new MenuItem {
					Title = "アプリについて",
					PageType = typeof(AboutPage),
					RequiresNavigationBar = true,
				},
			};

			listView.ItemTapped += (sender, e) => NavigateTo(e.Item as MenuItem);
			listView.ItemsSource = MenuItems;
		}

		private void NavigateTo(MenuItem item)
		{
			SelectedItem = item;
			var page = (Page)Activator.CreateInstance(item.PageType);
			page.Title = item.Title;
			NavigationPage.SetHasNavigationBar(page, item.RequiresNavigationBar);
			Navigation.InsertPageBefore(page, this);
			Navigation.PopAsync();
		}
	}
}

