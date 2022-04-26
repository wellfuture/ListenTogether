﻿namespace MusicPlayerOnline.Maui.ViewModels;

public static class ViewModelExtensions
{
    public static MauiAppBuilder ConfigureViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<SearchResultPageViewModel>();
        builder.Services.AddTransient<SearchResultViewModel>();
        builder.Services.AddTransient<PlaylistPageViewModel>();
        builder.Services.AddTransient<MyFavoritePageViewModel>();
        builder.Services.AddTransient<MyFavoriteDetailPageViewModel>();

        builder.Services.AddSingleton<ShellViewModel>();
        builder.Services.AddSingleton<SettingPageViewModel>();
        builder.Services.AddSingleton<PlayingPageViewModel>();
        builder.Services.AddSingleton<LoginPageViewModel>();
        builder.Services.AddSingleton<RegisterPageViewModel>();
        builder.Services.AddSingleton<CacheCleanViewModel>();
        builder.Services.AddSingleton<LogPageViewModel>();
        return builder;
    }
}