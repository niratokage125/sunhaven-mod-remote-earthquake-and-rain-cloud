using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Wish;

namespace RemoteEarthquakeAndRainCloud
{
    [HarmonyPatch(typeof(Cloud))]
    public static class CloudPatch
    {
        //SetCloudPath
        [HarmonyPatch(nameof(Cloud.SetCloudPath)), HarmonyPrefix]
        public static bool SetCloudPath_Prefix(Cloud __instance, global::Direction direction, int skillLevel, bool fromLocalPlayer)
        {
            if (fromLocalPlayer && Plugin.cloudSpell.Casting)
            {
                var position = Plugin.cloudPos;
                __instance.transform.position = new Vector3((float)(position.x), (float)(position.y) * 1.4142135f, -6f);
            }
            return true;
        }
    }
}
