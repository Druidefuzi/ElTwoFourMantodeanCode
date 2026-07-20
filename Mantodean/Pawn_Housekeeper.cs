using Verse;
using RimWorld;
using System.Collections.Generic;

namespace Mantodean
{
    /*
     * Housekeeper cats are some kind of intermediate between animal and humanlike. They have full-fledged cleaning and hauling work givers instead of stubs other animals have. They would be able to do all relevant jobs, like refueling!
     * 
     * But they are still not fully sapient, so you need to teach them what they have to do and keep them updated on it, hence necessary training.
     * 
     * TODO: maybe handle disabled work with a backstory?
     */
    public class Pawn_Housekeeper : Pawn
    {

        //public new bool IsColonyMech => base.Faction == Faction.OfPlayer && MentalStateDef == null;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (IsFormerHuman())
                return; // former humans have their own logic, ignore them

            if (skills == null)
            {
                // Can avoid this by making them using mech code, but it may require way more work so this hack would do
                skills = new Pawn_SkillTracker(this);
                foreach (SkillRecord skill in skills.skills) // to make skills neutral for price factor
                {
                    skill.Level = 6;
                }
            }

            if (story == null)
            {
                // FIXME: is this still necessary?
                story = new Pawn_StoryTracker(this) // necessary for job giver to work properly, but adds a bunch of problems since only humanlikes are supposed to have it
                {
                    bodyType = BodyTypeDefOf.Thin,
                    //crownType = CrownType.Average,
                    //childhood = xxx,
                    //adulthood = xxx
                };
            }

            if (workSettings == null) // only used for WorkGiversInOrderNormal / WorkGiversInOrderEmergency
            {
                workSettings = new Pawn_WorkSettings(this);
                workSettings.EnableAndInitialize();

                ApplyEnabledWorkTypes();

                // both genders can do both cleaning and hauling, but males prefer hauling and females prefer cleaning so they divide jobs and don't neglect one or another too much
                if (gender == Gender.Female)
                    workSettings.SetPriority(WorkTypeDefOf.Hauling, 4);
            }

            GetDisabledWorkTypes(); // init stuff
            workSettings.Notify_DisabledWorkTypesChanged();
        }

        /**
         * Makes RaceProps.mechEnabledWorkTypes actually decide what this pawn does.
         *
         * It did not before, and the reason is subtle. Pawn_WorkSettings.EnableAndInitialize
         * assigns priority 3 to every WorkTypeDef that is alwaysStartActive, plus the first six
         * of the rest sorted by AverageOfRelevantSkillsFor. SpawnSetup sets every skill to 6, so
         * that sort is a tie and the "first six" are simply the first six in DefDatabase order:
         * Doctor, Childcare, Warden, Handling, Cooking, Hunting. Mining and PlantCutting are
         * further down the list and never get a priority at all.
         *
         * That matters because Pawn_WorkSettings.CacheWorkGiversInOrder only puts a work type
         * into WorkGiversInOrderNormal when GetPriority(w) > 0, and WorkGiversInOrderNormal is
         * exactly what JobGiver_Housekeeper iterates. So the two work types the race actually
         * declares could never be reached, while six it does not declare were scanned every time
         * and then discarded one step later by PawnCanUseWorkGiver.
         *
         * EnableAndInitialize does have a mechEnabledWorkTypes branch, but it is guarded by
         * ModsConfig.BiotechActive && pawn.RaceProps.IsMechanoid, and a scarab is neither. The
         * GetDisabledWorkTypes override below cannot help either: it is declared `new` over a
         * non-virtual Pawn method, so vanilla calling through a Pawn reference always gets the
         * base version. Rather than fight that, set the priorities explicitly and let the XML be
         * the single source of truth.
         */
        private void ApplyEnabledWorkTypes()
        {
            List<WorkTypeDef> all = DefDatabase<WorkTypeDef>.AllDefsListForReading;
            for (int i = 0; i < all.Count; i++)
            {
                WorkTypeDef w = all[i];
                bool enabled = RaceProps.mechEnabledWorkTypes.Contains(w);

                // SetPriority logs an error when given a non-zero priority for a work type that
                // Pawn.WorkTypeIsDisabled reports as disabled, so ask the base implementation
                // first - that is the one SetPriority itself consults.
                if (enabled && base.WorkTypeIsDisabled(w))
                {
                    continue;
                }

                workSettings.SetPriority(w, enabled ? 3 : 0);
            }
        }

        /**
         * Not actually changed
         */
        public new bool WorkTagIsDisabled(WorkTags w)
        {
            return (CombinedDisabledWorkTags & w) != 0;
        }

        /**
         * Not actually changed
         */
        public new bool WorkTypeIsDisabled(WorkTypeDef w)
        {
            return GetDisabledWorkTypes().Contains(w);
        }

        private List<WorkTypeDef> cachedDisabledWorkTypes;
        private List<WorkTypeDef> cachedDisabledWorkTypesPermanent;
        /**
         * Stripped to bare bones. Uses mechanoid tags for available job tags.
         */
        public new List<WorkTypeDef> GetDisabledWorkTypes(bool permanentOnly = false)
        {
            if (IsFormerHuman())
                return base.GetDisabledWorkTypes(permanentOnly);

            // FillList used to run on every call, outside the null check, so the "cache" never
            // cached. WorkTypeIsDisabled below goes through here and JobGiver_Housekeeper calls
            // that once per WorkGiver per scan, so this walked every WorkTypeDef with a
            // list.Contains on each one, several times a second per scarab. Fill once, like
            // vanilla Pawn.GetDisabledWorkTypes does.
            if (permanentOnly)
            {
                if (cachedDisabledWorkTypesPermanent == null)
                {
                    cachedDisabledWorkTypesPermanent = new List<WorkTypeDef>();
                    FillList(cachedDisabledWorkTypesPermanent);
                }

                return cachedDisabledWorkTypesPermanent;
            }

            if (cachedDisabledWorkTypes == null)
            {
                cachedDisabledWorkTypes = new List<WorkTypeDef>();
                FillList(cachedDisabledWorkTypes);
            }

            return cachedDisabledWorkTypes;
            void FillList(List<WorkTypeDef> list)
            {
                List<WorkTypeDef> allDefsListForReading = DefDatabase<WorkTypeDef>.AllDefsListForReading;
                for (int j = 0; j < allDefsListForReading.Count; j++)
                {
                    if (!RaceProps.mechEnabledWorkTypes.Contains(allDefsListForReading[j]) && !list.Contains(allDefsListForReading[j]))
                    {
                        list.Add(allDefsListForReading[j]);
                    }
                }
            }
        }

        /**
         * For Pawnmorpher compatibility
         */
        public bool IsFormerHuman()
        {
            return story != null && (story.Childhood != null || story.Adulthood != null);
        }
    }
}
