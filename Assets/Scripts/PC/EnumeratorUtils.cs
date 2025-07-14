using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public static class EnumeratorUtils
{
    public delegate bool SelectWhereFunc<T1, T2>(T1 value, out T2 result);
    public static IEnumerable<T2> SelectWhere<T1, T2>(
        this IEnumerable<T1> self,
        SelectWhereFunc<T1, T2> fn)
    {
        return self.Select(v =>
        {
            var where = fn(v, out var result);
            return (where, result);
        })
        .Where(v => v.where)
        .Select(v => v.result);
    }
    public static T2 FirstWhere<T1, T2>(
        this IEnumerable<T1> self,
        SelectWhereFunc<T1, T2> fn)
    {
        return self.SelectWhere(fn).First();
    }
}
