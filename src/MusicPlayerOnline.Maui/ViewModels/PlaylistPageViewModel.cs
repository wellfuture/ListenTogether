﻿using JiuLing.CommonLibs.ExtensionMethods;
using System.Collections.ObjectModel;

namespace MusicPlayerOnline.Maui.ViewModels;

public class PlaylistPageViewModel : ViewModelBase
{
    private IServiceProvider _services;
    private IMusicService _musicService;
    private IPlaylistService _playlistService;
    private IMyFavoriteService _myFavoriteService;
    private readonly PlayerService _playerService;
    public ICommand AddToMyFavoriteCommand => new Command<MusicViewModel>(AddToMyFavorite);
    public ICommand PlayMusicCommand => new Command<MusicViewModel>(PlayMusic);
    public ICommand ClearPlaylistCommand => new Command(ClearPlaylist);

    public PlaylistPageViewModel(IServiceProvider services, PlayerService playerService)
    {
        Playlist = new ObservableCollection<PlaylistViewModel>();

        _services = services;
        _playerService = playerService;
    }

    public async Task InitializeAsync()
    {
        try
        {
            IsBusy = true;
            _playlistService = _services.GetService<IPlaylistServiceFactory>().Create();
            _musicService = _services.GetService<IMusicServiceFactory>().Create();
            _myFavoriteService = _services.GetService<IMyFavoriteServiceFactory>().Create();
            await GetPlaylist();

            OnPropertyChanged("IsPlaylistEmpty");
            OnPropertyChanged("IsPlaylistNotEmpty");
        }
        catch (Exception ex)
        {
            await ToastService.Show("播放列表加载失败");
            Logger.Error("播放列表页面初始化失败。", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
    private async Task GetPlaylist()
    {
        if (Playlist.Count > 0)
        {
            Playlist.Clear();
        }
        var playlist = await _playlistService.GetAllAsync();
        foreach (var item in playlist)
        {
            Playlist.Add(new PlaylistViewModel()
            {
                Id = item.Id,
                MusicId = item.MusicId,
                MusicName = item.MusicName,
                MusicArtist = item.MusicArtist
            });
        }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged("IsBusy");
            OnPropertyChanged("IsNotBusy");
        }
    }
    public bool IsNotBusy => !_isBusy;

    public bool IsPlaylistEmpty => IsBusy == false && (Playlist == null || Playlist.Count == 0);
    public bool IsPlaylistNotEmpty => !IsPlaylistEmpty;

    private string _searchKeyword;
    /// <summary>
    /// 搜索关键字
    /// </summary>
    public string SearchKeyword
    {
        get => _searchKeyword;
        set
        {
            _searchKeyword = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<PlaylistViewModel> _playlist;
    /// <summary>
    /// 搜索到的结果列表
    /// </summary>
    public ObservableCollection<PlaylistViewModel> Playlist
    {
        get => _playlist;
        set
        {
            _playlist = value;
            OnPropertyChanged();
        }
    }

    private async void PlayMusic(MusicViewModel selectedMusic)
    {
        var music = await _musicService.GetOneAsync(selectedMusic.Id);
        if (music == null)
        {
            await ToastService.Show("获取歌曲信息失败");
            return;
        }

        await _playerService.PlayAsync(music);
    }

    private async void AddToMyFavorite(MusicViewModel selectMusic)
    {
        try
        {
            IsBusy = true;

            var music = await _musicService.GetOneAsync(selectMusic.Id);
            if (music == null)
            {
                await ToastService.Show("歌曲不存在");
                return;
            }

            //构造待选择的歌单项
            string[] myFavoriteButtons = null;
            var myFavoriteList = await _myFavoriteService.GetAllAsync();
            if (myFavoriteList != null)
            {
                myFavoriteButtons = myFavoriteList.Select(x => x.Name).ToArray();
            }

            int selectedMyFavoriteId;
            string myFavoriteItem = await App.Current.MainPage.DisplayActionSheet("请选择要加入的歌单", "取消", "添加一个", myFavoriteButtons);
            if (myFavoriteItem.IsEmpty() || myFavoriteItem == "取消")
            {
                return;
            }

            if (myFavoriteItem != "添加一个")
            {
                //使用已有歌单
                selectedMyFavoriteId = myFavoriteList.First(x => x.Name == myFavoriteItem).Id;
            }
            else
            {
                //新增歌单
                string myFavoriteName = await App.Current.MainPage.DisplayPromptAsync("添加歌单", "请输入歌单名称：", "添加", "取消");
                if (myFavoriteName.IsEmpty())
                {
                    await ToastService.Show("操作取消");
                    return;
                }

                if (await _myFavoriteService.NameExist(myFavoriteName))
                {
                    await ToastService.Show("歌单名称已存在");
                    return;
                }

                var myFavorite = new MyFavorite()
                {
                    Name = myFavoriteName,
                    MusicCount = 0
                };
                var newMyFavorite = await _myFavoriteService.AddOrUpdateAsync(myFavorite);
                if (newMyFavorite == null)
                {
                    await ToastService.Show("添加失败");
                    return;
                }
                selectedMyFavoriteId = newMyFavorite.Id;
            }

            var result = await _myFavoriteService.AddMusicToMyFavorite(selectedMyFavoriteId, music);
            if (result == false)
            {
                await ToastService.Show("添加失败");
                return;
            }

            await _playerService.PlayAsync(music);
            await ToastService.Show("添加成功");
        }
        catch (Exception ex)
        {
            await ToastService.Show("添加失败，网络出小差了");
            Logger.Error("播放列表添加到我的歌单失败。", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async void RemovePlaylist(int id)
    {
        if (!await _playlistService.RemoveAsync(id))
        {
            await ToastService.Show("删除失败");
            return;
        }
        await GetPlaylist();
    }

    private async void ClearPlaylist()
    {
        var isOk = await App.Current.MainPage.DisplayAlert("提示", "确定要删除播放列表吗？", "确定", "取消");
        if (isOk == false)
        {
            return;
        }
        if (!await _playlistService.RemoveAllAsync())
        {
            await ToastService.Show("删除失败");
            return;
        }
        await ToastService.Show("删除成功");
        await InitializeAsync();
    }
}
