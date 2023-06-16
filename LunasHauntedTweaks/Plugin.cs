using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Utilla;

namespace HMMLunasTweaks
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class HauntedModMenuPlugin : BaseUnityPlugin
    {
        private bool inRoom;
        private GameObject menuObject = null;

        private AssetBundle LoadFontBundle() {
            var Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LunasHauntedTweaks.Resources.hmmlunastweaksfont");
            return AssetBundle.LoadFromStream(Stream);
        }

        private void Start()
        {
            // usage of Awake() now locks the game up on boot so we need this to be in Start now :(
            var fontBundle = LoadFontBundle();
            if (fontBundle == null)
                return;

            Utils.RefCache.CustomFont = fontBundle.LoadAsset<Font>("BitCell");
            fontBundle.Unload(false);

            foreach (BepInEx.PluginInfo plugin in Chainloader.PluginInfos.Values)
            {
                BaseUnityPlugin modPlugin = plugin.Instance;
                Type type = modPlugin.GetType();
                DescriptionAttribute modDescription = type.GetCustomAttribute<DescriptionAttribute>();

                if (modDescription == null)
                    continue;

                if (modDescription.Description.Contains("HauntedModMenu"))
                {
                    var enableImp = AccessTools.Method(type, "OnEnable");
                    var disableImp = AccessTools.Method(type, "OnDisable");

                    if (enableImp != null && disableImp != null)
                        Utils.RefCache.ModList.Add(new Utils.ModInfo(modPlugin, plugin.Metadata.Name));
                }
            }

            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        private void OnEnable()
        {
            if (menuObject != null && inRoom)
                menuObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (menuObject != null)
                menuObject.SetActive(false);
        }

        private void OnGameInitialized(object sender, EventArgs e)
        {
            Utils.RefCache.LeftHandFollower = GorillaLocomotion.Player.Instance.leftHandFollower.gameObject;
            Utils.RefCache.RightHandFollower = GorillaLocomotion.Player.Instance.rightHandFollower.gameObject;
            Utils.RefCache.CameraTransform = GorillaLocomotion.Player.Instance.headCollider.transform;
            Utils.RefCache.PlayerTransform = GorillaLocomotion.Player.Instance.turnParent.transform;

            Utils.RefCache.LeftHandRig = GorillaTagger.Instance.offlineVRRig.leftHandTransform.parent.gameObject;
            Utils.RefCache.RightHandRig = GorillaTagger.Instance.offlineVRRig.rightHandTransform.parent.gameObject;
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin()
        {
            inRoom = true;

            if (menuObject != null)
                return;

            menuObject = CreateTrigger();

            if (menuObject != null)
            {
                menuObject.AddComponent<Menu.MenuController>();
                menuObject.SetActive(enabled && inRoom);
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave()
        {
            inRoom = false;
            Destroy(menuObject);
        }

        private GameObject CreateTrigger()
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (go == null)
                return null;

            Collider col = go.GetComponent<Collider>();
            if (col != null)
                col.isTrigger = true;

            MeshRenderer render = go.GetComponent<MeshRenderer>();
            if (render != null)
                Destroy(render);

            return go;
        }
    }
}
