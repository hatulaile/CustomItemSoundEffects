using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ItemStatsSystem;
using UnityEngine;

namespace CustomItemSoundEffects.Utils;

public static class UseItemUtils
{
    private static readonly Lazy<FieldInfo> ItemFieldInfoLazy = new(() =>
        typeof(CA_UseItem).GetField("item", BindingFlags.NonPublic | BindingFlags.Instance));

    public static bool TryGetItem(CA_UseItem useItem, [NotNullWhen(true)] out Item? item)
    {
        item = (Item)ItemFieldInfoLazy.Value.GetValue(useItem);
        return item;
    }
}