using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using CustomItemSoundEffects.Enums;
using CustomItemSoundEffects.Extensions;
using CustomItemSoundEffects.Module;
using CustomItemSoundEffects.Utils;
using FMOD;
using FMODUnity;
using ItemStatsSystem;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CustomItemSoundEffects.Features;

public class ItemSoundEffectsManager : MonoBehaviour
{
    private const MODE SOUND_MODE = MODE.LOOP_OFF | MODE.CREATESTREAM;
    private const string ACTION_FILE_NAME = "action";
    private const string USE_FILE_NAME = "use";

    public static ItemSoundEffectsManager? Instance { get; private set; }

    private readonly Dictionary<int, ItemSoundEffects> _itemSoundEffects = new(64);
    public IReadOnlyDictionary<int, ItemSoundEffects> ItemSoundEffects => _itemSoundEffects;
    public IReadOnlyCollection<ItemSoundEffects> AllItemSoundEffects => _itemSoundEffects.Values;

    #region Add

    public SoundChangeResult TryAddItemSoundEffects(string itemText, string path)
    {
        return !ItemUtils.TryGetItem(itemText, out Item? item)
            ? new SoundChangeResult(Result.ErrUnknownItem)
            : TryAddItemSoundEffects(item, path);
    }

    public SoundChangeResult TryAddItemSoundEffects(Item item, string path)
    {
        SoundChangeResult result = CreateDirectorySoundEffectsInternal(path, out ItemSoundEffects effects);
        if (!result.AnySuccess) return result;
        return _itemSoundEffects.TryAdd(item.TypeID, effects) ? result : new SoundChangeResult(Result.ErrUnknown);
    }

    public SoundChangeResult TryAddItemSoundEffects(string itemText, string? actionPath, string? usePath)
    {
        return !ItemUtils.TryGetItem(itemText, out Item? item)
            ? new SoundChangeResult(Result.ErrUnknownItem)
            : TryAddItemSoundEffects(item, actionPath, usePath);
    }

    public SoundChangeResult TryAddItemSoundEffects(Item item, string? actionPath, string? usePath)
    {
        if (_itemSoundEffects.ContainsKey(item.TypeID))
            return new SoundChangeResult(Result.ErrSoundExistence);

        Sound action = default;
        Result actionResult;
        if (!string.IsNullOrEmpty(actionPath))
        {
            actionResult = CreateSound(actionPath!, SOUND_MODE, out action).ToResult();
            if (actionResult is not Result.Ok) action = default;
        }
        else
        {
            actionResult = Result.Empty;
        }

        Sound use = default;
        Result useResult;
        if (!string.IsNullOrEmpty(usePath))
        {
            useResult = CreateSound(usePath!, SOUND_MODE, out use).ToResult();
            if (useResult is not Result.Ok) action = default;
        }
        else
        {
            useResult = Result.Empty;
        }

        var result = new SoundChangeResult(actionResult, useResult);
        if (!result.AnySuccess)
            return result;

        var itemSoundEffects = new ItemSoundEffects(new ItemSoundEffects.ItemSoundEffectsEntry(action, use));
        if (!_itemSoundEffects.TryAdd(item.TypeID, itemSoundEffects))
            return new SoundChangeResult(Result.ErrUnknown);

        return result;
    }

    public SoundChangeResult TryAddItemSoundEffects(string itemText, Sound actionPath, Sound use)
    {
        return !ItemUtils.TryGetItem(itemText, out Item? item)
            ? new SoundChangeResult(Result.ErrUnknownItem)
            : TryAddItemSoundEffects(item, actionPath, use);
    }

