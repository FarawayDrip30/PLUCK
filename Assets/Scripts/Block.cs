using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider bc;
    RandomColourFromMaterial rcfm;

    int currCollisions = 0;

    public bool placed = false;

    public bool beingPlucked = false;

    [SerializeField] float bounceMin;
    [SerializeField] float bounceMax;

    [SerializeField] float velBounceMult;

    List<Block> connectedBlocks;

    RandomAudio bounceRandomAudio;
    RandomAudio thudRandomAudio;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
        rcfm = GetComponent<RandomColourFromMaterial>();

        rcfm.generateMaterial();

        connectedBlocks = new List<Block>();

        bounceRandomAudio = RandomAudio.singletons["Bounce"];
        thudRandomAudio = RandomAudio.singletons["Thud"];

        transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
    }

    private void OnTriggerEnter(Collider collision)
    {
        currCollisions++;
        Block collidedBlock = collision.GetComponent<Block>();
        if (collidedBlock != null) {
            connectedBlocks.Add(collidedBlock);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        removedContact();
        Block collidedBlock = collision.GetComponent<Block>();
        if (collidedBlock != null)
        {
            connectedBlocks.Remove(collidedBlock);
        }
    }

    public void removedContact()
    {
        currCollisions--;
    }

    public void hasBeenRemoved()
    {
        foreach (Block block in connectedBlocks)
        {
            block.removedContact();
        }

        connectedBlocks.Clear();
    }

    public int getCurrCollisions()
    {
        return currCollisions;
    }


    public void StartFalling()
    {
        bc.isTrigger = false;
        rb.isKinematic = false;


        rcfm.applyMaterial();

        thudRandomAudio.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Wall")
        {
            if (collision.contacts[0].point.y < transform.position.y)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Abs(rb.linearVelocity.y) * velBounceMult + Random.Range(bounceMin, bounceMax), rb.linearVelocity.z);
                bounceRandomAudio.Play();
            }
            else
            {
                thudRandomAudio.Play();
            }
        }
        else
        {
            thudRandomAudio.Play();
        }
    }

    public void setCurrCollisions(int newCurrCollisions)
    {
        currCollisions = newCurrCollisions;
    }
}
