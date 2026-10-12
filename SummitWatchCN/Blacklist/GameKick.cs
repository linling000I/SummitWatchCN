using System;
using Peak;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using PhotonPlayer = Photon.Realtime.Player;

namespace SummitWatchCN.Blacklist
{
    internal static class GameKick
    {
        public static void HostKick(PhotonPlayer target)
        {
            if (target == null || target.IsLocal || target.IsMasterClient) return;
            try { global::PlayerHandler.Kick(target.ActorNumber); }
            catch (Exception e) { Plugin.Log.LogWarning("PlayerHandler.Kick 失败: " + e.Message); }
            try { PhotonNetwork.CloseConnection(target); }
            catch (Exception e) { Plugin.Log.LogWarning("CloseConnection 失败: " + e.Message); }
        }

        public static bool LocalMute(PhotonPlayer target)
        {
            try
            {
                Character c = global::PlayerHandler.GetPlayerCharacter(target);
                if (c != null && c.data != null && !c.data.isMuted) { c.data.isMuted = true; return true; }
            }
            catch (Exception e) { Plugin.Log.LogWarning("本地静音失败: " + e.Message); }
            return false;
        }

        public static bool LocalHide(PhotonPlayer target)
        {
            try
            {
                global::Player gp = global::PlayerHandler.GetPlayer(target);
                if (gp != null && gp.gameObject != null && gp.gameObject.activeSelf) { gp.gameObject.SetActive(false); return true; }
            }
            catch (Exception e) { Plugin.Log.LogWarning("本地隐藏失败: " + e.Message); }
            return false;
        }

        public static bool StillInRoom(int actor)
        {
            try
            {
                return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null
                    && PhotonNetwork.CurrentRoom.GetPlayer(actor, false) != null;
            }
            catch { return false; }
        }
    }
}
