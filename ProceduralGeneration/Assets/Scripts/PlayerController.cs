using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    private Rigidbody rb;
    private Vector3 moveDirection;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 5, -5);
        Camera.main.transform.LookAt(transform);
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveX, 0, moveY).normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}
