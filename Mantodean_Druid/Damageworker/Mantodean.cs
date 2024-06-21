using System;
using System.Collections.Generic;
using Verse;

namespace L24
{
	// Token: 0x0200035B RID: 859
	public class DamageWorker_ExceptMantodean : DamageWorker_AddInjury
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

			Pawn pawn2 = dinfo.Instigator as Pawn;
			Boolean tradeTagCheckVictim = false;
			pawn.def.tradeTags.ForEach(tag => {
				if (tag.ToString() == "MantoInsect")
				{
					tradeTagCheckVictim = !tradeTagCheckVictim;
				}

			});

			if (pawn2.Faction != null)
			{
				if (tradeTagCheckVictim)
				{
					if (pawn.Faction != pawn2.Faction)
					{
						totalDamage = base.ReduceDamageToPreserveOutsideParts(totalDamage, dinfo, pawn);
						List<BodyPartRecord> list2 = new List<BodyPartRecord>();
						for (BodyPartRecord bodyPartRecord = dinfo.HitPart; bodyPartRecord != null; bodyPartRecord = bodyPartRecord.parent)
						{
							list2.Add(bodyPartRecord);
							if (bodyPartRecord.depth == BodyPartDepth.Outside)
							{
								break;
							}
						}
						for (int i = 0; i < list2.Count; i++)
						{
							BodyPartRecord bodyPartRecord2 = list2[i];
							float totalDamage2;
							if (list2.Count == 1)
							{
								totalDamage2 = totalDamage;
							}
							else
							{
								totalDamage2 = ((bodyPartRecord2.depth == BodyPartDepth.Outside) ? (totalDamage * 0.75f) : (totalDamage * 0.4f));
							}
							DamageInfo dinfo2 = dinfo;
							dinfo2.SetHitPart(bodyPartRecord2);
							base.FinalizeAndAddInjury(pawn, totalDamage2, dinfo2, result);
						}
					}
				}
			}

			if (pawn2.Faction == null)
			{
				if (tradeTagCheckVictim)
				{
					totalDamage = base.ReduceDamageToPreserveOutsideParts(totalDamage, dinfo, pawn);
					List<BodyPartRecord> list = new List<BodyPartRecord>();
					for (BodyPartRecord bodyPartRecord = dinfo.HitPart; bodyPartRecord != null; bodyPartRecord = bodyPartRecord.parent)
					{
						list.Add(bodyPartRecord);
						if (bodyPartRecord.depth == BodyPartDepth.Outside)
						{
							break;
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						BodyPartRecord bodyPartRecord2 = list[i];
						float totalDamage2;
						if (list.Count == 1)
						{
							totalDamage2 = totalDamage;
						}
						else
						{
							totalDamage2 = ((bodyPartRecord2.depth == BodyPartDepth.Outside) ? (totalDamage * 0.75f) : (totalDamage * 0.4f));
						}
						DamageInfo dinfo2 = dinfo;
						dinfo2.SetHitPart(bodyPartRecord2);
						base.FinalizeAndAddInjury(pawn, totalDamage2, dinfo2, result);
					}




				}




			}









			if (!tradeTagCheckVictim)
			{
				totalDamage = base.ReduceDamageToPreserveOutsideParts(totalDamage, dinfo, pawn);
				List<BodyPartRecord> list = new List<BodyPartRecord>();
				for (BodyPartRecord bodyPartRecord = dinfo.HitPart; bodyPartRecord != null; bodyPartRecord = bodyPartRecord.parent)
				{
					list.Add(bodyPartRecord);
					if (bodyPartRecord.depth == BodyPartDepth.Outside)
					{
						break;
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					BodyPartRecord bodyPartRecord2 = list[i];
					float totalDamage2;
					if (list.Count == 1)
					{
						totalDamage2 = totalDamage;
					}
					else
					{
						totalDamage2 = ((bodyPartRecord2.depth == BodyPartDepth.Outside) ? (totalDamage * 0.75f) : (totalDamage * 0.4f));
					}
					DamageInfo dinfo2 = dinfo;
					dinfo2.SetHitPart(bodyPartRecord2);
					base.FinalizeAndAddInjury(pawn, totalDamage2, dinfo2, result);
				}




			}
		}

		// Token: 0x040013AF RID: 5039
		private const float DamageFractionOnOuterParts = 0.75f;

		// Token: 0x040013B0 RID: 5040
		private const float DamageFractionOnInnerParts = 0.4f;
	}
}
