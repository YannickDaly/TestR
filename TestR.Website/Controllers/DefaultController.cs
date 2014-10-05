#region References

using System.Web.Mvc;

#endregion

namespace TestR.Website.Controllers
{
	public class DefaultController : Controller
	{
		#region Methods

		public ActionResult Index()
		{
			return View();
		}

		#endregion
	}
}