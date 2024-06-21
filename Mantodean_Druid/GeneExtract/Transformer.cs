#region Assembly Assembly-CSharp, Version=1.5.8909.13066, Culture=neutral, PublicKeyToken=null
// C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll
// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
#endregion

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;
using System.Text;

namespace L24
{
    [StaticConstructorOnStartup]
    public class Building_TransformerDruid : Building_Enterable, IThingHolderWithDrawnPawn, IThingHolder
    {
        private int ticksRemaining;
        private int amountOfTicks = 30000;
        private bool victimOrHaul = true;

        private int powerCutTicks;
        private string testScribe = "Here could be your value, lol";
        private bool startWorking = false;
        private string whatToDo;
        private HediffDef oldHediff;
        private HediffDef newHediff;
        private Pawn selectedHauler;

        [Unsaved(false)]
        private CompRefuelable cachedComp;

        private CompExtractor compExtractor;

        [Unsaved(false)]
        private Texture2D cachedInsertPawnTex;

        [Unsaved(false)]
        private Sustainer sustainerWorking;

        [Unsaved(false)]
        private Effecter progressBar;

        private const int TicksToExtract = 30000;

        private const int NoPowerEjectCumulativeTicks = 60000;

        private static readonly Texture2D CancelIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");
       

        private static readonly SimpleCurve GeneCountChanceCurve = new SimpleCurve
        {
            new CurvePoint(1f, 0.7f),
            new CurvePoint(2f, 0.2f),
            new CurvePoint(3f, 0.08f),
            new CurvePoint(4f, 0.02f)
        };

        private Pawn ContainedPawn => innerContainer.FirstOrDefault() as Pawn;

        //public bool PowerOn => PowerTraderComp.PowerOn;

        public override bool IsContentsSuspended => false;

        private CompRefuelable CompRefuelableComp
        {
            get
            {
                if (cachedComp == null)
                {
                    cachedComp = this.TryGetComp<CompRefuelable>();
                }

                return cachedComp;
            }
        }

        private CompExtractor CompExtractorComp
        {
            get
            {
                if (compExtractor == null)
                {
                    compExtractor = this.TryGetComp<CompExtractor>();
                }

                return compExtractor;
            }
        }



        public Texture2D InsertPawnTex
        {
            get
            {
                if (cachedInsertPawnTex == null)
                {
                    cachedInsertPawnTex = ContentFinder<Texture2D>.Get(CompExtractorComp.evolveIcon());
                }

                return cachedInsertPawnTex;
            }
        }
        public long ticksSinceStart = 0;

        public float HeldPawnDrawPos_Y => DrawPos.y + 0.03846154f;

        public float HeldPawnBodyAngle => base.Rotation.Opposite.AsAngle;

        public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;

        public override Vector3 PawnDrawOffset => IntVec3.West.RotatedBy(base.Rotation).ToVector3() / def.size.x;

