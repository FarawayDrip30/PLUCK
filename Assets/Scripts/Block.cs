using UnityEngine;

public class Block : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider bc;

    int currCollisions = 0;

    public bool placed = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        currCollisions++;
    }

    private void OnTriggerExit(Collider collision)
    {
        currCollisions--;
    }

    public int getCurrCollisions()
    {
        return currCollisions;
    }


    public void StartFalling()
    {
        bc.isTrigger = false;
        rb.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Wall")
        {
            if (collision.contacts[0].point.y < transform.position.y)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Abs(rb.linearVelocity.y) + Random.Range(5.0f, 20.0f), rb.linearVelocity.z);
            }
        }

    }
}
