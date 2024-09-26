using AutoQuest.Struct;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameFunctions;

namespace AutoQuest.Extension
{
    internal static class GameobjectExtension
    {
        public static unsafe bool IsVisible(this IGameObject gameObject)
        {
            var s = gameObject.Struct()->DrawObject;
            return s != null && s->IsVisible;
        }
        public static unsafe string Call(this IGameObject gameObject)
        {
            var obj = gameObject.Struct();
            if(obj != null)
            {
                var actor = obj->LuaActor;
                if(actor != null)
                {
                    var state = (lua_State*)actor->LuaState->State;
                    if(state != null && actor->LuaString.StringPtr != null)
                    {
                        var top = state->Old.lua_gettop();
                        try
                        {
                            state->lua_checkstack(10);
                            state->Old.lua_getfield(-10002, actor->LuaString.StringPtr);
                            state->Old.lua_getfield(-1, "IsEventObject");
                            if (state->Old.lua_type(-1) != FFXIVClientStructs.FFXIV.Common.Lua.LuaType.Function)
                            {
                                throw new LuaException($"not found");
                            }
                            state->Old.lua_remove(top + 1);
                            state->Old.lua_getfield(-10002, actor->LuaString.StringPtr);
                            if (state->Old.lua_pcall(1, -1, 0) != 0)
                            {
                                throw new LuaException($"call f");
                            }
                            var cnt = state->Old.lua_gettop() - top;
                            var str = string.Empty;
                            for(var i = 0; i < cnt;i++)
                            {
                                state->Old.luaB_tostring();
                                str += $"{i}:{state->Old.lua_tostring(-1)}";
                                state->Old.lua_remove(1);
                            }
                            return str;
                        }
                        finally
                        {
                            state->Old.lua_settop(top);
                        }
                    }
                }
            }
            return "none";
        }
    }
}
