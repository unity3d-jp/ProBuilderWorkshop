using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PingPongLift : MonoBehaviour
{
    public float duration = 4f;
    public Vector3 targetPosition;
    private Vector3 initPosition;

    private Rigidbody m_Rigidbody;
    public Rigidbody _rigidbody
    {
        get
        {
            if (m_Rigidbody == null) m_Rigidbody = GetComponent<Rigidbody>();
            return m_Rigidbody;
        }
    }

    private void Awake()
    {
        initPosition = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + targetPosition, 0.2f);
        Gizmos.DrawLine(transform.position, transform.position + targetPosition);
    }

    void FixedUpdate()
    {
        var pingpong = Mathf.PingPong(Time.fixedTime / duration, 1f);
        var value = Mathf.SmoothStep(0, 1f, pingpong);

        //_rigidbody.AddForce(new Vector3(0.1f, 0, 0), ForceMode.VelocityChange);
        _rigidbody.MovePosition(Vector3.Lerp(initPosition, initPosition + targetPosition, value));
    }
}
