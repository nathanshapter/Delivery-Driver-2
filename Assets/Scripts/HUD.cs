using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI speedText, gearText;
    PlayerControls playerControls;


    private void Start()
    {
        playerControls = FindObjectOfType<PlayerControls>();
    }

    private void Update()
    {
        speedText.text = playerControls.CurrentSpeedInKM().ToString("F1") + " km/h";
    }
}
