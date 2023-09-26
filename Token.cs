using System;

namespace CForth
{
    public class Token
    {
        public TokenType type;
        public string lexeme;
        public object literal;
        public int line;

        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString()
        {
            return $"[{type}]";
        }
    }

    public enum TokenType
    {
        // Single character tokens.
        LPAREN = 40, RPAREN = 41, LBRACE = 123, RBRACE = 125, SEMICOLON = 59, COMMA = 44, MINUS = 45, PLUS = 43, SLASH = 47, STAR = 42, DOT = 46,

        // Opreation character tokens
        BANG = 33, BANG_EQUAL = 20, EQUAL = 21, EQUAL_EQUAL = 22, GREATER = 23, GREATER_EQUAL = 24, LESS = 25, LESS_EQUAL = 26,

        // literals
        IDENTIFIER = 0, STRING = 1, NUMBER = 2,

        // Keywords
        AND = 3, CLASS = 4, ELSE = 5, FALSE = 6, FUN = 7, FOR = 8, IF = 9, NIL = 10, OR = 11, PRINT = 12, RETURN = 13, SUPER = 14, THIS = 15, TRUE = 16, VAR = 17, WHILE = 18,

        EOF = 19,
        PLUS_EQUAL = 126,
        MINUS_EQUAL = 127
    }
}