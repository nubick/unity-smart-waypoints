using System.Collections.Generic;
using UnityEngine;

namespace Assets.Plugins.Smart2DWaypoints.Scripts
{
	[RequireComponent(typeof(Path))]
	[RequireComponent(typeof(LineRenderer))]
	public class PathRenderer : MonoBehaviour
	{
		private Path _path;
		private LineRenderer _lineRenderer;
		private List<Vector3> _points;

		public void Awake()
		{
			_path = GetComponent<Path>();
			_lineRenderer = GetComponent<LineRenderer>();
			_lineRenderer.sortingOrder = 10;
			_points = _path.GetPoints();
			_lineRenderer.SetVertexCount(_points.Count);
			for (int i = 0; i < _points.Count; i++)
				_lineRenderer.SetPosition(i, _points[i]);
		}
	}
}
