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

    public bool SingleAxisInput = false;

    private GameObject JoystickObject;
    protected Vector3 JoystickObject_m_StartPos;
    public bool PointerIsDown = false;
    private float calibrationAngle = 0f;



    public void configureJoystick(bool Single, float calibANgle)
    {
        //Debug.LogWarning("JOYSTICK DONE");
        SingleAxisInput = Single;
        calibrationAngle = calibANgle;
    }

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
    /// Based on the position of the ARCamera, get the values of the axis when the Joystick is dragged.
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
        angle1 += calibrationAngle;
        // We operate in the 0 to 359 degrees region. So we will ajust any other angles that go over or under this range.
        while (angle1 > 360)
            angle1 -= 360;

        while (angle1 < 0)
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
    /// Based on the position of the ARCamera, get the values of the axis when the Joystick is dragged.
    /// <para> Alternative Syntax.</para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private void CalculateJoyStickAngle(ref Vector3 value)
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
        angle1 += calibrationAngle;
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

        value = new Vector3(newValX, newValY, 0);


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
    public new void UpdateVirtualAxes(Vector3 value)
    {

        // If same position, skip all
        if (value == m_StartPos)
        {
            m_HorizontalVirtualAxis.Update(0);
            m_VerticalVirtualAxis.Update(0);
            return;
        }

        var delta = m_StartPos - value;
        delta.y = -delta.y;
        delta /= MovementRange;

       



        // Only do the correction in if it has been activated and is p
        if (ARcorrection)
            delta = CalculateJoyStickAngle(delta);
        //CalculateJoyStickAngle(ref delta);
        //Debug.Log("updating axis");
        

        if (SingleAxisInput)
        {
            if (m_UseX)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    m_HorizontalVirtualAxis.Update(-delta.x);
            }

            if (m_UseY)
            {
                if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y))
                    m_VerticalVirtualAxis.Update(delta.y);
            }
        }
        else
        {
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Update(delta.x);
            }

            if (m_UseY)
            {
                m_VerticalVirtualAxis.Update(delta.y);
            }
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
        int delta;


        if (m_UseX)
        {
            //int delta = (int)(data.position.x - m_StartPos.x);
            delta = (int)(data.position.x - m_StartPos.x);
            //delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
            newPos.x = delta;
        }

        if (m_UseY)
        {
            //int delta = (int)(data.position.y - m_StartPos.y);
            delta = (int)(data.position.y - m_StartPos.y);
            //delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
            newPos.y = delta;
        }

        Vector3 value = Vector3.ClampMagnitude(new Vector3(newPos.x, newPos.y, newPos.z), MovementRange) + JoystickObject_m_StartPos;
        JoystickObject.transform.position = value;

        /* 
            if (newPos.x >= newPos.y) 
            { 
                newPos.y = 0; 
            } 
            else if (newPos.x < newPos.y) 
                newPos.x = 0; 
*/
        value = Vector3.ClampMagnitude(new Vector3(newPos.x, newPos.y, newPos.z), MovementRange) + m_StartPos;
        //JoystickObject.transform.position = value; 
        UpdateVirtualAxes(value);
    }


    public override void OnPointerDown(PointerEventData data)
    {
        //  Debug.LogWarning("POINTER IS DOWN DETECTED");
        if (!PointerIsDown)
        {

            m_StartPos = data.position;
            //  Debug.LogWarning("Selected dragging center" + m_StartPos);
            PointerIsDown = true;
        }

    }

    public override void OnPointerUp(PointerEventData data)
    {
        //transform.position = m_StartPos; 
        JoystickObject.transform.position = JoystickObject_m_StartPos;
        UpdateVirtualAxes(m_StartPos);
        PointerIsDown = false;
    }



    /// <summary>
    /// Depending on the configuration of the Joystick controller.
    /// If "InvertedX"/ "InvertedY" invert the corresponding axis input.
    /// If "SingleAxisInput": the output will be a vector with only the largest axis; the other will be set to 0. Any non-null value outputed this way will be either 0.5 or 1. 
    /// The regular vector is returned otherwise.
    /// </summary>
    /// <returns> Returns the vector2 with both axis represented as a -1 to 1 value.</returns>
    public Vector2 getAxisOutput()
    {
        float valueX = 0;
        float valueY = 0;
        Vector2 Output;
        if (m_UseX)
            valueX = CrossPlatformInputManager.GetAxisRaw(horizontalAxisName);
        if (m_UseY)
            valueY = CrossPlatformInputManager.GetAxisRaw(verticalAxisName);


        float x = Mathf.Abs(valueX);
        float y = Mathf.Abs(valueY);


        if (SingleAxisInput)
        {
            if (x > y)
            {
                valueX = (x > 0.5) ? 1f * Mathf.Sign(valueX) : 0.5f * Mathf.Sign(valueX);
                valueY = 0;
            }

            if (x < y)
            {
                valueY = (y > 0.5) ? 1f * Mathf.Sign(valueY) : 0.5f * Mathf.Sign(valueY);
                valueX = 0;
            }

        }
        else
        {

            valueY = -valueY;
            /*
            if (x != 0)
                valueX = -1f * Mathf.Sign(valueX);
            if (y != 0)
                valueY = 1f * Mathf.Sign(valueY);
                */
        }

        Output = new Vector2(valueX, valueY);

        //  Debug.LogError("AXIS OUTPUT: " + Output);

        return Output;
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

    void Start()
    {

        JoystickObject = transform.FindChild("MobileJoystick").gameObject;

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

        // m_StartPos = transform.position;
        JoystickObject_m_StartPos = JoystickObject.transform.position;
        CreateVirtualAxes();

    }

}

