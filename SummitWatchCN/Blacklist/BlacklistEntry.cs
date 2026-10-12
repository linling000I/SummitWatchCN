using System;

namespace SummitWatchCN.Blacklist
{
    [Serializable]
    public sealed class BlacklistEntry
    {
        public string id = "";
        public string name = "";
        public string note = "";
        public string added = "";

        public BlacklistEntry() { }

        public BlacklistEntry(string id, string name, string note)
        {
            this.id = id ?? "";
            this.name = name ?? "";
            this.note = note ?? "";
            added = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public string DisplayName => string.IsNullOrEmpty(name) ? "(未记录名称)" : name;
        public string DisplayId => string.IsNullOrEmpty(id) ? "(仅按名称匹配)" : id;
    }
}
