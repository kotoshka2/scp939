using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NPCDialogSys : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI interactText;

    [SerializeField] private TextMeshProUGUI dialogText;
    private AudioSource _audioSource;
    [SerializeField] private List<string> dialog;
    private int currentDialogRow = 0;
    private bool isPlayerInZone = false;
    [FormerlySerializedAs("Audios")] [SerializeField] private List<AudioClip> audios;
    private GameObject _player;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private GameObject _parent;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
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
        _player = other.GameObject();
        interactText.GameObject().SetActive(true);
        isPlayerInZone = true;

    }

    private void OnTriggerStay(Collider other)
    {
        RotateToPlayer(_player.transform);
    }

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
        _audioSource.clip = audios[currentDialogRow];
        _audioSource.Play();
        if (currentDialogRow == dialog.Count-1)
        { 
            currentDialogRow = 0;
        }
        else
        {
            currentDialogRow += 1;
        }
            
    }
    void RotateToPlayer(Transform player)
    {
        Vector3 direction = player.position - _parent.transform.position;
        

        if (direction.sqrMagnitude > 0.01f)
        {
            
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Debug.Log(targetRotation);

            _parent.transform.rotation = targetRotation;
        }
    }
}
