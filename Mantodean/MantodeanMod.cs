using HarmonyLib;
using Verse;

namespace Mantodean
{
	// Token: 0x02000002 RID: 2
	public class MantodeanMod : Mod
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public MantodeanMod(ModContentPack content) : base(content)
		{
			Harmony harmony = new Harmony("ElTwoFour.Mantodean");
			harmony.PatchAll();
		}
	}
}
