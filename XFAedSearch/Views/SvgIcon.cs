using System;
using Xamarin.Forms;
using NGraphics;
using System.Reflection;
using System.IO;

namespace XFAedSearch.Views
{
	public static class SvgIcon
	{
		private static readonly string directory = "XFAedSearch" +
			Device.OnPlatform<string>(".iOS", ".Droid", "Phone") + 
			".Misc";
		private static readonly Assembly assembly = typeof(SvgIcon).GetTypeInfo().Assembly;

		public static NGraphics.Graphic Load(string fileName)
		{
			Graphic graphic = null;
			var path = directory + "." + fileName;
			using(var reader = new StreamReader(assembly.GetManifestResourceStream(path)))
			{
				graphic = Graphic.LoadSvg(reader);
			}
			return graphic;
		}
	}
}

