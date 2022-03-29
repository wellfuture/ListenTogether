﻿using MusicPlayerOnline.Model;

namespace MusicPlayerOnline.Business.Interfaces;
public interface IMusicService
{
    Task<Music?> GetOneAsync(string id);
    Task<bool> AddOrUpdateAsync(Music music);
    Task<bool> UpdateCacheAsync(string id, string cachePath);
}