        public override void PostPostMake()
        {
            if (!ModLister.CheckBiotech("gene extractor"))
            {
                Destroy();
            }
            else
            {
                base.PostPostMake();
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            sustainerWorking = null;
            if (progressBar != null)
            {
                progressBar.Cleanup();
                progressBar = null;
            }

            base.DeSpawn(mode);
        }

        public string messageReset()
        {
            return "";
        }

        public string message = "";
        public override void Tick()
        {
            amountOfTicks = CompExtractorComp.timeInSeconds() * (int)timeType(compExtractor.ticksSecondsHoursDaysYears());
            CompExtractor c = this.CompExtractorComp;

            base.Tick();
            if (ContainedPawn == null)
            {
                ticksRemaining = amountOfTicks;
                startWorking = false;
            }
            if (ticksRemaining < amountOfTicks)
            {
                Log.Message(amountOfTicks);
                Log.Message(ticksRemaining);
                startWorking = true;
            }
            if (startWorking)
            {
                ticksSinceStart++;
            }

            if (innerContainer == null)
            {
                Log.Error("innerContainer is null in Building_DruidFungiTransformer.Tick()");
                return;
            }

            innerContainer.ThingOwnerTick();

            if (this.IsHashIntervalTick(250))
            {


                // PowerTraderComp.PowerOutput = (base.Working ? (0f - base.PowerComp.Props.PowerConsumption) : (0f - base.PowerComp.Props.idlePowerDraw));
            }

            if (base.Working)
            {
                Pawn ContainedPawn = this.ContainedPawn; 

                if (ContainedPawn == null)
                {
                    Log.Error("ContainedPawn is null in Building_DruidFungiTransformer.Tick() while Working");
                    Cancel();
                    return;
                }

                TickEffects();
                if (CompRefuelableComp != null && CompRefuelableComp.Fuel > 0 && ticksSinceStart > 0 && startWorking)
                {
                    ticksRemaining--;

                    this.message = "";
                    if (ticksSinceStart % 300 == 0)
                    {
                        CompRefuelableComp.ConsumeFuel(CompRefuelableComp.Props.fuelConsumptionRate);

                    }
                }
                else if (CompRefuelableComp == null && startWorking)
                {
                    ticksRemaining--;
                    this.message = "";
                    this.ticksSinceStart = 0;

                }
                else if (CompRefuelableComp != null && CompRefuelableComp.Fuel <= 0 && this.message == "")
                {
                    this.ticksSinceStart = 0;
                    this.startWorking = false;
                    if (whatToDo == "Transform" )
                    {
                        this.message = "Fuel is empty," + this.ContainedPawn.Name + "'s transformation has stopped";
                    }
                    else if (whatToDo == "Extract")
                    {
                        this.message = "Fuel is empty," + this.ContainedPawn.Name + "'s extraction has stopped";
                    }
                    else if (whatToDo == "Evolve")
                    {
                        this.message = "Fuel is empty," + this.ContainedPawn.Name + "'s evolving has stopped";
                    }

                    Messages.Message(message, new LookTargets(ContainedPawn), MessageTypeDefOf.PositiveEvent);

                }

                if (ticksRemaining % (c.extractEveryXSeconds() * 60) == 0 && whatToDo == "Extract" && startWorking)
                {
                    spawnThing(c.extractThing(), c.extractAmount());
                }




                if (ticksRemaining <= 0&&whatToDo!="Extract")
                {
                    Finish();
                }
                else if (ticksRemaining <= 0 && whatToDo == "Extract")
                {
                    ticksRemaining = CompExtractorComp.timeInSeconds() * (int)timeType(compExtractor.ticksSecondsHoursDaysYears());
                }

                    return;
            }

            



            if (selectedPawn != null && selectedPawn.Dead)
            {
                Cancel();
            }

            if (progressBar != null)
            {
                progressBar.Cleanup();
                progressBar = null;
            }
        }

        private void TickEffects()
        {
            if (sustainerWorking == null || sustainerWorking.Ended)
            {
                sustainerWorking = SoundDefOf.GeneExtractor_Working.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
            }
            else
            {
                sustainerWorking.Maintain();
            }

            if (progressBar == null && whatToDo == "Transform" || whatToDo == "Evolve" && progressBar == null)
            {
                progressBar = EffecterDefOf.ProgressBarAlwaysVisible.Spawn();
            }
            if ((progressBar != null && whatToDo == "Transform") || (progressBar != null && whatToDo == "Evolve"))
            {
                progressBar.EffectTick(new TargetInfo(base.Position + IntVec3.North.RotatedBy(base.Rotation), base.Map), TargetInfo.Invalid);
                MoteProgressBar mote = ((SubEffecter_ProgressBar)progressBar.children[0]).mote;
                if (mote != null)
                {
                    mote.progress = 1f - Mathf.Clamp01((float)ticksRemaining / (CompExtractorComp.timeInSeconds() * timeType(CompExtractorComp.ticksSecondsHoursDaysYears())));
                    mote.offsetZ = ((base.Rotation == Rot4.North) ? 0.5f : (-0.5f));
                }
            }
        }



        public override AcceptanceReport CanAcceptPawn(Pawn pawn)
        {
            if (!pawn.IsColonist && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony && (!pawn.IsColonyMutant || !pawn.IsGhoul))
            {
                return false;
            }

            if (selectedPawn != null && selectedPawn != pawn)
            {
                return false;
            }

            if (!pawn.RaceProps.Humanlike || pawn.IsQuestLodger())
            {
                return false;
            }

            //if (pawn.def == ThingDef.Named("Alien_FungiWormi")){
            //  return (pawn.Name + " is a wormoid.");
            //}
            // if(pawn.def == ThingDef.Named("Alien_Fungi"))
            //{
            //  return (pawn.Name + " is a Fungi. Even the worm tribe doesn´t put Fungi in here.");
            //}

            //  if (!PowerOn)
            // {
            //    return "NoPower".Translate().CapitalizeFirst();
            //   }

            if (innerContainer.Count > 0)
            {
                return "Occupied".Translate();
            }

            //  if (pawn.genes == null || !pawn.genes.GenesListForReading.Any((Gene x) => x.def.passOnDirectly))
            // {
            // return "PawnHasNoGenes".Translate(pawn.Named("PAWN"));
            //   }



            return true;
        }

        private void Cancel()
        {
            startTick = -1;
            selectedPawn = null;
            sustainerWorking = null;
            powerCutTicks = 0;
            startWorking = false;
            this.whatToDo = "";
            innerContainer.TryDropAll(def.hasInteractionCell ? InteractionCell : base.Position, base.Map, ThingPlaceMode.Near);



        }
        private void spawnThing(ThingDef thing, int amount)
        {
            CompExtractor c = this.CompExtractorComp;
            IntVec3 intVec = (def.hasInteractionCell ? InteractionCell : base.Position);

            for (int i = 0; i < amount; i++)
            {
                GenSpawn.Spawn(thing, intVec, base.Map);
            }
            DamageInfo dinfo =
                           new DamageInfo(c.extractDamage(), c.extractDamageAmount(), 999, -1.0f, null, null, null);
            dinfo.SetInstantPermanentInjury(false);
            DamageWorker.DamageResult dres = this.ContainedPawn.TakeDamage(dinfo);
            // if (this.Props.damageFilth && this.parent.pawn.Position != null && this.parent.pawn.Map != null)
            // {
            // FilthMaker.TryMakeFilth(this.parent.pawn.Position, this.parent.pawn.Map, this.parent.pawn.RaceProps.BloodDef);
            // }

        }
        private void Finish()
        {


            CompExtractor c = this.CompExtractorComp;

            if (whatToDo == "Transform")
            {
                selectedPawn = null;
                sustainerWorking = null;
                powerCutTicks = 0;

                if (ContainedPawn != null)
                {
                    if (this.whatToDo == "Transform")
                    {
                        Rand.PushState(ContainedPawn.thingIDNumber ^ startTick);
                        Messages.Message("GeneExtractionComplete".Translate(ContainedPawn.Named("PAWN")) + ": " + "was transformed into a " + c.transformPawn().race.label, new LookTargets(ContainedPawn), MessageTypeDefOf.PositiveEvent);

                        wormoidTransformCustom(ContainedPawn, c.transformPawn(), base.Position, base.Map);


                        startWorking = false;
                        ticksSinceStart = 0;
                        Rand.PopState();
                        if (this.whatToDo != "Transform")
                        {
                            innerContainer.TryDropAll(def.hasInteractionCell ? InteractionCell : base.Position, base.Map, ThingPlaceMode.Near);
                        }
                    }

                }
            }
            if (whatToDo == "Evolve")
            {
                if (this.ContainedPawn.health.hediffSet.HasHediff(oldHediff))
                {
                    Hediff oldOne = this.ContainedPawn.health.hediffSet.GetFirstHediffOfDef(oldHediff);
                    BodyPartRecord partOfOld = oldOne.Part;
                    Hediff newOne = HediffMaker.MakeHediff(newHediff, this.ContainedPawn, partOfOld);
                    Log.Message(oldOne);
                    Log.Message(newOne);
                    //this.ContainedPawn.health.RemoveHediff(oldOne);
                    this.ContainedPawn.health.AddHediff(newOne);
                    string letterMessage = ContainedPawn.Label + " has evolved and replaced " + ContainedPawn.Possessive() + " " + oldHediff.label + " with " + newHediff.label;
                    // Messages.Message(ContainedPawn.Label + " has evolved and replaced " + ContainedPawn.Possessive() + " " + oldHediff + " with " + newHediff.label + c.transformPawn().label, new LookTargets(ContainedPawn), MessageTypeDefOf.PositiveEvent);

                    Find.LetterStack.ReceiveLetter("Evolved!", letterMessage, LetterDefOf.PositiveEvent, new LookTargets(ContainedPawn), null, null, null, null, 1, true);





                    //(GenText.CapitalizeFirst(ContainedPawn.Label + " has evolved and replaced " + ContainedPawn.Possessive() + " " + oldHediff + " with " + newHediff.label), ContainedPawn.Label + " has evolved and replaced " + ContainedPawn.Possessive() + " " + oldHediff + " with " + newHediff.label, LetterDefOf.ThreatBig, null, ContainedPawn, new NamedArgument[0]);



                }

            }

            innerContainer.TryDropAll(def.hasInteractionCell ? InteractionCell : base.Position, base.Map, ThingPlaceMode.Near);


            startTick = -1;


        }

        public override void TryAcceptPawn(Pawn pawn)
        {
            if ((bool)CanAcceptPawn(pawn))
            {
                selectedPawn = pawn;
                bool num = pawn.DeSpawnOrDeselect();
                if (innerContainer.TryAddOrTransfer(pawn))
                {
                    startTick = Find.TickManager.TicksGame;
                    ticksRemaining = CompExtractorComp.timeInSeconds() * (int)timeType(compExtractor.ticksSecondsHoursDaysYears());
                }

                if (num)
                {
                    Find.Selector.Select(pawn, playSound: false, forceDesignatorDeselect: false);
                }
            }
        }
        private void SelectHauler(Pawn pawn)
        {
            selectedHauler = pawn;
            if (!pawn.IsPrisonerOfColony && !pawn.Downed)
            {
                if (selectedPawn != selectedHauler)
                    pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.CarryToBuilding, this, selectedPawn), JobTag.Misc);
            }
        }
        protected override void SelectPawn(Pawn pawn)
        {
            
                base.SelectPawn(pawn);
            
        }

