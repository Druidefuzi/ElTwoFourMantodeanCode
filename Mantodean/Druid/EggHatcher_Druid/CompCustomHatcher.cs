using RimWorld.Planet;
using Verse;
using RimWorld;

namespace Mantodean.Druid.EggHatcher_Druid
{
	// Token: 0x0200140D RID: 5133
	public class CompCustomHatcher : ThingComp
	{
		// Token: 0x170015F1 RID: 5617
		// (get) Token: 0x06007EAD RID: 32429 RVA: 0x002B7B5A File Offset: 0x002B5D5A
		public CompProperties_CustomHatcher Props
		{
			get
			{
				return (CompProperties_CustomHatcher)props;
			}
		}

		// Token: 0x170015F2 RID: 5618
		// (get) Token: 0x06007EAE RID: 32430 RVA: 0x002B7B67 File Offset: 0x002B5D67
		private CompTemperatureRuinable FreezerComp
		{
			get
			{
				return parent.GetComp<CompTemperatureRuinable>();
			}
		}

		// Token: 0x170015F3 RID: 5619
		// (get) Token: 0x06007EAF RID: 32431 RVA: 0x002B7B74 File Offset: 0x002B5D74
		public bool TemperatureDamaged
		{
			get
			{
				return FreezerComp != null && FreezerComp.Ruined;
			}
		}

