﻿using JiuLing.CommonLibs.ExtensionMethods;
using JiuLing.CommonLibs.Security;
using ListenTogether.EasyLog;
using ListenTogether.Model;
using ListenTogether.Model.Enums;
using ListenTogether.Network.Models.NetEase;
using ListenTogether.Network.Utils;
using System.Net;

namespace ListenTogether.Network.MusicProvider;
public class NetEaseMusicProvider : IMusicProvider
{
    private readonly HttpClient _httpClient;
    private const PlatformEnum Platform = PlatformEnum.NetEase;

    public NetEaseMusicProvider()
    {
        var handler = new HttpClientHandler();
        handler.AutomaticDecompression = DecompressionMethods.All;
        _httpClient = new HttpClient(handler);
    }

    public async Task<List<string>?> GetSearchSuggestAsync(string keyword)
    {
        string url = $"{UrlBase.NetEase.Suggest}";

        var postData = NetEaseUtils.GetPostDataForSuggest(keyword);
        var form = new FormUrlEncodedContent(postData);

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = form
        };
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
        request.Headers.Add("User-Agent", RequestHeaderBase.UserAgentEdge);
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        ResultBase<SearchSuggestHttpResult>? result;
        try
        {
            result = json.ToObject<ResultBase<SearchSuggestHttpResult>>();
        }
        catch (Exception ex)
        {
            Logger.Error("解析网易搜索建议失败。", ex);
            return null;
        }

