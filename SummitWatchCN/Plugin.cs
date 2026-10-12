using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SummitWatchCN.Blacklist;
using SummitWatchCN.I18n;
using UnityEngine;

namespace SummitWatchCN
{
    [BepInPlugin(GuidValue, PluginName, VersionValue)]
    [BepInDependency("summitwatch", BepInDependency.DependencyFlags.HardDependency)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GuidValue = "summitwatch.cn.blacklist";
        public const string PluginName = "SummitWatch 汉化与黑名单";
        public const string VersionValue = "1.0.0";

        internal static ManualLogSource Log;

        private Harmony _harmony;
        private BlacklistManager _manager;

        private void Awake()
        {
            Log = Logger;
            BlacklistConfig.Init(Config);
            BlacklistStore.Load();
            _harmony = new Harmony(GuidValue);
            GuiTranslatePatches.Apply(_harmony);
            var go = new GameObject("SummitWatchCN") { hideFlags = HideFlags.HideAndDontSave };
            Object.DontDestroyOnLoad(go);
            _manager = go.AddComponent<BlacklistManager>();
            Log.LogInfo("SummitWatch 汉化与黑名单附属 v" + VersionValue + " 已加载。");
            Log.LogInfo("黑名单文件: " + BlacklistStore.FilePath);
        }

        private void OnDestroy()
        {
            try { BlacklistStore.Save(); } catch { }
            if (_harmony != null) _harmony.UnpatchSelf();
        }
    }
}
