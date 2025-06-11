namespace System.Mvc
{
    public class ViewDataDictionary : Session
    {
        public object Model { get; set; }

        public void SetValue<T>(object value)
        {
            this[typeof(T).Name] = value;
        }
        public object GetValue<T>()
        {
            return this[typeof(T).Name];
        }
    }
}
