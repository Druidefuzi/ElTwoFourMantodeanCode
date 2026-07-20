using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Mantodean
{
    /*
     * A work tab for scarabs.
     *
     * Scarabs are Intelligence.Animal, so they never appear in the vanilla Work tab - that one
     * lists Find.CurrentMap.mapPawns.FreeColonists. The player therefore had no way to say which
     * scarab should clean and which should haul, which is the whole "make cleaning easier"
     * problem: the only lever was obedience training, and that decays.
     *
     * Nothing here reimplements the work UI. MainTabWindow_PawnTable builds a PawnTable from a
     * PawnTableDef, and the WorkPriority_<WorkTypeDef> columns vanilla generates for its own Work
     * tab are ordinary PawnColumnDefs sitting in the DefDatabase. ScarabTableColumns pulls the
     * relevant ones into this table at startup, so the priority boxes, the tooltips, the
     * shift-click column fill and the manual-priority mode all behave exactly as they do for
     * colonists.
     */
    public class MainTabWindow_Scarabs : MainTabWindow_PawnTable
    {
        protected override PawnTableDef PawnTableDef => MantodeanDefOf.Manto_ScarabWork;

        protected override IEnumerable<Pawn> Pawns
        {
            get
            {
                Map map = Find.CurrentMap;
                if (map == null)
                {
                    return Enumerable.Empty<Pawn>();
                }

                return map.mapPawns.PawnsInFaction(Faction.OfPlayer).Where(IsListable);
            }
        }

        /**
         * Former humans (Pawnmorpher) keep their own story and work settings and are handled by
         * the normal colonist UI, so listing them here would show the same pawn in two tabs.
         */
        public static bool IsListable(Pawn p)
        {
            return p is Pawn_Housekeeper housekeeper
                && !housekeeper.Dead
                && !housekeeper.IsFormerHuman()
                && housekeeper.workSettings != null;
        }
    }
}
