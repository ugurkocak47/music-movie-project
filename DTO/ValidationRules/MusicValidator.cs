using Entity;
using FluentValidation;

namespace DTO.ValidationRules;

public class MusicValidator:AbstractValidator<Music>
{
    public MusicValidator()
    {
        RuleFor(m => m.Name).NotEmpty().NotNull().WithMessage("Şarkı adı boş olamaz.2");
        RuleFor(m => m.SpotifyId).NotEmpty().NotNull().WithMessage("Spotify ID boş olamaz.");
        RuleFor(m => m.DurationMs).NotEmpty().NotEqual(0).WithMessage("Şarkı süresi boş ya da sıfır olamaz.");
        RuleFor(m => m.DurationMs).GreaterThan(20).WithMessage("Şarkı süresi 20 saniyeden kısa olamaz.");
    }
    
}