using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BachelorArbeitUnity
{
    public class MainMenu : MonoBehaviour
    {
        public InputField pathField;

        public void loadMesh()
        {
            Debug.Log("works");
            InformationHolder.pathToMesh = pathField.text;

            SceneManager.LoadScene("MainView");
        }

        public void exit()
        {
            Application.Quit();
        }
    }
}