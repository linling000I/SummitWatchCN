using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SummitWatchCN.Blacklist
{
    /// <summary>
    /// 黑名单持久化。手写 JSON（写盘/读盘），不依赖 JsonUtility 对 List 的支持——
    /// 该版本 Unity 的 JsonUtility 对 List&lt;自定义类&gt; 读写都会静默丢内容。
    /// 写入采用「临时文件 + File.Replace 原子替换」，避免中途崩溃留下空名单。
    /// </summary>
    internal static class BlacklistStore
    {
        public static readonly List<BlacklistEntry> Entries = new List<BlacklistEntry>();

        public static string FilePath
        {
            get
            {
                string root = Path.Combine(Paths.BepInExRootPath, "config");
                return Path.Combine(root, "summitwatch-blacklist.json");
            }
        }

        public static void Load()
        {
            Entries.Clear();
            try
            {
                if (!File.Exists(FilePath))
                {
                    Save();
                    return;
                }
                ParseFile(File.ReadAllText(FilePath, new UTF8Encoding(false)));
                Plugin.Log.LogInfo("黑名单加载完成：" + Entries.Count + " 条记录。");
            }
            catch (Exception e)
            {
                Plugin.Log.LogWarning("黑名单加载失败：" + e.Message);
            }
        }

        public static void Save()
        {
            try
            {
                string dir = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string tmp = FilePath + ".tmp";
                File.WriteAllText(tmp, BuildJson(), new UTF8Encoding(false));
                if (File.Exists(FilePath)) File.Replace(tmp, FilePath, null);
                else File.Move(tmp, FilePath);
            }
            catch (Exception e)
            {
                Plugin.Log.LogWarning("黑名单保存失败：" + e.Message);
            }
        }

        private static string BuildJson()
        {
            var sb = new StringBuilder();
            sb.Append("{\"version\":1,\"entries\":[");
            for (int i = 0; i < Entries.Count; i++)
            {
                BlacklistEntry e = Entries[i];
                if (i > 0) sb.Append(",");
                sb.Append("{\"id\":\"" + Escape(e.id) + "\"");
                sb.Append(",\"name\":\"" + Escape(e.name) + "\"");
                sb.Append(",\"note\":\"" + Escape(e.note) + "\"");
                sb.Append(",\"added\":\"" + Escape(e.added) + "\"}");
            }
            sb.Append("]}");
            return sb.ToString();
        }

        private static string Escape(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var sb = new StringBuilder(s.Length + 4);
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        private static void ParseFile(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            int i = text.IndexOf("entries", StringComparison.Ordinal);
            if (i < 0) return;
            int arrayStart = text.IndexOf('[', i);
            if (arrayStart < 0) return;
            int depth = 0;
            int arrayEnd = -1;
            for (int p = arrayStart; p < text.Length; p++)
            {
                char c = text[p];
                if (c == '[') depth++;
                else if (c == ']') { depth--; if (depth == 0) { arrayEnd = p; break; } }
            }
            if (arrayEnd < 0) return;
            string body = text.Substring(arrayStart + 1, arrayEnd - arrayStart - 1);
            int pos = 0;
            while (true)
            {
                int objStart = body.IndexOf('{', pos);
                if (objStart < 0) break;
                int objEnd = body.IndexOf('}', objStart);
                if (objEnd < 0) break;
                string obj = body.Substring(objStart + 1, objEnd - objStart - 1);
                Entries.Add(new BlacklistEntry(
                    ReadJsonString(obj, "id"),
                    ReadJsonString(obj, "name"),
                    ReadJsonString(obj, "note")));
                pos = objEnd + 1;
            }
        }

        private static string ReadJsonString(string obj, string key)
        {
            int k = obj.IndexOf('"' + key + "":", StringComparison.Ordinal);
            if (k < 0) return "";
            k += key.Length + 3;
            if (k >= obj.Length || obj[k] != '"') return "";
            k++;
            var sb = new StringBuilder();
            while (k < obj.Length)
            {
                char c = obj[k];
                if (c == '\\' && k + 1 < obj.Length)
                {
                    char n = obj[k + 1];
                    switch (n)
                    {
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        default: sb.Append(n); break;
                    }
                    k += 2;
                    continue;
                }
                if (c == '"') break;
                sb.Append(c);
                k++;
            }
            return sb.ToString();
        }

        public static bool IsBlacklisted(string name, string userId)
        {
            foreach (BlacklistEntry e in Entries)
            {
                if (!string.IsNullOrEmpty(e.id) && !string.IsNullOrEmpty(userId) &&
                    string.Equals(e.id.Trim(), userId.Trim(), StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.IsNullOrEmpty(e.id) && BlacklistConfig.MatchByName.Value &&
                    !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(e.name) &&
                    string.Equals(e.name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static BlacklistEntry FindById(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;
            foreach (BlacklistEntry e in Entries)
                if (!string.IsNullOrEmpty(e.id) &&
                    string.Equals(e.id.Trim(), userId.Trim(), StringComparison.OrdinalIgnoreCase))
                    return e;
            return null;
        }

        public static bool Add(string id, string name, string note)
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(name)) return false;
            BlacklistEntry ex = !string.IsNullOrEmpty(id) ? FindById(id) : null;
            if (ex != null)
            {
                ex.name = name ?? ex.name;
                ex.note = note ?? ex.note;
            }
            else
            {
                Entries.Add(new BlacklistEntry(id ?? "", name ?? "", note ?? ""));
            }
            Save();
            return true;
        }

        public static bool RemoveAt(int index)
        {
            if (index < 0 || index >= Entries.Count) return false;
            Entries.RemoveAt(index);
            Save();
            return true;
        }

        public static void Clear()
        {
            Entries.Clear();
            Save();
        }
    }
}
