﻿namespace ListenTogether.Data;

public class DataConfig
{
    /// <summary>
    /// API 的一些配置信息
    /// </summary>
    internal static ApiSettings ApiSetting { get; set; } = null!;

    public static void SetWebApi(string deviceId)
    {
        ApiSetting = new ApiSettings(deviceId);
    }
}