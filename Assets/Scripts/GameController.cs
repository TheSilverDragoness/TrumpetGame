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
        private GameObject healthItem;
        [HideInInspector]
        public Transform healthItemSpawn;

        public bool gameStarted;

        [HideInInspector]
        public GameObject curHealthItem {  get; private set; }

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
        }

        public void GameStart()
        {
            gameStarted = true; //Handled by GameState
            Time.timeScale = 1;
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

        public void SpawnNewHealthItem()
        {
            curHealthItem = Instantiate(healthItem, healthItemSpawn.position, Quaternion.identity, healthItemSpawn);
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
                case GameState.MainMenu:
                    SceneManager.LoadScene("MainMenu");
                    break;
                case GameState.GenerateLevel:
                    SceneManager.LoadScene("PCGScene");
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
        MainMenu,
        GenerateLevel,
        Play,
        Pause,
        Win,
        Lose,
        Quit
    }
}