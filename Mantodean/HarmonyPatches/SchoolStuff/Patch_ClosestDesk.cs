using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Mantodean.HarmonyPatches
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch]
	public static class Patch_ClosestDesk
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000020B6 File Offset: 0x000002B6
		[HarmonyTargetMethods]
		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(SchoolUtility), "ClosestSchoolDesk");
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020C0 File Offset: 0x000002C0
		[HarmonyPostfix]
		public static void Postfix(ref Thing __result, Pawn child, Pawn teacher)
		{
			if (ModsConfig.BiotechActive)
			{
                bool flag = __result != null || teacher == null;
                if (!flag)
                {
                    __result = GenClosest.ClosestThingReachable(child.Position, child.Map, ThingRequest.ForDef(MantodeanDefOf.L24_Manto_SchoolDesk), PathEndMode.InteractionCell, TraverseParms.For(child), 9999f, (d) => child.CanReserveSittableOrSpot(SchoolUtility.DeskSpotStudent(d)) && teacher.CanReserveSittableOrSpot(SchoolUtility.DeskSpotTeacher(d)) && !d.IsForbidden(child) && !d.IsForbidden(teacher));
                }
            }
		}
	}
}
