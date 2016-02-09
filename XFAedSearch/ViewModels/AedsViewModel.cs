using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using AedOpenDataApiWrapper;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace XFAedSearch.ViewModels
{
	public class AedsViewModel : INotifyPropertyChanged
	{
//		private string selectedAedName = "";
//		public string SelectedAedName {
//			get { return selectedAedName; }
//			set
//			{
//				selectedAedName = value;
//				RaisePropertyChanged(nameof(SelectedAedName));
//			}
//		}

		private AedViewModel selectedAed;
		public AedViewModel SelectedAed {
			get { return selectedAed; }
			set
			{
				if(selectedAed == value)
					return;

				selectedAed = value;
				RaisePropertyChanged(nameof(SelectedAed));
//				SelectedAedName = $"{selectedAed?.LocationName} {selectedAed?.FacilityName} {selectedAed?.FacilityPlace}";

				RaiseFocusTo(selectedAed);
			}
		}

		private List<AedViewModel> aeds;
		public List<AedViewModel> Aeds {
			get { return aeds; }
			set
			{
				aeds = value;
				RaisePropertyChanged(nameof(Aeds));

				if(aeds != null)
				{
					Console.WriteLine(@"Message published: key=""/Map/Pins/Update/NearAeds""");
					MessagingCenter.Send(this, "/Map/Pins/Update/NearAeds", aeds);
				}

				SelectedAed = aeds?.FirstOrDefault();
			}
		}

		public ICommand NextCommand
		{
			get;
			private set;
		}

		public ICommand PreviousCommand
		{
			get;
			private set;
		}

		public ICommand RaiseFocusToSelectedAedCommand
		{
			get;
			private set;
		}

		public AedsViewModel()
		{
			NextCommand = new Command(Next);
			PreviousCommand = new Command(Previous);
			RaiseFocusToSelectedAedCommand = new Command(() => RaiseFocusTo(selectedAed));
		}

		private void Next()
		{
			if(aeds == null || aeds.Count == 0)
				return;

			var index = Aeds.IndexOf(SelectedAed);

			if(index == -1)
				return;

			SelectedAed = index == Aeds.Count - 1
				? Aeds.First()
				: Aeds[index + 1];

		}

		private void Previous()
		{
			if(aeds == null || aeds.Count == 0)
				return;

			var index = Aeds.IndexOf(SelectedAed);

			if(index == -1)
				return;

			SelectedAed = index == 0
				? Aeds.Last()
				: Aeds[index - 1];
		}

		private void RaiseFocusTo(AedViewModel aed)
		{
			if(aed == null)
				return;
			
			Console.WriteLine(@"Message published: key=""/Map/FocusTo"" Position("+ aed.AedInfo.Latitude +", " + aed.AedInfo.Longitude + ")");
			Device.BeginInvokeOnMainThread(() =>
			{
				MessagingCenter.Send(this, "/Map/FocusTo",
					new Position(aed.AedInfo.Latitude, aed.AedInfo.Longitude));
			});
		}

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged = (_, __) => { };

		private void RaisePropertyChanged (string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}

