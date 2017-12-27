using Assets.Plugins.Smart2DWaypoints.Scripts;
using UnityEditor;

namespace Assets.Plugins.Editor.Smart2DWaypoints
{
    [CustomEditor(typeof(RigidbodyMover))]
    public class RigidbodyMoverEditor : WaypointsMoverEditor
    {
		protected override void DrawIsAlignToDirection ()
		{
			base.DrawIsAlignToDirection ();

			RigidbodyMover rigidbodyMover = _waypointsMover as RigidbodyMover;
			rigidbodyMover.RotationSpeed = EditorGUILayout.FloatField("Rotation speed", rigidbodyMover.RotationSpeed);
		}
    }
}