    public SoundChangeResult TryAddItemSoundEffects(Item item, Sound action, Sound use)
    {
        if (!action.hasHandle() && !use.hasHandle())
            return new SoundChangeResult(Result.Empty);

        if (_itemSoundEffects.ContainsKey(item.TypeID))
            return new SoundChangeResult(Result.ErrSoundExistence);

        var itemSoundEffects = new ItemSoundEffects(new ItemSoundEffects.ItemSoundEffectsEntry(action, use));
        if (!_itemSoundEffects.TryAdd(item.TypeID, itemSoundEffects))
            return new SoundChangeResult(Result.ErrUnknown);

        return new SoundChangeResult(
            action.hasHandle() ? Result.Ok : Result.Empty, use.hasHandle() ? Result.Ok : Result.Empty);
    }

    #endregion

    #region Remove

    public bool RemoveItemSoundEffects(Item item)
    {
        return _itemSoundEffects.Remove(item.TypeID);
    }

    public bool RemoveItemSoundEffects(string itemText)
    {
        if (!ItemUtils.TryGetItem(itemText, out Item? item))
            return false;

        return _itemSoundEffects.Remove(item.TypeID);
    }

    public bool RemoveItemSoundEffects(int itemTypeID)
    {
        return _itemSoundEffects.Remove(itemTypeID);
    }

    #endregion

    #region Set

    public SoundChangeResult TrySetItemSoundEffects(string itemText, string path)
    {
        return !ItemUtils.TryGetItem(itemText, out Item? item)
            ? new SoundChangeResult(Result.ErrUnknownItem)
            : TrySetItemSoundEffects(item, path);
    }

    public SoundChangeResult TrySetItemSoundEffects(Item item, string path)
    {
        SoundChangeResult result = CreateDirectorySoundEffectsInternal(path, out ItemSoundEffects effects);
        if (!result.AnySuccess) return result;
        _itemSoundEffects[item.TypeID] = effects;
        return result;
    }

    public SoundChangeResult TrySetItemSoundEffects(string itemText, string? actionPath, string? usePath)
    {
        return !ItemUtils.TryGetItem(itemText, out Item? item)
            ? new SoundChangeResult(Result.ErrUnknownItem)
            : TrySetItemSoundEffects(item, actionPath, usePath);
    }

    public SoundChangeResult TrySetItemSoundEffects(Item item, string? actionPath, string? usePath)
    {
        if (_itemSoundEffects.ContainsKey(item.TypeID))
            return new SoundChangeResult(Result.ErrSoundExistence);

        Sound action = default;
        Result actionResult;
        if (!string.IsNullOrEmpty(actionPath))
        {
            actionResult = CreateSound(actionPath!, SOUND_MODE, out action).ToResult();
            if (actionResult is not Result.Ok) action = default;
        }
        else
        {
            actionResult = Result.Empty;
        }

        Sound use = default;
        Result useResult;
        if (!string.IsNullOrEmpty(usePath))
        {
            useResult = CreateSound(usePath!, SOUND_MODE, out use).ToResult();
            if (useResult is not Result.Ok) action = default;
        }
        else
        {
            useResult = Result.Empty;
        }

        var result = new SoundChangeResult(actionResult, useResult);
        if (!result.AnySuccess)
            return result;

        var itemSoundEffects = new ItemSoundEffects(new ItemSoundEffects.ItemSoundEffectsEntry(action, use));
        _itemSoundEffects[item.TypeID] = itemSoundEffects;
        return result;
    }

    public SoundChangeResult TrySetItemSoundEffects(string itemText, Sound action, Sound use)
    {
        return !ItemUtils.TryGetItem(itemText, out Item? item)
            ? new SoundChangeResult(Result.ErrUnknownItem)
            : TrySetItemSoundEffects(item, action, use);
    }

    public SoundChangeResult TrySetItemSoundEffects(Item item, Sound action, Sound use)
    {
        if (!action.hasHandle() && !use.hasHandle())
            return new SoundChangeResult(Result.Empty);

        if (_itemSoundEffects.ContainsKey(item.TypeID))
            return new SoundChangeResult(Result.ErrSoundExistence);

        var itemSoundEffects = new ItemSoundEffects(new ItemSoundEffects.ItemSoundEffectsEntry(action, use));
        _itemSoundEffects[item.TypeID] = itemSoundEffects;

        return new SoundChangeResult(
            action.hasHandle() ? Result.Ok : Result.Empty, use.hasHandle() ? Result.Ok : Result.Empty);
    }

