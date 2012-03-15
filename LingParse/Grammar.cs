using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LingParse
{

    class Grammar
    {	
		/**
		 * File format:
		 * 0 <parent> -> <left> <right>
		 * 1 <parent> -> <left> <right>
		 * ...
		 * 
		 * <parent>: a string in the loaded SyntaxCategories file
		 * <left>: a string in the loaded SyntaxCategories file
		 * <right>: a string in the loaded SyntaxCategories file
		 * 
		 * **/
		public Grammar(string filename) {
			Rules = new List<GrammarDefinition>();
			
			using (StreamReader sr = new StreamReader(filename))
			{
				String line;
				while ((line = sr.ReadLine()) != null) {
					if (line.Length > 0)
					{
						string[] components = line.Split(' ');
						
						if (components.Length == 4)
						{
							int parent = SyntaxCategories.DefaultSyntaxCategories.IndexForName(components[0]);
							int left = SyntaxCategories.DefaultSyntaxCategories.IndexForName(components[2]);
							int right = SyntaxCategories.DefaultSyntaxCategories.IndexForName(components[3]);
							
							if (parent != -1 && left != -1 && right != -1)
								Rules.Add (new GrammarDefinition(parent, left, right));
							else
								throw new FormatException(String.Format ("Category name is not valid with current SyntaxCategories. Line: {0}", line));
						}
						else
							throw new FormatException("Grammar entry takes only three parameters.");
					
					}
				}
			}
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
                    if (rule.Right == SyntaxCategories.SYNTAX_CATEGORY_EMPTY)
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

                        if (modifiedInput.Count == 1 && modifiedInput[0].Type == SyntaxCategories.SYNTAX_CATEGORY_FULL)
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
                    if (rule.Right == SyntaxCategories.SYNTAX_CATEGORY_EMPTY)
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

            if (input.Count == 1 && input[0].Type == SyntaxCategories.SYNTAX_CATEGORY_FULL)
            {
                output.Add(input);
            }
        }
    }
}
