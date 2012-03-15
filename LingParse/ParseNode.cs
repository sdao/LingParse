using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LingParse
{
    class ParseNode
    {
        static ParseNode _empty;

        public static ParseNode EmptyParseNode() {
            if (_empty == null)
                _empty = new ParseNode(Categories.None, null);
            return _empty;
        }

        public ParseNode(Categories u, string term)
        {
            Type = u;
            Value = term;
        }

        public ParseNode(Categories u, string term, ParseNode l, ParseNode r) : this(u, term)
        {
            Left = l;
            Right = r;
        }

        public Categories Type
        {
            get;
            private set;
        }

        public string Value
        {
            get;
            private set;
        }

        public ParseNode Left
        {
            get;
            private set;
        }

        public ParseNode Right
        {
            get;
            private set;
        }

        public ParseNode Copy()
        {
            return new ParseNode(Type, Value, Left, Right);
        }

        public override string ToString()
        {
            if (Type == Categories.None)
            {
                return "-";
            }
            else if (Value != null)
            {
                return String.Format("{0}:\t{1}", Grammar.NameForUnit(Type), Value);
            }
            else if (Right != null && Right.Type != Categories.None)
            {
                return String.Format("{0} ->\n\t{1}\n\t{2}", Grammar.NameForUnit(Type), Left.ToString().Replace("\n", "\n\t"), Right.ToString().Replace("\n", "\n\t"));
            }
            else
            {
                return String.Format("{0} ->\n\t{1}", Grammar.NameForUnit(Type), Left.ToString().Replace("\n", "\n\t"));
            }
        }

        public override int GetHashCode()
        {
            int hash = Type.GetHashCode() * 17;
            if (Value != null)
                hash ^= Value.GetHashCode();
            if (Left != null)
                hash ^= (Left.GetHashCode() * 33);
            if (Right != null)
                hash ^= (Right.GetHashCode() * 33);

            return hash;
        }
    }
}
