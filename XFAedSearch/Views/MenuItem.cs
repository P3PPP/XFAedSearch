using System;
using Xamarin;
using Xamarin.Forms;
using XFAedSearch;
using XFAedSearch.Views;
using XFAedSearch.ViewModels;

namespace XFAedSearch.Views
{
	public class MenuItem
	{
		public Type PageType {
			get;
			set;
		}

		public object ViewModel {
			get;
			set;
		}

		public string Title {
			get;
			set;
		}
	}
}

