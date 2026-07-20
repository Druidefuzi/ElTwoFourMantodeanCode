using RimWorld;
using Verse;

namespace Mantodean
{
	// Token: 0x02000003 RID: 3
	[DefOf]
	public static class MantodeanDefOf
	{
		// These two ThingDefs live in Compatibility/Biotech/1.6, which LoadFolders.xml only mounts
		// under <li IfModActive="Ludeon.Rimworld.Biotech">. Without Biotech they genuinely do not
		// exist, and DefOfHelper.BindDefsFor only skips a field when it carries MayRequire - so
		// every Biotech-less start logged a red "Could not find ThingDef named ..." for both.
		// ModsConfig.IsActive lowercases the id before the lookup, so the casing here is not
		// load-bearing.

		// Token: 0x04000001 RID: 1
		[MayRequire("ludeon.rimworld.biotech")]
		public static ThingDef L24_Manto_SchoolDesk;

		// Token: 0x04000002 RID: 2
		[MayRequire("ludeon.rimworld.biotech")]
		public static ThingDef L24_Manto_Blackboard;

		public static ThingDef L24_Animal_Scarab;

    }
}
