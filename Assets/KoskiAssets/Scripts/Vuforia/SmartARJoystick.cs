using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

public class SmartARJoystick : Joystick
{

    // work in an AR environment.
    private bool ARcorrection = true;
    public Transform CameraPosition;




    new void OnEnable()
    {
        CreateVirtualAxes();
    }

    /// <summary>
    /// Enable AR movement estimation in an AR scene.
    /// </summary>
    public void enableARCorrection()
    {
        ARcorrection = true;
    }

    /// <summary>
    /// Disable AR movement estimation in an AR scene.
    /// </summary>
    public void DisableARCorrection()
    {
        ARcorrection = false;
    }

    /// <summary>
    ///  Checks which axis are to be expected as inputs and registers them as an input if they have not
    ///  been registered already
    /// </summary>
    private new void CreateVirtualAxes()
    {
        // set axes to use
        m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
        m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

        // create new axes based on axes to use
        if (m_UseX)
        {
            if (CrossPlatformInputManager.AxisExists(horizontalAxisName))
            {
                Debug.LogWarning("Trying to register already registered JoyStick axis:" + horizontalAxisName);
                CrossPlatformInputManager.UnRegisterVirtualAxis(horizontalAxisName);
            }

            m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
        }
        if (m_UseY)
        {
            if (CrossPlatformInputManager.AxisExists(verticalAxisName))
            {
                Debug.LogWarning("Trying to register already registered JoyStick axis:" + verticalAxisName);
                CrossPlatformInputManager.UnRegisterVirtualAxis(verticalAxisName);
            }

            m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
        }

    }

    /// <summary>
    /// A way to unregister the axis from external classes
    /// </summary>
    public void Disable()
    {
        OnDisable();
    }


    /// <summary>
    /// Based on the position of the ARCamera, we c
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private Vector3 CalculateJoyStickAngle(Vector3 value)
    {
        float valX = value.x;
        valX = -valX;  // The X axis is inverted by default in the original class.
        float valY = value.y;
        float newValX, newValY;


        float R = Mathf.Sqrt(Mathf.Pow(valX, 2) + Mathf.Pow(valY, 2));
        Mathf.Acos(valX / R);
        //Debug.LogWarning("value X = " + valX + "__  value Y = " + valY);

        float angle1 = Mathf.Acos(valX / R);
        float angle2 = Mathf.Asin(valY / R);

        angle1 = RadianToDegree(angle1);
        angle2 = RadianToDegree(angle2);


        // The pointed direction is in the SouthQuadrants
        if (angle2 < 0)
            angle1 = 360 - angle1;

        // Initialize default values
        newValX = 0;
        newValY = 0;


        float RotationCompensation;

        if (CameraPosition.rotation.eulerAngles.y < 0)
            RotationCompensation = CameraPosition.rotation.eulerAngles.y + 360;
        else
            RotationCompensation = CameraPosition.rotation.eulerAngles.y;
        //Debug.LogError("Camera Compensation = " + RotationCompensation + "Camera rot: " + CameraPosition.rotation.y);

        angle1 -= RotationCompensation;

        // We operate in the 0 to 359 degrees region. So we will ajust any other angles that go over or under this range.
        if (angle1 > 360)
            angle1 -= 360;
        else if (angle1 < 0)
            angle1 += 360;

        // When the joystick remains at the origin position
        if (valX == 0 && valY == 0)
        {
            newValX = 0;
            newValY = 0;
        }

        // First quadrant 0 - 90
        else if (angle1 < 90 && angle1 >= 0)
        {
            //  Debug.LogWarning("ESTIMATED ANGLE1 = " + angle1 + "__  angle2 = " + angle2 +" MOVING IN QUADRANT 1");
            newValX = Mathf.Cos(DegreeToRadian(angle1));
            newValY = Mathf.Sin(DegreeToRadian(angle1));
        }
        // Second quadrant 90 - 180
        else if (angle1 < 180 && angle1 >= 90)
        {
            //  Debug.LogWarning("ESTIMATED ANGLE1 = " + angle1 + "__  angle2 = " + angle2 + " MOVING IN QUADRANT 2");
            newValX = Mathf.Cos(DegreeToRadian(angle1));
            newValY = Mathf.Sin(DegreeToRadian(angle1));
        }
        // Third quadrant 180 -270 (-180 - -90)
        else if (angle1 < 270 && angle1 >= 180)
        {
            // Debug.LogWarning("ESTIMATED ANGLE1 = " + angle1 + "__  angle2 = " + angle2 + " MOVING IN QUADRANT 3");
            newValX = Mathf.Cos(DegreeToRadian(angle1));
            newValY = Mathf.Sin(DegreeToRadian(angle1));
        }


        // Fourth quadrant 270 - 360 (-90 - 0)
        else if (angle1 < 360 && angle1 >= 270)
        {
            //   Debug.LogWarning("ESTIMATED ANGLE1 = " + angle1 + "__  angle2 = " + angle2 + " MOVING IN QUADRANT 4");
            newValX = Mathf.Cos(DegreeToRadian(angle1));
            newValY = Mathf.Sin(DegreeToRadian(angle1));
        }

        Vector3 vec = new Vector3(newValX, newValY, 0);
        return vec;

    }



