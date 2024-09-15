namespace AutoQuest.Lua.MemberFunction
{
    internal class BoolResult : ICallMemberFunctionResult<bool>, ICallMemberFunctionPoplate
    {
        public bool Value => (Result[Result.Count - 1] is bool b) && b;
        public bool HasResult => Result.Count > 0;
        public Dictionary<int, object?> Result {  get; private set; } = new Dictionary<int, object?>();

    }
}
