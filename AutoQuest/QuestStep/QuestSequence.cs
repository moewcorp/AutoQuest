using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets2;
using Lumina.Text;

namespace AutoQuest.QuestStep
{
    internal class QuestSequence
    {
        public byte Seq;
        public List<SeString> TodoMessages = [];
        public List<QuestStep> MainQuestSteps = [];
        public List<QuestStep> AdditionalQuestSteps = [];
        public QuestSequence(QuestWrapper quest, byte seq)
        {
            Seq = seq;
            var listners = quest.Quest.QuestListenerParams.Where(l => l.ActorSpawnSeq == seq);
            var g = listners.ToLookup(l => l.ActorDespawnSeq != 0xff);
            var mainListener = g[true];
            TodoMessages = mainListener.Select(x => x.ActorDespawnSeq)
                .ToHashSet()
                .Select(i => quest.Quest.QuestTodoMessages[i].Value.Value)
                .ToList();
            MainQuestSteps = quest.MainInfo.First(x => x.Seq == seq).Info.Select(x => new QuestStep(quest, x.Listener, x.Level.Value)).ToList();
            var ranges = g[true].Where(x => x.Listener == 0x5000000);
            AdditionalQuestSteps = ranges.Select(x => new QuestStep(quest, x, Svc.Data.GetExcelSheet<Level>().GetRow(x.ConditionValue))).ToList();
        }
    }
}
