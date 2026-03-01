using AutoMapper;
using DTO.MusicCategories;
using Entity;

namespace DTO.AutoMapper.Movies;

public class MovieCategoryProfile:Profile
{
    public MovieCategoryProfile()
    {
        CreateMap<CreateMusicCategoryDto, MovieCategory>().ReverseMap();
        CreateMap<UpdateMusicCategoryDto, MovieCategory>().ReverseMap();
        CreateMap<GetMusicCategoryDto, MovieCategory>().ReverseMap();
    }
}