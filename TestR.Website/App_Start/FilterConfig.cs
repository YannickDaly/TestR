#region References

using System.Web.Mvc;

#endregion

namespace TestR.Website
{
	public static class FilterConfig
	{
		#region Static Methods

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		#endregion
	}
}