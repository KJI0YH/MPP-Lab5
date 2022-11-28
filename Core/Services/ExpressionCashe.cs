using Lab5.Core.Interfaces;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Lab5.Core.Services
{
    public class ExpressionCashe : IExpressionCashe
    {
        private readonly ConcurrentDictionary<string, Func<object, string>> _cashe = new();

        public string GetOrAdd(string identificator, object target)
        {
            string key = $"{target.GetType()}.{identificator}";
            Func<object, string> result;
            if (_cashe.TryGetValue(key, out result))
                return result(target);
            else
            {
                var id = identificator.Split(new char[2] { '[', ']' });

                var fields = target.GetType().GetFields();
                var properties = target.GetType().GetProperties();

                if (fields.Where(field => field.Name == id[0]).Any() ||
                    properties.Where(property => property.Name == id[0]).Any())
                {

                    var objParam = Expression.Parameter(typeof(object), "obj");


                    Expression propertyOrField;
                    if (id.Length > 1)
                    {
                        var array = Expression.PropertyOrField(Expression.TypeAs(objParam, target.GetType()), id[0]);
                        propertyOrField = Expression.ArrayIndex(array, Expression.Constant(int.Parse(id[1]), typeof(int)));
                    }
                    else
                    {
                        propertyOrField = Expression.PropertyOrField(Expression.TypeAs(objParam, target.GetType()), id[0]);
                    }

                    var toString = Expression.Call(propertyOrField, "ToString", null, null);
                    var func = Expression.Lambda<Func<object, string>>(toString, objParam).Compile();

                    result = _cashe.GetOrAdd(key, func);

                    return result(target);
                }
                else
                    throw new ArgumentException($"The object {target.GetType()} does not contain {identificator}");
            }
        }
    }
}
