using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Taste.Models;

namespace Taste.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<SongPost> PublicStories { get; set; } = new();

    public MainViewModel()
    {
        Title = "Taste Stories";
        LoadStories();
    }

    [RelayCommand]
    private void LikeStory(SongPost story)
    {
        // כאן נכתוב בעתיד את הלוגיקה שמעדכנת בשרת שסימנת לב
        // בינתיים זה רק לצורך ההבנה
    }

    private void LoadStories()
    {
        // דוגמה לסטורי בפיד הכללי
        PublicStories.Add(new SongPost 
        { 
            TrackName = "Starboy", 
            ArtistName = "The Weeknd",
            IsLocked = false // סטורי בפיד תמיד פתוח לכולם
        });
    }
}