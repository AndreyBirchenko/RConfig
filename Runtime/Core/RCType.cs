using System.Globalization;

namespace RConfig.Runtime
{
    public class RCType
    {
        private readonly string _value;

        public RCType(string value)
        {
            _value = value;
        }

        public int ToInt()
        {
            return int.Parse(_value, CultureInfo.InvariantCulture);
        }

        public float ToFloat()
        {
            return float.Parse(_value, CultureInfo.InvariantCulture);
        }

        public bool ToBool()
        {
            return bool.Parse(_value);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}