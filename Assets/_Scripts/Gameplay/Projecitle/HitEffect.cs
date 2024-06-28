using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitEffect : MonoBehaviour
{
    public abstract void DoThing(DamageElement owner, GameObject hitObject);
}
