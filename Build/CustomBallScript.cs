
/* <<<                       Foreword                          >>>
 * 
 * This script provides you with AMPLE posibilites of customizing your
 * experiment. This script is compiled at run time and executed without 
 * restrictions. From here you can do such things as:
 *  - Dynamically create objects for the ball and player to interact with
 *  - Affect the ball's trajectory, size, color, texture, etc.
 *  - Slow time!
 *  - And pretty much anything else the UnityEngine will allow you to do
 *  
 *  For integrity reasons, access to the player controller is not given 
 *  at this time. This may come in a later iteration, if requested.
 *  
 * HOWEVER! (A short warning)
 * 
 * Take for example ballTransform.GetComponent<MeshRenderer>().
 * This returns the mesh renderer of the ball, from where you can 
 * affect how the ball looks. If you were, for example,
 * to change the value of the meshRenderer.isVisible to false, 
 * the ball would also disappear from the experiment view.
 * In that specific instance, it might be desirable to use the
 * MAKE_VISIBLE() method provided, which only hides it from the
 * player. 
 * 
 * This is a pretty tame example, but it shows that simple things can have
 * unwanted implications. I tried to shield everything as well as possible,
 * but there still might be some things that when affected could cause
 * other things to crash. You might end up losing hours worth of data 
 * because you wanted the ball to be able to pass through the ground,
 * or something wild like that.
 * 
 * 
 * Be careful with some things, this script gives you a great amount of power 
 * to break things (not permanently, of course, but you might not realise they're 
 * broken until it's too late). 
 * 
 *                      Happy testing!
 */

/* <<<DIRECTIVES SECTION>>> */
/* Directives. Deleting any of these is highly likely to break this script.
 * Only change this section if you know what you are doing.
 * A number of redundant directives have been provided for convenience.*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fove;
using FoveSettings;
using UnityEngine;
using UnityEngine.UI;
/* <<<END OF DIRECTIVES SECTION>>> */

public class BallScript
{
    /* <<<DECLARATION SECTION>>> */
    // !CRITICAL CODE! //
    Rigidbody ballRigidbody;
    Transform ballTransform;
    Light lightSource;
    System.Random RANDOM;
    GameObject WALL_PREFAB;
    // !END OF CRITICAL CODE! //

    /* If you need anything declared within the scope of the experiment,
     * you can declare it here. Again, deleting things is very likely to 
     * break the script. Excercise caution. */

    // Insert anything you might need here

    /* <<<END OF DECLARATION SECTION>>> */
    ////////////////////////////////////
    /* <<<INITIALISATION SECTION>>> */
    /* You can initialise any of the variables you declared earlier here.
     * As previously inferred, a new instance of this script is created
     * for every new experiment (i.e. every time you press the Start button).
     * That is when this is called.
     * 
     * !!WARNING!! You CANNOT change the signature of this method (i.e. 
     * the parameters it takes or its name. This WILL break this script. */
    public BallScript(int RANDOM_SEED, GameObject WALL_PREFAB, Rigidbody ballRigidbody, Transform ballTransform, Light lightSource)
    {
        /* !CRITICAL CODE! */
        RANDOM = new System.Random(RANDOM_SEED);
        this.WALL_PREFAB = WALL_PREFAB;
        this.ballRigidbody = ballRigidbody;
        this.ballTransform = ballTransform;
        this.lightSource = lightSource;
        /* !END OF CRITICAL CODE! */

        // Initialise your variables here //

        
    }
    /* <<<END OF INITIALISATION SECTION>>> */
    ////////////////////////////////////////
    /* <<<SCRIPT FUNCTIONS SECTION>>>*/

