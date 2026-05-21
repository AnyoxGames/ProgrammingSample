using System.Collections.Generic;
using UnityEngine;

public abstract class ADamageable : MonoBehaviour
{
    //Use of the Odin Inspector package, commented out as it's not included with this project
    [/*BoxGroup("Damageable"), */SerializeField] private float maxHealth = 100;
    [/*BoxGroup("Damageable"), */SerializeField] private bool initialInvulnerable = false;
    [/*BoxGroup("Damageable"), */SerializeField] private bool initialHealable = true;
    
    private readonly List<string> invulnerableReasons = new();
    private readonly List<string> unhealableReasons = new();
    private float health;
    
    public float Health => health;
    public float MaxHealth => maxHealth;
    public bool IsInvulnerable => invulnerableReasons.Count > 0;
    public bool IsHealable => unhealableReasons.Count == 0;
    
    protected virtual void Awake()
    {
        health = MaxHealth;

        if (initialInvulnerable)
        {
            invulnerableReasons.Add("initial_invulnerable");
        }

        if (!initialHealable)
        {
            unhealableReasons.Add("initial_unhealable");
        }
    }

    public void TakeDamage(float amount, IDamageInvoker fromSource = null)
    {
        if (IsInvulnerable)
            return;
        
        health =  Mathf.Clamp(Health - amount, 0f, MaxHealth);
        OnDamaged(Mathf.Min(amount, MaxHealth - amount), fromSource);

        if (health == 0f)
        {
            OnDeath(fromSource);
        }
    }

    public void Heal(float amount, IHealInvoker fromSource = null)
    {
        if (!IsHealable)
            return;
        
        health = Mathf.Clamp(Health + amount, 0, MaxHealth);
        OnHealed(Mathf.Min(amount, MaxHealth - amount), fromSource);
    }

    protected virtual void OnHealed(float amountHealed, IHealInvoker fromSource) { }
    protected virtual void OnDamaged(float amountDamaged, IDamageInvoker fromSource) { }
    protected virtual void OnDeath(IDamageInvoker fromSource)
    {
        Destroy(gameObject);
    }
}
