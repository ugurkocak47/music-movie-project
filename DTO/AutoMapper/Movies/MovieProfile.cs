using AutoMapper;
using DTO.Movies;
using Entity;

namespace DTO.AutoMapper.Movies;

public class MovieProfile:Profile
{
    public MovieProfile()
    {
        CreateMap<CreateMovieDto, Movie>().ReverseMap();
        CreateMap<UpdateMovieDto, Movie>().ReverseMap();
        CreateMap<GetMovieDto, Movie>().ReverseMap();
    }
}