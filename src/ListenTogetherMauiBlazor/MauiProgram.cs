﻿using JiuLing.CommonLibs.ExtensionMethods;
using ListenTogether.Data;
using ListenTogether.Pages;
using ListenTogetherMauiBlazor.Storages;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Microsoft.Maui.LifecycleEvents;
using NativeMediaMauiLib;
namespace ListenTogetherMauiBlazor
{
    public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("NetConfig.json").Result;
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var netConfig = json.ToObject<NetConfig>();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseNativeMedia()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(windowsLifecycleBuilder =>
               {
                   windowsLifecycleBuilder.OnWindowCreated(window =>
                   {
                       window.ExtendsContentIntoTitleBar = false;
                       var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                       var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                       var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                       switch (appWindow.Presenter)
                       {
                           case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                               overlappedPresenter.SetBorderAndTitleBar(false, false);
                               break;
                       }
                   });
               });
            });
#endif

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DesktopMoving>();
            builder.Services.AddSingleton<DesktopNotification>();
            builder.Services.AddSingleton<ISearchHistoryStorage, SearchHistoryStorage>();
            builder.Services.AddSingleton<ILoginDataStorage, LoginDataStorage>();
            builder.Services.AddSingleton<ApiHttpMessageHandler>();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient("WebAPI", httpClient =>
            {
                httpClient.BaseAddress = new Uri(netConfig.ApiDomain);
                httpClient.Timeout = TimeSpan.FromSeconds(15);
            }).AddHttpMessageHandler<ApiHttpMessageHandler>();
            builder.Services.AddHttpClient("WebAPINoToken", httpClient =>
            {
                httpClient.BaseAddress = new Uri(netConfig.ApiDomain);
                httpClient.Timeout = TimeSpan.FromSeconds(15);
            });

            builder.Services.AddBusiness();
            builder.Services.AddMudServices();
            return builder.Build();
        }
    }
}