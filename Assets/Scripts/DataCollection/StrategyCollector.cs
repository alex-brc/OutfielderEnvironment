using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Collects data from the object it's attached to during a trial/practice.
/// </summary>
[RequireComponent(typeof(IStrategy))]
public class StrategyCollector : MonoBehaviour, ICollector
{
    public string fileName;
    public PathDisplay path;

    [Header("References")]
    public DataManager dataManager;

    private IStrategy strategy;
    private StringBuilder stringBuilder;
    private Coroutine writerCoroutine;
    private float startingFrame, startingTime;
    private bool first;

    private void Start()
    {
        // Give this to the manager
        dataManager.collectors.Add(this);
        strategy = gameObject.GetComponent<IStrategy>();
        first = false;
    }

    public void StartCollecting()
    {
        // Make the file
        string fullFileName = dataManager.testPath + "\\" + fileName;
        // Write the columns
        string output =
            "Time,Frame," +
            "Position_X,Position_Y,Position_Z\r\n";
        File.WriteAllText(fullFileName, output);

        startingFrame = Time.frameCount;
        startingTime = Time.time;

        // Clear the strategy path display
        path.Clear();

        stringBuilder = new StringBuilder();
        writerCoroutine = StartCoroutine(WriterRoutine(fullFileName));
    }

    public void StopCollecting()
    {
        // Dump the stringbuilder
        File.AppendAllText(fileName, stringBuilder.ToString());
        stringBuilder = new StringBuilder();

        // Skip one write on the next trial
        first = true;
    }

    private string Record()
    {
        object[] vals = {
            (Time.time - startingTime),
            (Time.frameCount - startingFrame),
            strategy.GetPrediction().ToCSVFormat()
            };

        // Make those values into a comma separated line
        return DataManager.ToCSVLine(vals);
    }

    // Writing operations are performed once every 
    // writeInterval seconds so as to avoid writing 
    // to file on every frame.
    private IEnumerator WriterRoutine(string fileName)
    {
        do
        {
            // Write contents of stringbuilder to file
            string output = stringBuilder.ToString();
            stringBuilder = new StringBuilder();
            File.AppendAllText(fileName, output);
            // Wait for writeInterval seconds before writing again.
            yield return new WaitForSeconds(dataManager.writeInterval);
        }
        while (dataManager.Running());
    }

    void FixedUpdate()
    {
        // If the writer is off don't write anything. 
        // This is only a thing inside the editor for testing purposes.
        if (!dataManager.isActive())
            return;

        if (dataManager.Running() && strategy.IsReady())
        {
            // We need to skip one write on subsequent trials, 
            // since it has the position from the last run. 
            if(first)
            {
                first = false;
                return;
            }
            // Add this to be written
            stringBuilder.Append(Record());
            // Also add position to the path
            path.UpdateLine(strategy.GetPrediction());
        }
    }
}
