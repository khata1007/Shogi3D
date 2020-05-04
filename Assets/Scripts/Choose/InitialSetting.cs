using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Choose
{
    public class InitialSetting : MonoBehaviour
    {
        public static int xLength; //将棋盤の一辺の長さ
        public static int yLength; //yが高さ
        public static int zLength;


        public void ChoosePvP993()
        {
            xLength = 9;
            yLength = 3;
            zLength = 9;
            SceneManager.LoadScene("PvP993");
        }

        public void ChooseCPU993()
        {
            xLength = 9;
            yLength = 3;
            zLength = 9;
            SceneManager.LoadScene("CPU993");
        }
    }

}
