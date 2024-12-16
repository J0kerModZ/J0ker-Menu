using HarmonyLib;

namespace J0kerMenu_GTAG.Patching
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    internal class RigPatch
    {
        public static bool Prefix(VRRig __instance)
        {
            if (__instance == GorillaTagger.Instance.offlineVRRig)
            {
                return false;
            }
            return true;
        }
    }
}