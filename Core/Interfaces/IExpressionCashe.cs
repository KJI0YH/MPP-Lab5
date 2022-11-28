namespace Lab5.Core.Interfaces
{
    public interface IExpressionCashe
    {
        string GetOrAdd(string identificator, object target);
    }
}
