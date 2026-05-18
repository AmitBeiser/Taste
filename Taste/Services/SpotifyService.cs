using SpotifyAPI.Web;
using System.Diagnostics;

namespace Taste.Services;

public class SpotifyService
{
    private SpotifyClient? _spotifyClient;
    private readonly string _clientId;
    private readonly string _clientSecret;

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
}
