using System.Collections.Generic;
using UnityEngine;

namespace Assets.Plugins.Smart2DWaypoints.Scripts
{
	public class BezierApproximation
	{
		public static Vector3 GetPoint(Vector3 point1, Vector3 handle1, Vector3 point2, Vector3 handle2, float t)
		{
			Vector3 part1 = Mathf.Pow(1 - t, 3) * point1;
			Vector3 part2 = 3 * Mathf.Pow(1 - t, 2) * t * handle1;
			Vector3 part3 = 3 * (1 - t) * Mathf.Pow(t, 2) * handle2;
			Vector3 part4 = Mathf.Pow(t, 3) * point2;
			return part1 + part2 + part3 + part4;
		}

		public static float GetLength(Vector3 point1, Vector3 handle1, Vector3 point2, Vector3 handle2, int linesCount)
		{
			return GetLength(GetLines(point1, handle1, point2, handle2, linesCount));
		}

		public static float GetLength(List<Vector3> lines)
		{
			float length = 0;
			for (int i = 0; i < lines.Count - 1; i++)
				length += (lines[i] - lines[i + 1]).magnitude;
			return length;
		}

		public static List<Vector3> GetLines(Vector3 point1, Vector3 handle1, Vector3 point2, Vector3 handle2, int count)
		{
			List<Vector3> lines = new List<Vector3>();
			lines.Add(point1);
			float dt = 1f/count;
			for (float t = dt; t < 1f - dt/2; t += dt)
				lines.Add(GetPoint(point1, handle1, point2, handle2, Mathf.Clamp01(t)));
			lines.Add(point2);
			return lines;
		}
	}
}
