using JsonDeepUtils;

namespace SeamSearchLaserScan.Logic.ProjectSettings
{
	public class UIChangerNonFloatingFormat<T> : UIChanger<T>
	{
		public UIChangerNonFloatingFormat(T value) : base(value) { }

		public override string Format()
		{
			return string.Format("{0:0}", Value);
		}
	}
}
