using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestResolve.QuestStep
{
    /// <summary>
    /// CheckNext 主要负责是否当前任务是否需要打断执行下一个任务
    /// </summary>
    internal interface IStep
    {
        IStep Start();
        bool CheckNext {  get; }
        void Cancel();
    }
}
