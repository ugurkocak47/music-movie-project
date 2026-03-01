using AutoMapper;
using DTO.MusicCategories;
using Entity;

namespace DTO.AutoMapper.Musics;

public class MusicCategoryProfile:Profile
{
    public MusicCategoryProfile()
    {
        CreateMap<CreateMusicCategoryDto, MusicCategory>().ReverseMap();
        CreateMap<UpdateMusicCategoryDto, MusicCategory>().ReverseMap();
        CreateMap<GetMusicCategoryDto, MusicCategory>().ReverseMap();
    }
}