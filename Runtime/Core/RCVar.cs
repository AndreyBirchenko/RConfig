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
            return RConfig.Get<T>(_key);
        }
    }
}