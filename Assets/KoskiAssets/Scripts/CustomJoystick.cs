using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CustomJoystick : Joystick {

    new void OnEnable()
    {
        CreateVirtualAxes();
    }

    new void Start()
    {
        m_StartPos = transform.position;
    }

}
