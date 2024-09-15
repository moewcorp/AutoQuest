namespace AutoQuest.Lua.MemberFunction
{
    internal class TwoNumberResult : ICallMemberFunctionResult<TodoResult> , ICallMemberFunctionPoplate
    {
        public TodoResult Value
        {
            get
            {
                var result = new TodoResult();
                if(Result.TryGetValue(0, out object? value) && value is double a)
                {
                    result.Max = (uint)a;
                }
                if(Result.TryGetValue(1, out value) && value is double b)
                {
                    result.Current = (uint)b;
                }
                return result;
            }
        }

        public Dictionary<int, object?> Result { get; private set; } = new();

        public bool HasResult => Result.Count > 0;
    }
    public struct TodoResult
    {
        public uint Current;
        public uint Max;
        public override string ToString()
        {
            return $"Current:{Current} Max:{Max}";
        }
    }
}
