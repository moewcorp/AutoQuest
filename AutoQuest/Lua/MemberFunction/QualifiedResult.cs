using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoQuest.Lua.MemberFunction
{
    internal class QualifiedResult : ICallMemberFunctionResult<bool>, ICallMemberFunctionPoplate
    {
        public bool Value => Result.TryGetValue(1, out var b) && b is bool c && c;
        public bool HasResult => Result.Count == 2;
        public Dictionary<int, object?> Result { get; private set; } = new Dictionary<int, object?>();
    }
}
