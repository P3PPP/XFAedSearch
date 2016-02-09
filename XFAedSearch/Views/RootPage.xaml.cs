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
				},
				new MenuItem {
					Title = "プライバシーポリシー1",
					PageType = typeof(PrivacyPlicyPage),
					ViewModel = new { Name = "Policy1" },
				},
				new MenuItem {
					Title = "プライバシーポリシー2",
					PageType = typeof(PrivacyPlicyPage),
					ViewModel = new { Name = "Policy2" },
				},
			};

			listView.ItemTapped += (sender, e) => NavigateTo(e.Item as MenuItem);
			listView.ItemsSource = MenuItems;

			SelectedMenuItem = MenuItems.First(x => x.ViewModel == nearAedPageViewModel);
			var nearAedPage = new NearAedPage {
				BindingContext = nearAedPageViewModel
			};
			Detail = nearAedPage;


			nearAedPageViewModel.UpdateNearAeds()
				.ContinueWith(x =>
				{
					if(x.Status != TaskStatus.Faulted)
					{
						var aed = nearAedPageViewModel.AedsViewModel.Value.Aeds.First();
						nearAedPage.MoveToReagion(MapSpan.FromCenterAndRadius(
							new Position(aed.AedInfo.Latitude, aed.AedInfo.Longitude),
							Distance.FromMeters(Settings.RegionRadius)));
					}
				});
		}

		private void NavigateTo(MenuItem item)
		{
			if(SelectedMenuItem != item)
			{
				SelectedMenuItem = item;
				Page page = (Page)Activator.CreateInstance(item.PageType);
				page.BindingContext = item.ViewModel;
				Detail = page;
			}
			IsPresented = false;
		}
	}


	public class PrivacyPlicyPage : ContentPage
	{
		public PrivacyPlicyPage()
		{
			var label = new Label {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Text = "ここに位置情報のプライバシーポリシーとかを書く"
			};
			label.SetBinding(Label.TextProperty, "Name");

			Content = label;
		}
	}
}

