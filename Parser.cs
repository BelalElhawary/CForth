using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace CForth
{
    public class Parser
    {
        private class ParseError : Exception { }

        private List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Stmt> parse()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!isAtEnd)
            {
                statements.Add(declaration());
            }

            return statements;
        }

        private Expr expression()
        {
            return assignment();
        }

        private Stmt statement()
        {
            if (match(TokenType.FOR)) return forStatement();
            if (match(TokenType.IF)) return ifStatement();
            if (match(TokenType.PRINT)) return printStatement();
            if (match(TokenType.RETURN)) return returnStatement();
            if (match(TokenType.WHILE)) return whileStatement();
            if (match(TokenType.LBRACE)) return new Stmt.Block(block());
            return expressionStatement();
        }

        private Stmt forStatement()
        {
            consume(TokenType.LPAREN, "Expect '(' after 'for.");

            Stmt initializer;
            if (match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (match(TokenType.VAR))
            {
                initializer = varDeclaration();
            }
            else
            {
                initializer = expressionStatement();
            }

            Expr condition = null;

            if (!check(TokenType.SEMICOLON))
            {
                condition = expression();
            }
            consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            Expr increment = null;

            if (!check(TokenType.RPAREN))
            {
                increment = expression();
            }
            consume(TokenType.RPAREN, "Expect ')' after for clauses.");

            Stmt body = statement();

            if (increment != null)
            {
                body = new Stmt.Block(new List<Stmt> { body, new Stmt.Expression(increment) });
            }

            if (condition == null) condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            if (initializer != null)
            {
                body = new Stmt.Block(new List<Stmt> { initializer, body });
            }

            return body;
        }

        private Stmt ifStatement()
        {
            consume(TokenType.LPAREN, "Expect '(' after 'if'.");
            Expr condition = expression();
            consume(TokenType.RPAREN, "Expect ')' after if condition.");

            Stmt thenBranch = statement();
            Stmt elseBranch = null;
            if (match(TokenType.ELSE))
            {
                elseBranch = statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private Stmt printStatement()
        {
            Expr value = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        private Stmt returnStatement()
        {
            Token keyword = previous();
            Expr value = null;

            if (!check(TokenType.SEMICOLON))
            {
                value = expression();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new Stmt.Return(keyword, value);
        }

        private Stmt varDeclaration()
        {
            Token name = consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (match(TokenType.EQUAL))
            {
                initializer = expression();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt whileStatement()
        {
            consume(TokenType.LPAREN, "Expect '(' after 'while'.");
            Expr condition = expression();
            consume(TokenType.RPAREN, "Expect ')' after condition.");
            Stmt body = statement();

            return new Stmt.While(condition, body);
        }

        private Stmt expressionStatement()
        {
            Expr expr = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private Stmt.Function function(string kind)
        {
            Token name = consume(TokenType.IDENTIFIER, "Expect " + kind + " name.");
            consume(TokenType.LPAREN, "Expect '(' after " + kind + " name.");
            List<Token> parameters = new List<Token>();

            if (!check(TokenType.RPAREN))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        error(peek(), "Can't have more than 255 parameters.");
                    }

                    parameters.Add(consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (match(TokenType.COMMA));
            }

            consume(TokenType.RPAREN, "Expect ')' after parameters.");

            consume(TokenType.LBRACE, "Expect '{' before " + kind + " body.");
            List<Stmt> body = block();
            return new Stmt.Function(name, parameters, body);
        }

        private Expr.Lambda lambda(string kind)
        {
            consume(TokenType.LPAREN, "Expect '(' after " + kind + " name.");
            List<Token> parameters = new List<Token>();

            if (!check(TokenType.RPAREN))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        error(peek(), "Can't have more than 255 parameters.");
                    }

                    parameters.Add(consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (match(TokenType.COMMA));
            }

            consume(TokenType.RPAREN, "Expect ')' after parameters.");

            consume(TokenType.LBRACE, "Expect '{' before " + kind + " body.");
            List<Stmt> body = block();
            return new Expr.Lambda(parameters, body);
        }

        private List<Stmt> block()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!check(TokenType.RBRACE) && !isAtEnd)
            {
                statements.Add(declaration());
            }

            consume(TokenType.RBRACE, "Expect '}' after block.");
            return statements;
        }

        private Expr equality()
        {
            Expr expr = comparison();

            while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token op = previous();
                Expr right = comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private bool match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (check(type))
                {
                    advance();
                    return true;
                }
            }

            return false;
        }

        private bool check(TokenType type)
        {
            if (isAtEnd) return false;
            return peek().type == type;
        }

        private Token advance()
        {
            if (!isAtEnd) current++;
            return previous();
        }

        private bool isAtEnd => peek().type == TokenType.EOF;

        private Token peek()
        {
            return tokens[current];
        }

        private Token previous()
        {
            return tokens[current - 1];
        }

        private Expr comparison()
        {
            Expr expr = term();

            while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = previous();
                Expr right = term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr term()
        {
            Expr expr = factor();

            while (match(TokenType.MINUS, TokenType.PLUS))
            {
                Token op = previous();
                Expr right = factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr factor()
        {
            Expr expr = unary();

            while (match(TokenType.SLASH, TokenType.STAR))
            {
                Token op = previous();
                Expr right = unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr unary()
        {
            if (match(TokenType.FUN))
            {
                return lambda("lambda");
            }

            if (match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = previous();
                Expr right = unary();
                return new Expr.Unary(op, right);
            }

            return call();
        }

        private Expr finishCall(Expr callee)
        {
            List<Expr> arguments = new List<Expr>();

            if (!check(TokenType.RPAREN))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        error(peek(), "Can't have more than 255 arguments.");
                    }
                    arguments.Add(expression());
                } while (match(TokenType.COMMA));
            }

            Token paren = consume(TokenType.RPAREN, "Expect ')' after arguments.");

            return new Expr.Call(callee, paren, arguments);
        }

        private Expr call()
        {
            Expr expr = primary();

            while (true)
            {
                if (match(TokenType.LPAREN))
                {
                    expr = finishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private Expr primary()
        {
            if (match(TokenType.FALSE)) return new Expr.Literal(false);
            if (match(TokenType.TRUE)) return new Expr.Literal(true);
            if (match(TokenType.NIL)) return new Expr.Literal(null);

            if (match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expr.Literal(previous().literal);
            }

            if (match(TokenType.IDENTIFIER))
            {
                return new Expr.Variable(previous());
            }

            if (match(TokenType.LPAREN))
            {
                Expr expr = expression();
                consume(TokenType.RPAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw error(peek(), "Expect expression.");
        }

        private Token consume(TokenType type, string message)
        {
            if (check(type)) return advance();

            throw error(peek(), message);
        }

        private ParseError error(Token token, string message)
        {
            throw new Exception(message);
            // SilkWorm.error(token, message);
            // return new ParseError();
        }

        private Stmt declaration()
        {
            try
            {
                if (match(TokenType.CLASS)) return classDeclaration();
                if (match(TokenType.FUN)) return function("function");
                if (match(TokenType.VAR)) return varDeclaration();

                return statement();
            }
            catch (ParseError error)
            {

                synchronize();
                throw error;
            }
        }

        private Stmt classDeclaration()
        {
            Token name = consume(TokenType.IDENTIFIER, "Expect class name.");
            consume(TokenType.LBRACE, "Expect '{' before class body.");

            List<Stmt> methods = new List<Stmt>();
            while (!check(TokenType.RBRACE) && !isAtEnd)
            {
                methods.Add(function("method"));
            }

            consume(TokenType.RBRACE, "Expect '}' after class body.");

            return new Stmt.Class(name, methods);
        }

        private Expr assignment()
        {
            Expr expr = or();

            if (match(TokenType.EQUAL))
            {
                Token equals = previous();
                Expr value = assignment();

                if (expr.GetType() == typeof(Expr.Variable))
                {
                    Token name = ((Expr.Variable)expr).name;
                    return new Expr.Assign(name, value);
                }

                error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr or()
        {
            Expr expr = and();

            while (match(TokenType.OR))
            {
                Token op = previous();
                Expr right = and();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private Expr and()
        {
            Expr expr = equality();

            while (match(TokenType.AND))
            {
                Token op = previous();
                Expr right = equality();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private void synchronize()
        {
            advance();

            while (!isAtEnd)
            {
                if (previous().type == TokenType.SEMICOLON) return;

                switch (peek().type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                advance();
            }
        }
    }
}
