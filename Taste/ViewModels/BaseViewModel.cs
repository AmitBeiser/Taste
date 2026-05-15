using CommunityToolkit.Mvvm.ComponentModel;
namespace Taste.ViewModels;
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    public bool IsNotBusy => !IsBusy;
}