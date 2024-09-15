using ECommons.DalamudServices;
using Lumina;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets2;
using Lumina.Text;
using Lumina.Text.Payloads;
using SaintCoinach.Text;
using System.Text;

namespace AutoQuest.Excel.GeneratedSheets
{
    /// <summary>
    /// 没有特性 因为表的名字不一样 需要通过string来创建
    /// </summary>
    internal class QuestMessage : ExcelRow
    {
        private SeString? _value = null;
        public SeString Value 
        {
            get
            {
                if (_value == null && ValueDecoder != null)
                {
                    _value = new SeString(ValueDecoder.ToString());
                }
                return _value;
            }
        }
        public SeString Variable { get; set; }
        public  XivString ValueDecoder { get; set; }
        public void GetRef()
        {
            foreach(var i in ValueDecoder.Children)
            {
                if(i.Tag == TagType.Sheet)
                    i.ToString(new StringBuilder());
            }
        }
        public override void PopulateData(RowParser parser, GameData gameData, Language language)
        {
            try
            {
                base.PopulateData(parser, gameData, language);
                Variable = parser.ReadColumn<SeString>(0);
                var _Value = parser.ReadColumn<SeString>(1);
                ValueDecoder = new XivStringDecoder().Decode(_Value?.RawData.ToArray());
            }
            catch(Exception ex)
            {
                LogHelper.Error($"{SheetName} {RowId} {SubRowId}");
                throw;
            }
        }
        public override string ToString()
        {
            return Variable.ToString()+":"+Value.ToString();
        }
        public static byte[] EventItemLink = [0x02, 0x28, 0x11, 0xff, 0x0a, 0x45, 0x76, 0x65, 0x6e, 0x74, 0x49, 0x74, 0x65, 0x6d, 0x76];
        private SeString GetValueString(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var payloads = new List<BasePayload>();
                using (var reader = new BinaryReader(stream))
                {
                    var a = new List<byte>();
                    byte b = 0;
                    var count = bytes.Length;
                    try
                    {
                        while (reader.BaseStream.Position < count)
                        {
                            b = reader.ReadByte();
                            if (b == 2)//link
                            {
                                if (reader.ReadByte() != 0x28)
                                {
                                    reader.BaseStream.Position--;
                                }
                                else
                                {
                                    if (a.Count > 0)
                                        payloads.Add(new TextPayload(new SeString(a.ToArray())));
                                    a.Clear();
                                    var c = new List<byte>
                                {
                                    b,0x28
                                };
                                    while (reader.BaseStream.Position < count)
                                    {
                                        b = reader.ReadByte();
                                        c.Add(b);
                                        if (b == 3)
                                        {
                                            payloads.Add(new TextPayload(new LinkPayload([.. c]).ToString()));
                                            break;
                                        }
                                    }
                                    continue;
                                }
                            }
                            a.Add(b);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(BitConverter.ToString(a.ToArray()));
                        LogHelper.Error(bytes.Length);
                        LogHelper.Error(BitConverter.ToString(bytes));
                        throw;
                    }
                    if (a.Count > 0)
                    {
                        payloads.Add(new TextPayload(new SeString(a.ToArray())));
                    }
                    return new SeString(payloads);
                }
            }
        }
    }
    internal class LinkPayload : BasePayload
    {
        public byte Type;
        public byte[] Row;
        public List<byte[]?> Lists = new();
        public string SheetName = "null";
        public string Value = "null";
        public LinkPayload(byte[] bytes)
        {
            using (var reader = new BinaryReader(new MemoryStream(bytes)))
            {
                Row = bytes;
                reader.ReadByte();//start
                Type = reader.ReadByte();//type
                var allcount = reader.ReadByte();//count

                switch(Type)
                {
                    case 0x28:
                        if (reader.ReadByte() == 0xff)//is string?
                        {
                            var sheetNameCount = reader.ReadByte();
                            SheetName = Encoding.ASCII.GetString(reader.ReadBytes(sheetNameCount - 1));
                            switch (reader.ReadByte())
                            {
                                case 0xFF: //string
                                    var valueCount = reader.ReadByte();
                                    Value = Encoding.ASCII.GetString(reader.ReadBytes(sheetNameCount - 1));
                                    break;
                                case 0xF6: // int?
                                    Value = ReadInt(reader, 4).ToString();
                                    break;
                                case 0xF4: // int?
                                    Value = ReadInt(reader, 3).ToString();
                                    break;
                                case 0xF2: // int?
                                    Value = ReadInt(reader, 3).ToString();
                                    break;
                                default:
                                    throw new Exception($"Unknow link {BitConverter.ToString(bytes)}");
                            }
                        }
                        break;
                }
            }
        }
        public uint ReadInt(BinaryReader reader,byte count)
        {
            uint a = 0;
            if(count >=4)
                a |= ((uint)reader.ReadByte()) << 16;
            if(count >= 3)
                a |= ((uint)reader.ReadByte()) << 8;
            if(count >= 2)
                a |= reader.ReadByte();
            return a;
        }
        public string DefaultString => $"Sheet({SheetName},{Value},0)";
        public override string ToString()
        {
            if (SheetName == "BNpcName")
                return $"{DefaultString}({Svc.Data.GetExcelSheet<BNpcName>().GetRow(uint.Parse(Value)).Singular})";
            if (SheetName == "EventItem")
                return $"{DefaultString}({Svc.Data.GetExcelSheet<EventItem>().GetRow(uint.Parse(Value)).Singular})";
            if (SheetName == "Item")
                return $"{DefaultString}({Svc.Data.GetExcelSheet<Item>().GetRow(uint.Parse(Value)).Singular})";
            return DefaultString;
        }
    }
}
