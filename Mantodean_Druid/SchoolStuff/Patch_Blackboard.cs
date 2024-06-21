using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace L24
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch]
	public static class Patch_Blackboard
	{
		// Token: 0x06000007 RID: 7 RVA: 0x0000214B File Offset: 0x0000034B
		[HarmonyTargetMethods]
		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(LearningUtility), "ConnectedBlackboards", null, null);
			yield break;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002154 File Offset: 0x00000354
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			short popped = 0;
			foreach (CodeInstruction instruction in instructions)
			{
				bool flag = instruction.opcode == OpCodes.Ldfld || (popped > 0 && popped < 3);
				if (flag)
				{
					popped += 1;
				}
				else
				{
					yield return instruction;
					//todo check if needed instruction = null;
				}
			}
			IEnumerator<CodeInstruction> enumerator = null;
			yield break;
			yield break;
		}
	}
}
