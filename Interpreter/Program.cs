using System.Diagnostics;
using System.Linq;

namespace Interpreter
{
    internal class Program
    {
        public static void Main(string[] arassaadgs)
        {
            new SScope(null) //这个相当于应用程序的全局作用域
                .BuildIn("+", (arg, scope) => arg.Evaluate<SNumber>(scope).Sum(s => s))
                .BuildIn("-", (arg, scope) =>
                {
                    var numbers = arg.Evaluate<SNumber>(scope).ToArray();
                    var firstValue = numbers[0];
                    if (numbers.Length == 1)
                    {
                        return -firstValue;
                    }
                    return firstValue - numbers.Skip(1).Sum(s => s);
                })
                .BuildIn("*", (arg, scope) => arg.Evaluate<SNumber>(scope).Aggregate((a, b) => a * b))
                .BuildIn("/", (arg, scope) =>
                {
                    var numbers = arg.Evaluate<SNumber>(scope).ToArray();
                    var firstValue = numbers[0];
                    return firstValue / numbers.Skip(1).Aggregate((a, b) => a * b);
                })
                .BuildIn("=", (args, scope) => args.ChainRelation(scope, (s1, s2) => (long) s1 == (long) s2))
                .BuildIn(">", (args, scope) => args.ChainRelation(scope, (s1, s2) => s1 > s2))
                .BuildIn("<", (args, scope) => args.ChainRelation(scope, (s1, s2) => s1 < s2))
                .BuildIn(">=", (args, scope) => args.ChainRelation(scope, (s1, s2) => s1 >= s2))
                .BuildIn("<=", (args, scope) => args.ChainRelation(scope, (s1, s2) => s1 <= s2))
                .BuildIn("first", (args, scope) => args.RetrieveSList(scope, "first").First())
                .BuildIn("rest", (args, scope) => new SList(args.RetrieveSList(scope, "rest").Skip(1)))
                .BuildIn("append", (args, scope) =>
                {
                    SList list0 = null, list1 = null;
                    (args.Length == 2
                     && (list0 = args[0].Evaluate(scope) as SList) != null
                     && (list1 = args[1].Evaluate(scope) as SList) != null).OrThrows("请输入两个list类型的数据");
                    Debug.Assert(list1 != null, "list1 != null");
                    Debug.Assert(list0 != null, "list0 != null");
                    return new SList(list0.Concat(list1));
                })
                //开始读取数据code为读取的字符串
                .KeepInterpretingInConsole((code, scope) => code.ParseAsIScheme().Evaluate(scope));
        }
    }
}