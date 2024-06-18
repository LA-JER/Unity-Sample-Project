using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileMovement : MonoBehaviour
{
    public Projectile projectile;
    public abstract void MoveProjectile();
}
