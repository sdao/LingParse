using System;
using System.Collections.Generic;
using System.IO;

namespace LingParse
{
	public class SyntaxCategories
	{
		/**
		 * File format:
		 * 0 - #Empty placeholder
		 * 1 S #Symbol for complete sentence or phrase
		 * 2 <symbol> : <tag*>
		 * 3 <symbol> : <tag*>
		 * ...
		 * 
		 * <symbol>: any Unicode string
		 * <tag*>: one or more characters defined as category synonyms
		 * 
		 * **/
		
		public const int SYNTAX_CATEGORY_NOT_IMPLEMENTED = -1;
		public const int SYNTAX_CATEGORY_EMPTY = 0;
		public const int SYNTAX_CATEGORY_FULL = 1;
		
		
		public static void LoadSyntaxCategories(string filename)
		{
			DefaultSyntaxCategories = new SyntaxCategories();
			List<string> defaultList = DefaultSyntaxCategories.Categories;
			Dictionary<string, int> defaultSyn = DefaultSyntaxCategories.Synonyms;
			
			using (StreamReader sr = new StreamReader(filename))
			{
				String line;
				while ((line = sr.ReadLine()) != null) {
					if (line.Length > 0) {
						string[] components = line.Split(' ');
						defaultList.Add(components[0]);
						if (components.Length > 2) {
							for (int i = 2; i < components.Length; i++) {
								defaultSyn.Add(components[i], defaultList.Count - 1);
							}
						}
					}
				}
			}
		}
		
		public static SyntaxCategories DefaultSyntaxCategories {
			get;
			set;
		}
		
		private SyntaxCategories() {
			Categories = new List<string>();
			Synonyms = new Dictionary<string, int>();
		}
		
		private List<string> Categories {
			get;
			set;
		}
		
		private Dictionary<string, int> Synonyms {
			get;
			set;
		}
		
		public string NameForIndex(int index) {
			return Categories[index];
		}
		
		public int IndexForName(string name) {
			int x = -1;
			if (Synonyms.TryGetValue(name, out x))
				return x;
			else
				return - 1;
		}
	}
}

