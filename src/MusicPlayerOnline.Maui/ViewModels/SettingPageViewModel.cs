﻿using JiuLing.CommonLibs.ExtensionMethods;
using MusicPlayerOnline.Model.Enums;

namespace MusicPlayerOnline.Maui.ViewModels;

public class SettingPageViewModel : ViewModelBase
{
    public ICommand OpenUrlCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
    public ICommand ClearCacheCommand => new Command(ClearCache);
    public ICommand OpenLogCommand => new Command(OpenLog);
    public ICommand LoginCommand => new Command(Login);
    public ICommand LogoutCommand => new Command(Logout);
    public ICommand GoToRegisterCommand => new Command(GoToRegister);

    private IEnvironmentConfigService _configService;
    private IUserService _userService;
    private IUserLocalService _userLocalService;
    public SettingPageViewModel(IEnvironmentConfigService configService, IUserService userService, IUserLocalService userLocalService)
    {
        _configService = configService;
        _userService = userService;
        _userLocalService = userLocalService;
    }

    public async Task InitializeAsync()
    {
        GetAppConfig();
        UpdateUserInfo();

        //TODO IAppVersionInfo
        //VersionName = DependencyService.Get<IAppVersionInfo>().GetVersionName();
    }

    private bool _isLoginOk;
    /// <summary>
    /// 是否登录
    /// </summary>
    public bool IsLoginOk
    {
        get => _isLoginOk;
        set
        {
            _isLoginOk = value;
            OnPropertyChanged();
        }
    }

    private UserInfoViewModel _userInfo;
    /// <summary>
    /// 用户信息
    /// </summary>
    public UserInfoViewModel UserInfo
    {
        get => _userInfo;
        set
        {
            _userInfo = value;
            OnPropertyChanged();
        }
    }


    private string _loginUsername;
    public string LoginUsername
    {
        get => _loginUsername;
        set
        {
            _loginUsername = value;
            OnPropertyChanged();
        }
    }

