﻿using System;
using System.ComponentModel;

using AedOpenDataApiWrapper;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XFAedSearch.ViewModels;
using Plugin.Geolocator;
using System.Threading.Tasks;

using XFAedSearch.Models;

namespace XFAedSearch.ViewModels
{
	public class NearAedPageViewModel
	{
		private readonly ApiClient apiClient = new ApiClient();

		public ReactiveProperty<AedsViewModel> AedsViewModel{ get; private set;}

		public ReactiveProperty<bool> IsUpdating { get; private set;}

		public ReactiveProperty<Position> UserLocation { get; private set;}

		public ReactiveCommand<Map> SearchNearAedsCommand { get; private set;}

		public NearAedPageViewModel ()
		{
			AedsViewModel = new ReactiveProperty<AedsViewModel>(new AedsViewModel());
			IsUpdating = new ReactiveProperty<bool>(false);
			UserLocation = new ReactiveProperty<Position>(new Position(Settings.RegionLatitude, Settings.RegionLongitude));
			SearchNearAedsCommand = IsUpdating.Select(x => !x).ToReactiveCommand<Map>();

			SearchNearAedsCommand.Subscribe(async map =>
			{
				if(map?.VisibleRegion != null)
				{
					var position = UserLocation.Value;
					Settings.RegionLatitude = position.Latitude;
					Settings.RegionLongitude = position.Longitude;
					Settings.RegionRadius = map.VisibleRegion.Radius.Meters;
				}

				await UpdateNearAeds();
			});
		}

		public async Task UpdateNearAeds()
		{
			if(IsUpdating.Value)
				return;

			IsUpdating.Value = true;

			var stopwatch = System.Diagnostics.Stopwatch.StartNew();

			var radius = Settings.RegionRadius;
			var position = UserLocation.Value;

			try
			{
				var nearestAedTask = apiClient.NearAedAsync(position.Latitude, position.Longitude);
				var nearAedsTask = apiClient.AedSearchAsync(position.Latitude, position.Longitude,
					(int)(radius * 1.2));

				await Task.WhenAll(nearestAedTask, nearAedsTask);

				var newAeds = nearestAedTask.Result
					.Concat(nearAedsTask.Result)
					.GroupBy(aed => aed.Id)
					.Select(group => new AedViewModel(group.First()))
					.ToList();

				Device.BeginInvokeOnMainThread(() =>
				{
					AedsViewModel.Value.Aeds = newAeds;
				});
			}
			catch(Exception exception)
			{
				Console.WriteLine(exception);
				Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
				Console.WriteLine(@"Message published: key=""/SearchNearAeds/?Result=Failed""");
				Device.BeginInvokeOnMainThread(() =>
				{
					MessagingCenter.Send(this, "/SearchNearAeds/?Result=Failed");
				});
			}
			finally
			{
				IsUpdating.Value = false;
			}
		}
	}
}

