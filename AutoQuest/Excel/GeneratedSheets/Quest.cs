using Lumina;
using Lumina.Data;
using Lumina.Excel;
using UnluacNET;

namespace AutoQuest.Excel.GeneratedSheets
{

    internal class Quest : Lumina.Excel.GeneratedSheets2.Quest
    {
        public LazyRowByExcelName<QuestMessage>[] AllQuestMessage;
        public LazyRowByExcelName<QuestMessage>[] QuestSeqMessages;
        public LazyRowByExcelName<QuestMessage>[] QuestTodoMessages;
        public LazyRowByExcelName<QuestMessage>[] QuestTextMessages;
        public LazyFile<FileResource> LuaFile;
        public string MessagesPath { get; private set; }
        public LazyRowByExcelName<QuestMessage>[] QuestDescription => QuestSeqMessages;
        public override void PopulateData(RowParser parser, GameData gameData, Language language)
        {
            base.PopulateData(parser, gameData, language);


            if (Id.ToString().Contains('_'))
            {
                MessagesPath = $"quest/{Id.ToString().Split('_')[1][..3]}/{Id}";
                var sheet = gameData.Excel.GetSheetRaw(MessagesPath);

                AllQuestMessage = new LazyRowByExcelName<QuestMessage>[sheet.RowCount];
                QuestSeqMessages = new LazyRowByExcelName<QuestMessage>[24];
                QuestTodoMessages = new LazyRowByExcelName<QuestMessage>[24];
                QuestTextMessages = new LazyRowByExcelName<QuestMessage>[sheet.RowCount - 48];
                for (var i = 0; i < sheet.RowCount; i++)
                {
                    var Messages = new LazyRowByExcelName<QuestMessage>(gameData, (uint)i, MessagesPath, language);
                    AllQuestMessage[i] = Messages;
                    if (i < 24)
                    {
                        QuestSeqMessages[i] = Messages;
                    }
                    else if (i < 48)
                    {
                        QuestTodoMessages[i - 24] = Messages;
                    }
                    else
                    {
                        QuestTextMessages[i - 48] = Messages;
                    }
                }

                var path = $"game_script/quest/{Id.ToString().Split('_')[1][..3]}/{Id}.luab";
                LuaFile = new LazyFile<FileResource>(gameData, path);
            }
        }
    }
}
