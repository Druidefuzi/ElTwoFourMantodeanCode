using RimWorld;
using Verse;

namespace Mantodean.Druid.Bullets
{
    // Token: 0x02000003 RID: 3
    public class Bullet_Hatcher : Bullet
    {
        // Token: 0x06000002 RID: 2 RVA: 0x00002064 File Offset: 0x00000264
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
		{
			
			GenSpawn.Spawn(ThingDef.Named("L24_Egg_BulletHatcher"), Position, Map, WipeMode.Vanish);
			Destroy(0);
		}
	}
}
