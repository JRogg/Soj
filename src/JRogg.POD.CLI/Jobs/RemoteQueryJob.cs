using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using CefSharp;
using CefSharp.OffScreen;
using JRogg.POD.CLI.Extensions;
using NLog;

namespace JRogg.POD.CLI.Jobs
{
	public class RemoteQueryJob
	{
		private static readonly Logger Log = LogManager.GetLogger(nameof(RemoteQueryJob));

		private static async Task<string> GetPageContentAsync(ChromiumWebBrowser browser, string rootUrl)
		{
			while (!browser.IsBrowserInitialized)
			{
				Log.Trace("Waiting for browser to be initialized.");
				await Task.Delay(100);
			}

			Log.Debug("Browser is ready. Starting request to {Url}.", rootUrl);

			await browser.LoadAsync(rootUrl);

			Log.Debug("Waiting {Delay} for browser content to update (API Load delay)", "2000ms");
			await Task.Delay(2000);
			var content = await browser.GetSourceAsync();

			return content;
		}

		private static void DisplayResults(string html)
		{
			var marker = ".search-result-row > .has-text-white.is-pulled-right.has-text-right";
			var document = GetDocument(html);
			var matches = document.QuerySelectorAll(marker)
				.OfType<IHtmlDivElement>()
				.ToArray();
			var contents = matches.Select(d => d.TextContent);
			var names = contents.Select(d => d.Split("|")[0]);

			Log.Info("Found {X} results.", matches.Length);
			Log.Info($"All names: {Environment.NewLine}, {{Names}}", string.Join(Environment.NewLine, names));
		}

		private static IHtmlDocument GetDocument(string html)
		{
			Log.Debug("Parsing document from {X} characters.", html.Length);
			var parser = new HtmlParser();
			var document = parser.ParseDocument(html);
			return document;
		}

		public async Task ExecuteAsync()
		{
			var rootUrl = "https://beta.pathofdiablo.com/trade-search?selectedItem=Aldur%27s+Advance+Battle+Boots";
			using (var browser = new ChromiumWebBrowser())
			{
				var content = await GetPageContentAsync(browser, rootUrl);

				DisplayResults(content);
			}
		}
	}
}