    private string _loginPassword;
    public string LoginPassword
    {
        get => _loginPassword;
        set
        {
            _loginPassword = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 页面标题
    /// </summary>
    public string Title => "设置";

    private bool _isAutoCheckUpdate;
    /// <summary>
    /// 自动检查更新
    /// </summary>
    public bool IsAutoCheckUpdate
    {
        get => _isAutoCheckUpdate;
        set
        {
            _isAutoCheckUpdate = value;
            OnPropertyChanged();

            GlobalConfig.MyUserSetting.General.IsAutoCheckUpdate = value;
            WriteGeneralConfig();
        }
    }

    /// <summary>
    /// 深色主题
    /// </summary>
    public bool IsDarkMode
    {
        get => App.Current.UserAppTheme == AppTheme.Dark;
        set
        {
            App.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
            OnPropertyChanged();

            GlobalConfig.MyUserSetting.General.IsDarkMode = value;
            WriteGeneralConfig();
        }
    }

    private bool _isEnableNetEase;
    /// <summary>
    /// 网易云
    /// </summary>
    public bool IsEnableNetEase
    {
        get => _isEnableNetEase;
        set
        {
            _isEnableNetEase = value;
            OnPropertyChanged();

            EnableNetEase();
        }
    }

    private bool _isEnableKuGou;
    /// <summary>
    /// 酷狗
    /// </summary>
    public bool IsEnableKuGou
    {
        get => _isEnableKuGou;
        set
        {
            _isEnableKuGou = value;
            OnPropertyChanged();

            EnableKuGou();
        }
    }

    private bool _isEnableMiGu;
    /// <summary>
    /// 咪咕
    /// </summary>
    public bool IsEnableMiGu
    {
        get => _isEnableMiGu;
        set
        {
            _isEnableMiGu = value;
            OnPropertyChanged();

            EnableMiGu();
        }
    }

    private bool _isHideShortMusic;
    /// <summary>
    /// 隐藏小于1分钟的歌曲
    /// </summary>
    public bool IsHideShortMusic
    {
        get => _isHideShortMusic;
        set
        {
            _isHideShortMusic = value;
            OnPropertyChanged();

            GlobalConfig.MyUserSetting.Search.IsHideShortMusic = value;
            WriteSearchConfig();
        }
    }


    private bool _isWifiPlayOnly;
    /// <summary>
    /// 仅WIFI下可播放
    /// </summary>
    public bool IsWifiPlayOnly
    {
        get => _isWifiPlayOnly;
        set
        {
            _isWifiPlayOnly = value;
            OnPropertyChanged();

            GlobalConfig.MyUserSetting.Play.IsWifiPlayOnly = value;
            WritePlayConfig();
        }
    }

    private bool _isAutoNextWhenFailed;
    /// <summary>
    /// 播放失败时自动跳到下一首
    /// </summary>
    public bool IsAutoNextWhenFailed
    {
        get => _isAutoNextWhenFailed;
        set
        {
            _isAutoNextWhenFailed = value;
            OnPropertyChanged();

            GlobalConfig.MyUserSetting.Play.IsAutoNextWhenFailed = value;
            WritePlayConfig();
        }
    }


    private bool _isCloseSearchPageWhenPlayFailed;
    /// <summary>
    /// 播放失败时关闭搜索页面
    /// </summary>
    public bool IsCloseSearchPageWhenPlayFailed
    {
        get => _isCloseSearchPageWhenPlayFailed;
        set
        {
            _isCloseSearchPageWhenPlayFailed = value;
            OnPropertyChanged();

            GlobalConfig.MyUserSetting.Search.IsCloseSearchPageWhenPlayFailed = value;
            WriteSearchConfig();
        }
    }

    private bool _isCleanPlaylistWhenPlayMyFavorite;
    /// <summary>
    /// 播放我的歌单前清空播放列表
    /// </summary>
    public bool IsCleanPlaylistWhenPlayMyFavorite
    {
        get => _isCleanPlaylistWhenPlayMyFavorite;
        set
        {
            _isCleanPlaylistWhenPlayMyFavorite = value;
            OnPropertyChanged();

            GlobalConfig.MyUserSetting.Play.IsCleanPlaylistWhenPlayMyFavorite = value;
            WritePlayConfig();
        }
    }

    private string _versionName;
    /// <summary>
    /// 版本号
    /// </summary>
    public string VersionName
    {
        get => _versionName;
        set
        {
            _versionName = value;
            OnPropertyChanged();
        }
    }

    private void GetAppConfig()
    {
        //常规设置
        IsAutoCheckUpdate = GlobalConfig.MyUserSetting.General.IsAutoCheckUpdate;

        //搜索平台设置
        IsEnableNetEase = CheckEnablePlatform(PlatformEnum.NetEase);
        IsEnableKuGou = CheckEnablePlatform(PlatformEnum.KuGou);
        IsEnableMiGu = CheckEnablePlatform(PlatformEnum.MiGu);
        IsHideShortMusic = GlobalConfig.MyUserSetting.Search.IsHideShortMusic;
        IsCloseSearchPageWhenPlayFailed = GlobalConfig.MyUserSetting.Search.IsCloseSearchPageWhenPlayFailed;

        //播放设置
        IsWifiPlayOnly = GlobalConfig.MyUserSetting.Play.IsWifiPlayOnly;
        IsAutoNextWhenFailed = GlobalConfig.MyUserSetting.Play.IsAutoNextWhenFailed;
        IsCleanPlaylistWhenPlayMyFavorite = GlobalConfig.MyUserSetting.Play.IsCleanPlaylistWhenPlayMyFavorite;
    }

    private void UpdateUserInfo()
    {
        if (GlobalConfig.CurrentUser == null)
        {
            IsLoginOk = false;
        }
        else
        {
            IsLoginOk = true;
            UserInfo = new UserInfoViewModel()
            {
                Username = GlobalConfig.CurrentUser.Username,
                Nickname = GlobalConfig.CurrentUser.Nickname,
                Avatar = GlobalConfig.CurrentUser.Avatar
            };
        }
    }

    private bool CheckEnablePlatform(PlatformEnum platform)
    {
        if ((GlobalConfig.MyUserSetting.Search.EnablePlatform & platform) == platform)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 保存通用配置
    /// </summary>
    private async void WriteGeneralConfig()
    {
        await Task.Run(() =>
        {
            _configService.WriteGeneralSetting(GlobalConfig.MyUserSetting.General);
        });
    }

    /// <summary>
    /// 保存播放配置
    /// </summary>
    private async void WritePlayConfig()
    {
        await Task.Run(() =>
        {
            _configService.WritePlaySetting(GlobalConfig.MyUserSetting.Play);
        });
    }

    private async void EnableNetEase()
    {
        if (IsEnableNetEase)
        {
            if (!CheckEnablePlatform(PlatformEnum.NetEase))
            {
                GlobalConfig.MyUserSetting.Search.EnablePlatform = GlobalConfig.MyUserSetting.Search.EnablePlatform | PlatformEnum.NetEase;
            }
        }
        else
        {
            if (CheckEnablePlatform(PlatformEnum.NetEase))
            {
                GlobalConfig.MyUserSetting.Search.EnablePlatform = GlobalConfig.MyUserSetting.Search.EnablePlatform & ~PlatformEnum.NetEase;
            }
        }
        await WriteSearchConfig();
    }

    private async void EnableKuGou()
    {
        if (IsEnableKuGou)
        {
            if (!CheckEnablePlatform(PlatformEnum.KuGou))
            {
                GlobalConfig.MyUserSetting.Search.EnablePlatform = GlobalConfig.MyUserSetting.Search.EnablePlatform | PlatformEnum.KuGou;
            }
        }
        else
        {
            if (CheckEnablePlatform(PlatformEnum.KuGou))
            {
                GlobalConfig.MyUserSetting.Search.EnablePlatform = GlobalConfig.MyUserSetting.Search.EnablePlatform & ~PlatformEnum.KuGou;
            }
        }
        await WriteSearchConfig();
    }

    private async void EnableMiGu()
    {
        if (IsEnableMiGu)
        {
            if (!CheckEnablePlatform(PlatformEnum.MiGu))
            {
                GlobalConfig.MyUserSetting.Search.EnablePlatform = GlobalConfig.MyUserSetting.Search.EnablePlatform | PlatformEnum.MiGu;
            }
        }
        else
        {
            if (CheckEnablePlatform(PlatformEnum.MiGu))
            {
                GlobalConfig.MyUserSetting.Search.EnablePlatform = GlobalConfig.MyUserSetting.Search.EnablePlatform & ~PlatformEnum.MiGu;
            }
        }
        await WriteSearchConfig();
    }

    private async Task WriteSearchConfig()
    {
        await Task.Run(() =>
        {
            _configService.WriteSearchSetting(GlobalConfig.MyUserSetting.Search);
        });
    }

    private async void ClearCache()
    {
        //TODO ClearCache
        //await Shell.Current.GoToAsync($"{nameof(ClearCachePage)}", true);
    }

    private async void OpenLog()
    {
        //TODO OpenLog
        //await Shell.Current.GoToAsync($"{nameof(LogPage)}", true);
    }

    private async void Login()
    {
        if (LoginUsername.IsEmpty() || LoginPassword.IsEmpty())
        {
            ToastService.Show("请输入用户名和密码");
            return;
        }

        var user = await _userService.Login(LoginUsername, LoginPassword);
        if (user == null)
        {
            ToastService.Show("登录失败：用户名或密码错误");
            return;
        }

        if (!_userLocalService.Write(user))
        {
            ToastService.Show("用户信息保存失败，请重试");
            return;
        }
        //清除页面绑定的用户名密码
        LoginUsername = "";
        LoginPassword = "";

        GlobalConfig.CurrentUser = user;
        UpdateUserInfo();
    }

    private async void Logout()
    {
        if (!await _userService.Logout())
        {
            ToastService.Show("操作失败，请重试");
            return;
        }

        _userLocalService.Remove();
        GlobalConfig.CurrentUser = null;
        UpdateUserInfo();
    }

    private async void GoToRegister()
    {
        await Shell.Current.GoToAsync($"{nameof(RegisterPage)}", true);
    }
}
