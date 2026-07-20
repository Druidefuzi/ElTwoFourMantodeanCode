using System.Collections.Generic;
using Verse;

namespace Mantodean.Conversion
{
    public class DefModExt_BasicConvert : DefModExtension
    {
        public PawnKindDef defaultPawnKind;
        public PawnKindDef animalPawnKind = null;
        /// <summary>
        /// Viable inputs for conversion. Uses the defName of the race itself. Should be able to accept any pawn race if left 'null'.
        /// </summary>
        public List<ThingDef> requiredinputDefs = null;

        public float requiredsize = 0.7f;

        public bool forceDropEquipment;
        public bool killiflowmass = false;
        public bool animalifwrongrace = false;
        public bool killifwrongrace = false;
        public bool killPawn;
    }
}
