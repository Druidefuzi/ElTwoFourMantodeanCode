using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Mantodean
{
    /*
     * Fills the Scarabs tab with one work-priority column per work type the scarab race declares.
     *
     * Vanilla's PawnColumnDefGenerator already creates a PawnColumnDef named
     * "WorkPriority_<WorkTypeDef>" for every visible work type and inserts it into the vanilla
     * Work table. That happens in DefGenerator.GenerateImpliedDefs_PreResolve, so by the time a
     * StaticConstructorOnStartup runs, every one of them is in the DefDatabase and can simply be
     * reused. Sharing a PawnColumnDef between two tables is safe: the worker is stored per column
     * def and every method that draws takes the PawnTable as an argument.
     *
     * Building the list here rather than hardcoding it in XML keeps RaceProps.mechEnabledWorkTypes
     * as the single source of truth. Adding <li>Doctor</li> to the race def is now enough to get a
     * Doctor column in this tab, with no code change.
     */
    [StaticConstructorOnStartup]
    public static class ScarabTableColumns
    {
        static ScarabTableColumns()
        {
            PawnTableDef table = MantodeanDefOf.Manto_ScarabWork;
            if (table?.columns == null)
            {
                return;
            }

            // The union over every race that uses Pawn_Housekeeper, rather than the scarab's list
            // alone. A second worker animal with a different whitelist then gets its own columns
            // without anyone touching this file; the per-pawn filtering still happens in
            // JobGiver_Housekeeper.PawnCanUseWorkGiver, which asks each pawn's own race.
            HashSet<WorkTypeDef> enabled = new HashSet<WorkTypeDef>();
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (def.thingClass != typeof(Pawn_Housekeeper) || def.race?.mechEnabledWorkTypes == null)
                {
                    continue;
                }

                foreach (WorkTypeDef w in def.race.mechEnabledWorkTypes)
                {
                    enabled.Add(w);
                }
            }

            if (enabled.Count == 0)
            {
                return;
            }

            // Keep RemainingSpace last - it is the filler that pushes the table to full width.
            int insertAt = table.columns.FindIndex(c => c.defName == "RemainingSpace");
            if (insertAt < 0)
            {
                insertAt = table.columns.Count;
            }

            // No obedience column here on purpose. It was added while
            // L24_ThinkTree_Scarab still gated JobGiver_Housekeeper behind
            // ThinkNode_ConditionalTrainableCompleted, to make that gate visible next to the
            // priorities it silently disabled. The gate has since been removed at the author's
            // request, so showing obedience in a work tab would now imply a dependency that no
            // longer exists. It is still on the Animals tab, where it belongs.

            // WorkTypeDefsInPriorityOrder is the same ordering the vanilla Work tab uses, so the
            // columns line up with what the player already knows.
            foreach (WorkTypeDef w in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder)
            {
                if (!w.visible || !enabled.Contains(w))
                {
                    continue;
                }

                PawnColumnDef col = DefDatabase<PawnColumnDef>.GetNamedSilentFail("WorkPriority_" + w.defName);
                if (col == null || table.columns.Contains(col))
                {
                    continue;
                }

                table.columns.Insert(insertAt, col);
                insertAt++;
            }
        }
    }
}
