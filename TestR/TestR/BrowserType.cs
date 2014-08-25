﻿namespace TestR
{
	/// <summary>
	/// The type of the browser.
	/// </summary>
	public enum BrowserType
	{
		/// <summary>
		/// Represents a Chrome browser.
		/// </summary>
		Chrome = 0x01,

		/// <summary>
		/// Represents a Internet Explorer browser.
		/// </summary>
		InternetExplorer = 0x02,

		/// <summary>
		/// Represents all browser types.
		/// </summary>
		All = 0xFF,
	}
}