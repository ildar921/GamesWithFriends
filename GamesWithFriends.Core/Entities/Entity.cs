using System.ComponentModel.DataAnnotations;

namespace GamesWithFriends.Core.Entities;

public class Entity
{
    [Required, Key] public Guid Id { get; init; }
    [Required] public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}