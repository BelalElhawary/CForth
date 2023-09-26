using System.Collections.Generic;

namespace CForth
{
    public abstract class Expr
    {
        public interface Visitor
        {
            void visitBinaryExpr(Binary expr);
            void visitGroupingExpr(Grouping expr);
            void visitLiteralExpr(Literal expr);
            void visitUnaryExpr(Unary expr);
            void visitAssignExpr(Assign expr);
            void visitVariableExpr(Variable expr);
            void visitLogicalExpr(Logical expr);
            void visitCallExpr(Call expr);
            void visitLambdaExpr(Lambda expr);
        }
        public abstract void accept(Visitor visitor);

        public class Binary : Expr
        {
            public Binary(Expr left, Token op, Expr right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public Expr left;
            public Token op;
            public Expr right;

            public override void accept(Visitor visitor)
            {
                visitor.visitBinaryExpr(this);
            }
        }

        public class Call : Expr
        {
            public Call(Expr callee, Token paren, List<Expr> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }

            public Expr callee;
            public Token paren;
            public List<Expr> arguments;

            public override void accept(Visitor visitor)
            {
                visitor.visitCallExpr(this);
            }
        }

        public class Grouping : Expr
        {
            public Grouping(Expr expression)
            {
                this.expression = expression;
            }

            public Expr expression;

            public override void accept(Visitor visitor)
            {
                visitor.visitGroupingExpr(this);
            }
        }
        public class Literal : Expr
        {
            public Literal(object value)
            {
                this.value = value;
            }

            public object value;

            public override void accept(Visitor visitor)
            {
                visitor.visitLiteralExpr(this);
            }
        }

        public class Logical : Expr
        {
            public Expr left;
            public Token op;
            public Expr right;

            public Logical(Expr left, Token op, Expr right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public override void accept(Visitor visitor)
            {
                visitor.visitLogicalExpr(this);
            }
        }
        public class Unary : Expr
        {
            public Unary(Token op, Expr right)
            {
                this.op = op;
                this.right = right;
            }

            public Token op;
            public Expr right;

            public override void accept(Visitor visitor)
            {
                visitor.visitUnaryExpr(this);
            }
        }

        public class Variable : Expr
        {
            public Variable(Token name)
            {
                this.name = name;
            }

            public Token name;

            public override void accept(Visitor visitor)
            {
                visitor.visitVariableExpr(this);
            }
        }

        public class Assign : Expr
        {
            public Assign(Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }

            public Token name;
            public Expr value;

            public override void accept(Visitor visitor)
            {
                visitor.visitAssignExpr(this);
            }
        }

        public class Lambda : Expr
        {
            public Lambda(List<Token> parameters, List<Stmt> body)
            {
                this.parameters = parameters;
                this.body = body;
            }

            public List<Token> parameters;
            public List<Stmt> body;

            public override void accept(Visitor visitor)
            {
                visitor.visitLambdaExpr(this);
            }
        }
    }
}