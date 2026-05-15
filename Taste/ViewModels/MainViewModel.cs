using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Taste.Models;

namespace Taste.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _currentSongUrl;

    public ObservableCollection<SongPost> PublicStories { get; set; } = new();

    public MainViewModel()
    {
        Title = "Taste Stories";
        LoadStories();
        
        // במקום להטעין מיד, אנחנו מפעילים תהליך השהיה
        _ = InitializeAudioAsync();
    }

    private async Task InitializeAudioAsync()
    {
        // השהיה של 2.5 שניות כדי לאפשר לאימולטור לסיים לטעון את הגרפיקה
        await Task.Delay(2500);
        
        if (PublicStories.Count > 0 && string.IsNullOrEmpty(CurrentSongUrl))
        {
            CurrentSongUrl = PublicStories[0].PreviewUrl;
            System.Diagnostics.Debug.WriteLine($"Audio initialized with: {CurrentSongUrl}");
        }
    }

    [RelayCommand]
    private void PlaySong(SongPost story)
    {
        if (story == null || string.IsNullOrEmpty(story.PreviewUrl)) return;

        // עדכון ה-URL יגרום ל-MediaElement ב-XAML להחליף שיר
        CurrentSongUrl = story.PreviewUrl;
        System.Diagnostics.Debug.WriteLine($"Switching to: {CurrentSongUrl}");
    }

    [RelayCommand]
    private void LikeStory(SongPost story)
    {
        // לוגיקה עתידית
    }

    private void LoadStories()
    {
        PublicStories.Add(new SongPost 
        { 
            TrackName = "Classic Rock", 
            ArtistName = "Sample Artist",
            AlbumImageUrl = "https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=200", 
            UserUploadedImageUrl = "https://images.unsplash.com/photo-1614613535308-eb5fbd3d2c17?w=800",
            PreviewUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3",
            IsLocked = false 
        });

        PublicStories.Add(new SongPost 
        { 
            TrackName = "Electronic Beat", 
            ArtistName = "Synth Master",
            AlbumImageUrl = "https://images.unsplash.com/photo-1470225620780-dba8ba36b745?w=200",
            UserUploadedImageUrl = "https://images.unsplash.com/photo-1493225255756-d9584f8606e9?w=800",
            PreviewUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-2.mp3",
            IsLocked = false 
        });
    }
}