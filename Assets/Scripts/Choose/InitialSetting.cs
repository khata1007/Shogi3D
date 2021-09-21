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

        private int audioPlaying = 0;
        public GameObject[] audioSources = new GameObject[3];
        public GameObject dropDown;

        public GameObject MainPanel;
        public GameObject SettingPanel;

        public void OnBGMChanged()
        {
            Debug.Log("before: " + audioPlaying);
            audioSources[audioPlaying].GetComponent<AudioSource>().Stop();
            audioPlaying = dropDown.GetComponent<Dropdown>().value;
            audioSources[audioPlaying].GetComponent<AudioSource>().Play();
            Debug.Log("after: " + audioPlaying);
        }

        public void OnSettingButtonClicked(){
            this.SettingPanel.SetActive(true);
        }
        public void OnSettingFinishButtonClicked(){
            this.SettingPanel.SetActive(false);
        }

        public void ChoosePvP993()
        {
            xLength = 9;
            yLength = 3;
            zLength = 9;
            SceneManager.LoadScene("PvP993");
        }

        public void ChoosePvP553()
        {
            xLength = 5;
            yLength = 3;
            zLength = 5;
            SceneManager.LoadScene("PvP553");
        }

        public void Start()
        {
            foreach(GameObject source in audioSources){
                source.transform.parent = null;
                DontDestroyOnLoad(source);
            }
        }
    }

}
