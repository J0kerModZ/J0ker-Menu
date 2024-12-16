using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using PlayFab.CloudScriptModels;
using PlayFab.Internal;
using PlayFab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace J0kerMenu_GTAG.Mod_Menu.Patching
{
    [HarmonyPatch(typeof(GorillaNot), "SendReport")]
    internal class AntiCheat
    {
        private static bool Prefix(string susReason, string susId, string susNick)
        {
            Debug.Log("REPORT: " + susReason);
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "LogErrorCount")]
    public class NoLogErrorCount : MonoBehaviour
    {
        private static bool Prefix(string logString, string stackTrace, LogType type)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "CloseInvalidRoom")]
    public class NoCloseInvalidRoom : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "CheckReports", MethodType.Enumerator)]
    public class NoCheckReports : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "QuitDelay", MethodType.Enumerator)]
    public class NoQuitDelay : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCallLocal")]
    public class NoIncrementRPCCallLocal : MonoBehaviour
    {
        private static bool Prefix(PhotonMessageInfoWrapped infoWrapped, string rpcFunction)
        {
            Debug.Log(infoWrapped.Sender.NickName + " sent rpc: " + rpcFunction);
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "GetRPCCallTracker")]
    internal class NoGetRPCCallTracker : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCall", new Type[] { typeof(PhotonMessageInfo), typeof(string) })]
    public class NoIncrementRPCCall : MonoBehaviour
    {
        private static bool Prefix(PhotonMessageInfo info, string callingMethod = "")
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(VRRig), "IncrementRPC", new Type[] { typeof(PhotonMessageInfoWrapped), typeof(string) })]
    public class NoIncrementRPC : MonoBehaviour
    {
        private static bool Prefix(PhotonMessageInfoWrapped info, string sourceCall)
        {
            return false;
        }
    }
    

    [HarmonyPatch(typeof(PlayFabDeviceUtil), "SendDeviceInfoToPlayFab")]
    internal class PlayfabUtil01 : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaComputer))]
    [HarmonyPatch("AutoBanPlayfabFunction")]
    internal class ComputerPatchBadName
    {
        private static bool Prefix(string nameToCheck, bool forRoom, Action<ExecuteFunctionResult> resultCallback)
        {
            if (forRoom)
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(nameToCheck, JoinType.Solo);
            }
            else
            {
                NetworkSystem.Instance.SetMyNickName(nameToCheck);
                GorillaComputer.instance.savedName = nameToCheck;
                GorillaComputer.instance.currentName = nameToCheck;
                GorillaComputer.instance.offlineVRRigNametagText.text = nameToCheck;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabHttp), "InitializeScreenTimeTracker")]
    internal class PlayfabUtil02 : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportDeviceInfo")]
    internal class PlayfabUtil03 : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "DispatchReport")]
    internal class NoDispatchReport : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaNot), "ShouldDisconnectFromRoom")]
    internal class NoShouldDisconnectFromRoom : MonoBehaviour
    {
        private static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaPlayerScoreboardLine), "ReportPlayer")]
    internal class DontReportPlayer : MonoBehaviour
    {
        private static bool Prefix(string PlayerID, GorillaPlayerLineButton.ButtonType buttonType, string OtherPlayerNickName)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(VRRig))]
    public static class TaggingPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("CheckTagDistanceRollback")]
        public static bool Prefix(VRRig otherRig, float max, float timeInterval)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CheckDistance")]
        public static bool Prefix(Vector3 position, float max, ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}