    /// <summary>
    /// Convert from degree units to radians.
    /// </summary>
    /// <param name="angle">Angle input in degrees to be converted</param>
    /// <returns>Radian unit Conversion</returns>
    private float DegreeToRadian(float angle)
    {
        return Mathf.PI * angle / 180.0f;
    }

    /// <summary>
    ///  Convert from radians to degree units.
    /// </summary>
    /// <param name="angle">Angle input in radians to be converted</param>
    /// <returns>Degree unit Conversion</returns>
    private float RadianToDegree(float angle)
    {
        return 180.0f * angle / Mathf.PI;
    }





    /// <summary>
    /// Get the values of the axis depending on the position to the origin.
    /// It will apply a AR based reajustment if ARcorrection is enabled.
    /// </summary>
    /// <param name="value">Vector3 containing only X and Y; used to make a complex estimation</param>
    public override void UpdateVirtualAxes(Vector3 value)
    {
        var delta = m_StartPos - value;
        delta.y = -delta.y;
        delta /= MovementRange;

        // Only do the correction in if it has been activated and is p
        if (ARcorrection)
            delta = CalculateJoyStickAngle(delta);
        //Debug.Log("updating axis");

        if (m_UseX)
        {
            m_HorizontalVirtualAxis.Update(delta.x);
        }

        if (m_UseY)
        {
            m_VerticalVirtualAxis.Update(delta.y);
        }
    }


    /// <summary>
    /// Translate the joystick as it is dragged in the UI.
    /// The JoyStick can only be displaced in around a circunference instead of the square the parent class allows.
    /// </summary>
    /// <param name="data">Dragging event</param>
    public override void OnDrag(PointerEventData data)
    {
        Vector3 newPos = Vector3.zero;

        if (m_UseX)
        {
            int delta = (int)(data.position.x - m_StartPos.x);
            delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
            newPos.x = delta;
        }

        if (m_UseY)
        {
            int delta = (int)(data.position.y - m_StartPos.y);
            delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
            newPos.y = delta;
        }

        // Circular joystick displacement version
        transform.position = Vector3.ClampMagnitude(new Vector3(newPos.x, newPos.y, newPos.z), MovementRange) + m_StartPos;

        // Square joystick displacement version
        //transform.position = new Vector3(m_StartPos.x + newPos.x, m_StartPos.y + newPos.y, m_StartPos.z + newPos.z);  

        UpdateVirtualAxes(transform.position);
    }



    /// <summary>
    /// Method made to make allow the joystick to be displaced a dynamic amount of pixels
    /// </summary>
    private void DynamicReajustMovementRange(float modifier = 1f)
    {
        int wid = Screen.width;
        int hei = Screen.height;

        MovementRange = (int)(wid * modifier) / 11;  // change  to reajust
    }

    new void Start()
    {
        try
        {
            CameraPosition = GameObject.Find("ARCamera").GetComponent<Transform>();
            enableARCorrection();
        }
        catch (UnityException e)
        {
            Debug.LogWarning("No ARCamera Detected");
            DisableARCorrection();
        }

        DynamicReajustMovementRange();

        m_StartPos = transform.position;
        CreateVirtualAxes();

    }

}
