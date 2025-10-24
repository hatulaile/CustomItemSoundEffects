using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace CustomItemSoundEffects.Utils;

public static class MusicUtils
{
    public static readonly string[] SupportedMusicExtensions = [".mp3", ".wav", ".ogg", ".aif", ".aiff", ".flac"];

    public static bool IsMusic(FileInfo file)
    {
        foreach (var supportedMusicExtension in SupportedMusicExtensions)
        {
            if (!file.Extension.Equals(supportedMusicExtension, StringComparison.OrdinalIgnoreCase)) continue;
            return true;
        }

        return false;
    }

    public static bool TryGetMusic(DirectoryInfo directoryInfo, string fileName,
        [NotNullWhen(true)] out FileInfo? fileInfo)
    {
        foreach (FileInfo info in directoryInfo.GetFiles($"{fileName}.*"))
        {
            if (!IsMusic(info)) continue;
            fileInfo = info;
            return true;
        }

        fileInfo = null;
        return false;
    }
}