#region References

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests
{
	public static class TestHelper
	{
		#region Static Methods
		
		public static void AreEqual<T>(T expected, T actual)
		{
			var compareObjects = new CompareLogic();
			compareObjects.Config.MaxDifferences = int.MaxValue;

			var result = compareObjects.Compare(expected, actual);
			Assert.IsTrue(result.AreEqual, result.DifferencesString);
		}

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