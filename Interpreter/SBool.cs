namespace Interpreter
{
    //bool类型
    public class SBool : SObject
    {
        public static readonly SBool False = new SBool();
        public static readonly SBool True = new SBool();

        public override string ToString()
        {
            return ((bool) this).ToString();
        }

        //SBool类型转bool类型
        public static implicit operator bool(SBool value)
        {
            return value.Equals(True);
        }

        //bool类型转SBool类型
        public static implicit operator SBool(bool value)
        {
            return value ? True : False;
        }
    }
}