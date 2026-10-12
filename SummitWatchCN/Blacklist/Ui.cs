using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

namespace SummitWatchCN.Blacklist
{
    internal sealed class UI
    {
        private static string _toast = "";
        private static float _toastUntil;

        public static void Notify(string text)
        {
            _toast = text;
            _toastUntil = Time.time + 5f;
        }

        public static void DrawToast()
        {
            if (string.IsNullOrEmpty(_toast) || Time.time > _toastUntil) return;
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                normal = { textColor = Color.white },
                font = FontHelper.Cjk
            };
            var size = style.CalcSize(new GUIContent(_toast));
            var rect = new Rect(Screen.width / 2 - size.x / 2 - 12, Screen.height - 90, size.x + 24, size.y + 14);
            GUI.color = new Color(0, 0, 0, 0.6f);
            GUI.Box(rect, "");
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 12, rect.y + 7, size.x, size.y), _toast, style);
        }
    }
}
