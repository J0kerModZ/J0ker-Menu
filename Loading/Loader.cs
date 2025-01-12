using BepInEx;
using UnityEngine;

namespace J0kerMenu_GTAG.Loading
{
    [BepInPlugin("com.J0kerModZ.J0kerGorillaTagMenu", "J0ker Menu", "1.0.0")]
    public class Loader : BaseUnityPlugin
    {
        private static GameObject IfActiveLoad;
        private static bool loaded = false;

        private void Update()
        {
            IfActiveLoad = GameObject.Find("Gameplay Scripts");
            if (IfActiveLoad != null && IfActiveLoad.activeInHierarchy && !loaded)
            {
                GameObject load = new GameObject("J0ker Loader");
                load.AddComponent<Client>();
                loaded = true;
            }
        }
    }
}
