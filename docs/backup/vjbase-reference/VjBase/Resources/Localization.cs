using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public static partial class Localization
{
    static void RefreshLanguage()
    {
        var conv = ConVar.GetString("vj_language");

        // Automatically set VJ Base to whatever GMod's language is set to
        // Based on: https://wiki.facepunch.com/gmod/Addon_Localization
        // Skip if VJ Base is set to a language unsupported by Garry's Mod
        if (ConVar.GetInt("vj_language_auto") == 1 && conv != "armenian")
        var gmod_conv = ConVar.GetString("gmod_language");
        var converted = tblGModtoVJ[gmod_conv];
        if (converted)
        RunConsoleCommand("vj_language", converted);
        conv = converted;



        // Obtain the current language's strings
        var langTable = strings_english;
        if (conv == "russian")
        langTable = strings_russian;
        else if (conv == "lithuanian")
        langTable = strings_lithuanian;
        else if (conv == "spanish_lt")
        langTable = strings_spanish_latin;
        else if (conv == "schinese")
        langTable = strings_chinese_simplified;
        else if (conv == "turkish")
        langTable = strings_turkish;


        // First set the English strings in case some aren't overridden by the current language
        for k, v in pairs(strings_english) do
        add(k, v);


        // Set the current language's strings to the game
        for k, v in pairs(langTable) do
        add(k, v);


        // Deprecated strings
        add("vjbase.menugeneral.default", "Default");

        MsgC(VJ.COLOR_LOGO_ORANGE_LIGHT, "VJ Base: ", VJ.COLOR_CLIENT, "Language set to ", VJ.COLOR_GREEN, conv + "\n");
    }

}