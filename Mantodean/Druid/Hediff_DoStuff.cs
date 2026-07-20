using RimWorld;
using UnityEngine;
using Verse;

namespace Mantodean.Druid
{

    public class HediffComp_DoStuff : HediffComp
    {
        public int researchTick = 0;

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600002D RID: 45 RVA: 0x00003444 File Offset: 0x00001644
        public HediffCompProperties_DoStuff Props
        {
            get { return (HediffCompProperties_DoStuff)props; }
        }

        // Token: 0x0600002E RID: 46 RVA: 0x00003461 File Offset: 0x00001661
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            Hatch();
        }

        public override void CompPostTickInterval(ref float severityAdjustment, int delta)
        {
            base.CompPostTickInterval(ref severityAdjustment, delta);

            if (Props.getsResearch)
            {
                if (researchTick > 0)
                {
                    researchTick--;

                    if (researchTick <= 0)
                    {
                        GenerateResearch();

                        researchTick = Props.researchEveryXTime;
                    }
                }
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            researchTick = Props.researchEveryXTime;
        }

        public override void CompPostMake()
        {
            base.CompPostMake();

            researchTick = Props.researchEveryXTime;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref researchTick, "researchTick", 0);
        }

        public void GenerateResearch()
        {
            if (parent.pawn.RaceProps.Humanlike && (parent.pawn.IsColonist || parent.pawn.IsPrisoner))
            {
                if (!Find.ResearchManager.IsCurrentProject(null))
                {
                    Find.ResearchManager.ResearchPerformed(Props.amountOfResearch, null);
                    Log.Message("[Hediff_DoStuff] Research point generated successfully.");
                }
            }
        }

        // Token: 0x0600002F RID: 47 RVA: 0x0000346C File Offset: 0x0000166C
        public void Hatch()

