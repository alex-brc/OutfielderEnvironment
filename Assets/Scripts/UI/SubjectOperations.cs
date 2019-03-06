using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubjectOperations : MonoBehaviour
{
    [Header("References")]
    public InputField subjectInputField;
    public Button setSubjectButton;
    public Button clearSubjectButton;
    public Button controlViewButton;
    public Text statusText;
    public TrialsManager trialsManager;
    public DataWriter dataWriter;

    bool hasSubject;
    internal string subjectName;

    void Start()
    {
        hasSubject = false;
        clearSubjectButton.interactable = false;
        controlViewButton.interactable = false;
    }

    public void SetSubject()
    {
        if (subjectInputField.text.Equals(""))
        {
            // Then field is empty, must have something
            statusText.text = "Fields empty";
            statusText.color = CustomColors.Red;
            return;
        }

        // Otherwise, we can make a new subject 
        // (or write to the folder of an older 
        // one if they have the same name)
        hasSubject = true;
        // Set the subject name
        subjectName = subjectInputField.text;
        // Disable this box and the button to set, enable the clear and control view
        subjectInputField.interactable = false;
        setSubjectButton.interactable = false;
        clearSubjectButton.interactable = true;
        controlViewButton.interactable = true;
        // Change status 
        statusText.text = "Subject set";
        statusText.color = CustomColors.Green;
        // Initialise data writer
        dataWriter.Init(subjectName);
    }

    public void ClearSubject()
    {
        // Reset buttons and boxes
        hasSubject = false;
        subjectName = "";
        subjectInputField.interactable = true;
        setSubjectButton.interactable = true;
        clearSubjectButton.interactable = false;
        controlViewButton.interactable = false;
        statusText.text = "Subject cleared";
        statusText.color = CustomColors.Black;
        // Also reset the trials
        trialsManager.ResetTests();
        // Also reset the dataWriter
        dataWriter.Reset();
    }
}
