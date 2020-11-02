using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io;
using CefSharp;
using CefSharp.DevTools.Log;
using CefSharp.OffScreen;
using JRogg.POD.CLI.Jobs;
using NLog;

namespace JRogg.POD.CLI
{
	class Program
	{
		private static readonly Logger Log = LogManager.GetLogger(nameof(Program));

		static void Main(string[] args)
		{
			try
			{
				Cef.EnableWaitForBrowsersToClose();
				Log.Debug("Initializing Chrome Embedded Framework.");
				Cef.Initialize(GetCefSettings());

				ProcessJobsAsync().GetAwaiter().GetResult();

				Log.Debug("Waiting for browser instances to terminate.");
				Cef.WaitForBrowsersToClose();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Application crashed.");
			}
			finally
			{
				Log.Debug("Shutting down Chrome Embedded Framework.");
				Cef.Shutdown();
			}
		}

		private static async Task ProcessJobsAsync()
		{
			while (!Cef.IsInitialized)
			{
				Log.Trace("Waiting for CEF to be initialized.");
				await Task.Delay(100);
			}

			Log.Debug("Processing jobs.");
			var remoteRequest = new RemoteQueryJob();
			await remoteRequest.ExecuteAsync();
		}

		private static CefSettings GetCefSettings()
		{
			var settings = new CefSettings();
			return settings;
		}
	}
}