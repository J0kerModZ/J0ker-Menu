using GorillaNetworking;
using HarmonyLib;
using PlayFab.CloudScriptModels;
using PlayFab.Internal;
using PlayFab;
using System;
using UnityEngine;
using GorillaTagScripts;

namespace J0kerMenu_GTAG.Mod_Menu.Patching
{
    [HarmonyPatch(typeof(GorillaNot), "SendReport")]
    internal class AntiCheat
    {
        private static bool Prefix(string susReason, string susId, string susNick)
        {
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

    [HarmonyPatch(typeof(GorillaNot), "GetRPCCallTracker")]
    internal class NoGetRPCCallTracker : MonoBehaviour
    {
        private static bool Prefix()
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

    [HarmonyPatch(typeof(BuilderTableNetworking), "PieceCreatedRPC")]
    public class BuilderPiecePatch
    {
        public static bool enabled = false;
        public static int pieceNeeded = 0;
        public static int pieceId = -1;
        private static void Postfix(int type, int id)
        {
            if (enabled)
            {
                if (pieceNeeded == type)
                {
                    pieceId = id;
                    enabled = false;
                }
            }
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
