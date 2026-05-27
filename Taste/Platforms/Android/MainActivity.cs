using Android.App;
using Android.Content.PM;

namespace Taste;

[Activity(
    Name = "com.companyname.taste.MainActivity",
    Theme = "@style/Maui.SplashTheme", 
    MainLauncher = true, 
    LaunchMode = LaunchMode.SingleTop, 
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
// ה-IntentFilter שהיה כאן הוסר כדי למנוע את ההתנגשות
public class MainActivity : MauiAppCompatActivity
{
}