using BepInEx;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaNetworking.Store;
using GorillaTagScripts;
using HarmonyLib;
using J0kerMenu_GTAG.Mod_Menu.Patching;
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
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;
using GameMode = GorillaGameModes.GameMode;

namespace J0kerMenu_GTAG.Menu
{
    public class Mods : MonoBehaviourPunCallbacks
    {
        static GameObject GunSphere;
        static VRRig lockedTarget = null;

        #region Player
        static float MatChangeSpeed;
        private static float keyboardDelay;
        static bool isOnPC;

        public static void Fly()
        {
            PCHelp();
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * 15;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            if (isOnPC) BasicWASD();
        }

        static void BasicWASD()
        {
            GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().useGravity = false;
            bool jump = false;
            int jumpint = 0;
            float Up;
            float Down;
            if (UnityInput.Current.GetKey(KeyCode.LeftShift))
            {
                Up = 40f;
                Down = -40f;
            }
            else
            {
                Up = 5f;
                Down = -5f;
            }
            if (UnityInput.Current.GetKey(KeyCode.LeftControl))
            {
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().AddForce(0f, -65f, 0f, ForceMode.Impulse);
            }
            if (UnityInput.Current.GetKey(KeyCode.Space))
            {
                jump = true;
            }
            if (UnityInput.Current.GetKey(KeyCode.W))
            {
                GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent").transform.position +=
                    GorillaLocomotion.Player.Instance.bodyCollider.transform.forward * Time.deltaTime * Up;
            }
            if (UnityInput.Current.GetKey(KeyCode.S))
            {
                GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent").transform.position +=
                    GorillaLocomotion.Player.Instance.bodyCollider.transform.forward * Time.deltaTime * Down;
            }
            if (UnityInput.Current.GetKey(KeyCode.A))
            {
                GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent").transform.Rotate(0f, -.9f, 0f);
            }
            if (UnityInput.Current.GetKey(KeyCode.D))
            {
                GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent").transform.Rotate(0f, .9f, 0f);
            }
            while (jump)
            {
                jumpint++;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().AddForce(0f, 65f, 0f, ForceMode.Impulse);
                jump = false;
            }
        }

        static void PCHelp()
        {
            if (UnityInput.Current.anyKey)
            {
                isOnPC = true;
            }
            else
            {
                isOnPC = false;
            }
        }

        #region RGB
        static float RGBDelay;
        static float r;
        static float g;
        static float b;

        public static void RGB()
        {
            if (Time.time > RGBDelay)
            {
                RGBDelay = Time.time + 0.1f;

                Rainbow();

                GorillaTagger.Instance.UpdateColor(r, g, b);

                if (PhotonNetwork.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { r, g, b });
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.InitializeNoobMaterialLocal(r, g, b);
                }
            }
        }

        static void Rainbow()
        {
            float amount = 0.1f;

            if (r == 1f)
            {
                if (b > 0f)
                {
                    b = Mathf.Clamp01(b - amount);
                    return;
                }
                if (g < 1f)
                {
                    g = Mathf.Clamp01(g + amount);
                    return;
                }
                r = Mathf.Clamp01(r - amount);
                return;
            }
            else if (g == 1f)
            {
                if (r > 0f)
                {
                    r = Mathf.Clamp01(r - amount);
                    return;
                }
                if (b < 1f)
                {
                    b = Mathf.Clamp01(b + amount);
                    return;
                }
                g = Mathf.Clamp01(g - amount);
                return;
            }
            else
            {
                if (b != 1f)
                {
                    r = Mathf.Clamp01(r + amount);
                    return;
                }
                if (g > 0f)
                {
                    g = Mathf.Clamp01(g - amount);
                    return;
                }
                if (r < 1f)
                {
                    r = Mathf.Clamp01(r + amount);
                    return;
                }
                b = Mathf.Clamp01(b - amount);
                return;
            }
        }
        #endregion

        public static void NoTagFreeze()
        {
            GorillaLocomotion.Player.Instance.disableMovement = false;
        }

