using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LingParse
{
    public enum Categories
    {
        None, Noun, Verb, Adjective, Preposition, Determiner, NounPhrase, VerbPhrase, PrepositionPhrase, NBar, Sentence
    }

    class Grammar
    {
        static Grammar _default;

        public static Grammar DefaultGrammar()
        {
            if (_default == null)
            {
                _default = new Grammar();

                _default.Rules.Add(new GrammarDefinition(Categories.NBar, Categories.Adjective, Categories.NBar));
                _default.Rules.Add(new GrammarDefinition(Categories.NBar, Categories.Noun, Categories.None));
                _default.Rules.Add(new GrammarDefinition(Categories.NounPhrase, Categories.Determiner, Categories.NBar));
                _default.Rules.Add(new GrammarDefinition(Categories.NounPhrase, Categories.NBar, Categories.None));
                _default.Rules.Add(new GrammarDefinition(Categories.NounPhrase, Categories.NounPhrase, Categories.PrepositionPhrase));

                _default.Rules.Add(new GrammarDefinition(Categories.PrepositionPhrase, Categories.Preposition, Categories.NounPhrase));

                _default.Rules.Add(new GrammarDefinition(Categories.VerbPhrase, Categories.Verb, Categories.None));
                _default.Rules.Add(new GrammarDefinition(Categories.VerbPhrase, Categories.Verb, Categories.NounPhrase));
                _default.Rules.Add(new GrammarDefinition(Categories.VerbPhrase, Categories.VerbPhrase, Categories.PrepositionPhrase));

                _default.Rules.Add(new GrammarDefinition(Categories.Sentence, Categories.NounPhrase, Categories.VerbPhrase));
            }
            return _default;
        }

        private Grammar()
        {
            Rules = new List<GrammarDefinition>();
        }

        public List<GrammarDefinition> Rules
        {
            get;
            private set;
        }

        public List<List<ParseNode>> Parse(List<ParseNode> input)
        {
            List<ParseNode> tree = input.GetRange(0, input.Count);
            List<List<ParseNode>> outputList = new List<List<ParseNode>>();
            HashQueue workQueue = new HashQueue();
            
            workQueue.Enqueue(tree);

            while (workQueue.Count > 0)
            {
                ParseOnePass(workQueue, outputList);
            }

            return outputList;
        }

        private void ParseOnePass(HashQueue work, List<List<ParseNode>> output)
        {
            List<ParseNode> input = work.Dequeue();

            for (int i = 0; i < input.Count; i++)
            {
                ParseNode l = input[i];
                ParseNode r = i + 1 < input.Count ? input[i + 1] : ParseNode.EmptyParseNode();

                foreach (GrammarDefinition rule in Rules)
                {
                    int modify = 0;
                    if (rule.Right == Categories.None)
                    {
                        if (rule.Left == l.Type)
                        {
                            modify = 1;
                        }
                    }
                    else
                    {
                        if (rule.Left == l.Type && rule.Right == r.Type)
                        {
                            modify = 2;
                        }
                    }

                    if (modify > 0)
                    {
                        List<ParseNode> modifiedInput = input.GetRange(0, input.Count);
                        modifiedInput.RemoveRange(i, modify);

                        ParseNode parent = new ParseNode(rule.Parent, null, l, modify > 1 ? r : ParseNode.EmptyParseNode());
                        modifiedInput.Insert(i, parent);

                        if (modifiedInput.Count == 1 && modifiedInput[0].Type == Categories.Sentence)
                            output.Add(modifiedInput);
                        else
                            work.Enqueue(modifiedInput);
                    }
                }
            }
        }

        private void ParseRecurse(List<ParseNode> input, List<List<ParseNode>> output)
        {
            for (int i = 0; i < input.Count; i++)
            {
                ParseNode l = input[i];
                ParseNode r = i + 1 < input.Count ? input[i + 1] : ParseNode.EmptyParseNode();

                foreach (GrammarDefinition rule in Rules)
                {
                    if (rule.Right == Categories.None)
                    {
                        if (rule.Left == l.Type)
                        {
                            List<ParseNode> modifiedInput = input.GetRange(0, input.Count);
                            modifiedInput.RemoveRange(i, 1);

                            ParseNode parent = new ParseNode(rule.Parent, null, l, ParseNode.EmptyParseNode());
                            modifiedInput.Insert(i, parent);

                            ParseRecurse(modifiedInput, output);
                        }
                    }
                    else
                    {
                        if (rule.Left == l.Type && rule.Right == r.Type)
                        {
                            List<ParseNode> modifiedInput = input.GetRange(0, input.Count);
                            modifiedInput.RemoveRange(i, 2);

                            ParseNode parent = new ParseNode(rule.Parent, null, l, r);
                            modifiedInput.Insert(i, parent);

                            ParseRecurse(modifiedInput, output);
                        }
                    }
                }
            }

            if (input.Count == 1 && input[0].Type == Categories.Sentence)
            {
                output.Add(input);
            }
        }

        public static string NameForUnit(Categories u)
        {
            switch (u)
            {
                case Categories.Adjective:
                    return "Adj";
                case Categories.Determiner:
                    return "Det";
                case Categories.NBar:
                    return "N'";
                case Categories.Noun:
                    return "N";
                case Categories.NounPhrase:
                    return "NP";
                case Categories.Preposition:
                    return "P";
                case Categories.PrepositionPhrase:
                    return "PP";
                case Categories.Sentence:
                    return "S";
                case Categories.Verb:
                    return "V";
                case Categories.VerbPhrase:
                    return "VP";
            }
            return "-";
        }
    }
}