		// Token: 0x06007EB0 RID: 32432 RVA: 0x002B7B8C File Offset: 0x002B5D8C
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref gestateProgress, "gestateProgress", 0f, false);
			Scribe_References.Look(ref hatcheeParent, "hatcheeParent", false);
			Scribe_References.Look(ref otherParent, "otherParent", false);
			Scribe_References.Look(ref hatcheeFaction, "hatcheeFaction", false);
		}

		// Token: 0x06007EB1 RID: 32433 RVA: 0x002B7BE8 File Offset: 0x002B5DE8
		public override void CompTick()
		{

			//float num2 = this.def.plant.visualSizeRange.LerpThroughRange(this.growthInt);
			//float num3 = this.parent.Graphic.drawSize.x * num2;

			if (!TemperatureDamaged)
			{
				float num = 1f / (Props.hatcherDaystoHatch * 60000f);
                gestateProgress += num;
				if (gestateProgress > 1f)
				{
                    Hatch();
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
			PawnKindDef thingToHatch = Props.GetRandomThingToHatch();

			try
			{
				Faction factionInitial = Faction.OfPlayer;
				PawnGenerationRequest request = new PawnGenerationRequest(thingToHatch, factionInitial, PawnGenerationContext.NonPlayer, -1, false, false, true, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, Props.developmentStage, null, null, null, false);

				for (int i = 0; i < parent.stackCount; i++)
				{
					Pawn pawn = PawnGenerator.GeneratePawn(request);
					if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, parent, null))
					{
						if (pawn != null)
						{
							if (hatcheeParent != null)
							{
								if (pawn.playerSettings != null && hatcheeParent.playerSettings != null && hatcheeParent.Faction == hatcheeFaction)
								{
									pawn.playerSettings.AreaRestrictionInPawnCurrentMap = hatcheeParent.playerSettings.AreaRestrictionInPawnCurrentMap;
								}
								if (pawn.RaceProps.IsFlesh)
								{
									pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, hatcheeParent);
								}
							}
							if (otherParent != null && (hatcheeParent == null || hatcheeParent.gender != otherParent.gender) && pawn.RaceProps.IsFlesh)
							{
								pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, otherParent);
							}
							if (Props.changeAge)
							{
								pawn.ageTracker.AgeBiologicalTicks = Props.ageInYears * 100 / 100 * 3600000;
							}

							if (Props.child != null)
							{
								AddChildBackstoryToPawn(pawn, Props.child);
							}

							if (Props.adult != null)
							{
								AddAdultBackstoryToPawn(pawn, Props.adult);
							}
							if (ModLister.AnomalyInstalled)
							{
								if (Props.mutantType != null)
								{

									MutantUtility.SetPawnAsMutantInstantly(pawn, Props.mutantType, RotStage.Fresh);
								}
							}



							if (Props.spawnsThings)
							{
								for (int j = 0; j < Props.amountOfThings; j++)
								{
									GenSpawn.Spawn(Props.thingToSpawn, pawn.Position,
												   pawn.Map, WipeMode.Vanish);
								}
							}
							if (Props.changeFaction)
							{
								Faction faction = Faction.OfPlayer;
								pawn.SetFaction(faction);
								FactionDef factionDef = FactionDef.Named(Props.factionForPawn);
								if (factionDef != null)
								{
									faction = FactionUtility.DefaultFactionFrom(factionDef);
									pawn.SetFaction(faction);

								}


							}

						}
						if (parent.Spawned)
						{
							if (Props.spawnsFilth)
							{
								FilthMaker.TryMakeFilth(parent.Position, parent.Map, Props.filthDef, Props.amountOfFilth, FilthSourceFlags.None, true);
							}
						}

						if (Props.berzerkAfterHatch)
						{ pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, false, false, false, null, false, false); }

						if (Props.doExplosion)
						{
							if (!Props.gasExplosion)
							{

								GenExplosion.DoExplosion(pawn.Position,
								pawn.Map,
                                Props.explosionRadius,
                                Props.explosionDamageDef,
								pawn,
                                Props.damageAmountExplosion,
                                Props.armorPenetrationExplosion,
                                Props.explosionSound, null, null, null,
                                Props.filthOrBloodToSpawn, 1, 6,
								null, null, 0, false, null, 0f, 0, 0, true, null, null, null, true, 0.6f, 0f, false, null, 1f);
								if (Props.getsKilledInExplosion)
								{
									pawn.Kill(null, null);

								}
							}
							if (Props.gasExplosion)
							{
								GenExplosion.DoExplosion(pawn.Position,
							pawn.Map,
                            Props.explosionRadius,
                            Props.explosionDamageDef,
							pawn,
                            Props.damageAmountExplosion,
                            Props.armorPenetrationExplosion,
                            Props.explosionSound, null, null, null,
                            Props.filthOrBloodToSpawn, 1, 6,
							null, null, 0, false, null, 0f, 0, 0, true, null, null, null, true, 0.6f, 0f, false, null, 1f);

								if (Props.getsKilledInExplosion)
								{
									pawn.Kill(null, null);

								}

							}

						}
						if (Props.addHediffs) {
							for (int k = 0; k < Props.partsToAffect.Count; k++)
							{

								BodyPartRecord[] allPartsList = pawn.def.race.body.AllParts.ToArray();

								for (int l = 0; l < allPartsList.Length; l++)
								{

									if (allPartsList[l].customLabel == Props.partsToAffect[k])
									{

										Hediff hediffAdd = HediffMaker.MakeHediff(Props.hediffs[k], pawn);

										pawn.health.AddHediff(hediffAdd, allPartsList[l]);
									
									}

								}
							}
							//	pawn.health.AddHediff(this.Props.hediffs[k], part);

						}
						if (Props.destroyPawn)
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
                parent.Destroy(DestroyMode.Vanish);
			}
		}

		// Token: 0x06007EB3 RID: 32435 RVA: 0x002B7E48 File Offset: 0x002B6048
		public override bool AllowStackWith(Thing other)
		{
			CompCustomHatcher comp = ((ThingWithComps)other).GetComp<CompCustomHatcher>();
			return TemperatureDamaged == comp.TemperatureDamaged && base.AllowStackWith(other);
		}

		// Token: 0x06007EB4 RID: 32436 RVA: 0x002B7E78 File Offset: 0x002B6078
		public override void PreAbsorbStack(Thing otherStack, int count)
		{
			float t = count / (float)(parent.stackCount + count);
			float b = ((ThingWithComps)otherStack).GetComp<CompCustomHatcher>().gestateProgress;

            gestateProgress = Lerp(gestateProgress, b, t);
		}

		// Token: 0x06007EB5 RID: 32437 RVA: 0x002B7EBB File Offset: 0x002B60BB
		public override void PostSplitOff(Thing piece)
		{
			CompCustomHatcher comp = ((ThingWithComps)piece).GetComp<CompCustomHatcher>();
			comp.gestateProgress = gestateProgress;
			comp.hatcheeParent = hatcheeParent;
			comp.otherParent = otherParent;
			comp.hatcheeFaction = hatcheeFaction;
		}

		// Token: 0x06007EB6 RID: 32438 RVA: 0x002B7EF7 File Offset: 0x002B60F7
		public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
		{
			base.PrePreTraded(action, playerNegotiator, trader);
			if (action == TradeAction.PlayerBuys)
			{
                hatcheeFaction = Faction.OfPlayer;
				return;
			}
			if (action == TradeAction.PlayerSells)
			{
                hatcheeFaction = trader.Faction;
			}
		}

		// Token: 0x06007EB7 RID: 32439 RVA: 0x002B7F22 File Offset: 0x002B6122
		public override void PostPostGeneratedForTrader(TraderKindDef trader, PlanetTile forTile, Faction forFaction)
		{
			base.PostPostGeneratedForTrader(trader, forTile, forFaction);
            hatcheeFaction = forFaction;
		}

		// Token: 0x06007EB8 RID: 32440 RVA: 0x002B7F34 File Offset: 0x002B6134
		public override string CompInspectStringExtra()
		{
			if (!TemperatureDamaged)
			{
				return "EggProgress".Translate() + ": " + gestateProgress.ToStringPercent() + "\n" + "HatchesIn".Translate() + ": " + "PeriodDays".Translate((Props.hatcherDaystoHatch * (1f - gestateProgress)).ToString("F1"));
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
