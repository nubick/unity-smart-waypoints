using System.Linq;
using Assets.Plugins.Smart2DWaypoints.Scripts;
using UnityEditor;
using UnityEngine;

namespace Assets.Plugins.Editor.Smart2DWaypoints
{
    [CustomEditor(typeof(WaypointsMover))]
    public class WaypointsMoverEditor : UnityEditor.Editor
    {
        protected WaypointsMover _waypointsMover;

        public void OnEnable()
        {
            _waypointsMover = target as WaypointsMover;

            if (Mathf.Abs(WaypointsMover.InitSpeed - _waypointsMover.Speed) < 0.00001f)
            {
                _waypointsMover.Speed = 0.2f*Camera.main.orthographicSize;
            }
        }

	    public override void OnInspectorGUI()
	    {
		    EditorGUILayout.BeginVertical();

		    DrawPathSettings();
		    DrawStartWaypointSettings();

	        float newSpeed = EditorGUILayout.FloatField("Speed", _waypointsMover.Speed);
	        if (newSpeed != _waypointsMover.Speed)
	            _waypointsMover.Speed = newSpeed;

			DrawLoopTypeSettings();

		    _waypointsMover.IsAlignToDirection = EditorGUILayout.Toggle("Align to direction",
			    _waypointsMover.IsAlignToDirection);
		    if (_waypointsMover.IsAlignToDirection)
			    DrawIsAlignToDirection();

		    DrawFlip();

		    EditorGUILayout.EndVertical();
		    EditorUtility.SetDirty(_waypointsMover);
	    }

	    private void DrawPathSettings()
        {
            Path newPath = EditorGUILayout.ObjectField("Path", _waypointsMover.Path, typeof (Path), true) as Path;
            if (newPath != _waypointsMover.Path && newPath != null && newPath.Waypoints.Any())
                _waypointsMover.transform.position = newPath.Waypoints.First().position;
            _waypointsMover.Path = newPath;
        }

        private void DrawStartWaypointSettings()
        {
	        if (_waypointsMover.Path == null)
		        return;

			GUILayout.BeginHorizontal();
			string[] options = _waypointsMover.Path.Waypoints.Select(_ => _.name).ToArray();
	        int selectedIndex = _waypointsMover.Path.GetIndex(_waypointsMover.StartWaypoint);
	        int newSelectedIndex = EditorGUILayout.Popup("Start waypoint", selectedIndex, options);
	        if (newSelectedIndex != selectedIndex)
	        {
				_waypointsMover.StartWaypoint = _waypointsMover.Path.Waypoints[newSelectedIndex];
		        _waypointsMover.transform.position = _waypointsMover.StartWaypoint.position;
	        }

	        if (GUILayout.Button("#", GUILayout.Width(20f), GUILayout.Height(15f)))
	        {
				EditorGUIUtility.PingObject(_waypointsMover.StartWaypoint);
	        }
			GUILayout.EndHorizontal();
        }

	    private void DrawLoopTypeSettings()
	    {
		    if (_waypointsMover.LoopType == LoopType.OneWay)
			    GUILayout.BeginVertical("Box");

			_waypointsMover.LoopType = (LoopType)EditorGUILayout.EnumPopup("Loop type", _waypointsMover.LoopType);
		    if (_waypointsMover.LoopType == LoopType.OneWay)
		    {
			    _waypointsMover.IsDestroyWhenOneWayFinished = EditorGUILayout.Toggle(
				    "Destroy when finished", _waypointsMover.IsDestroyWhenOneWayFinished);
		    }

		    if (_waypointsMover.LoopType == LoopType.OneWay)
			    GUILayout.EndVertical();
	    }

        private void DrawFlip()
        {
            _waypointsMover.IsXFlipEnabled = EditorGUILayout.Toggle("X-Flip", _waypointsMover.IsXFlipEnabled);
            _waypointsMover.IsYFlipEnabled = EditorGUILayout.Toggle("Y-Flip", _waypointsMover.IsYFlipEnabled);
        }

		protected virtual void DrawIsAlignToDirection()
		{
			_waypointsMover.RotationOffset = EditorGUILayout.FloatField("Rotation offset", _waypointsMover.RotationOffset);
		}

    }
}
