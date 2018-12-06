namespace Interpreter
{
    public class SObject
    {
        public static implicit operator SObject(long value)
        {
            return (SNumber) value;
        }

        public static implicit operator SObject(bool value)
        {
            return (SBool) value;
        }
    }
}