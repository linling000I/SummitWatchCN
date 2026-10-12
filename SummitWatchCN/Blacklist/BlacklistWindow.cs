using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

namespace SummitWatchCN.Blacklist
{
    internal sealed class BlacklistWindow
    {
        private Rect _window = new Rect(120, 120, 480, 560);
        private Vector2 _listScroll;
        private Vector2 _historyScroll;
        private string _addName = "";
        private string _addId = "";
        private string _addNote = "";
        private bool _visible;

        public bool Visible { get => _visible; set => _visible = value; }

        private GUIStyle Title => new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, font = FontHelper.Cjk };
        private GUIStyle Heading => new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, font = FontHelper.Cjk };
        private GUIStyle Body => new GUIStyle(GUI.skin.label) { fontSize = 12, font = FontHelper.Cjk };
        private GUIStyle Btn => new GUIStyle(GUI.skin.button) { fontSize = 12, font = FontHelper.Cjk };

        public void Draw()
        {
            _window = GUI.Window(-7652, _window, DrawWindow, "黑名单管理（F8 关闭）");
        }

        private void DrawWindow(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("峰会监视 · 黑名单", Title);

            GUILayout.Label("自动处理", Heading);
            BlacklistConfig.Enabled.Value = GUILayout.Toggle(BlacklistConfig.Enabled.Value, "启用黑名单自动处理", Body);
            BlacklistConfig.HostAutoKick.Value = GUILayout.Toggle(BlacklistConfig.HostAutoKick.Value, "主机自动踢出黑名单玩家", Body);

            GUILayout.BeginHorizontal();
            GUILayout.Label("你不是主机时：", Body);
            string label = BlacklistConfig.Label(BlacklistConfig.ClientAction.Value);
            if (GUILayout.Button(label, Btn))
            {
                ClientBlockAction next = BlacklistConfig.ClientAction.Value + 1;
                if (next > ClientBlockAction.LogOnly) next = ClientBlockAction.LocalBlock;
                BlacklistConfig.ClientAction.Value = next;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            GUILayout.Label("当前大厅玩家", Heading);
            _listScroll = GUILayout.BeginScrollView(_listScroll, GUILayout.MinHeight(160));
            DrawLobbyRows();
            GUILayout.EndScrollView();

            GUILayout.Space(6);
            GUILayout.Label("加入黑名单（手动）", Heading);
            GUILayout.BeginHorizontal();
            GUILayout.Label("名称：", Body); _addName = GUILayout.TextField(_addName, 18); GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("ID：", Body); _addId = GUILayout.TextField(_addId, 26); GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("备注：", Body); _addNote = GUILayout.TextField(_addNote, 20); GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("加入黑名单", Btn))
            {
                if (BlacklistStore.Add(_addId.Trim(), _addName.Trim(), _addNote.Trim()))
                {
                    _addName = _addId = _addNote = "";
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            GUILayout.Label("历史名单（" + BlacklistStore.Entries.Count + " 条）", Heading);
            _historyScroll = GUILayout.BeginScrollView(_historyScroll, GUILayout.MinHeight(120));
            for (int i = 0; i < BlacklistStore.Entries.Count; i++)
            {
                BlacklistEntry e = BlacklistStore.Entries[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label(e.DisplayName + " · " + e.DisplayId, Body);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("复制ID", Btn) && !string.IsNullOrEmpty(e.id)) GUIUtility.systemCopyBuffer = e.id;
                if (GUILayout.Button("移除", Btn)) BlacklistStore.RemoveAt(i);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打开数据文件", Btn))
            {
                try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(BlacklistStore.FilePath) { UseShellExecute = true }); }
                catch { }
            }
            if (GUILayout.Button("重载名单", Btn)) BlacklistStore.Load();
            if (GUILayout.Button("清空全部", Btn)) BlacklistStore.Clear();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void DrawLobbyRows()
        {
            if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
            {
                GUILayout.Label("当前不在大厅中。", Body);
                return;
            }
            var players = new List<PhotonPlayer>(PhotonNetwork.CurrentRoom.Players.Values);
            foreach (PhotonPlayer p in players)
            {
                string name = p.NickName ?? "(无名)";
                string uid = p.UserId ?? "";
                string hostTag = p.IsMasterClient ? " [主机]" : "";
                GUILayout.BeginHorizontal();
                GUILayout.Label(name + hostTag + "  " + uid, Body);
                GUILayout.FlexibleSpace();
                bool bl = BlacklistStore.IsBlacklisted(name, uid);
                if (bl)
                {
                    if (GUILayout.Button("移出黑名单", Btn))
                    {
                        BlacklistEntry e = BlacklistStore.FindById(uid);
                        if (e != null) BlacklistStore.Entries.Remove(e);
                    }
                }
                else
                {
                    if (GUILayout.Button("加入黑名单", Btn)) BlacklistStore.Add(uid, name, "");
                }
                if (PhotonNetwork.IsMasterClient && !p.IsMasterClient && !p.IsLocal
                    && GUILayout.Button("踢出", Btn)) GameKick.HostKick(p);
                GUILayout.EndHorizontal();
            }
        }
    }
}
