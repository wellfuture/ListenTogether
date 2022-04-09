﻿using JiuLing.CommonLibs.Net;
using MusicPlayerOnline.Model.Enums;

namespace MusicPlayerOnline.Maui.Services;
public class PlayerService
{
    private static IAudioService _audioService;
    private readonly IMusicNetworkService _musicNetworkService;
    private readonly IMusicService _musicService;
    private readonly IPlaylistService _playlistService;
    private readonly WifiOptionsService _wifiOptionsService;

    private readonly static HttpClientHelper _httpClient = new HttpClientHelper();

    public double CurrentPosition => _audioService.CurrentPosition;

    /// <summary>
    /// 是否正在播放
    /// </summary>
    public bool IsPlaying => _audioService.IsPlaying;

    /// <summary>
    /// 正在播放的歌曲信息
    /// </summary>
    public Music CurrentMusic => _currentMusic;
    private Music _currentMusic;

    public event EventHandler NewMusicAdded;
    public event EventHandler IsPlayingChanged;

    public PlayerService(IAudioService audioService, IMusicNetworkService musicNetworkService, IMusicServiceFactory musicServiceFactory, IPlaylistServiceFactory playlistServiceFactory, WifiOptionsService wifiOptionsService)
    {
        _audioService = audioService;
        _audioService.PlayFinished += async (_, _) => await Next();
        _audioService.PlayFailed += async (_, _) => await MediaFailed();

        _musicNetworkService = musicNetworkService;
        _musicService = musicServiceFactory.Create();
        _playlistService = playlistServiceFactory.Create();
        _wifiOptionsService = wifiOptionsService;
    }

    public async Task PlayAsync(Music music, double position = 0)
    {
        string musicPath = Path.Combine(GlobalConfig.MusicCacheDirectory, music.Id);
        if (!File.Exists(musicPath))
        {
            //缓存文件不存在时重新下载

            //网易的歌曲需要更新播放地址
            if (music.Platform == PlatformEnum.NetEase)
            {
                music = await _musicNetworkService.UpdatePlayUrl(music);
            }

            if (!_wifiOptionsService.HasWifiOrCanPlayWithOutWifi())
            {
                await MediaFailed();
                return;
            }

            var data = await _httpClient.GetReadByteArray(music.PlayUrl);
            File.WriteAllBytes(musicPath, data);
        }

        var isChangeMusic = _currentMusic?.Id != music.Id;

        if (isChangeMusic)
        {
            _currentMusic = music;
            if (_audioService.IsPlaying)
            {
                await _audioService.PauseAsync();
            }
            await _audioService.InitializeAsync(musicPath);
            await _audioService.PlayAsync(position);
            NewMusicAdded?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            await _audioService.PlayAsync(position);
        }
        IsPlayingChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task PauseAsync()
    {
        await _audioService.PauseAsync();
        IsPlayingChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 上一首
    /// </summary>
    public async Task Previous()
    {
        if (GlobalConfig.MyUserSetting.Player.PlayMode == PlayModeEnum.RepeatOne)
        {
            await PlayAsync(CurrentMusic);
            return;
        }

        var playlist = await _playlistService.GetAllAsync();
        if (playlist.Count == 0)
        {
            return;
        }

        if (GlobalConfig.MyUserSetting.Player.PlayMode == PlayModeEnum.RepeatList)
        {
            int nextId = 0;
            for (int i = 0; i < playlist.Count; i++)
            {
                if (playlist[i].MusicId == CurrentMusic.Id)
                {
                    nextId = i - 1;
                    break;
                }
            }
            //列表第一首
            if (nextId < 0)
            {
                nextId = playlist.Count - 1;
            }

            var music = await _musicService.GetOneAsync(playlist[nextId].MusicId);
            await PlayAsync(music);
            return;
        }
        if (GlobalConfig.MyUserSetting.Player.PlayMode == PlayModeEnum.Shuffle)
        {
            if (playlist.Count <= 1)
            {
                await PlayAsync(CurrentMusic);
                return;
            }

            string randomMusicId;
            do
            {
                randomMusicId = JiuLing.CommonLibs.Random.RandomUtils.GetOneFromList<Playlist>(playlist).MusicId;
            } while (randomMusicId == CurrentMusic.Id);
            var music = await _musicService.GetOneAsync(randomMusicId);
            await PlayAsync(music);
        }
    }

    /// <summary>
    /// 下一首
    /// </summary>
    public async Task Next()
    {
        if (GlobalConfig.MyUserSetting.Player.PlayMode == PlayModeEnum.RepeatOne)
        {
            await PlayAsync(CurrentMusic);
            return;
        }

        var playlist = await _playlistService.GetAllAsync();
        if (playlist.Count == 0)
        {
            return;
        }
        if (GlobalConfig.MyUserSetting.Player.PlayMode == PlayModeEnum.RepeatList)
        {
            int nextId = 0;
            for (int i = 0; i < playlist.Count; i++)
            {
                if (playlist[i].MusicId == CurrentMusic.Id)
                {
                    nextId = i + 1;
                    break;
                }
            }
            //列表最后一首
            if (playlist.Count == nextId)
            {
                nextId = 0;
            }

            var music = await _musicService.GetOneAsync(playlist[nextId].MusicId);
            await PlayAsync(music);
            return;
        }
        if (GlobalConfig.MyUserSetting.Player.PlayMode == PlayModeEnum.Shuffle)
        {
            if (playlist.Count <= 1)
            {
                await PlayAsync(CurrentMusic);
                return;
            }

            string randomMusicId;
            do
            {
                randomMusicId = JiuLing.CommonLibs.Random.RandomUtils.GetOneFromList<Playlist>(playlist).MusicId;
            } while (randomMusicId == CurrentMusic.Id);
            var music = await _musicService.GetOneAsync(randomMusicId);
            await PlayAsync(music);
        }
    }

    private async Task MediaFailed()
    {
        if (GlobalConfig.MyUserSetting.Play.IsAutoNextWhenFailed)
        {
            await Next();
        }
    }
}