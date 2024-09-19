using AutoQuest.Struct;

namespace AutoQuest.Lua
{
    internal static class LuaHelper
    {
        /// <summary>
        /// 调用lua的函数，已经压入了看起来固定的任务lua参数和本地角色参数 剩下的看情况加 例如<see cref="GetTodoArgs"/>
        /// </summary>
        /// <param name="functionName">lua函数</param>
        /// <param name="pushArg">剩下的参数压栈回调函数 例如<see cref="GetTodoArgs"/></param>
        /// <returns></returns>
        /// <exception cref="LuaException"></exception>
        public unsafe static ICallMemberFunctionResult<TRet> CallMemberFunction<TRet>(lua_State* state, byte* baseOfFunction, string functionName, Lua_PushArg pushArg, ICallMemberFunctionResult<TRet> poplate)
        {
            var top = state->Old.lua_gettop();
            try
            {
                state->lua_checkstack(10);
                state->Old.lua_getfield(-10002, baseOfFunction);
                state->Old.lua_getfield(-1, functionName);
                if (state->Old.lua_type(-1) != FFXIVClientStructs.FFXIV.Common.Lua.LuaType.Function)
                {
                    throw new LuaException($"{functionName} not found");
                }
                state->Old.lua_remove(top + 1);
                state->Old.lua_getfield(-10002, baseOfFunction);
                pushArg.Invoke(state);
                var d = state->Old.lua_gettop();
                if (state->Old.lua_pcall(d - top - 1, -1, 0) != 0)
                {
                    state->Old.luaB_tostring();
                    throw new LuaException($"call failed {state->Old.lua_tostring(-1)}");
                }
                var cnt = state->Old.lua_gettop() - top;
                if(poplate is ICallMemberFunctionPoplate C)
                    C.Populate(state, cnt);
                return poplate;
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
                throw;
            }
            finally
            {
                state->Old.lua_settop(top);
            }
        }
        public unsafe static ICallMemberFunctionResult<TRet> CallMemberFunction<TRet>(LuaState* state, byte* baseOfFunction, string functionName, Lua_PushArg pushArg, ICallMemberFunctionResult<TRet> poplate) => CallMemberFunction(state->State, baseOfFunction, functionName, pushArg, poplate);
    }
}
