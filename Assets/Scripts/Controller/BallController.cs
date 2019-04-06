using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("References")]
    public TrialManager manager;
    public InputField sizeField;
    public InputField massField;
    public InputField dragField;

    [Header("Ball Script References")]
    public GameObject wallPrefab;
    public Light lightSource;

    internal bool scriptCompiled;
    internal ScriptCompiler.CustomCompilerResults customScript;
    internal object scriptInstance;

    internal Configurable<int> preset = new Configurable<int>();
    internal Configurable<float> size = new Configurable<float>();
    internal Configurable<float> mass = new Configurable<float>();
    internal Configurable<float> drag = new Configurable<float>();
    private Rigidbody rb;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Start()
    {
        scriptCompiled = ScriptCompiler.CompileBallScript(ref customScript);
        if (!scriptCompiled)
            manager.confManager.statusText.text = "Custom ball script compile failed. Check log.";
        
    }

    public void Refresh()
    {
        if (preset != 0)
            BallPreset.SetPreset(preset.Get(), ref size, ref mass, ref drag);
           
        rb.mass = mass.Get();
        rb.drag = drag.Get();
        transform.localScale = new Vector3(size.Get(), size.Get(), size.Get());

        // Update UI boxes
        sizeField.text = size.ToString();
        massField.text = mass.ToString();
        dragField.text = drag.ToString();
    }

    public void Initialise()
    {
        // Get a new ball script
        if(scriptCompiled)
            scriptInstance = Activator.CreateInstance(customScript.type, new object[] { (int)Time.time*100, wallPrefab, rb, transform, lightSource });
    }

    public void BeforeCountdown()
    {
        if(scriptCompiled)
            customScript.BeforeCountdown.Invoke(scriptInstance, null);
    }

    public void BeforeLaunch()
    {
        if (scriptCompiled)
            customScript.BeforeLaunch.Invoke(scriptInstance, null);
    }

    private void FixedUpdate()
    {
        Vector3 newPlayerPos = manager.player.transform.position;

        if (scriptCompiled && manager.trialStatus == TrialManager.TrialStatus.TrialInProgress)
            customScript.WhileRunning.Invoke(scriptInstance, new object[] {
            manager.loadedTestCase.testNumber,
            Time.time - manager.startingTime,
            newPlayerPos,
            FoveInterface.GetHMDPosition()});
    }

    public void After(bool caught)
    {
        if (scriptCompiled)
            customScript.After.Invoke(scriptInstance, new object[] { caught });
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit the player or terrain
        bool caught = false;
        if (collision.collider.gameObject.tag == "Catcher")
            caught = true;

        if (caught || collision.collider.gameObject.tag == "Terrain")
            // Finish the trial
            manager.CompleteTrial(caught);
    }   

}
