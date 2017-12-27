using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Plugins.Smart2DWaypoints.Scripts
{
	public class Path : MonoBehaviour
	{
		private readonly Dictionary<WaypointsMover, List<Vector3>> _moversParts = new Dictionary<WaypointsMover, List<Vector3>>();
		private readonly Dictionary<WaypointsMover, Dictionary<int, Vector3>> _capturedPositions = new Dictionary<WaypointsMover, Dictionary<int, Vector3>>();

		public List<Vector3> BezierPoints = new List<Vector3>();
		public List<Transform> Waypoints = new List<Transform>();
		public bool IsCurved;
		public int LinesCount = 25;
		public Color Color = Color.white;
		public bool IsClosed;

		public int WaypointsCount { get { return Waypoints.Count; } }

		#region Gizmos

	    public void OnDrawGizmos()
	    {
	        Gizmos.color = Color;
	        List<Vector3> points = GetPoints();
	        for (int i = 0; i < points.Count - 1; i++)
	            Gizmos.DrawLine(points[i], points[i + 1]);

#if UNITY_EDITOR
            if (Selection.activeGameObject != gameObject)
            {
                foreach (Transform waypoint in Waypoints)
                    Gizmos.DrawSphere(waypoint.transform.position, HandleUtility.GetHandleSize(transform.position) * 0.05f);
            }
#endif
	    }

		public List<Vector3> GetPoints()
		{
			List<Vector3> points = new List<Vector3>();

			if (IsCurved)
			{
				for (int ind = 0; ind < Waypoints.Count; ind++)
				{
					if (!IsClosed && ind == Waypoints.Count - 1)
						continue;

					int nextInd = (ind + 1)%Waypoints.Count;

					List<Vector3> lines = BezierApproximation.GetLines(
						Waypoints[ind].position, GetHandle1(ind),
						Waypoints[nextInd].position, GetHandle2(nextInd), LinesCount);

					if (points.Any())
						points.RemoveAt(points.Count - 1);

					points.AddRange(lines);
				}
			}
			else
			{
				points.AddRange(Waypoints.Select(_ => _.position));
				if (IsClosed)
					points.Add(Waypoints[0].position);
			}

			return points;
		}

		#endregion

		public Vector3 GetHandle1(int ind)
		{
			return Waypoints[ind].TransformPoint(BezierPoints[ind]);
		}

		private Vector3 GetHandle1(WaypointsMover mover, int ind)
		{
			Vector3 dPos = _capturedPositions[mover][ind] - Waypoints[ind].position;
			return GetHandle1(ind) + dPos;
		}

		public void SetHandle1(int ind, Vector3 position)
		{
			BezierPoints[ind] = Waypoints[ind].InverseTransformPoint(position);
		}

		public Vector3 GetHandle2(int ind)
		{
			return 2*Waypoints[ind].position - GetHandle1(ind);
		}

		private Vector3 GetHandle2(WaypointsMover mover, int ind)
		{
			Vector3 dPos = _capturedPositions[mover][ind] - Waypoints[ind].position;
			return GetHandle2(ind) + dPos;
		}

		public Vector3 GetPoint(WaypointsMover mover, float time)
		{
			if (IsCurved)
			{
				float fullLength = BezierApproximation.GetLength(_moversParts[mover]);
				float remainedLength = fullLength*(time - 0.001f);
				for (int i = 0; i < _moversParts[mover].Count - 1; i++)
				{
					Vector3 point1 = _moversParts[mover][i];
					Vector3 point2 = _moversParts[mover][i + 1];
					float lineLength = Vector3.Distance(point1, point2);
					if (lineLength < remainedLength)
					{
						remainedLength -= lineLength;
					}
					else
					{
						return point1 + (point2 - point1).normalized*remainedLength;
					}
				}
				throw new Exception("Get point error for time:" + time + ", remainedLength: " + remainedLength);
			}
			else
			{
				Vector3 point1 = _moversParts[mover][0];
				Vector3 point2 = _moversParts[mover][1];
				float fullLength = Vector3.Distance(point1, point2);
				float remainedLength = fullLength*time;
				return point1 + (point2 - point1).normalized * remainedLength;
			}
		}

		public void StartNextPart(WaypointsMover mover, int fromInd, int toInd, bool isForwardDirection)
		{
			Vector3 toPosition = CapturePosition(mover, toInd);
			if (IsCurved)
			{
				Vector3 handle1 = isForwardDirection ? GetHandle1(mover, fromInd) : GetHandle2(mover, fromInd);
				Vector3 handle2 = isForwardDirection ? GetHandle2(mover, toInd) : GetHandle1(mover, toInd);
				_moversParts[mover] = BezierApproximation.GetLines(_capturedPositions[mover][fromInd], handle1, toPosition, handle2, LinesCount);
			}
			else
			{
				_moversParts[mover] = new List<Vector3> {_capturedPositions[mover][fromInd], toPosition};
			}

            ClearDestroyedMovers();
		}

		public float GetLength(WaypointsMover mover, int fromInd, int toInd, bool isForwardDirection)
		{
			if (IsCurved)
			{
				Vector3 handle1 = isForwardDirection ? GetHandle1(mover, fromInd) : GetHandle2(mover, fromInd);
				Vector3 handle2 = isForwardDirection ? GetHandle2(mover, toInd) : GetHandle1(mover, toInd);
				return BezierApproximation.GetLength(_capturedPositions[mover][fromInd], handle1, _capturedPositions[mover][toInd], handle2, LinesCount);
			}
			else
			{
				return Vector3.Distance(_capturedPositions[mover][fromInd], _capturedPositions[mover][toInd]);
			}
		}

		public Vector3 CapturePosition(WaypointsMover mover, int waypointIndex)
		{
			Transform waypoint = Waypoints[waypointIndex];
			RandomWaypoint randomWaypoint = waypoint.GetComponent<RandomWaypoint>();
			Vector3 position = randomWaypoint == null
				? waypoint.position
				: randomWaypoint.GenerateRandomPoint();

			if (!_capturedPositions.ContainsKey(mover))
				_capturedPositions[mover] = new Dictionary<int, Vector3>();

			_capturedPositions[mover][waypointIndex] = position;
			return position;
		}

		#region Delay

		public bool HasDelay(int waypointIndex)
		{
			return Waypoints[waypointIndex].GetComponent<DelayWaypoint>() != null;
		}

		public float GetDelay(int waypointIndex)
		{
			return Waypoints[waypointIndex].GetComponent<DelayWaypoint>().Delay;
		}

		#endregion

		public int GetIndex(Transform waypoint)
		{
			return Mathf.Max(0, Waypoints.IndexOf(waypoint));
		}

	    private void ClearDestroyedMovers()
	    {
	        foreach (WaypointsMover mover in _moversParts.Keys.ToArray())
	            if (mover == null)
	                _moversParts.Remove(mover);

	        foreach (WaypointsMover mover in _capturedPositions.Keys.ToArray())
	            if (mover == null)
	                _capturedPositions.Remove(mover);
	    }
	}
}
