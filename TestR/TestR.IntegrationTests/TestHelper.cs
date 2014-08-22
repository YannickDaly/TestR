#region References

using System.IO;
using System.Reflection;

#endregion

namespace TestR.IntegrationTests
{
	public static class TestHelper
	{
		#region Static Methods

		public static string GetTestFileFullPath(string relativePath)
		{
			var name = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) ?? "";
			name += "\\TestSite\\" + relativePath;
			name = name.Replace("\\", "/");
			name = name.Replace("file:/", "file:///");
			return name;
		}

		#endregion
	}
}