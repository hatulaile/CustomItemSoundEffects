using System.Diagnostics.CodeAnalysis;
using ItemStatsSystem;

namespace CustomItemSoundEffects.Utils;

public static class ItemUtils
{
    public static bool TryGetItem(string itemText, [NotNullWhen(true)] out Item? item)
    {
        item = null;
        if (!int.TryParse(itemText, out int result))
        {
            result = -1;
        }

        foreach (ItemAssetsCollection.Entry instanceEntry in ItemAssetsCollection.Instance.entries)
        {
            if (instanceEntry.typeID != result && !itemText.Equals(instanceEntry.prefab.DisplayNameRaw) &&
                !itemText.Equals(instanceEntry.prefab.DisplayName)) continue;
            item = instanceEntry.prefab;
            return true;
        }

        return false;
    }
}