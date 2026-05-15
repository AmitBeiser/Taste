using Taste.ViewModels;
namespace Taste;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
		MusicPlayer.MediaFailed += (s, e) => 
    {
        System.Diagnostics.Debug.WriteLine($"MEDIA ERROR: {e.ErrorMessage}");
    };

    // נרשם לשינויי מצב (Playing, Buffering, Paused)
    MusicPlayer.StateChanged += (s, e) =>
    {
        System.Diagnostics.Debug.WriteLine($"MEDIA STATE: {e.NewState}");
    };

        // אנחנו משתמשים ב-AudioManager.Current כדי לשלוח ל-ViewModel את הנגן שהוא צריך
        BindingContext = new MainViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}