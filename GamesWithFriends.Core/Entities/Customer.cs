using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GamesWithFriends.Core.Enums;

namespace GamesWithFriends.Core.Entities;

public sealed class Customer : Entity
{
    [Required, MinLength(3), MaxLength(32)]
    public string Username { get; set; } = null!;

    [Required] public string PasswordHash { get; set; } = null!;

    [Required, DefaultValue(0)] public decimal Balance { get; set; }

    [Required, DefaultValue(0)] public RoleType Role { get; set; }

    [Required] public ICollection<Notification> Notifications { get; set; } = new LinkedList<Notification>();
}