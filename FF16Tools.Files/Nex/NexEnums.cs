using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Nex;

public enum NexTableType
{
    Unknown = 0,
    SingleKeyed = 1,
    DoubleKeyed = 2,
    TripleKeyed = 3,
}

public enum NexTableCategory
{
    Unknown = 0,
    SingleKeyed = 1,
    SingleKeyed_Localized = 2,
    DoubleKeyed = 3,
    DoubleKeyed_Localized = 4,
    TripleKeyed = 5,
    TripleKeyed_Localized = 6,
}

public class NexUnions
{
    public static string? GetTypeNameForTypeId(string codeName, int unionTypeId)
    {
        if (UnionTypes.TryGetValue(codeName, out Dictionary<int, string> unionTypes))
        {
            if (unionTypes.TryGetValue(unionTypeId, out string typeName))
            {
                return typeName;
            }
        }

        return null;
    }

    // NOTE: Avoid renaming them when possible.
    // Names are written into exported sqlite databases.
    public static readonly Dictionary<string, Dictionary<int, string>> UnionTypes = new()
    {
        ["ffto"] = new()
        {
            [24] = "Speaker",
            [113] = "PlaceName",
            [115] = "CaptionFreeWord",
            [247] = "VoicePatternSituation",
        },

        ["faith"] = new Dictionary<int, string>()
        {
            [3] = "action",
            [15] = "result",
            [17] = "attackparam",
            [23] = "bnpcbase",
            [25] = "directorbankitem",
            [27] = "eid",
            //[41] = "unk_41",
            [42] = "command",
            [46] = "defaulttalk",

            // Not an actual table.
            // Guessed. Engine/executable creates factories for each director, each factory has an id which goes up to 14, matches directorchangebgmparam.
            // mapdirector = 1,
            // tutorialdirector = 2
            // systemassistdirector = 3
            // fieldeventdirector = 4
            // smobdirector = 5
            // sidequestbattledirector = 6
            // missionbattledirector = 7
            // battledirector = 8
            // behavioreventdirector = 9
            // battleblockdirector = 10
            // fixedpalettedirector = 11
            // 12 & 13 are used but unknown
            // abyssgatedirector = 14

            [48] = "directortype", // Not an actual table.
            [50] = "enpcbase",
            [55] = "layoutnamedinstance",
            [58] = "attackparam_atktype", // customaction (?)
            [79] = "quest",
            [82] = "questsequence", // (not an id to any table?)
            [99] = "itemshopbase",
            [100] = "smithshopbase",
            [105] = "charatimelinevariation",
            [107] = "shopbase",

            // not an actual table. maps to EquipmentData in the save file.
            [112] = "equipment_index",
            [114] = "sidequestbattledirector",
            [124] = "item",
            [127] = "levelcutscene",
            [131] = "partytalk",
            [135] = "droptable",
            [138] = "behavioreventactionset",
            [143] = "buff",
            [146] = "bgmmode",
            [147] = "placename",
            [177] = "equipitem",
            [192] = "cutsceneset",
            [198] = "transition",
            [204] = "questprogress",
            [208] = "summonmode",
            [224] = "speaker", // Used by panzer
            [255] = "partyselect",
            [256] = "partymember",
            [260] = "scenariocutscene",
            [266] = "icon_file_id", // Not sure
            [273] = "worldmapanchor",
            [277] = "usersituation",
            [282] = "levelevent",
            [312] = "battleevent",
            [314] = "bgmselect",
            [316] = "moviedata",
            [317] = "gamemap",
            //[330] = "unk_330",
            [359] = "missionbattledirector",
            [363] = "uicolor",
            [344] = "normalcameraparam2", // Same as 366 apparently, see ffxvi.exe 0F 84 ? ? ? ? 81 E9 ? ? ? ? 0F 84 ? ? ? ? 83 E9 ? 0F 84 ? ? ? ? 81 F9
            [366] = "normalcameraparam",
            [373] = "astralprojection",
            [375] = "mapdirectorsequence",
            [382] = "directorfaketargetsettings",
            [399] = "directoractorlist",
            [403] = "behaviormovesequence", // behaviormovesequence (or behaviormoveset)
            [405] = "behaviorwanderingparam",
            //[428] = "unk_428",
            [454] = "battletag",
            [455] = "behaviordialogueactionset",
            [458] = "usersituationflag",
            [484] = "shopchronicle",
            [486] = "layoutgroup", // groups from map
            [487] = "letterlist",
            [488] = "shoppastsight",
            //[491] = "unk_491",
            [494] = "battleblockdirector",
            [496] = "caption",
            [502] = "stageshopbase",
            [517] = "collectionlist",
            [523] = "shoplore",
            [524] = "battletalk",
            [528] = "howto",
            [530] = "tutorialdirector",
            [539] = "behaviorwaitparam",
            [541] = "vacuumedmoveparam",
            [545] = "tutorial",
            [557] = "directorcondition",
            [568] = "directoractormonitor",

            // mapdirectorflag (table) seems unused.
            // This seems to be used as a persistent map 'custom' flag state
            // i.e enable flag 1, check on it, disable it, etc. see: questsequence
            // up to 64
            [577] = "mapdirectorflag",
            [628] = "behaviorlookatactparam",
            [632] = "skill",
            [639] = "behavioreventactionsequence",
            [649] = "difficultylevel",
            [653] = "behaviormoverailparam",
            [655] = "directoreventtasklist", // mapdirectorsequence
            [664] = "captionfreeword",
            [665] = "directorforwardmoveparam",
            [668] = "questcharalayoutbnpc",
            [692] = "smobdirector",
            [700] = "systemassisttimertalkitem",
            // 702 = related to difficulty? ffxvi.exe steam 1.0.1 -> sub_14065170C
            [706] = "layoutenpcinstance",
            [719] = "shopmythril",
            [722] = "behaviorlinkmovetarget",
            [739] = "behaviorguidanceparam",
            [742] = "telemetryobjectset",
            [758] = "telemetrypropertyvalue",
            [791] = "letter",
            [793] = "telemetryobject",
            [818] = "shopfixedpaletteexit",
            [831] = "cutsceneconnect",
            [837] = "cutsceneconnectcamerapreset",
            [841] = "questcutscene",
            [843] = "mainpartytalksequence",
            [844] = "questpartytalksequence",
            [845] = "maindefaulttalksequence",
            [846] = "questdefaulttalksequence",
            [847] = "questsimpleventsequence",
            [848] = "mainsimpleeventsequence",
            [853] = "behavioreventidlestateparam",
            [854] = "behavioreventdirector",
            [856] = "simpleevent",
            [861] = "synopsis",
            [884] = "simpleeventmarkerpoint",
            [889] = "orchestrionlist",
            [890] = "orchestrion",
            [917] = "simpleeventlightpreset",
            [923] = "phoenixshiftmove",
            [932] = "questcharalayoutenpc",
            [934] = "directorshipswingparameter",
            [935] = "directormovecustomspeedparam",
            [942] = "simpleeventselect",
            [943] = "simpleeventsequencerandomset",
            [945] = "cutsceneconnectquestseqarg",
            [957] = "shopquestcounter",
            [976] = "shopfixedpaletteaccess",
            [977] = "shopfixedpalettewarp",
            [978] = "fixedpalette",
            [985] = "questdiscardlist",
            [989] = "icondiscovery",
            [998] = "shopfamevalue",
            //[1011] = "unk_1011",
            [1012] = "loresynopsysreference",
            [1027] = "envvoice",
            //[1044] = "unk_1044",
            [1047] = "abysseffect",
            [1049] = "battlescoreattackcategory",
            [1079] = "lorecutmreference",
            [1080] = "lorecutqreference",
            // set from 48 89 5C 24 ? 48 89 6C 24 ? 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 84 24 ? ? ? ? 48 8B 51
            [1094] = "fieldmapdiscoverymask",
            [1138] = "fieldmapobelisk",
            [1144] = "dlcentitlement",
            [1162] = "loredictionaryenemycategory",
            [1174] = "patchdlcversion",
            [1186] = "simpleeventlightpresetselect",
            [1249] = "abyssboostparam",
            //[1255] = "unk1255",
        }
    };
}