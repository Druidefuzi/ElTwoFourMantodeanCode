using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Mantodean.HarmonyPatches
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch]
	public static class Patch_Blackboard
	{
		// See the comment in Patch_DeskSpot.Prepare. The Biotech check belongs here, not inside
		// the transpiler: an empty transpiler does not disable the patch, it replaces the target
		// method with nothing. Without this, LearningUtility.ConnectedBlackboards was rebuilt with
		// an empty body on every Biotech-less start.
		public static bool Prepare()
		{
			return ModsConfig.BiotechActive;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000214B File Offset: 0x0000034B
		[HarmonyTargetMethods]
		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(LearningUtility), "ConnectedBlackboards");
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002154 File Offset: 0x00000354
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			int popped = 0;
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Ldfld || popped > 0 && popped < 3)
				{
					popped++;
				}
				else
				{
					yield return instruction;
					//todo check if needed instruction = null;
				}
			}
		}
	}
}
