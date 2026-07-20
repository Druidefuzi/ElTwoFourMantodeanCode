using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Mantodean.HarmonyPatches
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch]
    public static class Patch_DeskSpot
    {
        // Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public static bool IsDesk(ThingDef thing)
        {
            return thing == ThingDefOf.SchoolDesk || thing == MantodeanDefOf.L24_Manto_SchoolDesk;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x0000209D File Offset: 0x0000029D
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(SchoolUtility), "DeskSpotTeacher");
            yield return AccessTools.Method(typeof(SchoolUtility), "DeskSpotStudent");
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (ModsConfig.BiotechActive)
            {
                bool done = false;
                foreach (CodeInstruction instruction in instructions)
                {
                    if (instruction.opcode == OpCodes.Beq_S)
                    {
                        yield return new CodeInstruction(OpCodes.Brtrue, instruction.operand);
                    }
                    else if (instruction.opcode == OpCodes.Ldsfld && !done)
                    {
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_DeskSpot), "IsDesk"));
                        done = true;
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }
    }
}

    
