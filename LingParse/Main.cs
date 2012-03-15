using System;
using System.Collections.Generic;

namespace LingParse
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string s = "The airliner collapsed on a frightened, grassy puppy.";

            Console.WriteLine("Operating with these phrase-structure rules:");
            foreach (GrammarDefinition def in Grammar.DefaultGrammar().Rules)
                Console.WriteLine(def);

            s = s.ToUpper();
            char[] cArr = s.ToCharArray();
            cArr = Array.FindAll<char>(cArr, (c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')));
            s = new string(cArr);

            List<List<ParseNode>> possibleSentences = Lexicon.DefaultLexicon().Chunk(s);
            List<ParseNode> chosenSentence = possibleSentences[0];

            Console.WriteLine("\nSentence parsed as:");
            foreach (ParseNode n in chosenSentence)
                Console.WriteLine(n.ToString());

            List<List<ParseNode>> parseResults = Grammar.DefaultGrammar().Parse(chosenSentence);

            Console.WriteLine("\nThe results were:");
            int count = 0;
            foreach (List<ParseNode> result in parseResults)
            {
                Console.WriteLine("--{0}--", ++count);
                Console.WriteLine(result[0]);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
		}
	}
}
