using UnityEngine;

public interface IStrategy
{
    /// <summary>
    /// Initialise the strategy with the given conditions 
    /// of the ball and catcher.
    /// </summary>
    void Initialise(Rigidbody ballRb, Rigidbody catcherRb);

    /// <summary>
    /// Resets this strategy.
    /// </summary>
    void Terminate();

    /// <summary>
    /// Computes the correct position for the catcher
    /// at the current time step, given the catcher's 
    /// position.
    /// </summary>
    /// <returns>The current catcher position as predicted by this strategy</returns>
    void UpdatePrediction(float currentTime, Rigidbody ballRb);

    /// <summary>
    /// Returns the latest position predicted by the strategy
    /// </summary>
    Vector3 GetPrediction();
    
    /// <summary>
    /// Tells whether this strategy is ready to compute a 
    /// correct position or not
    /// </summary>
    bool IsReady();

    /// <summary>
    /// At what time this strategy should initialise.
    /// </summary>
    float TimeToInit();
}

