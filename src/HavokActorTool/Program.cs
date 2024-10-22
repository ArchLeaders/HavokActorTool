using HavokActorTool;

bool nx = false;
bool formatName = false;
bool newModel = false;
string? baseActor = null;

if (args.Length == 0) {
    Print("!error||Invalid arguments. Use -h for more information.");
    return;
}

foreach (var _arg in args) {
    var arg = _arg.ToLower();

    if (baseActor == "!pending") {
        baseActor = _arg;
    }
    else if (arg == "-s" || arg == "-switch" || arg == "-nx") {
        nx = true;
    }
    else if (arg == "-f" || arg == "-formatname" || arg == "-format") {
        formatName = true;
    }
    else if (arg == "-n" || arg == "-newmodel") {
        newModel = true;
    }
    else if (arg == "-b" || arg == "-baseactor" || arg == "-base") {
        baseActor = "!pending";
    }
    else if (arg == "-h" || arg == "-help") {
        Help();
    }
}

try {
    await new HavokActor(
        Directory.GetCurrentDirectory(), nx ? "01007EF00011E000\\romfs" : "content", args[0], formatName, newModel, baseActor
    ).Construct();
}
catch (Exception ex) {
    Print("!error||" + ex);
}

static void Help()
{
    Print("||\nUsage:\n\n" +
        "HavokActorTool.exe <path/to/file.hkrb> [-s|--switch] [-f|-formatname] [-n|--newmodel] [-b <ACTORNAME>|-baseactor <ACTORNAME>]\n\n" +
        "See https://github.com/ArchLeaders/HavokActorTool#ReadMe for more info."
    );
    Environment.Exit(0);
}