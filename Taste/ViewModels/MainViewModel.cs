using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using Taste.Models;
using Taste.Services;

namespace Taste.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly SpotifyService _spotifyService;

    // מאפיין המקושר לנגן ב-XAML. שינוי שלו מפעיל את השיר מיד
    [ObservableProperty]
    private string _currentSongUrl = string.Empty;

    // מאפיין בוליאני שמגדיר האם מצב חיפוש פעיל כרגע (פותח/סוגר את השכבה ב-XAML)
    [ObservableProperty]
    private bool _isSearchMode;

    // שומר את הטקסט שהמשתמש מקליד בתיבת החיפוש
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    // רשימת הפוסטים המוצגת בפיד הראשי (ב-CarouselView)
    public ObservableCollection<SongPost> PublicStories { get; set; } = new();

    // רשימת תוצאות השירים שחזרו מהחיפוש בספוטיפיי
    public ObservableCollection<SongPost> SearchResults { get; set; } = new();

    public MainViewModel(SpotifyService spotifyService)
    {
        _spotifyService = spotifyService;
        Title = "Taste Stories";
        
        // מתחיל לטעון ברקע את החיבור לספוטיפיי
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await InitializeAsync();
        });
    }

    /// <summary>
    /// מנהל את כל שלבי האתחול של המסך: חיבור לספוטיפיי, טעינת שירים והפעלת השיר הראשון.
    /// </summary>
    private async Task InitializeAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Starting initialization...");
            
            // 1. התחברות לשרתים של ספוטיפיי
            await _spotifyService.InitializeAsync();
            System.Diagnostics.Debug.WriteLine("Spotify API initialized successfully");

            // 2. שליפת השירים והפוסטים
            await LoadStoriesFromSpotifyAsync();

            // 3. גיבוי: אם ספוטיפיי נכשלה לחלוטין, נטען נתוני דמי פנימיים
            if (PublicStories.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No Spotify stories loaded, using dummy data");
                LoadStoriesDummy();
            }

            // 4. הפעלת השיר עבור הפוסט הראשון בפיד
            if (PublicStories.Count > 0)
            {
                await InitializeAudioAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Initialization error: {ex.Message}");
            LoadStoriesDummy();
            if (PublicStories.Count > 0)
            {
                await InitializeAudioAsync();
            }
        }
    }

    /// <summary>
    /// מחפש בספוטיפיי שירים פופולריים ומייצר מהם את רשימת הפוסטים (PublicStories)
    /// </summary>
    private async Task LoadStoriesFromSpotifyAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Loading stories from Spotify...");
            
            // רשימת השירים הפופולריים שיוצגו בפיד
            var searchQueries = new[] { "Blinding Lights", "Levitating", "As It Was", "Heat Waves", "Shut Up and Dance" };
            
            foreach (var query in searchQueries)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Searching for: {query}");
                    var tracks = await _spotifyService.SearchTracksAsync(query, limit: 1);
                    
                    if (tracks.Count > 0)
                    {
                        var track = tracks[0];
                        string imageUrl = track.Album?.Images?.FirstOrDefault()?.Url ?? "https://images.unsplash.com/photo-1493225255756-d9584f8606e9?w=800";
                        
                        string previewUrl = string.IsNullOrEmpty(track.PreviewUrl) ? "NONE" : track.PreviewUrl;

                        var post = new SongPost
                        {
                            UserId = track.Id ?? "",
                            TrackName = track.Name ?? "",
                            ArtistName = string.Join(", ", track.Artists.Select(a => a.Name ?? "")),
                            AlbumImageUrl = imageUrl,
                            UserUploadedImageUrl = imageUrl,
                            PreviewUrl = previewUrl,
                            UserProfileImageUrl = "https://randomuser.me/api/portraits/men/1.jpg",
                            UserUsername = track.Artists.FirstOrDefault()?.Name ?? "Spotify User",
                            PostCaption = $"Check out {track.Name ?? ""}!",
                            PostTimeText = "now",
                            IsLocked = false,
                            SpotifyTrackId = track.Id ?? "",
                            SpotifyArtistIds = string.Join(",", track.Artists.Select(a => a.Id ?? ""))
                        };

                        PublicStories.Add(post);
                        System.Diagnostics.Debug.WriteLine($"Added song to feed: {track.Name}");
                    }
                }
                catch (Exception queryEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error searching for '{query}': {queryEx.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Successfully loaded {PublicStories.Count} stories from Spotify");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading Spotify stories: {ex.Message}");
        }
    }

    /// <summary>
    /// מפעיל את המוזיקה של הפוסט הראשון כשהאפליקציה נפתחת
    /// </summary>
    private async Task InitializeAudioAsync()
    {
        await Task.Delay(2500);
        
        if (PublicStories.Count > 0 && string.IsNullOrEmpty(CurrentSongUrl))
        {
            await PlaySongAsync(PublicStories[0]);
        }
    }

    /// <summary>
    /// ה-Command שמקושר לנגן. מופעל בגלילה בפיד, או בלחיצה על כפתור ה-Play בחיפוש.
    /// </summary>
    [RelayCommand]
    private async Task PlaySongAsync(SongPost story)
    {
        if (story == null) return;

        // 1. במידה ולספוטיפיי יש קטע שמע תקין, נשתמש בו מיד
        if (!string.IsNullOrEmpty(story.PreviewUrl) && story.PreviewUrl != "NONE")
        {
            CurrentSongUrl = story.PreviewUrl;
            System.Diagnostics.Debug.WriteLine($"Playing Spotify preview: {CurrentSongUrl}");
            return;
        }

        // 2. במידה וחסר ("NONE"), נפנה שקופית ל-API החופשי של Apple Music
        System.Diagnostics.Debug.WriteLine($"Spotify preview missing for '{story.TrackName}'. Fetching from Apple Music...");
        
        string backupUrl = await GetBackupPreviewUrlAsync(story.TrackName, story.ArtistName);
        
        if (!string.IsNullOrEmpty(backupUrl))
        {
            CurrentSongUrl = backupUrl;
            System.Diagnostics.Debug.WriteLine($"Playing Apple Music backup preview: {CurrentSongUrl}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("[Taste] Fallback failed. No audio available for this track.");
        }
    }

    /// <summary>
    /// פותח או סוגר את מסך החיפוש, ומאפס נתונים בעת סגירה
    /// </summary>
    [RelayCommand]
    private void ToggleSearch()
    {
        IsSearchMode = !IsSearchMode;
        if (!IsSearchMode)
        {
            SearchResults.Clear();
            SearchQuery = string.Empty;
        }
    }

    /// <summary>
    /// פונה לספוטיפיי ומחפש שירים לפי מה שהמשתמש הקליד
    /// </summary>
    [RelayCommand]
    private async Task PerformSearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) return;

        try
        {
            System.Diagnostics.Debug.WriteLine($"Searching Spotify for: {SearchQuery}");
            var tracks = await _spotifyService.SearchTracksAsync(SearchQuery, limit: 15);
            
            SearchResults.Clear();
            foreach (var track in tracks)
            {
                string imageUrl = track.Album?.Images?.FirstOrDefault()?.Url ?? "https://images.unsplash.com/photo-1493225255756-d9584f8606e9?w=800";
                
                SearchResults.Add(new SongPost
                {
                    UserId = track.Id ?? "",
                    TrackName = track.Name ?? "",
                    ArtistName = string.Join(", ", track.Artists.Select(a => a.Name ?? "")),
                    AlbumImageUrl = imageUrl,
                    UserUploadedImageUrl = imageUrl, // זמני לתצוגה מקדימה
                    PreviewUrl = string.IsNullOrEmpty(track.PreviewUrl) ? "NONE" : track.PreviewUrl,
                    UserProfileImageUrl = "https://randomuser.me/api/portraits/men/1.jpg",
                    UserUsername = track.Artists.FirstOrDefault()?.Name ?? "Spotify User",
                    PostCaption = "",
                    PostTimeText = "now",
                    IsLocked = false,
                    SpotifyTrackId = track.Id ?? "",
                    SpotifyArtistIds = string.Join(",", track.Artists.Select(a => a.Id ?? ""))
                });
            }
            System.Diagnostics.Debug.WriteLine($"Found {SearchResults.Count} results for search.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
        }
    }

    /// <summary>
    /// פקודה שמופעלת בלחיצה על כפתור ה-➕ ברשימת החיפוש ומעבירה אותנו לשלב הבא
    /// </summary>
    [RelayCommand]
    private async Task SelectTrackAsync(SongPost selectedPost)
    {
        if (selectedPost == null) return;

        System.Diagnostics.Debug.WriteLine($"Selected track for posting: {selectedPost.TrackName}");
        
        // סגירת מצב החיפוש והחזרת המשתמש למסך הראשי
        IsSearchMode = false;
        
        // קופץ פופ-אפ זמני, כאן בהמשך נחבר את המצלמה/גלריה
        await Shell.Current.DisplayAlert("השלב הבא", $"בחרת את השיר '{selectedPost.TrackName}', עכשיו נצלם או נבחר תמונה לפוסט!", "מעולה");
    }

    /// <summary>
    /// פונקציית עזר פנימית שפונה ל-iTunes API כדי להשיג את קטע ה-30 שניות החסר
    /// </summary>
    private async Task<string?> GetBackupPreviewUrlAsync(string trackName, string artistName)
    {
        try
        {
            using var client = new HttpClient();
            string searchQuery = $"{trackName} {artistName}";
            string encodedQuery = HttpUtility.UrlEncode(searchQuery);
            
            string url = $"https://itunes.apple.com/search?term={encodedQuery}&media=music&limit=1";
            
            var response = await client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
            
            if (root.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
            {
                var firstResult = results[0];
                if (firstResult.TryGetProperty("previewUrl", out var previewUrlProperty))
                {
                    return previewUrlProperty.GetString();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Apple Music Backup Error]: {ex.Message}");
        }
        return null;
    }

    [RelayCommand]
    private void LikeStory(SongPost story)
    {
        // לוגיקה עתידית לעליית לייקים
    }

    /// <summary>
    /// רשימת גיבוי עם נתונים פיקטיביים למקרה שאין אינטרנט או שהחיבור לספוטיפיי נכשל
    /// </summary>
    private void LoadStoriesDummy()
    {
        PublicStories.Add(new SongPost 
        { 
            UserId = "1",
            TrackName = "Classic Rock", 
            ArtistName = "Sample Artist",
            AlbumImageUrl = "https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=200", 
            UserUploadedImageUrl = "https://images.unsplash.com/photo-1614613535308-eb5fbd3d2c17?w=800",
            PreviewUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3",
            UserProfileImageUrl = "https://randomuser.me/api/portraits/men/32.jpg", 
            UserUsername = "ClassicLover", 
            PostCaption = "Feelin' the nostalgic vibes with this classic frame setup.", 
            PostTimeText = "1d", 
            IsLocked = false 
        });

        PublicStories.Add(new SongPost 
        { 
            UserId = "2",
            TrackName = "Electronic Beat", 
            ArtistName = "Synth Master",
            AlbumImageUrl = "https://images.unsplash.com/photo-1470225620780-dba8ba36b745?w=200",
            UserUploadedImageUrl = "https://images.unsplash.com/photo-1493225255756-d9584f8606e9?w=800",
            PreviewUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-2.mp3",
            UserProfileImageUrl = "https://randomuser.me/api/portraits/women/44.jpg",
            UserUsername = "BeatMkr",
            PostCaption = "Synth city mornings.",
            PostTimeText = "3h",
            IsLocked = false 
        });
    }
}