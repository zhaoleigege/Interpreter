using System.Linq;

namespace Interpreter
{
    public class SFunction : SObject
    {
        public SExpression Body { get; }
        public string[] Parameters { get; }
        public SScope Scope { get; }

        public SFunction(SExpression body, string[] parameters, SScope scope)
        {
            Body = body;
            Parameters = parameters;
            Scope = scope;
        }

        private string[] ComputeFilledParameters()
        {
            return Parameters.Where(p => Scope.FindInTop(p) != null).ToArray();
        }

        public bool IsPartial => ComputeFilledParameters().Length.Between(1, Parameters.Length);

        public SObject Evaluate()
        {
            var fillParameters = ComputeFilledParameters();
            return fillParameters.Length < Parameters.Length ? this : Body.Evaluate(Scope);
        }

        public override string ToString()
        {
            return
                $"(func ({" ".CustomJoin(Parameters.Select(p => { SObject value; if ((value = Scope.FindInTop(p)) != null) return p + ": " + value; return p; }))}) {Body})";
        }

        public SFunction Update(SObject[] arguments)
        {
            var existingArguments = Parameters.Select(p => Scope.FindInTop(p)).Where(obj => obj != null);
            var newArguments = existingArguments.Concat(arguments).ToArray();
            var newScope = Scope.Parent.SpawnScopeWith(Parameters, newArguments);
            return new SFunction(Body, Parameters, newScope);
        }
    }
}