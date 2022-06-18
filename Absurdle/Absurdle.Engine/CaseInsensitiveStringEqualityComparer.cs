namespace Absurdle.Engine
{
    public class CaseInsensitiveStringEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            if (x is null || y is null)
                return x is null && y is null;

            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string s)
            => s.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}
