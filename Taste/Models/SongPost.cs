using CommunityToolkit.Mvvm.ComponentModel;

namespace Taste.Models;

public partial class SongPost : ObservableObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public string TrackName { get; set; }
    public string ArtistName { get; set; }
    public string AlbumImageUrl { get; set; }
    public string SpotifyUrl { get; set; }
    
    // זה השדה שמשתנה כשהחבר מחזיר שיר
    [ObservableProperty]
    private bool _isLocked = true;

    // כאן אפשר להוסיף את הניקוד של ה-Trendsetter שדיברנו עליו
    public int PopularityScore { get; set; }
}