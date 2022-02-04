using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace PvP553
{
    public class MouseDetector : MonoBehaviour
    {
        private int xLength = Choose.InitialSetting.xLength; //オセロ盤の一辺の長さ
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;

        public Canvas saveConfirmCanvas;
        public Game game;
        // Start is called before the first frame update
        void Start()
        {
            saveConfirmCanvas.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnCancelGridChooseButtonClick()
        {
            Debug.Log("cancelled");
            game.UnActivateChoosingGrid();
        }

        public void OnNariButtonClick(int n)
        {
            Koma.Nariflg = n;
        }

        public void OnMattaButtonClick()
        {
            if (game.MouseDetectable) game.Matta();
        }
        public void OnFinishButtonClick()
        {
            //セーブしてシーン遷移
            if (game.MouseDetectable)
            {
                game.MouseDetectable = false;
                saveConfirmCanvas.gameObject.SetActive(true);
                Debug.Log("FinishButton Clicked");
            }
        }
        public void OnSaveYesButtonClick()
        {
            game.SaveGame();
            SceneManager.LoadScene("choose");
        }
        public void OnSaveNoButtonClick()
        {
            SceneManager.LoadScene("choose");
        }
    }
}
