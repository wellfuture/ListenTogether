﻿using System.Collections.ObjectModel;

namespace MusicPlayerOnline.Maui.ViewModels;

[QueryProperty(nameof(AddedMusicId), nameof(AddedMusicId))]
public class AddToMyFavoritePageViewModel : ViewModelBase
{
    private IServiceProvider _services;
    private IMyFavoriteService _myFavoriteService;
    private IMusicService _musicService;

    public ICommand AddMyFavoriteCommand => new Command(AddMyFavorite);
    public ICommand SelectedChangedCommand => new Command<MyFavoriteViewModel>(SelectedChangedDo);
    public AddToMyFavoritePageViewModel(IServiceProvider services)
    {
        MyFavoriteList = new ObservableCollection<MyFavoriteViewModel>();
        _services = services;
    }

    public async Task InitializeAsync()
    {
        _myFavoriteService = _services.GetService<IMyFavoriteServiceFactory>().Create();
        _musicService = _services.GetService<IMusicServiceFactory>().Create();

        await BindingMyFavoriteList();

        if (AddedMusicId.IsNotEmpty())
        {
            AddedMusic = await _musicService.GetOneAsync(AddedMusicId);
        }
    }

    private string _addedMusicId;
    /// <summary>
    /// 要添加的歌曲ID
    /// </summary>
    public string AddedMusicId
    {
        get => _addedMusicId;
        set
        {
            _addedMusicId = value;
            OnPropertyChanged();
        }
    }

    private Music _addedMusic;
    public Music AddedMusic
    {
        get => _addedMusic;
        set
        {
            _addedMusic = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<MyFavoriteViewModel> _myFavoriteList;
    public ObservableCollection<MyFavoriteViewModel> MyFavoriteList
    {
        get => _myFavoriteList;
        set
        {
            _myFavoriteList = value;
            OnPropertyChanged();
        }
    }

    private async Task BindingMyFavoriteList()
    {
        MyFavoriteList.Clear();
        var myFavoriteList = await _myFavoriteService.GetAllAsync();
        foreach (var myFavorite in myFavoriteList)
        {
            MyFavoriteList.Add(new MyFavoriteViewModel()
            {
                Id = myFavorite.Id,
                Name = myFavorite.Name,
                MusicCount = myFavorite.MusicCount,
                ImageUrl = myFavorite.ImageUrl
            });
        }
    }

    private async void AddMyFavorite()
    {
        await Shell.Current.GoToAsync($"{nameof(MyFavoriteAddPage)}?{nameof(MyFavoriteAddPageViewModel.AddedMusicId)}={AddedMusicId}", true);
    }
    private async void SelectedChangedDo(MyFavoriteViewModel selected)
    {
        if (AddedMusic == null)
        {
            return;
        }
        var result = await _myFavoriteService.AddMusicToMyFavorite(selected.Id, AddedMusic);
        if (result == false)
        {
            await Shell.Current.GoToAsync($"..", true);
            ToastService.Show("添加失败");
            return;
        }
        await Shell.Current.GoToAsync($"..", true);
        ToastService.Show("添加成功");
    }
}