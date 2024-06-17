using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileHitEffect : MonoBehaviour
{
    public abstract void DoThing(Projectile owner, GameObject hitObject);
}
