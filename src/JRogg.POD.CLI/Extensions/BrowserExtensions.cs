using System;
using System.Threading.Tasks;

namespace JRogg.POD.CLI.Extensions
{
	public static class BrowserExtensions
	{
		private static readonly NLog.Logger Log = NLog.LogManager.GetLogger(nameof(BrowserExtensions));

		public static async Task LoadAsync(this ChromiumWebBrowser source, string url)
		{
			var tcs = new TaskCompletionSource<object>();
			EventHandler<LoadingStateChangedEventArgs> sourceOnLoadingStateChanged = null;
			sourceOnLoadingStateChanged = (sender, args) =>
			{
				if (!args.IsLoading)
				{
					tcs.TrySetResult(Task.CompletedTask);
					source.LoadingStateChanged -= sourceOnLoadingStateChanged;
				}
			};
			source.LoadingStateChanged += sourceOnLoadingStateChanged;

			Log.Debug("Loading {Url}", url);

			source.Load(url);
			await tcs.Task;
		}
	}
}