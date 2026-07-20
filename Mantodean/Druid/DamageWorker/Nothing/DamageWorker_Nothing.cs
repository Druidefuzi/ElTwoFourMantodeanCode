using Verse;

namespace Mantodean.Druuid.Bullets
{
	
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
	}
}
