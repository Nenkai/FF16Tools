using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Hashing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Timelines.Chara; 

/// <summary>
/// Used to verify that timelines are written correctly.
/// </summary>
public class TimelineSerializationIntegrityChecker
{
    public static void Check(string charaDir) // Path to chara
    {
        foreach (var sourceFile in Directory.GetFiles(charaDir, "*.tlb", SearchOption.AllDirectories))
        {
            try
            {
                Console.WriteLine("Reading " + sourceFile);

                var filee = CharaTimelineFile.Open(sourceFile);
                filee.Write("built.tlb");

                if (XxHash3.HashToUInt64(File.ReadAllBytes(sourceFile)) != XxHash3.HashToUInt64(File.ReadAllBytes("built.tlb")))
                {
                    File.Copy(sourceFile, "source.tlb", overwrite: true);
                    throw new DataException("Hash mismatch");
                }
                else
                    Console.WriteLine("Hash match.");

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not yet supported"))
                {
                    Console.WriteLine($"Skipping, not supported - {ex.Message}");
                    continue;
                }
            }
        }
    }
}
