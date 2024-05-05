using Enemy;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Transform[] spawnPoints;
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
    private GameObject healthItem;
    [SerializeField]
    private Transform healthItemSpawn;

    [SerializeField]
    private TMP_Text waveNumber;
    [SerializeField]
    private TMP_Text enemyCounter;
    [SerializeField]
    private TMP_Text waveCountdown;
    [SerializeField]
    private GameObject healthBar;

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

    private void Start()
    {
        Time.timeScale = 1;
        loseScreen.SetActive(false);
        winScreen.SetActive(false);
        pauseScreen.SetActive(false);
        enemyCounter.gameObject.SetActive(false);
        healthBar.SetActive(true);
        spawnList = waveList[waveNum].gameObject.GetComponent<Wave>().enemies;
        StartCoroutine(NewWave());
        UpdateWaveNumber();
        curTimeLeft = waveDelay;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
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
        int k = Random.Range(0, spawnPoints.Length);
        
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
        Debug.Log("SpawnEnemy called");
        yield return new WaitForSeconds(spawnDelay);
        GameObject newEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);
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
        WaitForSeconds waitForSeconds = new WaitForSeconds(waveDelay/2);
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
            foreach(GameObject enemy in enemiesList)
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

    public void LoseGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        healthBar.SetActive(false);
        loseScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        healthBar.SetActive(false);
        pauseScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseScreen.SetActive(false);
        healthBar.SetActive(true);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ExitGame()
    {
        Application.Quit();
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
}
