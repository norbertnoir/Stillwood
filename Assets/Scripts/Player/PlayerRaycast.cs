using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    public Transform playerCamera;       // Kamera gracza
    public float raycastDistance = 10f;  // Długość raycastu
    public LayerMask hitLayer;          // Layer, który będzie wykrywany przez raycast

    void Update()
    {
        // Wykonanie raycastu w kierunku przed gracza
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, raycastDistance, hitLayer))
        {
            if (hit.collider.gameObject.CompareTag("Interactable"))
            {
                IInteraction interactionObject = hit.collider.gameObject.GetComponent<IInteraction>();

                if (interactionObject.isInteractable == false)
                {
                    UIController.instance.ShowInteractionPanel(false);
                    return;
                }


                UIController.instance.ShowInteractionPanel(true, interactionObject.alertText); // Pokazanie panelu interakcji
                if (Input.GetKeyDown(KeyCode.E)) // Sprawdzenie, czy naciśnięto klawisz E
                {
                    // Wywołanie metody Interact() na obiekcie, który implementuje IInteraction
                    interactionObject.Interact();
                }

                return;
            }

            if (hit.collider.gameObject.CompareTag("Choice"))
            {
                Choice myChoice = hit.collider.gameObject.GetComponent<Choice>();

                UIController.instance.ShowChoicePanel(true, myChoice.choiceTextA, myChoice.choiceTextB); // Pokazanie panelu wyboru

                if (Input.GetKeyDown(KeyCode.Q)) // Sprawdzenie, czy naciśnięto klawisz 1
                {
                    myChoice.MakeChoiceA(); // Wywołanie metody wyboru A
                }
                else if (Input.GetKeyDown(KeyCode.E)) // Sprawdzenie, czy naciśnięto klawisz 2
                {
                    myChoice.MakeChoiceB(); // Wywołanie metody wyboru B
                }
            }
        }
        else
        {
            if (UIController.instance != null)
                UIController.instance.HidePanels();
        }

        // Opcjonalnie rysowanie wizualizacji raycastu w edytorze
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.red);
    }
}
