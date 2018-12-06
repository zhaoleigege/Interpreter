using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpreter
{
    //此法分析器
    public static class Lex
    {
        //把输进来的每个词语都放到数组中
        private static IEnumerable<string> Tokenize(string text)
        {
            return text
                .Replace("(", " ( ")
                .Replace(")", " ) ") //把空格 Tab换行符这些全部去掉
                .Split(" \t\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        //给int添加一个自定义方法Between
        public static bool Between(this int number, int lowest, int highest)
        {
            return number >= lowest && number < highest;
        }

        //扩展string的静态方法,自己手动添加一个新的方法
        public static string CustomJoin(this string separator, IEnumerable<object> value)
        {
            return string.Join(separator, value);
        }

        public static string PrettyPrint(string text)
        {
            return "[" + ", ".CustomJoin(Tokenize(text).Select(s => "'" + s + "'")) + "]";
        }

        //抽象语法树的构建
        public static SExpression ParseAsIScheme(this string code)
        {
            var program = new SExpression("", null);
            var current = program;

            foreach (var lex in Tokenize(code))
            {
                if (lex.Equals("("))
                {
                    var newNode = new SExpression("(", current);
                    current.Children.Add(newNode);
                    current = newNode;
                }
                else if (lex.Equals(")"))
                {
                    current = current.Parent;
                }
                else
                {
                    current.Children.Add(new SExpression(lex, current));
                }
            }

            return program.Children[0];
        }

        //给bool类型添加一个自定义方法
        public static void OrThrows(this bool condition, string message = null)
        {
            if (!condition)
            {
                throw new Exception(message ?? "错误");
            }
        }

        public static IEnumerable<T> Evaluate<T>(this IEnumerable<SExpression> expressions, SScope scope)
            where T : SObject
        {
            return expressions.Evaluate(scope).Cast<T>();
        }

        public static IEnumerable<SObject> Evaluate(this IEnumerable<SExpression> expressions, SScope scope)
        {
            return expressions.Select(exp => exp.Evaluate(scope));
        }

        public static SBool ChainRelation(this SExpression[] expressions, SScope scope,
            Func<SNumber, SNumber, bool> relation)
        {
            (expressions.Length > 1).OrThrows("该操作要求至少有一个操作符");
            var current = (SNumber) expressions[0].Evaluate(scope);
            foreach (var obj in expressions.Skip(1))
            {
                var next = (SNumber) obj.Evaluate(scope);
                if (relation(current, next))
                {
                    current = next;
                }
                else
                {
                    return SBool.False;
                }
            }
            return SBool.True;
        }

        public static SList RetrieveSList(this SExpression[] expressions, SScope scope, string operationName)
        {
            SList list = null;
            (expressions.Length == 1 && (list = expressions[0].Evaluate(scope) as SList) != null)
                .OrThrows("<" + operationName + "> 必须是一个list");
            return list;
        }

        public static void KeepInterpretingInConsole(this SScope scope, Func<string, SScope, SObject> evaluate)
        {
            while (true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(">> ");
                    string code;
                    if (string.IsNullOrWhiteSpace(code = Console.ReadLine())) continue;
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (code.Equals("quit", StringComparison.OrdinalIgnoreCase))
                        break;
                    Console.WriteLine(">> " + evaluate(code.PrefixConvert(), scope));
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(">> " + ex.Message);
                }
            }
        }

        private static readonly Dictionary<string, int> Operation = new Dictionary<string, int>
        {
            {"+", 0},
            {"-", 0},
            {"*", 1},
            {"/", 1},
            {">", 0},
            {"<", 0},
            {">=", 0},
            {"<=", 0}
        };

        private static void Transposition(out Convert convert, Stack<Convert> numberStack, string s)
        {
            convert = new Convert(null);
            convert.Children.Add(new Convert(s));
            var temp = numberStack.Pop();
            convert.Children.Add(numberStack.Pop());
            convert.Children.Add(temp);
        }

        //中缀表达式转前缀表达式
        public static string PrefixConvert(this string value)
        {
            if (value[0].Equals('('))
                return value;
            var numberStack = new Stack<Convert>();
            var operatorStack = new Stack<string>();

            IEnumerable<string> valueArray = value.Split(" \t\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var s in valueArray)
            {
                if (Operation.ContainsKey(s))
                {
                    if (operatorStack.Count > 0)
                    {
                        if (Operation[s] <= Operation[operatorStack.Peek()])
                        {
                            Convert convert;
                            Transposition(out convert, numberStack, operatorStack.Pop());
                            numberStack.Push(convert);
                            operatorStack.Push(s);
                            continue;
                        }
                    }
                    operatorStack.Push(s);
                }
                else
                {
                    numberStack.Push(new Convert(s));
                }
            }

            var stringBuilder = new StringBuilder();
            while (operatorStack.Count > 0)
            {
                Convert convert;
                Transposition(out convert, numberStack, operatorStack.Pop());
                numberStack.Push(convert);
            }
            while (numberStack.Count > 0)
            {
                stringBuilder.Append(numberStack.Pop());
            }

            return stringBuilder.ToString();
        }
    }
}