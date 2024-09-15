using AutoQuest.Wrapper.Reader;
using System.Text.RegularExpressions;
using UnluacNET;

namespace AutoQuest
{
    internal partial class QuestWrapper : IDisposable
    {
        public Lazy<Dictionary<string,FunctionInfo>> DecompileCode;
        private Dictionary<string, FunctionInfo> Decompile()
        {
            using (var stream = new MemoryStream(Quest.LuaFile.Value?.Data))
            {
                if (stream != null)
                {
                    var header = new BHeader(stream);
                    var main = header.Function.Parse(stream, header);
                    var d = new Decompiler(main);
                    d.Decompile();
                    using var sw = new StringWriter();
                    d.Print(new Output(sw));
                    return Parse(sw.ToString());
                }
            };
            throw new Exception("不能解析lua");
        }
        internal void DecompileAndSave()
        {
            using (var stream = new MemoryStream(Quest.LuaFile.Value?.Data))
            {
                if (stream != null)
                {
                    var header = new BHeader(stream);
                    var main = header.Function.Parse(stream, header);
                    var d = new Decompiler(main);
                    d.Decompile();
                    using var sw = new StringWriter();
                    d.Print(new Output(sw));
                    using var ss = new StreamWriter($"D:\\Dlamud_Quest\\{QuestId}_dec.lua");
                    ss.Write(sw.ToString());
                }
            };
        }
        private Dictionary<string, FunctionInfo> Parse(string str)
        {
            Dictionary<string, FunctionInfo> dict = new Dictionary<string, FunctionInfo>();
            using (var reader = new StringReaderWithLine(str))
            {
                string? tem;
                var name = Quest.Id.ToString().Split('_')[0];
                while ((tem = reader.ReadLine()) != null)
                {
                    if (Regex.Match(tem, @"^\s{2}function").Success)
                    {
                        reader.CurrentLine -= 1;
                        var a = new FunctionInfo(reader, name);
                        dict.Add(a.Name, a);
                    }
                }
            }
            return dict;
        }
        public bool IsQuestRewardScene(ushort scene)
        {
            foreach (var (_, func) in DecompileCode.Value)
            {
                if (func.IsScene && scene == func.SceneIndex)
                    return func.IsQuestReward;
            }
            return false;
        }
        public bool IsNpcTradeScene(ushort scene)
        {
            foreach (var (_, func) in DecompileCode.Value)
            {
                if (func.IsScene && scene == func.SceneIndex)
                    return func.IsNpcTrade;
            }
            return false;
        }
        public bool IsInventoryScene(ushort scene)
        {
            foreach (var (_, func) in DecompileCode.Value)
            {
                if (func.IsScene && scene == func.SceneIndex)
                    return func.IsInventory;
            }
            return false;
        }
        private QuestWrapper()
        {
            DecompileCode = new(Decompile);
        }
        void IDisposable.Dispose()
        {

        }
    }
}
