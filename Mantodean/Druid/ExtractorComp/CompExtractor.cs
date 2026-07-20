using System.Collections.Generic;
using Verse;

namespace Mantodean.Druid.ExtractorComp
{
    [StaticConstructorOnStartup]
    public class CompExtractor : ThingComp
    {

        public CompProperties_Extractor Props
        {
            get { return (CompProperties_Extractor)props; }
        }
        // public CompProperties_Extractor Props => (CompProperties_Extractor)props;

        public PawnKindDef transformPawn() { return Props.transformPawn; }
        public ThingDef extractThing() { return Props.extractThing; }

        public int extractAmount() { return Props.extractAmount; }

        public int extractDamageAmount() { return Props.extractDamageAmount; }

        public DamageDef extractDamage() { return Props.extractDamage; }

        public int extractEveryXSeconds() { return Props.extractEveryXSeconds; }

        public int timeInSeconds() { return Props.finishInXTime; }
        public List<HediffDef> oldHediffs(){ return Props.oldHediffs; }

        public List<HediffDef> newHediffs() { return Props.newHediffs; }

        public bool canTransform() { return Props.canTransform; }
        public bool canExtract() { return Props.canExtract; }
        public bool canEvolve() { return Props.canEvolve; }
        public string ticksSecondsHoursDaysYears() { return Props.ticksSecondsHoursDaysYears; }

        public string evolveIcon() { return Props.selectIconPath; }
        public string transformIcon() { return Props.transformIconPath; }
        public string extractIcon() { return Props.extractIconPath; }

        public string transformDescription() { return Props.transformDescription; }

        public string evolveDescription() { return Props.evolveDescription; }


    }
}