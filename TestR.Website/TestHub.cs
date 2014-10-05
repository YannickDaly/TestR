#region References

using Microsoft.AspNet.SignalR;

#endregion

namespace TestR.Website
{
	public class TestHub : Hub
	{
		#region Methods

		public void Hello()
		{
			Clients.All.hello();
		}

		#endregion
	}
}