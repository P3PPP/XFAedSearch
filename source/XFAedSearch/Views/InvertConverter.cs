using System;
using Xamarin.Forms;

namespace XFAedSearch.Views
{
	public class InvertConverter : IValueConverter
	{
		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(targetType == typeof(bool))
			{
				return !(bool)value;
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

