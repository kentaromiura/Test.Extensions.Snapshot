using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
    
namespace Snapshot
{
    public static class Extensions
    {
         public static string MethodToSentence(string str) 
        {
            return Regex.Replace(str, "[a-z][A-Z]", match => $"{match.Value[0]} {char.ToLower(match.Value[1])}");
        }
        public static bool ToMatchSnapshot(
            this object result,
            bool update = false,
            [CallerMemberName] string name = "",
            [CallerFilePath] string file = "",        
            [CallerLineNumber] int line = 0
        ) {
            var newLine = System.Environment.NewLine;
            var current = Directory.GetCurrentDirectory();
            var snapshotFolder = Path.Combine(current, "__snaphots__");
            var filename =  Path.Combine(snapshotFolder, string.Format("{0}.snap", Path.GetFileName(file)));
            var dictionary = new Dictionary<string, string>();
            var serializer = new JsonSerializer();
            var directoryExists = Directory.Exists(snapshotFolder);
            var fileExists = File.Exists(filename);
            
            var key = String.Format("{0} ({1})", MethodToSentence(name), line);
            if (directoryExists && fileExists)
            {
                var text = File.ReadAllText(filename);
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                
                if (dictionary.ContainsKey(key))
                {
                    var value = dictionary[key];
                    if (!update && value != result.ToString())
                    {
                        Console.Write("{0}Snapshot failed:{0}Diff:{0}", newLine);

                        var diff = new DiffMatchPatch.diff_match_patch();
                        var differences = diff.diff_main(value, result.ToString());
                        diff.diff_cleanupSemantic(differences);
                        foreach (var difference in differences)
                        {
                            switch(difference.operation)
                            {
                                case DiffMatchPatch.Operation.DELETE:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case DiffMatchPatch.Operation.EQUAL:
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    break;
                                case DiffMatchPatch.Operation.INSERT:
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    break;
                            }
                            Console.Write(difference.text);                        
                        }
                        Console.Write(newLine);
                        return false;
                    }
                }
            } else {
                Directory.CreateDirectory(snapshotFolder);
            }
            
            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);            
            }
            dictionary.Add(key, result.ToString());
            
            File.WriteAllText(filename, JsonConvert.SerializeObject(dictionary));        
            return true;
        }
    }
}
