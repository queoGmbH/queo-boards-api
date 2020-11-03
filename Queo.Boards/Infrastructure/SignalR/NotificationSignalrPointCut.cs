using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Queo.Boards.Core.Services;
using Spring.Aop;

namespace Queo.Boards.Infrastructure.SignalR {
    public class NotificationSignalrPointCut : IPointcut {

        public NotificationSignalrPointCut() {
            Debug.WriteLine("NotificationSignalrPointCut");
        }

        public static IList<string> Methods = new[] {
            "CreateCardNotification",
            "CreateCommentNotification"
        };

        private readonly MethodMatcherImpl _methodMatcherImpl = new MethodMatcherImpl();
        private readonly TypeFilterImpl _typeFilterImpl = new TypeFilterImpl();

        public IMethodMatcher MethodMatcher {
            get { return _methodMatcherImpl; }
        }

        public ITypeFilter TypeFilter {
            get { return _typeFilterImpl; }
        }

        private class MethodMatcherImpl : IMethodMatcher {
            public MethodMatcherImpl() {
                Debug.WriteLine("MethodMatcherImpl");
            }

            /// <summary>
            /// Is this <see cref="T:Spring.Aop.IMethodMatcher" /> dynamic?
            /// </summary>
            /// <remarks>
            /// <p>
            /// If <see langword="true" />, the three argument
            /// <see cref="M:Spring.Aop.IMethodMatcher.Matches(System.Reflection.MethodInfo,System.Type,System.Object[])" />
            /// method will be invoked if the two argument
            /// <see cref="M:Spring.Aop.IMethodMatcher.Matches(System.Reflection.MethodInfo,System.Type)" />
            /// method returns <see langword="true" />.
            /// </p>
            /// <p>
            /// Note that this property can be checked when an AOP proxy is created,
            /// and implementations need not check the value of this property again
            /// before each method invocation.
            /// </p>
            /// </remarks>
            /// <value>
            /// <see langword="true" /> if this
            /// <see cref="T:Spring.Aop.IMethodMatcher" /> is dynamic.
            /// </value>
            public bool IsRuntime {
                get {
                    return true;
                }
            }

            public bool Matches(MethodInfo method, Type targetType, object[] args) {
                return Matches(method, targetType);
            }

            public bool Matches(MethodInfo method, Type targetType) {
                return new TypeFilterImpl().Matches(targetType) && Methods.Contains(method.Name);
            }
        }

        private class TypeFilterImpl : ITypeFilter {
            /// <summary>
            ///     Nur Klassen zulassen, welche mindestens ein Interface implementieren, welches das ServiceContractAttribute hat
            /// </summary>
            public bool Matches(Type type) {
                return typeof(INotificationService).IsAssignableFrom(type);
            }
        }
    }
}