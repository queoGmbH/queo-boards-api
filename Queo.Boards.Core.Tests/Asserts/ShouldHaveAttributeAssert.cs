using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Queo.Boards.Core.Tests.Asserts {
    public static class ShouldHaveAttributeAssert {
        private static MethodInfo MethodOf(Expression<System.Action> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }

        public static bool ShouldHaveAttribute<T>(this Expression<System.Action> expression)
        {
            var method = MethodOf(expression);

            const bool includeInherited = false;
            return method.GetCustomAttributes(typeof(T), includeInherited).Any();
        }
    }
}