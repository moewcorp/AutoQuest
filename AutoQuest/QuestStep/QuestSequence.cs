using Lumina.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoQuest.QuestStep
{
    internal class QuestSequence
    {
        public byte Seq;
        public List<SeString> TodoMessages = [];
        public List<QuestStep> MainQuestSteps = [];
        public List<QuestStep> AddQuestSteps = [];
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
            var ranges = g[false].Where(x => x.Listener == 0x5000000);
            if(ranges.Any())
            {
                var allEobjects = listners.Where(x => x.Listener > 2000000 && x.Listener < 3000000);

            }
        }
    }
}
