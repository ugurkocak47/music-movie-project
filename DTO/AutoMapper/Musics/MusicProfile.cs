using AutoMapper;
using DTO.Musics;
using Entity;

namespace DTO.AutoMapper.Musics;

public class MusicProfile:Profile
{
    public MusicProfile()
    {
        CreateMap<CreateMusicDto, Music>().ReverseMap();
        CreateMap<UpdateMusicDto, Music>().ReverseMap();
        CreateMap<GetMusicDto, Music>().ReverseMap();
    }
}