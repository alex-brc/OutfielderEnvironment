using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastResult : MonoBehaviour
{
    public AudioClip catchClip;
    public AudioClip noCatchClip;

    private AudioSource audioSource;
    private Image background;
    private Text text;
    private int total;

    private void Start()
    {
        background = transform.GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();
        background.color = CustomColors.White;
        total = 0;
        audioSource = GetComponent<AudioSource>();
    }

    public void UpdateResult(bool caught, TestCase.TrialType type)
    {
        if (caught)
        {
            background.color = CustomColors.SoftGreen;
            text.text = "CATCH";
            if (type == TestCase.TrialType.Trial)
                total++;

            // Play sound
            audioSource.clip = catchClip;
            audioSource.Play();
        }
        else
        {
            background.color = CustomColors.SoftRed;
            text.text = "MISS";

            // Play sound
            audioSource.clip = noCatchClip;
            audioSource.Play();
        }
    }

    public void ShowTotal(int numRuns)
    {
        background.color = CustomColors.White;
        text.text = total + "/" + numRuns;
        total = 0;
    }
}
