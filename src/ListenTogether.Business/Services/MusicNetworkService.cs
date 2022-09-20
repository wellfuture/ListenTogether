﻿using ListenTogether.Business.Interfaces;
using ListenTogether.Model;
using ListenTogether.Model.Enums;
using ListenTogether.Network;

namespace ListenTogether.Business.Services;

public class MusicNetworkService : IMusicNetworkService
{
    private readonly MusicNetPlatform _musicNetPlatform;
    public MusicNetworkService(MusicNetPlatform musicNetPlatform)
    {
        _musicNetPlatform = musicNetPlatform;
    }

    public async Task<List<string>?> GetSearchSuggestAsync(string keyword)
    {
        return await _musicNetPlatform.GetSearchSuggestAsync(keyword);
    }

    public async Task<List<MusicSearchResult>> SearchAsync(PlatformEnum platform, string keyword)
    {
        return await _musicNetPlatform.SearchAsync(platform, keyword);
    }

    public async Task<Music?> GetDetailAsync(MusicSearchResult musicSearchResult, MusicFormatTypeEnum musicFormatType)
    {
        return await _musicNetPlatform.GetDetailAsync(musicSearchResult, musicFormatType);
    }

    public async Task<string?> GetPlayUrlAsync(Music music, MusicFormatTypeEnum musicFormatType)
    {
        return await _musicNetPlatform.GetPlayUrlAsync(music, musicFormatType);
    }

    public Task<string> GetMusicPlayPageUrlAsync(Music music)
    {
        return _musicNetPlatform.GetMusicPlayPageUrlAsync(music);
    }

    public async Task<List<string>?> GetHotWordAsync()
    {
        return await _musicNetPlatform.GetHotWordAsync();
    }
}