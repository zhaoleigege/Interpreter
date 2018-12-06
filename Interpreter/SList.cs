using System.Collections;
using System.Collections.Generic;

namespace Interpreter
{
    //列表类型
    public class SList : SObject, IEnumerable<SObject>
    {
        private readonly IEnumerable<SObject> _values;

        public SList(IEnumerable<SObject> values)
        {
            _values = values;
        }

        public override string ToString()
        {
            return "(list " + " ".CustomJoin(_values) + ")";
        }

        public IEnumerator<SObject> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }
    }
}