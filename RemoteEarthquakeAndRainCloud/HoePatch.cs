using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Wish;

namespace RemoteEarthquakeAndRainCloud
{
    [HarmonyPatch(typeof(Hoe))]
    public static class HoePatch
    {
        [HarmonyPatch("LateUpdate"), HarmonyReversePatch]
        public static void MyLateUpdate(object instance)
        {
            IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);

                for (int i = 0; i+2 < code.Count; i++)
                {
                    if (code[i].opcode == OpCodes.Callvirt && 
                        code[i].OperandIs(typeof(Player).GetMethod("get_ExactGraphicsPosition")) && 
                        code[i+2].OperandIs((Single)1.5))
                    {
                        code[i+2].operand = (Single)1000.0;
                    }
                }

                for (int i = 0; i < code.Count; i++)
                {
                    if (code[i].opcode == OpCodes.Callvirt &&
                        code[i].OperandIs(typeof(GameManager).GetMethod(nameof(GameManager.IsFarmableDataTile), new[] { typeof(Vector2Int) })))
                    {
                        code[i].operand = typeof(HoePatch).GetMethod(nameof(MyIsFarmableDataTile));
                    }
                }

                return code;
            }
            _ = Transpiler(null);
        }
        public static bool MyIsFarmableDataTile(object instance, Vector2Int pos)
        {
            return true;
        }

        [HarmonyPatch("LateUpdate"), HarmonyPrefix]
        public static bool LateUpdate_Prefix(Hoe __instance)
        {
            if (Plugin.modEnabled.Value && 
                Plugin.remoteKey.Value.IsPressed())
            {
                MyLateUpdate(__instance);
                return false;
            }
            return true;
        }

    }
}
