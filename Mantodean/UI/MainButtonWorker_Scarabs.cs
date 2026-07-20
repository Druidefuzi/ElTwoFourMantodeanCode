using RimWorld;
using Verse;

namespace Mantodean
{
    /*
     * Keeps the Scarabs button out of the bottom bar until the player actually owns a scarab.
     * MainButtonWorker.Visible is virtual and is consulted every frame while the bar is drawn;
     * PawnsInFaction is a list vanilla already maintains, so walking it is cheap.
     */
    public class MainButtonWorker_Scarabs : MainButtonWorker_ToggleTab
    {
        public override bool Visible
        {
            get
            {
                if (!base.Visible)
                {
                    return false;
                }

                Map map = Find.CurrentMap;
                if (map == null)
                {
                    return false;
                }

                foreach (Pawn p in map.mapPawns.PawnsInFaction(Faction.OfPlayer))
                {
                    if (MainTabWindow_Scarabs.IsListable(p))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
