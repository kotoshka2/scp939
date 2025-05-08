using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogSys : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI interactText;

    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private List<string> dialog;
    private int currentDialogRow = 0;
    private bool isPlayerInZone = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInZone)
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                dialogText.GameObject().SetActive(true);
                interactText.GameObject().SetActive(false);
                Dial();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        interactText.GameObject().SetActive(true);
        isPlayerInZone = true;

    }

    /*private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            dialogText.GameObject().SetActive(true);
            interactText.GameObject().SetActive(false);
            Dial();
        }
        
        
    }*/

    private void OnTriggerExit(Collider other)
    {
        isPlayerInZone = false;
        interactText.GameObject().SetActive(false);
        dialogText.GameObject().SetActive(false);
        dialogText.text = string.Empty;
        currentDialogRow = 0;
        
    }

    private void Dial()
    {
        dialogText.text = dialog[currentDialogRow];
        if (currentDialogRow == dialog.Count-1)
        { 
            currentDialogRow = 0;
        }
        else
        {
            currentDialogRow += 1;
        }
            
    }
}
