using HarmonyLib;
using RimWorld;

namespace Mantodean.HarmonyPatches
{
	[HarmonyPatch(typeof(ITab_Pawn_Character), nameof(ITab_Pawn_Character.IsVisible), MethodType.Getter)]
	public static class Patch_ITab_Pawn_Character
	{
		
		[HarmonyPostfix]
		public static void Postfix(ITab_Pawn_Character __instance, ref bool __result)
		{
			if(__instance.PawnToShowInfoAbout.story != null && __instance.PawnToShowInfoAbout.def == MantodeanDefOf.L24_Animal_Scarab)
			{
				__result = false;
			}
		}
	}
}
