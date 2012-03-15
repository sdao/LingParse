using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LingParse
{
    class Lexicon
    {
        static Lexicon _default;

        public static Lexicon DefaultLexicon()
        {
            if (_default == null)
            {
                _default = new Lexicon();

                _default.Add("a", Categories.Determiner);
                _default.Add("the", Categories.Determiner);

                _default.Add("airliner", Categories.Noun);
                _default.Add("box", Categories.Noun);
                _default.Add("cams", Categories.Noun);
                _default.Add("car", Categories.Noun);
                _default.Add("child", Categories.Noun);
                _default.Add("children", Categories.Noun);
                _default.Add("hill", Categories.Noun);
                _default.Add("house", Categories.Noun);
                _default.Add("ice", Categories.Noun);
                _default.Add("lane", Categories.Noun);
                _default.Add("passenger", Categories.Noun);
                _default.Add("puppy", Categories.Noun);
                _default.Add("sun", Categories.Noun);
                _default.Add("toy", Categories.Noun);
                _default.Add("tree", Categories.Noun);
                _default.Add("wind", Categories.Noun);

                _default.Add("collapsed", Categories.Verb);
                _default.Add("found", Categories.Verb);
                _default.Add("landed", Categories.Verb);
                _default.Add("melted", Categories.Verb);
                _default.Add("put", Categories.Verb);
                _default.Add("sped", Categories.Verb);
                _default.Add("swayed", Categories.Verb);

                _default.Add("crippled", Categories.Adjective);
                _default.Add("fast", Categories.Adjective);
                _default.Add("frightened", Categories.Adjective);
                _default.Add("grassy", Categories.Adjective);
                _default.Add("hot", Categories.Adjective);
                _default.Add("old", Categories.Adjective);
                _default.Add("small", Categories.Adjective);
                _default.Add("twin", Categories.Adjective);

                _default.Add("by", Categories.Preposition);
                _default.Add("in", Categories.Preposition);
                _default.Add("on", Categories.Preposition);
                _default.Add("with", Categories.Preposition);
            }
            return _default;
        }

        private Lexicon() {
            Library = new Dictionary<string, HashSet<Categories>>();
        }

        public Dictionary<string, HashSet<Categories>> Library
        {
            get;
            private set;
        }

        public bool Add(string t, Categories category)
        {
            string term = t.ToUpper();

            HashSet<Categories> categorySet;
            if (!Library.ContainsKey(term))
            {
                categorySet = new HashSet<Categories>();
                Library.Add(term, categorySet);
            }
            else
            {
                categorySet = Library[term];
            }

            return categorySet.Add(category);
        }

        public bool Remove(string t, Categories category)
        {
            string term = t.ToUpper();

            if (!Library.ContainsKey(term))
            {
                return false;
            }
            else
            {
                HashSet<Categories> categorySet = Library[term];
                bool removed = categorySet.Remove(category);
                if (categorySet.Count == 0)
                {
                    Library.Remove(term);
                    return true;
                }
                else
                {
                    return removed;
                }
            }
        }

        public bool Remove(string t)
        {
            string term = t.ToUpper();

            if (!Library.ContainsKey(term))
            {
                return false;
            }
            else
            {
                Library.Remove(term);
                return true;
            }
        }

        public bool Contains(string t)
        {
            string term = t.ToUpper();

            return Library.ContainsKey(term);
        }

        public HashSet<Categories> GetTypes(string t)
        {
            string term = t.ToUpper();

            return Library[term];
        }

        public List<List<ParseNode>> Chunk(string input)
        {
            string[] words = input.Split(' ');
            List<List<ParseNode>> output = new List<List<ParseNode>>();

            Chunk(words, new List<ParseNode>(), output);

            return output;
        }

        private void Chunk(string[] words, List<ParseNode> chain, List<List<ParseNode>> output)
        {
            if (chain.Count >= words.Length)
            {
                output.Add(new List<ParseNode>(chain));
            }
            else
            {
                string word = words[chain.Count];
                HashSet<Categories> availableTypes = GetTypes(word);

                foreach (Categories type in availableTypes) {
                    chain.Add(new ParseNode(type, word));
                    Chunk(words, chain, output);
                    chain.RemoveAt(chain.Count - 1);
                }
            }
        }
    }
}
