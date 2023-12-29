using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TINY_COMPILER
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;
        public List<string> errors = new List<string>();
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            // Program → Fnctions MainFunction

            Node program = new Node("Program");
            program.Children.Add(Functions());
            program.Children.Add(MainFunction());
            MessageBox.Show("Success");
            return program;
        }
        Node Functions()
        {
            // Functions → FunctionStat Functions | e 

            Node functions = new Node("Functions");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String) && InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer+1].token_type == Token_Class.Identifier)
            {
                functions.Children.Add(FunctionStat());
                functions.Children.Add(Functions());
            }
            else return null;
            return functions;
        }
        Node FunctionStat()
        {
            // FunctionStat → FunctionDeclare FunctionBody 

            Node functionstat = new Node("FunctionStat");

            functionstat.Children.Add(FunctionDeclare());
            functionstat.Children.Add(FunctionBody());
            return functionstat;
        }
        Node FunctionDeclare()
        {
            // FunctionDeclare → DataType identifier ( Parameters ) 

            Node functiondeclare = new Node("FunctionDeclare");

            functiondeclare.Children.Add(DataType());
            functiondeclare.Children.Add(match(Token_Class.Identifier));
            functiondeclare.Children.Add(match(Token_Class.LeftBracket));
            functiondeclare.Children.Add(Parameters());
            functiondeclare.Children.Add(match(Token_Class.RightBracket));
            return functiondeclare;
        }
        Node DataType()
        {
            // DataType → int | float | string  

            Node datatype = new Node("DataType");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Int)
            {
                datatype.Children.Add(match(Token_Class.Int));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                datatype.Children.Add(match(Token_Class.Float));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String)
            {
                datatype.Children.Add(match(Token_Class.String));
            }
            else errors.Add("Parsing Error: Wrong DataType");


            return datatype;
        }
        Node Parameters()
        {
            // Parameters → Parameter ParametersD | e 

            Node parameters = new Node("Parameters");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String))
            {
                parameters.Children.Add(Parameter());
                parameters.Children.Add(ParametersD());
            }
            else return null;
            return parameters;
        }
        Node Parameter()
        {
            // Parameter → DataType identifier  

            Node parameter = new Node("Parameter");
            parameter.Children.Add(DataType());
            parameter.Children.Add(match(Token_Class.Identifier));
            return parameter;
        }
        Node ParametersD()
        {
            // ParametersD → , Parameter ParametersD | e 

            Node parametersd = new Node("ParametersD");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                parametersd.Children.Add(match(Token_Class.Comma));
                parametersd.Children.Add(Parameter());
                parametersd.Children.Add(ParametersD());
            }
            else return null;
            return parametersd;
        }
        Node FunctionBody()
        {
            // FunctionBody → { Statements ReturnStat } 

            Node functionbody = new Node("FunctionBody");
            functionbody.Children.Add(match(Token_Class.LeftCurlyBracket));
            functionbody.Children.Add(Statements());
            functionbody.Children.Add(ReturnStat());
            functionbody.Children.Add(match(Token_Class.RightCurlyBracket));
            return functionbody;
        }

        Node Statements()
        {
            // Statements → Statement Statements |e

            Node statements = new Node("Statements");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Write || TokenStream[InputPointer].token_type == Token_Class.Read || TokenStream[InputPointer].token_type == Token_Class.Repeat|| TokenStream[InputPointer].token_type == Token_Class.If|| TokenStream[InputPointer].token_type == Token_Class.Identifier|| TokenStream[InputPointer].token_type == Token_Class.Int|| TokenStream[InputPointer].token_type == Token_Class.Float|| TokenStream[InputPointer].token_type == Token_Class.String))
            {
                statements.Children.Add(Statement());
                statements.Children.Add(Statements());
            }
            else return null;
            return statements;
        }
        Node Statement()
        {
            // Statement → WriteStat ; | ReadStat ; | DeclarationStat ; | RepeatStat | IfStat | AssignmentStat ;

            Node statement = new Node("Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                statement.Children.Add(WriteStat());
                statement.Children.Add(match(Token_Class.Semicolon));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                statement.Children.Add(ReadStat());
                statement.Children.Add(match(Token_Class.Semicolon));
            }
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Int || InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Float || InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.String))
            {
                statement.Children.Add(DeclarationStat());
                statement.Children.Add(match(Token_Class.Semicolon));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                statement.Children.Add(RepeatStat());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.If)
            {
                statement.Children.Add(IfStat());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                statement.Children.Add(AssignmentStat());
                statement.Children.Add(match(Token_Class.Semicolon));
            }
            else errors.Add("Parsing Error: Wrong Statement");
            return statement;
        }
        Node WriteStat()
        {
            // WriteStat → write WriteStatD

            Node writestat = new Node("WriteStat");
            writestat.Children.Add(match(Token_Class.Write));
            writestat.Children.Add(WriteStatD());
            return writestat;
        }
        Node WriteStatD()
        {
            // WriteStatD → Expression | endl

            Node writestatd = new Node("WriteStatD");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                writestatd.Children.Add(match(Token_Class.Endl));
            }
            else if ((InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.StringVal)||((InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LeftBracket) || (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier || TokenStream[InputPointer].token_type == Token_Class.Constant) && InputPointer + 1 < TokenStream.Count && (TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp || TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp || TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp)) || (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier && InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.LeftBracket))|| (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier|| TokenStream[InputPointer].token_type == Token_Class.Constant)))
            {
                writestatd.Children.Add(Expression());
            }
            else errors.Add("Parsing Error: Wrong WriteStat2");

            return writestatd;
        }
        Node Expression()
        {
            // Expression → string | Term | Equation  

            Node expression = new Node("Expression");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.StringVal)
            {
                expression.Children.Add(match(Token_Class.StringVal));

            }
            else if ((InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LeftBracket)||(InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier|| TokenStream[InputPointer].token_type == Token_Class.Constant) && InputPointer+1 < TokenStream.Count && (TokenStream[InputPointer+1].token_type == Token_Class.PlusOp|| TokenStream[InputPointer+1].token_type == Token_Class.MinusOp || TokenStream[InputPointer+1].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp))||(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier&& InputPointer+1 < TokenStream.Count && TokenStream[InputPointer+1].token_type == Token_Class.LeftBracket))
            {
                expression.Children.Add(Equation());
            }
            else if((InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier || TokenStream[InputPointer].token_type == Token_Class.Constant)))
            {
                expression.Children.Add(Term());
            }
            else errors.Add("Parsing Error: Wrong Expression");


            return expression;
        }
        Node Term()
        {
            // Term → identifier | number | FunctionCall  

            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier && InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.LeftBracket)
            {
                term.Children.Add(FunctionCall());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Constant)
            {
                term.Children.Add(match(Token_Class.Constant));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                term.Children.Add(match(Token_Class.Identifier));
            }
            else errors.Add("Parsing Error: Wrong Term");

            return term;
        }
        Node FunctionCall()
        {
            // FunctionCall → identifier ( Arguments )  

            Node functioncall = new Node("FunctionCall");
            functioncall.Children.Add(match(Token_Class.Identifier));
            functioncall.Children.Add(match(Token_Class.LeftBracket));
            functioncall.Children.Add(Arguments());
            functioncall.Children.Add(match(Token_Class.RightBracket));
            return functioncall;
        }
        
        Node Arguments()
        {
            // Arguments → Expression Argument | e  

            Node arguments = new Node("Arguments");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier|| TokenStream[InputPointer].token_type == Token_Class.Constant))
            {
                arguments.Children.Add(Expression());
                arguments.Children.Add(Argument());
            }
            else return null;
            return arguments;
        }
        Node Argument()
        {
            // Argument → , Expression Argument | e  

            Node argument = new Node("Argument");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                argument.Children.Add(match(Token_Class.Comma));
                argument.Children.Add(Expression());
                argument.Children.Add(Argument());
            }
            else return null;
            return argument;
        }
        Node Equation()
        {
            // Equation → Factor ArithmeticOp Factor  EquationD 

            Node equation = new Node("Equation");
            equation.Children.Add(Factor());
            equation.Children.Add(ArithmeticOp());
            equation.Children.Add(Factor());
            equation.Children.Add(EquationD());
            return equation;
        }
        Node EquationD()
        {
            // EquationD → ArithmeticOp Factor EquationD |e

            Node equationd = new Node("EquationD");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.PlusOp || TokenStream[InputPointer].token_type == Token_Class.MinusOp || TokenStream[InputPointer].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.DivideOp))
            {
                equationd.Children.Add(ArithmeticOp());
                equationd.Children.Add(Factor());
                equationd.Children.Add(EquationD());
            }
            else return null;

            return equationd;
        }
        Node Factor()
        {
            // Factor → ( Equation ) | Term

            Node factor = new Node("Factor");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LeftBracket )
            {
                factor.Children.Add(match(Token_Class.LeftBracket));
                factor.Children.Add(Equation());
                factor.Children.Add(match(Token_Class.RightBracket));
                
            }
            else if ((InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Identifier || TokenStream[InputPointer].token_type == Token_Class.Constant)))
            {
                factor.Children.Add(Term());
            }
            else errors.Add("Parsing Error: Wrong Factor");

            return factor;
        }
        Node ArithmeticOp()
        {
            // ArithmeticOp → + | - | * | /

            Node arithmeticop = new Node("ArithmeticOp");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                arithmeticop.Children.Add(match(Token_Class.PlusOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                arithmeticop.Children.Add(match(Token_Class.MinusOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                arithmeticop.Children.Add(match(Token_Class.MultiplyOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                arithmeticop.Children.Add(match(Token_Class.DivideOp));
            }
            else errors.Add("Parsing Error: Wrong ArithmeticOp");

            return arithmeticop;
        }
        
        Node ReadStat()
        {
            // ReadStat → read Identifier  

            Node readstat = new Node("ReadStat");

            readstat.Children.Add(match(Token_Class.Read));
            readstat.Children.Add(match(Token_Class.Identifier));
            return readstat;
        }
        Node DeclarationStat()
        {
            // DeclarationStat → DataType VarDecls

            Node declarationstat = new Node("DeclarationStat");

            declarationstat.Children.Add(DataType());
            declarationstat.Children.Add(VarDecls());   
            return declarationstat;
        }
        Node VarDecls()
        {
            // VarDecls →  VarDecls2 VarDecl   

            Node vardecls = new Node("VarDecls");

            vardecls.Children.Add(VarDecls2());
            vardecls.Children.Add(VarDecl());

            return vardecls;
        }
        Node VarDecls2()
        {
            // VarDecls2 →  identifier | AssignmentStat

            Node vardecls2 = new Node("VarDecls2");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier&& InputPointer+1 < TokenStream.Count && TokenStream[InputPointer+1].token_type == Token_Class.AssignmentOP)
            {
                vardecls2.Children.Add(AssignmentStat());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Identifier)
            {
                vardecls2.Children.Add(match(Token_Class.Identifier));
            }
            else errors.Add("Parsing Error: Wrong VarDecls2");

            return vardecls2;
        }
        Node VarDecl()
        {
            // VarDecl → , VarDecls | e

            Node vardecl = new Node("VarDecl");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                vardecl.Children.Add(match(Token_Class.Comma));
                vardecl.Children.Add(VarDecls());
            }
            else return null;

            return vardecl;
        }
        Node AssignmentStat()
        {
            // AssignmentStat → identifier := Expression

            Node assignmentstat = new Node("AssignmentStat");

            assignmentstat.Children.Add(match(Token_Class.Identifier));
            assignmentstat.Children.Add(match(Token_Class.AssignmentOP));
            assignmentstat.Children.Add(Expression());

            return assignmentstat;
        }
        Node RepeatStat()
        {
            // RepeatStat → repeat Statements until ConditionStat

            Node repeatstat = new Node("RepeatStat");

            repeatstat.Children.Add(match(Token_Class.Repeat));
            repeatstat.Children.Add(Statements());
            repeatstat.Children.Add(match(Token_Class.Until));
            repeatstat.Children.Add(ConditionStat());

            return repeatstat;
        }
        
        Node ConditionStat()
        {
            // ConditionStat → Condition ConditionStatD

            Node conditionstat = new Node("ConditionStat");

            conditionstat.Children.Add(Condition());
            conditionstat.Children.Add(ConditionStatD());

            return conditionstat;
        }
        Node Condition()
        {
            // Condition → identifier ConditionOp Term

            Node condition = new Node("Condition");

            condition.Children.Add(match(Token_Class.Identifier));
            condition.Children.Add(ConditionOp());
            condition.Children.Add(Term());

            return condition;
        }
        Node ConditionOp()
        {
            // ConditionOp → < | > | = | <>

            Node conditionop = new Node("ConditionOp");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                conditionop.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                conditionop.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                conditionop.Children.Add(match(Token_Class.EqualOp));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                conditionop.Children.Add(match(Token_Class.NotEqualOp));
            }
            else errors.Add("Parsing Error: Wrong ConditionOp");

            return conditionop;
        }
        Node ConditionStatD()
        {
            // ConditionStatD → BoolenOp Condition ConditionStatD | e

            Node conditionstatd = new Node("ConditionStatD");

            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.AndOp || InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.OrOp))
            {
                conditionstatd.Children.Add(BoolenOp());
                conditionstatd.Children.Add(Condition());
                conditionstatd.Children.Add(ConditionStatD());
            }
            else return null;

            return conditionstatd;
        }
        Node BoolenOp()
        {
            // BoolenOp → && | ||

            Node boolenop = new Node("BoolenOp");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                boolenop.Children.Add(match(Token_Class.AndOp));
            }
            else
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.OrOp)
            {
                boolenop.Children.Add(match(Token_Class.OrOp));
            }
            else errors.Add("Parsing Error: Wrong BoolenOp");

            return boolenop;
        }
        Node IfStat()
        {
            // IfStat → if ConditionStat Then StatementsORreturn RestIf

            Node ifstat = new Node("IfStat");

            ifstat.Children.Add(match(Token_Class.If));
            ifstat.Children.Add(ConditionStat());
            ifstat.Children.Add(match(Token_Class.Then));
            ifstat.Children.Add(StatementsORreturn());
            ifstat.Children.Add(RestIf());

            return ifstat;
        }
        Node StatementsORreturn()
        {
            // StatementsORreturn → Statements | ReturnStat

            Node statementsorreturn = new Node("StatementsORreturn");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                statementsorreturn.Children.Add(ReturnStat());
            }
            else if (InputPointer < TokenStream.Count && (TokenStream[InputPointer].token_type == Token_Class.Write || TokenStream[InputPointer].token_type == Token_Class.Read || TokenStream[InputPointer].token_type == Token_Class.Repeat || TokenStream[InputPointer].token_type == Token_Class.If || TokenStream[InputPointer].token_type == Token_Class.Identifier || TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String))
            {
                statementsorreturn.Children.Add(Statements());
            }
            else errors.Add("Parsing Error: Wrong StatementsORreturn");


            return statementsorreturn;
        }
        Node ReturnStat()
        {
            // ReturnStat → return Expression ;

            Node returnstat = new Node("ReturnStat");

            returnstat.Children.Add(match(Token_Class.Return));
            returnstat.Children.Add(Expression());
            returnstat.Children.Add(match(Token_Class.Semicolon));

            return returnstat;
        }
        Node RestIf()
        {
            // RestIf → ElseIfStat | ElseStat | end

            Node restif = new Node("RestIf");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.ElseIf)
            {
                restif.Children.Add(ElseIfStat());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                restif.Children.Add(ElseStat());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.End)
            {
                restif.Children.Add(match(Token_Class.End));
            }
            else errors.Add("Parsing Error: Wrong RestIf");

            return restif;
        }
        Node ElseIfStat()
        {
            // ElseIfStat → elseif ConditionStat then StatementsORreturn RestIf

            Node elseifstat = new Node("ElseIfStat");

            elseifstat.Children.Add(match(Token_Class.ElseIf));
            elseifstat.Children.Add(ConditionStat());
            elseifstat.Children.Add(match(Token_Class.Then));
            elseifstat.Children.Add(StatementsORreturn());
            elseifstat.Children.Add(RestIf());

            return elseifstat;
        }
        Node ElseStat()
        {
            // ElseStat → else StatementsORreturn end

            Node elsestat = new Node("ElseStat");

            elsestat.Children.Add(match(Token_Class.Else));
            elsestat.Children.Add(StatementsORreturn());
            elsestat.Children.Add(match(Token_Class.End));

            return elsestat;
        }
        Node MainFunction()
        {
            // MainFunction → DataType main () FunctionBody

            Node mainfunction = new Node("MainFunction");

            
            mainfunction.Children.Add(DataType());
            mainfunction.Children.Add(match(Token_Class.Main));
            mainfunction.Children.Add(match(Token_Class.LeftBracket));
            mainfunction.Children.Add(match(Token_Class.RightBracket));
            mainfunction.Children.Add(FunctionBody());

            return mainfunction;
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    errors.Add("Parsing Error: Expected "
                        + ExpectedToken + " and " +
                        TokenStream[InputPointer].token_type +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                errors.Add("Parsing Error: Expected "
                        + ExpectedToken + "\r\n");
                InputPointer++;
                return null;
            }
        }
        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
