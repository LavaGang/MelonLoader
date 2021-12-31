namespace IllusionPlugin
{
	public interface IPlugin
	{
		string Name { get; }
		string Version { get; }
		void OnApplicationStart();
		void OnApplicationQuit();
		void OnLevelWasLoaded(int level);
		void OnLevelWasInitialized(int level);
		void OnUpdate();
		void OnFixedUpdate();
	}
}
