public class DamageInstance
{
    public float damage = 0;
    public bool isCritical = false;

    public DamageInstance()
    {
        damage = 0;
        isCritical = false;
    }

    public DamageInstance(float damage)
    {
        this.damage = damage;
        isCritical = false;
    }
    public DamageInstance(float damage, bool isCritical)
    {
        this.damage = damage;
        this.isCritical = isCritical;
    }

    public void SetIsCritical(bool isCritical)
    {
        this.isCritical = isCritical;
    }
}