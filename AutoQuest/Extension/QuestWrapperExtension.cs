using AutoQuest.Enum;
using static Lumina.Excel.GeneratedSheets2.Quest;

namespace AutoQuest.Extension
{
    internal static class QuestWrapperExtension
    {
        public static Dictionary<uint, List<ListenerType>> a(this QuestWrapper quest)
        {
            var ret = new Dictionary<uint, List<ListenerType>>();
            for (int i = 0; i < 24; i++)
            {
                var listenersBySeq = quest.Quest.QuestListenerParams.Where(l => l.ActorSpawnSeq == i);
                if (listenersBySeq.Any())
                {
                    var mainlisteners = listenersBySeq.Where(l => l.ActorDespawnSeq != 0xFF);
                    var levels = quest.Quest.TodoParams.Where(l => l.ToDoCompleteSeq == i);
                    if (mainlisteners.Any())
                    {
                        var list = new List<ListenerType>();
                        foreach (var l in mainlisteners)
                        {
                            list.Add(l.Listener.GetListenerType());
                        }
                        ret.Add((uint)i,list);
                    }
                }
            }
            return ret;
        }
        public static ListenerType GetListenerType(this uint listener) => listener switch
        {
            < 100000 => ListenerType.Enemy,
            < 2000000 => ListenerType.Actor,
            < 3000000 => ListenerType.Eobjcet,
            5000000 => ListenerType.Range,
            5010000 => ListenerType.FinishDungeon,
            _ => ListenerType.Normal,
        };

        public static bool IsRange(this ListenerType value) => value == ListenerType.Range;
        public static bool IsRange(this uint listener) => GetListenerType(listener).IsRange();
        public static bool IsActor(this uint listener) => listener > 1000000 && listener < 2000000;
        public static bool IsQuestEvent(this uint EventId)
        {
            return EventId > 0x10000 && EventId < 0x20000;
        }
        public static QuestWrapper GetQuestWrapper(this uint EventId) => QuestWrapper.GetQuestById(EventId);
        public static QuestWrapper GetQuestWrapper(this ushort EventId) => QuestWrapper.GetQuestById(EventId);
        public static ListenerType GetListenerType(this QuestListenerParamsStruct listener) => listener.QuestUInt8A switch
        {
            1 => ListenerType.Normal,
            5 => ListenerType.Enemy,
            10 => ListenerType.Actor,
        };
    }
}
