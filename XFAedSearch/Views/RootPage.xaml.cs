using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

using XFAedSearch.Models;
using XFAedSearch.ViewModels;

namespace XFAedSearch.Views
{
	public partial class RootPage : MasterDetailPage
	{
		public List<MenuItem> MenuItems
		{
			get;
			private set;
		}

		private MenuItem SelectedMenuItem;

		public RootPage()
		{
			InitializeComponent();

			var nearAedPageViewModel = new NearAedPageViewModel();
			nearAedPageViewModel.UserLocation.Value = new Position(Settings.RegionLatitude, Settings.RegionLongitude);

			MenuItems = new List<MenuItem> {
				new MenuItem {
					Title = "地図で探す",
					PageType = typeof(NearAedPage),
					ViewModel = nearAedPageViewModel,
					RequiresNavigationBar = false,
				},
				new MenuItem {
					Title = "アプリについて",
					PageType = typeof(AboutPage),
					RequiresNavigationBar = true,
				},
			};

			listView.ItemTapped += (sender, e) => NavigateTo(e.Item as MenuItem);
			listView.ItemsSource = MenuItems;

			SelectedMenuItem = MenuItems.First(x => x.ViewModel == nearAedPageViewModel);
			var nearAedPage = new NearAedPage {
				BindingContext = nearAedPageViewModel
			};
			Detail = nearAedPage;
		}

		private void NavigateTo(MenuItem item)
		{
			if(SelectedMenuItem != item)
			{
				SelectedMenuItem = item;
				Page page = (Page)Activator.CreateInstance(item.PageType);
				page.BindingContext = item.ViewModel;
				Detail = item.RequiresNavigationBar
					? new NavigationPage(page) { Title = item.Title, }
					: page;
			}
			IsPresented = false;
		}
	}
}

