using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace L24
{

    public class HediffComp_DoStuff : HediffComp
    {
        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600002D RID: 45 RVA: 0x00003444 File Offset: 0x00001644
        public HediffCompProperties_DoStuff Props
        {
            get { return (HediffCompProperties_DoStuff)this.props; }
        }

        // Token: 0x0600002E RID: 46 RVA: 0x00003461 File Offset: 0x00001661
        public override void CompPostTick(ref float severityAdjustment)
        {
            this.Hatch();
        }

        // Token: 0x0600002F RID: 47 RVA: 0x0000346C File Offset: 0x0000166C
        public void Hatch()

        {
            if (this.parent.pawn.Map != null)
            {

                float time = 1;
                string timeDefinition = this.Props.timeType;
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
                DamageDef damageType = this.Props.damageType;
                if (damageType == null)
                {
                    damageType = DamageDefOf.Flame;
                }
                ThingDef thingDef = this.Props.thingDefToSpawn;
                if (thingDef == null)
                {
                    thingDef = ThingDef.Named("Beer");
                }
                PawnKindDef pawnKindDef = this.Props.pawnToSpawn;

                if (pawnKindDef == null)
                {
                    pawnKindDef = PawnKindDef.Named("Drifter");
                }
                FactionDef factionDef = this.Props.factionOfPawn;
                if (factionDef == null)
                {
                    factionDef = FactionDef.Named("PlayerColony");
                }
                Faction faction = FactionUtility.DefaultFactionFrom(factionDef);

                BodyPartRecord part = this.Props.bodyPartForHediff;
                if (part == null)
                {
                    this.parent.pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.BloodPumpingSource);
                }

                ThingDef filthExplosion = this.Props.filthOrBloodToSpawn;
                if (filthExplosion == null)
                {
                    filthExplosion = ThingDefOf.Filth_Slime;
                }
                DamageDef damageExplosion = this.Props.explosionDamageDef;
                if (damageExplosion == null)
                {
                    damageExplosion = DamageDefOf.Flame;
                }







                if (this.Props.getsResearch)
                {
                    if ((Find.TickManager.TicksGame % (this.Props.researchEveryXTime * time)) == 0)
                    {
                        if (this.parent.pawn.RaceProps.Humanlike &&
                            (this.parent.pawn.IsColonist || this.parent.pawn.IsPrisoner))
                            if (!Find.ResearchManager.IsCurrentProject(null)) {
                                Find.ResearchManager.ResearchPerformed(this.Props.amountOfResearch, null);
                            }
                           
                    }
                }

                if ((Find.TickManager.TicksGame % (this.Props.damageEveryXTime * time)) == 0)
                {
                    // BodyPartRecord bPart;
                    // bPart = null;// pawn.RaceProps.body.GetPartAtIndex(targetnum);

                    if (this.Props.getsDamage)
                    {
                        DamageInfo dinfo =
                            new DamageInfo(Props.damageType, this.Props.amountDamage, 999,
                                           -1.0f, null, null, null);
                        dinfo.SetInstantPermanentInjury(true);
                        DamageWorker.DamageResult dres = this.parent.pawn.TakeDamage(dinfo);
                    }
                }

                if (this.Props.canSpawnThing)
                {
                    if ((Find.TickManager.TicksGame % (this.Props.spawnThingEveryXTime * time)) ==
                        0)
                    {
                        for (int i = 0; i < this.Props.amountOfThings; i++)
                        {
                            GenSpawn.Spawn(thingDef, this.parent.pawn.Position,
                                           this.parent.pawn.Map, WipeMode.Vanish);
                        }
                    }
                }

                if (this.Props.canSpawnPawn)
                {
                    if ((Find.TickManager.TicksGame % (this.Props.spawnPawnEveryXTime * time)) == 0)
                    {
                        for (int i = 0; i < this.Props.amountOfPawns; i++)
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
                            if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, this.parent.pawn, null))
                                ;
                        }
                    }
                }


                if (this.Props.addHeddif)
                {
                    if ((Find.TickManager.TicksGame % (this.Props.addsHediffAfterXTime * time)) == 0)
                    {

                        Hediff hediff = HediffMaker.MakeHediff(this.Props.hediffToAdd, this.parent.pawn, null);
                        this.parent.pawn.health.AddHediff(hediff, part);
                    }
                }

                if (this.Props.getsOlder)
                {
                    string timeType = this.Props.yearsOrDaysOrTicks;
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

                    if ((Find.TickManager.TicksGame % (this.Props.getsOlderEveryXTime * time)) == 0)
                    {
                        string youngOrOld = this.Props.olderOrYounger;
                        if (youngOrOld == "Older")
                        {
                            long age = this.parent.pawn.ageTracker.AgeBiologicalTicks;
                            age += (this.Props.timeOlder * timeOlder);
                            this.parent.pawn.ageTracker.AgeBiologicalTicks = age;
                        }
                        if (youngOrOld == "Younger")
                        {
                            long age = this.parent.pawn.ageTracker.AgeBiologicalTicks;
                            age -= (this.Props.timeOlder * timeOlder);
                            this.parent.pawn.ageTracker.AgeBiologicalTicks = age;
                        }



                    }

                }

                if (this.Props.doExplosion)
                {
                    if (!this.Props.gasExplosion)
                    {
                        if ((Find.TickManager.TicksGame % (this.Props.explodesEveryXTime * time)) == 0)
                        {
                            GenExplosion.DoExplosion(parent.pawn.Position,
                            parent.pawn.Map,
                            this.Props.explosionRadius,
                            damageExplosion,
                            this.parent.pawn,
                            this.Props.damageAmountExplosion,
                            this.Props.armorPenetrationExplosion,
                            this.Props.explosionSound, null, null, null,
                            filthExplosion, 1, 6,
                            null, false, null, 0f, 0, 0, true, null, null, null, true, 0.6f, 0f, false, null, 1f);
                            if (this.Props.getsKilledInExplosion)
                            {
                                this.parent.pawn.Kill(null, null);

                            }
                        }
                        if (this.Props.gasExplosion)
                        {
                            GenExplosion.DoExplosion(parent.pawn.Position,
                           parent.pawn.Map,
                           this.Props.explosionRadius,
                           damageExplosion,
                           this.parent.pawn,
                           this.Props.damageAmountExplosion,
                           this.Props.armorPenetrationExplosion,
                           this.Props.explosionSound, null, null, null,
                           filthExplosion, 1, 6,
                           this.Props.gasType, false, null, 0f, 0, 0, true, null, null, null, true, 0.6f, 0f, false, null, 1f);
                            if (this.Props.getsKilledInExplosion)
                            {
                                this.parent.pawn.Kill(null, null);


                            }

                        }
                    }
                }
                if (this.Props.removeThis)
                {
                    if ((Find.TickManager.TicksGame % (this.Props.removeAfterXTime * time)) == 0)
                    {

                        Hediff sourceHediff = this.parent.pawn.health.hediffSet.GetFirstHediffOfDef(this.parent.sourceHediffDef);

                        this.parent.pawn.health.RemoveHediff(sourceHediff);

                    }
                }

                if ((Find.TickManager.TicksGame % (this.Props.getsKilledAfterXTime * time)) == 0)
                {
                    if (this.Props.getsKilled)
                    {
                        this.parent.pawn.Kill(null, null);
                    }


                }
            }
        }


        // Token: 0x04000018 RID: 24
        private int HatchingTicker = 0;
    }
}
