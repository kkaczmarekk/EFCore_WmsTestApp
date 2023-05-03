using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WmsApp.Persistence.Common.Migrations
{
    public class MigrationScriptFilterBuilder
    {
        private Expression<Func<MigrationScriptResourceData, bool>> _queryExpresion;

        public MigrationScriptFilterBuilder() { }

        public MigrationScriptFilterBuilder AddStoredProcedure(string name = "", string version = "")
        {
            Expression<Func<MigrationScriptResourceData, bool>> newExpresion = s => s.SctiptType == "StoredProcedures"
                && (s.SctiptName == name || name == "")
                && (s.VersionStr == version || version == "");

            _queryExpresion = AddOrExpresion(_queryExpresion, newExpresion);

            return this;
        }

        public MigrationScriptFilterBuilder AddFunction(string name = "", string version = "")
        {
            Expression<Func<MigrationScriptResourceData, bool>> newExpresion = s => s.SctiptType == "Functions"
                && (s.SctiptName == name || name == "")
                && (s.VersionStr == version || version == "");

            _queryExpresion = AddOrExpresion(_queryExpresion, newExpresion);

            return this;
        }

        internal Func<MigrationScriptResourceData, bool> Build()
        {
            if (_queryExpresion == null)
                return r => r.ToString() != "";
     
            return _queryExpresion.Compile();
        }

        private static Expression<Func<MigrationScriptResourceData, bool>> AddOrExpresion(
            Expression<Func<MigrationScriptResourceData, bool>> a,
            Expression<Func<MigrationScriptResourceData, bool>> b)
        {
            if (a == null)
                return b;

            return Expression.Lambda<Func<MigrationScriptResourceData, bool>>(Expression.OrElse(
                new SwapVisitor(a.Parameters[0], b.Parameters[0]).Visit(a.Body),
                    b.Body), b.Parameters);
        }


        //https://stackoverflow.com/questions/10613514/
        class SwapVisitor : ExpressionVisitor
        {
            private readonly Expression from, to;
            public SwapVisitor(Expression from, Expression to)
            {
                this.from = from;
                this.to = to;
            }
            public override Expression Visit(Expression node)
            {
                return node == from ? to : base.Visit(node);
            }
        }
    }
}
