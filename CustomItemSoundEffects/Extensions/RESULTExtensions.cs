using System.Diagnostics.CodeAnalysis;
using CustomItemSoundEffects.Enums;
using FMOD;

namespace CustomItemSoundEffects.Extensions;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class RESULTExtensions
{
    public static Result ToResult(this RESULT result)
    {
        return (Result)result;
    }
}