using System.Collections.Generic;
using System.Linq;

namespace Interpreter
{
    //定义抽象语法树
    public class SExpression
    {
        public string Value { get; }
        public List<SExpression> Children { get; }
        public SExpression Parent { get; }

        public SExpression(string value, SExpression parent)
        {
            Value = value;
            Parent = parent;
            Children = new List<SExpression>();
        }

        public override string ToString()
        {
            if (Value.Equals("("))
                return "(" + " ".CustomJoin(Children) + ")";
            return Value;
        }

        public SObject Evaluate(SScope scope)
        {
            var current = this;
            while (true)
            {
                if (current.Children.Count == 0)
                {
                    long number;
                    return long.TryParse(current.Value, out number) ? number : scope.Find(current.Value);
                }
                var first = current.Children[0];
                switch (first.Value)
                {
                    case "if":
                        var condition = (SBool) (current.Children[1].Evaluate(scope));
                        current = condition ? current.Children[2] : current.Children[3];
                        break;
                    case "def":
                        return scope.Define(current.Children[1].Value, current.Children[2].Evaluate(new SScope(scope)));
                    case "begin":
                        SObject result = null;
                        foreach (var statement in current.Children.Skip(1))
                        {
                            result = statement.Evaluate(scope);
                        }
                        return result;
                    case "func":
                        var body = current.Children[2];
                        var parameters = current.Children[1].Children.Select(exp => exp.Value).ToArray();
                        var newScope = new SScope(scope);
                        return new SFunction(body, parameters, newScope);
                    case "list":
                        return new SList(current.Children.Skip(1).Select(exp => exp.Evaluate(scope)));
                    default:
                        if (SScope.BuiltinFunctions.ContainsKey(first.Value))
                        {
                            var arguments = current.Children.Skip(1).ToArray();
                            return SScope.BuiltinFunctions[first.Value](arguments, scope);
                        }
                        else
                        {
                            var function = first.Value == "("
                                ? (SFunction) first.Evaluate(scope)
                                : (SFunction) scope.Find(first.Value);
                            var arguments = current.Children.Skip(1).Select(s => s.Evaluate(scope)).ToArray();
                            var newFunction = function.Update(arguments);
                            if (newFunction.IsPartial)
                            {
                                return scope.Define($"{first}{"".CustomJoin(arguments)}", newFunction.Evaluate());
                            }
                            current = newFunction.Body;
                            scope = newFunction.Scope;
                        }
                        break;
                }
            }
        }
    }
}