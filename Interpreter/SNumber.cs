namespace Interpreter
{
    //整型
    public class SNumber : SObject
    {
        private readonly long _value;

        public SNumber(long value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        //定义了一个隐式类型转换运算符
        //把一个SNumber类型的对象转换为long类型的对象
        public static implicit operator long(SNumber number)
        {
            return number._value;
        }

        //把long类型的对象转换为SNumber类型的对象
        public static implicit operator SNumber(long value)
        {
            return new SNumber(value);
        }
    }
}