        private void startTransform(Pawn pawn)
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("Are you sure you want to transform " + this.ContainedPawn.LabelShort + " into a " + CompExtractorComp.transformPawn().label + "? This cannot be undone. "+CompExtractorComp.transformDescription(), delegate
            {
                this.whatToDo = "Transform";
                this.startWorking = true;
            }));
        }

        private void SelectHediff(HediffDef oldHediff, HediffDef newHediff)
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("Are you sure you want to evolve " + this.ContainedPawn.LabelShort + "? This cannot be undone. "+ CompExtractorComp.evolveDescription(), delegate
            {
                finalSelectHediff(oldHediff, newHediff);
            }));
        }
        private void finalSelectHediff(HediffDef oldOne, HediffDef newOne)
        {

            oldHediff = oldOne;
            newHediff = newOne;
        }






        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (victimOrHaul)
            {
                foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
                {
                    yield return floatMenuOption;
                }

                if (!selPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly))
                {
                    yield return new FloatMenuOption("CannotEnterBuilding".Translate(this) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
                    yield break;
                }

                AcceptanceReport acceptanceReport = CanAcceptPawn(selPawn);
                if (acceptanceReport.Accepted)
                {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("EnterBuilding".Translate(this), delegate
                    {
                        SelectPawn(selPawn);
                    }), selPawn, this);
                }
                else if (selPawn.def != CompExtractorComp.transformPawn().race && base.SelectedPawn == selPawn && !selPawn.IsPrisonerOfColony)
                {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("EnterBuilding".Translate(this), delegate
                    {
                        selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.EnterBuilding, this), JobTag.Misc);
                        //selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.CarryToBuilding, this), JobTag.Misc);
                    }), selPawn, this);
                }

                else if (ContainedPawn == null && base.SelectedPawn != selPawn && !selPawn.IsPrisonerOfColony)
                {
                    // Ensure selectedPawn is not null
                    if (selectedPawn == null)
                    {
                        Log.Error("Selected pawn is null in GetFloatMenuOptions.");
                        yield break;
                    }

                    // Ensure 'this' refers to a valid building and is not null
                    if (this == null)
                    {
                        Log.Error("Building reference is null in GetFloatMenuOptions.");
                        yield break;
                    }

                    // Create the float menu option
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("Carry " + selectedPawn.Label + " to " + this.Label, delegate
                    {
                        // Ensure JobDefOf.CarryToBuilding is valid
                        if (JobDefOf.CarryToBuilding == null)
                        {
                            Log.Error("JobDefOf.CarryToBuilding is null.");
                            return;
                        }

                        // Create the job
                        Job haulJob = JobMaker.MakeJob(JobDefOf.CarryToBuilding, this, selectedPawn);

                        // Ensure haulJob is not null
                        if (haulJob == null)
                        {
                            Log.Error("Failed to create haulJob in GetFloatMenuOptions.");
                            return;
                        }

                        haulJob.count = 1;

                        // Ensure selPawn.jobs is valid
                        if (selPawn.jobs == null)
                        {
                            Log.Error("selPawn.jobs is null in GetFloatMenuOptions.");
                            return;
                        }

                        // Try to take the ordered job
                        selPawn.jobs.TryTakeOrderedJob(haulJob, JobTag.Misc);
                    }), selPawn, this);
                }

                else if (!acceptanceReport.Reason.NullOrEmpty())
                {
                    yield return new FloatMenuOption("CannotEnterBuilding".Translate(this) + ": " + acceptanceReport.Reason.CapitalizeFirst(), null);
                }
            }
        }



        public override IEnumerable<Gizmo> GetGizmos()
        {
            Texture2D transformIcon = ContentFinder<Texture2D>.Get(CompExtractorComp.transformIcon());
            Texture2D evolveIcon = ContentFinder<Texture2D>.Get(CompExtractorComp.evolveIcon());
            Texture2D extractIcon = ContentFinder<Texture2D>.Get(CompExtractorComp.extractIcon());
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            bool pawnCheck = ContainedPawn != null;

            if (pawnCheck && base.Working && startWorking == false)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "Cancel";
                command_Action.defaultDesc = "Cancel";
                command_Action.icon = CancelIcon;
                command_Action.action = delegate
                {
                    Cancel();
                };

                command_Action.activateSound = SoundDefOf.Designate_Cancel;
                yield return command_Action;
            }
            if (base.Working && startWorking == true && whatToDo == "Evolve")
            {
                Command_Action command_ActionC = new Command_Action();
                command_ActionC.defaultLabel = "Cancel evolving";
                command_ActionC.defaultDesc = "Cancel evolving";
                command_ActionC.icon = CancelIcon;
                command_ActionC.action = delegate
                {
                    Cancel();
                };

                command_ActionC.activateSound = SoundDefOf.Designate_Cancel;
                yield return command_ActionC;
            }

            if (base.Working && startWorking == true && whatToDo == "Transform")
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "Cancel transformation";
                command_Action.defaultDesc = "Cancel transformation";
                command_Action.icon = CancelIcon;
                command_Action.action = delegate
                {
                    Cancel();
                };

                command_Action.activateSound = SoundDefOf.Designate_Cancel;
                yield return command_Action;

            

                    if (DebugSettings.ShowDevGizmos)
                {
                    Command_Action command_Action2 = new Command_Action();
                    command_Action2.defaultLabel = "DEV: Finish transform";
                    command_Action2.action = delegate
                    {
                        Finish();
                    };
                    yield return command_Action2;
                }

                yield break;
            }

            if (base.Working && startWorking == true && whatToDo == "Extract")
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "Cancel extraction";
                command_Action.defaultDesc = "Cancel extraction";
                command_Action.icon = CancelIcon;
                command_Action.action = delegate
                {
                    Cancel();
                };
                command_Action.activateSound = SoundDefOf.Designate_Cancel;
                yield return command_Action;

                if (DebugSettings.ShowDevGizmos)
                {
                    Command_Action command_Action2 = new Command_Action();
                    command_Action2.defaultLabel = "DEV: Finish transform";
                    command_Action2.action = delegate
                    {
                        Finish();
                    };
                    yield return command_Action2;
                }

                yield break;
            }

            if (selectedPawn != null && this.ContainedPawn == null)
            {
                Command_Action command_Action3 = new Command_Action();
                command_Action3.defaultLabel = "CommandCancelLoad".Translate();
                command_Action3.defaultDesc = "CommandCancelLoadDesc".Translate();
                command_Action3.icon = CancelIcon;
                command_Action3.activateSound = SoundDefOf.Designate_Cancel;
                command_Action3.action = delegate
                {
                    innerContainer.TryDropAll(base.Position, base.Map, ThingPlaceMode.Near);
                    if (selectedPawn.CurJobDef == JobDefOf.EnterBuilding)
                    {
                        selectedPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }

                    selectedPawn = null;
                    startTick = -1;
                    sustainerWorking = null;
                    startWorking = false;
                };
                yield return command_Action3;
            }

            if (base.Working && startWorking == false)
            {
                if (CompExtractorComp.canTransform())
                {
                    Command_Action command_Action5 = new Command_Action();
                    command_Action5.defaultLabel = "start transformation";
                    command_Action5.defaultDesc = "start";

                    if (transformIcon == null)
                    {
                        Verse.Log.Error("Icon is null, you amateur.");
                    }
                    else
                    {
                        command_Action5.icon = transformIcon;
                    }

                    command_Action5.activateSound = SoundDefOf.Designate_Cancel;
                    command_Action5.action = delegate
                    {
                        startTransform(ContainedPawn);
                    };
                    yield return command_Action5;
                }

                // Second Gizmo
                if (CompExtractorComp.canExtract())
                {
                    Command_Action command_Action6 = new Command_Action();
                    command_Action6.defaultLabel = "start extracting";
                    command_Action6.defaultDesc = "start extracting";

                    if (extractIcon == null)
                    {
                        Verse.Log.Error("Icon is null, you amateur");
                    }
                    else
                    {
                        command_Action6.icon = extractIcon;
                    }

                    command_Action6.activateSound = SoundDefOf.Designate_Cancel;
                    command_Action6.action = delegate
                    {
                        this.whatToDo = "Extract";
                        this.startWorking = true;
                    };
                    yield return command_Action6;
                }
            }

            // Command Insert
            Command_Action command_Insert = new Command_Action();
            command_Insert.defaultLabel = "Select Pawn";
            command_Insert.defaultDesc = "Select the pawn that should be put into the " + this.Label;
            command_Insert.icon = InsertPawnTex;
            command_Insert.action = delegate
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (Pawn item in base.Map.mapPawns.AllPawnsSpawned)
                {
                    Pawn pawn = item;
                    if (pawn.genes != null)
                    {
                        AcceptanceReport acceptanceReport = CanAcceptPawn(pawn);
                        string text = pawn.LabelShortCap + ", " + pawn.genes.XenotypeLabelCap + "," + pawn.Label;
                        if (!acceptanceReport.Accepted)
                        {
                            if (!acceptanceReport.Reason.NullOrEmpty())
                            {
                                list.Add(new FloatMenuOption(text + ": " + acceptanceReport.Reason, null, pawn, Color.white));
                            }
                        }
                        else
                        {
                            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
                            if (firstHediffOfDef != null)
                            {
                                text = text + " (" + firstHediffOfDef.LabelBase + ", " + firstHediffOfDef.TryGetComp<HediffComp_Disappears>().ticksToDisappear.ToStringTicksToPeriod(allowSeconds: true, shortForm: true).Colorize(ColoredText.SubtleGrayColor) + ")";
                            }

                            list.Add(new FloatMenuOption(text, delegate
                            {
                                SelectPawn(pawn);
                            }, pawn, Color.white));
                        }
                    }
                }

                if (!list.Any())
                {
                    list.Add(new FloatMenuOption("No fitting pawn", null));
                }

                Find.WindowStack.Add(new FloatMenu(list));
            };

            yield return command_Insert;

            if (ContainedPawn != null && CompExtractorComp.canEvolve())
            {
                Command_Action command_Insert2 = new Command_Action();
                command_Insert2.defaultLabel = "Select Mutagen";
                command_Insert2.defaultDesc = "Select the pawn that should be put into the " + this.Label;
                command_Insert2.icon = InsertPawnTex;
                command_Insert2.action = delegate
                {
                    List<FloatMenuOption> listTwo = new List<FloatMenuOption>();
                    List<HediffDef> oldList = new List<HediffDef>();
                    List<HediffDef> newList = new List<HediffDef>();
                    oldList = CompExtractorComp.oldHediffs();
                    newList = CompExtractorComp.newHediffs();
                    oldList.ForEach((HediffDef mystring) =>
                    {

                        if (selectedPawn.health.hediffSet.HasHediff(mystring))
                        {
                            listTwo.Add(new FloatMenuOption("Evolve " + mystring.label + " into: " + newList[oldList.IndexOf(mystring)].label + " for: " + this.ContainedPawn.Label, delegate
                            {
                                SelectHediff(mystring, newList[oldList.IndexOf(mystring)]);
                                this.whatToDo = "Evolve";
                                this.startWorking = true;
                            }, this.ContainedPawn, Color.white));
                        }

                        else if (!selectedPawn.health.hediffSet.HasHediff(mystring))
                        {
                            listTwo.Add(new FloatMenuOption(mystring.label + " not present on: " + this.ContainedPawn.Label, null, this.ContainedPawn, Color.red));
                        }
                    });

                    if (!listTwo.Any())
                    {
                        listTwo.Add(new FloatMenuOption("No fitting Mutagen", null));
                    }

                    Find.WindowStack.Add(new FloatMenu(listTwo));
                };

                yield return command_Insert2;
            }


        }





        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            base.DynamicDrawPhaseAt(phase, drawLoc, flip);
            if (base.Working && ContainedPawn != null)
            {
                ContainedPawn.Drawer.renderer.DynamicDrawPhaseAt(phase, drawLoc + PawnDrawOffset, null, neverAimWeapon: true);
            }
        }

        public override string GetInspectString()
        { //Anzeige
            StringBuilder sb = new StringBuilder(base.GetInspectString());

            if (selectedPawn != null && innerContainer.Count == 0)
            {
                if (sb.Length > 0)
                {
                    sb.Append("\n");
                }

                sb.Append("WaitingForPawn".Translate(selectedPawn.Named("PAWN")).Resolve());
            }
            else if (base.Working && ContainedPawn != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append("\n");
                }
                if (whatToDo == "Transform")
                {
                    sb.Append("Transforming Pawn " + ContainedPawn.Name);

                    if (sb.Length > 0)
                    {
                        sb.Append("\n");
                    }
                }
                if (whatToDo == "Evolve")
                {
                    sb.Append("Evolving Pawn " + ContainedPawn.Name);

                    if (sb.Length > 0)
                    {
                        sb.Append("\n");
                    }
                }

                else if (whatToDo == "Extract")
                {
                    sb.Append("Growing " + this.compExtractor.extractThing().label + " in " + ContainedPawn.Name);

                    if (sb.Length > 0)
                    {
                        sb.Append("\n");
                    }

                }
                if (whatToDo == "Transform" || whatToDo == "Evolve")
                {
                    string remainingTime = RoundToOneDecimalPlace((ticksRemaining / timeType(CompExtractorComp.ticksSecondsHoursDaysYears()))) + "";

                  
                    sb.Append(CompExtractorComp.ticksSecondsHoursDaysYears() + " remaining:" + remainingTime);
                    if (sb.Length > 0)
                    {
                        sb.Append("\n");
                    }
                }
                if (!string.IsNullOrEmpty(this.message))
                {
                    sb.Append(this.message);

                }
            }

            return sb.ToString().Trim();
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
            Scribe_Values.Look<string>(ref whatToDo, "whatToDo", "");
            Scribe_Defs.Look<HediffDef>(ref oldHediff, "oldHediff");
            Scribe_Defs.Look<HediffDef>(ref newHediff, "newHediff");
            Scribe_Values.Look<string>(ref testScribe, "testScribe", "Here could be your value, lol");
        }


        public static float timeType(string TimeTypeAsWord)
        {
            int time = 1;
            switch (TimeTypeAsWord.ToLower())
            {
                case "seconds":
                    time = 60;
                    break;
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
            return time;
        }
        public static double RoundToOneDecimalPlace(double value)
        {
            return (double)((int)(value * 10 + 0.5)) / 10;
        }

        public static void wormoidTransformCustom(Pawn victim, PawnKindDef pawnAfterTransform, IntVec3 position, Map map, bool berzerk = false, bool warcall = false)
        {

            PawnKindDef pawnToSpawn = pawnAfterTransform;

            PawnGenerationRequest request = new PawnGenerationRequest(pawnToSpawn, victim.Faction, PawnGenerationContext.NonPlayer, -1, false, false, true, true, false, 0f, false, true, false, false, false, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, victim.DevelopmentalStage, null, null, null, false);
            Pawn worm = PawnGenerator.GeneratePawn(request);

            worm.ageTracker.AgeBiologicalTicks = victim.ageTracker.AgeBiologicalTicks;
            worm.ageTracker.AgeChronologicalTicks = 0;

            if (victim.RaceProps.Humanlike)
            {
                worm.relations.ClearAllNonBloodRelations();
                worm.relations.ClearAllRelations();

                worm.Name = victim.Name;
                if (victim.gender == Gender.Female)
                { worm.SetMother(victim); }
                else if (victim.gender == Gender.Male)
                {
                    worm.SetFather(victim);
                }
                worm.gender = victim.gender;
                worm.story.bodyType = victim.story.bodyType;
                worm.Drawer.renderer.SetAllGraphicsDirty();
                worm.apparel.DestroyAll();
                victim.equipment.DropAllEquipment(position);
                victim.apparel.DropAll(position);
            }



            GenSpawn.Spawn(worm, position, map);


            GenExplosion.DoExplosion(position,
         map,
         2,
         DamageDefOf.Bite,
         victim,
         0,
         0,
         null,
         null, null, null,
         victim.RaceProps.BloodDef, 1, 1,
         null, false, null, 0, 0, 0, true, null, null, null, true, 0.6f, 0, false, null, 1);

            if (berzerk)
            {
                if (warcall)
                {
                   // FungiUtility.TryGiveMentalState(MentalStateDefOf.BerserkWarcall, worm, Fungi.FungiUtility.fungiFaction(worm, "Alien_Fungi"), false);
                }
                else if (!warcall)
                {
                    worm.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, null, false, false, false, null);
                }

            }


            victim.Destroy(DestroyMode.KillFinalize);




        }
    }

}

