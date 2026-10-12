using UnityEngine;

namespace SummitWatchCN.Blacklist
{
    internal static class FontHelper
    {
        private static Font _cjk;

        public static Font Cjk
        {
            get
            {
                if (_cjk == null)
                {
                    try
                    {
                        _cjk = Font.CreateDynamicFontFromOSFont(
                            new[] { "Microsoft YaHei", "微软雅黑", "SimHei", "Arial Unicode MS", "Arial" }, 14);
                    }
                    catch { _cjk = GUI.skin != null ? GUI.skin.font : null; }
                }
                return _cjk;
            }
        }
    }
}
