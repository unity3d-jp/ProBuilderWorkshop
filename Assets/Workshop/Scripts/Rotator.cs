using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float force = 200f;
    public Axis axis = Axis.x;
    public enum Axis { x, y, z };

    private Rigidbody m_Rigidbody;
    public Rigidbody _rigidbody
    {
        get
        {
            if (m_Rigidbody == null) m_Rigidbody = GetComponent<Rigidbody>();
            return m_Rigidbody;
        }
    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, force, 0), ForceMode.Impulse);
    }

    // void Update()
    // {
    //     switch (axis)
    //     {
    //         case Axis.x:
    //             transform.Rotate(Time.deltaTime * force, 0, 0);
    //             break;
    //         case Axis.y:
    //             transform.Rotate(0, Time.deltaTime * force, 0);
    //             break;
    //         case Axis.z:
    //             transform.Rotate(0, 0, Time.deltaTime * force);
    //             break;
    //     }
    // }
}
