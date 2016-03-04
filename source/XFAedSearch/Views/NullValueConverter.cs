using System;
using Xamarin.Forms;

namespace XFAedSearch.Views
{
	public class NullValueConverter : IValueConverter
	{
		public Object SubstituteValue
		{
			get;
			set;
		}

		#region IValueConverter implementation
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value ?? SubstituteValue;

		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		#endregion

		public NullValueConverter()
		{
			SubstituteValue = "shokichi";
		}
	}
}

