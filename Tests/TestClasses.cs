using Lab5.Core.Services;

namespace Tests
{
    public class User
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string[] Orders { get; }
        public int[] Ints { get; }

        public User(string firstName, string lastName, string[] orders)
        {
            FirstName = firstName;
            LastName = lastName;
            Orders = orders;
        }

        public User(string firstName, string lastName, string[] orders, int[] ints)
        {
            FirstName = firstName;
            LastName = lastName;
            Orders = orders;
            Ints = ints;
        }
    }

    public class TestCashe : ExpressionCashe
    {
        public int CasheContain = 0;

        public override string GetOrAdd(string identificator, object target)
        {
            Func<object, string> result;
            if (_cashe.TryGetValue($"{target.GetType()}.{identificator}", out result))
            {
                CasheContain++;
            }
            return base.GetOrAdd(identificator, target);
        }
    }
}
