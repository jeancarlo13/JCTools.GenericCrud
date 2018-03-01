using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

namespace JCTools.GenericCrud.Helpers
{
    public static class ExpressionExtensions
    {
        public static Expression ToExpression(this Models.ICrudDetails model, string expression, string modelParameterName)
        {
            var data = model.GetData();
            var p = Expression.Parameter(data.GetType(), modelParameterName);
            var e = System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda(new []
            {
                p
            }, null, expression);

            return e;
        }
    }
}