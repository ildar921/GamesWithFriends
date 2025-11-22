// ReSharper disable CollectionNeverUpdated.Global

using System.Diagnostics.CodeAnalysis;
using GamesWithFriends.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace GamesWithFriends.Core.Options;

public class CookiePresetsOptions
{
    public Dictionary<CookiePresetType, CookieOptions> Presets { get; init; } = new();

    public bool TryGetValue(CookiePresetType type, [MaybeNullWhen(false)] out CookieOptions value)
    {
        var result = Presets.TryGetValue(type, out var options);

        value = options;
        
        return result;
    }
}