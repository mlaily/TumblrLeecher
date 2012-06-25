using System;
using System.Collections.Generic;

class Script
{
	static public void Main(string[] args)
	{
		string[] EXCLUDE = new string[] { "TotalLineCountScript.cs" };
		Console.WriteLine("\nThis script will show the total number of lines in *.cs files...\nPress Enter to continue...");
		int totalCount = 0;
		//todo: handle multiline comments
		int commentsCount = 0;
		int whiteSpacesCount = 0;
		foreach (var item in System.IO.Directory.GetFiles(Environment.CurrentDirectory, "*", System.IO.SearchOption.AllDirectories))
		{
			bool excluded = false;
			foreach (var exclusion in EXCLUDE)
			{
				if (exclusion == System.IO.Path.GetFileName(item))
				{
					Console.WriteLine("File \"{0}\" excluded.", item);
					excluded = true;
					break;
				}
			}
			if (excluded) continue;
			if (System.IO.Path.GetExtension(item).ToLowerInvariant() == ".cs")
			{
				try
				{
					Console.WriteLine("\nOpening file \"{0}\"", item);
					string[] allLines = System.IO.File.ReadAllLines(item);
					totalCount += allLines.Length;
					foreach (var line in allLines)
					{
						string trimmed = line.Trim(" \n\r\t".ToCharArray());
						if (trimmed.StartsWith("//")) commentsCount++;
						if (string.IsNullOrWhiteSpace(trimmed)) whiteSpacesCount++;
					}
					Console.WriteLine("\t{0} Line(s) in this file.", allLines.Length);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error: {0}", ex.ToString());
				}
			}
		}
		Console.WriteLine("\nResults: ");
		Console.WriteLine("\tTotal: " + totalCount.ToString());
		Console.WriteLine("\tComments: " + commentsCount.ToString());
		Console.WriteLine("\tBlank Lines: " + whiteSpacesCount.ToString());
		Console.WriteLine("\tFinal Result: " + (totalCount - commentsCount - whiteSpacesCount).ToString());
		Console.WriteLine("\nDone !");
		Console.ReadLine();
	}
}