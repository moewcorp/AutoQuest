using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace AutoQuest.Excel
{
    internal class LazyRowByExcelName<T> : ILazyRow where T : ExcelRow
    {
        private readonly GameData _gameData;
        private readonly uint _row;
        private readonly Language _language;
        private readonly string _name;
        public string Path => _name;
        private T? _value;
        public uint Row => _row;
        public bool IsValueCreated => _value != null;
        public Language Language => _language;
        public ExcelRow? RawRow => _value;
        public LazyRowByExcelName(GameData gameData, uint row, string excelName, Language language = Language.None)
        {
            _gameData = gameData;
            _row = row;
            _language = language;
            _name = excelName;
        }
        public T? Value
        {
            get
            {
                if (IsValueCreated)
                {
                    return _value;
                }
                _value = _gameData.Excel.GetSheet<T>(_language, _name)?.GetRow(_row);
                return _value;
            }
        }
        public override string ToString()
        {
            return $"{typeof(T).FullName}#{_row}";
        }
    }
}
