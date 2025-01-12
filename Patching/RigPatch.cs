using HarmonyLib;

namespace J0kerMenu_GTAG.Patching
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    internal class RigPatch
    {
        public static bool Prefix(VRRig rig)
        {
            return !(rig == GorillaTagger.Instance.offlineVRRig);
        }
    }
}