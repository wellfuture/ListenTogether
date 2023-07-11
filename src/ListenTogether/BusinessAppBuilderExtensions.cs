﻿using ListenTogether.Business.Services;
using ListenTogether.Data.Interface;
using ListenTogether.Data.Maui;
using ListenTogether.Data.Repositories.Api;
using ListenTogether.Network;

namespace ListenTogether;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseBusiness(this MauiAppBuilder builder)
    {
        //网络数据平台
        builder.Services.AddSingleton<MusicNetPlatform>();
        //本地服务
        builder.Services.AddSingleton<IEnvironmentConfigService, EnvironmentConfigService>();
        builder.Services.AddSingleton<IMusicNetworkService, MusicNetworkService>();
        builder.Services.AddSingleton<IPlaylistService, PlaylistService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IMyFavoriteService, MyFavoriteService>();
        builder.Services.AddSingleton<IMusicService, MusicService>();

        //数据服务
        builder.Services.AddSingleton<IEnvironmentConfigRepository, EnvironmentConfigLocalRepository>();
        builder.Services.AddSingleton<IMusicRepository, MusicApiRepository>();
        builder.Services.AddSingleton<IMyFavoriteRepository, MyFavoriteApiRepository>();
        builder.Services.AddSingleton<IPlaylistRepository, PlaylistLocalRepository>();
        builder.Services.AddSingleton<IUserApiRepository, UserApiRepository>();
        return builder;
    }
}
