using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Loader : MonoBehaviour
{
    [FormerlySerializedAs("GameManager")] public GameManager gameManager;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            Instantiate(gameManager);
        }
    }
}