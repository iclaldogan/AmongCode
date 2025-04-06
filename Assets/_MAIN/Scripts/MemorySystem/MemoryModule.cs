using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;

public class MemoryModule : MonoBehaviour
{
    [System.Serializable]
    public class MemoryEntry
    {
        public string agentName;
        public string location;
        public float timestamp;

        public MemoryEntry(string name, string room, float time)
        {
            agentName = name;
            location = room;
            timestamp = time;
        }
    }

    public List<MemoryEntry> memoryLog = new List<MemoryEntry>();

    [Header("Raycasting")]
    public float rayLength = 10f;
    public LayerMask visionMask;

    [Header("Debugging")]
    public bool debugDrawRays = true;

    public void ClearMemory()
    {
        memoryLog.Clear();
    }

    public void CollectVisionObservations(VectorSensor sensor, Transform origin)
    {
        Vector3[] directions = new Vector3[]
        {
        origin.forward,
        -origin.forward,
        origin.right,
        -origin.right,
        (origin.forward + origin.right).normalized,
        (origin.forward - origin.right).normalized,
        (-origin.forward + origin.right).normalized,
        (-origin.forward - origin.right).normalized
        };

        foreach (var dir in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(origin.position, dir, out hit, rayLength, visionMask))
            {
                float distance = hit.distance / rayLength; // Normalize [0,1]
                string tag = hit.collider.tag;

                float isPlayer = (tag == "crewmate" || tag == "imposter") ? 1f : 0f;

                sensor.AddObservation(distance);
                sensor.AddObservation(isPlayer);

                if (debugDrawRays)
                    Debug.DrawRay(origin.position, dir * hit.distance, Color.green);
            }
            else
            {
                sensor.AddObservation(1f); // max distance
                sensor.AddObservation(0f); // no player
                if (debugDrawRays)
                    Debug.DrawRay(origin.position, dir * rayLength, Color.red);
            }
        }
    }



    public void RecordRoomEntry(Collider roomOrCorridor)
    {
        // Optional: for remembering where Alfred went
        memoryLog.Add(new MemoryEntry("self", roomOrCorridor.name, Time.time));
    }

    private string GetCurrentRoomName(Vector3 position)
    {
        // Optional helper — returns closest room or collider
        Collider[] hits = Physics.OverlapSphere(position, 1f);
        foreach (var col in hits)
        {
            if (col.CompareTag("room") || col.CompareTag("corridor"))
                return col.name;
        }
        return "Unknown";
    }
}
