using System;
using Assets.Plugins.Smart2DWaypoints.Scripts;
using UnityEngine;

namespace Assets.Plugins.Smart2DWaypoints.Demo
{
	public class RocketsDemoScript : MonoBehaviour
	{
		public Path[] RocketPaths;
		public WaypointsMover RocketPrefab;
		public ParticleSystem ExplosionPrefab;

		public void OnEnable()
		{
			Invoke("LaunchRockets", 1f);
			Invoke("LaunchRockets", 2f);
			Invoke("LaunchRockets", 2.5f);
		}

		public void LaunchRockets()
		{
			foreach (Path path in RocketPaths)
			{
				WaypointsMover mover = Instantiate(RocketPrefab);
				mover.OneWayFinished += OnOneWayFinished;
				mover.Go(path);
			    mover.transform.SetParent(transform);
			}
		}

		void OnOneWayFinished(object sender, EventArgs e)
		{
			WaypointsMover mover = sender as WaypointsMover;
			mover.OneWayFinished -= OnOneWayFinished;

			ParticleSystem explosion = Instantiate(ExplosionPrefab);
			explosion.transform.position = mover.transform.position;
			Destroy(explosion.gameObject, explosion.duration);

			mover.GetComponent<SpriteRenderer>().enabled = false;
			mover.GetComponentInChildren<ParticleSystem>().Stop();
			Destroy(mover.gameObject, 2f);
		}
	}
}
