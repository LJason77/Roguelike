using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiButtons : MonoBehaviour
{
    public void Restart()
    {
        Debug.Log("重新开始");
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("Main");
        GameManager.Instance.Invoke("HideBlack", 1);
    }

    public void Quit()
    {
        Debug.Log("退出");
        Application.Quit();
    }
}