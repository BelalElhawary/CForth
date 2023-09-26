using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CForth
{
    public class CForthValue
    {
        public enum ValueType
        {
            Literal,
            Compiled
        }

        public ValueType type;
        public object value;

        public CForthValue(ValueType type, object value)
        {
            this.type = type;
            this.value = value;
        }
    }
    public class Compiler : Expr.Visitor, Stmt.Visitor
    {
        int index;
        public StreamWriter writer;

        public Compiler(StreamWriter writer)
        {
            this.writer = writer;
        }


        public class RuntimeError : Exception
        {
            public Token token;

            public RuntimeError(Token token, string message) : base(message)
            {
                this.token = token;
            }
        }

        public void report_error(RuntimeError err)
        {
            Console.WriteLine($"[Error] on line {err.token.line}: {err.Message}");
        }

        public void compile(List<Stmt> statements)
        {
            try
            {
                writer.Write(Common.HEADER);
                writer.Write(Common.CODE_SEGMENT);
                writer.Write(Common.DUMP_FUNCTION);
                writer.Write(Common.ENTRY);
                foreach (Stmt statement in statements)
                {
                    statement.accept(this);
                }
                writer.Write(Common.PROGRAM_RETURN(0));
            }
            catch (RuntimeError error)
            {
                report_error(error);
            }
        }

        public void visitBinaryExpr(Expr.Binary expr)
        {
            expr.left.accept(this);
            expr.right.accept(this);

            switch (expr.op.type)
            {
                case TokenType.PLUS:
                    writer.Write(Common.PLUS);
                    break;
                case TokenType.MINUS:
                    writer.Write(Common.MINUS);
                    break;
                case TokenType.EQUAL_EQUAL:
                    writer.Write(Common.EQUAL);
                    break;
                case TokenType.BANG_EQUAL:
                    writer.Write(Common.NOT_EQUAL);
                    break;
                default: break;
            }
        }

        public void visitGroupingExpr(Expr.Grouping expr)
        {
            throw new NotImplementedException();
        }

        public void visitLiteralExpr(Expr.Literal expr)
        {
            writer.Write(Common.PUSH(expr.value));
        }

        public void visitUnaryExpr(Expr.Unary expr)
        {
            throw new NotImplementedException();
        }

        public void visitAssignExpr(Expr.Assign expr)
        {
            throw new NotImplementedException();
        }

        public void visitVariableExpr(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }

        public void visitLogicalExpr(Expr.Logical expr)
        {
            throw new NotImplementedException();
        }

        public void visitCallExpr(Expr.Call expr)
        {
            throw new NotImplementedException();
        }

        public void visitLambdaExpr(Expr.Lambda expr)
        {
            throw new NotImplementedException();
        }

        public void visitExpressionStmt(Stmt.Expression stmt)
        {
            throw new NotImplementedException();
        }

        public void visitPrintStmt(Stmt.Print stmt)
        {
            stmt.expression.accept(this);
            writer.Write(Common.DUMP);
        }

        public void visitVarStmt(Stmt.Var stmt)
        {
            throw new NotImplementedException();
        }

        public void visitBlockStmt(Stmt.Block stmt)
        {
            foreach (var state in stmt.statements)
            {
                state.accept(this);
            }
        }

        public void visitIfStmt(Stmt.If stmt)
        {
            var i = index;
            index = stmt.elseBranch == null ? index + 1 : index + 2;
            stmt.condition.accept(this);
            if (stmt.elseBranch != null)
            {
                writer.Write(Common.IF(i + 1));
                stmt.thenBranch.accept(this);
                writer.Write(Common.ELSE(i, i + 1));
                stmt.elseBranch.accept(this);
                writer.Write($"addr_{i}:");
            }
            else
            {
                writer.Write(Common.IF(i));
                stmt.thenBranch.accept(this);
                writer.Write($"addr_{i}:");
            }
        }

        public void visitWhileStmt(Stmt.While stmt)
        {
            throw new NotImplementedException();
        }

        public void visitFunctionStmt(Stmt.Function stmt)
        {
            throw new NotImplementedException();
        }

        public void visitReturnStmt(Stmt.Return stmt)
        {
            throw new NotImplementedException();
        }

        public void visitClassStmt(Stmt.Class stmt)
        {
            throw new NotImplementedException();
        }

        public void Log(string msg)
        {
            writer.Write($";; -------------------- {msg} --------------------\n");
        }
    }
}