    /*
     * The WhileRunning() method is called on every FixedUpdate(), which occurs 
     * roughly at 100Hz. You should update the state of the ball in this method.
     * You can (and should) obtain the exact time in seconds since the last update
     * from "Time.deltaTime".
     * 
     * NOTE: 
     * You can think of FixedUpdate() as "every time physics gets updated".
     * This is NOT called every frame. Sometimes it may be called more 
     * frequently than every frame, other times less frequently.
     * This information is provided for completeness, but for most intents 
     * and purposes, this should be irrelevant to you.
     * 
     * Available variables: 
     *      
     *  ballTransform -> The transform component of the ball.
     *      This holds the position, rotation and scale of the ball.
     *      It also has some very useful methods. Reference the Unity 
     *      API for "Transform" for more information. 
     * 
     *  ballRigidbody -> The physics component of the ball. 
     *      This component holds all physics-related information and functions.
     *      Reference the Unity API for "Rigidbody" for more information.
     *      
     *  currentTest -> The ID of the test currently being run. Note that this 
     *      ID is zero-based.
     *  
     *  currentTime -> The current time WITHIN the running trial in seconds. 
     *      More explicitly, this is 0 at the beginning of the trial.]
     *      
     *  catcherPosition -> The position of the catcher on the field. 
     *      Note that this is NOT the position of the headset. This 
     *      is guaranteed to have the Y coordiante equal to 1. 
     *      
     *  headsetPosition -> The position of the headset. 
     *      The headset is most likely at a short distance (<2m) above
     *      the catcher and some ways to the side or front/back.
     *      
     * The FoveInterface class is also available to you. It offers
     * eye rays, convergence data and more. Reference the FoveInterface
     * plugin for Unity API for more information.
     */
    public void WhileRunning(int currentTest, float currentTime, Vector3 catcherPosition, Vector3 headsetPosition)
    {
        // Insert your code here

    }

    /* The BeforeCountdown() method is called before a new trial is 
     * started, before the countdown ever starts. Any changes made
     * here, the subject will have pause_between_trials seconds to 
     * notice them and adjust.
     * 
     * Available variables:  ballRigidbody, ballTransform, lightSource
     */
    public void BeforeCountdown()
    {
        // Insert your code here

    }

    /* The BeforeLaunch() method is called before a new trial is 
     * started, the instant before the ball is launched.
     * 
     * Available variables:  ballRigidbody, ballTransform
     */
    public void BeforeLaunch()
    {
        // Insert your code here

    }

    /* The After() method is called at the end of a trial. 
     * This is where you should destroy any gameobjects you created
     * and generally just clean up whatever mess has been made during
     * the trial
     * 
     * Available variables: ballRigidbody, ballTransform, lightSource
     * 
     *  caught -> Whether the player caught the ball this trial.
     */
    public void After(bool caught)
    {
        // Insert your code here

    }
    /* <<<END OF SCRIPT FUNCTIONS SECTION>>>*/
    //////////////////////////////////

    /* <<<HELPER FUNCTIONS SECTION>>> */
    /* These are not critical parts of this script.
     * They don't do anything special, all of this can be
     * done inside the script functions. However, they are
     * commonly used and are provided here for convenience. */

    /* Produce a random integer in the interval [from,to] (inclusive) */
    private int RANDOM_INT(int from, int to)
    {
        return RANDOM.Next(from, to);
    }

    /* Make a wall at the designated position with the designated rotation. 
     * If visible is set to false, it will be hidden from the catcher's camera
     * but visible to you from the computer camera. */
    private GameObject MAKE_WALL(Vector3 position, Vector3 rotationDegrees, bool visible)
    {
        GameObject wall = GameObject.Instantiate(WALL_PREFAB);
        wall.transform.position = position;
        wall.transform.rotation = Quaternion.Euler(rotationDegrees);
        if (!visible)
            wall.layer = 9; 
        
        return wall;
    }

    /* Set visibility for a given game object. The object will remain visible to 
     * the computer camera, but will be hidden from the player. */
    private void SET_VISIBLE(GameObject gameObject, bool visible)
    {
        if (visible)
            gameObject.layer = 0;
        else
            gameObject.layer = 9;
    }

    /* Use this function to record any custom data you might want to a file.  */
    private bool RECORD_LINE(string data, string fileName)
    {
        try {
            using (StreamWriter writer = new StreamWriter(fileName))
                writer.WriteLine(data);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}
/* <<<END OF FILE>>> */
