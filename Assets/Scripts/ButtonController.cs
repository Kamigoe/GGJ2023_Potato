using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    [SerializeField] GameObject credit;
 
    public void clickStartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void clickCreditButton()
    {
        credit.SetActive(true);
    }

    public void clickBackButton()
    {
        credit.SetActive(false);
    }
}
