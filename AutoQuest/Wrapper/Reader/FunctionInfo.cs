using ImGuiNET;
using System.Text.RegularExpressions;

namespace AutoQuest.Wrapper.Reader
{
    internal class FunctionInfo
    {
        public string Name { get; set; }
        public Dictionary<uint, string?> Code { get; set; } = new Dictionary<uint, string?>();
        public uint StartLine { get; set; }
        public uint EndLine { get; set; }
        public bool IsScene { get; set; }
        public bool IsNpcTrade { get; set; }
        public bool IsQuestReward { get; set; }
        public uint SceneIndex { get; set; }
        public bool IsInventory { get; set; }
        public bool IsBattleStart { get; set; }
        public bool IsBattleCheck { get; set; }
        public FunctionInfo(StringReaderWithLine reader, string questName)
        {
            string? str;
            while ((str = reader.ReadLine()) != null)
            {
                var line = reader.CurrentLine - 1;
                Code.Add((uint)line, str);
                CheckFunctionStart(str, questName, line);
                CheckQuestReward(str);
                CheckNpcTrade(str);
                CheckInventory(str);
                CheckBattle(str);
                if (CheckFunctionEnd(str, line))
                    break;
            }
        }

        private void CheckBattle(string str)
        {
            var reg = Regex.Match(str, @"_ARG_0_:GetQuestBattleProgress\(\) == 0");
            if (reg.Success)
            {
                IsBattleStart = true;
            }
            reg = Regex.Match(str, @"_ARG_0_:GetQuestBattleProgress\(\) == 1");
            if (reg.Success)
            {
                IsBattleCheck = true;
            }
        }

        private void CheckFunctionStart(string str, string questName, int line)
        {
            if (Name == null)
            {
                var reg = Regex.Match(str, @"^\s{2}function\s{1}(\w+)\.(\w+)\(");
                if (reg.Success)
                {
                    if (reg.Groups[1].Value == questName)
                    {
                        Name = reg.Groups[2].Value;
                        StartLine = (uint)line;
                        var reg2 = Regex.Match(Name, @"OnScene(\d{5})");
                        if (reg2.Success)
                        {
                            IsScene = true;
                            SceneIndex = uint.Parse(reg2.Groups[1].Value);
                        }

                    }
                }
            }
        }
        private bool CheckFunctionEnd(string str, int line)
        {
            if (EndLine == 0)
            {
                var reg = Regex.Match(str, @"^\s{2}end");
                if (reg.Success)
                {
                    EndLine = (uint)line;
                    return true;
                }
            }
            return false;
        }
        private void CheckQuestReward(string str)
        {
            if (!IsQuestReward)
            {
                var reg = Regex.Match(str, @"QuestReward");
                if (reg.Success)
                {
                    IsQuestReward = true;
                }
            }
        }
        private void CheckNpcTrade(string str)
        {
            if (!IsQuestReward)
            {
                var reg = Regex.Match(str, @":SetNpcTradeItem\(");
                if (reg.Success)
                {
                    IsNpcTrade = true;
                }
            }
        }
        private void CheckInventory(string str)
        {
            if (!IsQuestReward)
            {
                var reg = Regex.Match(str, @":Inventory\(");
                if (reg.Success)
                {
                    IsInventory = true;
                }
            }
        }
        public void Draw()
        {
            ImGui.Text($"{IsScene} Trade:{IsNpcTrade} Reward:{IsQuestReward} Inventory{IsInventory}");
            foreach (var i in Code)
            { 
                ImGui.Text(i.Value); 
            }  
        }
    }
}
