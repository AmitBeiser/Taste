using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Taste.Services;

namespace Taste;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkitMediaElement(false)
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		var spotifyClientId = "6c412689f85841deb7a94109dc3b7d7e";
		var spotifyClientSecret = "f98686bec80d4d6eaede77ea8c3377d3";
		
		builder.Services.AddSingleton(new SpotifyService(spotifyClientId, spotifyClientSecret));

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
