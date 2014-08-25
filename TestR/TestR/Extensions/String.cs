#region References

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

#endregion

namespace TestR.Extensions
{
	/// <summary>
	/// Represents a static helper class.
	/// </summary>
	public static partial class Helper
	{
		#region Static Methods

		/// <summary>
		/// Deserialize JSON data into a JToken class.
		/// </summary>
		/// <param name="data">The JSON data to deserialize.</param>
		/// <returns>The JToken class of the data.</returns>
		public static JToken AsJToken(this string data)
		{
			var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
			return (JToken) JsonConvert.DeserializeObject(data, jsonSerializerSettings);
		}

		#endregion
	}
}