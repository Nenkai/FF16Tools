using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.HighPerformance.Buffers;

namespace FF16Tools.Pack;

/// <summary>
/// General pack manager (Disposable object).
/// </summary>
public class FF16PackManager : IDisposable, IAsyncDisposable
{
    private Dictionary<string, FF16Pack> _packFiles { get; set; } = [];
    public IReadOnlyDictionary<string, FF16Pack> PackFiles => _packFiles;

    /// <summary>
    /// Opens a directory containing pack files.
    /// </summary>
    /// <param name="directory"></param>
    public static FF16PackManager Open(string directory)
    {
        var packManager = new FF16PackManager();

        foreach (var file in Directory.GetFiles(directory, "*.pac", SearchOption.TopDirectoryOnly))
        {
            FF16Pack pack = FF16Pack.Open(file);
            packManager._packFiles.Add(Path.GetFileNameWithoutExtension(file), pack);
        }

        return packManager;
    }

    /// <summary>
    /// Returns a pack file instance.
    /// </summary>
    /// <param name="packName"></param>
    /// <returns>null if not found.</returns>
    public FF16Pack GetPack(string packName)
    {
        if (_packFiles.TryGetValue(packName, out FF16Pack pack))
            return pack;

        return null;
    }

    /// <summary>
    /// Gets a file information.
    /// </summary>
    /// <param name="gamePath"></param>
    /// <param name="includeDiff">Whether to try fetching from diff packs (which override base packs)</param>
    /// <returns>null if not found.</returns>
    public FF16PackFile GetFileInfo(string gamePath, bool includeDiff = true)
    {
        foreach (KeyValuePair<string, FF16Pack> packFile in _packFiles)
        {
            if (packFile.Value.FileExists(gamePath))
            {
                if (includeDiff)
                {
                    string diffFile = packFile.Key + ".diff";
                    if (_packFiles.TryGetValue(diffFile, out FF16Pack diffPack))
                    {
                        if (diffPack.FileExists(gamePath))
                            return diffPack.GetFileInfo(gamePath);
                    }
                }

                return packFile.Value.GetFileInfo(gamePath);
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a file's data.
    /// </summary>
    /// <param name="gamePath"></param>
    /// <param name="includeDiff">Whether to try fetching from diff packs (which override base packs)</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task<MemoryOwner<byte>> GetFileData(string gamePath, bool includeDiff = true, CancellationToken ct = default)
    {
        foreach (KeyValuePair<string, FF16Pack> packFile in _packFiles)
        {
            if (packFile.Value.FileExists(gamePath))
            {
                if (includeDiff)
                {
                    string diffFile = packFile.Key + ".diff";
                    if (_packFiles.TryGetValue(diffFile, out FF16Pack diffPack))
                    {
                        if (diffPack.FileExists(gamePath))
                            return await diffPack.GetFileData(gamePath, ct: ct);
                    }
                }

                return await packFile.Value.GetFileData(gamePath, ct: ct);
            }
        }

        throw new FileNotFoundException("File was not found in any registed packs.");
    }

    /// <summary>
    /// Gets a file's data into the specified output stream.
    /// </summary>
    /// <param name="gamePath"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task GetFileData(string gamePath, Stream outputStream, bool includeDiff = true, CancellationToken ct = default)
    {
        foreach (KeyValuePair<string, FF16Pack> packFile in _packFiles)
        {
            if (packFile.Value.FileExists(gamePath))
            {
                if (includeDiff)
                {
                    string diffFile = packFile.Key + ".diff";
                    if (_packFiles.TryGetValue(diffFile, out FF16Pack diffPack))
                    {
                        if (diffPack.FileExists(gamePath))
                        {
                            await diffPack.GetFileDataStream(gamePath, outputStream, ct: ct);
                            return;
                        }
                    }
                }

                await packFile.Value.GetFileDataStream(gamePath, outputStream, ct: ct);
                return;
            }
        }

        throw new FileNotFoundException("File was not found in any registed packs.");
    }

    public void Dispose()
    {
        foreach (var pack in _packFiles)
            pack.Value.Dispose();

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var pack in _packFiles)
            await pack.Value.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
