using System.Collections.Generic;
using System.Linq;
using Assets.Plugins.Smart2DWaypoints.Scripts;
using UnityEngine;

namespace Assets.Test
{
    public class PauseTestManager : MonoBehaviour
    {
        public List<WaypointsMover> Movers;

        public void OnGUI()
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("Pause Random"))
            {
                WaypointsMover[] movers = FindObjectsOfType<WaypointsMover>();
                movers[Random.Range(0, movers.Length)].Pause();
            }

            if (GUILayout.Button("Pause All"))
            {
                FindObjectsOfType<WaypointsMover>().ToList().ForEach(_ => _.Pause());
            }

            if (GUILayout.Button("Resume Random"))
            {
                WaypointsMover[] movers = FindObjectsOfType<WaypointsMover>().Where(_ => _.IsPaused()).ToArray();
                if (movers.Any())
                    movers[Random.Range(0, movers.Length)].Resume();
            }

            if (GUILayout.Button("Resume All"))
            {
                FindObjectsOfType<WaypointsMover>().ToList().ForEach(_ => _.Resume());
            }



            GUILayout.EndVertical();
        }
    }
}
