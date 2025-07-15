namespace JsonDeepUtils
{
	public class UIChanger<T>(T value) : IParseData
	{
		public T GetValue() { return Value; }
		public bool SetValue(T value) { Value = value; return true; }
		public T Value { get; set; } = value; public virtual string Format()
		{
			return string.Format("{0:0.0}", Value);
		}
	}
}
