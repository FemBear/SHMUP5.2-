using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelect : MonoBehaviour
{
    [SerializeField]
    private Button primaryButton;
    void Start()
    {
        primaryButton = GetComponent<Button>();
        primaryButton.Select();
    }
}
