using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [Header("Target Settings")]
    public GameObject targetPrefab;
    public int targetCount = 3;
    public Transform environmentRoot;
    public List<Collider> spawnZones = new List<Collider>();

    public List<GameObject> spawnedTargets = new List<GameObject>();

    public int TargetCount => targetCount;
    public List<GameObject> SpawnedTargets => spawnedTargets;

    public void SpawnTargets()
    {
        RemoveAllTargets();

        for (int i = 0; i < targetCount; i++)
        {
            GameObject newTarget = Instantiate(targetPrefab);
            newTarget.transform.parent = environmentRoot;

            Vector3 spawnPos = GetValidSpawnLocation();
            newTarget.transform.position = spawnPos;

            spawnedTargets.Add(newTarget);
            Debug.Log($"🎯 Target {i + 1} spawned at {spawnPos}");
        }
    }

    private Vector3 GetValidSpawnLocation()
    {
        int maxAttempts = 10;
        Vector3 result = Vector3.zero;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Collider zone = spawnZones[Random.Range(0, spawnZones.Count)];
            Bounds bounds = zone.bounds;

            Vector3 candidate = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                0.3f,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (IsFarEnough(candidate))
                return candidate;
        }

        return new Vector3(0, 0.3f, 0); // fallback
    }

    private bool IsFarEnough(Vector3 pos)
    {
        foreach (GameObject target in spawnedTargets)
        {
            if (target == null) continue;
            if (Vector3.Distance(pos, target.transform.position) < 5f)
                return false;
        }

        GameObject[] agents = GameObject.FindGameObjectsWithTag("crewmate");
        foreach (GameObject agent in agents)
        {
            if (Vector3.Distance(pos, agent.transform.position) < 5f)
                return false;
        }

        return true;
    }

    public void RemoveAllTargets()
    {
        foreach (GameObject t in spawnedTargets)
        {
            if (t != null) Destroy(t);
        }
        spawnedTargets.Clear();
    }

    public void RemoveTargetFromList(GameObject target)
    {
        spawnedTargets.Remove(target);
    }

    public bool AllTargetsCollected()
    {
        return spawnedTargets.Count == 0;
    }
}
