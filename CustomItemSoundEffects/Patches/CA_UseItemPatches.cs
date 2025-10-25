// ReSharper disable InconsistentNaming

using CustomItemSoundEffects.Features;
using CustomItemSoundEffects.Utils;
using HarmonyLib;
using ItemStatsSystem;

namespace CustomItemSoundEffects.Patches;

[HarmonyPatch(typeof(CA_UseItem))]
internal static class CA_UseItemPatches
{
    [HarmonyPrefix]
    [HarmonyPatch("PostActionSound")]
    public static bool PostActionSoundPrefix(CA_UseItem __instance)
    {
        if (!UseItemUtils.TryGetItem(__instance, out Item? item)) return true;
        if (ItemSoundEffectsManager.Instance is null) return true;
        if (!ItemSoundEffectsManager.Instance.TryGetItemSoundEffects(item.TypeID, out ItemSoundEffects? effects))
            return true;
        if (effects.UseDefaultActionSound) return true;
        effects.PlayAction();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("PostUseSound")]
    public static bool PostUseSoundPrefix(CA_UseItem __instance)
    {
        if (!UseItemUtils.TryGetItem(__instance, out Item? item)) return true;
        if (ItemSoundEffectsManager.Instance is null) return true;
        if (!ItemSoundEffectsManager.Instance.TryGetItemSoundEffects(item.TypeID, out ItemSoundEffects? effects))
            return true;
        if (effects.UseDefaultUseSound) return true;
        effects.PlayUse();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("StopSound")]
    public static void StopSoundPrefix(CA_UseItem __instance)
    {
        if (!UseItemUtils.TryGetItem(__instance, out Item? item)) return;
        if (ItemSoundEffectsManager.Instance is null) return;
        if (ItemSoundEffectsManager.Instance.TryGetItemSoundEffects(item.TypeID,
                out ItemSoundEffects? effects)) effects.FadeStop();
    }
}