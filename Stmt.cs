using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CForth
{
    public abstract class Stmt
    {
        public interface Visitor
        {
            void visitExpressionStmt(Expression stmt);
            void visitPrintStmt(Print stmt);
            void visitVarStmt(Var stmt);
            void visitBlockStmt(Block stmt);
            void visitIfStmt(If stmt);
            void visitWhileStmt(While stmt);
            void visitFunctionStmt(Function stmt);
            void visitReturnStmt(Return stmt);
            void visitClassStmt(Class stmt);
        }
        public abstract void accept(Visitor visitor);

        public class Block : Stmt
        {
            public Block(List<Stmt> statements)
            {
                this.statements = statements;
            }

            public List<Stmt> statements;

            public override void accept(Visitor visitor)
            {
                visitor.visitBlockStmt(this);
            }
        }

        public class Class : Stmt
        {
            public Class(Token name, List<Stmt> methods)
            {
                this.name = name;
                this.methods = methods;
            }

            public Token name;
            public List<Stmt> methods;

            public override void accept(Visitor visitor)
            {
                visitor.visitClassStmt(this);
            }
        }


        public class Expression : Stmt
        {
            public Expression(Expr expression)
            {
                this.expression = expression;
            }

            public Expr expression;

            public override void accept(Visitor visitor)
            {
                visitor.visitExpressionStmt(this);
            }
        }

        public class Function : Stmt
        {
            public Function(Token name, List<Token> parameters, List<Stmt> body)
            {
                this.name = name;
                this.parameters = parameters;
                this.body = body;
            }

            public Token name;
            public List<Token> parameters;
            public List<Stmt> body;

            public override void accept(Visitor visitor)
            {
                visitor.visitFunctionStmt(this);
            }
        }

        public class If : Stmt
        {
            public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            public Expr condition;
            public Stmt thenBranch;
            public Stmt elseBranch;

            public override void accept(Visitor visitor)
            {
                visitor.visitIfStmt(this);
            }
        }

        public class Print : Stmt
        {
            public Print(Expr expression)
            {
                this.expression = expression;
            }

            public Expr expression;

            public override void accept(Visitor visitor)
            {
                visitor.visitPrintStmt(this);
            }
        }

        public class Return : Stmt
        {
            public Return(Token keyword, Expr value)
            {
                this.keyword = keyword;
                this.value = value;
            }

            public Token keyword;
            public Expr value;

            public override void accept(Visitor visitor)
            {
                visitor.visitReturnStmt(this);
            }
        }

        public class Var : Stmt
        {
            public Var(Token name, Expr initializer)
            {
                this.name = name;
                this.initializer = initializer;
            }

            public Token name;
            public Expr initializer;

            public override void accept(Visitor visitor)
            {
                visitor.visitVarStmt(this);
            }
        }

        public class While : Stmt
        {
            public While(Expr condition, Stmt body)
            {
                this.condition = condition;
                this.body = body;
            }

            public Expr condition;
            public Stmt body;

            public override void accept(Visitor visitor)
            {
                visitor.visitWhileStmt(this);
            }
        }
    }
}