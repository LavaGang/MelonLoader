namespace IllusionPlugin
{
	public interface IEnhancedPlugin : IPlugin
	{
		string[] Filter { get; }
		void OnLateUpdate();
	}
}
