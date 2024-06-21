using System;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using System.Collections.Generic;
using RimWorld;

namespace L24
{
	// Token: 0x0200140D RID: 5133
	public class CompCustomHatcher : ThingComp
	{
		// Token: 0x170015F1 RID: 5617
		// (get) Token: 0x06007EAD RID: 32429 RVA: 0x002B7B5A File Offset: 0x002B5D5A
		public CompProperties_customHatcher Props
		{
			get
			{
				return (CompProperties_customHatcher)this.props;
			}
		}

		// Token: 0x170015F2 RID: 5618
		// (get) Token: 0x06007EAE RID: 32430 RVA: 0x002B7B67 File Offset: 0x002B5D67
		private CompTemperatureRuinable FreezerComp
		{
			get
			{
				return this.parent.GetComp<CompTemperatureRuinable>();
			}
		}

		// Token: 0x170015F3 RID: 5619
		// (get) Token: 0x06007EAF RID: 32431 RVA: 0x002B7B74 File Offset: 0x002B5D74
		public bool TemperatureDamaged
		{
			get
			{
				return this.FreezerComp != null && this.FreezerComp.Ruined;
			}
		}

		// Token: 0x06007EB0 RID: 32432 RVA: 0x002B7B8C File Offset: 0x002B5D8C
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<float>(ref this.gestateProgress, "gestateProgress", 0f, false);
			Scribe_References.Look<Pawn>(ref this.hatcheeParent, "hatcheeParent", false);
			Scribe_References.Look<Pawn>(ref this.otherParent, "otherParent", false);
			Scribe_References.Look<Faction>(ref this.hatcheeFaction, "hatcheeFaction", false);
		}

		// Token: 0x06007EB1 RID: 32433 RVA: 0x002B7BE8 File Offset: 0x002B5DE8
		public override void CompTick()
		{

			//float num2 = this.def.plant.visualSizeRange.LerpThroughRange(this.growthInt);
			//float num3 = this.parent.Graphic.drawSize.x * num2;

			if (!this.TemperatureDamaged)
			{
				float num = 1f / (this.Props.hatcherDaystoHatch * 60000f);
				this.gestateProgress += num;
				if (this.gestateProgress > 1f)
				{
					this.Hatch();
				}
			}
		}

		public float Lerp(float a, float b, float t)
		{
			// Sicherstellen, dass t zwischen 0 und 1 liegt
			if (t < 0) t = 0;
			else if (t > 1) t = 1;

			// Lineare Interpolation zwischen a und b
			return a + (b - a) * t;
		}

		// Token: 0x06007EB2 RID: 32434 RVA: 0x002B7C38 File Offset: 0x002B5E38
		public void Hatch()
		{
			PawnKindDef thingToHatch = this.Props.GetRandomThingToHatch();

			try
			{
				Faction factionInitial = Faction.OfPlayer;
				PawnGenerationRequest request = new PawnGenerationRequest(thingToHatch, factionInitial, PawnGenerationContext.NonPlayer, -1, false, false, true, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, this.Props.developmentStage, null, null, null, false);


				for (int i = 0; i < this.parent.stackCount; i++)
				{
					Pawn pawn = PawnGenerator.GeneratePawn(request);
					if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, this.parent, null))
					{
						if (pawn != null)
						{
							if (this.hatcheeParent != null)
							{
								if (pawn.playerSettings != null && this.hatcheeParent.playerSettings != null && this.hatcheeParent.Faction == this.hatcheeFaction)
								{
									pawn.playerSettings.AreaRestrictionInPawnCurrentMap = this.hatcheeParent.playerSettings.AreaRestrictionInPawnCurrentMap;
								}
								if (pawn.RaceProps.IsFlesh)
								{
									pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.hatcheeParent);
								}
							}
							if (this.otherParent != null && (this.hatcheeParent == null || this.hatcheeParent.gender != this.otherParent.gender) && pawn.RaceProps.IsFlesh)
							{
								pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, this.otherParent);
							}
							if (this.Props.changeAge)
							{
								pawn.ageTracker.AgeBiologicalTicks = (this.Props.ageInYears * 100 / 100) * 3600000;
							}

							if (this.Props.child != null)
							{
								AddChildBackstoryToPawn(pawn, this.Props.child);
							}

							if (this.Props.adult != null)
							{
								AddAdultBackstoryToPawn(pawn, this.Props.adult);
							}
							if (ModLister.AnomalyInstalled)
							{
								if (this.Props.mutantType != null)
								{

									MutantUtility.SetPawnAsMutantInstantly(pawn, this.Props.mutantType, RotStage.Fresh);
								}
							}



							if (this.Props.spawnsThings)
							{
								for (int j = 0; j < this.Props.amountOfThings; j++)
								{
									GenSpawn.Spawn(this.Props.thingToSpawn, pawn.Position,
												   pawn.Map, WipeMode.Vanish);
								}
							}
							if (this.Props.changeFaction)
							{
								Faction faction = Faction.OfPlayer;
								pawn.SetFaction(faction);
								FactionDef factionDef = FactionDef.Named(this.Props.factionForPawn);
								if (factionDef != null)
								{
									faction = FactionUtility.DefaultFactionFrom(factionDef);
									pawn.SetFaction(faction);

								}


							}

						}
						if (this.parent.Spawned)
						{
							if (this.Props.spawnsFilth)
							{
								FilthMaker.TryMakeFilth(this.parent.Position, this.parent.Map, this.Props.filthDef, this.Props.amountOfFilth, FilthSourceFlags.None, true);
							}
						}

