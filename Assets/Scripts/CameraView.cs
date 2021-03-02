using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public Vector3 Lockrot;
    public Vector3 Lockpos;
    public Quaternion Lockrot1;
    public Vector3 Lockpos1;
    public GameObject Car;
    public CarControllers carControllers;

    private void Start()
    {
        carControllers = Car.GetComponent<CarControllers>();
    }

    void LateUpdate() {
        if (carControllers.IsGrounded())
            transform.rotation = Quaternion.Euler(Lockrot.x, Car.transform.eulerAngles.y, Lockrot.z);
        transform.position = Car.transform.position + transform.TransformVector(Lockpos);
    }
}
