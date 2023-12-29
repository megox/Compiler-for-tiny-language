using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TINY_COMPILER
{
    public static class Tiny_compiler
    {
        public static Scanner Tiny_Scanner = new Scanner();
        public static Parser Tiny_Parser = new Parser();
        public static List<Token> TokenStream = new List<Token>();
        public static Node treeroot;
        public static void Start_Compiling(string SourceCode)
        {
            Tiny_Scanner.start_scanner(SourceCode);
            Tiny_Parser.StartParsing(TokenStream);
            treeroot = Tiny_Parser.root;
        }
    }
}
