using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using DootEmUp.Gameplay;
using DootEmUp.Gameplay.Player;
using DootEmUp.UI;

namespace DootEmUp.PCG
{
    public class GenerateMesh : MonoBehaviour
    {
        [SerializeField]
        private GameObject wallPrefab;
        [SerializeField]
        private GameObject floorPrefab;
        [SerializeField]
        private Material wallMaterial;
        [SerializeField]
        private Material floorMaterial;
        [SerializeField]
        private NavMeshSurface navSurface;
        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private GameObject spawnObstacle;
        [SerializeField]
        private GameObject enemySpawn;
        [SerializeField]
        private GameObject compassUI;
        [SerializeField]
        private GameObject healthBar;
        [SerializeField]
        private GameObject mouseSensitivitySlider;
        [SerializeField]
        private float cellSize;

        private List<Transform> enemySpawnPoints = new List<Transform>();

        private PlayerController playerController;

        private GameObject levelGeometry = null;
        private GameObject walls = null;
        private GameObject floor = null;
        private GameObject playerSpawn = null;


        public void GenerateWalls(Grid grid)
        {
            SetUpLevel();

            for (int x = 0; x < grid.grid.GetLength(0); x++)
            {
                for (int z = 0; z < grid.grid.GetLength(1); z++)
                {
                    if (grid.grid[x, z].type == 0)
                    {
                        continue;
                    }
                    if (grid.grid[x, z].type > 0)
                    {
                        CreateFloor(x, z);
                    }
                    if ((z + 1 < grid.grid.GetLength(1) && grid.grid[x, z + 1].type == 0) || z + 1 == grid.grid.GetLength(1))
                    {
                        if (grid.grid[x, z].type == 2)
                        {
                            CreateEnemySpawn(x, z, Vector3.forward, 270);
                        }
                        else
                        {
                            CreateWall(x, z, Vector3.forward, 90);
                        }
                    }
                    if ((z - 1 >= 0 && grid.grid[x, z - 1].type == 0) || z == 0)
                    {
                        if (grid.grid[x, z].type == 2)
                        {
                            CreateEnemySpawn(x, z, Vector3.back, 90);
                        }
                        else
                        {
                            CreateWall(x, z, Vector3.back, 90);
                        }
                    }
                    if ((x + 1 < grid.grid.GetLength(1) && grid.grid[x + 1, z].type == 0) || x + 1 == grid.grid.GetLength(1))
                    {
                        if (grid.grid[x, z].type == 2)
                        {
                            CreateEnemySpawn(x, z, Vector3.right, 0);
                        }
                        else
                        {
                            CreateWall(x + 1, z, Vector3.left, 0);
                        }
                    }
                    if ((x - 1 >= 0 && grid.grid[x - 1, z].type == 0) || x == 0)
                    {
                        if (grid.grid[x, z].type == 2)
                        {
                            CreateEnemySpawn(x, z, Vector3.left, 180);
                        }
                        else
                        {
                            CreateWall(x - 1, z, Vector3.right, 0);
                        }
                    }
                }
            }
        }

        public void SetUpLevel()
        {
            Destroy(levelGeometry);
            levelGeometry = null;

            levelGeometry = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, null);
            levelGeometry.name = "LevelGeometry";

            walls = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, levelGeometry.transform);
            walls.name = "Walls";

            floor = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, levelGeometry.transform);
            floor.name = "Floor";
        }

        private void CreateWall(int x, int z, Vector3 direction, int rotation)
        {
            Vector3 wallRotation = new Vector3(0, rotation, 0);
            Vector3 wallPosition = new Vector3(x * cellSize, 0, z * cellSize) + direction * (cellSize / 2);
            GameObject spawnedWall = Instantiate(wallPrefab, wallPosition, Quaternion.Euler(wallRotation), walls.transform);
            spawnedWall.GetComponent<MeshRenderer>().material = wallMaterial;
            spawnedWall.transform.localScale = new Vector3(0.05f, cellSize + 1.5f, cellSize);
        }

        private void CreateFloor(int x, int z)
        {
            Vector3 floorPosition = new Vector3(x * cellSize, -0.5f, z * cellSize);
            GameObject spawnedFloor = Instantiate(floorPrefab, floorPosition, Quaternion.identity, floor.transform);
            spawnedFloor.GetComponent<MeshRenderer>().material = floorMaterial;
            spawnedFloor.transform.localScale = new Vector3(cellSize, 0.05f, cellSize);
        }

        private void CreateEnemySpawn(int x, int z, Vector3 direction, int rotation)
        {
            Vector3 enemySpawnRotation = new Vector3(0, rotation, 0);
            Vector3 enemySpawnPosition = new Vector3(x * cellSize, -0.5f, z * cellSize) + direction * (cellSize);
            GameObject spawnedSpawnPoint = Instantiate(enemySpawn, enemySpawnPosition, Quaternion.Euler(enemySpawnRotation));
            enemySpawnPoints.Add(spawnedSpawnPoint.transform.Find("SpawnPoint"));
        }

        public void CreatePlayerSpawn(int x, int z)
        {
            Vector3 playerSpawnPosition = new Vector3(x * cellSize, 1.5f, z * cellSize);
            playerSpawn = Instantiate(new GameObject(), playerSpawnPosition, Quaternion.identity);
            playerSpawn.name = "PlayerSpawn";
            GameManager.instance.healthItemSpawn = playerSpawn.transform;
            WaveManager.instance.spawnPoints = enemySpawnPoints;
        }

        public void CreateSpawnRoomObstacle(int x, int z)
        {
            Vector3 obstacleSpawnPosition = new Vector3(x * cellSize, -0.25f, z * cellSize);
            GameObject obstacle = Instantiate(spawnObstacle, obstacleSpawnPosition, Quaternion.identity);
            obstacle.name = "SpawnObstacle";
        }

        public void CreateNavMesh(int size)
        {
            navSurface.transform.localScale = new Vector3(size * cellSize, 2, size * cellSize);
            navSurface.BuildNavMesh();
        }

        public void SpawnPlayer()
        {
            GameObject player = Instantiate(playerPrefab, playerSpawn.transform.position, Quaternion.identity);
            playerController = player.GetComponent<PlayerController>();
            compassUI.GetComponentInChildren<DootEmUp.UI.Compass>().AssignPlayer(player.transform);
        }

        public void SetUpScene()
        {

        }
    }
}

