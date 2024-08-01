using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] GameObject optionsText;
    [SerializeField] GameObject controlsPic;
      [SerializeField] GameObject towerInfoPic;

    void Start()
    {
        optionsText.SetActive(false);
        controlsPic.SetActive(false);
        towerInfoPic.SetActive(false);
    }
    public void TowerInfo()
    {
        towerInfoPic.SetActive(true);
    }

    public void goBackTowerInfo()
    {
        towerInfoPic.SetActive(false);
    }

   public  void ButtonNewGame()
    {
        SceneManager.LoadScene(2);
    }

    public void ButtonEndlessMode()
    {
        SceneManager.LoadScene(1);
    }
    public void ButtonOptions()
    {
        optionsText.SetActive(true);
    }
    public void ButtonControls()
    {
        controlsPic.SetActive(true);
    }

    public void ButtonGoBack()
    {
        controlsPic.SetActive(false);
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
}
