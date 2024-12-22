using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaNetworking;
using GorillaNetworking.Store;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
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

        public static void UnCapVelocity()
        {
            GorillaLocomotion.Player.Instance.velocityLimit = float.MinValue;
        }

        public static void ReCapVelocity()
        {
            GorillaLocomotion.Player.Instance.velocityLimit = 0.3f;
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

        #region Players
        static bool DoTracers;

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
            else if (GameMode.ActiveGameMode.GameType() == GameModeType.FreezeTag)
            {
                foreach (VRRig rigs in GorillaParent.instance.vrrigs)
                {
                    if (rigs != GorillaTagger.Instance.offlineVRRig)
                    {
                        GorillaFreezeTagManager gorillaFreezeTag = FindObjectOfType<GorillaFreezeTagManager>();
                        if (rigs.mainSkin.material.name.Contains("Ice"))
                        {
                            rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            rigs.mainSkin.material.color = Color.red;
                        }
                        else if (gorillaFreezeTag.IsFrozen(rigs.OwningNetPlayer))
                        {
                            rigs.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                            rigs.mainSkin.material.color = Color.Lerp(Color.red, rigs.playerColor, Mathf.PingPong(Time.time, 1f));
                        }
                    }
                }
            }

            if (DoTracers)
            {
                LinesToPlayers();
            }
        }

        public static void disablePlayerESP()
        {
            foreach (VRRig rigs in GorillaParent.instance.vrrigs)
            {
                if (rigs != GorillaTagger.Instance.offlineVRRig)
                {
                    rigs.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                }
            }
        }

        static void LinesToPlayers()
        {
            foreach (VRRig rigs in GorillaParent.instance.vrrigs)
            {
                GameObject gameObject = new GameObject("PlayerLine");
                LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
                Color Color = rigs.playerColor;
                lineRenderer.startColor = Color;
                lineRenderer.endColor = Color;
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                lineRenderer.positionCount = 2;
                lineRenderer.useWorldSpace = true;
                lineRenderer.SetPosition(0, GorillaLocomotion.Player.Instance.rightControllerTransform.position);
                lineRenderer.SetPosition(1, rigs.bodyTransform.position);
                lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
                UnityEngine.Object.Destroy(lineRenderer, Time.deltaTime);
                UnityEngine.Object.Destroy(gameObject, Time.deltaTime);
            }
        }

        public static void EnableTracers()
        {
            DoTracers = true;
        }

        public static void DisableTracers()
        {
            DoTracers = false;
        }
        #endregion

        #region Bone
        static int[] GorillaBones = { 4, 3, 5, 4, 19, 18, 20, 19, 3, 18, 21, 20, 22, 21, 25, 21, 29, 21, 31, 29, 27, 25, 24, 22, 6, 5, 7, 6, 10, 6, 14, 6, 16, 14, 12, 10, 9, 7 };

        public static void BoneESP()
        {
            Material material = new Material(Shader.Find("GUI/Text Shader"));
            material.color = Color.red;

            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isOfflineVRRig && !vrrig.isMyPlayer)
                {
                    if (!vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>())
                    {
                        vrrig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
                    }
                    vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().endWidth = 0.025f;
                    vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().startWidth = 0.025f;
                    vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().material = material;
                    vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0, 0.160f, 0));
                    vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0, 0.4f, 0));

                    for (int i = 0; i < GorillaBones.Count(); i += 2)
                    {
                        if (!vrrig.mainSkin.bones[GorillaBones[i]].gameObject.GetComponent<LineRenderer>())
                        {
                            vrrig.mainSkin.bones[GorillaBones[i]].gameObject.AddComponent<LineRenderer>();
                        }
                        vrrig.mainSkin.bones[GorillaBones[i]].gameObject.GetComponent<LineRenderer>().endWidth = 0.025f;
                        vrrig.mainSkin.bones[GorillaBones[i]].gameObject.GetComponent<LineRenderer>().startWidth = 0.025f;
                        vrrig.mainSkin.bones[GorillaBones[i]].gameObject.GetComponent<LineRenderer>().material = material;
                        vrrig.mainSkin.bones[GorillaBones[i]].gameObject.GetComponent<LineRenderer>().SetPosition(0, vrrig.mainSkin.bones[GorillaBones[i]].position);
                        vrrig.mainSkin.bones[GorillaBones[i]].gameObject.GetComponent<LineRenderer>().SetPosition(1, vrrig.mainSkin.bones[GorillaBones[i + 1]].position);
                    }
                }
            }

            if (DoTracers)
            {
                LinesToPlayers();
            }
        }

        public static void disableBoneESP()
        {
            foreach (VRRig vrrigs in GorillaParent.instance.vrrigs)
            {
                if (!vrrigs.isOfflineVRRig && !vrrigs.isMyPlayer)
                {
                    for (int i = 0; i < GorillaBones.Count(); i += 2)
                    {
                        if (vrrigs.mainSkin.bones[GorillaBones[i]].gameObject.GetComponent<LineRenderer>())
                        {
                            UnityEngine.Object.Destroy(vrrigs.mainSkin.bones[GorillaBones[i]].gameObject.GetComponent<LineRenderer>());
                        }
                        if (vrrigs.head.rigTarget.gameObject.GetComponent<LineRenderer>())
                        {
                            UnityEngine.Object.Destroy(vrrigs.head.rigTarget.gameObject.GetComponent<LineRenderer>());
                        }
                    }
                }
            }
        }
        #endregion

        #region Name
        public static void NameTagESP()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.isOfflineVRRig && !rig.isMyPlayer)
                {
                    GameObject nameTag = rig.transform.Find("Name Mod")?.gameObject;

                    nameTag = new GameObject("Name Mod");

                    TextMeshPro textMesh = nameTag.AddComponent<TextMeshPro>();
                    textMesh.text = rig.OwningNetPlayer.NickName + "\n USER ID: " + rig.OwningNetPlayer.UserId;
                    textMesh.font = Client.Font;
                    textMesh.fontSize = 2f;
                    textMesh.alignment = TextAlignmentOptions.Center;
                    textMesh.color = rig.playerColor;

                    nameTag.transform.SetParent(rig.transform);


                    UnityEngine.Object.Destroy(nameTag, Time.deltaTime);

                    Transform nameTagTransform = nameTag.transform;
                    nameTagTransform.position = rig.headConstraint.position + new Vector3(0f, 0.7f, 0f);
                    nameTagTransform.LookAt(Camera.main.transform.position);
                    nameTagTransform.Rotate(0f, 180f, 0f);

                    nameTag.GetComponent<TextMeshPro>().renderer.material.shader = Shader.Find("GUI/Text Shader");
                    nameTag.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
            }

            if (DoTracers)
            {
                LinesToPlayers();
            }
        }

        public static void disableNameTagESP()
        {
            Destroy(GameObject.Find("Name Mod"));
        }
        #endregion

        #region Box
        public static void BoxESP()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.isOfflineVRRig && !rig.isMyPlayer)
                {
                    if (rig == null) continue;

                    if (rig.transform.Find("ESPBox") != null) continue;

                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    box.name = "ESPBox";

                    box.transform.localScale = new Vector3(0.5f, 0.8f, 0.5f);

                    box.transform.SetParent(rig.transform);

                    box.transform.localPosition = Vector3.zero;

                    Renderer renderer = box.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material = new Material(Shader.Find("GUI/Text Shader"));
                        Color color = rig.playerColor;
                        color.a = 0.5f;
                        renderer.material.color = color;
                    }

                    Collider collider = box.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = false;
                    }
                }
            }

            if (DoTracers)
            {
                LinesToPlayers();
            }
        }

        public static void disableBoxESP()
        {
            Destroy(GameObject.Find("ESPBox"));
        }
        #endregion

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

        public static void FlingAllBlock()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
            {
                var randomRigs = GorillaParent.instance.vrrigs.OrderBy(x => UnityEngine.Random.value).Take(UnityEngine.Random.Range(1, GorillaParent.instance.vrrigs.Count + 1)).ToList();

                foreach (VRRig rig in randomRigs)
                {
                    if (Time.time > BlockDelay)
                    {
                        BuilderPiece[] validPieces = Object.FindObjectsOfType<BuilderPiece>().Where(piece => piece.name.Contains("Floor")).ToArray();

                        if (validPieces.Length > 0)
                        {
                            BuilderPiece randomPiece = validPieces[Random.Range(0, validPieces.Length)];
                            BuilderTable.instance.RequestCreatePiece(randomPiece.pieceType, rig.bodyTransform.position, Quaternion.identity, randomPiece.materialType);
                            Flush();
                        }
                        BlockDelay = Time.time + 0.2f;
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
            if (PhotonNetwork.IsMasterClient)
            {
                if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
                {
                    if (Time.time > FortuneDelay)
                    {
                        FortuneDelay = Time.time + 0.1f;
                        FortuneTeller fortuneTeller = FindObjectOfType<FortuneTeller>();
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
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (Time.time > DoorDelay)
                {
                    DoorDelay = Time.time + 0.5f;
                    GTDoor door = FindObjectOfType<GTDoor>();
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
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                FittingRoomButton[] buttons = Object.FindObjectsOfType<FittingRoomButton>();

                if (buttons.Length > 0)
                {
                    int randomIndex = Random.Range(0, buttons.Length);
                    if (Time.time > CosmeticDelay && buttons[randomIndex] != null)
                    {
                        CosmeticDelay = Time.time + 0.1f;
                        buttons[randomIndex].ButtonActivationWithHand(false);

                        if (buttons[randomIndex].buttonText.text == "N/A")
                        {
                            AddRandomToCart();
                        }
                    }
                }
            }
        }

        public static void AddRandomToCart()
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
                PhotonNetwork.NetworkingClient.EventReceived += AntiReportEvent;

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

        static void AntiReportEvent(EventData data) // Idk If This Works
        {
            if (data.Code == 50)
            {
                object[] array = (object[])data.CustomData;
                if ((string)array[0] == PhotonNetwork.LocalPlayer.UserId)
                {
                    PhotonNetwork.Disconnect();
                }
            }
        }

        #endregion

        #region Projectiles 

        #region Throwable
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

        #region Elf 
        public static void ElfMiniGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                EnableElf();

                ElfLauncher launcher = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/ElfLauncherAnchor(Clone)/LMANE.").GetComponent<ElfLauncher>();
                MethodInfo launchMethod = typeof(ElfLauncher).GetMethod("Shoot", BindingFlags.NonPublic | BindingFlags.Instance);
                launchMethod.Invoke(launcher, null);

                Flush();
            }
        }

        public static void LaunchElf()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                EnableElf();

                ElfLauncher launcher = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/TransferrableItemRightShoulder/DropZoneAnchor/ElfLauncherAnchor(Clone)/LMANE.").GetComponent<ElfLauncher>();

                Vector3 randomSpread = Quaternion.Euler(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 0f) * GorillaTagger.Instance.rightHandTransform.forward;

                var args = new object[]
                {
                    (int)Traverse.Create(((RubberDuckEvents)Traverse.Create(launcher).Field("_events").GetValue()).Activate).Field("_eventId").GetValue(), GorillaTagger.Instance.rightHandTransform.position, randomSpread
                };

                PhotonNetwork.RaiseEvent(176, args, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = false, Encrypt = true });
                Flush();
            }
        }

        public static void ElfExplode()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                EnableElf();

                ElfLauncher launcher = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/TransferrableItemRightShoulder/DropZoneAnchor/ElfLauncherAnchor(Clone)/LMANE.").GetComponent<ElfLauncher>();

                Vector3 randomSpread = Quaternion.Euler(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f)) * Vector3.down;

                var args = new object[]
                {
                    (int)Traverse.Create(((RubberDuckEvents)Traverse.Create(launcher).Field("_events").GetValue()).Activate).Field("_eventId").GetValue(), GorillaTagger.Instance.rightHandTransform.position, randomSpread
                };

                PhotonNetwork.RaiseEvent(176, args, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = false, Encrypt = true });
                Flush();
            }
        }

        public static void LaunchElfRain()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                EnableElf();

                ElfLauncher launcher = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/TransferrableItemRightShoulder/DropZoneAnchor/ElfLauncherAnchor(Clone)/LMANE.").GetComponent<ElfLauncher>();
                TryOnBundleButton activeButton = GameObject.Find("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab/CosmeticsRoomAnchor/nicegorillastore_prefab/DressingRoom_Mirrors_Prefab/TryOnStand/Console Center/Bottom/BundleButton Group 1/BundleFittingRoomButton-1").GetComponent<TryOnBundleButton>();

                Vector3 randomPosition = GorillaTagger.Instance.headCollider.transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(2f, 2f), Random.Range(-2f, 2f));
                Vector3 randomSpread = Quaternion.Euler(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 0f) * Vector3.down;

                var args = new object[]
                {
                    (int)Traverse.Create(((RubberDuckEvents)Traverse.Create(launcher).Field("_events").GetValue()).Activate).Field("_eventId").GetValue(), randomPosition, randomSpread
                };

                PhotonNetwork.RaiseEvent(176, args, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = false, Encrypt = true });
                Flush();

                if (activeButton != null && !activeButton.isOn)
                {
                    activeButton.ButtonActivationWithHand(false);
                }
            }
        }

        static void EnableElf()
        {
            if (!GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed.Contains("LMANE."))
            {
                var activeButton = GameObject.Find("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab/CosmeticsRoomAnchor/nicegorillastore_prefab/DressingRoom_Mirrors_Prefab/TryOnStand/Console Center/Bottom/BundleButton Group 1/BundleFittingRoomButton-1");

                var button = activeButton.GetComponent<TryOnBundleButton>();
                if (button != null && !button.isOn)
                {
                    button.ButtonActivationWithHand(false);
                }
            }
        }

        #endregion

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
                Impact(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, Random.Range(150f, 255f), Random.Range(150f, 255f), Random.Range(150f, 255f));
            }

            if (ControllerInputPoller.instance.leftGrab)
            {
                Impact(GorillaTagger.Instance.offlineVRRig.leftHandTransform.position, Random.Range(150f, 255f), Random.Range(150f, 255f), Random.Range(150f, 255f));
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
                            Impact(randomRig.headConstraint.position, Random.Range(150f, 255f), Random.Range(150f, 255f), Random.Range(150f, 255f));
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

                    Impact(impactPosition, Random.Range(150f, 255f), Random.Range(150f, 255f), Random.Range(150f, 255f));
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

                            Impact(player.headConstraint.transform.position, Random.Range(150f, 255f), Random.Range(150f, 255f), Random.Range(150f, 255f));
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
            foreach (WatchableStringSO stringSO in Resources.FindObjectsOfTypeAll<WatchableStringSO>())
            {
                stringSO.Value = GameMode.ToUpper();
                stringSO.InitialValue = GameMode.ToUpper();
            }
        }

        #region Infection
        public static void TagAll()
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.Infection)
            {
                bool foundNonTaggedPlayer = false;
                Vector3 targetPosition = Vector3.zero;
                VRRig targetPlayer = null;

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    foreach (VRRig players in GorillaParent.instance.vrrigs)
                    {
                        if (players != GorillaTagger.Instance.offlineVRRig)
                        {
                            if (!players.mainSkin.material.name.Contains("fected"))
                            {
                                foundNonTaggedPlayer = true;

                                targetPlayer = players;
                                targetPosition = players.transform.position;

                                GorillaTagger.Instance.offlineVRRig.enabled = false;
                            }
                        }
                    }

                    if (foundNonTaggedPlayer)
                    {
                        GorillaTagger.Instance.offlineVRRig.transform.position = targetPosition;

                        if (targetPlayer != null)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                GorillaGameModes.GameMode.ReportTag(targetPlayer.OwningNetPlayer);
                            }
                        }
                    }
                    else
                    {
                        GorillaTagger.Instance.offlineVRRig.enabled = true;
                    }
                }
            }
        }

        public static void TagAura()
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.Infection)
            {
                float tagDistance = 3f;
                Vector3 forwardDirection = GorillaTagger.Instance.offlineVRRig.head.rigTarget.forward;

                VRRig closestPlayer = null;
                float closestDistance = Mathf.Infinity;

                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    Vector3 targetPosition = vrrig.headMesh.transform.position;
                    Vector3 playerPosition = GorillaTagger.Instance.offlineVRRig.head.rigTarget.position;
                    Vector3 toTarget = targetPosition - playerPosition;

                    float playerDistance = toTarget.magnitude;

                    if (!vrrig.mainSkin.material.name.Contains("fected") && playerDistance < tagDistance && Vector3.Dot(forwardDirection, toTarget.normalized) > 0)
                    {
                        if (playerDistance < closestDistance)
                        {
                            closestPlayer = vrrig;
                            closestDistance = playerDistance;
                        }
                    }
                }

                if (closestPlayer != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        GorillaGameModes.GameMode.ReportTag(closestPlayer.OwningNetPlayer);
                    }
                }
            }
        }


        public static void TagGun()
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.Infection)
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

                                for (int i = 0; i < 4; i++)
                                {
                                    GorillaGameModes.GameMode.ReportTag(player.OwningNetPlayer);
                                }
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
        }

        public static void TagSelf()
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.Infection)
            {
                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    GorillaTagManager gorillaTagManager = FindObjectOfType<GorillaTagManager>();

                    foreach (VRRig rigs in GorillaParent.instance.vrrigs)
                    {
                        if (rigs.mainSkin.material.name.Contains("fected"))
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = false;
                            GorillaTagger.Instance.offlineVRRig.transform.position = rigs.rightHandTransform.position;

                            for (int i = 0; i < 4; i++)
                            {
                                GorillaGameModes.GameMode.ReportTag(PhotonNetwork.LocalPlayer);
                            }
                        }

                        if (gorillaTagManager.IsInfected(GorillaTagger.Instance.myVRRig.Owner))
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
        #endregion

        #region Freeze Tag
        static GorillaFreezeTagManager cachedFreezeTagManager;

        public static void FreezeAll()
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.FreezeTag)
            {
                bool foundNonTaggedPlayer = false;
                Vector3 targetPosition = Vector3.zero;

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    GetFreezeTag();

                    foreach (VRRig players in GorillaParent.instance.vrrigs)
                    {
                        if (players != GorillaTagger.Instance.offlineVRRig)
                        {
                            if (!players.mainSkin.material.name.Contains("Ice") && !cachedFreezeTagManager.IsFrozen(players.OwningNetPlayer))
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
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void UnFreezeAll()
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.FreezeTag)
            {
                bool foundNonTaggedPlayer = false;
                Vector3 targetPosition = Vector3.zero;

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    GetFreezeTag();

                    foreach (VRRig players in GorillaParent.instance.vrrigs)
                    {
                        if (players != GorillaTagger.Instance.offlineVRRig)
                        {
                            if (!GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("Ice") && cachedFreezeTagManager.IsFrozen(players.OwningNetPlayer))
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
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
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

                    VRRig player = hitinfo.collider.GetComponent<VRRig>();

                    if (player != null)
                    {
                        if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                        {
                            GameObject.Destroy(GunSphere, Time.deltaTime);
                            GunSphere.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
                            if (GameMode.ActiveGameMode.GameType() == GameModeType.FreezeTag)
                            {
                                GorillaTagger.Instance.offlineVRRig.enabled = false;

                                GorillaTagger.Instance.offlineVRRig.transform.position = player.transform.position;
                                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = player.transform.position;
                                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = player.transform.position;

                                GorillaLocomotion.Player.Instance.rightControllerTransform.position = player.transform.position;
                            }
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

        public static void FreezeSelf()
        {
            if (GameMode.ActiveGameMode.GameType() == GameModeType.FreezeTag)
            {
                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
                {
                    GetFreezeTag();

                    foreach (VRRig rigs in GorillaParent.instance.vrrigs)
                    {
                        if (rigs.mainSkin.material.name.Contains("Ice") && !cachedFreezeTagManager.IsFrozen(GorillaTagger.Instance.myVRRig.Owner))
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = false;
                            GorillaTagger.Instance.offlineVRRig.transform.position = rigs.rightHandTransform.position;
                        }
                    }

                    if (cachedFreezeTagManager.IsFrozen(GorillaTagger.Instance.myVRRig.Owner))
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

        static void GetFreezeTag()
        {
            if (cachedFreezeTagManager == null)
            {
                cachedFreezeTagManager = FindObjectOfType<GorillaFreezeTagManager>();
            }
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

        public static void DropAll()
        {
            foreach (VRRig plr in GorillaParent.instance.vrrigs)
            {
                if (plr == null || plr == GorillaTagger.Instance.offlineVRRig) continue;

                NetworkView netView = GetNetworkView(plr);

                if (netView != null)
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
        #endregion

        #endregion

        #region Name

        public static void SetName(string PlayerName, bool Random)
        {
            if (Random)
            {
                if (NetworkSystem.Instance.PlayerListOthers.Length > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, NetworkSystem.Instance.PlayerListOthers.Length);
                    NetPlayer randomPlayer = NetworkSystem.Instance.PlayerListOthers[randomIndex];
                    PlayerName = randomPlayer.NickName;
                }
            }

            GorillaComputer.instance.currentName = PlayerName;
            PhotonNetwork.LocalPlayer.NickName = PlayerName;
            GorillaComputer.instance.offlineVRRigNametagText.text = PlayerName;
            GorillaTagger.Instance.offlineVRRig.playerText1.text = PlayerName;
            GorillaTagger.Instance.offlineVRRig.playerText2.text = PlayerName;
            GorillaComputer.instance.savedName = PlayerName;
            PlayerPrefs.SetString("playerName", PlayerName);
            PlayerPrefs.Save();
        } 

        #endregion
    }
}