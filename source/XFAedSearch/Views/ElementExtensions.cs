using System;
using Xamarin.Forms;

namespace XFAedSearch.Views
{
	public static class ElementExtensions
	{
		public static T FindAncestor<T> (this Element element) where T : Element
		{
			if(element == null)
				throw new ArgumentNullException("element");

			Element ancestor = element.Parent;
			while(!(ancestor is T) && ancestor != null)
			{
				ancestor = ancestor.Parent;
			}

			return (T)ancestor;
		}
	}
}

