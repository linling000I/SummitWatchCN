using BepInEx.Configuration;
using UnityEngine;

namespace SummitWatchCN.Blacklist
{
    public enum ClientBlockAction
    {
        LocalBlock = 0,
        LeaveRoom = 1,
        AskHost = 2,
        LogOnly = 3
    }

    internal static class BlacklistConfig
    {
        public static ConfigEntry<bool> LocalizationEnabled;
        public static ConfigEntry<KeyCode> PanelKey;
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<bool> ToastsEnabled;
        public static ConfigEntry<bool> HostAutoKick;
        public static ConfigEntry<bool> MatchByName;
        public static ConfigEntry<ClientBlockAction> ClientAction;
        public static ConfigEntry<float> KickRetrySeconds;
        public static ConfigEntry<int> KickMaxRetries;
        public static ConfigEntry<bool> FallbackLocalBlock;
        public static ConfigEntry<bool> LocalMute;
        public static ConfigEntry<bool> LocalHide;

        public static void Init(ConfigFile cfg)
        {
            LocalizationEnabled = cfg.Bind("General", "LocalizationEnabled", true,
                "汉化 Summit Watch 的面板、玩家资料、按钮、提示和弹窗（不影响举报文本和证据日志）。");
            PanelKey = cfg.Bind("General", "BlacklistPanelKey", KeyCode.F8,
                "打开/关闭黑名单管理窗口的按键（默认 F8；F3 会被 NVIDIA 游戏滤镜占用）。");
            Enabled = cfg.Bind("Blacklist", "Enabled", true, "启用黑名单自动处理。");
            ToastsEnabled = cfg.Bind("Blacklist", "ToastsEnabled", true, "处理黑名单玩家时弹出中文提示。");
            HostAutoKick = cfg.Bind("Blacklist", "HostAutoKick", true, "主机自动踢出黑名单玩家。");
            MatchByName = cfg.Bind("Blacklist", "MatchByName", true, "无ID条目按玩家名称匹配。");
            ClientAction = cfg.Bind("Blacklist", "ClientAction", ClientBlockAction.LocalBlock,
                "非主机处理：LocalBlock/LeaveRoom/AskHost/LogOnly。");
            KickRetrySeconds = cfg.Bind("Blacklist", "KickRetrySeconds", 3f, "踢出重试间隔（秒）。");
            KickMaxRetries = cfg.Bind("Blacklist", "KickMaxRetries", 5, "最大重试次数。");
            FallbackLocalBlock = cfg.Bind("Blacklist", "FallbackLocalBlock", true, "重试耗尽后本地屏蔽。");
            LocalMute = cfg.Bind("Blacklist", "LocalMute", true, "本地屏蔽时静音。");
            LocalHide = cfg.Bind("Blacklist", "LocalHide", true, "本地屏蔽时隐藏角色。");
        }

        public static string Label(ClientBlockAction a)
        {
            switch (a)
            {
                case ClientBlockAction.LocalBlock: return "本地屏蔽";
                case ClientBlockAction.LeaveRoom: return "离开大厅";
                case ClientBlockAction.AskHost: return "请求主机踢出";
                default: return "仅提示记录";
            }
        }
    }
}
