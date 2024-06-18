using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffVFXManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializedDictionary("Buff ID", "VFX Sprite")]
    [SerializeField] public SerializedDictionary< int, Sprite> buffVFX = new SerializedDictionary< int, Sprite>();

    private BuffManager buffManager;
    // Start is called before the first frame update
    void Start()
    {
        buffManager = GetComponent<BuffManager>();
        if(buffManager != null)
        {
            buffManager.OnBuffApply += BuffManager_OnBuffApply;
        } else
        {
            Debugger.Log(Debugger.AlertType.Warning, "Could not find buff manager, did you forget to add the component?");
        }
    }

    private void BuffManager_OnBuffApply(int buffID, bool isActive)
    {
        if(spriteRenderer != null)
        {
            if(buffVFX.ContainsKey(buffID))
            {
                if (isActive)
                {
                    Sprite cfx = buffVFX[buffID];
                    spriteRenderer.sprite = cfx;
                    spriteRenderer.enabled = true;
                }
                else
                {
                    spriteRenderer.enabled = false;
                }
            }
            
        }
        else
        {
            Debugger.Log(Debugger.AlertType.Warning, "Did not have sprite render set, did you forget to add the component?");
        }
    }


    private void OnDestroy()
    {
        if (buffManager != null)
        {
            buffManager.OnBuffApply -= BuffManager_OnBuffApply;
        }
    }
}
