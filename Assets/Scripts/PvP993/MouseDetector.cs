using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PvP993
{
    public class MouseDetector : MonoBehaviour
    {
        private int xLength = Choose.InitialSetting.xLength; //オセロ盤の一辺の長さ
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;

        public Button changeDimensionButton;
        public Game game;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnDimensionButtonClick()
        {
            game.ChangeDimension();
        }


        
    }
}
