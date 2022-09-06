﻿namespace ListenTogether.Filters.MusicSearchFilter;
internal class ShortMusicFilter : IMusicSearchFilter
{
    public List<MusicSearchResult>? Filter(List<MusicSearchResult>? musics)
    {
        if (musics == null)
        {
            return default;
        }

        return musics.Where(x => x.Duration.TotalMilliseconds == 0 || x.Duration.TotalMilliseconds >= 60 * 1000).ToList();
    }
}