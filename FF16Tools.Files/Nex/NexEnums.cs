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

public enum NexUnionType : ushort
{
    // Avoid renaming them when possible.
    // Names are written into exported sqlite databases.
    action = 3,
    result = 15, // Not sure, only telemetryarray uses this, but DLCFlags seem to match
    attackparam = 17,
    bnpcbase = 23,
    directorbankitem = 25,
    eid = 27,
    //unk_41 = 41,
    command = 42,
    defaulttalk = 46,

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
    directortype = 48,

    enpcbase = 50,
    layoutnamedinstance = 55,
    attackparam_atktype = 58, // customaction (?)
    quest = 79,
    questsequence = 82, // (not an id to any table?)
    itemshopbase = 99,
    smithshopbase = 100,
    charatimelinevariation = 105,
    shopbase = 107,

    // not an actual table. maps to EquipmentData in the save file.
    equipment_index = 112,
    sidequestbattledirector = 114,
    item = 124,
    levelcutscene = 127,
    partytalk = 131,
    droptable = 135,
    behavioreventactionset = 138,
    buff = 143,
    bgmmode = 146,
    placename = 147,
    equipitem = 177,
    cutsceneset = 192,
    transition = 198,
    questprogress = 204,
    summonmode = 208,
    partyselect = 255,
    partymember = 256,
    scenariocutscene = 260,
    icon_file_id = 266, // Not sure
    worldmapanchor = 273,
    usersituation = 277,
    levelevent = 282,
    battleevent = 312,
    bgmselect = 314,
    moviedata = 316,
    gamemap = 317,
    //unk_330 = 330,
    missionbattledirector = 359,
    uicolor = 363,
    normalcameraparam2 = 344, // Same as 366 apparently, see ffxvi.exe 0F 84 ? ? ? ? 81 E9 ? ? ? ? 0F 84 ? ? ? ? 83 E9 ? 0F 84 ? ? ? ? 81 F9
    normalcameraparam = 366,
    astralprojection = 373,
    mapdirectorsequence = 375,
    directorfaketargetsettings = 382,
    directoractorlist = 399,
    behaviormovesequence = 403, // behaviormovesequence (or behaviormoveset)
    behaviorwanderingparam = 405,
    //unk_428 = 428,
    battletag = 454,
    behaviordialogueactionset = 455,
    usersituationflag = 458,
    shopchronicle = 484,
    layoutgroup = 486, // groups from map
    letterlist = 487,
    shoppastsight = 488,
    //unk_491 = 491,
    battleblockdirector = 494,
    caption = 496,
    stageshopbase = 502,
    collectionlist = 517,
    shoplore = 523,
    battletalk = 524,
    howto = 528,
    tutorialdirector = 530,
    behaviorwaitparam = 539,
    vacuumedmoveparam = 541,
    tutorial = 545,
    directorcondition = 557,
    directoractormonitor = 568,

    // mapdirectorflag (table) seems unused.
    // This seems to be used as a persistent map 'custom' flag state
    // i.e enable flag 1, check on it, disable it, etc. see: questsequence
    // up to 64
    mapdirectorflag = 577,

    behaviorlookatactparam = 628,
    skill = 632,
    behavioreventactionsequence = 639,
    difficultylevel = 649,
    behaviormoverailparam = 653,
    directoreventtasklist = 655, // mapdirectorsequence
    captionfreeword = 664,
    directorforwardmoveparam = 665,
    questcharalayoutbnpc = 668,
    smobdirector = 692,
    systemassisttimertalkitem = 700,
    // 702 = related to difficulty? ffxvi.exe steam 1.0.1 -> sub_14065170C
    layoutenpcinstance = 706,
    shopmythril = 719,
    behaviorlinkmovetarget = 722,
    behaviorguidanceparam = 739,
    telemetryobjectset = 742,
    telemetrypropertyvalue = 758,
    letter = 791,
    telemetryobject = 793,
    shopfixedpaletteexit = 818,
    cutsceneconnect = 831,
    cutsceneconnectcamerapreset = 837,
    questcutscene = 841,
    mainpartytalksequence = 843,
    questpartytalksequence = 844,
    maindefaulttalksequence = 845,
    questdefaulttalksequence = 846,
    questsimpleventsequence = 847,
    mainsimpleeventsequence = 848,
    behavioreventidlestateparam = 853,
    behavioreventdirector = 854,
    simpleevent = 856,
    synopsis = 861,
    simpleeventmarkerpoint = 884,
    orchestrionlist = 889,
    orchestrion = 890,
    simpleeventlightpreset = 917,
    phoenixshiftmove = 923,
    questcharalayoutenpc = 932,
    directorshipswingparameter = 934,
    directormovecustomspeedparam = 935,
    simpleeventselect = 942,
    simpleeventsequencerandomset = 943,
    cutsceneconnectquestseqarg = 945,
    shopquestcounter = 957,
    shopfixedpaletteaccess = 976,
    shopfixedpalettewarp = 977,
    fixedpalette = 978,
    questdiscardlist = 985,
    icondiscovery = 989,
    shopfamevalue = 998,
    //unk_1011 = 1011,
    loresynopsysreference = 1012,
    envvoice = 1027,
    //unk_1044 = 1044,
    abysseffect = 1047,
    battlescoreattackcategory = 1049,
    lorecutmreference = 1079,
    lorecutqreference = 1080,
    // set from 48 89 5C 24 ? 48 89 6C 24 ? 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 84 24 ? ? ? ? 48 8B 51
    fieldmapdiscoverymask = 1094,
    fieldmapobelisk = 1138,
    dlcentitlement = 1144,
    loredictionaryenemycategory = 1162,
    patchdlcversion = 1174,
    simpleeventlightpresetselect = 1186,
    abyssboostparam = 1249,
    //unk1255 = 1255,
}