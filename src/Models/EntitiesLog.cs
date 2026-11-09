using System;

namespace Zse.Models;

[Flags]
public enum EntitiesLog : uint
{
    None = 0u,
    ManSlender = 1u,
    FriendlySkinwalker = 2u,
    Dexter = 4u,
    Osmis = 8u,
    Giant = 16u,
    NaberMudur = 32u,
    Zoomie = 64u,
    Shark = 128u,
    ScaryFaces = 256u,
    Pirates = 512u,
    LittleRaft = 1024u,
    Gobit = 2048u,
    CRoaches = 4096u,
    Bubbie = 8192u,
    BlindJohn = 16384u,
    BigGuy = 32768u,
    Ness = 65536u,
    AminakuBoss = 131072u,
    AminakuHito = 262144u
}

