using UnityEngine;

public class Bullet : MonoBehaviour 
{
    private Rigidbody rb;
    public float bulletSpeed = 20f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 确保子弹不受重力影响
        if(rb != null)
        {
            rb.useGravity = false;
        }
    }
}