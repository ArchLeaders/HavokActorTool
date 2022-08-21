# HavokActorTool

Simple tool for creating actors with appended collision rigid body files (HKRB).

## Usage:

```
HavokActorTool.exe <path/to/file.hkrb> [-s|--switch] [-f|-formatname] [-n|--newmodel] [-b <ACTORNAME>|-baseactor <ACTORNAME>]
```

- **`-s` | `-switch`**
  
  - Run in Switch mode. <br>
    *Note: Cafe resource (BFRES) parsing currently only works in WiiU mode.*
  
- **`-f` | `-formatname` | `-format`**

  - Format the HKRB filename (which is the resulting actor name) to match the BOTW naming scheme.<br>
    ***e.g.** The file `Example.hkrb` by default creates an actor named `Example`, using `-f` will make an actor named `FldObj_Example_A_01`.*
  
- **`-n` | `-newmodel`**

  - Using this flag sets the new actors ModelList user to a new ModelList named after the actor.<br>
    By default, the base actor model name will be used.
  
- **`-b <ACTORNAME>` | `-baseactor <ACTORNAME>` | `-base <ACTORNAME>`**

  - Assign a base actor to be used when generating the actor. By default, an actor is chosen based on the closest matching HKRB size.<br>
    *Note: Assigning a base actor could make the [instSize](https://zeldamods.org/wiki/ActorInfo.product.sbyml#Actors) unstable, if the game crashes when
    loading your actor try increasing the value by a margin of `1,000-10,000`
    until it works.*

## Thanks:

**GingerAvalanche**: ActorInfo help, BYML library.<br>
**KillzXGaming**: Library help and creation.<br>
**Nivium**: Debugging and early testing.<br>
