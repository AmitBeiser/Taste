using Taste.Models;
using Taste.ViewModels;
using Taste.Services;

namespace Taste;

public partial class MainPage : ContentPage
{
    private SongPost? _lastPlayedSong;

    public MainPage()
    {
        InitializeComponent();
        MusicPlayer.MediaFailed += (s, e) => 
        {
            System.Diagnostics.Debug.WriteLine($"MEDIA ERROR: {e.ErrorMessage}");
        };

        MusicPlayer.StateChanged += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine($"MEDIA STATE: {e.NewState}");
        };
        
        // Get SpotifyService from dependency injection
        var spotifyService = IPlatformApplication.Current?.Services.GetService<SpotifyService>();
        BindingContext = new MainViewModel(spotifyService ?? throw new InvalidOperationException("SpotifyService not registered"));
    }

    private void OnCarouselCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        if (e.CurrentItem is SongPost currentSong)
        {
            PlaySongIfDifferent(currentSong);
            System.Diagnostics.Debug.WriteLine($"Carousel changed to: {currentSong?.TrackName}");
        }
    }

    private void PlaySongIfDifferent(SongPost? song)
    {
        if (song == null) return;

        if (_lastPlayedSong?.Id != song.Id && BindingContext is MainViewModel viewModel)
        {
            if (viewModel.PlaySongCommand != null && viewModel.PlaySongCommand.CanExecute(song))
            {
                viewModel.PlaySongCommand.Execute(song);
                _lastPlayedSong = song;
                System.Diagnostics.Debug.WriteLine($"Now playing: {song.TrackName}");
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}