        if (result == null)
        {
            Logger.Info("解析网易搜索建议失败，服务器返回空。");
            return null;
        }
        if (result.code != 200 || result.result.songs == null)
        {
            Logger.Info("解析网易搜索建议失败，服务器返回参数异常。");
            return null;
        }
        return result.result.songs.Select(x => x.name).Distinct().ToList();
    }

    public async Task<List<MusicResultShow>> SearchAsync(string keyword)
    {
        var musics = new List<MusicResultShow>();

        string url = $"{UrlBase.NetEase.Search}";

        var postData = NetEaseUtils.GetPostDataForSearch(keyword);
        var form = new FormUrlEncodedContent(postData);

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = form
        };
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
        request.Headers.Add("User-Agent", RequestHeaderBase.UserAgentEdge);

        ResultBase<MusicSearchHttpResult>? result;
        try
        {
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            result = json.ToObject<ResultBase<MusicSearchHttpResult>>();
        }
        catch (Exception ex)
        {
            Logger.Error("解析网易搜索结果失败。", ex);
            return musics;
        }

        if (result == null || result.code != 200)
        {
            return musics;
        }

        foreach (var song in result.result.songs)
        {
            try
            {
                string artistName = "";
                if (song.ar.Length > 0)
                {
                    artistName = string.Join("、", song.ar.Select(x => x.name).ToList());
                }
                var music = new MusicResultShow()
                {
                    Id = MD5Utils.GetStringValueToLower($"{Platform}-{song.id}"),
                    Platform = Platform,
                    PlatformInnerId = song.id.ToString(),
                    Name = song.name,
                    Artist = artistName,
                    Album = song.al.name,
                    ImageUrl = $"{song.al.picUrl}?param=250y250",
                    Duration = TimeSpan.FromMilliseconds(song.dt),
                    Fee = GetFeeFlag(song.privilege)
                };
                musics.Add(music);
            }
            catch (Exception ex)
            {
                Logger.Error("构建网易搜索结果失败。", ex);
            }
        }
        return musics;
    }

    private FeeEnum GetFeeFlag(Privilege privilege)
    {

        if (privilege == null)
        {
            return FeeEnum.Free;
        }
        if (privilege.fee == 1)
        {
            return FeeEnum.Vip;
        }

        //猜测fee=0 flag=256 应该是地区无权限，样本太少没法分析
        //TODO 验证后在增加相关逻辑
        return FeeEnum.Free;
    }

    public Task<Music?> GetDetailAsync(MusicSearchResult sourceMusic, MusicFormatTypeEnum musicFormatType)
    {
        Music music = new Music()
        {
            Id = sourceMusic.Id,
            Platform = sourceMusic.Platform,
            //PlatformName = sourceMusic.Platform.GetDescription(),
            PlatformInnerId = sourceMusic.PlatformInnerId,
            Name = sourceMusic.Name,
            Artist = sourceMusic.Artist,
            Album = sourceMusic.Album,
            ImageUrl = sourceMusic.ImageUrl,
            ExtendData = ""
        };

#pragma warning disable CS8619 // 值中的引用类型的为 Null 性与目标类型不匹配。
        return Task.FromResult(music);
#pragma warning restore CS8619 // 值中的引用类型的为 Null 性与目标类型不匹配。
    }

    public async Task<string?> GetPlayUrlAsync(Music music, MusicFormatTypeEnum musicFormatType)
    {
        string url = $"{UrlBase.NetEase.GetMusic}";
        var postData = NetEaseUtils.GetPostDataForMusicUrl(music.PlatformInnerId);
        var form = new FormUrlEncodedContent(postData);

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = form
        };
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
        request.Headers.Add("User-Agent", RequestHeaderBase.UserAgentEdge);
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var httpResult = json.ToObject<ResultBase<MusicUrlHttpResult>>();
        if (httpResult == null)
        {
            return null;
        }
        if (httpResult.code != 200)
        {
            return null;
        }

        if (httpResult.data.Count == 0)
        {
            return null;
        }

        return httpResult.data[0].url;
    }

    public Task<List<SongMenu>> GetSongMenusFromTop()
    {
        throw new NotImplementedException();
    }

    public Task<List<MusicResultShow>> GetTopMusicsAsync(string topId)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>?> GetHotWordAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string> GetShareUrlAsync(Music music)
    {
        return Task.FromResult($"{UrlBase.NetEase.GetMusicPlayPage}?id={music.PlatformInnerId}");
    }

    public async Task<string?> GetLyricAsync(Music music)
    {
        //获取歌词
        var url = $"{UrlBase.NetEase.Lyric}";
        var postData = NetEaseUtils.GetPostDataForLyric(music.PlatformInnerId);
        var form = new FormUrlEncodedContent(postData);

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = form
        };
        request.Headers.Add("Accept", "application/json, */*");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
        request.Headers.Add("User-Agent", RequestHeaderBase.UserAgentEdge);
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var lyricResult = json.ToObject<MusicLyricHttpResult>();
        if (lyricResult == null)
        {
            return null;
        }
        if (lyricResult.code != 200)
        {
            return null;
        }

        if (lyricResult.lrc == null)
        {
            return null;
        }
        return lyricResult.lrc.lyric;
    }

    public async Task<(List<MusicTag> HotTags, List<MusicTypeTag> AllTypes)> GetMusicTagsAsync()
    {
        //热门标签
        string url = $"{UrlBase.NetEase.GetHotTagsUrl}";
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        foreach (var header in JiuLing.CommonLibs.Net.BrowserDefaultHeader.EdgeHeaders)
        {
            request.Headers.Add(header.Key, header.Value);
        }
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        var buffer = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        var html = System.Text.Encoding.UTF8.GetString(buffer);

        var hotTags = NetEaseUtils.GetHotTags(html);

        //全部标签
        url = $"{UrlBase.NetEase.GetAllTypesUrl}";
        request = new HttpRequestMessage()
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        foreach (var header in JiuLing.CommonLibs.Net.BrowserDefaultHeader.EdgeHeaders)
        {
            request.Headers.Add(header.Key, header.Value);
        }
        response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        buffer = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        html = System.Text.Encoding.UTF8.GetString(buffer);
        var allTypes = NetEaseUtils.GetAllTypes(html);

        return (hotTags, allTypes);
    }

    public Task<List<SongMenu>> GetSongMenusFromTagAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<MusicResultShow>> GetTagMusicsAsync(string tagId)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetPlayUrlAsync(string id, object? extendData = null)
    {
        throw new NotImplementedException();
    }
}
