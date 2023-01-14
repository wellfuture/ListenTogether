﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using ListenTogether.Model.Enums;

namespace ListenTogether.ViewModels;

[QueryProperty(nameof(TagId), nameof(TagId))]
public partial class MiGuPageViewModel : ViewModelBase
{
    private string _allTypesJson;

    [ObservableProperty]
    private string _tagId = null!;
    async partial void OnTagIdChanged(string value)
    {
        if (value.IsEmpty())
        {
            return;
        }
        await GetTagDetailInnerAsync(value);
    }

    [ObservableProperty]
    private ObservableCollection<MusicTagViewModel> _hotTags;

    [ObservableProperty]
    private ObservableCollection<SongMenuViewModel> _songMenus;

    private readonly IMusicNetworkService _musicNetworkService;

    public MiGuPageViewModel(IMusicNetworkService musicNetworkService)
    {
        _musicNetworkService = musicNetworkService;

        HotTags = new ObservableCollection<MusicTagViewModel>();
        SongMenus = new ObservableCollection<SongMenuViewModel>();
    }
    public async Task InitializeAsync()
    {
        if (_allTypesJson.IsEmpty())
        {
            var (hotTags, allTypes) = await _musicNetworkService.GetMusicTagsAsync(PlatformEnum.MiGu);

            _allTypesJson = allTypes.ToJson();

            HotTags = new ObservableCollection<MusicTagViewModel>();
            HotTags.Add(new MusicTagViewModel()
            {
                Id = "榜单",
                Name = "榜单"
            });
            foreach (var hotTag in hotTags)
            {
                HotTags.Add(new MusicTagViewModel()
                {
                    Id = hotTag.Id,
                    Name = hotTag.Name,
                });
            }
            HotTags.Add(new MusicTagViewModel()
            {
                Id = "选择分类",
                Name = "选择分类"
            });

            await GetTagDetailInnerAsync("榜单");
        }
    }

    [RelayCommand]
    private async void GetTagDetailAsync(string Id)
    {
        await GetTagDetailInnerAsync(Id);
    }

    private async Task GetTagDetailInnerAsync(string tagId)
    {
        foreach (var tag in HotTags)
        {
            tag.BackgroundColor = tag.Id == tagId ? Color.FromArgb("#C98FFF") : Color.FromArgb("#00000000");
        }

        switch (tagId)
        {
            case "榜单":
                break;
            case "选择分类":
                await Shell.Current.GoToAsync($"ChooseTagPage?AllTypesJson={_allTypesJson}");
                break;
            default:
                await GetTagPlaylistAsync(tagId);
                break;
        }
    }

    private async Task GetTagPlaylistAsync(string tagId)
    {
        SongMenus.Clear();

        var songMenus = await _musicNetworkService.GetSongMenusFromTagAsync(PlatformEnum.MiGu, tagId);
        foreach (var songMenu in songMenus)
        {
            SongMenus.Add(new SongMenuViewModel()
            {
                Name = songMenu.Name,
                ImageUrl = songMenu.ImageUrl,
                LinkUrl = songMenu.LinkUrl
            });
        }
    }
}