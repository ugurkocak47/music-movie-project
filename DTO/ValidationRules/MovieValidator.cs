using Entity;
using FluentValidation;

namespace DTO.ValidationRules;

public class MovieValidator:AbstractValidator<Movie>
{
    public MovieValidator()
    {
        RuleFor(m => m.Categories).NotEmpty().WithMessage("Film kategorisi boş olamaz.");
        RuleFor(m => m.Rating).NotEmpty().NotEqual(0).WithMessage("Film puanı boş olamaz.");
        RuleFor(m => m.Rating).GreaterThan(0.5).LessThan(10.0)
            .WithMessage("Film puanı kabul edilen aralıkta değil (0.5-10).");
        RuleFor(m => m.Title).NotEmpty().NotNull().WithMessage("Film adı boş olamaz.");
        RuleFor(m => m.TmdbId).NotEmpty().NotNull().NotEqual(0).WithMessage("Film ID boş olamaz.");
        
    }
}