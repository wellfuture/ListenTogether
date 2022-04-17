﻿using MusicPlayerOnline.Business.Factories;
using MusicPlayerOnline.Business.Interfaces;
using MusicPlayerOnline.Business.Services;
using MusicPlayerOnline.Data.Interfaces;
using MusicPlayerOnline.Data.Repositories.Api;
using MusicPlayerOnline.Data.Repositories.Local;
using MusicPlayerOnline.Network;

namespace MusicPlayerOnline.Business;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseBusiness(this MauiAppBuilder builder)
    {
        //网络数据平台
        builder.Services.AddSingleton<MusicNetPlatform>();

        //本地、远程服务工厂
        builder.Services.AddSingleton<IMusicServiceFactory, MusicServiceFactory>();
        builder.Services.AddSingleton<IMyFavoriteServiceFactory, MyFavoriteServiceFactory>();
        builder.Services.AddSingleton<IPlaylistServiceFactory, PlaylistServiceFactory>();

        //本地服务
        builder.Services.AddSingleton<IEnvironmentConfigService, EnvironmentConfigService>();
        builder.Services.AddSingleton<IMusicNetworkService, MusicNetworkService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IUserLocalService, UserLocalService>();
        builder.Services.AddSingleton<IApiLogService, ApiLogService>();

        //数据服务
        builder.Services.AddSingleton<IEnvironmentConfigRepository, EnvironmentConfigLocalRepository>();
        builder.Services.AddSingleton<IMusicRepository, MusicApiRepository>();
        builder.Services.AddSingleton<IMusicRepository, MusicLocalRepository>();
        builder.Services.AddSingleton<IMyFavoriteRepository, MyFavoriteApiRepository>();
        builder.Services.AddSingleton<IMyFavoriteRepository, MyFavoriteLocalRepository>();
        builder.Services.AddSingleton<IPlaylistRepository, PlaylistApiRepository>();
        builder.Services.AddSingleton<IPlaylistRepository, PlaylistLocalRepository>();
        builder.Services.AddSingleton<IUserLocalRepository, UserLocalRepository>();
        builder.Services.AddSingleton<IUserApiRepository, UserApiRepository>();
        builder.Services.AddSingleton<ILogRepository, LogApiRepository>();
        return builder;
    }
}
