using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LingParse
{
    class GrammarDefinition
    {
        public GrammarDefinition(int parent, int l, int r)
        {
            Parent = parent;
            Left = l;
            Right = r;
        }

        public int Parent {
            get;
            private set;
        }

        public int Left {
            get;
            private set;
        }

        public int Right {
            get;
            private set;
        }

        public override string ToString()
        {
            return String.Format("{0} -> {1} {2}", SyntaxCategories.DefaultSyntaxCategories.NameForIndex(Parent), SyntaxCategories.DefaultSyntaxCategories.NameForIndex(Left), SyntaxCategories.DefaultSyntaxCategories.NameForIndex(Right));
        }
    }
}
