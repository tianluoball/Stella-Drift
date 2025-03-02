using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 10f;          // Base damage dealt to player
    public bool destroyOnCollision = true;    // Whether to destroy self on collision
    
    [Header("Advanced Settings")]
    public bool scaleBasedDamage = true;      // Whether to adjust damage based on size
    public float damageMultiplier = 1.0f;     // Damage multiplier for fine-tuning
    public string playerTag = "Player";       // Tag for player parts
    
    [Header("Effects")]
    public GameObject hitEffectPrefab;        // Hit effect prefab (optional)
    public float effectDuration = 1.0f;       // How long the effect lasts

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
        //Debug.Log($"碰撞检测触发：{gameObject.name} 碰到了 {collision.gameObject.name}");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
        //Debug.Log($"碰撞检测触发：{gameObject.name} 碰到了 {other.gameObject.name}");
    }
    
    private void HandleCollision(GameObject collidedObject)
    {
        //Debug.Log($"开始处理碰撞: {gameObject.name} 碰到 {collidedObject.name}, 标签: {collidedObject.tag}");
    
        // 检查标签
        if (!collidedObject.CompareTag(playerTag))
        {
            //Debug.Log($"对象不是玩家标签(期望 '{playerTag}')");
            return;
        }
        
        // 尝试直接获取
        PlayerHealth playerHealth = collidedObject.GetComponent<PlayerHealth>();
        //Debug.Log($"直接查找PlayerHealth: {(playerHealth != null ? "成功" : "失败")}");
        
        // 如果没找到，尝试在父对象查找
        if (playerHealth == null)
        {
            playerHealth = collidedObject.GetComponentInParent<PlayerHealth>();
            //Debug.Log($"在父对象查找PlayerHealth: {(playerHealth != null ? "成功" : "失败")}");
            
            // 如果还没找到，打印层级结构以便调试
            if (playerHealth == null)
            {
                //Debug.Log("在整个层级中查找PlayerHealth失败，打印对象层级:");
                Transform current = collidedObject.transform;
                while (current != null)
                {
                    //Debug.Log($"层级对象: {current.name}, 有PlayerHealth组件: {current.GetComponent<PlayerHealth>() != null}");
                    current = current.parent;
                }
                return;
            }
        }
        
        // 计算伤害
        float finalDamage = damageAmount * damageMultiplier;
        if (scaleBasedDamage)
        {
            float scaleMultiplier = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f;
            finalDamage *= scaleMultiplier;
        }
        
        //Debug.Log($"即将对玩家造成 {finalDamage} 点伤害, 当前血量: {playerHealth.currentHealth}");
        
        // 尝试造成伤害
        playerHealth.TakeDamage(finalDamage);
        
        //Debug.Log($"已造成伤害, 玩家现在的血量: {playerHealth.currentHealth}");
        
        // Spawn hit effect if set
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            
            // Auto-destroy effect
            Destroy(effect, effectDuration);
        }
        
        // Destroy self if configured to do so
        if (destroyOnCollision)
        {
            Destroy(gameObject);
        }
        
        //Debug.Log($"Object {gameObject.name} hit player part {collidedObject.name}, dealt {finalDamage} damage");
    }
}