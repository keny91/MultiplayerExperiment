using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

public class CustomJoystick : Joystick {

    public bool ARcorrection = true;
    public Transform CameraPosition;

    new void OnEnable()
    {
        CreateVirtualAxes();
    }

    private new void CreateVirtualAxes()
    {
        // set axes to use
        m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
        m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

        // create new axes based on axes to use
        if (m_UseX)
        {
            m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
        }
        if (m_UseY)
        {
            m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
        }

    }



    private Vector3 CalculateJoyStickAngle(Vector3 value)
    {
        float valX = value.x;
        valX = -valX;
        float valY = value.y;
        float newValX, newValY, newValR;
        float angleJoystick, newAngleJoystick;

        float R = Mathf.Sqrt(Mathf.Pow(valX, 2) + Mathf.Pow(valY, 2));
        Mathf.Acos(valX / R);
        Debug.LogWarning("value X = " + valX + "__  value Y = " + valY);

        float angle1 = Mathf.Acos(valX / R);
        float angle2 = Mathf.Asin(valY / R);

        angle1 = RadianToDegree(angle1);
        angle2 = RadianToDegree(angle2);

        if (angle2 < 0)
            angle1 = 360 - angle1;

        //Debug.LogWarning("ESTIMATED ANGLE1 = " + angle1 + "__  angle2 = " + angle2);
        //s Debug.LogWarning("ESTIMATED ANGLE1 = " + RadianToDegree(angle1) + "__  angle2 = " + RadianToDegree(angle2));

        //Debug.Log("ESTIMATED ANGLE1 = " + Mathf.Acos(valX / R) + "__  angle2 = " + Mathf.Asin(valY / R));

        newValX = 0;
        newValY = 0;

        /*
        //Debug.LogError(Mathf.Sin(DegreeToRadian(30)) +" "+ Mathf.Sin(DegreeToRadian(30+360)));
        
                float RotationCompensation;
                if (CameraPosition.rotation.y < 0)
                    RotationCompensation = CameraPosition.rotation.y + 360;
                else
                    RotationCompensation = CameraPosition.rotation.y;

                angle1 =+RotationCompensation;
        //angle2 =+RotationCompensation;
        */
        float RotationCompensation;
        if (CameraPosition.rotation.eulerAngles.y < 0)
            RotationCompensation = CameraPosition.rotation.eulerAngles.y + 360;
        else
            RotationCompensation = CameraPosition.rotation.eulerAngles.y;
        //Debug.LogError("Camera Compensation = " + RotationCompensation + "Camera rot: " + CameraPosition.rotation.y);

        angle1 -= RotationCompensation;

        if (angle1 > 360)
            angle1 -= 360;
        else if (angle1 < 0)
            angle1 += 360;


        if (valX == 0 && valY == 0)
        {
            newValX = 0;
            newValY = 0;
        }
        // First quadrant 0 - 90
        // else if (valX>0 && valY > 0)
        else if (angle1<90 && angle1 >= 0)
        {
          //  Debug.LogWarning("ESTIMATED ANGLE1 = " + angle1 + "__  angle2 = " + angle2 +" MOVING IN QUADRANT 1");
            newValX = Mathf.Cos(DegreeToRadian(angle1));
            newValY = Mathf.Sin(DegreeToRadian(angle1));
        }
        // Second quadrant 90 - 180
        //else if (valX < 0 && valY > 0) 
        else if (angle1 < 180 && angle1 >= 90)
        {
          //  Debug.LogWarning("ESTIMATED ANGLE1 = " + angle1 + "__  angle2 = " + angle2 + " MOVING IN QUADRANT 2");
            newValX = Mathf.Cos(DegreeToRadian(angle1));
            newValY = Mathf.Sin(DegreeToRadian(angle1));
        }
        // Third quadrant 180 -270 (-180 - -90)
        //else if (valX < 0 && valY < 0)
        else if (angle1 < 270 && angle1 >= 180)
        {
           // Debug.LogWarning("ESTIMATED ANGLE1 = " + angle1 + "__  angle2 = " + angle2 + " MOVING IN QUADRANT 3");
            newValX = Mathf.Cos(DegreeToRadian(angle1));
            newValY = Mathf.Sin(DegreeToRadian(angle1));
        }
        // Third quadrant 270 - 360 (-90 - 0)

        //else if (valX >= 0 && valY < 0)
        else if (angle1 < 360 && angle1 >= 270)
        {
         //   Debug.LogWarning("ESTIMATED ANGLE1 = " + angle1 + "__  angle2 = " + angle2 + " MOVING IN QUADRANT 4");
            newValX = Mathf.Cos(DegreeToRadian(angle1));
            newValY = Mathf.Sin(DegreeToRadian(angle1));
        }

        Vector3 vec = new Vector3(newValX, newValY, 0);
        return vec;

    }


    private float DegreeToRadian(float angle)
    {
        return Mathf.PI * angle / 180.0f;
    }

    private float RadianToDegree(float angle)
    {
        return 180.0f * angle /  Mathf.PI;
    }

    /*
     private void CalculateJoyStickAngle(Vector3 value, float angle)
    {
        float valX = value.x;
        float valY = value.y;
        float newValX, newValY, newValR;
        float angleJoystick, newAngleJoystick;

        float R = Mathf.Sqrt(Mathf.Pow(valX, 2) + Mathf.Pow(valX, 2));


        
    }
         */



    public override void UpdateVirtualAxes(Vector3 value)
    {
        var delta = m_StartPos - value;
        delta.y = -delta.y;
        delta /= MovementRange;

        delta  = CalculateJoyStickAngle(delta);
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
        transform.position = Vector3.ClampMagnitude(new Vector3(newPos.x, newPos.y, newPos.z), MovementRange) + m_StartPos;
        //transform.position = new Vector3(m_StartPos.x + newPos.x, m_StartPos.y + newPos.y, m_StartPos.z + newPos.z);

        
        UpdateVirtualAxes(transform.position);
    }


    new void Start()
    {
        CameraPosition = GameObject.Find("ARCamera").GetComponent<Transform>();
        m_StartPos = transform.position;
    }

}
