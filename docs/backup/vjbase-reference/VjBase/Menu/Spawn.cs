using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public static partial class Spawn
{
    public static object CreateDupe_NPC = function( ply, mdl, class, equipment, spawnflags, data );
    public static object CreateDupe_Entity = function( ply, data );
    public static object CreateDupe_Weapon = function( ply, data );

}