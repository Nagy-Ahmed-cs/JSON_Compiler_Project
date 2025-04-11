using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp, Elseif, Return, Endl,
    Idenifier, Constant, Comment, StringLiteral, Int, Float, Main, Number, CurlyBraket, String, Repeat,
    AssignmentOp,AND,OR,Underscore,StrangeChar
}

namespace JASON_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;

        // Default constructor
        public Token()
        {
        }

        // Parameterized constructor
        public Token(Token_Class type, string lexeme)
        {
            this.token_type = type;
            this.lex = lexeme;
        }
    }


    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Symbols = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("main", Token_Class.Main);

           
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
            Symbols.Add("{", Token_Class.CurlyBraket);  
            Symbols.Add("}", Token_Class.CurlyBraket);
            Operators.Add(":=", Token_Class.AssignmentOp); 
            Operators.Add("&&", Token_Class.AND);
            Operators.Add("||", Token_Class.OR);
            Operators.Add("_", Token_Class.Underscore);
            Operators.Add("–", Token_Class.MinusOp);






        }

        public void StartScanning(string SourceCode)
        {
            Tokens.Clear();
            Errors.Error_List.Clear();
            int i = 0;

            while (i < SourceCode.Length)
            {
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();// transfer char to string
                int j = i + 1;

                //  tokens  لو في مسافات أو سطر جديد خلاص يبقى مفيش 
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                {
                    i++;
                    continue;
                }
                
                // هنا بياخد الكلمة و يحدد نوعها سواء كانت Reserved Word أو Identifier
                if (char.IsLetter(CurrentChar))
                {
                    
                    
                    while (j < SourceCode.Length && char.IsLetterOrDigit(SourceCode[j]))
                    {
                        
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                }


                // هنا بيقرأ الارقام سواء كانت Integers أو Doubles
                // Number handler (for Integers and Doubles)
                //else if (char.IsDigit(CurrentChar))
                //{
                //    bool hasDot = false;

                //    // Read all consecutive digits and at most one decimal point
                //    while (j < SourceCode.Length && (char.IsDigit(SourceCode[j]) || SourceCode[j] == '.'))
                //    {
                //        CurrentLexeme += SourceCode[j];

                //        // Check for invalid format with multiple dots
                //        if (SourceCode[j] == '.')
                //        {
                //            if (hasDot)
                //            {
                //                // Found a second dot, but keep reading to get the full invalid token
                //                j++;
                //                // Continue collecting the rest of the "number" to report the full invalid lexeme
                //                while (j < SourceCode.Length && (char.IsDigit(SourceCode[j]) || SourceCode[j] == '.'))
                //                {
                //                    CurrentLexeme += SourceCode[j];
                //                    j++;
                //                }

                //                Errors.Error_List.Add($"Invalid number format (multiple decimal points): {CurrentLexeme}");
                //                i = j;
                //                continue;
                //            }
                //            hasDot = true;
                //        }
                //        j++;
                //  }
                //// Check for trailing decimal point
                //if (CurrentLexeme.EndsWith("."))
                //    {
                //        Errors.Error_List.Add($"Invalid number format (trailing decimal point): {CurrentLexeme}");
                //    }
                //    else
                //    {
                //        Tokens.Add(new Token(Token_Class.Number, CurrentLexeme));
                //    }
                //    i = j;
                //    continue;
                //}
                else if (char.IsDigit(CurrentChar))
                {
                    // Collect full alphanumeric sequence (e.g., "123var")
                    while (j < SourceCode.Length && (char.IsLetterOrDigit(SourceCode[j]) || SourceCode[j] == '.'))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    // Send to FindTokenClass for classification
                    FindTokenClass(CurrentLexeme);
                    i = j;
                    continue;
                }

               

                // commnet handeler .....
                else if (CurrentChar == '/' && j < SourceCode.Length && SourceCode[j] == '*')
                {
                    CurrentLexeme += SourceCode[j];
                    j++;
                    while (j < SourceCode.Length - 1 && !(SourceCode[j] == '*' && SourceCode[j + 1] == '/'))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    if (j < SourceCode.Length - 1)
                    {
                        CurrentLexeme += "*/";
                        j += 2;
                    }
                }
                // String Reader (handler) .....
                else if (CurrentChar == '"')
                {
                    while (j < SourceCode.Length && SourceCode[j] != '"')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    if (j < SourceCode.Length)
                    {
                        CurrentLexeme += '"';
                        j++;
                    }
                }
                else if (CurrentChar == ':' && j < SourceCode.Length && SourceCode[j] == '=')
                {
                    CurrentLexeme += SourceCode[j];
                    j++;
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                    continue;
                }

                // Opearators Handelrs .....
                else
                {
                    if (j < SourceCode.Length)
                    {
                        char SecondChar = SourceCode[j];
                        if ((CurrentChar == '&' && SecondChar == '&') ||
                            (CurrentChar == '|' && SecondChar == '|') ||
                            (CurrentChar == '<' && SecondChar == '>') ||
                            (CurrentChar == ':' && SecondChar == '='))
                        {
                            CurrentLexeme += SecondChar;
                            j++;
                        }
                    }
                }

                
                FindTokenClass(CurrentLexeme);
                i = j;
            }

            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            if (string.IsNullOrWhiteSpace(Lex)) return;

            Token Tok = new Token();
            Tok.lex = Lex;

            // token is a lexeme and token type ..
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
            }
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
            }
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
            }
            else if (Symbols.ContainsKey(Lex))
            {
                Tok.token_type = Symbols[Lex];
            }
            // Is acomment ?
            else if (IsComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
            }
            else if (isStringLiteral(Lex))
            {
                Tok.token_type = Token_Class.StringLiteral;
            }
            else if (isDigit(Lex))
            {
                Tok.token_type = Token_Class.Number;
            }
            else
            {
                Errors.Error_List.Add("Invalid Token: " + Lex); // from 
                return;
            }

            Tokens.Add(Tok);
        }

        bool isIdentifier(string lex)
        {
            // Reject if starts with digit
            if (char.IsDigit(lex[0]))
            {
                Errors.Error_List.Add($"Invalid identifier: identifiers cannot start with digits: '{lex}'");
                return false;
            }

            // Check remaining characters
            for (int i = 1; i < lex.Length; i++)
            {
                if (!char.IsLetterOrDigit(lex[i]) && lex[i] != '_')
                {
                    Errors.Error_List.Add($"Invalid character in identifier: '{lex[i]}' in '{lex}'");
                    return false;
                }
            }
            return true;
        }

        bool IsComment(string lex)
        {
            return lex.StartsWith("/*") && lex.EndsWith("*/");
        }

        bool isStringLiteral(string lex)
        {
            return lex.StartsWith("\"") && lex.EndsWith("\"");
        }
        bool isDigit(string lex)
        {
            

           return Regex.IsMatch(lex, @"^[0-9]+(\.[0-9]+)?$");


        }
        

    }
}
