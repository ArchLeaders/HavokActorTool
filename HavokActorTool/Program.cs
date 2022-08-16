using HavokActorTool;

bool nx = false;
bool formatName = false;
bool newModel = false;
string? baseActor = null;

foreach (var arg in args) {
    if (baseActor == "!pending") {
        baseActor = arg;
    }
    else if (arg == "-s") {
        nx = true;
    }
    else if (arg == "-f") {
        formatName = true;
    }
    else if (arg == "-n") {
        newModel = true;
    }
    else if (arg == "-b") {
        baseActor = "!pending";
    }
}

try {
    HavokActor havokActor = new(Directory.GetCurrentDirectory(), nx ? "01007EF00011E000\\romfs" : "content", args[0], formatName, newModel, baseActor);
    await havokActor.Construct();
}
catch (Exception ex) {
    Console.WriteLine(ex);
}