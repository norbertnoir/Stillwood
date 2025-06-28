using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventDecisionManager : MonoBehaviour
{
    [SerializeField] TMP_Text QAnswer;
    [SerializeField] TMP_Text EAnswer;
    [SerializeField] TMP_Text Question;

    EventDecision currentDecision = null;

    void Start()
    {
        QAnswer.text = "";
        EAnswer.text = "";
        Question.text = "";
    }
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f))
        {
            if (hit.collider != null)
            {
                EventDecision decision = hit.collider.GetComponent<EventDecision>();
                if (decision != null && decision != currentDecision)
                {
                    currentDecision = decision;
                    Question.text = decision.Question;
                    QAnswer.text = decision.QAnswer;
                    EAnswer.text = decision.EAnswer;
                }
            }
        }
        else
        {
            if (currentDecision != null)
            {
                currentDecision = null;
                QAnswer.text = "";
                EAnswer.text = "";
                Question.text = "";
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && currentDecision != null)
        {
            if (currentDecision.EResults != null)
            {
                currentDecision.EResults.Invoke();
            }
            if (currentDecision.DeleteAfter)
            {
                Destroy(currentDecision.gameObject);
            }
            currentDecision = null;
            QAnswer.text = "";
            EAnswer.text = "";
            Question.text = "";
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && currentDecision != null)
        {
            if (currentDecision.QResult != null)
            {
                currentDecision.QResult.Invoke();
            }
            if (currentDecision.DeleteAfter)
            {
                Destroy(currentDecision.gameObject);
            }
            
            currentDecision = null;
            QAnswer.text = "";
            EAnswer.text = "";
            Question.text = "";
        }
    }
}
