using AutoMapper;
using DTO.Playlists;
using Entity;

namespace DTO.AutoMapper.Playlists;

public class PlaylistProfile:Profile

{
    public PlaylistProfile()
    {

        CreateMap<CreatePlaylistDto, Playlist>().ReverseMap();
        CreateMap<UpdatePlaylistDto, Playlist>().ReverseMap();
        CreateMap<GetPlaylistDto, Playlist>().ReverseMap();
    }
}