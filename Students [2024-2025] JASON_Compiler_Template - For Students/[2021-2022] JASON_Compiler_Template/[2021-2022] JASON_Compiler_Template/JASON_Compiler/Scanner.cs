using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,Elseif , Return, Endl,
    Idenifier, Constant,Comment, StringLiteral,Int,Float,Main,Number,CurlyBraket,String,Repeat 
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
        Dictionary<String, Token_Class> Symbols = new Dictionary<string, Token_Class>();
        //public object Symbols { get; private set; } no need for this because it alreay declared ...

        public Scanner()
        {
            ReservedWords.Add("Int", Token_Class.Int);
            ReservedWords.Add("Float", Token_Class.Float);
            ReservedWords.Add("String", Token_Class.String);
            ReservedWords.Add("Read", Token_Class.Read);
            ReservedWords.Add("Write", Token_Class.Write);
            ReservedWords.Add("Repeat", Token_Class.Repeat);
            ReservedWords.Add("Until", Token_Class.Until);
            ReservedWords.Add("If", Token_Class.If);
            ReservedWords.Add("Elseif", Token_Class.Elseif);
            ReservedWords.Add("Else", Token_Class.Else);
            ReservedWords.Add("Then", Token_Class.Then);
            ReservedWords.Add("Return", Token_Class.Return);
            ReservedWords.Add("Endl", Token_Class.Endl);
            ReservedWords.Add("End", Token_Class.End);
            ReservedWords.Add("Main", Token_Class.Main);
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
            Tokens.Clear();
            Errors.Error_List.Clear();
            int Last_index = -1; // used for opearators Lexemes ....
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();
                int next = i + 1;

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')//If true move the next word to identify it 
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
                else if (char.IsDigit(CurrentChar) || Symbols.ContainsKey(CurrentLexeme))
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
                // String Reader (handler)  .....
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
                        {// && || <> :=
                            CurrentLexeme += SecondChar;
                            next++;
                        }
                        if (Operators.ContainsKey(CurrentLexeme)|| Symbols.ContainsKey(CurrentLexeme))
                        {

                            FindTokenClass(CurrentLexeme);
                            i = next - 1;// 3ashan el condition yetal3 false fe el loop de we yod5l 3ala el word el tanyia  
                            Last_index = next;
                            continue;
                        }

                    }
                    FindTokenClass(CurrentLexeme);
                    i = next - 1;// 3ashan el condition yetal3 false fe el loop de we yod5l 3ala el word el tanyia  
                    Last_index = next;

                }
                if (Last_index == SourceCode.Length)
                   { 
                    FindTokenClass(SourceCode[Last_index - 1].ToString());
                    JASON_Compiler.TokenStream = Tokens; // recheck this area again ... 
                }
            }
            
            
        }
        void FindTokenClass(string Lex)
        {
            if (Lex == null) // lexeme is empty here .....
            {
                return;
            }

            Token_Class TC; // not used until now 
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it an undefined?

                  // token is a lexeme and token type .. 
             
            //Is a Reserved Word
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }

            //Is it a Constant?
            //else if (isConstant(Lex))
            //{
            //    Tok.lex = Lex;
            //    Tok.token_type = Token_Class.Constant; // this our enum which is decalared above 
            //    Tokens.Add(Tok);
            //}

            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is a Symbols ...
            else if (Symbols.ContainsKey(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Symbols[Lex];
                Tokens.Add(Tok);
            }
            //Is acomment ? 
            else if (IsComment(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }
            else if (isStringLiteral(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.StringLiteral;
            }
            
            else
            {
                Errors.Error_List.Add("Invalid Token: " + Lex);
            }

            
        }

        private bool IsStringLiteral(string lex)
        {
            throw new NotImplementedException();
        }


        bool isIdentifier(string lex)
        {
            bool isValid=true;
            // Check if the lex is an identifier or not.
            Regex reg = new Regex(@"^([a-zA-Z])([0-9a-zA-Z])*$", RegexOptions.Compiled);
            isValid =  reg.IsMatch(lex);
            return isValid;
        }
        //bool isConstant(string lex)
        //{
        //    bool isDecimal = false;
        //    int i = 0;

        //    // Check if the lexeme is a valid integer or float
        //    while (i < lex.Length && IsNumber(lex[i]))
        //    {
        //        i++;
        //    }

        //    if (i < lex.Length && lex[i] == '.')
        //    {
        //        isDecimal = true;
        //        i++;
        //    }

        //    while (i < lex.Length && IsNumber(lex[i]))
        //    {
        //        i++;
        //    }

        //    // If we have reached the end of the lexeme, it is a valid constant
        //    return i == lex.Length && (isDecimal || lex.Length > 0);
        //}
        bool IsComment(String Lex)
        {
            return (Lex.Length >= 4 && Lex.StartsWith("/*") && Lex.EndsWith("*/"));
        }
       
        bool isStringLiteral(string lex)
        {
            bool isValid;
            int len = lex.Length;
            if ((lex[0] == '"' && lex[len - 1] == '"'))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            return isValid;
        }
        bool IsNumber(char letter)
        {
            return letter >= '0' || letter <= '9';
        }
    }
}
