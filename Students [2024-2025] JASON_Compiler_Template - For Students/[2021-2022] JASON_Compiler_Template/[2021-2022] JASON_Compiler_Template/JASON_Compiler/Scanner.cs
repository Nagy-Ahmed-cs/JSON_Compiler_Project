using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant,Comment
}
namespace JASON_Compiler
{
    

    public class Token
    {
       public string lex;
       public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public object Symbols { get; private set; }

        public Scanner()
        {
            ReservedWords.Add("IF", Token_Class.If);
            ReservedWords.Add("BEGIN", Token_Class.Begin);
            ReservedWords.Add("CALL", Token_Class.Call);
            ReservedWords.Add("DECLARE", Token_Class.Declare);
            ReservedWords.Add("END", Token_Class.End);
            ReservedWords.Add("DO", Token_Class.Do);
            ReservedWords.Add("ELSE", Token_Class.Else);
            ReservedWords.Add("ENDIF", Token_Class.EndIf);
            ReservedWords.Add("ENDUNTIL", Token_Class.EndUntil);
            ReservedWords.Add("ENDWHILE", Token_Class.EndWhile);
            ReservedWords.Add("INTEGER", Token_Class.Integer);
            ReservedWords.Add("PARAMETERS", Token_Class.Parameters);
            ReservedWords.Add("PROCEDURE", Token_Class.Procedure);
            ReservedWords.Add("PROGRAM", Token_Class.Program);
            ReservedWords.Add("READ", Token_Class.Read);
            ReservedWords.Add("REAL", Token_Class.Real);
            ReservedWords.Add("SET", Token_Class.Set);
            ReservedWords.Add("THEN", Token_Class.Then);
            ReservedWords.Add("UNTIL", Token_Class.Until);
            ReservedWords.Add("WHILE", Token_Class.While);
            ReservedWords.Add("WRITE", Token_Class.Write);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("!", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);



        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();
                int next = i + 1;

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;
                //  identifer of reservered word  هنا بياخد الكلمة و يحدد نوعها سواء كانت 
                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    // this  CurrentChar >= 'A' && CurrentChar <= 'z'== char.Isletter(CurrentChar);
                    while (next < SourceCode.Length && char.IsLetterOrDigit(SourceCode[next]))
                    {
                        CurrentLexeme += SourceCode[next];
                        next++;
                    }

                }
                // Integers or Doubles هنا بيقرأ الارقام سواء كانت  
                else if (char.IsDigit(CurrentChar))
                {
                    bool hasDot = false;

                    while (next < SourceCode.Length &&
                          (char.IsDigit(SourceCode[next]) || (SourceCode[next] == '.' && !hasDot)))
                    {
                        if (SourceCode[next] == '.')
                            hasDot = true;

                        CurrentLexeme += SourceCode[next];
                        next++;
                    }


                    if (CurrentLexeme.EndsWith("."))
                    {

                        throw new Exception($"Invalid number format: {CurrentLexeme}");
                    }
                }
                // commnet handeler .....
                else if (CurrentChar == '/')
                {
                    if (next < SourceCode.Length && SourceCode[next] == '*')
                    {
                        CurrentLexeme += SourceCode[next];
                        next++;
                        while (next < SourceCode.Length - 1 && (SourceCode[next] != '*' && SourceCode[next + 1] != '/'))
                        {
                            CurrentLexeme += SourceCode[next];
                            next++;
                        }
                        if (next < SourceCode.Length - 1)
                        {
                            CurrentLexeme += "*/";
                            next += 2;
                        }
                    }

                }
                // String Reader .....
                else if (CurrentChar == '"')
                {
                    while (next < SourceCode.Length && SourceCode[next] != '"')
                    {
                        CurrentLexeme += SourceCode[next];
                        next++;
                    }
                    if (next < SourceCode.Length)
                    {
                        CurrentLexeme += '"';
                        next++;
                    }
                }
                // Opearators Hnadelrs .....
                else
                {
                    if (next < SourceCode.Length)
                    {
                        Char SecondChar = SourceCode[next];
                        if ((CurrentChar == '&' && SecondChar == '&') ||
                            (CurrentChar == '|' && SecondChar == '|') ||
                            (CurrentChar == '<' && SecondChar == '>') ||//  Condition wa7ed  كل دا
                            (CurrentChar == ':' && SecondChar == '='))
                        {
                            CurrentLexeme += SecondChar;
                            next++;
                        }
                        if (Operators.ContainsKey(CurrentLexeme))
                        {

                        }

                    }
                }

            }
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (Lex == null) // lexeme is empty here .....
            {
                return;
            }

            //Is it an identifier?   // token is a lexeme and token type .. 

             
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it a Constant?
            else if (isConstant(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.Constant; // this our enum which is decalared above 
                Tokens.Add(Tok);
            }

            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is a Symbols ...
            //else if (Symbols.ContainsKey(Lex))
            //{
            //    Tok.lex = Lex;
            //    Tok.token_type = Symbols[Lex];
            //    Tokens.Add(Tok);
            //}
            //Is acomment ? 
            else if (IsComment(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }

            //Is it an undefined?
        }
        bool IsNumber(char letter)
        {
            return letter >= '0' || letter <= '9';
        }
        bool isIdentifier(string lex)
        {
            bool isValid=true;
            // Check if the lex is an identifier or not.
            Regex reg = new Regex(@"^([a-zA-Z])([0-9a-zA-Z])*$", RegexOptions.Compiled);
            isValid =  reg.IsMatch(lex);
            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.

            return isValid;
        }
        bool IsComment(String Lex)
        {
            return (Lex.Length >= 4 && Lex.StartsWith("/*") && Lex.EndsWith("*/"));
        }
        bool IsString(String Lex)
        {
            return Lex[0] == '"' && Lex[Lex.Length - 1] == '"';
        }
    }
}
