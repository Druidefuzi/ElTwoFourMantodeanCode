using System;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace Mantodean.Druid.EggHatcher_Druid
{
	// Token: 0x0200140C RID: 5132
	public class CompProperties_CustomHatcher : CompProperties
	{
		// Token: 0x06007EAC RID: 32428 RVA: 0x002B7B37 File Offset: 0x002B5D37
		public CompProperties_CustomHatcher()
		{
			compClass = typeof(CompCustomHatcher);
		}

		// Token: 0x04004737 RID: 18231
		public float hatcherDaystoHatch = 1f;
		public List<PawnKindDef> thingsToHatch = new List<PawnKindDef>();
		public List<string> partsToAffect = new List<string>();

		public List<HediffDef> hediffs = new List<HediffDef>();

		public bool addHediffs = false;
		public int amount = 1;
		public bool spawnsPawn = true;
		public DevelopmentalStage developmentStage = DevelopmentalStage.Adult;
		public bool changeAge = false;
		public long ageInYears = 0;
		public bool spawnsFilth = false;
		public int amountOfFilth = 1;
		public ThingDef filthDef;
		public ThingDef thingToSpawn;
		public bool spawnsThings = false;
		public int amountOfThings = 1;
		public bool changeFaction = false;
		public string factionForPawn;
		public bool berzerkAfterHatch = false;
		public bool doExplosion = false;
		public bool getsKilledInExplosion = false;
		public bool gasExplosion = false;
		public DamageDef explosionDamageDef;
		public ThingDef filthOrBloodToSpawn;
		public int explosionRadius = 5;
		public int armorPenetrationExplosion = 0;
		public int damageAmountExplosion = 1;
		public SoundDef explosionSound;
		public GasType gasType;
		public bool destroyPawn = false;
		public BackstoryDef adult;
		public BackstoryDef child;
		[MayRequireAnomaly]
		public MutantDef mutantType;

		public PawnKindDef GetRandomThingToHatch()
		{
			if (thingsToHatch.Count == 0)
			{
				return null; // Rückgabe null, wenn die Liste leer ist
			}

			Random rand = new Random();
			int randomIndex = rand.Next(0, thingsToHatch.Count); // Zufälliger Index von 0 bis zur Anzahl der Elemente - 1
			return thingsToHatch[randomIndex]; // Rückgabe des zufälligen Elements
		}

		public int lengthOfList()
		{
			if (thingsToHatch.Count > 0)

				return thingsToHatch.Count;
			else
			{
				Log.Error("You didnt enter any pawnKind");
				return 0;
			}
		}

		// Token: 0x04004738 RID: 18232
		public PawnKindDef hatcherPawn;
	}
}
