using System.Collections;

namespace AutoQuest.Lua.MemberFunction
{
    internal class GetEventItemsResult : ICallMemberFunctionResult<EventItemsResults>, ICallMemberFunctionPoplate
    {
        public EventItemsResults Value
        {
            get
            {
                EventItemsResults ret = new EventItemsResults();
                for (int i = 0;i < Result.Count/3;i++)
                {
                    if (Result.TryGetValue(Result.Count - 1 - (i * 3), out var ite) && Result.TryGetValue(Result.Count - 2 - (i * 3), out var c) && Result.TryGetValue(Result.Count - 3 - (i * 3), out var n))
                    {
                        if(ite is double uite && c is double cc && n is bool u)
                            ret[i] = new EventItemsResultItem()
                            {
                                ItemId = (uint)uite,
                                Count = (uint)cc,
                                Unk = u
                            };
                    }
                }
                ret.Count = Result.Count / 3;
                return ret;
            }
        }

        public bool HasResult  => Result.Count > 0;

        public Dictionary<int, object?> Result { get; private set; } = new();
    }
    public struct EventItemsResultItem
    {
        public uint ItemId;
        public uint Count;
        public bool Unk;
    }
    public struct EventItemsResults : IEnumerable<EventItemsResultItem>
    {
        public int Count;
        public EventItemsResultItem Item1;
        public EventItemsResultItem Item2;
        public EventItemsResultItem Item3;
        public EventItemsResultItem this[int index]
        {
            get
            {
                return index switch
                {
                    0 => Item1,
                    1 => Item2,
                    2 => Item3,
                    _ => throw new ArgumentException()
                };
            }
            set
            {
                switch(index)
                {
                    case 0:Item1 = value; break;
                    case 1:Item2 = value; break;
                    case 2:Item3 = value; break;
                    default:break;
                }
            }
        }

        public IEnumerator<EventItemsResultItem> GetEnumerator()
        {
            yield return Item1;
            yield return Item2;
            yield return Item3;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
