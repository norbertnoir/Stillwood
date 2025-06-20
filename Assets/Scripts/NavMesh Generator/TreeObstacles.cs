using UnityEngine;
using UnityEngine.AI;

public class TreeObstacles : MonoBehaviour
{
    public GameObject container;
    public Terrain terrain;

    public Vector3 obstacleSize = new Vector3(1.5f, 5f, 1.5f); // dopasuj do rozmiaru drzewa

    void Start()
    {
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        foreach (var tree in data.treeInstances)
        {
            Vector3 worldPos = Vector3.Scale(tree.position, data.size) + terrainPos;

            GameObject obstacle = new GameObject("TreeObstacle");
            obstacle.transform.position = worldPos;

            obstacle.transform.SetParent(container.transform);
            BoxCollider col = obstacle.AddComponent<BoxCollider>();
            col.size = obstacleSize;

            NavMeshObstacle navObs = obstacle.AddComponent<NavMeshObstacle>();
            navObs.carving = true;
        }
    }
}