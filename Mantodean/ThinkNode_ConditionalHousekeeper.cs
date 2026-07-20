using Verse;
using Verse.AI;

namespace Mantodean
{
    /*
     * Passes for any pawn whose race uses Mantodean.Pawn_Housekeeper as its thingClass.
     *
     * This replaces the ThinkNode_ConditionalPawnKind that named L24_Animal_Scarab explicitly.
     * The insert tag is Animal_PreWander, which every animal think tree passes through, so
     * SOMETHING has to narrow it down - but naming one pawn kind meant a second worker animal
     * needed a second ThinkTreeDef, and a third needed a third.
     *
     * Keying on the class instead makes adding a worker animal a pure XML job: give the race
     * Mantodean.Pawn_Housekeeper as its thingClass and list what it may do in
     * race/mechEnabledWorkTypes. Nothing else - no code, no extra think tree, no DefOf entry.
     * Pawn_Housekeeper.ApplyEnabledWorkTypes already reads the whitelist off whichever race the
     * pawn actually belongs to, and MainTabWindow_Scarabs already lists by class rather than by
     * kind, so both of those were generic from the start.
     *
     * Former humans (Pawnmorpher) are excluded for the same reason they are excluded from the
     * tab: they keep their own story and work settings and are driven by the colonist think tree.
     */
    public class ThinkNode_ConditionalHousekeeper : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn is Pawn_Housekeeper housekeeper && !housekeeper.IsFormerHuman();
        }
    }
}
