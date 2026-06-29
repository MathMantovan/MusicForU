namespace MusicForU.Application.DTOs;

public record CreatePlaylistDto(string Name);
public record AddSongToPlaylistDto(int SongId);
public record PlaylistDto(int Id, string Name, List<SongResultDto> Songs);
