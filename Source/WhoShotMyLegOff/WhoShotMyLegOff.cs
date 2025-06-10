using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WhoShotMyLegOff
{
    [StaticConstructorOnStartup]
    public static class WhoShotMyLegOff
    {
        static WhoShotMyLegOff()
        {
            var harmony = new Harmony("com.tixiv.WhoShotMyLegOff");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(HealthCardUtility))]
    [HarmonyPatch(nameof(HealthCardUtility.GetCombatLogInfo))]
    public static class Patch_HealthCardUtility_GetCombatLogInfo
    {
        // Postfix
        public static void Postfix(IEnumerable<Hediff> diffs, ref bool __result, ref TaggedString combatLogText, ref LogEntry combatLogEntry)
        {
            // Patch the game bug: We had this functionality all along! The game is already saving the log string!
            // Juts this method returns false whenever there is no log entry, which doesn't seem to be intended
            // because the two outputs are checked individually anyway in all places where this function is used.

            __result = __result || !combatLogText.NullOrEmpty();

            // Feature: Add OccurredTimeAgo in LogInfo

            if (!combatLogText.NullOrEmpty() && diffs.Count() == 1)
            {
                var hediff = diffs.First();
                if (hediff.tickAdded > 0)
                {
                    int age = Find.TickManager.TicksGame - hediff.tickAdded;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendTagged(combatLogText);
                    sb.AppendLine().AppendTagged("OccurredTimeAgo".Translate(age.ToStringTicksToPeriod()).CapitalizeFirst() + ".");
                    combatLogText = sb.ToString();
                }
            }
        }
    }
}