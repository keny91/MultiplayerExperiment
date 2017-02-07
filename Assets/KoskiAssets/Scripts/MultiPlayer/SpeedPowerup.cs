using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : PowerUpControl {


    /// <summary>
    /// Trigger an effect on the player 
    /// </summary>
    /// <param name="theP"></param>
    public override void Effect(MultiplayerControlScript theP)
    {
        theP.movingSpeed = theP.movingSpeed*1.5f;
    }
        
}
