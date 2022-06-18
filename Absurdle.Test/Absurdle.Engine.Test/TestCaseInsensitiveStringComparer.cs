namespace Absurdle.Engine.Test
{
    [TestClass]
    public class TestCaseInsensitiveStringComparer
    {
        [
            TestMethod,
            DataRow(null, null),
            DataRow("", ""),
            DataRow("a", "a"),
            DataRow("a", "A"),
            DataRow("hello, world!", "HeLlO, WoRLD!"),
        ]
        public void TestEqual(string? x, string? y)
        {
            CaseInsensitiveStringEqualityComparer comparer = new();

            Assert.IsTrue(
                comparer.Equals(x, y)
            );
        }

        [
            TestMethod,
            DataRow(null, ""),
            DataRow("", null),
            DataRow("x", "y"),
            DataRow("a", "aa"),
            DataRow("a", "B"),
            DataRow("hello  world!", "HeLlO, WoRLD!"),
        ]
        public void TestNotEqual(string? x, string? y)
        {
            CaseInsensitiveStringEqualityComparer comparer = new();

            Assert.IsFalse(
                comparer.Equals(x, y)
            );
        }

        [
            TestMethod,
            DataRow("a", "a"),
            DataRow("a", "A"),
            DataRow("hello, world!", "hello, world!"),
            DataRow("hello, world!", "HeLlO, WoRLD!"),
        ]
        public void TestHashCodes(string x, string y)
        {
            CaseInsensitiveStringEqualityComparer comparer = new();

            Assert.AreEqual(
                comparer.GetHashCode(x),
                comparer.GetHashCode(y)
            );
        }
    }
}
