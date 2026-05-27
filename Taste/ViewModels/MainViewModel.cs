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


    /// מנהל את כל שלבי האתחול של המסך: חיבור לספוטיפיי, טעינת שירים והפעלת השיר הראשון.
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

    /// מחפש בספוטיפיי שירים פופולריים ומייצר מהם את רשימת הפוסטים (PublicStories)
    private async Task LoadStoriesFromSpotifyAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Loading stories from Spotify...");
            
            // רשימה גדולה של שירים שונים
            var allSearchQueries = new[] 
            { 
                "Blinding Lights", "Levitating", "As It Was", "Heat Waves", "Shut Up and Dance",
                "Bohemian Rhapsody", "Thriller", "Imagine", "Hotel California", "Stairway to Heaven",
                "Yesterday", "Like a Virgin", "Smells Like Teen Spirit", "Wonderwall", "Shape of You",
                "One Dance", "Sorry", "Look What You Made Me Do", "Anti-Hero", "Flowers",
                "Good as Hell", "Levitate", "Peaches", "STAY", "Blinding Lights (Remix)",
                "Toxic", "Umbrella", "Rolling in the Deep", "Someone Like You", "We Are Never Ever",
                "All Too Well", "Kill Bill", "Cruel Summer", "Midnights", "Fortnight"
            };
            
            // שנשתמש בשאילתות אקראיות כל פעם שהאפליקציה נפתחת
            var random = new Random();
            var randomQueries = allSearchQueries.OrderBy(_ => random.Next()).Take(5).ToArray();
            
            foreach (var query in randomQueries)
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


    /// מפעיל את המוזיקה של הפוסט הראשון כשהאפליקציה נפתחת
    private async Task InitializeAudioAsync()
    {
        await Task.Delay(2500);
        
        if (PublicStories.Count > 0 && string.IsNullOrEmpty(CurrentSongUrl))
        {
            await PlaySongAsync(PublicStories[0]);
        }
    }


    /// ה-Command שמקושר לנגן. מופעל בגלילה בפיד, או בלחיצה על כפתור ה-Play בחיפוש.
    [RelayCommand]
    private async Task PlaySongAsync(SongPost story)
    {
        if (story == null) return;

        // עצור את כל השירים האחרים בפיד
        foreach (var post in PublicStories)
        {
            if (post != story)
                post.IsPlaying = false;
        }
        foreach (var post in SearchResults)
        {
            if (post != story)
                post.IsPlaying = false;
        }

        // 1. במידה ולספוטיפיי יש קטע שמע תקין, נשתמש בו מיד
        if (!string.IsNullOrEmpty(story.PreviewUrl) && story.PreviewUrl != "NONE")
        {
            CurrentSongUrl = story.PreviewUrl;
            story.IsPlaying = true; // סמן שהשיר מתנגן
            System.Diagnostics.Debug.WriteLine($"Playing Spotify preview: {CurrentSongUrl}");
            return;
        }

        // 2. במידה וחסר ("NONE"), נפנה שקופית ל-API החופשי של Apple Music
        System.Diagnostics.Debug.WriteLine($"Spotify preview missing for '{story.TrackName}'. Fetching from Apple Music...");
        
        string? backupUrl = await GetBackupPreviewUrlAsync(story.TrackName, story.ArtistName);
        
        if (!string.IsNullOrEmpty(backupUrl))
        {
            CurrentSongUrl = backupUrl;
            story.IsPlaying = true; // סמן שהשיר מתנגן
            System.Diagnostics.Debug.WriteLine($"Playing Apple Music backup preview: {CurrentSongUrl}");
        }
        else
        {
            story.IsPlaying = false;
            System.Diagnostics.Debug.WriteLine("[Taste] Fallback failed. No audio available for this track.");
        }
    }

    [RelayCommand]
    private async Task OpenInSpotifyAsync(SongPost story)
    {
        if (story == null) return;
        // ה-URI שפותח את השיר הספציפי בתוך אפליקציית ספוטיפיי
        string spotifyUri = $"spotify:track:{story.SpotifyTrackId}";

        if (await Launcher.Default.CanOpenAsync(spotifyUri))
        {
            await Launcher.Default.OpenAsync(spotifyUri);
        }
        else
        {
            // אם האפליקציה לא מותקנת, נפתח את השיר בדפדפן
            await Browser.Default.OpenAsync($"https://open.spotify.com/track/{story.SpotifyTrackId}");
        }
    }

    [RelayCommand]
    private async Task LikeOrAddTrackAsync(SongPost story)
    {
        if (story == null) return;

        // כאן אתה יכול להוסיף לוגיקה של UI (כמו שינוי צבע הלב)
        await Shell.Current.DisplayAlertAsync("ספוטיפיי", $"השיר {story.TrackName} הועבר לטיפול בספוטיפיי!", "אישור");

        // פתיחת ספוטיפיי לביצוע הפעולה (כרגע פותח את השיר, 
        // בהמשך עם API תוכל לבצע Like ישירות)
        await OpenInSpotifyAsync(story);
    }


