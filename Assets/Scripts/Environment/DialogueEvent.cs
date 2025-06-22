using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueEvent : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] List<string> dialogues;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] List<string> secondaryDialogue;

    private bool invoked = false;

    private void Update()
    {
        if (dialogueManager.isArrangedDialogActive != true)
        {
            if (playerMovement != null)
            {
                playerMovement.isFrozen = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (invoked)
            {
                dialogueManager.SetArrangedDialogLines(secondaryDialogue);
            }
            else
            {
                invoked = true;
                dialogueManager.SetArrangedDialogLines(dialogues);
                if (playerMovement != null)
                {
                    playerMovement.isFrozen = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueManager.ArrangedDialogLines.Clear();
        }
    }
}
