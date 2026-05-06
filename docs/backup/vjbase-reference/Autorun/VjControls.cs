using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public static partial class VjControls
{
    public static object AddPlugin = function(name, type, version);
    public static object AddCategoryInfo = function(category, options);
    public static object AddKillIcon = addKillIcon;
    public static object AddNPC = function(name, class, category, adminOnly, customFunc);
    public static object AddNPC_HUMAN = function(name, class, weapons, category, adminOnly, customFunc);
    public static object AddNPCWeapon = function(name, class, category);
    public static object AddWeapon = function(name, class, adminOnly, category, customFunc);
    public static object AddEntity = function(name, class, author, adminOnly, offset, dropToFloor, category, customFunc);
    public static object AddParticle = function(fileName, particleList);
    public static object AddConVar = function(name, defValue, flags, helpText, min, max);
    public static object AddClientConVar = function(name, defValue, helpText, min, max, save);
    public static object AddAddonProperty = VJ.AddPlugin;

}