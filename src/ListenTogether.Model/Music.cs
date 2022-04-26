﻿using MusicPlayerOnline.Model.Enums;

namespace MusicPlayerOnline.Model;

public class Music : MusicBase
{
    /// <summary>
    /// 平台
    /// </summary>
    public PlatformEnum Platform { get; set; }

    /// <summary>
    /// 平台名称
    /// </summary>
    public string PlatformName { get; set; } = null!;
    /// <summary>
    /// 对应平台的ID
    /// </summary>
    public string PlatformInnerId { get; set; } = null!;

    /// <summary>
    /// 缓存文件名
    /// </summary>
    public string CacheFileName => $"{PlatformName}-{Name}-{Artist}";
}