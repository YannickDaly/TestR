#region References

using System.Web.Mvc;
using System.Web.Routing;

#endregion

namespace TestR.Website
{
	public static class RouteConfig
	{
		#region Static Methods

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute("Default", "{controller}/{action}/{id}", new { Controller = "Default", action = "Index", id = UrlParameter.Optional });
		}

		#endregion
	}
}