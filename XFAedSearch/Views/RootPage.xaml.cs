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

			NavigateTo(MenuItems.First(x => x.ViewModel == nearAedPageViewModel));
		}

		private void NavigateTo(MenuItem item)
		{
			if(SelectedMenuItem != item)
			{
				SelectedMenuItem = item;
				Detail = CreatePage(item);
				SetActionBarIsVisible(item.RequiresNavigationBar);
			}
			IsPresented = false;
		}

		private Page CreatePage(MenuItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");

			var inner = (Page)Activator.CreateInstance(item.PageType);
			inner.BindingContext = item.ViewModel;
			inner.Title = item.Title;
			NavigationPage.SetHasNavigationBar(inner, item.RequiresNavigationBar);
			var page = new NavigationPage(inner)
			{
				Title = item.Title,
			};
			return page;
		}

		protected override void OnAppearing()
		{
			SetActionBarIsVisible(SelectedMenuItem.RequiresNavigationBar);
			base.OnAppearing();
		}

		private void SetActionBarIsVisible(bool isVisible)
		{
			MessagingCenter.Send<RootPage, bool>(this, "ActionBarIsVisible", isVisible);
		}
	}
}

