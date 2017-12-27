using UnityEngine;
using Assets.Plugins.Smart2DWaypoints.Scripts;

public class GemsDemoScript : MonoBehaviour 
{
	public WaypointsMover GemPrefabs;
	public Path[] GemPaths;

	public void SpawnGems()
	{
		Path path = GemPaths[Random.Range(0, GemPaths.Length)];
		for (int i = 0; i < 10; i++)
		{
			WaypointsMover gem = Instantiate(GemPrefabs);
			gem.Go(path);
		}
	}
}
