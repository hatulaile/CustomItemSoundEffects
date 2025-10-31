using System;
using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Channel = FMOD.Channel;
using Debug = UnityEngine.Debug;

namespace CustomItemSoundEffects.Features;

public class ItemSoundEffects
{
    private const string BUS_PATH = "bus:/Master/SFX";
    private const float FADE_TIME = 0.25f;

    public ItemSoundEffectsEntry Entry { get; }

    public Sound Action => Entry.Action;

    //这里或许可以改成 Config
    public bool UseDefaultActionSound => !Action.hasHandle();

    public Sound Use => Entry.Use;

    //这里或许可以改成 Config
    public bool UseDefaultUseSound => !Use.hasHandle();

    private Channel _currentChannel;

    internal ItemSoundEffects(ItemSoundEffectsEntry entry)
    {
        Entry = entry;
    }

    public void PlayAction()
    {
        if (!Action.hasHandle()) return;
        RESULT result = RuntimeManager.CoreSystem.playSound(Action,
            TryGetChannelGroup(out ChannelGroup group) ? group : default,
            false, out _currentChannel);
        if (result is not RESULT.OK)
        {
            Debug.LogError("播放物品 Action 音效时发生错误: " + result);
        }
    }

    public void PlayUse()
    {
        if (!Use.hasHandle()) return;
        RESULT result = RuntimeManager.CoreSystem.playSound(Use,
            TryGetChannelGroup(out ChannelGroup group) ? group : default,
            false, out _);
        if (result is not RESULT.OK)
        {
            Debug.LogError("播放物品 Use 音效时发生错误: " + result);
        }
    }

    public void Stop()
    {
        if (!_currentChannel.hasHandle()) return;
        _currentChannel.stop();
    }

    public void FadeStop()
    {
        _ = FadeOutAsync(FADE_TIME);
    }

    private async UniTask FadeOutAsync(float fadeTime = 1f)
    {
        if (!_currentChannel.hasHandle()) return;
        var currentChannel = _currentChannel;
        _currentChannel = default;
        if (currentChannel.getVolume(out float defaultVolume) is not RESULT.OK) return;
        while (currentChannel.hasHandle() &&
               currentChannel.getVolume(out float volume) is RESULT.OK && volume > 0.02f)
        {
            await UniTask.NextFrame();
            if (currentChannel.setVolume(volume - (defaultVolume * Time.deltaTime / fadeTime)) is not RESULT.OK)
            {
                break;
            }
        }

        currentChannel.stop();
    }

    private static bool TryGetChannelGroup(out ChannelGroup group)
    {
        try
        {
            Bus bus = RuntimeManager.GetBus(BUS_PATH);
            RESULT result = bus.getChannelGroup(out group);
            if (result is not RESULT.OK)
            {
                Debug.LogError("无法获取音效组!");
                group = default;
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("获取音效组时发生错误: " + e);
            group = default;
            return false;
        }
    }

    public struct ItemSoundEffectsEntry
    {
        internal ItemSoundEffectsEntry(Sound action, Sound use)
        {
            Action = action;
            Use = use;
        }

        public Sound Action { get; }

        public Sound Use { get; }
    }
}