using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Wish;

namespace RemoteEarthquakeAndRainCloud
{
    [HarmonyPatch(typeof(Tool))]
    public static class ToolPatch
    {
        [HarmonyPatch("Use1"), HarmonyPrefix]
        public static bool Use1_Prefix(Tool __instance, Vector2Int ___pos)
        {
            if (__instance is Hoe &&
                Plugin.modEnabled.Value &&
                Plugin.remoteKey.Value.IsPressed() && 
                Plugin.earthqueakeSpell != null)
            {
                if (GameSave.Farming.GetNodeAmount("Farming5a", 3, true) > 0)
                {
                    Plugin.earthqueakePos = ___pos;
                    Plugin.earthqueakeSpell.UseDown1();
                }
                return false;
            }
            return true;
        }
    }
}
