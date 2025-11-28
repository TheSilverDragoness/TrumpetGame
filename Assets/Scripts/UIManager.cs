using DootEmUp.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace DootEmUp.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        [SerializeField]
        private GameObject mainMenu;
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
        private TMP_Text waveNumberText;
        [SerializeField]
        private TMP_Text enemyCounter;
        [SerializeField]
        private TMP_Text waveCountdown;

        //HUD Manager?
        [SerializeField]
        private GameObject healthBar;
        [SerializeField]
        private GameObject compassUI;
        [SerializeField]
        private GameObject gameUI;

        private void OnEnable()
        {
            GameManager.OnGameStateExit += ExitState;
            GameManager.OnGameStateEnter += EnterState;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateExit -= ExitState;
            GameManager.OnGameStateEnter -= EnterState;
        }

        private void EnterState(GameState newState)
        {
            switch (newState)
            {
                case GameState.MainMenu:
                    EnterMainMenu();
                    break;
                case GameState.GenerateLevel:
                    EnterPCG();
                    break;
                case GameState.Play:
                    EnterPlay();
                    break;
                case GameState.Pause:
                    EnterPause();
                    break;
                case GameState.Win:
                    EnterWin();
                    break;
                case GameState.Lose:
                    EnterLose();
                    break;
                case GameState.Quit:
                    //Disable
                    break;
            }
        }

        private void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    ExitMainMenu();
                    break;
                case GameState.GenerateLevel:
                    ExitPCG();
                    break;
                case GameState.Play:
                    ExitPlay();
                    break;
                case GameState.Pause:
                    ExitPause();
                    break;
                case GameState.Win:
                    ExitWin();
                    break;
                case GameState.Lose:
                    ExitLose();
                    break;
                case GameState.Quit:
                    //Disable
                    break;
            }
        }

        private void EnterMainMenu()
        {
            GameStart();
        }

        private void ExitMainMenu()
        {
            //throw new NotImplementedException();
        }

        private void EnterPCG()
        {
            throw new NotImplementedException();
        }

        private void ExitPCG()
        {
            throw new NotImplementedException();
        }

        private void EnterPlay()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            healthBar.SetActive(true);
            compassUI.SetActive(true);
        }

        private void ExitPlay()
        {
            healthBar.SetActive(false);
            compassUI.SetActive(false);
        }

        private void EnterPause()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            pauseScreen.SetActive(true);
        }

        private void ExitPause()
        {
            pauseScreen.SetActive(false);
        }

        private void EnterWin()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            winScreen.SetActive(true);
        }

        private void ExitWin()
        {
            winScreen.SetActive(false);
        }

        private void EnterLose()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            loseScreen.SetActive(true);
        }

        private void ExitLose()
        {
            loseScreen.SetActive(false);
        }

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

            gameUI.SetActive(false);
        }

        private void GameStart()
        {
            loseScreen.SetActive(false);
            winScreen.SetActive(false);
            pauseScreen.SetActive(false);
            attributionsScreen.SetActive(false);
            helpScreen.SetActive(false);
            gameUI.SetActive(true);
            healthBar.SetActive(true);
            enemyCounter.gameObject.SetActive(false);
            mainMenu.SetActive(true);
        }

        public void UpdateTimer(float timeLeft)
        {
            timeLeft = Mathf.Round(timeLeft);
            waveCountdown.text = "Time until next wave: " + timeLeft.ToString();
        }

        public void UpdateWaveNumber(int waveNumber)
        {
            waveNumberText.text = "Wave " + waveNumber;
        }

        public void UpdateEnemyCounter(int enemiesDooted, int totalEnemies)
        {
            enemyCounter.text = "Enemies Dooted: " + enemiesDooted.ToString() + "/" + totalEnemies.ToString();
        }

        public void StartOfRountUIUpdate()
        {
            waveCountdown.gameObject.SetActive(false);
            enemyCounter.gameObject.SetActive(true);
        }

        public void EndOfRoundUIUpdate()
        {
            waveCountdown.gameObject.SetActive(true);
            enemyCounter.gameObject.SetActive(false);
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
    }
}