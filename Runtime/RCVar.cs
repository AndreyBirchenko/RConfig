namespace RConfig.Runtime
{
    public class RCVar<T> where T : RCScheme
    {
        private T _value;
        private string _key;

        public RCVar(string key)
        {
            _key = key;
        }

        public T Get()
        {
            if (_value == null)
            {
                UpdateValue();
            }

            return _value;
        }

        private void UpdateValue()
        {
            _value = RConfig.Get<T>(_key);
        }
    }
}