using System;

namespace DTO.AppUsers
{
    public class CurrentUserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public bool Gender { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}
