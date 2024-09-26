using AutoQuest.Lua;
using AutoQuest.Lua.MemberFunction;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using static Lumina.Excel.GeneratedSheets2.Quest;
using EventHandler = FFXIVClientStructs.FFXIV.Client.Game.Event.EventHandler;

namespace AutoQuest.Struct
{
    public unsafe delegate void Lua_PushArg(lua_State* state);
    [StructLayout(LayoutKind.Explicit, Size = 0x610)]
    internal unsafe struct QuestEventHandler
    {
        [FieldOffset(0x00)] public FFXIVClientStructs.FFXIV.Client.Game.Event.QuestEventHandler FCSQuestEventHandler;
        [FieldOffset(0x00)] public long* vtbls;
        [FieldOffset(0x00)] public LuaEventHandler LuaEventHandler;
        [FieldOffset(0x338)] public uint QuestId;
        [FieldOffset(0x340)] public Utf8String QuestName;
        [FieldOffset(0x3A8)] public Utf8String QuestClassName;
        [FieldOffset(0x410)] public Utf8String MessageName;
        [FieldOffset(0x500)] public StdMap<byte, StdSet<ListenerData>> Listeners;
        [FieldOffset(0x508)] public byte ListenersCount;
        #region Function
        public ICallMemberFunctionResult<TRet> CallMemberFunction<TRet>(string functionName, Lua_PushArg pushArg, ICallMemberFunctionResult<TRet> poplate) => LuaHelper.CallMemberFunction(LuaEventHandler.LuaState, LuaEventHandler.LuaKey.StringPtr, functionName, pushArg, poplate);
        public TodoResult GetTodoArgs(byte seq) => CallMemberFunction("GetTodoArgs", state =>
        {
            state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
            state->lua_pushinteger(seq);
        }, new TwoNumberResult()).Value;
        /// <summary>
        /// kind dataid LayoutID unk1 0
        /// </summary>
        /// <returns></returns>
        public bool IsAcceptEvent(IGameObject obj)
        {
            if (IsFunction("IsAcceptEvent"))
            { 
                return CallMemberFunction("IsAcceptEvent", state =>
                {
                    state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
                    state->lua_pushinteger((int)obj.Struct()->GetObjectKind());
                    state->lua_pushinteger((int)obj.Struct()->BaseId);
                    state->lua_pushinteger((int)obj.Struct()->LayoutId);
                    state->lua_pushinteger(1);
                    state->lua_pushinteger(0);
                }, new BoolResult()).Value;
            }
            return false;
        }
        public bool IsAcceptEvent(QuestListenerParamsStruct listener)
        {
            if (IsFunction("IsAcceptEvent"))
            {
                return CallMemberFunction("IsAcceptEvent", state =>
                {
                    state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
                    state->lua_pushinteger(0);
                    state->lua_pushinteger((int)listener.Listener);
                    state->lua_pushinteger((int)listener.ConditionValue);
                    state->lua_pushinteger(listener.QuestUInt8A);
                    state->lua_pushinteger(0);
                }, new BoolResult()).Value;
            }
            return false;
        }
        public bool IsAnnounce(QuestListenerParamsStruct listener)
        {
            if (IsFunction("IsAnnounce"))
            {
                return CallMemberFunction("IsAnnounce", state =>
                {
                    state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
                    state->lua_pushinteger(0);
                    state->lua_pushinteger((int)listener.Listener);
                    state->lua_pushinteger((int)listener.ConditionValue);
                    state->lua_pushinteger(listener.QuestUInt8A);
                    state->lua_pushinteger(0);
                }, new BoolResult()).Value;
            }
            return false;
        }
        public bool IsEventItemUsable(IGameObject obj, uint itemId) => CallMemberFunction("IsEventItemUsable", state => 
        {
            state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
            state->Old.lua_getfield(-10002, obj.Struct()->LuaActor->LuaString.StringPtr);
            state->lua_pushinteger((int)itemId);
        }, new BoolResult()).Value;
        /// <summary>
        /// 方法返回和seq相关
        /// </summary>
        /// <returns></returns>
        public EventItemsResults GetEventItems() => CallMemberFunction("GetEventItems", state => 
        {
            state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
        }, new GetEventItemsResult()).Value;
        public bool TryGetEventItems(out EventItemsResults results) => (results = GetEventItems()).Count > 0;
        public bool IsBattleNpcOwner(bool unk1= true,bool unk2 = true,bool unk3 = false) => CallMemberFunction("IsBattleNpcOwner", state =>
        {
            state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
            state->lua_pushbool(unk1);
            state->lua_pushbool(unk2);
            state->lua_pushbool(unk3);
        }, new BoolResult()).Value;
        public bool IsBattleNpcTriggerOwner(IGameObject obj, bool unk1 = true, bool unk2 = true, bool unk3 = false) => CallMemberFunction("IsBattleNpcTriggerOwner", state =>
        {
            state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
            state->Old.lua_getfield(-10002, obj.Struct()->LuaActor->LuaString.StringPtr);
            state->lua_pushbool(unk1);
            state->lua_pushbool(unk2);
            state->lua_pushbool(unk3);
        }, new BoolResult()).Value;
        public bool IsQualified(QuestListenerParamsStruct listener) => CallMemberFunction("IsQualified", state =>
        {
            state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
            state->lua_pushinteger(0);
            state->lua_pushinteger((int)listener.Listener);
            state->lua_pushinteger((int)listener.ConditionValue);
            state->lua_pushinteger(listener.QuestUInt8A);
            state->lua_pushinteger(0);
        }, new QualifiedResult()).Value;
        public bool IsEnableEventRange(uint dd) => CallMemberFunction("IsEnableEventRange", state =>
        {
            state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
            state->lua_pushinteger(0);
            state->lua_pushinteger(5000000);
            state->lua_pushinteger((int)dd);
            state->lua_pushinteger(0);
            state->lua_pushinteger(10);
        }, new BoolResult()).Value;
        //register function
        public ICallMemberFunctionPoplate IsAcceptSayEvent(IGameObject obj,string str) => CallMemberFunction("IsAcceptSayEvent", state =>
        {
            state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
            state->Old.lua_getfield(-10002, obj.Struct()->LuaActor->LuaString.StringPtr);
            state->lua_pushstring(str);
        }, new BoolResult()) as ICallMemberFunctionPoplate;
        public bool IsInEventRange(uint dd) => CallMemberFunction("IsInEventRange", state =>
        {
            state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
            state->lua_pushinteger((int)dd);
        }, new BoolResult()).Value;
        public bool IsTodoChecked(byte seq) => CallMemberFunction("IsTodoChecked", state =>
        {
            state->Old.lua_getfield(-10002, (Svc.ClientState.LocalPlayer as IGameObject).Struct()->LuaActor->LuaString.StringPtr);
            state->lua_pushinteger(seq);
        }, new BoolResult()).Value;
        public bool IsTargetingPossible(IGameObject obj,bool notEobjAsTrue = true)
        {
            fixed(QuestEventHandler* ptr = &this)
            {
                return ((delegate* unmanaged[Stdcall]<QuestEventHandler*, nint, bool, bool>)(nint)(*(vtbls + 206)))(ptr, obj.Address, notEobjAsTrue);
            }
        }
        public bool IsFunction(string name)
        {
            if (LuaEventHandler.LuaKey.StringPtr != null && LuaEventHandler.LuaState != null && LuaEventHandler.LuaState->State != null)
            {
                var top = LuaEventHandler.LuaState->State->Old.lua_gettop();
                try
                {
                    LuaEventHandler.LuaState->State->Old.lua_getfield(-10002, LuaEventHandler.LuaKey.StringPtr);
                    LuaEventHandler.LuaState->State->Old.lua_getfield(-1, name);
                    return LuaEventHandler.LuaState->State->Old.lua_type(-1) == FFXIVClientStructs.FFXIV.Common.Lua.LuaType.Function;
                }
                catch (Exception e)
                {
                    LogHelper.Error(e);
                }
                finally
                {
                    LuaEventHandler.LuaState->State->Old.lua_settop(top);
                }
            }
            return false;
        }
        public int DIRECTOR_RESULT_ID_INSTANCE_CONTENT()
        {
            if (LuaEventHandler.LuaKey.StringPtr != null && LuaEventHandler.LuaState != null && LuaEventHandler.LuaState->State != null)
            {
                var top = LuaEventHandler.LuaState->State->Old.lua_gettop();
                try
                {
                    LuaEventHandler.LuaState->State->Old.lua_getfield(-10002, LuaEventHandler.LuaKey.StringPtr);
                    LuaEventHandler.LuaState->State->Old.lua_getfield(-1, "DIRECTOR_RESULT_ID_INSTANCE_CONTENT");
                    return (int)LuaEventHandler.LuaState->State->Old.lua_tonumber(-1);
                }
                catch (Exception e)
                {
                    LogHelper.Error(e);
                }
                finally
                {
                    LuaEventHandler.LuaState->State->Old.lua_settop(top);
                }
            }
            return 0;
        }
        #endregion
    }
    [StructLayout(LayoutKind.Explicit,Pack = 4)]
    public struct ListenerData
    {
        [FieldOffset(0x00)] public uint Listener;//1c
        [FieldOffset(0x04)] public byte Unk4;
        [FieldOffset(0x05)] public byte Type;
        [FieldOffset(0x06)] public byte Unk6;
        [FieldOffset(0x08)] public uint Unk8;
        [FieldOffset(0x0C)] public uint ConditionValue;
        [FieldOffset(0x10)] public uint Unk10;
        [FieldOffset(0x14)] public byte Unk14;
        public override string ToString()
        {
            return $"Listener:{Listener.ToString().PadLeft(7,'0')} Unk4:{Unk4.ToString("X").PadLeft(2, '0')} Type:{Type.ToString("X").PadLeft(2, '0'):X} Unk6:{Unk6.ToString("X").PadLeft(2, '0')} Unk8:{Unk8} ConditionValue:{ConditionValue.ToString().PadLeft(7,'0')} Unk10:{Unk10} Unk14:{Unk14}";
        }
    }
    #region New
    [StructLayout(LayoutKind.Explicit, Size = 0x330)]
    public struct QuestEventHandlerVirtualFunctionTable
    {

    }
    [StructLayout(LayoutKind.Explicit, Size = 0x330)]
    public unsafe partial struct LuaEventHandler
    {
        [FieldOffset(0x000)] public EventHandler EventHandler;
        [FieldOffset(0x210)] public LuaState* LuaState;
        [FieldOffset(0x218)] public LuaScriptLoader<LuaEventHandler> LuaScriptLoader;
        [FieldOffset(0x240)] public Utf8String LuaClass;
        [FieldOffset(0x2A8)] public Utf8String LuaKey;
        [FieldOffset(0x310)] public StdMap<uint, Utf8String> Message;
    }
    [StructLayout(LayoutKind.Sequential, Size = 0x28)]
    public unsafe struct LuaScriptLoader<T> where T : unmanaged
    {
        public LuaScriptLoader Loader;
        public LuaState* LuaState;
        public T* Parent;
        public StdMap<uint, Pointer<ResourceHandle>> Handles;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x8)]
    public unsafe partial struct LuaScriptLoader;
    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public unsafe partial struct LuaState
    {
        [FieldOffset(0x08)] public lua_State* State;
        [FieldOffset(0x10)] public bool GCEnabled;
        [FieldOffset(0x18)] public long LastGCRestart;
        [FieldOffset(0x20)] public delegate* unmanaged<lua_State*, int> db_errorfb;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0xB8)]
    public unsafe partial struct lua_State
    {
        [FieldOffset(0x00)] public FFXIVClientStructs.FFXIV.Common.Lua.lua_State Old;
        [FieldOffset(0x00)] public GCheader gch; // CommonHeader
        [FieldOffset(0x0A)] public byte status;
        [FieldOffset(0x10)] public TValue* top; /* first free slot in the stack */
        [FieldOffset(0x18)] public TValue* _base; /* base of current function */
        [FieldOffset(0x20)] public global_State* l_G;
        [FieldOffset(0x28)] public CallInfo* ci; /* call info for current function */
        [FieldOffset(0x30)] public uint* savedpc; /* 'savedpc' of current function */
        [FieldOffset(0x38)] public TValue* stack_last; /* last free slot in the stack */
        [FieldOffset(0x40)] public TValue* stack; /* stack base */
        [FieldOffset(0x48)] public CallInfo* end_ci; /* points after end of ci array*/
        [FieldOffset(0x50)] public CallInfo* base_ci; /* array of CallInfo's */
        [FieldOffset(0x58)] public int stacksize;
        [FieldOffset(0x5C)] public int size_ci; /* size of array 'base_ci' */
        [FieldOffset(0x60)] public ushort nCcalls; /* number of nested C calls */
        [FieldOffset(0x62)] public ushort baseCcalls; /* nested C calls when resuming coroutine */
        [FieldOffset(0x64)] public byte hookmask;
        [FieldOffset(0x65)] public byte allowhook;
        [FieldOffset(0x68)] public int basehookcount;
        [FieldOffset(0x6C)] public int hookcount;
        [FieldOffset(0x70)] public long hook; // lua_Hook
        [FieldOffset(0x78)] public TValue l_gt; /* table of globals */
        [FieldOffset(0x88)] public TValue env; /* temporary place for environments */
        [FieldOffset(0x98)] public GCObject* openupval; /* list of open upvalues in this stack */
        [FieldOffset(0xA0)] public GCObject* gclist;
        [FieldOffset(0xA8)] public long errorJmp; /* current error recover point */
        [FieldOffset(0xB0)] public long errfunc; // ptrdiff_t /* current error handling function (stack index) */
        private static nint fpLua_CheckStack => Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 ?? ?? ?? E8 ?? ?? ?? ?? 48 ?? ?? ?? 44 ?? ?? 44");
        public void lua_checkstack(int count)
        {
            fixed (lua_State* ptr = &this)
            {
                ((delegate* unmanaged[Stdcall]<lua_State*, int, void>)fpLua_CheckStack)(ptr, 10);
            }
        }
        private static nint fpLua_PushInteger => Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 ?? ?? ?? ?? 33 ?? E8 ?? ?? ?? ?? BB");
        public void lua_pushinteger(int value)
        {
            fixed (lua_State* ptr = &this)
            {
                ((delegate* unmanaged[Stdcall]<lua_State*, int, void>)fpLua_PushInteger)(ptr, value);
            }
        }
        private static nint fpLua_ToBool => Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 85 ?? 0F ?? ?? 88 ?? EB");
        public bool lua_tobool(int idx)
        {
            fixed (lua_State* ptr = &this)
            {
                return ((delegate* unmanaged[Stdcall]<lua_State*, int, bool>)fpLua_ToBool)(ptr, idx);
            }
        }
        private static nint fpLua_PushBool => Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 83 ?? ?? 48 ?? ?? 2B");
        public void lua_pushbool(bool value)
        {
            fixed (lua_State* ptr = &this)
            {
                ((delegate* unmanaged[Stdcall]<lua_State*, bool, void>)fpLua_PushBool)(ptr, value);
            }
        }
        private static nint fpLua_PushString => Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 44 ?? ?? ?? ?? ?? ?? ?? 8D ?? ?? 48 ?? ?? ?? 45 ?? ?? E8");
        public void lua_pushstring(byte* value)
        {
            fixed (lua_State* ptr = &this)
            {
                ((delegate* unmanaged[Stdcall]<lua_State*, byte*, void>)fpLua_PushString)(ptr, value);
            }
        }
        public void lua_pushstring(string value)
        {
            int byteCount = Encoding.UTF8.GetByteCount(value);
            Span<byte> span = ((byteCount > 512) ? ((Span<byte>)new byte[byteCount + 1]) : stackalloc byte[byteCount + 1]);
            Span<byte> bytes = span;
            Encoding.UTF8.GetBytes(value, bytes);
            bytes[byteCount] = 0;
            fixed (byte* k2 = bytes)
            {
                lua_pushstring(k2);
            }
        }
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public unsafe struct CallInfo
    {
        [FieldOffset(0x00)] public TValue* _base; /* base for this function */
        [FieldOffset(0x08)] public TValue* func; /* function index in the stack */
        [FieldOffset(0x10)] public TValue* top; /* top for this function */
        [FieldOffset(0x18)] public uint* savedpc;
        [FieldOffset(0x20)] public int nresults; /* expected number of results from this function */
        [FieldOffset(0x24)] public int tailcalls; /* number of tail calls lost under this entry */
    }
    [StructLayout(LayoutKind.Explicit, Size = 0xB8)]
    public struct GCObject
    {
        [FieldOffset(0x00)] public GCheader gch;
        [FieldOffset(0x00)] public TString ts;
        [FieldOffset(0x00)] public Udata u;
        [FieldOffset(0x00)] public Closure cl;
        [FieldOffset(0x00)] public Table h;
        [FieldOffset(0x00)] public Proto p;
        [FieldOffset(0x00)] public UpVal uv;
        [FieldOffset(0x00)] public lua_State th; /* thread */
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public struct TValue
    {
        [FieldOffset(0x00)] public Value value;
        [FieldOffset(0x08)] public int tt;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    public unsafe struct TKey
    {
        [FieldOffset(0x00)] public TValue tvk;
        [FieldOffset(0x10)] public Node* next; /* for chaining */
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public struct Node
    {
        [FieldOffset(0x00)] public TValue i_val;
        [FieldOffset(0x10)] public TKey i_key;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x08)]
    public unsafe struct Value
    {
        // Union of all Lua values
        [FieldOffset(0x00)] public GCObject* gc;
        [FieldOffset(0x00)] public void* p;
        [FieldOffset(0x00)] public double n;
        [FieldOffset(0x00)] public int b;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public unsafe struct UpVal
    {
        [FieldOffset(0x00)] public GCheader gch; // CommonHeader
        [FieldOffset(0x10)] public TValue* v; /* points to stack or to its own value */
        [FieldOffset(0x18)] public TValue value; /* the value (when closed) */
        [FieldOffset(0x18)] public UpVal* prev; /* double linked list (when open) */
        [FieldOffset(0x20)] public UpVal* next;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x78)]
    public unsafe struct Proto
    {
        [FieldOffset(0x00)] public GCheader gch; // CommonHeader
        [FieldOffset(0x10)] public TValue* k; /* constants used by the function */
        [FieldOffset(0x18)] public uint* code;
        [FieldOffset(0x20)] public Proto** p; /* functions defined inside the function */
        [FieldOffset(0x28)] public int* lineinfo; /* map from opcodes to source lines */
        [FieldOffset(0x30)] public LocVar* locvars; /* information about local variables */
        [FieldOffset(0x38)] public TString** upvalues; /* upvalue names */
        [FieldOffset(0x40)] public TString* source;
        [FieldOffset(0x48)] public int sizeupvalues;
        [FieldOffset(0x4C)] public int sizek; /* size of 'k' */
        [FieldOffset(0x50)] public int sizecode;
        [FieldOffset(0x54)] public int sizelineinfo;
        [FieldOffset(0x58)] public int sizep; /* size of 'p' */
        [FieldOffset(0x5C)] public int sizelocvars;
        [FieldOffset(0x60)] public int linedefined;
        [FieldOffset(0x64)] public int lastlinedefined;
        [FieldOffset(0x68)] public GCObject* gclist;
        [FieldOffset(0x70)] public byte nups; /* number of upvalues */
        [FieldOffset(0x71)] public byte numparams;
        [FieldOffset(0x72)] public byte is_vararg;
        [FieldOffset(0x73)] public byte maxstacksize;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x0A)]
    public unsafe struct GCheader
    {
        // struct version of the CommonHeader macro
        [FieldOffset(0x00)] public GCObject* next;
        [FieldOffset(0x08)] public byte tt;
        [FieldOffset(0x09)] public byte marked;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public unsafe struct LocVar
    {
        [FieldOffset(0x00)] public TString* varname;
        [FieldOffset(0x08)] public int startpc; /* first point where variable is active */
        [FieldOffset(0x0C)] public int endpc; /* first point where variable is dead */
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    public struct TString
    {
        [FieldOffset(0x00)] public GCheader gch; // CommonHeader
        [FieldOffset(0x0A)] public byte reserved;
        [FieldOffset(0x0C)] public uint hash;
        [FieldOffset(0x10)] public ulong len;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x38)]
    public unsafe struct CClosure
    {
        [FieldOffset(0x00)] public GCheader gch; // CommonHeader
        [FieldOffset(0x0A)] public byte isC;
        [FieldOffset(0x0B)] public byte nupvalues;
        [FieldOffset(0x10)] public GCObject* gclist;
        [FieldOffset(0x18)] public Table* env;
        // end of ClosureHeader
        [FieldOffset(0x20)] public delegate* unmanaged<lua_State*, int> f; // lua_CFunction
        [FieldOffset(0x28)] internal TValue _upvalue;  // TValue upvalue[1];
        public Span<TValue> upvalue => new(Unsafe.AsPointer(ref _upvalue), nupvalues);
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x38)]
    public struct Closure
    {
        [FieldOffset(0x00)] public CClosure c;
        [FieldOffset(0x00)] public LClosure l;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public unsafe struct LClosure
    {
        [FieldOffset(0x00)] public GCheader gch; // CommonHeader
        [FieldOffset(0x0A)] public byte isC;
        [FieldOffset(0x0B)] public byte nupvalues;
        [FieldOffset(0x10)] public GCObject* gclist;
        [FieldOffset(0x18)] public Table* env;
        // end of ClosureHeader
        [FieldOffset(0x20)] public Proto* p;
        [FieldOffset(0x28)] internal UpVal* _upvals; // UpVal* upvals[1];
        public Span<Pointer<UpVal>> upvals
        {
            get
            {
                fixed (UpVal** ptr = &_upvals)
                    return new Span<Pointer<UpVal>>(ptr, nupvalues);
            }
        }
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public unsafe struct Udata
    {
        [FieldOffset(0x00)] public GCheader gch; // CommonHeader
        [FieldOffset(0x10)] public Table* metatable;
        [FieldOffset(0x18)] public Table* env;
        [FieldOffset(0x20)] public ulong len;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x40)]
    public unsafe struct Table
    {
        [FieldOffset(0x00)] public GCheader gch; // CommonHeader
        [FieldOffset(0x0A)] public byte flags; /* 1<<p means tagmethod(p) is not present */
        [FieldOffset(0x0B)] public byte lsizenode; /* log2 of size of 'node' array */
        [FieldOffset(0x10)] public Table* metatable;
        [FieldOffset(0x18)] public TValue* array; /* array part */
        [FieldOffset(0x20)] public Node* node;
        [FieldOffset(0x28)] public Node* lastfree; /* any free position is before this position */
        [FieldOffset(0x30)] public GCObject* gclist;
        [FieldOffset(0x38)] public int sizearray; /* size of 'array' array */
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x1B0)]
    public unsafe partial struct global_State
    {
        [FieldOffset(0x00)] public stringtable strt; /* hash table for strings */
        [FieldOffset(0x10)] public delegate* unmanaged<void*, void*, ulong, ulong, void*> frealloc; // lua_Alloc /* function to reallocate memory */
        [FieldOffset(0x18)] public void* ud; /* auxiliary data to 'frealloc' */
        [FieldOffset(0x20)] public byte currentwhite;
        [FieldOffset(0x21)] public byte gcstate; /* state of garbage collector */
        [FieldOffset(0x24)] public int sweepstrgc; /* position of sweep in 'strt' */
        [FieldOffset(0x28)] public GCObject* rootgc; /* list of all collectable objects */
        [FieldOffset(0x30)] public GCObject** sweepgc; /* position of sweep in 'rootgc' */
        [FieldOffset(0x38)] public GCObject* gray; /* list of gray objects */
        [FieldOffset(0x40)] public GCObject* grayagain; /* list of objects to be traversed atomically */
        [FieldOffset(0x48)] public GCObject* weak; /* list of weak tables (to be cleared) */
        [FieldOffset(0x50)] public GCObject* tmudata; /* last element of list of userdata to be GC */
        [FieldOffset(0x58)] public Mbuffer buff; /* temporary buffer for string concatenation */
        [FieldOffset(0x70)] public ulong GCthreshold; // lu_mem
        [FieldOffset(0x78)] public ulong totalbytes; // lu_mem /* number of bytes currently allocated */
        [FieldOffset(0x80)] public ulong estimate; // lu_mem /* an estimate of number of bytes actually in use */
        [FieldOffset(0x88)] public ulong gcdept; // lu_mem /* how much GC is 'behind schedule' */
        [FieldOffset(0x90)] public int gcpause; /* size of pause between successive GCs */
        [FieldOffset(0x94)] public int gcstepmul; /* GC 'granularity' */
        [FieldOffset(0x98)] public delegate* unmanaged<lua_State*, int> panic; // lua_CFunction /* to be called in unprotected errors */
        [FieldOffset(0xA0)] public TValue l_registry;
        [FieldOffset(0xB0)] public lua_State* mainthread;
        [FieldOffset(0xB8)] public UpVal uvhead; /* head of double-linked list of all open upvalues */
        [FieldOffset(0xE0)] internal fixed long _mt[9]; /* metatables for basic types */
        [FieldOffset(0x128)] internal fixed long _tmname[17]; /* array with tag-method names */
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public unsafe struct stringtable
    {
        [FieldOffset(0x00)] public GCObject** hash;
        [FieldOffset(0x08)] public uint nuse; /* number of elements */
        [FieldOffset(0x0C)] public int size;
    }
    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    public unsafe struct Mbuffer
    {
        [FieldOffset(0x00)] public byte* buffer; // char*
        [FieldOffset(0x08)] public ulong n;
        [FieldOffset(0x10)] public ulong buffsize;
    }
    #endregion
}
