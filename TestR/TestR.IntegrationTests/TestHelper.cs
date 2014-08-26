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

		public static void AllExists<T>(IList<T> expected, IList<T> actual)
		{
			var builder = new StringBuilder();
			foreach (var item in expected.Except(actual))
			{
				builder.AppendLine("Missing [" + item + "] in actual collection.");
			}

			if (builder.Length > 0)
			{
				Assert.Fail(builder.ToString());
			}
		}

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