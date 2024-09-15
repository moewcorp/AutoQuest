using AutoQuest.Struct;

namespace AutoQuest
{
    internal unsafe class LuaException : Exception
    {
        private string MyMessege = string.Empty;
        public LuaException(lua_State* state): base()
        {
            state->Old.luaB_tostring();
            MyMessege = state->Old.lua_tostring(-1) ?? "null";
        }
        public LuaException(string myMessege) : base()
        {
            MyMessege = myMessege;
        }

        public override string Message => base.Message + "\n"+MyMessege;
    }
}
