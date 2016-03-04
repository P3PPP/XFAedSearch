using System;
using AedOpenDataApiWrapper;

namespace XFAedSearch.ViewModels
{
	public class AedViewModel
	{
		public AedInfo AedInfo
		{
			get;
			private set;
		}

		public string LocationName
		{
			get;
			private set;
		}

		public string Detail
		{
			get;
			private set;
		}

		public AedViewModel(AedInfo aed)
		{
			if(aed == null)
				throw new ArgumentNullException("aed");

			AedInfo = aed;
			LocationName = aed.LocationName;

			string address = $"{aed.Perfecture}{aed.City}{aed.AddressArea}";
			string facility = $"{aed.FacilityName}{aed.FacilityPlace}";

			Detail =
				address + Environment.NewLine +
				facility + Environment.NewLine +
				aed.ScheduleDayType;
		}
	}}

