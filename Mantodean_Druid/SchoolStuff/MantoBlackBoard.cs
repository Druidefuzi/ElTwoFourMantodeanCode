using System;
using HarmonyLib;
using Verse;

namespace L24
{
	// Token: 0x02000002 RID: 2
	public class NeolithicBlackboardMod : Mod
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public NeolithicBlackboardMod(ModContentPack content) : base(content)
		{
			Harmony harmony = new Harmony("Hol.NeolStuff");
			harmony.PatchAll();
		}
	}
}
