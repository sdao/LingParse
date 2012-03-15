using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LingParse
{
    class GrammarDefinition
    {
        public GrammarDefinition(Categories parent, Categories l, Categories r)
        {
            Parent = parent;
            Left = l;
            Right = r;
        }

        public Categories Parent {
            get;
            private set;
        }

        public Categories Left {
            get;
            private set;
        }

        public Categories Right {
            get;
            private set;
        }

        public override string ToString()
        {
            return String.Format("{0} -> {1} {2}", Grammar.NameForUnit(Parent), Grammar.NameForUnit(Left), Grammar.NameForUnit(Right));
        }
    }
}
