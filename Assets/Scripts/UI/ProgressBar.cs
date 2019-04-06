using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private const int TRIALBAR_OFFSET = 10;
    [Header("References")]
    public TrialRunner runner;

    private RectTransform practiceBar;
    private RectTransform trialsBar;
    private RectTransform backgroundBar;
    internal Text percentage;

    private float sizePerTest;
    private bool first;

    public void Start()
    {
        // Grab the references
        practiceBar = transform.Find("Practice%").GetComponent<RectTransform>();
        trialsBar = transform.Find("Trials%").GetComponent<RectTransform>();
        backgroundBar = transform.GetComponent<RectTransform>();
        Transform pd = transform.Find("PercentageDisplay");  
        percentage = pd.Find("Text").GetComponent<Text>();
        
        // Clean up whatever's already on the bars
        practiceBar.SetWidth(0);
        trialsBar.SetWidth(0);
        percentage.text = "0%";
    }

    public void Initialise()
    {
        // Cleanup
        practiceBar.SetWidth(0);
        trialsBar.SetWidth(0);
        percentage.text = "0%";

        // Compute sizes
        sizePerTest = backgroundBar.rect.width / (runner.NumTrials + runner.NumPractices);
        float maxPracticeBarSize = runner.NumPractices * sizePerTest;
        // Practice bar starts at one end
        practiceBar.SetX(-backgroundBar.rect.width);
        // Trial bar starts where practicebar ends,
        // minus an adjustment for rounded corners 
        // (so they fit together nice) 
        trialsBar.SetX(-backgroundBar.rect.width + maxPracticeBarSize - TRIALBAR_OFFSET);
        first = true;
    }

    public void UpdateBars(TestCase.TrialType trialType)
    {
        if (trialType == TestCase.TrialType.Practice)
        {
            // Not finished practices yet, work on practice bar size
            practiceBar.SetWidth(practiceBar.rect.width + sizePerTest);
        }
        else
        {
            // Practices finished, work on trial bar size
            if (first) // This is for that rounded corner adjustment
            {
                trialsBar.SetWidth(TRIALBAR_OFFSET);
                first = false;
            }
            trialsBar.SetWidth(trialsBar.rect.width + sizePerTest);

        }
        // Write percents
        string percent = (1.0f * (runner.CurrentIndex + 1)/ (runner.NumTrials + runner.NumPractices)).ToString("0%");
        string count = " (" + (runner.CurrentIndex + 1) + "/" + (runner.NumTrials + runner.NumPractices) + ")";
        percentage.text = percent + count;
    }
}
