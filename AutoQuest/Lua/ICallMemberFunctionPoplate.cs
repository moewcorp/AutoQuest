using AutoQuest.Struct;

namespace AutoQuest.Lua
{
    internal unsafe interface ICallMemberFunctionPoplate
    {
        Dictionary<int, object?> Result { get; }
        void Populate(lua_State* state, int resCount)
        {
            for (int i = 0; i < resCount; i++)
            {
                Result[i] = state->Old.lua_type(-i - 1) switch
                {
                    FFXIVClientStructs.FFXIV.Common.Lua.LuaType.Boolean => state->lua_tobool(-i - 1),
                    FFXIVClientStructs.FFXIV.Common.Lua.LuaType.Number => state->Old.lua_tonumber(-i - 1),
                    _ => null,
                };
            }
        }
    }
}
