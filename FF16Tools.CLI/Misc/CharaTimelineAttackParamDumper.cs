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
        SortedDictionary<int, string> names = [];

        string dir = @"C:\Users\Ron\Desktop\ff116mods\timelines\chara";
        foreach (var file in Directory.GetFiles(dir, "*.tlb", SearchOption.AllDirectories))
        {
            try
            {
                var fcut = new CharaTimelineFile();
                fcut.Read(file);

                foreach (var elem in fcut.Timeline.Elements)
                {
                    if (elem.TimelineElemUnionTypeOrLayerId == ((int)TimelineUnionType.kTimelineElem_9))
                    {
                        var type1009 = elem.DataUnion.ElementData as TimelineElement_9;
                        names.TryAdd(type1009.AttackParamId, $"Used by {file[(dir.Length + 1)..]}");
                    }
                    else if (elem.TimelineElemUnionTypeOrLayerId == ((int)TimelineUnionType.Attack))
                    {
                        var type1002 = elem.DataUnion.ElementData as TimelineElement_1002;
                        if (names.TryGetValue(type1002.AttackParamId, out string n) && n != type1002.Path)
                            ;

                        names.TryAdd(type1002.AttackParamId, type1002.Path);

                        if (type1002.field_0x0C != 0)
                            names.TryAdd(type1002.field_0x0C, type1002.Path + " (2?)");
                    }
                    else if (elem.TimelineElemUnionTypeOrLayerId == ((int)TimelineUnionType.kTimelineElem_1064))
                    {
                        var type1064 = elem.DataUnion.ElementData as TimelineElement_1064;
                        if (names.TryGetValue(type1064.AttackParamId, out string n) && n != type1064.Path)
                            ;

                        names.TryAdd(type1064.AttackParamId, type1064.Path);
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
