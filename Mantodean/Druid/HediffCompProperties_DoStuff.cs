using Verse;
using RimWorld;

namespace Mantodean.Druid
{
	// Token: 0x0200000E RID: 14
	public class HediffCompProperties_DoStuff : HediffCompProperties
	{
		// Token: 0x06000024 RID: 36 RVA: 0x000032D2 File Offset: 0x000014D2
		public HediffCompProperties_DoStuff()
		{
            compClass = typeof(HediffComp_DoStuff);
		}

		// Token: 0x04000012 RID: 18
		public static DamageDef Frostbite;

		// Token: 0x04000014 RID: 20
		public string timeType = "Ticks";

		public int damageEveryXTime = 300;

		public int researchEveryXTime = 300;

		public int spawnThingEveryXTime = 300;

		public int spawnPawnEveryXTime = 300;

		public int getsOlderEveryXTime = 300;

		public int addsHediffAfterXTime = 300;

		public int explodesEveryXTime = 300;


		public bool getsDamage = false;
		public int amountDamage = 0;
		public DamageDef damageType;

		public bool getsResearch = false;
		public int amountOfResearch = 1;
		public bool canSpawnThing = false;
		public ThingDef thingDefToSpawn;
		public int amountOfThings = 2;


		public bool canSpawnPawn = false;
		public int amountOfPawns = 2;
		public PawnKindDef pawnToSpawn;
		public FactionDef factionOfPawn;


		public bool getsOlder = false;
		public string olderOrYounger = "Older";
		public string yearsOrDaysOrTicks = "Ticks";
		public long timeOlder = 0;

		public bool getsKilled = false;
		public float getsKilledAfterXTime = 0;


		public bool addHeddif = false;
		public HediffDef hediffToAdd;
		public BodyPartRecord bodyPartForHediff;

		public bool doExplosion = false;
		public bool gasExplosion = false;
		public DamageDef explosionDamageDef;
		public ThingDef filthOrBloodToSpawn;
		public bool getsKilledInExplosion = false;
		public int explosionRadius = 5;
		public int armorPenetrationExplosion = 0;
		public int damageAmountExplosion = 1;
		public SoundDef explosionSound;
		public GasType gasType;

		public bool removeThis = false;
		public int removeAfterXTime = 300;


	}
}
