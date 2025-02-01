using FF16Tools.Files.CharaTimeline.Elements;
using FF16Tools.Files.CharaTimeline;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.CLI.Misc;

public class CharaTimelineAttackParamDumper
{
    public static void Dump()
    {
        SortedDictionary<uint, string> names = [];

        string dir = "<path to chara folder>";
        foreach (var file in Directory.GetFiles(dir, "*.tlb", SearchOption.AllDirectories))
        {
            try
            {
                var fcut = new CharaTimelineFile();
                fcut.Read(file);

                foreach (var elem in fcut.Elements)
                {
                    if (elem.TimelineElemUnionTypeOrLayerId == 9)
                    {
                        var type1009 = elem.Data as TimelineElement9;
                        names.TryAdd(type1009.AttackParamId, $"Used by {file[(dir.Length + 1)..]}");
                    }
                    else if (elem.TimelineElemUnionTypeOrLayerId == 1002)
                    {
                        var type1002 = elem.Data as TimelineElement1002;
                        if (names.TryGetValue(type1002.AttackParamId, out string n) && n != type1002.Name)
                            ;

                        names.TryAdd(type1002.AttackParamId, type1002.Name);

                        if (type1002.Field_0x0C != 0)
                            names.TryAdd(type1002.Field_0x0C, type1002.Name + " (2?)");
                    }
                    else if (elem.TimelineElemUnionTypeOrLayerId == 1064)
                    {
                        var type1064 = elem.Data as TimelineElement1064;
                        if (names.TryGetValue(type1064.AttackParamId, out string n) && n != type1064.Name)
                            ;

                        names.TryAdd(type1064.AttackParamId, type1064.Name);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not process file: {e.Message}");
            }
        }

        foreach (var line in names)
        {
            Console.WriteLine($"set_comment|{line.Key}|0||{line.Value}");
        }
    }
}
