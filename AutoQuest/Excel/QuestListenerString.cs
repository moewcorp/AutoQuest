using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets2;
using System.Numerics;
using static Lumina.Excel.GeneratedSheets2.Quest;

namespace AutoQuest.Excel
{
    internal class QuestListenerString
    {
        public uint Listener;
        public uint ConditionValue;
        public uint Content;
        public byte Type;
        public QuestListenerString(QuestListenerParamsStruct listener, uint content = 0) : this(listener.Listener, listener.ConditionValue, listener.QuestUInt8A, content) { }
        public QuestListenerString(uint listener,uint conditionValue, byte type,uint content = 0)
        {
            Listener = listener;
            ConditionValue = conditionValue;
            Type = type;
            Content = content;
        }
        public override string ToString()
        {
            if (Listener == 5000000)
            {
                var data = Svc.Data.GetExcelSheet<Level>()?.GetRow(ConditionValue);
                return $"{(EventType)Type} {data} Pos:{data.Territory.Value.PlaceName.Value.Name}{new Vector3(data.X, data.Y, data.Z)} Object:{data.Object.Row}";
            }
            else if(Listener == 5010000)
            {
                if(Content != 0)
                {
                    var str = string.Empty;
                    foreach (var s in Svc.Data.GetExcelSheet<ContentFinderCondition>()?.Where(c => c.Content.Row == Content))
                    {
                        str += $"{s} {s.Name}";
                    }
                    return $"{(EventType)Type} {str}";
                }
                return Content.ToString();
            }//5020000 可能是去某个地图
            else if (Listener > 3000000)
            {
                return "null";
            }
            else if (Listener > 2000000)
            {
                var eobj = Svc.Data.GetExcelSheet<EObj>()?.GetRow(Listener);
                var eobjname = Svc.Data.GetExcelSheet<EObjName>()?.GetRow(Listener);
                return $"{(EventType)Type} {eobj} {eobjname.Singular}";
            }
            else if (Listener > 1000000)
            {
                var enpc = Svc.Data.GetExcelSheet<ENpcBase>()?.GetRow(Listener);
                var enpcname = Svc.Data.GetExcelSheet<ENpcResident>()?.GetRow(Listener);
                return $"{(EventType)Type} {enpc} {enpcname.Singular}";
            }
            else
            {
                var bnpc = Svc.Data.GetExcelSheet<BNpcBase>()?.GetRow(Listener);
                if (ConditionValue == 0)
                {
                    return $"{(EventType)Type} {bnpc} BaseId:{Listener}";
                }
                else
                {
                    var lel = Svc.Data.GetExcelSheet<Level>()?.GetRow(ConditionValue);
                    return $"{(EventType)Type} EventBNpc#{Listener} Pos:{lel.Territory.Value.PlaceName.Value.Name}{new Vector3(lel.X, lel.Y, lel.Z)} Object:{lel.Object.Row}";
                }
            }
            return "null";
        }
        public static explicit operator string(QuestListenerString s) 
        {
            return s.ToString();
        }
    }
    public enum EventType : byte
    {
        Normal = 1,
        RangeEnemy = 5,
        UseEventItem = 8,
        Enemy = 9,
        Range = 10,
        EnterTerr = 15,
    }
}
