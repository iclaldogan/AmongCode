using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;

public class MemoryModule : MonoBehaviour
{
    private Dictionary<string, float> lastSeenTimestamps = new Dictionary<string, float>();
    public float tagMemoryCooldown = 10f; // seconds

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

                // For observation input
                float isPlayer = (tag == "crewmate" || tag == "imposter") ? 1f : 0f;
                sensor.AddObservation(distance);
                sensor.AddObservation(isPlayer);

                // Record memory for important tags only
                // Record memory for important tags only
                if (tag == "crewmate" || tag == "imposter" || tag == "button" || tag == "deadbody")
                {
                    if (!lastSeenTimestamps.ContainsKey(tag) || Time.time - lastSeenTimestamps[tag] > tagMemoryCooldown)
                    {
                        memoryLog.Add(new MemoryEntry("self", $"Seen: {tag}", Time.time));
                        lastSeenTimestamps[tag] = Time.time;
                    }
                }


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

        // Optional: keep memory log size under control
        if (memoryLog.Count > 200)
            memoryLog.RemoveAt(0);
    }


    public void RecordRoomEntry(Collider roomOrCorridor)
    {
        memoryLog.Add(new MemoryEntry("self", roomOrCorridor.name, Time.time));
    }

    public void RecordSightedObject(string tagName, float distance)
    {
        memoryLog.Add(new MemoryEntry("self", $"Seen: {tagName}", Time.time));
    }


    public string GetCurrentRoomName(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, 1f);
        foreach (var col in hits)
        {
            if (col.CompareTag("room") || col.CompareTag("corridor"))
                return col.name;
        }
        return "Unknown";
    }

    // MemoryStats method for HUD or debug logs
    public int MemoryStats()
    {
        return memoryLog.Count;
    }
}
