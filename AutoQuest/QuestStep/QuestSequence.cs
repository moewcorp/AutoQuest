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
        public List<QuestStep> QuestSteps = [];
        public QuestSequence(QuestWrapper quest, byte seq)
        {
            Seq = seq;
            foreach (var (Listener, Level) in quest.MainInfo.First(x => x.Seq == seq).Info)
            {

            }

        }
    }
}
