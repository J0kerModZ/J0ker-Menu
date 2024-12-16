using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaNetworking.Store;
using GorillaTagScripts;
using HarmonyLib;
using OculusSampleFramework;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using Debug = UnityEngine.Debug;
using GameMode = GorillaGameModes.GameMode;

namespace J0kerMenu_GTAG.Menu
{
    public class Mods : MonoBehaviourPunCallbacks
    {
        static GameObject GunSphere;

        #region Player

        public static void Fly()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * 15;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public static void SpeedBoost()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
            {
                GorillaLocomotion.Player.Instance.maxJumpSpeed = 15f;
            }
            else
            {
                GorillaLocomotion.Player.Instance.maxJumpSpeed = 6.5f;
            }
        }

        public static void NoGrav()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().useGravity = true;
            }
        }

        public static void NoTagFreeze()
        {
            GorillaLocomotion.Player.Instance.disableMovement = false;
        }

        public static void GhostMonkey()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void InvisMonkey()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = Vector3.zero;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void TPGun()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo);
                GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GunSphere.transform.position = hitinfo.point;
                GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                GunSphere.GetComponent<Renderer>().material.color = Color.white;
                GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                GameObject.Destroy(GunSphere.GetComponent<Collider>());

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    GameObject.Destroy(GunSphere, Time.deltaTime);
                    GunSphere.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;

                    GorillaLocomotion.Player.Instance.transform.position = GunSphere.transform.position;
                }

            }
            if (GunSphere != null)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                GameObject.Destroy(GunSphere, Time.deltaTime);
            }
        }

        public static void RigGun()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo);
                GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GunSphere.transform.position = hitinfo.point;
                GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                GunSphere.GetComponent<Renderer>().material.color = Color.white;
                GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                GameObject.Destroy(GunSphere.GetComponent<Collider>());

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    GameObject.Destroy(GunSphere, Time.deltaTime);
                    GunSphere.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;

                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = GunSphere.transform.position + new Vector3(0f, 1f, 0f);
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
            if (GunSphere != null)
            {
                GameObject.Destroy(GunSphere, Time.deltaTime);
            }
        }

        #region Plats

        private static GameObject PlatsLeft, PlatsRight;
        private static bool PlatLSpawn, PlatRSpawn;

        public static void PlatL()
        {
            PlatsLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(PlatsLeft.GetComponent<Rigidbody>());
            Object.Destroy(PlatsLeft.GetComponent<BoxCollider>());
            Object.Destroy(PlatsLeft.GetComponent<Renderer>());
            PlatsLeft.transform.localScale = new Vector3(0.25f, 0.3f, 0.25f);

            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.transform.parent = PlatsLeft.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.1f, 1f, 1f);
            gameObject.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            gameObject.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
            gameObject.transform.position = new Vector3(0.02f, 0f, 0f);
        }

        public static void PlatR()
        {
            PlatsRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(PlatsRight.GetComponent<Rigidbody>());
            Object.Destroy(PlatsRight.GetComponent<BoxCollider>());
            Object.Destroy(PlatsRight.GetComponent<Renderer>());

            PlatsRight.transform.localScale = new Vector3(0.25f, 0.3f, 0.25f);
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.transform.parent = PlatsRight.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.1f, 1f, 1f);
            gameObject.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            gameObject.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
            gameObject.transform.position = new Vector3(-0.02f, 0f, 0f);
        }

        public static void Platforms()
        {
            if (ControllerInputPoller.instance.leftControllerGripFloat > 0.1f && PlatsLeft == null)
            {
                PlatL();
            }
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f && PlatsRight == null)
            {
                PlatR();
            }

            if (ControllerInputPoller.instance.leftControllerGripFloat > 0.1f && PlatsLeft != null && !PlatLSpawn)
            {
                PlatsLeft.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                PlatsLeft.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                PlatLSpawn = true;
            }
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f && PlatsRight != null && !PlatRSpawn)
            {
                PlatsRight.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                PlatsRight.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
                PlatRSpawn = true;
            }

            if (!ControllerInputPoller.instance.leftGrab && PlatsLeft != null)
            {
                GameObject.Destroy(PlatsLeft);
                PlatsLeft = null;
                PlatLSpawn = false;
            }
            if (!ControllerInputPoller.instance.rightGrab && PlatsRight != null)
            {
                GameObject.Destroy(PlatsRight);
                PlatsRight = null;
                PlatRSpawn = false;
            }
        }
        #endregion

        #endregion

        #region ESP

        public static void PlayerESP() // All The Modes
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.Casual)
            {
                foreach (VRRig rigs in GorillaParent.instance.vrrigs)
                {
                    if (rigs != GorillaTagger.Instance.offlineVRRig)
                    {
                        rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                        rigs.mainSkin.material.color = rigs.playerColor;
                    }
                }
            }
            else if (GameMode.ActiveGameMode.GameType() == GameModeType.Infection)
            {
                foreach (VRRig rigs in GorillaParent.instance.vrrigs)
                {
                    if (rigs != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (rigs.mainSkin.material.name.Contains("it") || rigs.mainSkin.material.name.Contains("fected"))
                        {
                            rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            rigs.mainSkin.material.color = Color.red;
                        }
                        else
                        {
                            rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            rigs.mainSkin.material.color = Color.green;
                        }
                    }
                }
            }
            else if (GameMode.ActiveGameMode.GameType() == GameModeType.Paintbrawl)
            {
                foreach (VRRig rigs in GorillaParent.instance.vrrigs)
                {
                    if (rigs != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (rigs.mainSkin.material.name.Contains("blue"))
                        {
                            rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            rigs.mainSkin.material.color = Color.cyan;
                        }
                        else if (rigs.mainSkin.material.name.Contains("orange"))
                        {
                            rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            rigs.mainSkin.material.color = new Color32(255, 165, 0, 255); // Orange
                        }
                        else if (rigs.mainSkin.material.name.Contains("paintsplatter"))
                        {
                            rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            rigs.mainSkin.material.color = Color.white;
                        }
                    }
                }
            }
        }

        public static void DisableESP()
        {
            foreach (VRRig rigs in GorillaParent.instance.vrrigs)
            {
                if (rigs != GorillaTagger.Instance.offlineVRRig)
                {
                    rigs.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                }
            }
        }
        #endregion

        #region Tag

        public static void TagAll()
        {
            bool foundNonTaggedPlayer = false;
            Vector3 targetPosition = Vector3.zero;
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
            {
                foreach (VRRig players in GorillaParent.instance.vrrigs)
                {
                    if (players != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (!players.mainSkin.material.name.Contains("fected"))
                        {
                            foundNonTaggedPlayer = true;

                            targetPosition = players.transform.position;

                            GorillaTagger.Instance.offlineVRRig.enabled = false;

                            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = targetPosition;
                            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = targetPosition;

                            GorillaLocomotion.Player.Instance.rightControllerTransform.position = targetPosition;
                        }
                    }
                }

                if (foundNonTaggedPlayer)
                {
                    GorillaTagger.Instance.offlineVRRig.transform.position = targetPosition;
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void TagAura()
        {
            float tagDistance = 3f;

            Vector3 forwardDirection = GorillaTagger.Instance.offlineVRRig.head.rigTarget.forward;

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                Vector3 targetPosition = vrrig.headMesh.transform.position;
                Vector3 playerPosition = GorillaTagger.Instance.offlineVRRig.head.rigTarget.position;
                Vector3 toTarget = targetPosition - playerPosition;

                float playerDistance = toTarget.magnitude;

                if (!vrrig.mainSkin.material.name.Contains("fected") && playerDistance < tagDistance && Vector3.Dot(forwardDirection, toTarget.normalized) > 0)
                {
                    GorillaLocomotion.Player.Instance.rightControllerTransform.position = targetPosition;
                }
            }
        }

        public static void TagGun()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo))
                {
                    GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    GunSphere.transform.position = hitinfo.point;
                    GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                    GunSphere.GetComponent<Renderer>().material.color = Color.white;
                    GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                    GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                    GameObject.Destroy(GunSphere.GetComponent<Collider>());

                    VRRig player = hitinfo.collider.GetComponent<VRRig>();

                    if (player != null)
                    {
                        if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                        {
                            GameObject.Destroy(GunSphere, Time.deltaTime);
                            GunSphere.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;

                            GorillaTagger.Instance.offlineVRRig.enabled = false;

                            GorillaTagger.Instance.offlineVRRig.transform.position = player.transform.position;
                            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = player.transform.position;
                            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = player.transform.position;

                            GorillaLocomotion.Player.Instance.rightControllerTransform.position = player.transform.position;
                        }
                        else
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = true;
                        }
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.enabled = true;
                    }
                }
            }
            if (GunSphere != null)
            {
                GameObject.Destroy(GunSphere, Time.deltaTime);
            }
        }


        public static void TagSelf()
        {
            foreach (VRRig rigs in GorillaParent.instance.vrrigs)
            {
                if (rigs.mainSkin.material.name.Contains("fected"))
                {
                    if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                    {
                        GorillaTagger.Instance.offlineVRRig.enabled = false;
                        GorillaTagger.Instance.offlineVRRig.transform.position = rigs.rightHandTransform.position;
                    }
                }
            }
        }
        #endregion

        #region Freeze Tag

        public static void FreezeAll()
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.FreezeTag && PhotonNetwork.IsMasterClient)
            {
                GorillaFreezeTagManager gorillaFreezeTag = FindObjectOfType<GorillaFreezeTagManager>();
                foreach (NetPlayer player in NetworkSystem.Instance.PlayerListOthers)
                {
                    gorillaFreezeTag.AddInfectedPlayer(player);
                }
                Flush();
            }
        }

        public static void FreezeTagGun()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo))
                {
                    GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    GunSphere.transform.position = hitinfo.point;
                    GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                    GunSphere.GetComponent<Renderer>().material.color = Color.white;
                    GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                    GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                    GameObject.Destroy(GunSphere.GetComponent<Collider>());

                    GorillaFreezeTagManager gorillaFreezeTag = FindObjectOfType<GorillaFreezeTagManager>();
                    VRRig player = hitinfo.collider.GetComponent<VRRig>();

                    if (player != null)
                    {
                        if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                        {
                            GameObject.Destroy(GunSphere, Time.deltaTime);
                            GunSphere.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
                            if (GameMode.ActiveGameMode.GameType() == GameModeType.FreezeTag && PhotonNetwork.IsMasterClient)
                            {
                                gorillaFreezeTag.AddInfectedPlayer(player.OwningNetPlayer);
                                Flush();
                            }
                        }
                    }
                }
            }
            if (GunSphere != null)
            {
                GameObject.Destroy(GunSphere, Time.deltaTime);
            }
        }

        public static void FreezeSelf()
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.FreezeTag && PhotonNetwork.IsMasterClient)
            {
                GorillaFreezeTagManager gorillaFreezeTag = FindObjectOfType<GorillaFreezeTagManager>();
                gorillaFreezeTag.AddInfectedPlayer(GorillaTagger.Instance.myVRRig.Owner);
                Flush();
            }
        }

        public static void UnFreezeSelf()
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.FreezeTag && PhotonNetwork.IsMasterClient)
            {
                GorillaFreezeTagManager gorillaFreezeTag = FindObjectOfType<GorillaFreezeTagManager>();
                gorillaFreezeTag.currentInfected.Remove(GorillaTagger.Instance.myVRRig.Owner);
                Flush();
            }
        }

        #endregion

        #region Water
        static float WaterDelay;

        public static void WaterSpamHands()
        {
            if (Time.time > WaterDelay)
            {
                WaterDelay = Time.time + 0.1f;
                if (ControllerInputPoller.instance.leftControllerGripFloat > 0.1f)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.offlineVRRig.leftHandTransform.position,
                        GorillaTagger.Instance.offlineVRRig.leftHandTransform.rotation,
                        4f,
                        100f,
                        true,
                        false
                    });
                }

                if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.offlineVRRig.rightHandTransform.position,
                        GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation,
                        4f,
                        100f,
                        true,
                        false
                    });
                }
                Flush();
            }
        }

        public static void WaterSpamBody()
        {
            if (Time.time > WaterDelay)
            {
                WaterDelay = Time.time + 0.1f;

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.offlineVRRig.bodyTransform.position,
                        GorillaTagger.Instance.offlineVRRig.bodyTransform.rotation,
                        4f,
                        100f,
                        true,
                        false
                    });
                    Flush();
                }
            }
        }

        public static void WaterSpamHead()
        {
            if (Time.time > WaterDelay)
            {
                WaterDelay = Time.time + 0.1f;
                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        GorillaTagger.Instance.offlineVRRig.headConstraint.position,
                        GorillaTagger.Instance.offlineVRRig.headConstraint.rotation,
                        4f,
                        100f,
                        true,
                        false
                    });
                    Flush();
                }
            }
        }

        public static void WaterSpamAura()
        {
            if (Time.time > WaterDelay)
            {
                WaterDelay = Time.time + 0.1f;

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    float auraRadius = 1.5f;
                    float angle = Random.Range(0f, Mathf.PI * 2);
                    Vector3 offset = new Vector3(Mathf.Cos(angle) * auraRadius, 0, Mathf.Sin(angle) * auraRadius);

                    Vector3 auraPosition = GorillaTagger.Instance.offlineVRRig.bodyTransform.position + offset;

                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All, new object[]
                    {
                        auraPosition,
                        Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)),
                        4f,
                        100f,
                        true,
                        false
                    });

                    Flush();
                }
            }
        }

        #endregion

        #region Blocks
        static float BlockDelay;

        public static void BigBlockSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
            {
                if (Time.time > BlockDelay)
                {
                    BuilderPiece[] validPieces = Object.FindObjectsOfType<BuilderPiece>().Where(piece => piece.name.Contains("Terrain")).ToArray();

                    if (validPieces.Length > 0)
                    {
                        BuilderPiece randomPiece = validPieces[Random.Range(0, validPieces.Length)];
                        BuilderTable.instance.RequestCreatePiece(randomPiece.pieceType, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.rotation, randomPiece.materialType);
                        Flush();
                    }
                    BlockDelay = Time.time + 0.2f;
                }
            }
        }

        public static void BlockSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
            {
                if (Time.time > BlockDelay)
                {
                    BuilderPiece[] validPieces = Object.FindObjectsOfType<BuilderPiece>().Where(piece => piece.name.Contains("Wall") || piece.name.Contains("Floor") || piece.name.Contains("Roof")).ToArray(); 
                    
                    if (validPieces.Length > 0)
                    {
                        BuilderPiece randomPiece = validPieces[Random.Range(0, validPieces.Length)];
                        BuilderTable.instance.RequestCreatePiece(randomPiece.pieceType, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.rotation, randomPiece.materialType);
                        Flush();
                    }
                    BlockDelay = Time.time + 0.2f;
                }
            }
        }
    

        public static void XmasBlockSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
            {
                if (Time.time > BlockDelay)
                {
                    BuilderPiece[] winterPieces = Object.FindObjectsOfType<BuilderPiece>().Where(piece => piece.displayName.Contains("Winter")).ToArray();
                    if (winterPieces.Length > 0)
                    {
                        BuilderPiece randomPiece = winterPieces[Random.Range(0, winterPieces.Length)];
                        BuilderTable.instance.RequestCreatePiece(randomPiece.pieceType, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.rotation, randomPiece.materialType);
                        Flush();
                    }
                    BlockDelay = Time.time + 0.2f;
                }
            }
        }

        public static void BlockLauncher()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (Time.time > BlockDelay)
                    {
                        BuilderPiece[] validPieces = Object.FindObjectsOfType<BuilderPiece>().Where(piece => piece.name.Contains("Wall") || piece.name.Contains("Floor") || piece.name.Contains("Roof")).ToArray();

                        if (validPieces.Length > 0)
                        {
                            BuilderPiece randomPiece = validPieces[Random.Range(0, validPieces.Length)];
                            BuilderTableNetworking.instance.photonView.RPC("RequestDropPieceRPC", RpcTarget.MasterClient, new object[] { randomPiece.pieceId, randomPiece.pieceId, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation, GorillaTagger.Instance.offlineVRRig.rightHandTransform.up * 8.44f, Vector3.zero, PhotonNetwork.LocalPlayer });
                            Flush();
                        }
                        BlockDelay = Time.time + 0.1f;
                    }
                }
            }
        }
        #endregion

        #region Sounds
        static float SoundDelay;

        public static void HandTapSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (Time.time > SoundDelay)
                {
                    SoundDelay = Time.time + 0.1f;
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                    {
                        1,
                        false,
                        100f
                    });
                    Flush();
                }
            }
        }

        public static void PopSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (Time.time > SoundDelay)
                {
                    SoundDelay = Time.time + 0.1f;

                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                    {
                        84,
                        false,
                        100f
                    });
                    Flush();
                }
            }
        }

        public static void EatSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (Time.time > SoundDelay)
                {
                    SoundDelay = Time.time + 0.1f;

                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                    {
                        86,
                        false,
                        100f
                    });
                    Flush();
                }
            }
        }

        public static void RandomSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (Time.time > SoundDelay)
                {
                    SoundDelay = Time.time + 0.1f;

                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                    {
                        Random.Range(1, 360),
                        false,
                        100f
                    });
                    Flush();
                }
            }
        }

        #endregion

        #region Party
        static float BracletDelay;
        static bool Enable;

        public static void BracletSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (Time.time > BracletDelay)
                {
                    BracletDelay = Time.time + 0.1f;

                    GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
                    {
                        Enable = !Enable,
                        false
                    });
                    Flush();
                }
            }
        }

        public static void BracletAdd()
        {
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
            {
                true,
                false
            });
        }

        public static void BracletRemove()
        {
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
            {
                false,
                false
            });
        }

        public static void LeaveParty()
        {
            if (FriendshipGroupDetection.Instance.IsInParty)
            {
                FriendshipGroupDetection.Instance.LeaveParty();
            }
        }

        #endregion

        #region City
        static float FortuneDelay;
        static float DoorDelay;
        static float CosmeticDelay;

        public static void FortuneTellerSpammer()
        {
            FortuneTeller fortuneTeller = FindObjectOfType<FortuneTeller>();

            if (PhotonNetwork.IsMasterClient)
            {
                if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
                {
                    if (Time.time > FortuneDelay)
                    {
                        FortuneDelay = Time.time + 0.1f;
                        if (fortuneTeller != null)
                        {
                            fortuneTeller.photonView.RPC("TriggerAttractAnimRPC", RpcTarget.All, System.Array.Empty<object>());
                            Flush();
                        }
                    }
                }
            }
        }

        public static void DoorSpammer()
        {
            GTDoor door = FindObjectOfType<GTDoor>();

            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (Time.time > DoorDelay)
                {
                    DoorDelay = Time.time + 0.5f;
                    if (door != null)
                    {
                        door.photonView.RPC("ChangeDoorState", RpcTarget.AllViaServer, new object[]
                        {
                            GTDoor.DoorState.Opening
                        });
                        Flush();
                    }
                }
            }
        }

        public static void CosmeticSpam()
        {
            FittingRoomButton[] buttons = Object.FindObjectsOfType<FittingRoomButton>();

            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (buttons.Length > 0)
                {
                    int randomIndex = Random.Range(0, buttons.Length);
                    if (Time.time > CosmeticDelay && buttons[randomIndex] != null)
                    {
                        CosmeticDelay = Time.time + 0.1f;
                        buttons[randomIndex].ButtonActivationWithHand(false);
                        AddRandomToCart();
                    }
                }
            }
        }

        static void AddRandomToCart()
        {
            DynamicCosmeticStand[] buttons = Object.FindObjectsOfType<DynamicCosmeticStand>();

            if (buttons.Length > 0)
            {
                int randomIndex = Random.Range(0, buttons.Length);
                if (buttons[randomIndex] != null)
                {
                    buttons[randomIndex].PressCosmeticStandButton();
                }
            }
        }
        #endregion

        #region Guardian

        public static void GetGuardian()
        {
            foreach (TappableGuardianIdol tapple in Object.FindObjectsOfType(typeof(TappableGuardianIdol)))
            {
                if (!tapple.isChangingPositions)
                {
                    foreach (GorillaGuardianManager guardianManager in Object.FindObjectsOfType(typeof(GorillaGuardianManager)))
                    {
                        if (!guardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = false;
                            GorillaTagger.Instance.offlineVRRig.transform.position = tapple.transform.position;

                            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = tapple.transform.position;
                            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = tapple.transform.position;

                            tapple.manager.photonView.RPC("SendOnTapRPC", RpcTarget.All, tapple.tappableId, UnityEngine.Random.Range(0.2f, 0.4f));
                            Flush();
                        }
                        else
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = true;
                        }
                    }
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        static NetworkView GetNetworkView(VRRig vrRig)
        {
            if (vrRig != null && vrRig != GorillaTagger.Instance.offlineVRRig)
            {
                NetworkView netView = Traverse.Create(vrRig).Field("netView").GetValue() as NetworkView;
                if (netView != null)
                {
                    return netView;
                }
            }
            return null;
        }

        public static void FlingAll()
        {
            foreach (VRRig plr in GorillaParent.instance.vrrigs)
            {
                if (plr == null || plr == GorillaTagger.Instance.offlineVRRig) continue;

                NetworkView netView = GetNetworkView(plr);

                if (netView != null)
                {
                    if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
                    {
                        GorillaGuardianManager manager = FindObjectOfType<GorillaGuardianManager>();
                        if (manager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            netView.SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                            netView.SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, 20f, 0f) });
                        }
                        else
                        {
                            GetGuardian();
                        }

                        Flush();
                    }
                }
            }
        }

        public static void GrabAll()
        {
            foreach (VRRig plr in GorillaParent.instance.vrrigs)
            {
                if (plr == null || plr == GorillaTagger.Instance.offlineVRRig) continue;

                NetworkView netView = GetNetworkView(plr);

                if (netView != null)
                {
                    if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
                    {
                        GorillaGuardianManager manager = FindObjectOfType<GorillaGuardianManager>();
                        if (manager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            netView.SendRPC("GrabbedByPlayer", RpcTarget.Others, new object[] { true, false, false });
                        }
                        else
                        {
                            GetGuardian();
                        }

                        Flush();
                    }
                }
            }
        }

        public static void DropAll()
        {
            foreach (VRRig plr in GorillaParent.instance.vrrigs)
            {
                if (plr == null || plr == GorillaTagger.Instance.offlineVRRig) continue;

                NetworkView netView = GetNetworkView(plr);

                if (netView != null)
                {
                    if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
                    {
                        GorillaGuardianManager manager = FindObjectOfType<GorillaGuardianManager>();
                        if (manager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            netView.SendRPC("DroppedByPlayer", RpcTarget.Others, new object[] { new Vector3(0f, -20f, 0f) });
                        }
                        else
                        {
                            GetGuardian();
                        }

                        Flush();
                    }
                }
            }
        }
        #endregion

        #region Safty
        static float distanceToLeave = 0.35f;

        public static void Flush()
        {
            PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
            PhotonNetwork.RemoveBufferedRPCs();

            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);

            if (GorillaTagger.Instance.myVRRig != null)
            {
                PhotonNetwork.OpCleanRpcBuffer(GorillaTagger.Instance.myVRRig.GetView);
            }

            GorillaNot.instance.rpcErrorMax = 999;
            GorillaNot.instance.rpcCallLimit = 999;
            GorillaNot.instance.logErrorMax = 999;

            PhotonNetwork.RemoveRPCsInGroup(999);
            GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        public static void AntiReport()
        {
            try
            {
                foreach (GorillaPlayerScoreboardLine scoreBoardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (scoreBoardLine.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform reportButton = scoreBoardLine.reportButton.gameObject.transform;
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (vrrig != GorillaTagger.Instance.offlineVRRig)
                            {
                                float DistanceRight = Vector3.Distance(vrrig.rightHandTransform.position, reportButton.position);
                                float DistanceLeft = Vector3.Distance(vrrig.leftHandTransform.position, reportButton.position);

                                if (DistanceRight < distanceToLeave || DistanceLeft < distanceToLeave)
                                {
                                    if (!PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED"))
                                    {
                                        PhotonNetwork.Disconnect();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
        #endregion

        #region Paintbrawl
        static float slingShotAutoFire_Float;
        static bool slingShotAutoFire_Bool;

        public static void AutoFire()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                if (GameMode.ActiveGameMode.GameType() == GameModeType.Paintbrawl)
                {
                    ControllerInputPoller.instance.rightControllerGripFloat = slingShotAutoFire_Bool ? 1f : 0f;
                    slingShotAutoFire_Bool = !slingShotAutoFire_Bool;

                    if (Time.time > slingShotAutoFire_Float)
                    {
                        slingShotAutoFire_Float = Time.time + 0.2f;
                        ControllerInputPoller.instance.rightControllerIndexFloat = 0f;
                    }
                    else
                    {
                        ControllerInputPoller.instance.rightControllerIndexFloat = 1f;
                    }
                }
            }
        }

        public static void Aimbot() // Very bad lol
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                if (GameMode.ActiveGameMode.GameType() == GameModeType.Paintbrawl)
                {
                    List<VRRig> randomRigs = new List<VRRig>();

                    foreach (SlingshotProjectile projectile in GameObject.FindObjectsOfType<SlingshotProjectile>())
                    {
                        foreach (VRRig rigs in GorillaParent.instance.vrrigs)
                        {
                            randomRigs.Add(rigs);
                            if (rigs != GorillaTagger.Instance.offlineVRRig)
                            {
                                if (projectile.projectileOwner == (NetPlayer)PhotonNetwork.LocalPlayer)
                                {
                                    if (randomRigs.Count > 0)
                                    {
                                        VRRig randomRig = randomRigs[Random.Range(0, randomRigs.Count)];

                                        if (randomRig.paintbrawlBalloons.balloons.Length > 0)
                                        {
                                            if (randomRig.mainSkin.material.name.Contains("blue") && GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("orange"))
                                            {
                                                projectile.gameObject.transform.position = randomRig.headConstraint.transform.position;
                                            }

                                            if (randomRig.mainSkin.material.name.Contains("orange") && GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("blue"))
                                            {
                                                projectile.gameObject.transform.position = randomRig.headConstraint.transform.position;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Revive()
        {
            if (PhotonNetwork.IsMasterClient && GameMode.ActiveGameMode.GameType() == GameModeType.Paintbrawl)
            {
                GorillaPaintbrawlManager gorillaPaintbrawlManager = FindObjectOfType<GorillaPaintbrawlManager>();
                gorillaPaintbrawlManager.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 3;
            }
        }

        public static void KillAll()
        {
            if (PhotonNetwork.IsMasterClient && GameMode.ActiveGameMode.GameType() == GameModeType.Paintbrawl)
            {
                GorillaPaintbrawlManager gorillaPaintbrawlManager = FindObjectOfType<GorillaPaintbrawlManager>();
                foreach (Photon.Realtime.Player players in PhotonNetwork.PlayerListOthers)
                {
                    gorillaPaintbrawlManager.playerLives[players.ActorNumber] = 0;
                }
            }
        }

        #endregion

        #region Projectiles 

        static GameObject gorillaVelocityEstimatorCustome;
        static GorillaVelocityEstimator scriptedGorillaVelEst;
        static bool ObjMade;
        static float projectileDelay;
        static bool projModsEnabled = false;

        public static void LaunchSnowBallProjectile(string Path, string Name, Vector3 velocity, Vector3 position, Quaternion rotation, Color color, bool RGB)
        {
            if (!ObjMade)
            {
                gorillaVelocityEstimatorCustome = new GameObject("GorillaVelocityEstimator (J0kerMenu)");
                scriptedGorillaVelEst = gorillaVelocityEstimatorCustome.AddComponent<GorillaVelocityEstimator>();
            }

            SnowballThrowable snowball = GameObject.Find(Path + "/").transform.Find(Name).GetComponent<SnowballThrowable>();
            if (!snowball.gameObject.activeSelf)
            {
                snowball.EnableSnowballLocal(true);
                if (RGB)
                {
                    snowball.randomizeColor = true;
                }
                else
                {
                    snowball.randomizeColor = false;
                }
                snowball.velocityEstimator = scriptedGorillaVelEst;
                snowball.transform.position = position;
                snowball.transform.rotation = rotation;
            }
            if (Time.time > projectileDelay)
            {
                projectileDelay = Time.time + 0.2f;

                Rigidbody gorillaRigidbody = GorillaTagger.Instance.GetComponent<Rigidbody>();
                Vector3 originalVelocity = gorillaRigidbody.velocity;
                Vector3 originalPosition = snowball.transform.position;

                gorillaRigidbody.velocity = velocity;

                GorillaTagger.Instance.offlineVRRig.SetThrowableProjectileColor(true, color);
                MethodInfo launchMethod = typeof(SnowballThrowable).GetMethod("LaunchSnowball", BindingFlags.NonPublic | BindingFlags.Instance);
                launchMethod.Invoke(snowball, null);

                gorillaRigidbody.velocity = originalVelocity;
                snowball.transform.position = originalPosition;

                Flush();
            }
            if (!ControllerInputPoller.instance.rightGrab)
            {
                snowball.EnableSnowballLocal(false);
            }
        }

        public static void LaunchElf() // This One Only Works With The Cosmetic
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                ElfLauncher launcher = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/ElfLauncherAnchor(Clone)/LMANE.").GetComponent<ElfLauncher>();
                MethodInfo launchMethod = typeof(ElfLauncher).GetMethod("Shoot", BindingFlags.NonPublic | BindingFlags.Instance);
                launchMethod.Invoke(launcher, null);

                Flush();
            }
        }

        public static void LaunchElfSpaz() // This One Only Works With The Cosmetic
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                ElfLauncher launcher = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/ElfLauncherAnchor(Clone)/LMANE.").GetComponent<ElfLauncher>();
                MethodInfo launchMethod = typeof(ElfLauncher).GetMethod("Shoot", BindingFlags.NonPublic | BindingFlags.Instance);
                launchMethod.Invoke(launcher, null);

                GorillaLocomotion.Player.Instance.rightControllerTransform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

                Flush();
            }
        }

        public static void LaunchElfRain() // This One Only Works With The Cosmetic
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                ElfLauncher launcher = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/ElfLauncherAnchor(Clone)/LMANE.").GetComponent<ElfLauncher>();
                MethodInfo launchMethod = typeof(ElfLauncher).GetMethod("Shoot", BindingFlags.NonPublic | BindingFlags.Instance);
                launchMethod.Invoke(launcher, null);

                GorillaLocomotion.Player.Instance.rightControllerTransform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.up * 1f);
                GorillaLocomotion.Player.Instance.rightControllerTransform.rotation = Quaternion.Euler(-90f, Random.Range(0f, 360f), -90f);

                Flush();
            }
        }

        static void EnableAllProjs() // Auto enables if using projectiles
        {
            string[] projectilePaths = new string[]
            {
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/SnowballRightAnchor(Clone)/LMACF. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/WaterBalloonRightAnchor(Clone)/LMAEY. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/LavaRockAnchor(Clone)/LMAGE. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/TrickTreatFunctionalAnchorRIGHT Variant(Clone)/LMAMO. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/AppleRightAnchor(Clone)/LMAMV.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/ScienceCandyRightAnchor(Clone)/LMAIF. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/BucketGiftFunctionalAnchor_Right(Clone)/LMAHR. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/VotingRockAnchor_RIGHT(Clone)/LMAMT. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/FishFoodRightAnchor(Clone)/LMAIP. RIGHT."
            };

            string[] transferablePaths = new string[]
            {
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballRightAnchor(Clone)/LMACF. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/WaterBalloonRightAnchor(Clone)/LMAEY. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/LavaRockAnchor(Clone)/LMAGE. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/TrickTreatFunctionalAnchorRIGHT Variant(Clone)/LMAMO. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/AppleRightAnchor(Clone)/LMAMV.",
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/ScienceCandyRightAnchor(Clone)/LMAIF. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/BucketGiftFunctionalAnchor_Right(Clone)/LMAHR. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/VotingRockAnchor_RIGHT(Clone)/LMAMT. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/FishFoodRightAnchor(Clone)/LMAIP. RIGHT."
            };

            foreach (string path in projectilePaths)
            {
                GameObject obj = GameObject.Find(path);
                if (obj != null) obj.SetActive(true);
            }

            foreach (string path in transferablePaths)
            {
                GameObject obj = GameObject.Find(path);
                if (obj != null) obj.SetActive(false);
            }

            projModsEnabled = true;
        }

        public static void SnowBallGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (projModsEnabled)
                {
                    Vector3 Val = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 8.33f;
                    Vector3 Pos = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position;
                    Quaternion Rot = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;

                    LaunchSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/SnowballRightAnchor(Clone)", "LMACF. RIGHT.", Val, Pos, Rot, Color.white, false);
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }

        public static void WaterBalloonGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (projModsEnabled)
                {
                    Vector3 Val = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 8.33f;
                    Vector3 Pos = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position;
                    Quaternion Rot = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;

                    LaunchSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/WaterBalloonRightAnchor(Clone)", "LMAEY. RIGHT.", Val, Pos, Rot, Color.white, true);
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }

        public static void LavaRockGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (projModsEnabled)
                {
                    Vector3 Val = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 8.33f;
                    Vector3 Pos = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position;
                    Quaternion Rot = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;

                    LaunchSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/LavaRockAnchor(Clone)", "LMAGE. RIGHT.", Val, Pos, Rot, Color.white, false);
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }

        public static void VoteRockGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (projModsEnabled)
                {
                    Vector3 Val = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 8.33f;
                    Vector3 Pos = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position;
                    Quaternion Rot = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;

                    LaunchSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/VotingRockAnchor_RIGHT(Clone)", "LMAMT. RIGHT.", Val, Pos, Rot, Color.white, false);
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }

        public static void HalloweenCandyGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (projModsEnabled)
                {
                    Vector3 Val = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 8.33f;
                    Vector3 Pos = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position;
                    Quaternion Rot = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;

                    LaunchSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/TrickTreatFunctionalAnchorRIGHT Variant(Clone)", "LMAMO. RIGHT.", Val, Pos, Rot, Color.white, false);
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }

        public static void AppleGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (projModsEnabled)
                {
                    Vector3 Val = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 8.33f;
                    Vector3 Pos = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position;
                    Quaternion Rot = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;

                    LaunchSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/AppleRightAnchor(Clone)", "LMAMV.", Val, Pos, Rot, Color.white, false);
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }

        public static void MentoGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (projModsEnabled)
                {
                    Vector3 Val = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 8.33f;
                    Vector3 Pos = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position;
                    Quaternion Rot = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;

                    LaunchSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/ScienceCandyRightAnchor(Clone)", "LMAIF. RIGHT.", Val, Pos, Rot, Color.white, true);
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }

        public static void GiftGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (projModsEnabled)
                {
                    Vector3 Val = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 8.33f;
                    Vector3 Pos = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position;
                    Quaternion Rot = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;

                    LaunchSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/BucketGiftFunctionalAnchor_Right(Clone)", "LMAHR. RIGHT.", Val, Pos, Rot, Color.white, false);
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }

        public static void FishFoodGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (projModsEnabled)
                {
                    Vector3 Val = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 8.33f;
                    Vector3 Pos = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position;
                    Quaternion Rot = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;

                    LaunchSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/FishFoodRightAnchor(Clone)", "LMAIP. RIGHT.", Val, Pos, Rot, Color.white, false);
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }
        #endregion

        #region Impact

        static float impactDelay;

        public static void Impact(Vector3 position, float red, float green, float blue)
        {
            const float delayInterval = 0.2f;
            const byte eventCode = 3;

            if (Time.time <= impactDelay) return;

            impactDelay = Time.time + delayInterval;

            var impactData = new object[] { position, red, green, blue, 1f, 1 };
            var eventData = new object[] { PhotonNetwork.ServerTimestamp, (byte)1, impactData };

            try
            {
                PhotonNetwork.RaiseEvent(eventCode, eventData, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);
            }
            catch { }
        }

        public static void ImpactSpam()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                Impact(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, Random.Range(0f, 255f), Random.Range(0f, 255f), Random.Range(0f, 255f));
            }

            if (ControllerInputPoller.instance.leftGrab)
            {
                Impact(GorillaTagger.Instance.offlineVRRig.leftHandTransform.position, Random.Range(0f, 255f), Random.Range(0f, 255f), Random.Range(0f, 255f));
            }
        }

        public static void ImpactOthers()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                List<VRRig> randomRigs = new List<VRRig>();

                foreach (VRRig rigs in GorillaParent.instance.vrrigs)
                {
                    randomRigs.Add(rigs);

                    if (randomRigs.Count > 0)
                    {
                        VRRig randomRig = randomRigs[Random.Range(0, randomRigs.Count)];

                        if (randomRig != GorillaTagger.Instance.offlineVRRig)
                        {
                            Impact(randomRig.headConstraint.position, Random.Range(0f, 255f), Random.Range(0f, 255f), Random.Range(0f, 255f));
                        }
                    }
                }
            }
        }

        public static void ImpactAura()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                int impactCount = 20;
                float radius = 1.5f;
                Transform playerTransform = GorillaTagger.Instance.offlineVRRig.transform;

                for (int i = 0; i < impactCount; i++)
                {
                    Vector3 randomDirection = Random.onUnitSphere * radius;
                    Vector3 impactPosition = playerTransform.position + randomDirection;

                    Impact(impactPosition, Random.Range(0f, 255f), Random.Range(0f, 255f), Random.Range(0f, 255f));
                }
            }
        }

        public static void ImpactGun()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo))
                {
                    GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    GunSphere.transform.position = hitinfo.point;
                    GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                    GunSphere.GetComponent<Renderer>().material.color = Color.white;
                    GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                    GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                    GameObject.Destroy(GunSphere.GetComponent<Collider>());

                    VRRig player = hitinfo.collider.GetComponent<VRRig>();

                    if (player != null)
                    {
                        if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                        {
                            GameObject.Destroy(GunSphere, Time.deltaTime);
                            GunSphere.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
                            GunSphere.transform.position = player.transform.position;

                            Impact(player.headConstraint.transform.position, Random.Range(0f, 255f), Random.Range(0f, 255f), Random.Range(0f, 255f));
                        }
                    }
                }
            }
            if (GunSphere != null)
            {
                GameObject.Destroy(GunSphere, Time.deltaTime);
            }
        }


        #endregion

        #region Server
        static bool AutoOpen;

        public static void ViewAllDates()
        {
            List<VRRig> vrRigs = GorillaParent.instance.vrrigs;
            var photonPlayers = PhotonNetwork.PlayerList;

            if (vrRigs == null || photonPlayers == null)
            {
                return;
            }

            foreach (VRRig player in vrRigs)
            {
                Photon.Realtime.Player matchingPhotonPlayer = photonPlayers.FirstOrDefault(p => p.NickName == player.playerNameVisible);

                if (matchingPhotonPlayer == null || string.IsNullOrEmpty(matchingPhotonPlayer.UserId))
                {
                    continue;
                }

                string playerCustomID = matchingPhotonPlayer.UserId;

                PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
                {
                    PlayFabId = playerCustomID
                },
                (GetAccountInfoResult result) =>
                {
                    string creationDate = result.AccountInfo.Created.ToString("yyyy-MM-dd");

                    if (player.playerText1 != null)
                    {
                        player.playerText1.text = $"{player.playerNameVisible}\n DATE: {creationDate}\n USER ID: {matchingPhotonPlayer.UserId}";
                    }

                    if (player.playerText2 != null)
                    {
                        player.playerText2.text = $"{player.playerNameVisible}\n DATE: {creationDate}\n USER ID: {matchingPhotonPlayer.UserId}";
                    }
                },
                (PlayFabError error) =>
                {
                    Debug.LogError($"Failed to get account info for {player.playerNameVisible}: {error.GenerateErrorReport()}");
                });
            }
        }

        public static void GrabAllIDS()
        {
            string text = "=======================PLAYER INFO!=========================";

            foreach (Photon.Realtime.Player players in PhotonNetwork.PlayerListOthers)
            {
                string playerName = players.NickName;
                string playerID = players.UserId;

                text += $"\nName: {playerName}, ID: {playerID}\n\n";
            }

            text += "\n==========================================================\n";

            if (!Directory.Exists("J0ker Menu"))
            {
                Directory.CreateDirectory("J0ker Menu");
            }
            File.AppendAllText("J0ker Menu/Player Info.txt", text);
        }

        public static void GrabAllBlockIDS()
        {
            string text = "=======================Block INFO!=========================";

            foreach (BuilderPiece piece in Object.FindObjectsOfType(typeof(BuilderPiece)))
            {
                string BlockName = piece.displayName;
                string BlockID = piece.pieceId.ToString();
                string BlockType = piece.pieceType.ToString();
                string BlockMatType = piece.materialType.ToString();

                text += $"\nBlock Name: {BlockName} | Block ID: {BlockID} | Block Type: {BlockType} | Material Type: {BlockMatType}\n\n";
            }

            text += "\n==========================================================\n";
            if (!Directory.Exists("J0ker Menu"))
            {
                Directory.CreateDirectory("J0ker Menu");
            }
            File.AppendAllText("J0ker Menu/Block Info.txt", text);
        }

        public static void GrabAllSoundIDS()
        {
            string text = "=======================Sound INFO!=========================";

            foreach (GorillaLocomotion.Player.MaterialData materialData in GorillaLocomotion.Player.Instance.materialData)
            {
                string AudioName = materialData.matName;
                string Slide = materialData.slidePercent.ToString();
                string Sound = materialData.audio.name;

                text += $"\nName: {AudioName} | Slide: {Slide} | Type: {Sound}\n\n";
            }

            text += "\n==========================================================\n";
            if (!Directory.Exists("J0ker Menu"))
            {
                Directory.CreateDirectory("J0ker Menu");
            }
            File.AppendAllText("J0ker Menu/Sound Info.txt", text);
        }

        public static void GrabAllRPCS()
        {
            string text = "=======================RPC INFO!=========================";

            foreach (string rpc in PhotonNetwork.PhotonServerSettings.RpcList)
            {
                text += $"RPC: {rpc}\n\n";
            }

            text += "\n==========================================================\n";
            if (!Directory.Exists("J0ker Menu"))
            {
                Directory.CreateDirectory("J0ker Menu");
            }
            File.AppendAllText("J0ker Menu/RPC Info.txt", text);
        }

        #endregion

        #region GameMode

        public static void SetMode(string GameMode)
        {
            // Find Modes In DNSPY (GameModeType)

            foreach (WatchableStringSO stringSO in Resources.FindObjectsOfTypeAll<WatchableStringSO>())
            {
                stringSO.Value = GameMode.ToUpper();
                stringSO.InitialValue = GameMode.ToUpper();
            }
        }
        #endregion
    }
}