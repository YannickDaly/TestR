#region References

using System.Web.Optimization;

#endregion

namespace TestR.Website
{
	public static class BundleConfig
	{
		#region Static Methods

		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/Scripts/js").Include(
				"~/Scripts/jquery-{version}.js",
				"~/Scripts/jquery.signalR-{version}.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/site.css"));
		}

		#endregion
	}
}