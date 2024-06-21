using System;
using System.Collections.Generic;
using Verse;

namespace L24
{
	// Token: 0x0200035B RID: 859c
	public class DamageWorker_Nothing : DamageWorker_AddInjury
	{
		// Token: 0x0600181C RID: 6172 RVA: 0x00093B2C File Offset: 0x00091D2C
		protected override BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
		{

			BodyPartRecord randomNotMissingPart = pawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, dinfo.Depth, null);
			if (randomNotMissingPart.depth != BodyPartDepth.Inside && Rand.Chance(this.def.stabChanceOfForcedInternal))
			{
				BodyPartRecord randomNotMissingPart2 = pawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, BodyPartHeight.Undefined, BodyPartDepth.Inside, randomNotMissingPart);
				if (randomNotMissingPart2 != null)
				{
					return randomNotMissingPart2;
				}
			}
			return randomNotMissingPart;
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x00093B9C File Offset: 0x00091D9C
		protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
		{



		}

		// Token: 0x040013AF RID: 5039
		private const float DamageFractionOnOuterParts = 0.75f;

		// Token: 0x040013B0 RID: 5040
		private const float DamageFractionOnInnerParts = 0.4f;
	}
}
