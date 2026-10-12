using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SummitWatch.Core;
using UnityEngine;

namespace SummitWatchCN.Blacklist
{
    public sealed class LobbyPlayer
    {
        public int Actor;
        public string Name;
        public string UserId;
        public bool IsHost;
        public bool IsLocal;
        public object Record;
        public bool HasRecord;
    }

    /// <summary>
    /// 通过反射访问 SummitWatch 的 internal 成员（Watch / PlayerRecord / ProfileWindow / EvidenceLog）。
    /// 所有成员按需缓存，找不到时安全降级，不抛异常到调用方。
    /// </summary>
    internal static class SwAccess
    {
        private static Type _watchType;
        private static PropertyInfo _instanceProp;
        private static FieldInfo _recordsField;
        private static FieldInfo _profileField;
        private static PropertyInfo _hostHasModProp;
        private static PropertyInfo _myRoleProp;
        private static PropertyInfo _panelOpenProp;
        private static PropertyInfo _modeProp;
        private static MethodInfo _kickMethod;
        private static MethodInfo _requestKickMethod;
        private static MethodInfo _profileOpen;
        private static MethodInfo _evidenceLine;
        private static Type _stylesType;
        private static FieldInfo[] _stylesFields;
        private static Type _recordType;
        private static FieldInfo _recordActor;
        private static FieldInfo _recordName;
        private static FieldInfo _recordUserId;
        private static bool _initialized;

        public static bool Init()
        {
            if (_initialized) return _watchType != null;
            _initialized = true;
            try
            {
                _watchType = AccessTools.TypeByName("SummitWatch.Watch");
                if (_watchType == null) return false;
                _instanceProp = AccessTools.Property(_watchType, "Instance");
                _recordsField = AccessTools.Field(_watchType, "Records");
                _profileField = AccessTools.Field(_watchType, "Profile");
                _hostHasModProp = AccessTools.Property(_watchType, "HostHasMod");
                _myRoleProp = AccessTools.Property(_watchType, "MyRole");
                _panelOpenProp = AccessTools.Property(_watchType, "PanelOpen");
                _modeProp = AccessTools.Property(_watchType, "Mode");
                _kickMethod = AccessTools.Method(_watchType, "Kick");
                _requestKickMethod = AccessTools.Method(_watchType, "RequestKick");
                _recordType = AccessTools.TypeByName("SummitWatch.PlayerRecord");
                if (_recordType != null)
                {
                    _recordActor = AccessTools.Field(_recordType, "Actor");
                    _recordName = AccessTools.Field(_recordType, "Name");
                    _recordUserId = AccessTools.Field(_recordType, "UserId");
                }
                var profileType = AccessTools.TypeByName("SummitWatch.ProfileWindow");
                if (profileType != null)
                {
                    _profileOpen = AccessTools.Method(profileType, "Open");
                }
                var evidenceType = AccessTools.TypeByName("SummitWatch.EvidenceLog");
                if (evidenceType != null) _evidenceLine = AccessTools.Method(evidenceType, "Line");
                return true;
            }
            catch (Exception e)
            {
                Plugin.Log.LogWarning("初始化 SummitWatch 反射访问失败: " + e);
                return false;
            }
        }

        public static object Instance
        {
            get
            {
                if (!Init() || _instanceProp == null) return null;
                try { return _instanceProp.GetValue(null); } catch { return null; }
            }
        }

        public static bool PanelOpen
        {
            get
            {
                if (!Init() || _panelOpenProp == null) return false;
                try { return (bool)_panelOpenProp.GetValue(null); } catch { return false; }
            }
        }

        public static bool HostHasMod
        {
            get
            {
                object inst = Instance;
                if (inst == null || _hostHasModProp == null) return false;
                try { return (bool)_hostHasModProp.GetValue(inst); } catch { return false; }
            }
        }

        public static Role MyRole
        {
            get
            {
                object inst = Instance;
                if (inst == null || _myRoleProp == null) return Role.None;
                try { return (Role)_myRoleProp.GetValue(inst); } catch { return Role.None; }
            }
        }

        public static void ApplySummitWatchFonts(Font font)
        {
            if (font == null || !Init()) return;
            try
            {
                if (_stylesType == null)
                {
                    _stylesType = AccessTools.TypeByName("SummitWatch.Styles");
                    if (_stylesType != null)
                    {
                        var list = new List<FieldInfo>();
                        foreach (FieldInfo fi in _stylesType.GetFields(
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                        {
                            if (fi.FieldType == typeof(GUIStyle)) list.Add(fi);
                        }
                        _stylesFields = list.ToArray();
                    }
                }
                if (_stylesFields != null)
                {
                    foreach (FieldInfo fi in _stylesFields)
                    {
                        var style = fi.GetValue(null) as GUIStyle;
                        if (style != null && style.font != font) style.font = font;
                    }
                }
            }
            catch { }
        }

        public static void EvidenceLine(string text)
        {
            if (!Init() || _evidenceLine == null) return;
            try { _evidenceLine.Invoke(null, new object[] { text }); } catch { }
        }

        private static Dictionary<int, object> RecordMap()
        {
            object inst = Instance;
            var map = new Dictionary<int, object>();
            if (inst == null || _recordsField == null) return map;
            try
            {
                if (_recordsField.GetValue(inst) is IDictionary dict)
                {
                    foreach (DictionaryEntry entry in dict)
                    {
                        if (entry.Key is int actor && entry.Value != null) map[actor] = entry.Value;
                    }
                }
            }
            catch (Exception e) { Plugin.Log.LogWarning("读取 SummitWatch.Records 失败: " + e); }
            return map;
        }

        public static List<LobbyPlayer> SnapshotRecords()
        {
            var result = new List<LobbyPlayer>();
            if (!Init()) return result;
            foreach (var kv in RecordMap())
            {
                var p = new LobbyPlayer { Actor = kv.Key, Name = "", UserId = "" };
                result.Add(p);
            }
            return result;
        }

        public static object RecordOf(int actor)
        {
            if (!Init()) return null;
            var map = RecordMap();
            return map.TryGetValue(actor, out var r) ? r : null;
        }

        public static void InvokeKick(int actor)
        {
            object inst = Instance;
            object record = RecordOf(actor);
            if (inst == null || record == null || _kickMethod == null) return;
            try { _kickMethod.Invoke(inst, new[] { record }); }
            catch (Exception e) { Plugin.Log.LogWarning("调用 SummitWatch.Kick 失败: " + e); }
        }

        public static void InvokeRequestKick(int actor)
        {
            object inst = Instance;
            object record = RecordOf(actor);
            if (inst == null || record == null || _requestKickMethod == null) return;
            try { _requestKickMethod.Invoke(inst, new[] { record }); }
            catch (Exception e) { Plugin.Log.LogWarning("调用 SummitWatch.RequestKick 失败: " + e); }
        }
    }
}
