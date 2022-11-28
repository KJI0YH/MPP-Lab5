using Lab5.Core;
using System.Collections.Concurrent;

namespace Tests
{
    public class Tests
    {
        [Test]
        public void CorrectTemplateTest()
        {
            User user = new User("Aliaksei", "Kryzhanouski", new string[] { "sleep" });
            string result = StringFormatter.Shared.Format("User {FirstName} {LastName} order {Orders[0]}", user);
            Assert.That($"User {user.FirstName} {user.LastName} order {user.Orders[0]}".Equals(result));
        }

        [Test]
        public void EscapingTest()
        {
            User user = new User("Aliaksei", "Kryzhanouski", new string[] { "sleep" });
            string result = StringFormatter.Shared.Format("User {{{FirstName}}} {{{LastName}}} order {{{Orders[0]}}}", user);
            Assert.That($"User {{{user.FirstName}}} {{{user.LastName}}} order {{{user.Orders[0]}}}".Equals(result));
        }

        [Test]
        public void InvalidTemplateTest()
        {
            User user = new User("Aliaksei", "Kryzhanouski", new string[] { "sleep" });

            Assert.Multiple(() =>
            {
                Assert.Catch<ArgumentException>(() =>
                {
                    string result = StringFormatter.Shared.Format("User {FirstName}} {LastName} order {Orders[0]}", user);
                });

                Assert.Catch<ArgumentException>(() =>
                {
                    string result = StringFormatter.Shared.Format("User {{FirstName} {LastName} order {Orders[0]}", user);
                });

                Assert.Catch<ArgumentException>(() =>
                {
                    string result = StringFormatter.Shared.Format("User {First} {LastName} order {Orders[0]}", user);
                });
            });
        }

        [Test]
        public void ArrayIndexTest()
        {
            User user = new User("Aliaksei", "Kryzhanouski", new string[] { "sleep", "swim", "repeat" }, new int[] { 1, 2, 3, });
            string result = StringFormatter.Shared.Format("User contains {Orders[0]}, {Orders[1]}, {Orders[2]} and {Ints[0]}, {Ints[1]}, {Ints[2]}", user);
            Assert.That($"User contains {user.Orders[0]}, {user.Orders[1]}, {user.Orders[2]} and {user.Ints[0]}, {user.Ints[1]}, {user.Ints[2]}".Equals(result));
        }

        [Test]
        public void InvalidArrayIndexTest()
        {
            User user = new User("Aliaksei", "Kryzhanouski", new string[] { "sleep", "swim", "repeat" }, new int[] { 1, 2, 3, });

            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                string result = StringFormatter.Shared.Format("User contains {Orders[3]}, {Ints[3]}", user);
            });
        }

        [Test]
        public void MultiThreadTest()
        {
            User user = new User("Aliaksei", "Kryzhanouski", new string[] { "sleep", "swim", "repeat" }, new int[] { 1, 2, 3, });
            var threads = new List<Thread>();
            var threadsCount = 10;

            var results = new ConcurrentBag<bool>();
            for (int i = 0; i < threadsCount; i++)
            {
                var j = i;
                var thread = new Thread(() =>
                {
                    results.Add(ThreadProc("User {FirstName} {LastName} order {Orders[0]} and {Ints[0]}", user,
                        $"User {user.FirstName} {user.LastName} order {user.Orders[0]} and {user.Ints[0]}"));
                });
                threads.Add(thread);
                thread.Start();
            }

            Assert.That(threads.Count, Is.EqualTo(threadsCount));

            for (int i = 0; i < threadsCount; i++)
            {
                threads[i].Join();
            }

            Assert.Multiple(() =>
            {
                Assert.That(results.Count.Equals(threadsCount));
                Assert.That(!results.Contains(false));
            });
        }

        public bool ThreadProc(string template, object target, string standart)
        {
            int loopCount = 10;
            for (int i = 0; i < loopCount; i++)
            {
                string result = StringFormatter.Shared.Format(template, target);
                if (result != standart)
                {
                    return false;
                }
            }
            return true;
        }

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
    }
}