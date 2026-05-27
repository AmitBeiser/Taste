using System.Diagnostics;

namespace Taste;

public partial class SplashPage : ContentPage
{
    private bool hasNavigated = false;

    public SplashPage()
    {
        InitializeComponent();

#if ANDROID
        string packageName = Android.App.Application.Context.PackageName;
        SplashVideoPlayer.Source = $"android.resource://{packageName}/raw/splash_video";
        Debug.WriteLine($"Android splash video source set: android.resource://{packageName}/raw/splash_video");
#else
        SplashVideoPlayer.Source = "splash_video.mp4";
        Debug.WriteLine("iOS splash video source set: splash_video.mp4");
#endif

        // Add a timeout fallback to ensure navigation happens
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(5500); // Wait 5.5 seconds then navigate if video hasn't ended
            if (!hasNavigated)
            {
                Debug.WriteLine("Timeout triggered - navigating to main");
                NavigateToMain();
            }
        });
    }

    private void OnVideoEnded(object? sender, EventArgs e)
    {
        Debug.WriteLine("Video ended event triggered");
        NavigateToMain();
    }

    private void NavigateToMain()
    {
        if (hasNavigated)
        {
            Debug.WriteLine("Already navigated, skipping");
            return;
        }

        hasNavigated = true;
        Debug.WriteLine("Starting navigation to AppShell");
        
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                Debug.WriteLine("Stopping video player");
                SplashVideoPlayer.Stop();
                await Task.Delay(100);
                Debug.WriteLine("Video stopped");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping video: {ex.Message}");
            }

            try
            {
                Debug.WriteLine("Creating AppShell instance");
                var appShell = new AppShell();
                Debug.WriteLine("AppShell instance created, setting as MainPage");
                
                // Just set the MainPage directly - don't try to disconnect handlers
                Application.Current.MainPage = appShell;
                Debug.WriteLine("MainPage set to AppShell successfully");
                
                // Give it a moment to render
                await Task.Delay(200);
                Debug.WriteLine("Navigation complete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during navigation: {ex}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        });
    }
}