using System;
using System.Collections.Generic;

namespace LingParse
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args[0] == "/?" || args[0] == "/help") {
				Console.WriteLine("usage: LingParse.exe syn-file lex-file lem-file gra-file input");
				return;
			}
			
			SyntaxCategories.LoadSyntaxCategories(args[0]);
			Lexicon lex = new Lexicon(args[1], args[2]);
			Grammar gra = new Grammar(args[3]);
			
			string s;
			if (args.Length > 4)
				s = args[4];
			else {
				Console.Write("Input sentence> ");
				s = Console.ReadLine();
			}

            List<List<ParseNode>> possibleSentences = lex.Chunk(s, true);

			int count = 0;
			foreach (List<ParseNode> sentence in possibleSentences)
			{
				List<List<ParseNode>> parseResults = gra.Parse(sentence);

            	foreach (List<ParseNode> result in parseResults)
            	{
                	Console.WriteLine("--{0}--", ++count);
                	Console.WriteLine(result[0]);
					Console.WriteLine();
            	}
			}
			if (count == 0)
				Console.WriteLine("--NO OUTPUT--");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
		}
	}
}
