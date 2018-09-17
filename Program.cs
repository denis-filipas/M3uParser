using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using M3uParser.DatabaseModel;
using M3uParser.Modes;

namespace M3uParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = "-parse -path /Users/mask/Downloads/playlists/parse/".Split(' ');
            //args = "-check".Split(' ');
            args = "-export -path /Users/mask/Downloads/playlists/out_playlist.m3u".Split(' ');

            if (args == null || args.Length == 0)
                return;

            // get app mode
            AppMode mode = null;
            if (args.Contains("-parse"))
            {
                mode = new ParserMode(args);
            }
            else if(args.Contains("-groups"))
            {
                mode = new GroupsMode(args);
            }
            else if(args.Contains("-check"))
            {
                mode = new CheckMode(args);
            }
            else if(args.Contains("-export"))
            {
                mode = new ExportMode(args);
            }

            mode?.Run();
        }
    }
}
