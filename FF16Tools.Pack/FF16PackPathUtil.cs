using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Frozen;

namespace FF16Tools.Pack;

public class FF16PackPathUtil
{
    private static readonly FrozenDictionary<string, string> _knownFolderToPackName = new Dictionary<string, string>()
    {
        { "animation", "0000" },
        { "chara", "0001" },
        { "cut", "0002" },
        // gap
        { "gracommon", "0004" },
        // gap
        { "movie", "0006" },
        { "nxd", "0007" },
        { "shader", "0008" },
        { "sound/battle", "0009" },
        { "sound/chara", "0010" },
        { "sound/cut", "0011" },
        // gap (2)
        { "sound/driverconfig", "0014" },
        { "sound/env", "0015" },
        { "sound/masts", "0016" },
        { "sound/movie", "0017" },
        { "sound/music", "0018" },
        { "sound/physics", "0019" },
        { "sound/sevoice", "0020" },
        { "sound/ui", "0021" },
        { "sound/vfx", "0022" },
        { "sound/vibration", "0023" },
        { "sound/voice", "0024" },
        // gap
        { "stageset", "0026" },
        { "system", "0027" },
        { "ui", "0028" },
        { "vfx", "0029" },

        // Environment
        { "env/basic", "0101" },
        { "env/bgparts", "0102" },
        { "env/grassset", "0103" },
        { "env/shader", "0106" },
        { "env/speedtree", "0107" },
        { "env/splinerope", "0108" },
        { "env/stageset", "0109" },
        { "env/terrain/a", "0112" },
        { "env/terrain/b", "0113" },
        { "env/terrain/c", "0114" },
        { "env/terrain/d", "0115" },
        { "env/terrain/e", "0116" },
        { "env/terrain/t", "0117" },
        { "env/terrain/w", "0118" },

        // Game maps
        { "map/a", "0201" },
        { "map/b", "0202" },
        { "map/c", "0203" },
        { "map/d", "0204" },
        { "map/e", "0205" },
        { "map/t", "0206" },
        { "map/w", "0207" },
        { "map/movie", "0208" },
        { "map/title", "0209" },

        // DLC 2 (Echoes of the Fallen)
        // 2000 - everything else
        { "movie/dlc2", "2001" },
        { "nxd/text/dlc2", "2002" },
        { "sound/voice/dlc2", "2003" },

        // DLC 3 (The Rising Tide)
        // 2000 - everything else
        { "movie/dlc3", "3001" },
        { "nxd/text/dlc3", "3002" },
        { "sound/voice/dlc3", "3003" },
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, string> _knownFolderToPackNameDemo = new Dictionary<string, string>()
    {
        // 0000 is everything else
        { "nxd", "0001" },
        { "sound/voice", "0002" },
        { "movie", "0003" },
    }.ToFrozenDictionary();

    /// <summary>
    /// Folder to pack name mappings (ffxvi_demo.exe).
    /// </summary>
    public static FrozenDictionary<string, string> KnownFolderToPathNameDemo => _knownFolderToPackNameDemo;

    /// <summary>
    /// Folder to pack name mappings (ffxvi.exe).
    /// </summary>
    public static FrozenDictionary<string, string> KnownFolderToPathName => _knownFolderToPackName;

    private static readonly FrozenSet<string> _packLocales = new List<string>() { "ar", "cs", "ct", "de", "en", "es", "fr", "it", "ja", "ko", "ls", "pb", "pl", "ru" }.ToFrozenSet();

    /// <summary>
    /// List of locales ('ar', 'cs', 'en', 'es', etc.)
    /// </summary>
    public static FrozenSet<string> PackLocales => _packLocales;

    /// <summary>
    /// Gets the pack name for the provided path.
    /// </summary>
    /// <param name="gamePath">Game path, example: nxd/ui.nxd</param>
    /// <param name="packName">Returned pack name, example: 0001</param>
    /// <param name="gamePathFolder">Returned game path for said pack, example: nxd</param>
    /// <param name="demo">Whether to use demo paths</param>
    /// <returns>Whether a pack name was found for the provided path. If not, use 0001 instead.</returns>
    public static bool TryGetPackNameForPath(string gamePath, out string packName, out string gamePathFolder, bool demo = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath, nameof(gamePath));

        gamePath = NormalizePath(gamePath);

        if (!demo)
        {
            foreach (var knownFolderToPackName in _knownFolderToPackName)
            {
                if (gamePath.StartsWith(knownFolderToPackName.Key))
                {
                    packName = knownFolderToPackName.Value;
                    gamePathFolder = knownFolderToPackName.Key;
                    return true;
                }
            }

            // 2000.pac contains a bunch of files, mostly goes under */dlc2/ or */dlc3/
            // There's chara files without /dlc2/ or /dlc3/, so that means it'll go under 0001, but that's fine
            if (gamePath.Contains("/dlc2/"))
            {
                packName = "2000";
                gamePathFolder = string.Empty;
                return true;
            }

            if (gamePath.Contains("/dlc3/"))
            {
                packName = "3000";
                gamePathFolder = string.Empty;
                return true;
            }
        }
        else
        {
            foreach (var knownFolderToPackName in _knownFolderToPackNameDemo)
            {
                if (gamePath.StartsWith(knownFolderToPackName.Key))
                {
                    packName = knownFolderToPackName.Value;
                    gamePathFolder = knownFolderToPackName.Key;
                    return true;
                }
            }

            // Everything else fits in 0001 in demo
            packName = "0001";
            gamePathFolder = string.Empty;
            return true;
        }

        packName = null;
        gamePathFolder = null;

        return false;
    }

    /// <summary>
    /// Normalizes a path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string NormalizePath(string path)
    {
        return path.Replace("\\", "/").ToLower();
    }
}
