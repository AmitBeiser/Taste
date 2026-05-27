using SpotifyAPI.Web;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Maui.Authentication;

namespace Taste.Services;

public class SpotifyService
{
    private SpotifyClient? _spotifyClient;      // קליינט ציבורי לחיפושים כלליים
    private SpotifyClient? _userSpotifyClient;  // קליינט פרטי לפעולות המשתמש (לייקים, פלייליסטים)
    private readonly string _clientId;
    private readonly string _clientSecret;
    private const string RedirectUri = "tasteapp://callback";
    private string? _cachedAccessToken; // שמור את ה-token כדי לא תצטרך להתחבר שוב

    public SpotifyService(string clientId, string clientSecret)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    public async Task InitializeAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
                throw new InvalidOperationException("Spotify credentials not configured");
            
            Debug.WriteLine($"Initializing Spotify API with Client ID: {_clientId.Substring(0, Math.Min(8, _clientId.Length))}...");
            
            // Authenticate using Client Credentials flow
            var auth = new ClientCredentialsAuthenticator(_clientId, _clientSecret);
            var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(auth);
            _spotifyClient = new SpotifyClient(config);
            
            Debug.WriteLine("Spotify API initialized successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Spotify initialization failed: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<List<FullTrack>> SearchTracksAsync(string query, int limit = 10)
    {
        if (_spotifyClient == null)
            throw new InvalidOperationException("Spotify service not initialized. Call InitializeAsync first.");

        try
        {
            var searchRequest = new SearchRequest(SearchRequest.Types.Track, query) { Limit = limit };
            var searchResults = await _spotifyClient.Search.Item(searchRequest);
            
            return searchResults.Tracks.Items?.ToList() ?? new List<FullTrack>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error searching tracks: {ex.Message}");
            return new List<FullTrack>();
        }
    }

    public async Task<List<FullPlaylist>> GetFeaturedPlaylistsAsync(int limit = 10)
    {
        if (_spotifyClient == null)
            throw new InvalidOperationException("Spotify service not initialized. Call InitializeAsync first.");

        try
        {
            var playlists = await _spotifyClient.Browse.GetFeaturedPlaylists();
            return playlists.Playlists.Items?.Cast<FullPlaylist>().Take(limit).ToList() ?? new List<FullPlaylist>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching featured playlists: {ex.Message}");
            return new List<FullPlaylist>();
        }
    }

    /// מנהל את תהליך ההתחברות האישי של המשתמש דרך ה-WebAuthenticator של MAUI
    public async Task<bool> LoginAndAuthenticateUserAsync()
    {
        // אם יש token כבר במטמון, השתמש בו
        if (!string.IsNullOrEmpty(_cachedAccessToken))
        {
            _userSpotifyClient = new SpotifyClient(_cachedAccessToken);
            Debug.WriteLine("Using cached Spotify access token");
            return true;
        }

        if (_userSpotifyClient != null) return true; // המשתמש כבר מחובר

        // הרשאות ללייקים, קריאת פלייליסטים, ועריכת פלייליסטים (ציבוריים ופרטיים)
        var scopes = "user-library-modify playlist-read-private playlist-modify-public playlist-modify-private";
        
        // הכתובת הרשמית והתקינה של ספוטיפיי
        var authUrl = $"https://accounts.spotify.com/authorize?client_id={_clientId}&response_type=code&redirect_uri={Uri.EscapeDataString(RedirectUri)}&scope={Uri.EscapeDataString(scopes)}&show_dialog=true";

        try
        {
            var authResult = await WebAuthenticator.Default.AuthenticateAsync(
                new Uri(authUrl),
                new Uri(RedirectUri));

            // חילוץ ה-authorization code מתוך התוצאה
            string? authCode = authResult?.Properties?.FirstOrDefault(p => p.Key == "code").Value;
            
            if (string.IsNullOrEmpty(authCode))
            {
                Debug.WriteLine("No authorization code received from Spotify");
                return false;
            }

            // שלב 2: החלפת ה-code בתוך access token
            bool tokenExchangeSuccess = await ExchangeCodeForAccessTokenAsync(authCode);
            return tokenExchangeSuccess;
        }
        catch (TaskCanceledException)
        {
            Debug.WriteLine("User canceled the login process.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Authentication error: {ex.Message}");
        }

        return false;
    }

    /// מחליף את ה-authorization code בתוך access token דרך ה-backend
    private async Task<bool> ExchangeCodeForAccessTokenAsync(string authCode)
    {
        try
        {
            using var client = new HttpClient();
            
            // בקשה לקבלת access token
            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authCode },
                { "redirect_uri", RedirectUri },
                { "client_id", _clientId },
                { "client_secret", _clientSecret }
            };

            var content = new FormUrlEncodedContent(requestBody);
            
            // פנייה לשרת הטוקנים הרשמי של ספוטיפיי
            var response = await client.PostAsync("https://accounts.spotify.com/api/token", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                if (root.TryGetProperty("access_token", out var accessTokenElement))
                {
                    string accessToken = accessTokenElement.GetString()!;
                    _cachedAccessToken = accessToken; // שמור את ה-token במטמון
                    _userSpotifyClient = new SpotifyClient(accessToken);
                    Debug.WriteLine("User authenticated successfully with Spotify!");
                    return true;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Token exchange failed: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error exchanging code for token: {ex.Message}");
        }

        return false;
    }

    /// שולח בקשת API רשמית לשמירת השיר ב-Liked Songs של המשתמש
    public async Task<bool> LikeTrackAsync(string trackId)
    {
        try
        {
            var isAuth = await LoginAndAuthenticateUserAsync();
            if (!isAuth || _userSpotifyClient == null) return false;

            // הספרייה מצפה ל-ID נקי, ללא הקידומת של ספוטיפיי
            var request = new LibrarySaveItemsRequest(new List<string> { trackId });
            await _userSpotifyClient.Library.SaveItems(request);
            
            Debug.WriteLine($"Successfully liked track {trackId} on Spotify!");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error liking track: {ex.Message}");
            return false;
        }
    }

    /// מוסיף שיר לפלייליסט ייעודי של האפליקציה בחשבון של המשתמש. 
    /// אם הפלייליסט לא קיים, הוא מייצר אותו אוטומטית!
    public async Task<bool> AddTrackToPlaylistAsync(string trackId)
    {
        try
        {
            var isAuth = await LoginAndAuthenticateUserAsync();
            if (!isAuth || _userSpotifyClient == null) return false;

            // 1. נגלה מי המשתמש המחובר כרגע כדי לקבל את ה-ID שלו
            var currentUser = await _userSpotifyClient.UserProfile.Current();
            
            // 2. נחפש אם כבר קיים פלייליסט בשם "Taste Stories" אצל המשתמש
            var userPlaylists = await _userSpotifyClient.Playlists.CurrentUsers();
            var targetPlaylist = userPlaylists.Items?.FirstOrDefault(p => p.Name == "Taste Stories");

            string playlistId;

            // 3. אם הפלייליסט לא קיים, ניצור אותו מאפס
            if (targetPlaylist == null)
            {
                var createRequest = new PlaylistCreateRequest("Taste Stories")
                {
                    Public = true,
                    Description = "Songs added from my Taste App!"
                };
                var newPlaylist = await _userSpotifyClient.Playlists.Create(createRequest);
                playlistId = newPlaylist.Id!;
                Debug.WriteLine("Created a new 'Taste Stories' playlist.");
            }
            else
            {
                playlistId = targetPlaylist.Id!;
            }

            // 4. נוסיף את השיר לתוך הפלייליסט (כאן דווקא חובה להשתמש בקידומת)
            var addRequest = new PlaylistAddItemsRequest(new List<string> { $"spotify:track:{trackId}" });
            await _userSpotifyClient.Playlists.AddPlaylistItems(playlistId, addRequest);
            Debug.WriteLine($"Successfully added track {trackId} to playlist {playlistId}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding track to playlist: {ex.Message}");
            return false;
        }
    }
}