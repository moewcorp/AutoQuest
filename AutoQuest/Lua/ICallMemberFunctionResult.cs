namespace AutoQuest.Lua
{
    internal unsafe interface ICallMemberFunctionResult<TValue>
    {
        TValue Value { get; }
        bool HasResult { get; }
    }
}
