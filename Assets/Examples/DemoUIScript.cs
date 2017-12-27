using System.Linq;
using Assets.Plugins.Smart2DWaypoints.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Plugins.Smart2DWaypoints.Demo
{
	public class DemoUIScript : MonoBehaviour
	{
		public Material LineRendererMaterial;
		public GameObject DemosHoster;
	    public GameObject[] Demos;
	    public Text TimeScale;

		public void UpdatePath(bool isShowPath)
		{
			if (isShowPath)
				ShowPath();
			else
				HidePath();
		}

		private void ShowPath()
		{
			Path[] paths = DemosHoster.GetComponentsInChildren<Path>(true);
			foreach (Path path in paths)
			{
				path.gameObject.AddComponent<PathRenderer>();
				LineRenderer lineRenderer = path.GetComponent<LineRenderer>();
				lineRenderer.material = LineRendererMaterial;
				lineRenderer.SetWidth(0.05f, 0.05f);
				lineRenderer.SetColors(Color.blue, Color.blue);
			}
		}

		private void HidePath()
		{
			Path[] paths = DemosHoster.GetComponentsInChildren<Path>(true);
			foreach (Path path in paths)
			{
				PathRenderer pathRenderer = path.GetComponent<PathRenderer>();
				Destroy(pathRenderer);
				LineRenderer lineRenderer = path.GetComponent<LineRenderer>();
				Destroy(lineRenderer);
			}
		}

		public void UpdateRandomWaypoints(bool isShow)
		{
			if (isShow)
				ShowRandomWaypoints();
			else
				HideRandomWaypoints();
		}

		private void ShowRandomWaypoints()
		{
			RandomWaypoint[] waypoints = DemosHoster.GetComponentsInChildren<RandomWaypoint>(true);
			foreach (RandomWaypoint waypoint in waypoints)
			{
				waypoint.gameObject.AddComponent<RandomWaypointRenderer>();
				LineRenderer lineRenderer = waypoint.GetComponent<LineRenderer>();
				lineRenderer.material = LineRendererMaterial;
				lineRenderer.SetWidth(0.05f, 0.05f);
				lineRenderer.SetColors(Color.green, Color.green);
			}
		}

		private void HideRandomWaypoints()
		{
			RandomWaypoint[] waypoints = DemosHoster.GetComponentsInChildren<RandomWaypoint>(true);
			foreach (RandomWaypoint waypoint in waypoints)
			{
				RandomWaypointRenderer waypointRenderer = waypoint.GetComponent<RandomWaypointRenderer>();
				Destroy(waypointRenderer);
				LineRenderer lineRenderer = waypoint.GetComponent<LineRenderer>();
				Destroy(lineRenderer);
			}
		}

	    public void ActivateDemo(int index)
	    {
	        for (int i = 0; i < Demos.Length; i++)
	        {
	            if (Demos[i].activeSelf && i == index)
	                return;

	            if (Demos[i].activeSelf)
	            {
                    Demos[i].GetComponentsInChildren<WaypointsMover>().ToList().ForEach(_ => _.Pause());
	                Demos[i].SetActive(false);
	            }
	        }

            Demos[index].SetActive(true);
	        Demos[index].GetComponentsInChildren<WaypointsMover>().ToList().ForEach(_ => _.Resume());
	    }

	    public void UpdateTimeScale(float timeScale)
	    {
	        Time.timeScale = timeScale;
	        TimeScale.text = timeScale.ToString();
	    }
    }
}
