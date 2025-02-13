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
    attackparam = 17,
    bnpcbase = 23,
    directorbankitem = 25,
    //unk_41 = 41,
    command = 42,
    defaulttalk = 46,
    enpcbase = 50,
    layoutnamedinstance = 55,
    attackparam_atktype = 58, // customaction (?)
    quest = 79,
    questsequence = 82, // (not an id to any table?)
    itemshopbase = 99,
    smithshopbase = 100,
    charatimelinevariation = 105,
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
    normalcameraparam = 366,
    astralprojection = 373,
    mapdirectorsequence = 375,
    directorfaketargetsettings = 382,
    directoractorlist = 399,
    behaviormovesequence = 403, // behaviormovesequence (or behaviormoveset)
    //unk_428 = 428,
    battletag = 454,
    usersituationflag = 458,
    shopchronicle = 484,
    //unk_486 = 486,
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
    tutorial = 545,
    directorcondition = 557,
    directoractormonitor = 568,

    // mapdirectorflag (table) seems unused.
    // This seems to be used as a persistent map 'custom' flag state
    // i.e enable flag 1, check on it, disable it, etc. see: questsequence
    // up to 64
    mapdirectorflag = 577,

    behavioreventactionsequence = 639,
    //unk_649 = 649,
    behaviormoverailparam = 653,
    directoreventtasklist = 655, // mapdirectorsequence
    captionfreeword = 664,
    directorforwardmoveparam = 665,
    questcharalayoutbnpc = 668,
    systemassisttimertalkitem = 700,
    // 702 = related to difficulty? ffxvi.exe steam 1.0.1 -> sub_14065170C
    layoutenpcinstance = 706,
    shopmythril = 719,
    behaviorlinkmovetarget = 722,
    telemetryobject = 793,
    shopfixedpaletteexit = 818,
    cutsceneconnect = 831,
    questcutscene = 841,
    mainpartytalksequence = 843,
    questpartytalksequence = 844,
    maindefaulttalksequence = 845,
    questdefaulttalksequence = 846,
    questsimpleventsequence = 847,
    mainsimpleeventsequence = 848,
    behavioreventdirector = 854,
    simpleevent = 856,
    synopsis = 861,
    simpleeventmarkerpoint = 884,
    orchestrionlist = 889,
    orchestrion = 890,
    simpleeventlightpreset = 917,
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
    lorecutmreference = 1079,
    lorecutqreference = 1080,
    fieldmapobelisk = 1138,
    dlcentitlement = 1144,
    patchdlcversion = 1174,
    simpleeventlightpresetselect = 1186,
    abyssboostparam = 1249,
    //unk1255 = 1255,
}