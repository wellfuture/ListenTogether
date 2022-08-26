﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace ListenTogether.ViewModels;

public class SearchResultGroupViewModel : List<SearchResultViewModel>
{
    public string Name { get; private set; }

    public SearchResultGroupViewModel(string name, List<SearchResultViewModel> searchResults) : base(searchResults)
    {
        Name = name;
    }
}

public partial class SearchResultViewModel : ObservableObject
{
    /// <summary>
    /// 平台名称
    /// </summary>
    [ObservableProperty]
    private string _platform;

    /// <summary>
    /// 歌曲名称
    /// </summary>
    [ObservableProperty]
    private string _name;

    /// <summary>
    /// 歌曲别名
    /// </summary>
    [ObservableProperty]
    private string _alias;

    /// <summary>
    /// 歌手名称
    /// </summary>
    [ObservableProperty]
    private string _artist;

    /// <summary>
    /// 专辑名称
    /// </summary>
    [ObservableProperty]
    private string _album;

    /// <summary>
    /// 时长
    /// </summary>
    [ObservableProperty]
    private string _duration;

    /// <summary>
    /// 费用（免费、VIP等）
    /// </summary>
    [ObservableProperty]
    private string _fee;

    /// <summary>
    /// 源数据，对应的MusicSearchResult
    /// </summary>
    [ObservableProperty]
    private MusicSearchResult _sourceData;
}