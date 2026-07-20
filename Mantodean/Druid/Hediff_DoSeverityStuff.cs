using RimWorld;
using Verse;
using Mantodean.Druid.Defs;

namespace Mantodean.Druid
{

    public class HediffComp_DoSeverityStuff : HediffComp
    {
        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600002D RID: 45 RVA: 0x00003444 File Offset: 0x00001644
        public HediffCompProperties_DoSeverityStuff Props
        {
            get { return (HediffCompProperties_DoSeverityStuff)props; }
        }

        // Token: 0x0600002E RID: 46 RVA: 0x00003461 File Offset: 0x00001661
        public override void CompPostTick(ref float severityAdjustment)
        {
            Hatch();
        }

        // Token: 0x0600002F RID: 47 RVA: 0x0000346C File Offset: 0x0000166C
        public bool compare(float sever, float compareObject)
        {
            if (Props.lowerOrHigher == "Higher")
            {
                return sever >= compareObject;
            }
            if (Props.lowerOrHigher == "Lower")
            {
                return sever <= compareObject;
            }



            return true;
        }
        public void Hatch()

        {

            float severity = Props.doAtSeverity;
            string lowOrHigh = Props.lowerOrHigher;





            DamageDef damageType = Props.damageType;
            if (damageType == null)
            {
                damageType = MantoDamageDefOf.Flame;
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







            if (Props.getsResearch)
            {
                if (compare(parent.Severity, severity))
                {
                    if (parent.pawn.RaceProps.Humanlike &&
                        (parent.pawn.IsColonist || parent.pawn.IsPrisoner))
                        Find.ResearchManager.ResearchPerformed(Props.amountOfResearch, null);

                }
            }

            if (compare(parent.Severity, severity))
            {
                // BodyPartRecord bPart;
                // bPart = null;// pawn.RaceProps.body.GetPartAtIndex(targetnum);

                if (Props.getsDamage)
                {
                    DamageInfo dinfo =
                        new DamageInfo(Props.damageType, Props.amountDamage, 999,
                                       -1.0f, null, null, null);
                    dinfo.SetInstantPermanentInjury(true);
                    DamageWorker.DamageResult dres = parent.pawn.TakeDamage(dinfo);

                }
            }

            if (Props.canSpawnThing)
            {
                if (compare(parent.Severity, severity))
                {
                    for (int i = 0; i < Props.amountOfThings; i++)
                    {
                        GenSpawn.Spawn(thingDef, parent.pawn.Position,
                                       parent.pawn.Map, WipeMode.Vanish);

                    }
                }
            }

            if (Props.canSpawnPawn)
            {
                if (compare(parent.Severity, severity))
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
                            ;

                    }
                }
            }


            if (Props.addHeddif)
            {
                if (compare(parent.Severity, severity))
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


                if (compare(parent.Severity, severity))
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
                if (compare(parent.Severity, severity))
                    if (!Props.gasExplosion)
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

                        if (Props.corpseDestroyedAfterkill)
                        {
                            Corpse pawnCorpse = parent.pawn.Corpse;

                            pawnCorpse.Destroy(DestroyMode.Vanish);
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
                        if (Props.corpseDestroyedAfterkill)
                        {
                            Corpse pawnCorpse = parent.pawn.Corpse;

                            pawnCorpse.Destroy(DestroyMode.Vanish);
                        }

                    }
                }

            }
            if (Props.removeThis)
            {
                if (compare(parent.Severity, severity))
                {

                    Hediff sourceHediff = parent.pawn.health.hediffSet.GetFirstHediffOfDef(parent.sourceHediffDef);

                    parent.pawn.health.RemoveHediff(sourceHediff);

                }
            }

            if (!Props.removeThis)
            {
                switch (lowOrHigh)
                {
                    case "Higher": parent.Severity = parent.Severity * 101 / 100; break;

                    case "Lower": parent.Severity = parent.Severity * 99 / 100; break;
                }
            }

            if (compare(parent.Severity, severity))
            {
                if (Props.getsKilled)
                {
                    parent.pawn.Kill(null, null);

                }


            }

        }


        // Token: 0x04000018 RID: 24
        private int HatchingTicker = 0;
    }
}