        public static void GhostMonkey()
        {
            GorillaTagger.Instance.offlineVRRig.enabled = false;
            if (ControllerInputPoller.instance.rightControllerSecondaryButton || UnityInput.Current.GetKey(KeyCode.B))
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
            if (ControllerInputPoller.instance.rightControllerSecondaryButton || UnityInput.Current.GetKey(KeyCode.B))
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
            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetMouseButton(1))
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo);

                if (Mouse.current.rightButton.isPressed)
                {
                    Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                    Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out hitinfo, 100);
                }

                GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GunSphere.transform.position = hitinfo.point;
                GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                GunSphere.GetComponent<Renderer>().material.color = Color.white;
                GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                GameObject.Destroy(GunSphere.GetComponent<Collider>());

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetMouseButton(0))
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
            if (ControllerInputPoller.instance.rightGrab || Mouse.current.rightButton.isPressed)
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo);

                if (Mouse.current.rightButton.isPressed)
                {
                    Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                    Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out hitinfo, 100);
                }

                GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GunSphere.transform.position = hitinfo.point;
                GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                GunSphere.GetComponent<Renderer>().material.color = Color.white;
                GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                GameObject.Destroy(GunSphere.GetComponent<Collider>());

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || Mouse.current.leftButton.isPressed)
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

        public static void SetBodyType(GorillaBodyType type, bool Default)
        {
            GorillaBodyRenderer gorilla = GorillaTagger.Instance.offlineVRRig.gameObject.GetComponent<GorillaBodyRenderer>();

            gorilla.SetCosmeticBodyType(type);

            if (Default)
            {
                gorilla.SetMaterialIndex(0);
            }
        }

        public static void SetMatIndex()
        {
            GorillaBodyRenderer gorilla = GorillaTagger.Instance.offlineVRRig.gameObject.GetComponent<GorillaBodyRenderer>();

            if (Time.time > MatChangeSpeed)
            {
                MatChangeSpeed = Time.time + 0.089f;
                gorilla.SetMaterialIndex(Random.Range(0, 14));
            }
        }

        public static void ButtonClickerGun()
        {
            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetMouseButton(1))
            {
                RaycastHit hitinfo;

                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out hitinfo);

                if (Mouse.current.rightButton.isPressed)
                {
                    Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                    Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out hitinfo, 100);
                }

                GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GunSphere.transform.position = hitinfo.point;
                GunSphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                GunSphere.GetComponent<Renderer>().material.color = Color.white;

                GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                GameObject.Destroy(GunSphere.GetComponent<Collider>());

                GorillaPressableButton gorillaPressable = hitinfo.collider.GetComponentInParent<GorillaPressableButton>();

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetMouseButton(0))
                {
                    GunSphere.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;

                    if (gorillaPressable)
                    {
                        typeof(GorillaPressableButton).GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(gorillaPressable, new object[] { GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider").GetComponent<Collider>() });
                    }

                    GorillaKeyboardButton keyboardButton = hitinfo.collider.GetComponentInParent<GorillaKeyboardButton>();
                    if (keyboardButton && Time.time > keyboardDelay)
                    {
                        keyboardDelay = Time.time + 0.1f;
                        typeof(GorillaKeyboardButton).GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(keyboardButton, new object[] { GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider").GetComponent<Collider>() });
                    }

                    GorillaPlayerLineButton lineButton = hitinfo.collider.GetComponentInParent<GorillaPlayerLineButton>();
                    if (lineButton && Time.time > keyboardDelay)
                    {
                        keyboardDelay = Time.time + 0.1f;
                        typeof(GorillaPlayerLineButton).GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(lineButton, new object[] { GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider").GetComponent<Collider>() });
                    }

                    GameObject.Destroy(GunSphere, Time.deltaTime);
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
                if (rigs != GorillaTagger.Instance.offlineVRRig)
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
                if (ControllerInputPoller.instance.leftControllerGripFloat > 0.1f || UnityInput.Current.GetMouseButton(0))
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

                if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetMouseButton(1))
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

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
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
                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
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

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
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
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
            {
                if (Time.time > BlockDelay)
                {
                    BuilderPiece[] validPieces = Object.FindObjectsOfType<BuilderPiece>().Where(piece => piece.name.Contains("Terrain")).ToArray();

                    if (validPieces.Length > 0)
                    {
                        BuilderPiece randomPiece = validPieces[Random.Range(0, validPieces.Length)];
                        BuilderTable.instance.RequestCreatePiece(randomPiece.pieceType, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.rotation, randomPiece.materialType);
                        Flush();
                        BuilderPiecePatch.enabled = true;
                    }
                    BlockDelay = Time.time + 0.05f;
                }
            }
        }

        public static void BlockSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
            {
                if (Time.time > BlockDelay)
                {
                    BuilderPiece[] validPieces = Object.FindObjectsOfType<BuilderPiece>().Where(piece => piece.name.Contains("Wall") || piece.name.Contains("Floor") || piece.name.Contains("Roof")).ToArray();

                    if (validPieces.Length > 0)
                    {
                        BuilderPiece randomPiece = validPieces[Random.Range(0, validPieces.Length)];
                        BuilderTable.instance.RequestCreatePiece(randomPiece.pieceType, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.rotation, randomPiece.materialType);
                        BuilderPiecePatch.enabled = true;
                        Flush();
                    }
                    BlockDelay = Time.time + 0.05f;
                }
            }
        }

        public static void NewSetBlockSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
            {
                if (Time.time > BlockDelay)
                {
                    BuilderPiece[] winterPieces = Object.FindObjectsOfType<BuilderPiece>().Where(piece => piece.displayName.Contains("Winter")).ToArray();
                    if (winterPieces.Length > 0)
                    {
                        BuilderPiece randomPiece = winterPieces[Random.Range(0, winterPieces.Length)];
                        BuilderTable.instance.RequestCreatePiece(randomPiece.pieceType, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.rotation, randomPiece.materialType);
                        BuilderPiecePatch.enabled = true;
                        Flush();
                    }
                    BlockDelay = Time.time + 0.05f;
                }
            }
        }
        #endregion

        #region Sounds
        static float SoundDelay;
        static bool ShouldMute;
        public static void HandTapSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (Time.time > SoundDelay)
                {
                    SoundDelay = Time.time + 0.1f;
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                    {
                        1,
                        false,
                        1000f
                    });
                    Flush();
                }
            }
        }

        public static void PopSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (Time.time > SoundDelay)
                {
                    SoundDelay = Time.time + 0.1f;

                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                    {
                        84,
                        false,
                        1000f
                    });
                    Flush();
                }
            }
        }

        public static void EatSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (Time.time > SoundDelay)
                {
                    SoundDelay = Time.time + 0.1f;

                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                    {
                        86,
                        false,
                        1000f
                    });
                    Flush();
                }
            }
        }

        public static void RandomSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (Time.time > SoundDelay)
                {
                    SoundDelay = Time.time + 0.1f;

                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, new object[]
                    {
                        Random.Range(1, 360),
                        false,
                        1000f
                    });
                    Flush();
                }
            }
        }

        public static void MuteAll()
        {
            ShouldMute = !ShouldMute;

            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (!line.muteButton.isAutoOn)
                {
                    line.PressButton(ShouldMute, GorillaPlayerLineButton.ButtonType.Mute);
                }
            }
        }

        #endregion

        #region Party
        static float BracletDelay;
        static bool Enable;

        public static void BracletSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetMouseButton(0))
            {
                if (Time.time > BracletDelay)
                {
                    BracletDelay = Time.time + 0.1f;
                    GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[] { Enable = !Enable, false });
                    Flush();
                }
            }

            if (ControllerInputPoller.instance.leftControllerGripFloat > 0.1f || UnityInput.Current.GetMouseButton(1))
            {
                if (Time.time > BracletDelay)
                {
                    BracletDelay = Time.time + 0.1f;
                    GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[] { Enable = !Enable, true });
                    Flush();
                }
            }
        }

        public static void BracletAdd()
        {
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[] { true, false });

            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[] { true, true });
        }

        public static void BracletRemove()
        {
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[] { false, false });

            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[] { false, true });
        }

        public static void LeaveParty()
        {
            if (FriendshipGroupDetection.Instance.IsInParty) FriendshipGroupDetection.Instance.LeaveParty();
        }

        #endregion

        #region City
        static float FortuneDelay, DoorDelay, CosmeticDelay;

        public static void DoorSpammer()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
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
        public static void FortuneTellerSpammer()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
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

        public static void CosmeticSpam()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
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
        static float distanceToLeave = 0.35f, distanceToFling = 0.75f;
        public static bool playedReportSound;

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

            GorillaNot.instance.rpcErrorMax = int.MaxValue;
            GorillaNot.instance.rpcCallLimit = int.MaxValue;
            GorillaNot.instance.logErrorMax = int.MaxValue;

            PhotonNetwork.RemoveRPCsInGroup(int.MaxValue);
            GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);

            PhotonNetwork.SendAllOutgoingCommands();
        }

        public static void AntiReportLeave()
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
                                    PhotonNetwork.Disconnect();
                                    if (!playedReportSound)
                                    {
                                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(40, false, 1000f);
                                        playedReportSound = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public static void AntiReportFling()
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

                                if (DistanceRight < distanceToFling || DistanceLeft < distanceToFling)
                                {
                                    Vector3 Val = Vector3.forward * 999;
                                    Vector3 Pos = reportButton.position;
                                    Quaternion Rot = Quaternion.identity;

                                    LaunchGrowingSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/GrowingSnowballRightAnchor(Clone)", "LMACF. RIGHT.", Val, Pos, Rot, 5, true, true);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        static void AntiReportEvent(EventData data)
        {
            if (data.Code == 50)
            {
                if (data.CustomData is object[] array && array.Length > 0 && array[0] is string userId)
                {
                    if (userId == PhotonNetwork.LocalPlayer.UserId)
                    {
                        PhotonNetwork.Disconnect();

                        if (!playedReportSound)
                        {
                            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(40, false, 1000f);
                            playedReportSound = true;
                        }
                    }
                }
            }
        }

        public static void AntiStaff()
        {
            foreach (VRRig rigs in GorillaParent.instance.vrrigs)
            {
                if (rigs != GorillaTagger.Instance.offlineVRRig)
                {
                    if (rigs.concatStringOfCosmeticsAllowed.Contains("LBAAD.") || rigs.concatStringOfCosmeticsAllowed.Contains("LBAAK."))
                    {
                        PhotonNetwork.Disconnect();

                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(40, false, 1000f);
                    }
                }
            }
        }

        #endregion

        #region Projectiles 

        #region Throwable

        static GameObject gorillaVelocityEstimatorCustome;
        static GorillaVelocityEstimator scriptedGorillaVelEst;
        private PhotonEvent snowballThrowEvent;
        static bool ObjMade;
        static float projectileDelay;
        static bool projModsEnabled = false;
        static int changeSnowBall;

        public static void LaunchSnowBallProjectile(string Path, string Name, Vector3 velocity, Vector3 position, Quaternion rotation, Color color, bool RGB)
        {
            if (!ObjMade)
            {
                gorillaVelocityEstimatorCustome = new GameObject("GorillaVelocityEstimator (J0kerMenu)");
                scriptedGorillaVelEst = gorillaVelocityEstimatorCustome.AddComponent<GorillaVelocityEstimator>();
                ObjMade = true;
            }

            SnowballThrowable snowball = GameObject.Find(Path + "/").transform.Find(Name).GetComponent<SnowballThrowable>();
            if (!snowball.gameObject.activeSelf)
            {
                snowball.SetSnowballActiveLocal(true);
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
                MethodInfo launchMethod = typeof(SnowballThrowable).GetMethod("PerformSnowballThrowAuthority", BindingFlags.NonPublic | BindingFlags.Instance);
                launchMethod.Invoke(snowball, null);

                gorillaRigidbody.velocity = originalVelocity;
                snowball.transform.position = originalPosition;

                Flush();
            }
            if (!ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetKeyUp(KeyCode.G))
            {
                snowball.SetSnowballActiveLocal(false);
            }
        }

        public static void LaunchGrowingSnowBallProjectile(string Path, string Name, Vector3 velocity, Vector3 position, Quaternion rotation, int Size, bool onGun, bool shouldFling)
        {
            if (!ObjMade)
            {
                gorillaVelocityEstimatorCustome = new GameObject("GorillaVelocityEstimator (J0kerMenu)");
                scriptedGorillaVelEst = gorillaVelocityEstimatorCustome.AddComponent<GorillaVelocityEstimator>();
                ObjMade = true;
            }

            GrowingSnowballThrowable snowball = GameObject.Find(Path + "/").transform.Find(Name).GetComponent<GrowingSnowballThrowable>();
            if (!snowball.gameObject.activeSelf)
            {
                snowball.SetSnowballActiveLocal(true);
                snowball.velocityEstimator = scriptedGorillaVelEst;
                snowball.transform.position = position;
                snowball.transform.rotation = rotation;
                snowball.transform.localRotation = rotation;
                snowball.IncreaseSize(5);
            }

            if (Time.time > projectileDelay)
            {
                projectileDelay = Time.time + 0.2f;

                Rigidbody gorillaRigidbody = GorillaTagger.Instance.GetComponent<Rigidbody>();
                Vector3 originalVelocity = gorillaRigidbody.velocity;
                Vector3 originalPosition = snowball.transform.position;

                gorillaRigidbody.velocity = velocity;

                GorillaTagger.Instance.offlineVRRig.SetThrowableProjectileColor(true, Color.white);

                MethodInfo launchMethod = typeof(GrowingSnowballThrowable).GetMethod("PerformSnowballThrowAuthority", BindingFlags.NonPublic | BindingFlags.Instance);
                launchMethod.Invoke(snowball, null);

                gorillaRigidbody.velocity = originalVelocity;
                snowball.transform.position = originalPosition;

                Flush();
            }

            if (!onGun)
            {
                if (!ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetKeyUp(KeyCode.G))
                {
                    snowball.SetSnowballActiveLocal(false);
                }
            }

            if (shouldFling)
            {
                var photonEvent = Traverse.Create(snowball).Field("snowballThrowEvent").GetValue<PhotonEvent>();
                var args = new object[] { position, new Vector3(0f, -9999f, 0f), 5f };
                photonEvent.RaiseOthers(args);
            }
        }
    

        static void EnableAllProjs() // Auto enables if using projectiles
        {
            string[] projectilePaths = new string[]
            {
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/GrowingSnowballRightAnchor(Clone)/LMACF. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/WaterBalloonRightAnchor(Clone)/LMAEY. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/TrickTreatFunctionalAnchorRIGHT Variant(Clone)/LMAMO. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/AppleRightAnchor(Clone)/LMAMV.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/ScienceCandyRightAnchor(Clone)/LMAIF. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/BucketGiftFunctionalAnchor_Right(Clone)/LMAHR. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/VotingRockAnchor_RIGHT(Clone)/LMAMT. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/Holdables/FishFoodRightAnchor(Clone)/LMAIP. RIGHT."
            };

            string[] transferablePaths = new string[]
            {
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/GrowingSnowballRightAnchor(Clone)/LMACF. RIGHT.",
                "Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/WaterBalloonRightAnchor(Clone)/LMAEY. RIGHT.",
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
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (projModsEnabled)
                {
                    Vector3 Val = -GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 15.33f;
                    Vector3 Pos = GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.position;
                    Quaternion Rot = GorillaTagger.Instance.offlineVRRig.rightHandTransform.rotation;

                    LaunchGrowingSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/GrowingSnowballRightAnchor(Clone)", "LMACF. RIGHT.", Val, Pos, Rot, 999, false, false);
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }

        public static void WaterBalloonGun()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (projModsEnabled)
                {
                    Vector3 Val = -GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 15.33f;
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
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (projModsEnabled)
                {
                    Vector3 Val = -GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 15.33f;
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
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (projModsEnabled)
                {
                    Vector3 Val = -GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 15.33f;
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
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (projModsEnabled)
                {
                    Vector3 Val = -GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 15.33f;
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
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (projModsEnabled)
                {
                    Vector3 Val = -GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 15.33f;
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
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (projModsEnabled)
                {
                    Vector3 Val = -GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 15.33f;
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
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (projModsEnabled)
                {
                    Vector3 Val = -GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 15.33f;
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
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (projModsEnabled)
                {
                    Vector3 Val = -GorillaTagger.Instance.offlineVRRig.rightHandTransform.transform.up * 15.33f;
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

        public static void BigSnowBalls()
        {
            changeSnowBall = (changeSnowBall + 1) % 6;
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
            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetMouseButton(1))
            {
                Impact(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, Random.Range(150f, 255f), Random.Range(150f, 255f), Random.Range(150f, 255f));
            }

            if (ControllerInputPoller.instance.leftGrab || UnityInput.Current.GetMouseButton(0))
            {
                Impact(GorillaTagger.Instance.offlineVRRig.leftHandTransform.position, Random.Range(150f, 255f), Random.Range(150f, 255f), Random.Range(150f, 255f));
            }
        }

        public static void ImpactOthers()
        {
            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetKey(KeyCode.G))
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
            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetKey(KeyCode.G))
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
            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetMouseButton(1))
            {
                if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo))
                {

                    if (Mouse.current.rightButton.isPressed)
                    {
                        Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out hitinfo, 100);
                    }

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
                        if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || Mouse.current.leftButton.isPressed)
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
        public static float rejoinAfterKickDelay;
        public static string lastRoomName;

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

        public static void Reconnect()
        {
            lastRoomName = PhotonNetwork.CurrentRoom.Name;
            PhotonNetwork.Disconnect();
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

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
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
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
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
                if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetMouseButton(1))
                {
                    if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo))
                    {
                        if (Mouse.current.rightButton.isPressed)
                        {
                            Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                            Physics.Raycast(ray, out hitinfo, 100);
                        }

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
                            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetMouseButton(0))
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
                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
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

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
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

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
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
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetMouseButton(1))
            {
                if (Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo))
                {
                    if (Mouse.current.rightButton.isPressed)
                    {
                        Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                        Physics.Raycast(ray, out hitinfo, 100);
                    }

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
                        if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetMouseButton(0))
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
                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.T))
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
                    if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f || UnityInput.Current.GetKey(KeyCode.G))
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

        #region Ropes
        static float RopeDelay;

        static void RopeLaunch(Vector3 velocity)
        {
            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (Time.time > RopeDelay)
                {
                    RopeDelay = Time.time + 0.25f;

                    foreach (GorillaRopeSwing ropeSwing in Object.FindObjectsOfType(typeof(GorillaRopeSwing)))
                    {
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
                        {
                            ropeSwing.ropeId,
                            1,
                            velocity,
                            true,
                            null
                        });
                    }
                    Flush();
                }
            }
        }

        public static void RopesUp()
        {
            RopeLaunch(new Vector3(10f, 999f, 0f));
        }

        public static void RopesTweak()
        {
            RopeLaunch(new Vector3(-10f, -999f, 0f));
        }

        public static void RopesSpaz()
        {
            Vector3 Spaz = new Vector3(Random.Range(50f, -50f), Random.Range(50f, -5f), Random.Range(50f, -50f));
            RopeLaunch(Spaz);
        }

        public static void RopeGun()
        {
            if (ControllerInputPoller.instance.rightGrab || Mouse.current.rightButton.isPressed)
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo);

                if (Mouse.current.rightButton.isPressed)
                {
                    Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                    Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out hitinfo, 100);
                }

                GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GunSphere.transform.position = hitinfo.point;
                GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                GunSphere.GetComponent<Renderer>().material.color = Color.white;
                GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                GameObject.Destroy(GunSphere.GetComponent<Collider>());

                GorillaRopeSwing ropeSwing = hitinfo.collider.GetComponentInParent<GorillaRopeSwing>();
                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || Mouse.current.leftButton.isPressed)
                {
                    GameObject.Destroy(GunSphere, Time.deltaTime);
                    GunSphere.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;

                    if (ropeSwing && Time.time > RopeDelay)
                    {
                        RopeDelay = Time.time + 0.25f;
                        RopeSwingManager.instance.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { ropeSwing.ropeId, 1, new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)), true, null });

                        Flush();
                    }
                }
            }
            if (GunSphere != null)
            {
                GameObject.Destroy(GunSphere, Time.deltaTime);
            }
        }
        #endregion

        #region OP
        public static float partyKickDelay, flingAllDelay;

        public static void GroupKickAll()
        {
            if (!PhotonNetwork.CurrentRoom.IsVisible)
            {
                GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Remove(PhotonNetwork.LocalPlayer.UserId);
                GorillaComputer.instance.OnGroupJoinButtonPress(0, GorillaComputer.instance.friendJoinCollider);
                Reconnect();
            }
        }

        public static void PartyKickAll()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("J0kerModZ>>Kicked>>You", JoinType.ForceJoinWithParty);
        }

        public static void BlockLagAll()
        {
            if (ControllerInputPoller.instance.rightGrab || UnityInput.Current.GetKey(KeyCode.G))
            {
                if (Time.time > BlockDelay)
                {
                    BlockDelay = Time.time + 0.05f;

                    for (int i = 0; i < 15; i++)
                    {
                        BuilderTable.instance.RequestCreatePiece(1700948013, GorillaTagger.Instance.offlineVRRig.headConstraint.transform.position, Quaternion.identity, 0);
                    }

                    BuilderTable.instance.ClearTable();

                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(-124.1707f, 16.6173f, -226.5921f);
                    BuilderPiecePatch.enabled = true;
                    Flush();
                }
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void FlingGun()
        {
            if (ControllerInputPoller.instance.rightGrab || Mouse.current.rightButton.isPressed)
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo);

                if (Mouse.current.rightButton.isPressed)
                {
                    Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
                    Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                    Physics.Raycast(ray, out hitinfo, 100);
                }

                GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GunSphere.transform.position = hitinfo.point;
                GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                GunSphere.GetComponent<Renderer>().material.color = Color.white;
                GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                GameObject.Destroy(GunSphere.GetComponent<Collider>());
                VRRig rig = hitinfo.collider.GetComponentInParent<VRRig>();

                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || Mouse.current.leftButton.isPressed)
                {
                    if (lockedTarget == null && rig != null)
                    {
                        lockedTarget = rig;
                    }

                    if (lockedTarget != null)
                    {
                        GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                        GunSphere.GetComponent<Renderer>().material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
                        GunSphere.transform.position = lockedTarget.transform.position;
                    }
                    else
                    {
                        Destroy(GunSphere, Time.deltaTime);
                    }

                    if (projModsEnabled)
                    {

                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = lockedTarget.transform.position - new Vector3(0f, 0.6f, 0f);
                        cube.transform.localScale = new Vector3(2f, 0.1f, 2f);
                        Destroy(cube, Time.deltaTime);

                        GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                        GunSphere.GetComponent<Renderer>().material.color = Color.white;

                        Vector3 Val = Vector3.down * 999;
                        Vector3 Pos = lockedTarget != null ? lockedTarget.headConstraint.transform.position + new Vector3(0f, 0.1f, 0f) : Vector3.zero;
                        Quaternion Rot = Quaternion.identity;

                        LaunchGrowingSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/GrowingSnowballRightAnchor(Clone)", "LMACF. RIGHT.", Val, Pos, Rot, 5, true, true);
                    }
                    else
                    {
                        EnableAllProjs();
                    }
                }
                else
                {
                    lockedTarget = null;
                }
            }
            if (GunSphere != null)
            {
                GameObject.Destroy(GunSphere, Time.deltaTime);
            }
        }

        public static void FlingAllSnowBall()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                foreach (VRRig rRig in GorillaParent.instance.vrrigs)
                {
                    if (rRig != GorillaTagger.Instance.offlineVRRig)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = rRig.transform.position - new Vector3(0f, 0.6f, 0f);
                        cube.transform.localScale = new Vector3(1f, 0.1f, 1f);
                        Destroy(cube, Time.deltaTime);
                    }
                }

                VRRig randomRig = GorillaParent.instance.vrrigs[Random.Range(0, GorillaParent.instance.vrrigs.Count)];

                if (projModsEnabled)
                {
                    if (randomRig != GorillaTagger.Instance.offlineVRRig)
                    {
                        Vector3 Val = Vector3.down * 999f;
                        Vector3 Pos = randomRig.headConstraint.position;
                        Quaternion Rot = Quaternion.identity;

                        LaunchGrowingSnowBallProjectile("Player Objects/Local VRRig/Local Gorilla Player/RigAnchor/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/TransferrableItemRightHand/GrowingSnowballRightAnchor(Clone)", "LMACF. RIGHT.", Val, Pos, Rot, 5, false, true);
                    }
                }
                else
                {
                    EnableAllProjs();
                }
            }
        }

        #endregion
    }
}