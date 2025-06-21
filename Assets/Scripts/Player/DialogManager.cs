using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [SerializeField] TMP_Text dialogText;

    public List<string> ArrangedDialogLines;
    public List<string> RandomDialogLines;
    public float DialogDisplayTime = 5f;
    public float FullTextDisplayTime = 2f;

    private int currentLineIndex = 0;
    private bool isArrangedDialogActive = true;
    private float delayBetweenLines = 0f;



    void Start()
    {
        if (dialogText == null)
        {
            Debug.LogError("Dialog Text is not assigned in the DialogManager.");
            return;
        }
        currentLineIndex = 0;
    }
    void Update()
    {
        delayBetweenLines += Time.deltaTime;
        float ratio = delayBetweenLines/(DialogDisplayTime - FullTextDisplayTime);
        if (ratio > 1)
        {
            ratio = 1;
        }

        if (isArrangedDialogActive)
        {
            dialogText.text = ArrangedDialogLines[currentLineIndex].Substring(0, Mathf.RoundToInt(ArrangedDialogLines[currentLineIndex].Length * ratio));
            if (delayBetweenLines >= DialogDisplayTime)
            {
                delayBetweenLines = 0f;
                currentLineIndex++;
                if (currentLineIndex >= ArrangedDialogLines.Count)
                {
                    dialogText.text = "";
                    isArrangedDialogActive = false;
                    currentLineIndex = 0;
                    ArrangedDialogLines.Clear();
                }
            }
        }
        else
        {
            dialogText.text = RandomDialogLines[currentLineIndex].Substring(0, Mathf.RoundToInt(RandomDialogLines[currentLineIndex].Length * ratio));
            if (delayBetweenLines >= DialogDisplayTime)
            {
                delayBetweenLines = 0f;
                if (RandomDialogLines.Count > 0)
                {
                    int randomIndex = Random.Range(0, RandomDialogLines.Count);
                }
                else
                {
                    dialogText.text = "";
                }
            }
        }
    }
}
