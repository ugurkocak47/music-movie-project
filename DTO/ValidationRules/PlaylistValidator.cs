using Entity;
using FluentValidation;

namespace DTO.ValidationRules;

public class PlaylistValidator:AbstractValidator<Playlist>
{
    public PlaylistValidator()
    {
        RuleFor(p => p.PlaylistName).NotEmpty().NotNull().WithMessage("Playlist adı boş olamaz.");
        RuleFor(p => p.PlaylistName).MaximumLength(50).MinimumLength(1)
            .WithMessage("Playlist adı en az 1, en fazla 50 karakter içerebilir.");
    }
}