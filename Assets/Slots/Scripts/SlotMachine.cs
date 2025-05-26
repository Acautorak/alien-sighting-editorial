using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{

    public List<Reel> reels; 
    public Symbol[] symbols; 
    public int reelsCount = 3; 


    private float spinDuration = 2.0f; 
    private float stopDelay = 0.5f; 
    private float reelSpeed = 10.0f; 
    public void Spin()
    {
        foreach (Reel reel in reels)
        {
            reel.StartSpinning();
        }
    }

    // Method to stop the spinning and check for winning combinations
    public void Stop()
    {
        Debug.Log("Stopping the slot machine...");
        // Implement stopping logic and checking for winning combinations here
    }
}
