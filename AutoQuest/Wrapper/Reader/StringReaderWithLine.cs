namespace AutoQuest.Wrapper.Reader
{
    internal class StringReaderWithLine : StringReader
    {
        private Dictionary<int, string?> LineString = new();
        private int Line = 0;
        public int CurrentLine
        {
            get { return Line; }
            set
            {
                Line = value < 0 ? 0 : value;
            }

        }
        public StringReaderWithLine(string str) : base(str) { }
        public override string? ReadLine()
        {
            if (LineString.TryGetValue(Line, out var str))
            {
                Line++;
                return str;
            }
            str = base.ReadLine();
            LineString.Add(Line++, str);
            return str;
        }
        public string? ReadLine(int line)
        {
            if (LineString.TryGetValue(line, out var str))
            {
                return str;
            }
            while (Line < line)
            {
                str = ReadLine();
            }
            return str;
        }
    }
}
