# Spotify Integration Setup Guide

## What Was Implemented

Your Taste app now integrates with **Spotify's Web API** to fetch real song preview data! Here's what was added:

### New Components:

1. **SpotifyService** (`Services/SpotifyService.cs`)
   - Handles all Spotify API communication
   - Methods to search tracks, get featured playlists, and fetch track details
   - Automatic authentication using Client Credentials flow

2. **Updated MainViewModel**
   - Initializes Spotify service on app startup
   - Fetches real song data from Spotify instead of dummy data
   - Falls back to dummy data if Spotify authentication fails

3. **Updated SongPost Model**
   - Added `SpotifyTrackId` and `SpotifyArtistIds` fields
   - Made properties nullable to prevent compilation issues

## Getting Spotify API Credentials

### Step 1: Create a Spotify Developer Account
1. Go to https://developer.spotify.com/dashboard
2. Sign up or log in with your Spotify account
3. Accept the terms and create an app

### Step 2: Get Your Credentials
1. After creating an app, you'll see your **Client ID** and **Client Secret**
2. Keep these credentials **secure** - never commit them to public repositories

## How to Add Your Credentials

Open `MauiProgram.cs` and replace the placeholder values:

```csharp
// In MauiProgram.cs, around line 24:
var spotifyClientId = "YOUR_SPOTIFY_CLIENT_ID";
var spotifyClientSecret = "YOUR_SPOTIFY_CLIENT_SECRET";
```

Replace them with your actual credentials:

```csharp
var spotifyClientId = "abc123def456...";  // Your Client ID
var spotifyClientSecret = "xyz789...";    // Your Client Secret
```

### ⚠️ Security Best Practice
For production, you should:
- Store credentials in environment variables
- Use Azure Key Vault or similar secure storage
- Never commit credentials to version control

## How It Works

1. **App Startup**: When the app loads, it initializes the Spotify service
2. **Song Loading**: The app searches for popular songs on Spotify
3. **Preview URLs**: Each song's 30-second preview URL is fetched
4. **CarouselView Display**: Songs are displayed in the carousel
5. **Auto-Play**: When you swipe to a new song, it automatically plays the preview

## Features

✅ Real Spotify song data (name, artist, album art)  
✅ 30-second song previews from Spotify  
✅ Automatic song switching when swiping  
✅ Fallback to dummy data if Spotify fails  
✅ Error logging for debugging  

## Testing

1. Add your Spotify credentials to `MauiProgram.cs`
2. Build and run the app
3. You should see real Spotify tracks with their previews
4. Swipe through the carousel - songs should auto-play

## Troubleshooting

**No songs loading?**
- Check your Client ID and Secret are correct
- Check internet connection
- Look at the Debug console output for error messages

**Authentication failing?**
- Verify your Spotify credentials
- Ensure your app is registered on Spotify Developer Dashboard

**Songs not previewing?**
- Some Spotify tracks don't have preview URLs available
- Try searching for different popular tracks
- Check the debug output: "Loaded X stories from Spotify"

## Next Steps

You can enhance this by:
- Adding a search UI to let users find specific songs
- Saving favorite songs to a database
- Implementing user authentication with Spotify
- Adding playlist support
- Sharing songs with friends
