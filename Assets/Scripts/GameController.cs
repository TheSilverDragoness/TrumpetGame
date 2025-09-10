using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DootEmUp.Gameplay.Player;
using DootEmUp.Gameplay.Enemy;
using System;
using Random = UnityEngine.Random;

namespace DootEmUp.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public GameState state { get; private set; }

        public static event Action<GameState> OnGameStateChange;
        public static event Action OnWin;
        public static event Action OnLose;
        public static event Action OnPause;
        public static event Action OnResume;

        [SerializeField]
        public List<Transform> spawnPoints;
        [SerializeField]
        private GameObject[] waveList;
        [SerializeField]
        private float waveDelay;

        [SerializeField]
        private GameObject healthItem;
        [HideInInspector]
        public Transform healthItemSpawn;

        //Wave Manager
        [SerializeField]
        private TMP_Text waveNumber;
        [SerializeField]
        private TMP_Text enemyCounter;
        [SerializeField]
        private TMP_Text waveCountdown;
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
        public bool gameStarted; //Handled by GameState

        private GameObject curHealthItem;
        private GameObject[] spawnList;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(instance);
            }
            else
            {
                instance = this;
            }

            gameStarted = false; //Handled by GameState
            spawnPoints.Clear();
            spawnPoints = new List<Transform>();
        }

        public void GameStart()
        {
            gameStarted = true; //Handled by GameState
            Time.timeScale = 1;
            enemyCounter.gameObject.SetActive(false);
            spawnList = waveList[waveNum].gameObject.GetComponent<Wave>().enemies;
            StartCoroutine(NewWave());
            UpdateWaveNumber();
            curTimeLeft = waveDelay;
        }

        private void Update()
        {
            if (!gameStarted) //Handled by GameState
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Escape) && state != GameState.Pause)
            {
                UpdateGameState(GameState.Pause);
            }
            if (!waveInProgress)
            {
                curTimeLeft -= Time.deltaTime;
                UpdateTimer(curTimeLeft);
            }
        }

        //Move to Wave Manager
        private void NewEnemy()
        {
            waveInProgress = true;
            curTimeLeft = waveDelay;
            waveCountdown.gameObject.SetActive(false);
            enemyCounter.gameObject.SetActive(true);
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

        //Move to Wave Manager
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

        //Move to Wave Manager
        private void PrepareNewWave()
        {
            Debug.Log("PrepareNewWave called");
            waveInProgress = false;
            waveCountdown.gameObject.SetActive(true);
            enemyCounter.gameObject.SetActive(false);
            waveNum++;
            UpdateWaveNumber();

            if (waveNum == waveList.Length)
            {
                StartCoroutine(WinGame());
            }
            else
            {
                enemyNum = 0;
                spawnList = waveList[waveNum].GetComponent<Wave>().enemies;
                Debug.Log("spawn delay " + spawnDelay);
                if (curHealthItem == null)
                {
                    curHealthItem = Instantiate(healthItem, healthItemSpawn.position, Quaternion.identity, healthItemSpawn);
                }
                StartCoroutine(NewWave());
            }
        }

        //Move to Wave Manager
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

        //Move to Wave Manager
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
        private void Win()
        {
            OnWin?.Invoke();
            StartCoroutine(WinGame());
        }

        IEnumerator WinGame()
        {
            yield return new WaitForSeconds(2);
            Time.timeScale = 0;
            yield break;
        }

        private void LoseGame()
        {
            Time.timeScale = 0;
        }

        private void Pause()
        {
            OnPause!.Invoke();
            Time.timeScale = 0;
        }

        private void Resume()
        {
            OnResume?.Invoke();
            Time.timeScale = 1;
        }

        private void Restart()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        private void UpdateTimer(float timeLeft)
        {
            timeLeft = Mathf.Round(timeLeft);
            waveCountdown.text = "Time until next wave: " + timeLeft.ToString();
        }

        private void UpdateWaveNumber()
        {
            int adjustedWavenum = waveNum + 1;
            waveNumber.text = "Wave " + adjustedWavenum;
        }

        public void UpdateEnemyCounter()
        {
            if (firstWave)
            {
                curEnemyDooted++;
                enemyCounter.text = "Enemies Dooted: " + curEnemyDooted.ToString() + "/" + (spawnList.Length - 1).ToString();
            }
            else
            {
                curEnemyDooted++;
                enemyCounter.text = "Enemies Dooted: " + curEnemyDooted.ToString() + "/" + spawnList.Length.ToString();
            }
        }

        public void SetPlayerCameraSensitivity(float sensitivity)
        {
            FindObjectOfType<PlayerCamera>().SetMouseSensitivity(sensitivity);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void UpdateGameState(GameState newState)
        {
            state = newState;

            switch (newState)
            {
                case GameState.GenerateLevel:
                    break;
                case GameState.Play:
                    Resume();
                    break;
                case GameState.Pause:
                    Pause();
                    break;
                case GameState.Win:
                    Win();
                    break;
                case GameState.Lose:
                    OnLose?.Invoke();
                    LoseGame();
                    break;
                case GameState.Quit:
                    ExitGame();
                    break;
                default:
                    Debug.Log("Update Game State Error: Game State Not Recognised");
                    break;
            }

            OnGameStateChange?.Invoke(newState);
        }
    }

    public enum GameState
    {
        GenerateLevel,
        Play,
        Pause,
        Win,
        Lose,
        Quit
    }
}