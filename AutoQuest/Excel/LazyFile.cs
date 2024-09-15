using Lumina;
using Lumina.Data;

namespace AutoQuest.Excel
{
    internal class LazyFile<T> where T: FileResource
    {
        private string FilePath;
        private T? _value;
        private GameData _gameData;
        public string Path => FilePath;
        public LazyFile(GameData gameData,string path)
        {
            _gameData = gameData;
            FilePath = path;
        }
        public T? Value
        {
            get
            {
                if (IsValueCreated)
                {
                    return _value;
                }
                _value = _gameData.GetFile<T>(FilePath);
                return _value;
            }
        }
        public bool IsValueCreated => _value != null;
    }
}
