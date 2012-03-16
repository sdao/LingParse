//#define ERROR_ON_BAD_CATEGORY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LingParse
{
    class Lexicon
    {
		/**
		 * Dictionary file format:
		 * 0 <word>	(tab)	<tag*>
		 * 1 <word>	(tab)	<tag*>
		 * ...
		 * 
		 * <word>: any alphanumeric Unicode string
		 * <tag*>: one or more characters defined as category synonyms
		 * 
		 * Morphology file format:
		 * 0 -<suffix>	(tab)	<tag*>
		 * 1 <prefix>-	(tab)	<tag*>
		 * ...
		 * 
		 * <suffix>|<prefix>: any alphanumeric Unicode string
		 * <tag*>: one or more characters defined as category synonyms
		 * 
		 * **/
		public Lexicon(string dictFilename, string morphFilename) {
			Library = new Dictionary<string, HashSet<int>>();
			MorphologyPrefixes = new Dictionary<string, HashSet<int>>();
			MorphologySuffixes = new Dictionary<string, HashSet<int>>();
			
			using (StreamReader sr = new StreamReader(dictFilename))
			{
				String line;
				while ((line = sr.ReadLine()) != null) {
					if (line.Length > 0)
					{
						string[] components = line.Split('\t');
						
						if (components.Length == 2)
						{
							for (int i = 0; i < components[1].Length; i++) {
								string x = components[1].Substring(i, 1);
								int category = SyntaxCategories.DefaultSyntaxCategories.IndexForName(x);
								
								if (category > SyntaxCategories.SYNTAX_CATEGORY_NOT_IMPLEMENTED)
									Add (components[0], category);
								
#if ERROR_ON_BAD_CATEGORY
									throw new FormatException(String.Format ("Category name is not valid with current SyntaxCategories. Line: {0}; Name: {1}", line, x));
#endif
							}
						}
						else
							throw new FormatException(String.Format("Word entry takes only two parameters. Line: {0}", line));
					}
				}
			}
			
			using (StreamReader sr = new StreamReader(morphFilename))
			{
				String line;
				while ((line = sr.ReadLine()) != null) {
					if (line.Length > 0)
					{
						string[] components = line.Split('\t');
						
						if (components.Length == 2)
						{
							string name = components[0];
							bool isPrefix = (name.Substring (name.Length - 1) == "-");
							if (isPrefix) {
								name = name.Substring (0, name.Length - 1);
							} else {
								name = name.Substring (1);
							}
							
							for (int i = 0; i < components[1].Length; i++) {
								string x = components[1].Substring(i, 1);
								int category = SyntaxCategories.DefaultSyntaxCategories.IndexForName(x);
								
								if (category >= SyntaxCategories.SYNTAX_CATEGORY_NOT_IMPLEMENTED) {
									if (isPrefix) {
										AddPrefix (name, category);
									}
									else {
										AddSuffix (name, category);
									}
								}
								else
									throw new FormatException(String.Format ("Category name is not valid with current SyntaxCategories. Line: {0}; Name: {1}", line, x));
							}
						}
						else
							throw new FormatException(String.Format("Word entry takes only two parameters. Line: {0}", line));
					}
				}
			}
		}
		
        public Dictionary<string, HashSet<int>> Library
        {
            get;
            private set;
        }
		
		public Dictionary<string, HashSet<int>> MorphologyPrefixes
		{
			get;
			private set;
		}
		
		public Dictionary<string, HashSet<int>> MorphologySuffixes
		{
			get;
			private set;
		}

        public bool Add(string t, int category)
        {
            string term = t.ToUpper();

            HashSet<int> categorySet;
            if (!Library.ContainsKey(term))
            {
                categorySet = new HashSet<int>();
                Library.Add(term, categorySet);
            }
            else
            {
                categorySet = Library[term];
            }

            return categorySet.Add(category);
        }
		
		public bool AddPrefix(string t, int category)
        {
            string term = t.ToUpper();

            HashSet<int> categorySet;
            if (!MorphologyPrefixes.ContainsKey(term))
            {
                categorySet = new HashSet<int>();
                MorphologyPrefixes.Add(term, categorySet);
            }
            else
            {
                categorySet = MorphologyPrefixes[term];
            }

            return categorySet.Add(category);
        }
		
		public bool AddSuffix(string t, int category)
        {
            string term = t.ToUpper();

            HashSet<int> categorySet;
            if (!MorphologySuffixes.ContainsKey(term))
            {
                categorySet = new HashSet<int>();
                MorphologySuffixes.Add(term, categorySet);
            }
            else
            {
                categorySet = MorphologySuffixes[term];
            }

            return categorySet.Add(category);
        }

        public bool Remove(string t, int category)
        {
            string term = t.ToUpper();

            if (!Library.ContainsKey(term))
            {
                return false;
            }
            else
            {
                HashSet<int> categorySet = Library[term];
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

        public bool Contains(string t, out HashSet<int> types)
        {
            string term = t.ToUpper();
			types = new HashSet<int>();
			
			if (Contains(term)) {
				types.UnionWith(GetTypes(term));
			}
			
            foreach (string prefix in MorphologyPrefixes.Keys) {
				if (term.StartsWith (prefix) && term.Length > prefix.Length) {
					string stem = term.Substring (prefix.Length);
					
					if (Contains(stem)) {
						types.UnionWith(MorphologyPrefixes[prefix].Intersect(Library[stem]));
					}
				}
			}
			
			foreach (string suffix in MorphologySuffixes.Keys) {
				if (term.EndsWith(suffix) && term.Length > suffix.Length) {
					string stem = term.Substring(0, term.Length - suffix.Length);
					
					if (Contains(stem)) {
						types.UnionWith(MorphologySuffixes[suffix].Intersect(Library[stem]));
					}
				}
			}
			
			return types.Count > 0;
        }
		
		public bool Contains(string t) {
			return Library.ContainsKey(t);
		}
		
        public HashSet<int> GetTypes(string t)
        {
            string term = t.ToUpper();

            return Library[term];
        }

        public List<List<ParseNode>> Chunk(string input, bool sanitize)
        {
			if (sanitize) {
				input = input.ToUpper();
            	char[] cArr = input.ToCharArray();
            	cArr = Array.FindAll<char>(cArr, (c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')));
            	input = new string(cArr);
			}
			
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
				HashSet<int> types = null;
				bool contains = Contains(word, out types);
				
				if (!contains)
					throw new Exception(String.Format("Encountered word not in lexicon: {0}", word));

                foreach (int type in types) {
                    chain.Add(new ParseNode(type, word));
                    Chunk(words, chain, output);
                    chain.RemoveAt(chain.Count - 1);
                }
            }
        }
    }
}
