using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SubjectOperations : MonoBehaviour
{
    [Header("References")]
    public InputField nameField;
    public InputField ageField;
    public InputField otherField;
    public Slider handednessSlider;
    public Slider genderSlider;
    public Button setSubjectButton;
    public Button clearSubjectButton;
    public AutoManualToggle autoManualToggle;
    public Text statusText;
    public TrialManager manager;
    public DataManager dataWriter;
    public ConfigurationManager confManager;

    internal bool hasSubject;
    internal string subjectName;
    internal string age;
    internal string otherInfo;
    internal string gender;     // false = male,  true = female
    internal string handedness; // false = lefty, true = righty 

    void Start()
    {
        hasSubject = false;
        clearSubjectButton.interactable = false;
        autoManualToggle.Interactable = false;
    }

    public void SetSubject()
    {
        if (nameField.text.Equals(""))
        {
            // Then field is empty, must have something
            statusText.text = "Name field cannot be empty.";
            statusText.color = CustomColors.Red;
            return;
        }

        // Otherwise, we can make a new subject 
        // (or write to the folder of an older 
        // one if they have the same name)
        hasSubject = true;
        // Set the subject values
        subjectName = nameField.text;
        age = ageField.text; // textbox only takes integer inputs
        gender = (genderSlider.value == 0) ? "Male" : "Female";
        handedness = (handednessSlider.value == 0) ? "Left" : "Right";
        otherInfo = otherField.text;

        // Disable this box and the button to set, enable the clear and control view
        nameField.interactable = false;
        ageField.interactable = false;
        genderSlider.interactable = false;
        handednessSlider.interactable = false;
        otherField.interactable = false;
        setSubjectButton.interactable = false;
        clearSubjectButton.interactable = true;
        // Change status 
        statusText.text = "Subject set";
        statusText.color = CustomColors.Black;
        // Initialise data writer
        dataWriter.Init(subjectName,age,gender,handedness,otherInfo);
        // Unlock test settings
        autoManualToggle.Interactable = true;
    }

    public void ClearSubject()
    {
        // Reset buttons and boxes
        hasSubject = false;
        subjectName = "";
        otherField.text = "";
        genderSlider.value = 0;
        handednessSlider.value = 1;
        
        nameField.interactable = true;
        ageField.interactable = true;
        genderSlider.interactable = true;
        handednessSlider.interactable = true;
        otherField.interactable = true;
        setSubjectButton.interactable = true;
        clearSubjectButton.interactable = false;
        statusText.text = "Subject cleared";
        statusText.color = CustomColors.Black;
        // Also reset the trials
        manager.ResetTests();
        // Also reset the dataWriter
        dataWriter.ResetWriter();
        // And block load test button
        autoManualToggle.Interactable = false;
    }
}
