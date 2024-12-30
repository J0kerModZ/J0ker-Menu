using J0kerMenu_GTAG.Menu;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using HarmonyLib;
using System.Collections;
using UnityEngine.Networking;
using BepInEx;
using UnityEngine.InputSystem;

namespace J0kerMenu_GTAG
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "LateUpdate", MethodType.Normal)]
    public class Client : MonoBehaviour
    {
        #region Shader/Text
        static Shader MenuShader = Shader.Find("GorillaTag/UberShader");
        public static TMP_FontAsset Font = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdtext").GetComponent<TextMeshPro>().font;
        #endregion

        #region Menu Var
        public static GameObject MenuPrefab, MenuPointer, textObject, spherePrefab;
        public static SphereCollider pointerCollider;
        static TextMeshPro textMesh;
        static bool MenuActive = false;
        static string Catagory;
        static string[] categories = { "Player", "Rig", "Tag", "Water", "Sounds", "Party", "City", "Guardian", "Projectile", "Projectile", "Impact", "Freeze Tag", "Blocks", "PaintBrawl", "Mode", "Info", "Name", "ESP", "Body", "Ropes", "Misc" };
        static string checkMenuVersion = ""; // Prevents You From Having A Outdated Menu
        static bool PCMenu;
        #endregion

        #region Buttons Var
        static List<string> buttonTexts = new List<string>
        {
            "Fly [B]",
            "Platforms",
            "UnCap Velocity",
            "No Tag Freeze",
            "Ghost Monkey [B]",
            "Invis Monkey [B]",
            "TP Gun",
            "Rig Gun",
            "Tag All [T]",
            "Tag Aura",
            "Tag Gun",
            "Tag Self [T]",
            "Water Spam Hands [G]",
            "Water Spam Body [T]",
            "Water Spam Head [T]",
            "Water Spam Aura [T]",
            "Hand Tap Spammer [G]",
            "Pop Spammer [G]",
            "Eat Spammer [G]",
            "Random Spammer [G]",
            "Braclet Spammer [G]",
            "Braclet Add",
            "Braclet Remove",
            "Leave Party",
            "Fortune Teller Spaz [G] [M]",
            "Basement Door Spam [G]",
            "Cosmetic Spam [G] [Try On]",
            "Add Random [Try On]",
            "Grab All",
            "Drop All",
            "Fling All [G]",
            "Get Guardian",
            "SnowBall Launcher [G]",
            "WaterBalloon Launcher [G]",
            "Halloween Candy Launcher [G]",
            "Fish Food Launcher [G]",
            "Vote Rock Launcher [G]",
            "Apple Launcher [G]",
            "Mento Launcher [G]",
            "Gift Launcher [G]",
            "Impact Spam [G]",
            "Impact Others [G]",
            "Impact Aura [G]",
            "Impact Gun",
            "Freeze All [T]",
            "Un Freeze All [T]",
            "Freeze Gun",
            "Freeze Self [T]",
            "Big Block Spammer [T]",
            "Block Spammer [T]",
            "Block Spammer [T] [XMAS]",
            "Block Launcher [G] [M]",
            "Auto Fire [G]",
            "Aimbot [G]",
            "Revive [M]",
            "Kill All [M]",
            "Set Mode [Paintbrawl]",
            "Set Mode [Ambush]",
            "Set Mode [Ghost]",
            "Set Mode [Hunt]",
            "Get Player Info",
            "Get Block Info",
            "Get Sound Info",
            "Get RPC Info",
            "Set Name [No Name]",
            "Set Name [J0kerMenu]",
            "Set Name [Random]",
            "Set Name [Custom] [GUI]",
            "Player ESP",
            "Bone ESP",
            "NameTag ESP",
            "Box ESP",
            "No Head [CS]",
            "Skeleton Body [CS]",
            "Material Changer [CS]",
            "Default Body [CS]",
            "Ropes Up [G]",
            "Rope Tweak [G]",
            "Rope Spaz [G]",
            "Fast Ropes",
            "Player Info [Name Tag]",
            "Allow Tracers [ESP]",
            "Button Click Gun",
            "Mute All [CS]",
            "Anti Report",
        };
        public static List<bool> buttonFlags = new List<bool>();
        public static List<GameObject> ButtonObjects = new List<GameObject>();
        static bool ButtonsCreated;
        #endregion

        #region Page var
        static int currentPage = 0, buttonsPerPage = 4;
        static float pageSwitchCooldown = 0.5f, lastPageSwitchTime = 0f;
        #endregion

        public void Start()
        {
            Panel();
            Pointer();
            UpdateButtonVisibility();
        }

        // I ADDED PC VIEW SUPPORT (THE CODE LOOKS LIKE SHIT NOW BUT IT WORKS!)
        public void Update()
        {
            if (checkMenuVersion == "")
            {
                StartCoroutine(CheckVersion());
                StartCoroutine(CheckLockDown());
            }

            if (checkMenuVersion == "Done")
            {
                if (ControllerInputPoller.instance.leftControllerSecondaryButton)
                {
                    MenuActive = true;
                }
                else
                {
                    if (!PCMenu)
                    {
                        MenuActive = false;
                    }
                }

                if (UnityInput.Current.GetKey(KeyCode.Q))
                {
                    PCMenu = true;
                    MenuActive = true;
                }
                else
                {
                    PCMenu = false;
                }
            }

            MenuPrefab.SetActive(MenuActive);
            MenuPointer.SetActive(MenuActive);

            if (!PCMenu)
            {
                MenuPrefab.transform.parent = GorillaLocomotion.Player.Instance.leftHandFollower;
                Vector3 offset = new Vector3(0.1f, 0f, 0.1f);
                MenuPrefab.transform.localPosition = offset;
                MenuPrefab.transform.localRotation = Quaternion.Euler(-90f, 180f, 0f);
            }
            else
            {

                MenuPrefab.transform.parent = null;
                MenuPrefab.transform.position = new Vector3(0.22f, -0.01f, 0.21f);
                MenuPrefab.transform.rotation = Quaternion.Euler(0f, 317.3493f, 0f);

                GameObject cam = GameObject.Find("Shoulder Camera");

                cam.transform.parent = null;
                cam.GetComponent<Cinemachine.CinemachineBrain>().enabled = false;
                cam.transform.position = Vector3.zero;
                cam.transform.rotation = Quaternion.Euler(0f, 47f, 0f);
            }

            if (UnityInput.Current.GetKeyUp(KeyCode.Q))
            {
                GameObject.Find("Shoulder Camera").transform.parent = GameObject.Find("Third Person Camera").transform;
                GameObject.Find("Shoulder Camera").GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
            }

            ButtonsActive();
            CatagoryHanlder();
            MenuNameHandler();

            if (MenuActive && ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f || UnityInput.Current.GetMouseButton(4))
            {
                NextPage();
            }

            if (MenuActive && ControllerInputPoller.instance.leftControllerIndexFloat > 0.1f || UnityInput.Current.GetMouseButton(3))
            {
                BackPage();
            }

            // Scale The Menu With The Player (DONT DO 0.5)
            if (GorillaTagger.Instance.offlineVRRig.gameObject.transform.localScale == new Vector3(3f, 3f, 3f))
            {
                MenuPrefab.transform.localScale = new Vector3(0.06f, 1.1f, 1.1f);
            }
            else if (GorillaTagger.Instance.offlineVRRig.gameObject.transform.localScale == new Vector3(1f, 1f, 1f))
            {
                MenuPrefab.transform.localScale = new Vector3(0.02f, 0.3f, 0.3f);
            }
        }

        static void ButtonsActive()
        {
            #region Color
            for (int i = 0; i < buttonFlags.Count; i++)
            {
                if (!buttonFlags[i])
                {
                    GameObject button = ButtonObjects[i];
                    button.GetComponent<Renderer>().material.color = Color.white;
                }
                else
                {
                    GameObject button = ButtonObjects[i];
                    button.GetComponent<Renderer>().material.color = Color.gray;
                }
            }
            #endregion

            if (buttonFlags[0]) Mods.Fly();

            if (buttonFlags[1]) Mods.Platforms();

            if (buttonFlags[2]) Mods.UnCapVelocity(); else Mods.ReCapVelocity();

            if (buttonFlags[3]) Mods.NoTagFreeze();

            if (buttonFlags[4]) Mods.GhostMonkey();

            if (buttonFlags[5]) Mods.InvisMonkey();

            if (buttonFlags[6]) Mods.TPGun();

            if (buttonFlags[7]) Mods.RigGun();

            if (buttonFlags[8]) Mods.TagAll();

            if (buttonFlags[9]) Mods.TagAura();

            if (buttonFlags[10]) Mods.TagGun();

            if (buttonFlags[11]) Mods.TagSelf();

            if (buttonFlags[12]) Mods.WaterSpamHands();

            if (buttonFlags[13]) Mods.WaterSpamBody();

            if (buttonFlags[14]) Mods.WaterSpamHead();

            if (buttonFlags[15]) Mods.WaterSpamAura();

            if (buttonFlags[16]) Mods.HandTapSpammer();

            if (buttonFlags[17]) Mods.PopSpammer();

            if (buttonFlags[18]) Mods.EatSpammer();

            if (buttonFlags[19]) Mods.RandomSpammer();

            if (buttonFlags[20]) Mods.BracletSpammer();

            if (buttonFlags[21]) Mods.BracletAdd(); buttonFlags[21] = false;

            if (buttonFlags[22]) Mods.BracletRemove(); buttonFlags[22] = false;

            if (buttonFlags[23]) Mods.LeaveParty(); buttonFlags[23] = false;

            if (buttonFlags[24]) Mods.FortuneTellerSpammer();

            if (buttonFlags[25]) Mods.DoorSpammer();

            if (buttonFlags[26]) Mods.CosmeticSpam();

            if (buttonFlags[27]) Mods.AddRandomToCart(); buttonFlags[27] = false;

            if (buttonFlags[28]) Mods.GrabAll(); buttonFlags[28] = false;

            if (buttonFlags[29]) Mods.DropAll(); buttonFlags[29] = false;

            if (buttonFlags[30]) Mods.FlingAll();

            if (buttonFlags[31]) Mods.GetGuardian();

            if (buttonFlags[32]) Mods.SnowBallGun();

            if (buttonFlags[33]) Mods.WaterBalloonGun();

            if (buttonFlags[34]) Mods.HalloweenCandyGun();

            if (buttonFlags[35]) Mods.FishFoodGun();

            if (buttonFlags[36]) Mods.VoteRockGun();

            if (buttonFlags[37]) Mods.AppleGun();

            if (buttonFlags[38]) Mods.MentoGun();

            if (buttonFlags[39]) Mods.GiftGun();

            if (buttonFlags[40]) Mods.ImpactSpam();

            if (buttonFlags[41]) Mods.ImpactOthers();

            if (buttonFlags[42]) Mods.ImpactAura();

            if (buttonFlags[43]) Mods.ImpactGun();

            if (buttonFlags[44]) Mods.FreezeAll();

            if (buttonFlags[45]) Mods.UnFreezeAll();

            if (buttonFlags[46]) Mods.FreezeTagGun();

            if (buttonFlags[47]) Mods.FreezeSelf();

            if (buttonFlags[48]) Mods.BigBlockSpammer();

            if (buttonFlags[49]) Mods.BlockSpammer();

            if (buttonFlags[50]) Mods.XmasBlockSpammer();

            if (buttonFlags[51]) Mods.BlockLauncher();

            if (buttonFlags[52]) Mods.AutoFire();

            if (buttonFlags[53]) Mods.Aimbot();

            if (buttonFlags[54]) Mods.Revive(); buttonFlags[54] = false;

            if (buttonFlags[55]) Mods.KillAll(); buttonFlags[55] = false;

            if (buttonFlags[56]) Mods.SetMode("Paintbrawl"); buttonFlags[56] = false;

            if (buttonFlags[57]) Mods.SetMode("Ambush"); buttonFlags[57] = false;

            if (buttonFlags[58]) Mods.SetMode("Ghost"); buttonFlags[58] = false;

            if (buttonFlags[59]) Mods.SetMode("Hunt"); buttonFlags[59] = false;

            if (buttonFlags[60]) Mods.GrabAllIDS(); buttonFlags[60] = false;

            if (buttonFlags[61]) Mods.GrabAllBlockIDS(); buttonFlags[61] = false;

            if (buttonFlags[62]) Mods.GrabAllSoundIDS(); buttonFlags[62] = false;

            if (buttonFlags[63]) Mods.GrabAllRPCS(); buttonFlags[63] = false;

            if (buttonFlags[64]) Mods.SetName("ㅤ", false); buttonFlags[64] = false;

            if (buttonFlags[65]) Mods.SetName("J0kerMenu", false); buttonFlags[65] = false;

            if (buttonFlags[66]) Mods.SetName("", true); buttonFlags[66] = false;

            if (buttonFlags[67]) Mods.SetName(customName, false);
            
            if (buttonFlags[68]) Mods.PlayerESP(); else Mods.disablePlayerESP();

            if (buttonFlags[69]) Mods.BoneESP(); else Mods.disableBoneESP();

            if (buttonFlags[70]) Mods.NameTagESP(); else Mods.disableNameTagESP();

            if (buttonFlags[71]) Mods.BoxESP(); else Mods.disableBoxESP();

            if (buttonFlags[72]) Mods.SetBodyType(GorillaBodyType.NoHead, false); buttonFlags[72] = false;

            if (buttonFlags[73]) Mods.SetBodyType(GorillaBodyType.Skeleton, false); buttonFlags[73] = false;

            if (buttonFlags[74]) Mods.SetMatIndex();

            if (buttonFlags[75]) Mods.SetBodyType(GorillaBodyType.Default, true); buttonFlags[75] = false;

            if (buttonFlags[76]) Mods.RopesUp();

            if (buttonFlags[77]) Mods.RopesTweak();

            if (buttonFlags[78]) Mods.RopesSpaz();

            if (buttonFlags[79]) Mods.FastRopes(); buttonFlags[79] = false;

            if (buttonFlags[80]) Mods.ViewAllDates(); buttonFlags[80] = false;

            if (buttonFlags[81]) Mods.EnableTracers(); else Mods.DisableTracers();

            if (buttonFlags[82]) Mods.ButtonClickerGun();

            if (buttonFlags[83]) Mods.MuteAll(); buttonFlags[83] = false;

            if (buttonFlags[84]) Mods.AntiReport();
        }

        static void Panel()
        {
            MenuPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(MenuPrefab.GetComponent<BoxCollider>());

            MenuPrefab.name = "Mod Menu";
            MenuPrefab.GetComponent<Renderer>().material.shader = MenuShader;
            MenuPrefab.GetComponent<Renderer>().material.color = Color.black;

            textObject = new GameObject("MenuText");
            textMesh = textObject.AddComponent<TextMeshPro>();

            CreateButtons(buttonTexts.Count);
        }

        static void Pointer()
        {
            MenuPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            MenuPointer.name = "Menu Pointer";
            MenuPointer.tag = "Pointer";
            MenuPointer.GetComponent<Renderer>().material.shader = MenuShader;

            pointerCollider = MenuPointer.AddComponent<SphereCollider>();
            pointerCollider.isTrigger = true;
            pointerCollider.radius = 0.05f;

            MenuPointer.transform.parent = GorillaLocomotion.Player.Instance.rightControllerTransform;
            MenuPointer.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            MenuPointer.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }

        static void CreateButtons(int numberOfButtons)
        {
            if (!ButtonsCreated)
            {
                float buttonHeight = 0.1f;
                float buttonSpacing = 0.08f;

                for (int i = 0; i < numberOfButtons; i++)
                {
                    GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    button.name = "Button " + (i + 1);
                    button.tag = "Button";
                    button.AddComponent<BoxCollider>().isTrigger = true;
                    button.GetComponent<Renderer>().material.shader = MenuShader;
                    button.GetComponent<Renderer>().material.color = Color.white;

                    button.transform.parent = MenuPrefab.transform;
                    button.transform.localScale = new Vector3(0.5f, 0.1f, 0.8f);
                    button.transform.localPosition = new Vector3(-0.6f, 0.2f - ((i % buttonsPerPage) * (buttonHeight + buttonSpacing)), 0f);

                    GameObject buttonTextObject = new GameObject("ButtonText");
                    TextMeshPro buttonText = buttonTextObject.AddComponent<TextMeshPro>();

                    buttonText.text = buttonTexts[i];
                    buttonText.fontSize = 8;
                    buttonText.font = Font;
                    buttonText.color = Color.black;
                    buttonText.alignment = TextAlignmentOptions.Center;

                    buttonTextObject.transform.SetParent(button.transform);
                    buttonTextObject.transform.localPosition = new Vector3(-0.6f, 0f, 0f);
                    buttonTextObject.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    buttonTextObject.transform.localScale = new Vector3(0.06f, 0.5f, 1f);

                    button.AddComponent<ButtonTrigger>();
                    ButtonObjects.Add(button);

                    ButtonTrigger buttonTrigger = button.GetComponent<ButtonTrigger>();
                    buttonTrigger.SetButtonIndex(i);

                    buttonFlags.Add(false);
                }
                ButtonsCreated = true;
            }
        }

        static void UpdateButtonVisibility()
        {
            for (int i = 0; i < ButtonObjects.Count; i++)
            {
                bool onCurrentPage = i / buttonsPerPage == currentPage;
                ButtonObjects[i].SetActive(onCurrentPage);
            }
        }

        static void NextPage()
        {
            if (Time.time - lastPageSwitchTime >= pageSwitchCooldown)
            {
                currentPage = (currentPage + 1) % Mathf.CeilToInt((float)buttonTexts.Count / buttonsPerPage);
                lastPageSwitchTime = Time.time;
                UpdateButtonVisibility();
            }
        }

        static void BackPage()
        {
            if (Time.time - lastPageSwitchTime >= pageSwitchCooldown)
            {
                if (currentPage > 0)
                {
                    currentPage = (currentPage - 1) % Mathf.CeilToInt((float)buttonTexts.Count / buttonsPerPage);
                }
                else
                {
                    currentPage = Mathf.CeilToInt((float)buttonTexts.Count / buttonsPerPage) - 1;
                }
                lastPageSwitchTime = Time.time;
                UpdateButtonVisibility();
            }
        }

        static void CatagoryHanlder()
        {
            if (currentPage >= 0 && currentPage < categories.Length)
            {
                Catagory = categories[currentPage];
            }
        }

        static void MenuNameHandler()
        {
            textMesh.text = $"<size=6>J0ker Menu: {Catagory}</size>\n====================";
            textMesh.fontSize = 8;
            textMesh.font = Font;
            textMesh.color = Color.white;
            textMesh.fontStyle = FontStyles.Italic;
            textMesh.alignment = TextAlignmentOptions.Center;

            textObject.transform.SetParent(MenuPrefab.transform);
            textObject.transform.localPosition = new Vector3(-0.6f, 0.40f, 0f);
            textObject.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            textObject.transform.localScale = Vector3.one * 0.1f;
        }

        #region Version Checker
        private string lockDownURL = "https://pastebin.com/raw/pxqcgumS";
        private string versionUrl = "https://pastebin.com/raw/HC97dXU1";
        private string expectedVersion = "1.0.1";
        private string shouldLockDown = "Safe";

        private IEnumerator CheckVersion()
        {
            UnityWebRequest request = UnityWebRequest.Get(versionUrl);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string latestVersion = request.downloadHandler.text.Trim();

                if (latestVersion == expectedVersion)
                {
                    checkMenuVersion = "Done";
                }
                else
                {
                    checkMenuVersion = "OutDated";

                    GameObject.Find("motd (1)").GetComponent<TextMeshPro>().text = "<color=red>OUTDATED!</color>";

                    TextMeshPro motdText = GameObject.Find("motdtext").GetComponent<TextMeshPro>();
                    motdText.text = "<color=red>THIS MENU IS OUT DATED PLEASE UPDATE THE MENU!</color>\n<size=150>j0kermodz.lol</size>";
                    motdText.alignment = TextAlignmentOptions.Center;
                    motdText.verticalAlignment = VerticalAlignmentOptions.Top;
                }
            }
        }

        private IEnumerator CheckLockDown()
        {
            UnityWebRequest request = UnityWebRequest.Get(lockDownURL);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string latestShouldLock = request.downloadHandler.text.Trim();

                if (latestShouldLock == shouldLockDown)
                {
                    checkMenuVersion = "Done";
                }
                else
                {
                    checkMenuVersion = "LOCKED";

                    GameObject.Find("motd (1)").GetComponent<TextMeshPro>().text = "<color=red>LOCKED DOWN!</color>";

                    TextMeshPro motdText = GameObject.Find("motdtext").GetComponent<TextMeshPro>();
                    motdText.text = "<color=red>THIS MENU IS LOCKED AS A MOD OR MENU GOT DETECTED!</color>\n<size=150>MORE INFO:\nj0kermodz.lol</size>";
                    motdText.alignment = TextAlignmentOptions.Center;
                    motdText.verticalAlignment = VerticalAlignmentOptions.Top;
                }
            }
        }
        #endregion

        #region Name GUI
        public static string customName;

        public void OnGUI()
        {
            if (!buttonFlags[67]) return;

            customName = GUILayout.TextArea(customName, GUILayout.Height(20));

            if (GUILayout.Button("Set Name", GUILayout.Height(20)))
            {
                Mods.SetName(customName, false);
                buttonFlags[67] = false;
            }
        }

        #endregion
    }

    public class ButtonTrigger : MonoBehaviour
    {
        public int buttonIndex = -1;
        private float cooldownTime = 0.5f;
        private float lastClickTime = 0f;
        public string ISclick;

        public void SetButtonIndex(int index)
        {
            buttonIndex = index;
        }

        public void Update()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            Camera cam = GameObject.Find("Shoulder Camera").GetComponent<Camera>();
            Ray ray = cam.ScreenPointToRay(mousePos);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                ButtonTrigger buttons = hit.transform.GetComponent<ButtonTrigger>();
                if (buttons != null)
                {
                    if (UnityInput.Current.GetMouseButton(0))
                    {
                        buttons.OnButtonClick();
                    }
                }
            }
        }

        public void OnButtonClick()
        {
            if (Time.time - lastClickTime >= cooldownTime)
            {
                if (buttonIndex >= 0 && buttonIndex < Client.buttonFlags.Count)
                {
                    Client.buttonFlags[buttonIndex] = !Client.buttonFlags[buttonIndex];
                }

                lastClickTime = Time.time;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == "Menu Pointer")
            {
                if (Time.time - lastClickTime >= cooldownTime)
                {
                    if (buttonIndex >= 0 && buttonIndex < Client.buttonFlags.Count)
                    {
                        Client.buttonFlags[buttonIndex] = !Client.buttonFlags[buttonIndex];
                    }

                    lastClickTime = Time.time;
                }
            }
        }
    }
}
