using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoQuest.Enum
{
    internal enum ListenerType
    {
        Normal,
        Actor,
        Eobjcet,
        Range,
        Enemy,
        FinishDungeon,   
    }
    internal enum ListenerType2
    {
        Normal = 1,
        EventEnemy = 5,
        UseEventItem = 8,
        NormalEnemy = 9,//可能是打怪获得物品
        EnterRange = 10,
        FinishDungeon = 15
    }
}
