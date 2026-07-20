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
            ThingDef scarab = MantodeanDefOf.L24_Animal_Scarab;

            List<WorkTypeDef> enabled = scarab?.race?.mechEnabledWorkTypes;
            if (table?.columns == null || enabled.NullOrEmpty())
            {
                return;
            }

            // Keep RemainingSpace last - it is the filler that pushes the table to full width.
            int insertAt = table.columns.FindIndex(c => c.defName == "RemainingSpace");
            if (insertAt < 0)
            {
                insertAt = table.columns.Count;
            }

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
