using DootEmUp.Gameplay.Enemy;
using DootEmUp.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DootEmUp.Gameplay
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager instance;

        [SerializeField]
        public List<Transform> spawnPoints;
        [SerializeField]
        private GameObject[] waveList;
        [SerializeField]
        private float waveDelay;
        [SerializeField]
        private int maxEnemies;
        private List<GameObject> enemiesList = new List<GameObject>();
        private float spawnDelay;
        private float curTimeLeft;
        private int curEnemyDooted;
        private int waveNum = 0;
        private int enemyNum = 1;
        private bool firstWave = true;
        private bool waveInProgress;
        private GameObject[] spawnList;

        private void Awake()
        {
            if (instance !=  null && instance != this)
            {
                Destroy(instance);
            }
            else
            {
                instance = this;
            }

            spawnPoints.Clear();
            spawnPoints = new List<Transform>();
        }

        private void Update()
        {
            if (!waveInProgress)
            {
                curTimeLeft -= Time.deltaTime;
                UIManager.instance.UpdateTimer(curTimeLeft);
            }
        }

        private void GameStart()
        {
            spawnList = waveList[waveNum].gameObject.GetComponent<Wave>().enemies;
            StartCoroutine(NewWave());
            UpdateWaveNumber();
            curTimeLeft = waveDelay;
        }

        private void UpdateWaveNumber()
        {
            int adjustedWavenum = waveNum + 1;
            UIManager.instance.UpdateWaveNumber(adjustedWavenum);
        }

        private void NewEnemy()
        {
            waveInProgress = true;
            curTimeLeft = waveDelay;

            UIManager.instance.StartOfRountUIUpdate();

            Debug.Log("NewEnemy called");
            int k = Random.Range(0, spawnPoints.Count);

            if (enemyNum < spawnList.Length)
            {
                StartCoroutine(SpawnEnemy(spawnPoints[k], spawnList[enemyNum]));
            }
            else if (enemyNum == spawnList.Length)
            {
                StartCoroutine(WaitUntilWaveDone());
            }
        }

        IEnumerator SpawnEnemy(Transform spawnPoint, GameObject enemy)
        {
            Debug.LogWarning("SpawnEnemy called");
            yield return new WaitForSeconds(spawnDelay);
            GameObject newEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);
            //newEnemy.GetComponent<EnemyController>().SetUpEnemy(compassUI.GetComponentInChildren<UI.Compass>());
            Debug.Log(newEnemy + " spawned");
            enemiesList.Add(newEnemy);
            enemyNum++;
            NewEnemy();
            yield break;
        }

        private void PrepareNewWave()
        {
            Debug.Log("PrepareNewWave called");
            waveInProgress = false;

            UIManager.instance.EndOfRoundUIUpdate();

            
            waveNum++;
            UpdateWaveNumber();

            if (waveNum == waveList.Length)
            {
                GameManager.instance.UpdateGameState(GameState.Win);
            }
            else
            {
                enemyNum = 0;
                spawnList = waveList[waveNum].GetComponent<Wave>().enemies;
                Debug.Log("spawn delay " + spawnDelay);
                if (GameManager.instance.curHealthItem == null)
                {
                    GameManager.instance.SpawnNewHealthItem();
                }
                StartCoroutine(NewWave());
            }
        }

        public void UpdateEnemyCounter()
        {
            if (firstWave)
            {
                curEnemyDooted++;
                UIManager.instance.UpdateEnemyCounter(curEnemyDooted, spawnList.Length - 1);
            }
            else
            {
                curEnemyDooted++;
                UIManager.instance.UpdateEnemyCounter(curEnemyDooted, spawnList.Length);
            }
        }

        IEnumerator NewWave()
        {
            Debug.Log("NewWave called");
            spawnDelay = waveList[waveNum].GetComponent<Wave>().spawnTimer;
            WaitForSeconds waitForSeconds = new WaitForSeconds(waveDelay / 2);
            yield return waitForSeconds;

            if (!firstWave)
            {
                foreach (GameObject enemy in enemiesList)
                {
                    Destroy(enemy);
                }
                enemiesList.Clear();
            }

            yield return waitForSeconds;
            curEnemyDooted = -1;
            UpdateEnemyCounter();
            NewEnemy();
            yield break;
        }

        IEnumerator WaitUntilWaveDone()
        {
            while (waveInProgress)
            {
                bool allEnemiesDead = true;
                foreach (GameObject enemy in enemiesList)
                {
                    if (!enemy.GetComponent<EnemyController>().dead)
                    {
                        allEnemiesDead = false;
                        break;
                    }
                }

                if (allEnemiesDead)
                {
                    PrepareNewWave();
                    firstWave = false;
                    break;
                }
                yield return null;
            }
        }
    }
}