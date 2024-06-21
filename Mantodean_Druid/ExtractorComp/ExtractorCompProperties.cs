#region Assembly Assembly-CSharp, Version=1.5.8909.13066, Culture=neutral, PublicKeyToken=null
// C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll
// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
#endregion

using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace L24
{
    public class CompProperties_Extractor : CompProperties
    {
        public CompProperties_Extractor()
        {
            this.compClass = typeof(CompExtractor);
        }
        public bool canTransform = false;
        public bool canExtract = false;
        public bool canEvolve = false; 
        public PawnKindDef transformPawn = PawnKindDefOf.FleshmassNucleus;
        public ThingDef extractThing;
        public int extractAmount = 0;
        public float fuelConsumptionRate = 1f;
        public DamageDef extractDamage=DamageDefOf.Bite;
        public List<HediffDef> oldHediffs = new List<HediffDef>();
        public List<HediffDef> newHediffs = new List<HediffDef>();

        public int extractDamageAmount=0;
        public int extractEveryXSeconds = 1;
        public int finishInXTime = 600;
        public string ticksSecondsHoursDaysYears = "Days";
        public string selectIconPath = "UI/Gizmos/InsertPawn";
        public string transformIconPath ;
        public string extractIconPath;
        public string evolveDescription = "";
        public string transformDescription = "";






    }
}
#if false // Dekompilierungsprotokoll
44 Elemente im Cache
------------------
Auflösen: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Einzelne Assembly gefunden: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Laden von: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll
------------------
Auflösen: UnityEngine.IMGUIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: UnityEngine.IMGUIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.IMGUIModule.dll
------------------
Auflösen: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.CoreModule.dll
------------------
Auflösen: UnityEngine.TextRenderingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: UnityEngine.TextRenderingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.TextRenderingModule.dll
------------------
Auflösen: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Einzelne Assembly gefunden: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Laden von: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Core.dll
------------------
Auflösen: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Einzelne Assembly gefunden: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Laden von: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll
------------------
Auflösen: NAudio, Version=1.7.3.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: NAudio, Version=1.7.3.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\NAudio.dll
------------------
Auflösen: NVorbis, Version=0.8.4.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: NVorbis, Version=0.8.4.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\NVorbis.dll
------------------
Auflösen: UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: com.rlabrecque.steamworks.net, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: com.rlabrecque.steamworks.net, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\com.rlabrecque.steamworks.net.dll
------------------
Auflösen: System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Einzelne Assembly gefunden: System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Laden von: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Xml.dll
------------------
Auflösen: Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp-firstpass.dll
------------------
Auflösen: System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
Der Name "System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" wurde nicht gefunden.
------------------
Auflösen: Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.AssetBundleModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.AssetBundleModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: Unity.Mathematics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "Unity.Mathematics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: ISharpZipLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: ISharpZipLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\ISharpZipLib.dll
------------------
Auflösen: UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.PerformanceReportingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.PerformanceReportingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.ImageConversionModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.ImageConversionModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.ScreenCaptureModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Der Name "UnityEngine.ScreenCaptureModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" wurde nicht gefunden.
------------------
Auflösen: UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Einzelne Assembly gefunden: UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Laden von: C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.UI.dll
#endif