/// פותח או סוגר את מסך החיפוש, ומאפס נתונים בעת סגירה
    [RelayCommand]
    private async Task ToggleSearchAsync()
    {
        IsSearchMode = !IsSearchMode;
        
        if (!IsSearchMode)
        {
            // המשתמש סגר את החיפוש וחזר למסך הבית - מאפסים נתונים
            SearchResults.Clear();
            SearchQuery = string.Empty;

            // מחזירים את הנגן לנגן את השיר הראשון בפיד הראשי
            if (PublicStories.Count > 0)
            {
                await PlaySongAsync(PublicStories[0]);
            }
            else
            {
                CurrentSongUrl = string.Empty; // אם אין פוסטים, פשוט משתיקים
            }
        }
        else
        {
            // המשתמש פתח את מסך החיפוש - עצור את המוזיקה
            CurrentSongUrl = string.Empty;
        }
    }
// משתנה שיודע לבטל משימות רקע (כמו חיפוש קודם שעוד לא הסתיים)
private CancellationTokenSource? _searchCts;



/// מתודה פנימית של המערכת שמופעלת אוטומטית בכל פעם שהטקסט בתיבת החיפוש משתנה
partial void OnSearchQueryChanged(string value)
{
    // 1. אם יש חיפוש קודם שעדיין רץ, נבטל אותו מיד כי המשתמש הקליד אות חדשה
    _searchCts?.Cancel();
    _searchCts = new CancellationTokenSource();
    // 2. הפעלת הלוגיקה של החיפוש בזמן אמת עם הטוקן לביטול
    _ = LiveSearchAsync(value, _searchCts.Token);
}


/// לוגיקת החיפוש החכמה בזמן אמת
private async Task LiveSearchAsync(string query, CancellationToken token)
{
    if (string.IsNullOrWhiteSpace(query))
    {
        MainThread.BeginInvokeOnMainThread(() => SearchResults.Clear());
        return;
    }

    try
    {
        // קודם כל עוברים לרוץ ברקע ומחכים חצי שנייה (כדי לא לתקוע את ה-UI בזמן הקלדה)
        await Task.Delay(500, token).ConfigureAwait(false);

        System.Diagnostics.Debug.WriteLine($"Live searching Spotify for: {query}");
        
        // שינוי ל-limit קטן יותר (5) וביצוע הקריאה לחלוטין ברקע
        var tracks = await _spotifyService.SearchTracksAsync(query, limit: 5).ConfigureAwait(false);

        if (token.IsCancellationRequested) return;

        // רק כשחוזרים לעדכן את ה-UI בפועל, קופצים חזרה ל-MainThread
        MainThread.BeginInvokeOnMainThread(() =>
        {
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
                    UserUploadedImageUrl = imageUrl,
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
        });
    }
    catch (TaskCanceledException)
    {
        // התעלמות, המשתמש פשוט המשיך להקליד אות נוספת
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Live search error: {ex.Message}");
    }
}


    /// פקודה שמופעלת בלחיצה על כפתור ה-➕ ברשימת החיפוש ומעבירה אותנו לשלב הבא
    [RelayCommand]
    private async Task SelectTrackAsync(SongPost selectedPost)
    {
        if (selectedPost == null) return;

    System.Diagnostics.Debug.WriteLine($"Selected track for posting: {selectedPost.TrackName}");
    
    // עצירת השיר של החיפוש מיד עם הבחירה
    CurrentSongUrl = string.Empty;
    IsSearchMode = false;
    
    // קופץ פופ-אפ זמני, כאן בהמשך נחבר את המצלמה/גלריה
    await Shell.Current.DisplayAlertAsync("השלב הבא", $"בחרת את השיר '{selectedPost.TrackName}', עכשיו נצלם או נבחר תמונה לפוסט!", "מעולה");
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
    private async Task LikeStoryAsync(SongPost story)
    {
        if (story == null) return;
        
        try
        {
            // Prevent rapid repeated clicks
            if (story.IsLiked)
                return;
            
            // עדכון ממשק משתמש מיד
            story.IsLiked = true;
            
            // שליחה לספוטיפיי
            bool success = await _spotifyService.LikeTrackAsync(story.SpotifyTrackId);
            
            if (!success)
            {
                // אם נכשל, חזור למצב הקודם
                story.IsLiked = false;
                System.Diagnostics.Debug.WriteLine("Failed to like track on Spotify");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Song '{story.TrackName}' liked on Spotify!");
            }
        }
        catch (Exception ex)
        {
            story.IsLiked = false;
            System.Diagnostics.Debug.WriteLine($"Error liking story: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddToPlaylistAsync(SongPost story)
    {
        if (story == null) return;
        
        try
        {
            // Prevent rapid repeated clicks
            if (story.IsAddedToPlaylist)
                return;
            
            // עדכון ממשק משתמש מיד
            story.IsAddedToPlaylist = true;
            
            // שליחה לספוטיפיי
            bool success = await _spotifyService.AddTrackToPlaylistAsync(story.SpotifyTrackId);
            
            if (!success)
            {
                // אם נכשל, חזור למצב הקודם
                story.IsAddedToPlaylist = false;
                System.Diagnostics.Debug.WriteLine("Failed to add track to playlist on Spotify");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Song '{story.TrackName}' added to playlist on Spotify!");
            }
        }
        catch (Exception ex)
        {
            story.IsAddedToPlaylist = false;
            System.Diagnostics.Debug.WriteLine($"Error adding to playlist: {ex.Message}");
        }
    }

    /// רשימת גיבוי עם נתונים פיקטיביים למקרה שאין אינטרנט או שהחיבור לספוטיפיי נכשל
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