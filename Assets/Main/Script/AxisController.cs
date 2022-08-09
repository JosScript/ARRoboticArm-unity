using UnityEngine;

public class AxisController : MonoBehaviour
{
    [SerializeField] FixedJoystick joystickXY;
    [SerializeField] FixedJoystick joystickZ;
    private Vector3 direction;

    public Vector3 GetAxis()
    {
        return direction;
    }

    private void SetAxis()
    {
        direction = Vector3.right * joystickXY.Horizontal + Vector3.up * joystickXY.Vertical + Vector3.forward * joystickZ.Vertical;
    }

    private void Update()
    {
        //Debug.Log("Joystick: " + GetAxis().ToString());
    }

    private void FixedUpdate()
    {
        SetAxis();
    }
}
