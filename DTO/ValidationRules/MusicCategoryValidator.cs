using Entity;
using FluentValidation;

namespace DTO.ValidationRules;

public class MusicCategoryValidator:AbstractValidator<MusicCategory>
{
    public MusicCategoryValidator()
    {
        RuleFor(m => m.Name).NotEmpty().NotNull().WithMessage("Müzik kategorisi adı boş olamaz.");
        RuleFor(m => m.Description).MaximumLength(300)
            .WithMessage("Müzik kategorisi açıklaması 300 karakterden fazla olamaz.");
    }
}