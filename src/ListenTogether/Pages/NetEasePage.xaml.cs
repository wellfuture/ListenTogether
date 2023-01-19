using ListenTogether.Model.Enums;

namespace ListenTogether.Pages;

public partial class NetEasePage : ContentPage
{
    DiscoverViewModel vm => BindingContext as DiscoverViewModel;
    public NetEasePage(DiscoverViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.InitializeAsync(PlatformEnum.NetEase);
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}