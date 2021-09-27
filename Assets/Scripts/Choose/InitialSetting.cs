using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Choose
{
    public class InitialSetting : MonoBehaviour
    {
        public static int xLength = 5; //将棋盤の一辺の長さ
        public static int yLength = 3; //yが高さ
        public static int zLength = 5;

        private int gameMode = 0; //0...CPU戦, 1...対人戦

        private int audioPlaying = 0;
        public GameObject[] audioSources = new GameObject[3];
        public GameObject dropDown;

        public GameObject mainPanel;
        public GameObject settingPanel;
        public GameObject checkPlaySavedGameCanvas;

        public void OnBGMChanged()
        {
            Debug.Log("before: " + audioPlaying);
            audioSources[audioPlaying].GetComponent<AudioSource>().Stop();
            audioPlaying = dropDown.GetComponent<Dropdown>().value;
            audioSources[audioPlaying].GetComponent<AudioSource>().Play();
            Debug.Log("after: " + audioPlaying);
        }

        public void OnSettingButtonClicked()
        {
            this.settingPanel.SetActive(true);
        }
        public void OnSettingFinishButtonClicked()
        {
            this.settingPanel.SetActive(false);
        }

        public void ChooseCPU553()
        {
            xLength = 5;
            yLength = 3;
            zLength = 5;
            gameMode = 0;
            SceneManager.LoadScene("PvP553");
        }

        public void ChoosePvP553()
        {
            xLength = 5;
            yLength = 3;
            zLength = 5;
            gameMode = 1;
            if (PlayerPrefs.HasKey("savedGame")) checkPlaySavedGameCanvas.SetActive(true);
            else SceneManager.LoadScene("PvP553");
        }

        public void OnPlaySavedGameYesButton()
        {
            PlayerPrefs.SetInt("playSavedGame", 0);
            PlayerPrefs.Save();
            string scene = (gameMode == 0) ? "PvP553" : "PvP553";
            SceneManager.LoadScene(scene);
        }
        public void OnPlaySavedGameNoButton()
        {
            string scene = (gameMode == 0) ? "PvP553" : "PvP553";
            SceneManager.LoadScene(scene);
        }

        public void Start()
        {
            settingPanel.SetActive(false);
            checkPlaySavedGameCanvas.SetActive(false);
            foreach (GameObject source in audioSources)
            {
                source.transform.parent = null;
                DontDestroyOnLoad(source);
            }
        }
    }

}
