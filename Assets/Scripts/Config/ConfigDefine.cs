public readonly struct ItemConfig : IConfig
{
	public readonly int ID;
	public readonly int ResName;
	public readonly int Type;
	public readonly int Quality;
}
public readonly struct MapConfig : IConfig
{
	public readonly int ID;
	public readonly int[][] MapData;
	public readonly int Time;
}
