namespace TicketDesk.Domain
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Resources;
    using System.Threading;

    public static class PropertyUtility
    {
        public static T GetAttribute<T>(this MemberInfo member, bool isRequired) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on member {1}",
                        typeof(T).Name,
                        member.Name));
            }

            return (T)attribute;
        }

        public static string GetPropertyDisplayString<T>(Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(@"No property reference expression was found.", nameof(propertyExpression));
            }

            var displayAttribute = memberInfo.GetAttribute<DisplayAttribute>(false);

            if (displayAttribute != null)
            {
                Debug.Assert(displayAttribute.ResourceType.FullName != null, "displayAttribute.ResourceType.FullName != null");
                var resourceManager = new ResourceManager(displayAttribute.ResourceType.FullName, displayAttribute.ResourceType.Assembly);
                var entry = resourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true)
                      .OfType<DictionaryEntry>()
                      .FirstOrDefault(p => p.Key.ToString() == displayAttribute.Name);

                var key = entry.Value.ToString();
                return key;
            }

            var displayNameAttribute = memberInfo.GetAttribute<DisplayNameAttribute>(false);
            return displayNameAttribute != null ? displayNameAttribute.DisplayName : memberInfo.Name;
        }

        private static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            Debug.Assert(propertyExpression != null, "propertyExpression != null");
            var memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                var unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }
    }
}