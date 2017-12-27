using UnityEngine;
using Assets.Plugins.Smart2DWaypoints.Scripts;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
	public Transform[] SpawnPrefabs;
	public float Time;
	public float RepeatRate;
	public bool AutomaticSpawn;

	public WaypointsMover[] GemsPrefabs;
	public Path[] GemPaths;

	public void Start ()
	{
		if (AutomaticSpawn)
			InvokeRepeating("SpawnNext", Time, RepeatRate);
	}
	
	public void SpawnNext()
	{
		Transform item = Instantiate(SpawnPrefabs[Random.Range(0, SpawnPrefabs.Length)]);
		item.transform.position = transform.position;	
	}

	public void SpawnGems()
	{
		Path path = GemPaths[Random.Range(0, GemPaths.Length)];
		for (int i = 0; i < 10; i++)
		{
			WaypointsMover gem = Instantiate(GemsPrefabs[Random.Range(0, GemsPrefabs.Length)]);
			gem.Go(path);
		}
	}
}
