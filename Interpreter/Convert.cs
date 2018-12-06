using System.Collections.Generic;

namespace Interpreter
{
    //中缀表达式转换为后缀表达式
    public class Convert
    {
        private readonly string _value;
        public List<Convert> Children;

        public Convert(string value)
        {
            _value = value;
            Children = new List<Convert>();
        }

        public override string ToString()
        {
            if (Children.Count > 0)
            {
                return "(" + " ".CustomJoin(Children) + ")";
            }
            return _value;
        }
    }
}