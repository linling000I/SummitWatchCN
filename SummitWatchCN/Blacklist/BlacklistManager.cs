using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

namespace SummitWatchCN.Blacklist
{
    internal sealed class BlacklistManager : MonoBehaviour, IInRoomCallbacks
    {
        private readonly Dictionary<int, string> _localShield = new Dictionary<int, string>();
        private readonly HashSet<int> _notified = new HashSet<int>();
        private BlacklistWindow _window;

        private void Start()
        {
            _window = new BlacklistWindow();
            PhotonNetwork.AddCallbackTarget(this);
            ApplyToCurrentLobby();
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(BlacklistConfig.PanelKey.Value)) _window.Visible = !_window.Visible;
            ProcessRetries();
        }

        private void OnGUI() { if (_window != null && _window.Visible) _window.Draw(); }

        public void OnPlayerEnteredRoom(PhotonPlayer newPlayer) { ApplyTo(newPlayer); }
        public void OnPlayerLeftRoom(PhotonPlayer otherPlayer) { _localShield.Remove(otherPlayer.ActorNumber); }
        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable p) { }
        public void OnPlayerPropertiesUpdate(PhotonPlayer p, ExitGames.Client.Photon.Hashtable ch) { }
        public void OnMasterClientSwitched(PhotonPlayer newMaster) { }

        private void ApplyToCurrentLobby()
        {
            if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null) return;
            foreach (PhotonPlayer p in PhotonNetwork.CurrentRoom.Players.Values) ApplyTo(p);
        }

        private void ApplyTo(PhotonPlayer player)
        {
            if (player == null || player.IsLocal) return;
            if (!BlacklistConfig.Enabled.Value) return;
            string name = player.NickName ?? "";
            string userId = player.UserId ?? "";
            if (!BlacklistStore.IsBlacklisted(name, userId)) return;
            BlacklistEntry e = BlacklistStore.FindById(userId);
            string display = e != null ? e.DisplayName : name;
            if (player.IsMasterClient)
            {
                if (!_notified.Contains(player.ActorNumber))
                {
                    _notified.Add(player.ActorNumber);
                    Notify("黑名单玩家 " + display + " 是主机，无法踢出。");
                }
                return;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                if (BlacklistConfig.HostAutoKick.Value)
                {
                    Notify("主机：已踢出黑名单玩家 " + display);
                    GameKick.HostKick(player);
                    StartCoroutine(RetainKick(player, display));
                }
                return;
            }

            string key = player.ActorNumber + "|" + userId;
            if (_localShield.ContainsKey(player.ActorNumber)) return;
            switch (BlacklistConfig.ClientAction.Value)
            {
                case ClientBlockAction.LocalBlock:
                {
                    bool acted = false;
                    if (BlacklistConfig.LocalMute.Value) acted |= GameKick.LocalMute(player);
                    if (BlacklistConfig.LocalHide.Value) acted |= GameKick.LocalHide(player);
                    _localShield[player.ActorNumber] = key;
                    Notify("已本地屏蔽黑名单玩家 " + display + (acted ? "（静音+隐藏角色）" : "（名单已记录）"));
                    break;
                }
                case ClientBlockAction.LeaveRoom:
                    Notify("黑名单玩家 " + display + " 在本大厅中：你已离开大厅。");
                    if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom(false);
                    break;
                case ClientBlockAction.AskHost:
                    SwAccess.InvokeRequestKick(player.ActorNumber);
                    Notify("已向主机请求踢出黑名单玩家 " + display);
                    break;
                default:
                    Notify("发现黑名单玩家 " + display + "（仅记录，未处理）");
                    break;
            }
        }

        private readonly Dictionary<int, RetryState> _retries = new Dictionary<int, RetryState>();
        private sealed class RetryState { public int Actor; public int Count; public float NextAt; }

        private void StartRetainKick(PhotonPlayer player, string display)
        {
            var st = new RetryState { Actor = player.ActorNumber, Count = 0, NextAt = Time.time + BlacklistConfig.KickRetrySeconds.Value };
            _retries[player.ActorNumber] = st;
        }

        private System.Collections.IEnumerator RetainKick(PhotonPlayer player, string display)
        {
            yield return new WaitForSeconds(BlacklistConfig.KickRetrySeconds.Value);
            int n = 0;
            while (n < BlacklistConfig.KickMaxRetries.Value && GameKick.StillInRoom(player.ActorNumber))
            {
                GameKick.HostKick(player);
                n++;
                yield return new WaitForSeconds(BlacklistConfig.KickRetrySeconds.Value);
            }
            if (GameKick.StillInRoom(player.ActorNumber) && BlacklistConfig.FallbackLocalBlock.Value)
            {
                if (BlacklistConfig.LocalMute.Value) GameKick.LocalMute(player);
                if (BlacklistConfig.LocalHide.Value) GameKick.LocalHide(player);
                Notify("黑名单玩家 " + display + " 疑似用作弊菜单无视踢出，已改为本地屏蔽。");
            }
        }

        private void ProcessRetries() { }

        private void Notify(string text)
        {
            if (BlacklistConfig.ToastsEnabled.Value) UI.Notify(text);
            Plugin.Log.LogInfo("[黑名单] " + text);
        }
    }
}
