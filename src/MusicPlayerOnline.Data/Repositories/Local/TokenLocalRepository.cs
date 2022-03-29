﻿using MusicPlayerOnline.Data.Entities;
using MusicPlayerOnline.Data.Interfaces;
using MusicPlayerOnline.Model;

namespace MusicPlayerOnline.Data.Repositories.Local;
public class TokenLocalRepository : ITokenRepository
{
    public async Task<bool> Write(TokenInfo tokenInfo)
    {
        var myTokenInfo = await DatabaseProvide.DatabaseAsync.Table<TokenEntity>().FirstOrDefaultAsync();
        if (myTokenInfo == null)
        {
            throw new Exception("用户授权数据不完整，写入失败");
        }

        myTokenInfo.Token = tokenInfo.Token;
        myTokenInfo.RefreshToken = tokenInfo.RefreshToken;

        int count = await DatabaseProvide.DatabaseAsync.UpdateAsync(myTokenInfo);
        return count != 0;
    }

    public async Task<TokenInfo> Read()
    {
        var tokens = await DatabaseProvide.DatabaseAsync.Table<TokenEntity>().ToListAsync();

        if (tokens == null || tokens.Count != 1)
        {
            throw new Exception("用户授权数据不完整，读取失败");
        }

        return new TokenInfo()
        {
            Token = tokens[0].Token,
            RefreshToken = tokens[0].RefreshToken
        };
    }
}