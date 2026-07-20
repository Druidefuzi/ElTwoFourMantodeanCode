using RimWorld;
using Verse;

namespace Mantodean.Druid.Bullets
{
	
	public class Bullet_DelayedHatcher : Bullet
	{
		
		protected override void Impact(Thing hitThing, bool blockedByShield = false)
		{
			
			GenSpawn.Spawn(ThingDef.Named("L24_Egg_BulletDelayedHatcher"), Position, Map, WipeMode.Vanish);
			Destroy(0);
		}

	}
}
