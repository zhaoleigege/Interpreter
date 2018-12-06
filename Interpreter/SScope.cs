using System;
using System.Collections.Generic;

namespace Interpreter
{
    //变量的作用域
    public class SScope
    {
        public SScope Parent { get; }
        private readonly Dictionary<string, SObject> _variableTable;

        public SScope(SScope parent)
        {
            Parent = parent;
            _variableTable = new Dictionary<string, SObject>();
        }

        public SObject FindInTop(string name)
        {
            return _variableTable.ContainsKey(name) ? _variableTable[name] : null;
        }

        public SObject Find(string name)
        {
            var current = this;
            while (current != null)
            {
                if (current._variableTable.ContainsKey(name))
                {
                    return current._variableTable[name];
                }
                current = current.Parent;
            }

            throw new Exception($"{name}没有定义");
        }

        public SObject Define(string name, SObject value)
        {
            _variableTable.Add(name, value);

            return value;
        }

        public static Dictionary<string, Func<SExpression[], SScope, SObject>> BuiltinFunctions { get; } =
            new Dictionary<string, Func<SExpression[], SScope, SObject>>();

        public SScope BuildIn(string name, Func<SExpression[], SScope, SObject> builtinFuntion)
        {
            BuiltinFunctions.Add(name, builtinFuntion);
            return this;
        }

        public SScope SpawnScopeWith(string[] names, SObject[] values)
        {
            (names.Length >= values.Length).OrThrows("参数过多");
            var scope = new SScope(this);
            for (var i = 0; i < values.Length; i++)
            {
                scope._variableTable.Add(names[i], values[i]);
            }
            return scope;
        }
    }
}