        {
            if (parent.pawn.Map != null)
            {
                float time = 1;
                string timeDefinition = Props.timeType;
                switch (timeDefinition)
                {
                    case "Ticks":
                        time = 1;

                        break;
                    case "ticks":
                        time = 1;
                        break;
                    case "hours":
                        time = 2500;

                        break;
                    case "Hours":
                        time = 2500;

                        break;

                    case "Days":
                        time = 60000;

                        break;
                    case "days":
                        time = 60000;

                        break;
                    case "Years":
                        time = 3600000;

                        break;
                    case "years":
                        time = 3600000;

                        break;
                }
                DamageDef damageType = Props.damageType;
                if (damageType == null)
                {
                    damageType = DamageDefOf.Flame;
                }
                ThingDef thingDef = Props.thingDefToSpawn;
                if (thingDef == null)
                {
                    thingDef = ThingDef.Named("Beer");
                }
                PawnKindDef pawnKindDef = Props.pawnToSpawn;

                if (pawnKindDef == null)
                {
                    pawnKindDef = PawnKindDef.Named("Drifter");
                }
                FactionDef factionDef = Props.factionOfPawn;
                if (factionDef == null)
                {
                    factionDef = FactionDef.Named("PlayerColony");
                }
                Faction faction = FactionUtility.DefaultFactionFrom(factionDef);

                BodyPartRecord part = Props.bodyPartForHediff;
                if (part == null)
                {
                    parent.pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.BloodPumpingSource);
                }

                ThingDef filthExplosion = Props.filthOrBloodToSpawn;
                if (filthExplosion == null)
                {
                    filthExplosion = ThingDefOf.Filth_Slime;
                }
                DamageDef damageExplosion = Props.explosionDamageDef;
                if (damageExplosion == null)
                {
                    damageExplosion = DamageDefOf.Flame;
                }

                //if (Props.getsResearch)
                //{
                //    if ((int) (Find.TickManager.TicksGame % (Props.researchEveryXTime * time)) == 0)
                //    {
                //        if (parent.pawn.RaceProps.Humanlike && (parent.pawn.IsColonist || parent.pawn.IsPrisoner))
                //        {
                //            if (!Find.ResearchManager.IsCurrentProject(null))
                //            {
                //                Find.ResearchManager.ResearchPerformed(Props.amountOfResearch, null);
                //                Log.Message("[Hediff_DoStuff] Research point generated successfully.");
                //            }
                //        }
                //    }
                //    //Log.Warning("[Hediff_DoStuff] Passive Research Failed, reason: modulus condition returned " + Find.TickManager.TicksGame % (Props.researchEveryXTime * time) + " or false.")
                //}

                if (Find.TickManager.TicksGame % (Props.damageEveryXTime * time) == 0)
                {
                    // BodyPartRecord bPart;
                    // bPart = null;// pawn.RaceProps.body.GetPartAtIndex(targetnum);

                    if (Props.getsDamage)
                    {
                        DamageInfo dinfo = new DamageInfo(Props.damageType, Props.amountDamage, 999, -1.0f, null, null, null);
                        dinfo.SetInstantPermanentInjury(true);
                        DamageWorker.DamageResult dres = parent.pawn.TakeDamage(dinfo);
                    }
                }

                if (Props.canSpawnThing)
                {
                    if (Find.TickManager.TicksGame % (Props.spawnThingEveryXTime * time) ==
                        0)
                    {
                        for (int i = 0; i < Props.amountOfThings; i++)
                        {
                            GenSpawn.Spawn(thingDef, parent.pawn.Position, parent.pawn.Map, WipeMode.Vanish);
                        }
                    }
                }

                if (Props.canSpawnPawn)
                {
                    if (Find.TickManager.TicksGame % (Props.spawnPawnEveryXTime * time) == 0)
                    {
                        for (int i = 0; i < Props.amountOfPawns; i++)
                        {

                            PawnGenerationRequest request = new PawnGenerationRequest(
                                pawnKindDef,
                                faction,
                                PawnGenerationContext.NonPlayer, -1, false, false, true, true, false,
                                1f, false, true, false, true, true, false, false, false, false, 0f,
                                0f, null, 1f, null, null, null, null, null, null, null, null, null,
                                null, null, null, false, false, false, false, null, null, null, null,
                                null, 0f, DevelopmentalStage.Newborn, null, null, null, false);

                            Pawn pawn = PawnGenerator.GeneratePawn(request);
                            if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, parent.pawn, null))
                            {
                                //Log.Warning("[Hediff_DoStuff] Trying to TrySpawnHatchedOrBornPawn with NULL position.");
                            }
                        }
                    }
                }


                if (Props.addHeddif)
                {
                    if (Find.TickManager.TicksGame % (Props.addsHediffAfterXTime * time) == 0)
                    {

                        Hediff hediff = HediffMaker.MakeHediff(Props.hediffToAdd, parent.pawn, null);
                        parent.pawn.health.AddHediff(hediff, part);
                    }
                }

                if (Props.getsOlder)
                {
                    string timeType = Props.yearsOrDaysOrTicks;
                    int timeOlder = 1;
                    switch (timeType)
                    {
                        case "Ticks":
                            timeOlder = 1;
                            break;

                        case "Days":
                            timeOlder = 60000;
                            break;
                        case "Years":
                            timeOlder = 3600000;
                            break;
                    }

                    if (Find.TickManager.TicksGame % (Props.getsOlderEveryXTime * time) == 0)
                    {
                        string youngOrOld = Props.olderOrYounger;
                        if (youngOrOld == "Older")
                        {
                            long age = parent.pawn.ageTracker.AgeBiologicalTicks;
                            age += Props.timeOlder * timeOlder;
                            parent.pawn.ageTracker.AgeBiologicalTicks = age;
                        }
                        if (youngOrOld == "Younger")
                        {
                            long age = parent.pawn.ageTracker.AgeBiologicalTicks;
                            age -= Props.timeOlder * timeOlder;
                            parent.pawn.ageTracker.AgeBiologicalTicks = age;
                        }



                    }

                }

                if (Props.doExplosion)
                {
                    if (!Props.gasExplosion)
                    {
                        if (Find.TickManager.TicksGame % (Props.explodesEveryXTime * time) == 0)
                        {
                            GenExplosion.DoExplosion(parent.pawn.Position,
                                parent.pawn.Map,
                                Props.explosionRadius,
                                damageExplosion,
                                parent.pawn,
                                Props.damageAmountExplosion,
                                Props.armorPenetrationExplosion,
                                Props.explosionSound, null, null, null,
                                filthExplosion, 1, 6,
                                null, null, 0, false, null, 0f, 0, 0, true, null, null, null, true, 0.6f, 0f, false, null, 1f);

                            if (Props.getsKilledInExplosion)
                            {
                                parent.pawn.Kill(null, null);
                            }
                        }
                        if (Props.gasExplosion)
                        {
                            GenExplosion.DoExplosion(parent.pawn.Position,
                                parent.pawn.Map,
                                Props.explosionRadius,
                                damageExplosion,
                                parent.pawn,
                                Props.damageAmountExplosion,
                                Props.armorPenetrationExplosion,
                                Props.explosionSound, null, null, null,
                                filthExplosion, 1, 6,
                                Props.gasType, null, 0, false, null, 0f, 0, 0, true, null, null, null, true, 0.6f, 0f, false, null, 1f);
                            
                            if (Props.getsKilledInExplosion)
                            {
                                parent.pawn.Kill(null, null);
                            }

                        }
                    }
                }
                if (Props.removeThis)
                {
                    if (Find.TickManager.TicksGame % (Props.removeAfterXTime * time) == 0)
                    {

                        Hediff sourceHediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(parent.sourceHediffDef);

                        parent.pawn.health.RemoveHediff(sourceHediff);
                    }
                }

                if (Find.TickManager.TicksGame % (Props.getsKilledAfterXTime * time) == 0)
                {
                    if (Props.getsKilled)
                    {
                        parent.pawn.Kill(null, null);
                    }
                }
            }
        }

        // Token: 0x04000018 RID: 24
        private int HatchingTicker = 0;
    }
}
