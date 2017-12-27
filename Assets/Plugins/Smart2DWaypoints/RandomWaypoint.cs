using UnityEngine;

namespace Assets.Plugins.Smart2DWaypoints.Scripts
{
	public class RandomWaypoint : MonoBehaviour
	{
		public const int EllipseResolution = 25;

		public float RadiusX;
		public float RadiusY;
		public float Rotation;

		public Vector3 GenerateRandomPoint()
		{
			Vector2 point = Random.insideUnitCircle;
			point.x = point.x*RadiusX;
			point.y = point.y*RadiusY;
			Quaternion q = Quaternion.AngleAxis(Rotation, Vector3.forward);
			point = q*point + transform.position;
			return point;
		}

		public Vector3[] GetEllipsePoints()
		{
			Vector3[] points = new Vector3[EllipseResolution + 1];
			UpdateEllipsePoints(points);
			return points;
		}

		public void UpdateEllipsePoints(Vector3[] ellipsePoints)
		{
			Quaternion q = Quaternion.AngleAxis(Rotation, Vector3.forward);
			for (int i = 0; i <= EllipseResolution; i++)
			{
				float angle = i/(float) EllipseResolution*2.0f*Mathf.PI;
				ellipsePoints[i] = new Vector3(RadiusX*Mathf.Cos(angle), RadiusY*Mathf.Sin(angle), 0.0f);
				ellipsePoints[i] = q*ellipsePoints[i] + transform.position;
			}
		}

	}
}
