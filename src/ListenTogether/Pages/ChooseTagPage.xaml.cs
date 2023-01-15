namespace ListenTogether.Pages;

public partial class ChooseTagPage : ContentPage
{
    private ChooseTagPageViewModel ViewModel => BindingContext as ChooseTagPageViewModel;
    public ChooseTagPage(ChooseTagPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.InitializeAsync();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}