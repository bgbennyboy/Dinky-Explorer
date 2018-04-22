using System;
using System.Collections.Generic;
using System.IO;

namespace ThimbleweedLibrary
{
    public class Decoders
    {
        public static bool ExtractText(Stream SourceStream, out String[] DestStrings)
        {
            var list = new List<string>();
            using (var sr = new StreamReader(SourceStream))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }

            DestStrings = list.ToArray();
            return true;
        }
    }
}