using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using Lamb.UI.Settings;
using MMTools;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SkipSplashScreens
{
    [BepInPlugin(PluginGuid, PluginName, PluginVer)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "IngoH.cotl.SkipSplashScreens";
        public const string PluginName = "SkipSplashScreens";
        public const string PluginVer = "1.0.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");
            Plugin.Log = base.Logger;

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();

        }

        [HarmonyPatch(typeof(LoadMainMenu), "RunSplashScreens")]
        public class SplashScreenPatch
        {
            public static bool Prefix(LoadMainMenu __instance)
            {
                MMTransition.Play(MMTransition.TransitionType.ChangeSceneAutoResume, MMTransition.Effect.BlackFade, "Main Menu", 1f, "", null);
                return false;
            }

            public static IEnumerator Postfix(IEnumerator enumerator, LoadMainMenu __instance)
            {
                yield break;
            }
        }
    }
}