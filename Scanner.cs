using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
   Constant, Identifier,
   Int, Float, String,Read, Write, Repeat, Until, If, ElseIf, Else, Then, Return, Endl, Main, End,
   LeftBracket, RightBracket, LeftCurlyBracket, RightCurlyBracket, PlusOp, MinusOp, MultiplyOp,
   DivideOp, EqualOp, LessThanOp, GreaterThanOp, NotEqualOp, AssignmentOP, AndOp, OrOp,
   Semicolon, Comma,
   StringVal,
   Comment
}
namespace TINY_COMPILER
{
    public class Token
    {
        public string lexema;
        public Token_Class token_type;
    }
    public  class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        public List<string> errors = new List<string>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        public Scanner() {
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float) ;
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("main", Token_Class.Main);
            ReservedWords.Add("end", Token_Class.End);

            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add(":=", Token_Class.AssignmentOP);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add(")", Token_Class.RightBracket);
            Operators.Add("(", Token_Class.LeftBracket);
            Operators.Add("}", Token_Class.RightCurlyBracket);
            Operators.Add("{", Token_Class.LeftCurlyBracket);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);

        }

        public void start_scanner(string code)
        {
            string lex = "";
            for(int i=0; i<code.Length; i++) {
                lex = "";
                if (code[i] == '\n' || code[i] == ' ' || code[i] == '\r' || code[i] == '\t') continue;
                if ( ( code[i] >='A' && code[i] <= 'Z' ) || (code[i] >= 'a' && code[i] <= 'z') )
                {
                    for( ; i < code.Length; i++)
                    {
                        if (!((code[i] >= 'A' && code[i] <= 'Z') || (code[i] >= 'a' && code[i] <= 'z') || (code[i] >= '0' && code[i] <= '9') ) )
                        {
                            i--;
                            break;
                        }
                        lex += code[i];
                    }
                    FindToken(lex);
                }
                else if((code[i] >= '0' && code[i] <= '9' )|| code[i] == '.')
                {
                    for (; i < code.Length; i++)
                    {
                        if ( ! ( (code[i] >= 'A' && code[i] <= 'Z') || (code[i] >= 'a' && code[i] <= 'z') || (code[i] >= '0' && code[i] <= '9') || code[i]=='.') )
                        {
                            i--;
                            break;
                        }
                        lex += code[i];
                    }
                    FindToken(lex);
                }
                else if (code[i] == '"')
                {
                    lex += code[i];
                    i++;
                    bool StringClosed = false;
                    for (; i < code.Length; i++)
                    {
                        if (code[i] == '\n')
                        {
                            break;
                        }
                        lex += code[i];
                        if (code[i] == '"')
                        {
                            StringClosed = true;
                            break;
                        }
                    }
                    if (StringClosed)
                    {
                        Token Tok = new Token();
                        Tok.lexema = lex;
                        Tok.token_type = Token_Class.StringVal;
                        Tokens.Add(Tok);
                        lex = "";
                    }
                    else
                    {
                        errors.Add("Unrecognized token: " + lex);
                        lex = "";
                    }
                }
                else if ( i+1 < code.Length && code[i] == '/' && code[i + 1] == '*')
                {
                    lex = "/*";
                    i +=2;
                    bool CommentClosed = false;
                    for (; i < code.Length; i++)
                    {
                        lex += code[i];
                        if (i+1 < code.Length && code[i] == '*' && code[i+1] == '/')
                        {
                            CommentClosed = true;
                            i++;
                            lex += code[i];
                            break;
                        }
                    }
                    if (CommentClosed)
                    {
                        Token Tok = new Token();
                        Tok.lexema = lex;
                        Tok.token_type = Token_Class.Comment;
                        //Tokens.Add(Tok);
                        lex = "";
                    }
                    else
                    {
                        errors.Add("Unrecognized token: " + lex);
                        lex = "";
                    }
                }
                else
                {
                    lex += code[i];
                    if (i + 1 < code.Length)
                    {
                        if (code[i] == ':' && code[i + 1] == '=')
                        {
                            i++;
                            lex +=code[i];
                        }
                        else if (code[i] == '<' && code[i + 1] == '>')
                        {
                            i++;
                            lex += code[i];
                        }
                        else if (code[i] == '&' && code[i + 1] == '&')
                        {
                            i++;
                            lex += code[i];
                        }
                        else if (code[i] == '|' && code[i + 1] == '|')
                        {
                            i++;
                            lex += code[i];
                        }
                    }
                    FindToken(lex);
                }
            
            }
            Tiny_compiler.TokenStream = Tokens;
        }
        public bool IsIdentifier(string lex)
        {
            var s = new Regex("^[a-zA-Z]([a-zA-Z0-9])*$");
            if (!s.IsMatch(lex))
            {
                return false;
            }
            return true;
        }

        public bool IsConstant(string lex)
        {
            var s = new Regex("^[0-9]+(.[0-9]+)?$");
            if (!s.IsMatch(lex))
            {
                return false;
            }
            return true;
        }
        public void FindToken (string lex)
        {
            Token Tok = new Token();
            Tok.lexema = lex;
            if (ReservedWords.ContainsKey(lex))
            {
                Tok.token_type = ReservedWords[lex];
                Tokens.Add(Tok);

            }
            else if(Operators.ContainsKey(lex))
            {
                Tok.token_type = Operators[lex];
                Tokens.Add(Tok);
            }
            else if (IsIdentifier(lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);
            }
            else if (IsConstant(lex))
            {
                Tok.token_type = Token_Class.Constant;
                Tokens.Add(Tok);
            }
            else
            {
                errors.Add("Unrecognized token: " + lex);
            }
        }
    }
}
