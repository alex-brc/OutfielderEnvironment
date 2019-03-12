using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastResult : MonoBehaviour
{
    private Image background;
    private Text text;
    private int total;

    private void Start()
    {
        background = transform.GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();
        background.color = CustomColors.White;
        total = 0;
    }

    public void UpdateResult(bool caught, TestCase.TrialType type)
    {
        if (caught)
        {
            background.color = CustomColors.SoftGreen;
            text.text = "CATCH";
            if (type == TestCase.TrialType.Trial)
                total++;
        }
        else
        {
            background.color = CustomColors.SoftRed;
            text.text = "MISS";
        }
    }

    public void ShowTotal(int numRuns)
    {
        background.color = CustomColors.White;
        text.text = total + "/" + numRuns;
        total = 0;
    }
}
