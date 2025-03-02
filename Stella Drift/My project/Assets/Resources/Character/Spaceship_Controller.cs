using UnityEngine;
using Leap;

public class LeapRotationController : MonoBehaviour
{
    private LeapProvider provider;
    private Transform objectTransform;
    
    // 子弹相关设置
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    
    // 发射控制
    private float nextFireTime = 0f;
    public float fireRate = 0.5f;
    
    // 手势状态追踪
    private bool previousHandClosed = false;
    
    void Start()
    {
        provider = FindFirstObjectByType<LeapProvider>();
        if (provider == null)
        {
            Debug.LogError("Scene is missing a LeapProvider!");
            enabled = false;
            return;
        }
        
        objectTransform = transform;
        
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    void Update()
    {
        Frame frame = provider.CurrentFrame;
        if (frame != null && frame.Hands.Count > 0)
        {
            Hand hand = frame.Hands[0];
            
            if (hand != null)
            {
                // 恢复最初的旋转逻辑
                Vector3 palmNormal = new Vector3(hand.PalmNormal.x, hand.PalmNormal.y, hand.PalmNormal.z);
                Vector3 palmDirection = new Vector3(hand.Direction.x, hand.Direction.y, hand.Direction.z);
                
                float rotZ = Mathf.Atan2(palmNormal.x, -palmNormal.y) * Mathf.Rad2Deg;
                float rotX = -Mathf.Atan2(palmDirection.y, palmDirection.z) * Mathf.Rad2Deg;
                float rotY = Mathf.Atan2(palmDirection.x, palmDirection.z) * Mathf.Rad2Deg;
                
                objectTransform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
                
                // 使用改进后的握拳检测和开火逻辑
                bool currentHandClosed = IsHandClosed(hand);
                
                if (currentHandClosed && !previousHandClosed && Time.time >= nextFireTime)
                {
                    FireBullet();
                    nextFireTime = Time.time + 1f/fireRate;
                }
                
                previousHandClosed = currentHandClosed;
            }
        }
        else
        {
            // 重置手势状态
            previousHandClosed = false;
        }
    }
    
    private bool IsHandClosed(Hand hand)
    {
        float averageBend = 0f;
        Vector3 handDirection = new Vector3(hand.Direction.x, hand.Direction.y, hand.Direction.z);
        
        int validFingers = 0;
        foreach (Finger finger in hand.fingers)
        {
            if (finger.Type == Finger.FingerType.THUMB) continue;
            
            Vector3 fingerDirection = new Vector3(finger.Direction.x, finger.Direction.y, finger.Direction.z);
            averageBend += Vector3.Angle(fingerDirection, handDirection);
            validFingers++;
        }
        
        if (validFingers == 0) return false;
        
        averageBend /= validFingers;
        return averageBend > 65f;
    }
    
    private void FireBullet()
    {
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                Vector3 shootDirection = transform.forward;
                rb.AddForce(shootDirection * bulletSpeed, ForceMode.Impulse);
                
                Debug.Log($"Bullet velocity: {rb.linearVelocity}");
                Debug.DrawRay(firePoint.position, shootDirection * 5f, Color.red, 2f);
            }
            
            Destroy(bullet, 5f);
        }
    }
}