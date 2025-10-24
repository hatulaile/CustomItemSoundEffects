using CustomItemSoundEffects.Features;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace CustomItemSoundEffects
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Object? _foodSoundEffectsManager;
        private Harmony? _harmony;

        protected override void OnAfterSetup()
        {
            base.OnAfterSetup();
            _foodSoundEffectsManager = new GameObject("ItemSoundEffectsManager");
            _foodSoundEffectsManager.AddComponent<ItemSoundEffectsManager>();
            _harmony = new Harmony("replace_food_SFX_hatu");
            _harmony.PatchAll();
        }

        protected override void OnBeforeDeactivate()
        {
            base.OnBeforeDeactivate();
            if (_foodSoundEffectsManager is not null)
                Object.Destroy(_foodSoundEffectsManager);
            _harmony?.UnpatchAll();
        }
    }
}