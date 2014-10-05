#region References

using Owin;

#endregion

namespace TestR.Website
{
	public class Startup
	{
		#region Methods

		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();
		}

		#endregion
	}
}