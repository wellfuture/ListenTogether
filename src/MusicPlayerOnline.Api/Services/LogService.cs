﻿using MusicPlayerOnline.Api.DbContext;
using MusicPlayerOnline.Api.Entities;
using MusicPlayerOnline.Api.Interfaces;
using MusicPlayerOnline.Model.Api.Request;

namespace MusicPlayerOnline.Api.Services;
public class LogService : ILogService
{
    private readonly DataContext _context;
    public LogService(DataContext dataContext)
    {
        _context = dataContext;
    }
    public async Task WriteAsync(int userId, LogRequest log)
    {
        var myLog = new LogEntity()
        {
            UserBaseId = userId,
            LogType = log.LogType.ToString(),
            Message = log.Message,
            Timestamp = log.Timestamp,
            CreateTime = DateTime.Now
        };
        _context.Logs.Add(myLog);
        await _context.SaveChangesAsync();
    }
}