    #endregion

    #region Get

    public bool TryGetItemSoundEffects(Item item, [NotNullWhen(true)] out ItemSoundEffects? itemSoundEffects)
    {
        return _itemSoundEffects.TryGetValue(item.TypeID, out itemSoundEffects);
    }

    public bool TryGetItemSoundEffects(string itemText, [NotNullWhen(true)] out ItemSoundEffects? itemSoundEffects)
    {
        if (!ItemUtils.TryGetItem(itemText, out Item? item))
        {
            itemSoundEffects = null;
            return false;
        }

        return _itemSoundEffects.TryGetValue(item.TypeID, out itemSoundEffects);
    }

    public bool TryGetItemSoundEffects(int itemTypeID, [NotNullWhen(true)] out ItemSoundEffects? itemSoundEffects)
    {
        return _itemSoundEffects.TryGetValue(itemTypeID, out itemSoundEffects);
    }

    #endregion

    #region Internal

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        RegisteredItemSoundEffects();
    }

    private void OnDestroy()
    {
        Instance = null;
        _itemSoundEffects.Clear();
    }

    private void RegisteredItemSoundEffects()
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

                if (_itemSoundEffects.ContainsKey(item.TypeID))
                {
                    Debug.LogWarning($"已存在物品: {itemText} - {item.DisplayNameRaw}");
                    continue;
                }

                SoundChangeResult result = CreateDirectorySoundEffectsInternal(info, out ItemSoundEffects effects);
                _itemSoundEffects.Add(item.TypeID, effects);
                Debug.Log($"添加物品: {itemText} - {item.DisplayName}: " +
                          $"acion [{(result.ActionSoundSuccess ? "成功" : result.ActionSoundResult.ToString())}] " +
                          $"use [{(!result.UseSoundSuccess ? "成功" : result.UseSoundResult.ToString())}] ");
            }
            catch (Exception e)
            {
                Debug.LogError("注册音效时发生错误: " + e);
            }
        }
    }

    private SoundChangeResult CreateDirectorySoundEffectsInternal(string path,
        out ItemSoundEffects itemSoundEffects)
    {
        return CreateDirectorySoundEffectsInternal(new DirectoryInfo(path), out itemSoundEffects);
    }

    private SoundChangeResult CreateDirectorySoundEffectsInternal(DirectoryInfo soundPath,
        out ItemSoundEffects itemSoundEffects)
    {
        Sound action = default;
        Result actionResult;
        if (MusicUtils.TryGetMusic(soundPath, ACTION_FILE_NAME, out FileInfo? actionFile))
        {
            actionResult = CreateSound(actionFile, SOUND_MODE, out action).ToResult();
            if (actionResult is not Result.Ok)
                action = default;
        }
        else
        {
            actionResult = Result.ErrFileNotfound;
        }

        Sound use = default;
        Result useResult;
        if (MusicUtils.TryGetMusic(soundPath, USE_FILE_NAME, out FileInfo? useFile))
        {
            useResult = CreateSound(useFile, SOUND_MODE, out use).ToResult();
            if (useResult is not Result.Ok)
                use = default;
        }
        else
        {
            useResult = Result.ErrFileNotfound;
        }

        itemSoundEffects = new ItemSoundEffects(new ItemSoundEffects.ItemSoundEffectsEntry(action, use));
        return new SoundChangeResult(actionResult, useResult);
    }

    private RESULT CreateSound(FileInfo path, MODE mode, out Sound sound)
    {
        return CreateSound(path.FullName, mode, out sound);
    }

    private RESULT CreateSound(string path, MODE mode, out Sound sound)
    {
        return RuntimeManager.CoreSystem.createSound(path,
            mode, out sound);
    }

    #endregion
}