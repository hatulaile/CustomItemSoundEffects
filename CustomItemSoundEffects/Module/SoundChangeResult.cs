using CustomItemSoundEffects.Enums;
using CustomItemSoundEffects.Extensions;
using FMOD;

namespace CustomItemSoundEffects.Module;

public readonly struct SoundChangeResult
{
    public Result ActionSoundResult { get; }

    public bool ActionSoundSuccess => ActionSoundResult is Result.Ok;

    public Result UseSoundResult { get; }

    public bool UseSoundSuccess => UseSoundResult is Result.Ok;

    public bool AllSuccess => ActionSoundSuccess && UseSoundSuccess;

    public bool AnySuccess => ActionSoundSuccess || UseSoundSuccess;

    public SoundChangeResult() :
        this(RESULT.OK)
    {
    }

    public SoundChangeResult(RESULT actionSoundResult, RESULT useSoundResult) :
        this(actionSoundResult.ToResult(), useSoundResult.ToResult())
    {
    }

    public SoundChangeResult(Result actionSoundResult, Result useSoundResult)
    {
        ActionSoundResult = actionSoundResult;
        UseSoundResult = useSoundResult;
    }

    public SoundChangeResult(RESULT result) :
        this(result.ToResult())
    {
    }

    public SoundChangeResult(Result result)
    {
        ActionSoundResult = result;
        UseSoundResult = result;
    }
}