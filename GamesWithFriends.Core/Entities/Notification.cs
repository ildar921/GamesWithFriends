using System.ComponentModel.DataAnnotations;

namespace GamesWithFriends.Core.Entities;

public sealed class Notification : Entity
{
    [Required] public Customer Receiver { get; set; } = null!;

    [Required, MinLength(1), MaxLength(60)]
    public string Title { get; set; } = null!;

    [Required, MinLength(1), MaxLength(256)]
    public string Text { get; set; } = null!;
}