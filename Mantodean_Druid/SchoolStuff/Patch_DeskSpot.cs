using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace L24
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch]
    public static class Patch_DeskSpot
    {
        // Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public static bool IsDesk(ThingDef thing)
        {
            return thing == ThingDefOf.SchoolDesk || thing == InternalDefOf.L24_Manto_SchoolDesk;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x0000209D File Offset: 0x0000029D
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(SchoolUtility), "DeskSpotTeacher", null, null);
            yield return AccessTools.Method(typeof(SchoolUtility), "DeskSpotStudent", null, null);
            yield break;
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                // Example: Skip 'Ldfld' instructions
                if (instruction.opcode == OpCodes.Ldfld)
                {
                    // Skip this 'Ldfld' instruction
                    continue;
                }

                // Return the original instruction if not modified
                yield return instruction;
            }
        }
    }
}

    