						if (this.Props.berzerkAfterHatch)
						{ pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, false, false, false, null, false, false); }

						if (this.Props.doExplosion)
						{
							if (!this.Props.gasExplosion)
							{

								GenExplosion.DoExplosion(pawn.Position,
								pawn.Map,
								this.Props.explosionRadius,
								this.Props.explosionDamageDef,
								pawn,
								this.Props.damageAmountExplosion,
								this.Props.armorPenetrationExplosion,
								this.Props.explosionSound, null, null, null,
								this.Props.filthOrBloodToSpawn, 1, 6,
								null, false, null, 0f, 0, 0, true, null, null, null, true, 0.6f, 0f, false, null, 1f);
								if (this.Props.getsKilledInExplosion)
								{
									pawn.Kill(null, null);

								}
							}
							if (this.Props.gasExplosion)
							{
								GenExplosion.DoExplosion(pawn.Position,
							pawn.Map,
							this.Props.explosionRadius,
							this.Props.explosionDamageDef,
							pawn,
							this.Props.damageAmountExplosion,
							this.Props.armorPenetrationExplosion,
							this.Props.explosionSound, null, null, null,
							this.Props.filthOrBloodToSpawn, 1, 6,
							null, false, null, 0f, 0, 0, true, null, null, null, true, 0.6f, 0f, false, null, 1f);

								if (this.Props.getsKilledInExplosion)
								{
									pawn.Kill(null, null);

								}

							}

						}
						if (this.Props.addHediffs) {
							for (int k = 0; k < this.Props.partsToAffect.Count; k++)
							{

								BodyPartRecord[] allPartsList = pawn.def.race.body.AllParts.ToArray();

								for (int l = 0; l < allPartsList.Length; l++)
								{

									if (allPartsList[l].customLabel == this.Props.partsToAffect[k])
									{

										Hediff hediffAdd = HediffMaker.MakeHediff(this.Props.hediffs[k], pawn);

										pawn.health.AddHediff(hediffAdd, allPartsList[l]);
									
									}

								



								}
							}
							//	pawn.health.AddHediff(this.Props.hediffs[k], part);



						}
						if (this.Props.destroyPawn)
						{
							pawn.Destroy(DestroyMode.Vanish);
						}




					}


					else
					{
						Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
					}
				}
			}

			finally
			{
				this.parent.Destroy(DestroyMode.Vanish);
			}
		}

		// Token: 0x06007EB3 RID: 32435 RVA: 0x002B7E48 File Offset: 0x002B6048
		public override bool AllowStackWith(Thing other)
		{
			CompCustomHatcher comp = ((ThingWithComps)other).GetComp<CompCustomHatcher>();
			return this.TemperatureDamaged == comp.TemperatureDamaged && base.AllowStackWith(other);
		}

		// Token: 0x06007EB4 RID: 32436 RVA: 0x002B7E78 File Offset: 0x002B6078
		public override void PreAbsorbStack(Thing otherStack, int count)
		{
			float t = (float)count / (float)(this.parent.stackCount + count);
			float b = ((ThingWithComps)otherStack).GetComp<CompCustomHatcher>().gestateProgress;

			this.gestateProgress = Lerp(this.gestateProgress, b, t);
		}

		// Token: 0x06007EB5 RID: 32437 RVA: 0x002B7EBB File Offset: 0x002B60BB
		public override void PostSplitOff(Thing piece)
		{
			CompCustomHatcher comp = ((ThingWithComps)piece).GetComp<CompCustomHatcher>();
			comp.gestateProgress = this.gestateProgress;
			comp.hatcheeParent = this.hatcheeParent;
			comp.otherParent = this.otherParent;
			comp.hatcheeFaction = this.hatcheeFaction;
		}

		// Token: 0x06007EB6 RID: 32438 RVA: 0x002B7EF7 File Offset: 0x002B60F7
		public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
		{
			base.PrePreTraded(action, playerNegotiator, trader);
			if (action == TradeAction.PlayerBuys)
			{
				this.hatcheeFaction = Faction.OfPlayer;
				return;
			}
			if (action == TradeAction.PlayerSells)
			{
				this.hatcheeFaction = trader.Faction;
			}
		}

		// Token: 0x06007EB7 RID: 32439 RVA: 0x002B7F22 File Offset: 0x002B6122
		public override void PostPostGeneratedForTrader(TraderKindDef trader, int forTile, Faction forFaction)
		{
			base.PostPostGeneratedForTrader(trader, forTile, forFaction);
			this.hatcheeFaction = forFaction;
		}

		// Token: 0x06007EB8 RID: 32440 RVA: 0x002B7F34 File Offset: 0x002B6134
		public override string CompInspectStringExtra()
		{
			if (!this.TemperatureDamaged)
			{
				return "EggProgress".Translate() + ": " + this.gestateProgress.ToStringPercent() + "\n" + "HatchesIn".Translate() + ": " + "PeriodDays".Translate((this.Props.hatcherDaystoHatch * (1f - this.gestateProgress)).ToString("F1"));
			}
			return null;
		}

		private void AddChildBackstoryToPawn(Pawn pawn, BackstoryDef child)
		{
			if (child != null)
			{
				pawn.story.Childhood = child;
			}
		}
		private void AddAdultBackstoryToPawn(Pawn pawn, BackstoryDef adult)
		{
			pawn.story.Adulthood = adult;
		}

		// Token: 0x04004739 RID: 18233
		private float gestateProgress;

		// Token: 0x0400473A RID: 18234
		public Pawn hatcheeParent;

		// Token: 0x0400473B RID: 18235
		public Pawn otherParent;

		// Token: 0x0400473C RID: 18236
		public Faction hatcheeFaction;
	}
}
