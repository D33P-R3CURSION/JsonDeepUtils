using Newtonsoft.Json.Linq;

namespace SeamSearchLaserScan.Logic.ProjectSettings
{
	public class UIChanger<T> : ISettingsData
	{
		public UIChanger(T value) { Value = value; }

		public T GetValue() { return Value; }
		public bool SetValue(T value) { Value = value; return true; }
		public T Value { get; set; }
		public virtual string Format()
		{
			return string.Format("{0:0.0}", Value);
		}
	}

	public class UIChangerNonFloatingFormat<T> : UIChanger<T>
	{
		public UIChangerNonFloatingFormat(T value) : base(value) { }

		public override string Format()
		{
			return string.Format("{0:0}", Value);
		}
	}
}
