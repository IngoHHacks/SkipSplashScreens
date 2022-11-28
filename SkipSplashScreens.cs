using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MMTools;
using System.Collections;
using BepInEx.Configuration;
using Lamb.UI;
using Lamb.UI.MainMenu;
using UnityEngine;

namespace SkipSplashScreens
{
    [BepInPlugin(PluginGuid, PluginName, PluginVer)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "IngoH.cotl.SkipSplashScreens";
        public const string PluginName = "SkipSplashScreens";
        public const string PluginVer = "1.0.2";
        
        public static ConfigEntry<bool> SkipSplashScreens { get; private set; }
        public static ConfigEntry<bool> SkipMenu { get; private set; }
        public static ConfigEntry<int> SaveFileToLoad { get; private set; }

        internal static ManualLogSource Log;

        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");
            Plugin.Log = base.Logger;
            
            var config = base.Config;
            SkipSplashScreens = config.Bind("General", "SkipSplashScreens", true, "Skip the splash screens");
            SkipMenu = config.Bind("General", "SkipMenu", false, "Skip main menu, load straight into the game");
            SaveFileToLoad = config.Bind("General", "SaveFileToLoad", 0, "Save file to load, 0 to 2");

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();

        }

        [HarmonyPatch(typeof(LoadMainMenu), "RunSplashScreens")]
        public class SplashScreenPatch
        {
            public static bool Prefix(LoadMainMenu __instance)
            {
                if (SkipMenu.Value)
                {
                    SaveAndLoad.SAVE_SLOT = SaveFileToLoad.Value;
                    AudioManager.Instance.StopCurrentMusic();
                    MMTransition.Play(MMTransition.TransitionType.ChangeRoomWaitToResume, MMTransition.Effect.BlackFade,
                        "Base Biome 1", 1f, "",
                        () =>
                        {
                            AudioManager.Instance.StopCurrentMusic();
                            SaveAndLoad.Load(SaveAndLoad.SAVE_SLOT);
                        });
                    return false;
                }
                if (SkipSplashScreens.Value)
                {
                    MMTransition.Play(MMTransition.TransitionType.ChangeSceneAutoResume, MMTransition.Effect.BlackFade,
                        "Main Menu", 1f, "", null);
                    return false;
                }
                return true;
            }

            public static IEnumerator Postfix(IEnumerator enumerator, LoadMainMenu __instance)
            {
                yield break;
            }
        }
    }
}