using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace CForth
{
    internal class Scanner
    {
        private string source;
        private List<Token> tokens;

        private int start = 0, current = 0, line = 1;

        bool isAtEnd => current >= source.Length;

        private char advance()
        {
            return source[current++];
        }

        private const string characters = "(){},.-+;*";
        private static Dictionary<string, TokenType> keywords;

        public static void init_keywords()
        {
            keywords = new Dictionary<string, TokenType>
            {
                { "and", TokenType.AND },
                { "class", TokenType.CLASS },
                { "else", TokenType.ELSE },
                { "false", TokenType.FALSE },
                { "for", TokenType.FOR },
                { "fun", TokenType.FUN },
                { "if", TokenType.IF },
                { "nil", TokenType.NIL },
                { "or", TokenType.OR },
                { "print", TokenType.PRINT },
                { "return", TokenType.RETURN },
                { "super", TokenType.SUPER },
                { "this", TokenType.THIS },
                { "true", TokenType.TRUE },
                { "var", TokenType.VAR },
                { "while", TokenType.WHILE }
            };
        }

        public Scanner(string source)
        {
            init_keywords();
            this.source = source;
            tokens = new List<Token>();
        }

        public List<Token> scanTokens()
        {
            while (!isAtEnd)
            {
                start = current;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        private void scanToken()
        {
            char c = advance();
            if (characters.Contains(c.ToString()))
            {
                addToken((TokenType)c);
            }
            else
            {
                switch (c)
                {
                    case '\'': String('\''); break;
                    case '"': String('"'); break;
                    case '!':
                        addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                        break;
                    case '=':
                        addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                        break;
                    case '<':
                        addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                        break;
                    case '>':
                        addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                        break;
                    case '/':
                        if (match('/'))
                        {
                            // A comment goes until the end of the line.
                            while (peek() != '\n' && !isAtEnd) advance();
                        }
                        else if (match('*'))
                        {
                            while (peek() != '*' && peekNext() != '/' && !isAtEnd)
                            {
                                if (peek() == '\n') line++;
                                advance();
                            }
                            advance();
                            advance();
                        }
                        else
                        {
                            addToken(TokenType.SLASH);
                        }
                        break;
                    case ' ':
                        // Ignore whitespace.
                        break;
                    case '\r':
                        // Ignore whitespace.
                        break;
                    case '\t':
                        // Ignore whitespace.
                        break;
                    case '\n':
                        line++;
                        break;
                    default:
                        if (char.IsDigit(c))
                        {
                            Number();
                        }
                        else if (char.IsLetter(c))
                        {
                            Identifier();
                        }
                        else
                        {
                            // SilkWorm.error(line, "Unexpected character.");
                        }
                        break;
                }
            }
        }

        bool isAlphaNumeric(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        private void Identifier()
        {
            while (isAlphaNumeric(peek())) advance();

            string text = source.Substring(start, current - start);
            var contains = keywords.TryGetValue(text, out TokenType type);
            if (!contains) type = TokenType.IDENTIFIER;
            addToken(type);
        }


        private void Number()
        {
            while (char.IsDigit(peek())) advance();

            // Look for a fractional part.
            if (peek() == '.' && char.IsDigit(peekNext()))
            {
                // Consume the "."
                advance();

                while (char.IsDigit(peek())) advance();
            }

            addToken(TokenType.NUMBER, double.Parse(source.Substring(start, current - start)));
        }

        private void String(char c)
        {
            while (peek() != c && !isAtEnd)
            {
                if (peek() == '\n') line++;
                advance();
            }

            if (isAtEnd)
            {
                // SilkWorm.error(line, "Unterminated string.");
                return;
            }

            // The closing ".
            advance();

            // Trim the surrounding quotes.
            string value = source.Substring(start + 1, (current - start) - 2);
            addToken(TokenType.STRING, Regex.Unescape(value));
        }

        private char peek()
        {
            if (isAtEnd) return '\0';
            return source[current];
        }

        private char peekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private bool match(char expected)
        {
            if (isAtEnd) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        private void addToken(TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}