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

        private int audioPlaying;

        static AudioPlayer prevAudioPlayer = null;
        public AudioPlayer audioPlayer;
        public Slider bgmSlider;
        public Slider seSlider;
        public GameObject dropDown;

        public GameObject mainPanel;
        public GameObject settingPanel;
        public GameObject checkPlaySavedGameCanvas;

        public void OnBGMChanged()
        {
            int val = dropDown.GetComponent<Dropdown>().value;
            if (PlayerPrefs.GetInt("BGMid", -1) == val) return;
            Debug.Log("before: " + audioPlaying);
            audioPlayer.stop();
            audioPlaying = val;
            audioPlayer.play(audioPlaying);
            Debug.Log("after: " + audioPlaying);
        }

        public void OnCreditButtonClicked()
        {

        }

        public void OnOthelloButtonClicked()
        {
            Application.OpenURL("https://unityroom.com/games/3dothello_demo");
        }

        public void OnSettingButtonClicked()
        {
            this.settingPanel.SetActive(true);
        }
        public void OnSettingFinishButtonClicked()
        {
            PlayerPrefs.SetInt("BGMid", dropDown.GetComponent<Dropdown>().value);
            PlayerPrefs.SetFloat("BGMVolume", bgmSlider.GetComponent<Slider>().value);
            PlayerPrefs.SetFloat("SEVolume", seSlider.GetComponent<Slider>().value);
            PlayerPrefs.Save();
            this.settingPanel.SetActive(false);
        }

        public void OnTutorialButtonClicked()
        {

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
            audioPlaying = PlayerPrefs.GetInt("BGMid", 0);
            float defaultBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
            float defaultSEVolume = PlayerPrefs.GetFloat("SEVolume", 0.5f);
            bgmSlider.GetComponent<Slider>().value = defaultBGMVolume;
            seSlider.GetComponent<Slider>().value = defaultSEVolume;
            if (prevAudioPlayer == null)
            {
                prevAudioPlayer = audioPlayer;
                audioPlayer.play(audioPlaying);
            }
            else
            {
                Destroy(audioPlayer.gameObject);
                audioPlayer = prevAudioPlayer;
            }
            audioPlayer.GetComponent<AudioSource>().volume = defaultBGMVolume;
            dropDown.GetComponent<Dropdown>().value = PlayerPrefs.GetInt("BGMid", 0);
        }
        void Update()
        {
            audioPlayer.GetComponent<AudioSource>().volume = bgmSlider.GetComponent<Slider>().value;
        }
    }

}
