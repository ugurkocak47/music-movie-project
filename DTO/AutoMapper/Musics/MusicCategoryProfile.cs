using AutoMapper;
using DTO.MusicCategories;
using Entity;

namespace DTO.AutoMapper.Musics;

public class MusicCategoryProfile:Profile
{
    public MusicCategoryProfile()
    {
        CreateMap<CreateMusicCategoryDto, MusicCategory>();
        CreateMap<UpdateMusicCategoryDto, MusicCategory>();
        
        // Explicitly ignore the inverse navigation property to prevent circular references
        CreateMap<MusicCategory, GetMusicCategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
            .ForMember(dest => dest.DeletedDate, opt => opt.MapFrom(src => src.DeletedDate))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted));
        
        CreateMap<GetMusicCategoryDto, MusicCategory>();
    }
}