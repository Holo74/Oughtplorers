using Godot;
using System;

///<summary>The basis for everything that is a kinematic body that also connects with health.  It should not be used by itself as it doesn't handle death</summary>
public class HealthKinematic : KinematicBody, Health
{
    private bool init = false;
    //Used in conjunction with another body that is considered the main body
    private HealthKinematic master;
    //Needs to be formated to have a certain amount of health
    public void Init(float maxHealth)
    {
        health = maxHealth;
        this.maxHealth = maxHealth;
        init = true;
    }
    public void Init(HealthKinematic parent, float health)
    {
        this.health = health;
        this.maxHealth = health;
        master = parent;
        init = true;
    }
    private float health = 0, maxHealth = 0;
    public float GetHealth()
    {
        if (!init)
            GD.Print("Health has not been initiated just yet on " + Name);
        return health;
    }
    public virtual bool TakeDamage(float damage, DamageType typing, Node Source)
    {
        if (master != null)
        {
            master.health -= damage;
        }
        Damaged(damage);
        return true;
    }
    protected void Damaged(float damage)
    {
        if (!init)
            GD.Print("Health has not been initiated just yet on " + Name);
        if (master != null)
        {
            master.health -= damage;
        }
        health -= damage;
    }
    public virtual bool IsDead()
    {
        return GetHealth() <= 0;
    }
    public virtual bool Heal(float amount)
    {
        health = Mathf.Clamp(health + amount, -1, maxHealth);
        return true;
    }
}
