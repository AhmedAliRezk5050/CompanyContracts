using Microsoft.AspNetCore.Identity;

namespace Core.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public string? UserId { get; set; } 
    public AppUser? User { get; set; }
}