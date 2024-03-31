using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Wish;

namespace RemoteEarthquakeAndRainCloud
{
    [HarmonyPatch(typeof(WateringCan))]
    public static class WateringCanPatch
    {
        [HarmonyPatch("LateUpdate"), HarmonyReversePatch]
        public static void MyLateUpdate(object instance)
        {
            IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);

                for (int i = 0; i + 2 < code.Count; i++)
                {
                    if (code[i].opcode == OpCodes.Callvirt &&
                        code[i].OperandIs(typeof(Player).GetMethod("get_ExactPosition")) &&
                        code[i + 2].OperandIs((Single)1.5))
                    {
                        code[i + 2].operand = (Single)1000.0;
                    }
                }

                for (int i = 0; i < code.Count; i++)
                {
                    if (code[i].opcode == OpCodes.Callvirt &&
                        code[i].OperandIs(typeof(TileManager).GetMethod(nameof(TileManager.IsWaterable), new[] { typeof(Vector2Int) })))
                    {
                        code[i].operand = typeof(WateringCanPatch).GetMethod(nameof(MyIsWaterable));
                    }
                }

                return code;
            }
            _ = Transpiler(null);
        }
        public static bool MyIsWaterable(object instance, Vector2Int pos)
        {
            return true;
        }
  
        [HarmonyPatch("LateUpdate"), HarmonyPrefix]
        public static bool LateUpdate_Prefix(WateringCan __instance)
        {
            if (Plugin.modEnabled.Value &&
                Plugin.remoteKey.Value.IsPressed())
            {
                MyLateUpdate(__instance);
                return false;
            }
            return true;
        }

        [HarmonyPatch("Use1"), HarmonyPrefix]
        public static bool Use1_Prefix(Vector2Int ___pos)
        {
            if (Plugin.modEnabled.Value &&
                Plugin.remoteKey.Value.IsPressed() &&
                Plugin.cloudSpell != null)
            {
                if (GameSave.Farming.GetNodeAmount("Farming7a", 3, true) > 0)
                { 
                    Plugin.cloudPos = ___pos;
                    Plugin.cloudSpell.UseDown1();
                }
                return false;
            }
            return true;
        }
    }
}