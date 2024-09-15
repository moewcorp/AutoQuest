using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lumina.Excel.GeneratedSheets2.Quest;

namespace AutoQuest.Excel
{
    internal static class SheetHelper
    {
        public static object? ListenerToSheet(this QuestListenerParamsStruct listener)
        {
            if (listener.Listener == 5000000)
            {
                return Svc.Data.GetExcelSheet<Level>()?.GetRow(listener.ConditionValue);
            }
            else if(listener.Listener > 3000000)
            { 
                return null;
            }
            else if(listener.Listener > 2000000)
            {
                return Svc.Data.GetExcelSheet<EObj>()?.GetRow(listener.Listener);
            }
            else if(listener.Listener > 1000000)
            {
                return Svc.Data.GetExcelSheet<ENpcBase>()?.GetRow(listener.Listener);
            }
            else
            {
                return Svc.Data.GetExcelSheet<Level>()?.GetRow(listener.ConditionValue);
            }
        }
    }
}
