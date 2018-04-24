using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pulsing : MonoBehaviour
{

    // Grow parameters
    public float approachSpeed = 0.01f;
    public float growthBound = .01f;
    public float shrinkBound = 0.5f;
    private float currentRatioRight = 1;
    private float currentRatioLeft = 1;

    public GameObject rightPulse;
    public GameObject leftPulse;

    // And something to do the manipulating
    private Coroutine routine;
    private bool keepGoing = true;
    private bool closeEnough = false;

    // Attach the coroutine
    void Awake()
    {
        // Then start the routine
        this.routine = StartCoroutine(this.Pulse());
    }

    IEnumerator Pulse()
    {
        // Run this indefinitely
        while (keepGoing)
        {
            // Get bigger for a few seconds
            while (this.currentRatioRight != this.growthBound)
            {
                // Determine the new ratio to use
                currentRatioRight = Mathf.MoveTowards(currentRatioRight, growthBound, approachSpeed);

                // Update our text element
                rightPulse.transform.localScale = Vector3.one * currentRatioRight;


                // Determine the new ratio to use
                currentRatioLeft = Mathf.MoveTowards(currentRatioLeft, shrinkBound, approachSpeed);

                // Update our text element
                leftPulse.transform.localScale = Vector3.one * currentRatioLeft;
                
                yield return new WaitForEndOfFrame();
            }

            // Shrink for a few seconds
            while (this.currentRatioRight != this.shrinkBound)
            {
                // Determine the new ratio to use
                currentRatioRight = Mathf.MoveTowards(currentRatioRight, shrinkBound, approachSpeed);

                // Update our text element
                this.transform.localScale = Vector3.one * currentRatioRight;

                // Determine the new ratio to use
                currentRatioLeft = Mathf.MoveTowards(currentRatioLeft, growthBound, approachSpeed);

                // Update our text element
                leftPulse.transform.localScale = Vector3.one * currentRatioLeft;

                yield return new WaitForEndOfFrame();
            }
        }
    }
}