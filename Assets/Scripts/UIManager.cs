using DootEmUp.Gameplay;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DootEmUp.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

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
            GameManager.OnWin += HandleWin;
            GameManager.OnLose += HandleLose;
            GameManager.OnPause += HandlePause;
            GameManager.OnResume += HandleResume;
        }

        private void OnDisable()
        {
            
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

        private void HandleWin()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            healthBar.SetActive(false);
            winScreen.SetActive(true);
        }

        private void HandleLose()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            healthBar.SetActive(false);
            loseScreen.SetActive(true);
        }

        private void HandlePause()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            healthBar.SetActive(false);
            pauseScreen.SetActive(true);
            compassUI.SetActive(false);
        }

        private void HandleResume()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            pauseScreen.SetActive(false);
            healthBar.SetActive(true);
            compassUI.SetActive(true);
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