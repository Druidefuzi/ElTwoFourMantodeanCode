using System;
using RimWorld;
using Verse;

namespace L24
{
	
	public class Bullet_DelayedHatcher : Bullet
	{
		
		protected override void Impact(Thing hitThing, bool blockedByShield = false)
		{
			
			GenSpawn.Spawn(ThingDef.Named("L24_Egg_BulletDelayedHatcher"), base.Position, base.Map, WipeMode.Vanish);
			this.Destroy(0);
		}


		
	}
}