#if false // Dekompilierungsprotokoll
44 Elemente im Cache
------------------
Auflösen: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Einzelne Assembly gefunden: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Laden von: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll
------------------
Auflösen: UnityEngine.IMGUIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: UnityEngine.IMGUIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.IMGUIModule.dll
------------------
Auflösen: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.CoreModule.dll
------------------
Auflösen: UnityEngine.TextRenderingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: UnityEngine.TextRenderingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.TextRenderingModule.dll
------------------
Auflösen: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Einzelne Assembly gefunden: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Laden von: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Core.dll
------------------
Auflösen: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Einzelne Assembly gefunden: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Laden von: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll
------------------
Auflösen: NAudio, Version=1.7.3.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: NAudio, Version=1.7.3.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\NAudio.dll
------------------
Auflösen: NVorbis, Version=0.8.4.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: NVorbis, Version=0.8.4.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\NVorbis.dll
------------------
Auflösen: UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: com.rlabrecque.steamworks.net, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: com.rlabrecque.steamworks.net, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\com.rlabrecque.steamworks.net.dll
------------------
Auflösen: System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Einzelne Assembly gefunden: System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Laden von: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Xml.dll
------------------
Auflösen: Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp-firstpass.dll
------------------
Auflösen: System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Der Name "System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" wurde nicht gefunden.
------------------
Auflösen: Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.AssetBundleModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.AssetBundleModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: Unity.Mathematics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "Unity.Mathematics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: ISharpZipLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: ISharpZipLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\ISharpZipLib.dll
------------------
Auflösen: UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.PerformanceReportingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.PerformanceReportingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.ImageConversionModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.ImageConversionModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.ScreenCaptureModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.ScreenCaptureModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.UI.dll
#endif
