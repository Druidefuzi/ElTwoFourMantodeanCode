using System;
using Verse;
using RimWorld;

namespace L24
{
	// Token: 0x0200000E RID: 14
	public class HediffCompProperties_DoSeverityStuff : HediffCompProperties
	{
		// Token: 0x06000024 RID: 36 RVA: 0x000032D2 File Offset: 0x000014D2
		public HediffCompProperties_DoSeverityStuff()
		{
			this.compClass = typeof(HediffComp_DoSeverityStuff);
		}

		// Token: 0x04000012 RID: 18
		public static DamageDef Frostbite;

		// Token: 0x04000014 RID: 20







		public string lowerOrHigher = "Higher";
		public float doAtSeverity = 1;

		public Boolean getsDamage = false;
		public int amountDamage = 0;
		public DamageDef damageType;

		public Boolean getsResearch = false;
		public int amountOfResearch = 1;
		public Boolean canSpawnThing = false;
		public ThingDef thingDefToSpawn;
		public int amountOfThings = 2;


		public Boolean canSpawnPawn = false;
		public int amountOfPawns = 2;
		public PawnKindDef pawnToSpawn;
		public FactionDef factionOfPawn;


		public Boolean getsOlder = false;
		public string olderOrYounger = "Older";
		public string yearsOrDaysOrTicks = "Ticks";
		public long timeOlder = 0;

		public Boolean getsKilled = false;
		public float getsKilledAfterXTime = 0;


		public Boolean addHeddif = false;
		public HediffDef hediffToAdd;
		public BodyPartRecord bodyPartForHediff;
		public Boolean gasExplosion = false;
		public Boolean doExplosion = false;
		public Boolean getsKilledInExplosion = false;
		public Boolean corpseDestroyedAfterkill = false;
		public DamageDef explosionDamageDef;
		public ThingDef filthOrBloodToSpawn;
		public int explosionRadius = 5;
		public int armorPenetrationExplosion = 0;
		public int damageAmountExplosion = 1;
		public SoundDef explosionSound;
		public GasType gasType;

		public Boolean removeThis = false;



	}
}
