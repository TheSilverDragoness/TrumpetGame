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

        [SerializeField]
        public List<Transform> spawnPoints;
        [SerializeField]
        private GameObject[] waveList;
        [SerializeField]
        private float waveDelay;

        [SerializeField]
        private GameObject loseScreen;
        [SerializeField]
        private GameObject winScreen;
        [SerializeField]
        private GameObject pauseScreen;
        [SerializeField]
        private GameObject attributionsScreen;
        [SerializeField]
        private GameObject helpScreen;

        [SerializeField]
        private GameObject healthItem;
        [HideInInspector]
        public Transform healthItemSpawn;

        [SerializeField]
        private TMP_Text waveNumber;
        [SerializeField]
        private TMP_Text enemyCounter;
        [SerializeField]
        private TMP_Text waveCountdown;
        [SerializeField]
        private GameObject healthBar;
        [SerializeField]
        private GameObject compassUI;

        [SerializeField]
        private GameObject gameUI;

        [SerializeField]
        private int maxEnemies;

        private GameObject curHealthItem;
        private GameObject[] spawnList;

        private List<GameObject> enemiesList = new List<GameObject>();
        private float spawnDelay;
        private float curTimeLeft;
        private int curEnemyDooted;
        private int waveNum = 0;
        private int enemyNum = 1;
        private bool firstWave = true;
        private bool waveInProgress;
        public bool gameStarted;

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

            gameStarted = false;
            gameUI.SetActive(false);
            spawnPoints.Clear();
            spawnPoints = new List<Transform>();
        }

        private void OnEnable()
        {
            Health.onPlayerDeath += LoseGame;
        }

        private void OnDisable()
        {
            Health.onPlayerDeath -= LoseGame;
        }

        public void GameStart()
        {
            gameStarted = true;
            gameUI.SetActive(true);
            Time.timeScale = 1;
            loseScreen.SetActive(false);
            winScreen.SetActive(false);
            pauseScreen.SetActive(false);
            attributionsScreen.SetActive(false);
            helpScreen.SetActive(false);
            enemyCounter.gameObject.SetActive(false);
            healthBar.SetActive(true);
            spawnList = waveList[waveNum].gameObject.GetComponent<Wave>().enemies;
            StartCoroutine(NewWave());
            UpdateWaveNumber();
            curTimeLeft = waveDelay;
        }

        private void Update()
        {
            if (!gameStarted)
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

        IEnumerator SpawnEnemy(Transform spawnPoint, GameObject enemy)
        {
            Debug.LogWarning("SpawnEnemy called");
            yield return new WaitForSeconds(spawnDelay);
            GameObject newEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);
            newEnemy.GetComponent<EnemyController>().SetUpEnemy(compassUI.GetComponentInChildren<UI.Compass>());
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

        IEnumerator WinGame()
        {
            yield return new WaitForSeconds(2);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            healthBar.SetActive(false);
            winScreen.SetActive(true);
            Time.timeScale = 0;
            yield break;
        }

        private void LoseGame()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            healthBar.SetActive(false);
            loseScreen.SetActive(true);
            Time.timeScale = 0;
        }

        private void Pause()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            healthBar.SetActive(false);
            pauseScreen.SetActive(true);
            compassUI.SetActive(false);
            Time.timeScale = 0;
        }

        private void Resume()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            pauseScreen.SetActive(false);
            healthBar.SetActive(true);
            compassUI.SetActive(true);
            Time.timeScale = 1;
        }

        private void Restart()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        public void OpenAttributions()
        {
            pauseScreen.SetActive(false);
            attributionsScreen.SetActive(true);
        }

        public void CloseAttributions()
        {
            attributionsScreen.SetActive(false);
            pauseScreen.SetActive(true);
        }

        public void OpenHelp()
        {
            pauseScreen.SetActive(false);
            helpScreen.SetActive(true);
        }

        public void CloseHelp()
        {
            helpScreen.SetActive(false);
            pauseScreen.SetActive(true);
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
                    StartCoroutine(WinGame());
                    break;
                case GameState.Lose:
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