using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using CommunityToolkit.HighPerformance.Buffers;

namespace FF16Tools.Pack;

/// <summary>
/// General pack manager (Disposable object).
/// </summary>
public class FF16PackManager : IDisposable, IAsyncDisposable
{
    private Dictionary<string, FF16Pack> _packFiles { get; set; } = [];
    public IReadOnlyDictionary<string, FF16Pack> PackFiles => _packFiles;

    private ILogger _logger;

    public FF16PackManager(ILoggerFactory loggerFactory = null)
    {
        if (loggerFactory is not null)
            _logger = loggerFactory.CreateLogger(GetType().ToString());
    }

    /// <summary>
    /// Opens a directory containing pack files.
    /// </summary>
    /// <param name="directory">Directory containing pack files. Normally 'data' folder.</param>
    public void Open(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory, nameof(directory));

        foreach (var file in Directory.GetFiles(directory, "*.pac", SearchOption.TopDirectoryOnly))
        {
            try
            {
                FF16Pack pack = FF16Pack.Open(file);
                _packFiles.Add(Path.GetFileNameWithoutExtension(file), pack);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to open pack {file}", file);
            }
        }

        _logger?.LogInformation("Loaded {count} pack(s).", _packFiles.Count);
    }

    /// <summary>
    /// Returns a pack file instance.
    /// </summary>
    /// <param name="packName">Pack name (without .pac extension).</param>
    /// <returns>null if not found.</returns>
    public FF16Pack GetPack(string packName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packName, nameof(packName));

        if (_packFiles.TryGetValue(packName, out FF16Pack pack))
            return pack;

        return null;
    }

    /// <summary>
    /// Gets a file information (globally, from all packs).
    /// </summary>
    /// <param name="gamePath"></param>
    /// <param name="includeDiff">Whether to try fetching from diff packs (which override base packs)</param>
    /// <returns>null if not found.</returns>
    public FF16PackFile GetFileInfo(string gamePath, bool includeDiff = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath, nameof(gamePath));

        gamePath = FF16PackPathUtil.NormalizePath(gamePath);

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
    /// Gets a file information from the specified pack.
    /// </summary>
    /// <param name="gamePath">Game path.</param>
    /// <param name="packName">Pack name (without .pac extension). Throws if does not exist.</param>
    /// <returns>null if not found.</returns>
    /// <exception cref="KeyNotFoundException">Pack file does not exist.</exception>
    public FF16PackFile GetFileInfoFromPack(string gamePath, string packName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath, nameof(gamePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(packName, nameof(packName));

        gamePath = FF16PackPathUtil.NormalizePath(gamePath);

        if (!_packFiles.TryGetValue(packName, out FF16Pack packFile))
            throw new KeyNotFoundException($"Pack '{packName}' was not found in pack manager.");

        return packFile.GetFileInfo(gamePath);
    }

    /// <summary>
    /// Gets a file's data.
    /// </summary>
    /// <param name="gamePath">Game path.</param>
    /// <param name="includeDiff">Whether to try fetching from diff packs (which override base packs)</param>
    /// <returns>Disposable MemoryOwner.</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task<MemoryOwner<byte>> GetFileDataAsync(string gamePath, bool includeDiff = true, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath, nameof(gamePath));

        gamePath = FF16PackPathUtil.NormalizePath(gamePath);

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
                            return await diffPack.GetFileDataAsync(gamePath, ct: ct);
                    }
                }

                return await packFile.Value.GetFileDataAsync(gamePath, ct: ct);
            }
        }

        throw new FileNotFoundException($"File '{gamePath}' was not found in any registed packs.");
    }

    /// <summary>
    /// Gets a file's data into the specified output stream.
    /// </summary>
    /// <param name="gamePath">Game path.</param>
    /// <param name="outputStream">Stream to place the data in. It should be writable.</param>
    /// <param name="includeDiff">Whether to include files from 'diff' packs.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task GetFileDataAsync(string gamePath, Stream outputStream, bool includeDiff = true, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath, nameof(gamePath));
        ArgumentNullException.ThrowIfNull(outputStream, nameof(outputStream));

        gamePath = FF16PackPathUtil.NormalizePath(gamePath);

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
                            await diffPack.GetFileDataStreamAsync(gamePath, outputStream, ct: ct);
                            return;
                        }
                    }
                }

                await packFile.Value.GetFileDataStreamAsync(gamePath, outputStream, ct: ct);
                return;
            }
        }

        throw new FileNotFoundException($"File '{gamePath}' was not found in any registed packs.");
    }

    /// <summary>
    /// Gets a file's data from the specified pack.
    /// </summary>
    /// <param name="gamePath">Game path.</param>
    /// <param name="packName">Pack name (without .pac extension)</param>
    /// <param name="includeDiff">Whether to try fetching from diff packs (which override base packs)</param>
    /// <returns>Disposable MemoryOwner.</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task<MemoryOwner<byte>> GetFileDataFromPackAsync(string gamePath, string packName, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath, nameof(gamePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(packName, nameof(packName));

        if (!_packFiles.TryGetValue(packName, out FF16Pack packFile))
            throw new KeyNotFoundException($"Pack '{packName}' was not found in pack manager.");

        gamePath = FF16PackPathUtil.NormalizePath(gamePath);

        if (packFile.FileExists(gamePath))
            return await packFile.GetFileDataAsync(gamePath, ct: ct);
        
        throw new FileNotFoundException($"File '{gamePath}' was not found in pack '{packName}'.");
    }

    /// <summary>
    /// Gets a file's data from a specific pack into the specified output stream.
    /// </summary>
    /// <param name="gamePath">Game path. Throws if not found.</param>
    /// <param name="packName">Pack name (without .pac extension). Throws if not found.</param>
    /// <param name="outputStream">Output stream. It should be writable.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task GetFileDataFromPackAsync(string gamePath, string packName, Stream outputStream, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath, nameof(gamePath));
        ArgumentNullException.ThrowIfNull(outputStream, nameof(outputStream));

        gamePath = FF16PackPathUtil.NormalizePath(gamePath);

        if (!_packFiles.TryGetValue(packName, out FF16Pack packFile))
            throw new KeyNotFoundException($"Pack '{packName}' was not found in pack manager.");

        if (packFile.FileExists(gamePath))
        {
            await packFile.GetFileDataStreamAsync(gamePath, outputStream, ct: ct);
            return;
        }

        throw new FileNotFoundException($"File '{gamePath}' was not found in pack '{packName}'.");
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
