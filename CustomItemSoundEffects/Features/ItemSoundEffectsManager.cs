using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using CustomItemSoundEffects.Utils;
using FMOD;
using ItemStatsSystem;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CustomItemSoundEffects.Features;

public class ItemSoundEffectsManager : MonoBehaviour
{
   public static ItemSoundEffectsManager? Instance { get; private set; }

    private Dictionary<int, ItemSoundEffects> _foodSoundEffects = new(64);
    public IReadOnlyDictionary<int, ItemSoundEffects> FoodSoundEffects => _foodSoundEffects;
    public IReadOnlyCollection<ItemSoundEffects> AllFoodSoundEffects => _foodSoundEffects.Values;

    public bool TryGetFoodSoundEffects(int itemTypeID, [NotNullWhen(true)] out ItemSoundEffects? itemSoundEffects)
    {
        return _foodSoundEffects.TryGetValue(itemTypeID, out itemSoundEffects);
    }

    //todo: 添加外部 API

    // public ItemSoundEffects CreateFoodSoundEffects(string itemText)
    // {
    //     
    // }

    public bool RemoveFoodSoundEffects(int itemTypeID)
    {
        return _foodSoundEffects.Remove(itemTypeID);
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        RegisteredFoodSoundEffects();
    }

    private void OnDestroy()
    {
        Instance = null;
        _foodSoundEffects.Clear();
    }

    private void RegisteredFoodSoundEffects()
    {
        DirectoryInfo directoryInfo =
            new DirectoryInfo(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("无法获取程序集路径"),
                "SFX"));
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
            Debug.Log($"{directoryInfo.FullName} 不存在, 新建文件夹~");
        }

        foreach (DirectoryInfo info in directoryInfo.GetDirectories())
        {
            try
            {
                string itemText = info.Name;
                if (!ItemUtils.TryGetItem(itemText, out Item? item))
                {
                    Debug.LogWarning($"未找到物品: {itemText}");
                    continue;
                }

                if (_foodSoundEffects.ContainsKey(item.TypeID))
                {
                    Debug.LogWarning($"已存在物品: {itemText} - {item.DisplayNameRaw}");
                    continue;
                }

                ItemSoundEffects itemSoundEffectsInternal = CreateFoodSoundEffectsInternal(item, info);
                _foodSoundEffects.Add(item.TypeID, itemSoundEffectsInternal);
                Debug.Log($"已加载物品: {itemText} - {item.DisplayNameRaw}");
            }
            catch (Exception e)
            {
                Debug.LogError("注册音效时发生错误: " + e);
            }
        }
    }

    private ItemSoundEffects CreateFoodSoundEffectsInternal(Item item, DirectoryInfo soundPath)
    {
        Sound action = default;
        if (MusicUtils.TryGetMusic(soundPath, "action", out FileInfo? actionFile))
        {
            RESULT result = FMODUnity.RuntimeManager.CoreSystem.createSound(actionFile.FullName,
                MODE.LOOP_OFF | MODE.CREATESTREAM, out action);
            if (result is not RESULT.OK)
            {
                Debug.LogError($"创建物品 {item.DisplayNameRaw} action音效: {result}");
            }
        }

        Sound use = default;
        if (MusicUtils.TryGetMusic(soundPath, "use", out FileInfo? useFile))
        {
            RESULT result = FMODUnity.RuntimeManager.CoreSystem.createSound(useFile.FullName,
                MODE.LOOP_OFF | MODE.CREATESTREAM, out use);
            if (result is not RESULT.OK)
            {
                Debug.LogError($"创建物品 {item.DisplayNameRaw} use音效: {result}");
            }
        }

        return new ItemSoundEffects(new ItemSoundEffects.ItemSoundEffectsEntry(action, use));
    }
}