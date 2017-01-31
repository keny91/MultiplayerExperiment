using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : PowerUpControl {


    public override void Effect(PlayerControllerScript theP)
    {
        theP.movingSpeed = theP.movingSpeed*1.5f;
    }
        
}
