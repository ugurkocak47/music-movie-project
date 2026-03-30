using Entity;
using FluentValidation;

namespace DTO.ValidationRules;

public class MovieCategoryValidator:AbstractValidator<MovieCategory>
{
    public MovieCategoryValidator()
    {
        RuleFor(m => m.Name).NotEmpty().NotNull().WithMessage("Film kategorisi adı boş olamaz.");
        RuleFor(m => m.Description).MaximumLength(300)
            .WithMessage("Film kategorisi açıklaması en fazla 300 karakter olabilir.");
        // SuggestedMusicCategories validation removed - these are populated automatically via GenreMappingHelper
    }
}