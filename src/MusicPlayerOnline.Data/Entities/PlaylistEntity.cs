﻿using SQLite;

namespace MusicPlayerOnline.Data.Entities;

[Table("Playlist")]
internal class PlaylistEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string MusicId { get; set; } = null!;
    public string MusicName { get; set; } = null!;
    public string MusicArtist { get; set; } = null!;
}