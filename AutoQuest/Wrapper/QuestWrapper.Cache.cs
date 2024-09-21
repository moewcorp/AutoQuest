using AutoQuest.Excel;
using ECommons.DalamudServices;

namespace AutoQuest
{
    internal partial class QuestWrapper
    {
        private static Dictionary<uint, QuestWrapper> _cache = new();
        private static Dictionary<uint, QuestWrapper?> _invalid = new();
        private static HashSet<byte> types = new();
        public static QuestWrapper? GetQuestById(uint id)
        {
            if (_invalid.TryGetValue(id, out _))
                return null;
            if (_cache.TryGetValue(id, out var value))
                return value;
            var s = Svc.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets2.Quest>()?.GetRow(id);
            if (s == null || s.Name == "")
                _invalid.Add(id, null);
            else
            {
                var res = new QuestWrapper(id);
                foreach(var i in res.Quest.QuestListenerParams)
                {
                    if ((EventType)(i.QuestUInt8A) is EventType.None or EventType.Unk11 or EventType.Unk18 or EventType.Unk19 && i.Listener != 0)
                        LogHelper.Info(res.Quest.Name + " "+ res.QuestId.ToString() +" " + i.QuestUInt8A);
                }
                _cache.Add(id, res);
                return res;
            }
            return null;
        }
        public static QuestWrapper? GetQuestById(ushort id) => GetQuestById(id | 0x10000u);
        public static IEnumerable<QuestWrapper> AllQuest => Svc.Data.GetExcelSheet<Excel.GeneratedSheets.Quest>()!.Where(q => q.Name.ToString() != "").Select(s => GetQuestById(s.RowId)!);
    }
}
