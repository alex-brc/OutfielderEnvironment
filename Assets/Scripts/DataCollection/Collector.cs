using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Collects data from the object it's attached to during a trial/practice.
/// </summary>
public abstract class Collector : MonoBehaviour
{
    public string fileName;

    [Header("References")]
    public DataManager dataManager;

    protected StringBuilder stringBuilder;
    protected Coroutine writerCoroutine;
    protected float startingFrame, startingTime;

    public abstract string GetColumns();
    public abstract object[] GetData();

    protected void Start()
    {
        // Give this to the manager
        dataManager.collectors.Add(this);
    }

    public void StartCollecting()
    {
        // Make the file
        string fullFileName = dataManager.testPath + "\\" + fileName;
        // Write the columns
        string output = GetColumns();
        File.WriteAllText(fullFileName, output);

        startingFrame = Time.frameCount;
        startingTime = Time.time;

        stringBuilder = new StringBuilder();
        writerCoroutine = StartCoroutine(WriterRoutine(fullFileName));
    }

    public void StopCollecting()
    {
        // Dump the stringbuilder
        File.AppendAllText(fileName, stringBuilder.ToString());
        stringBuilder = new StringBuilder();
    }

    protected string Record()
    {
        // Get objects we want to record
        object[] objs = GetData();
        // Make those values into a comma separated line
        return DataManager.ToCSVLine(objs);
    }

    // Writing operations are performed once every 
    // writeInterval seconds so as to avoid writing 
    // to file on every frame.
    protected IEnumerator WriterRoutine(string fileName)
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

    protected void FixedUpdate()
    {
        // If the writer is off don't write anything. 
        // This is only a thing inside the editor for testing purposes.
        if (!dataManager.isActive())
            return;

        if (dataManager.Running())
            stringBuilder.Append(Record());
    }
}
