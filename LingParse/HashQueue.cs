using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LingParse
{
    class HashQueue
    {
        private Queue<List<ParseNode>> _q;
        private HashSet<int> _h;

        public HashQueue()
        {
            _q = new Queue<List<ParseNode>>();
            _h = new HashSet<int>();
        }

        public static int GetHashCodeFor(List<ParseNode> obj)
        {
            if (obj.Count == 0) return 0;
            else
            {
                int hash = obj[0].GetHashCode();
                for (int i = 1; i < obj.Count; i++)
                    hash ^= obj[i].GetHashCode();
                return hash;
            }
        }

        public bool Enqueue(List<ParseNode> obj)
        {
            // Oh, yes, hacked-together "hash code" :(
            int hash = GetHashCodeFor(obj);
            if (_h.Contains(hash))
                return false;
            else
            {
                _q.Enqueue(obj);
                _h.Add(hash);
                return true;
            }
        }

        public List<ParseNode> Dequeue()
        {
            List<ParseNode> obj = _q.Dequeue();
            _h.Remove(GetHashCodeFor(obj));
            return obj;
        }

        public int Count
        {
            get { return _q.Count; }
        }

        public bool Contains(List<ParseNode> obj)
        {
            return _h.Contains(GetHashCodeFor(obj));
        }
    }
}
