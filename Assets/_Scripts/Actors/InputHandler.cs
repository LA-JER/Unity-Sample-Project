using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InputHandler : MonoBehaviour
{
    public delegate void CombineTurret(GameObject owner);
    public static event CombineTurret OnMouseDragEvent;
    public static event CombineTurret OnMouseExitEvent;
    public static event CombineTurret OnMouseEnterEvent;

    public delegate void MouseInput();
    public event MouseInput OnMouseUpEvent;
    public static event MouseInput OnMouseDragStopEvent;


    [SerializeField] private Turret owner;
    private Collider2D collide;
    private Vector2 originalPosition;
    private bool isDragging = false;
    private Vector2 offset;
    private int activations = 0;
    private bool canInput = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (owner != null)
        {
            owner.OnActivate += Owner_OnActivate;
        } else
        {
            Debugger.Log(Debugger.AlertType.Error, "Did not have owner(Turret) set!");
        }
        collide = GetComponent<Collider2D>();
        CombineManager.OnPossibleConsume += CombineManager_OnPossibleConsume;
        CombineManager.OnNoConsume += CombineManager_OnNoConsume;
    }

    private void CombineManager_OnNoConsume(GameObject consumer, GameObject eaten)
    {
        if(eaten == owner.gameObject)
        {
            canInput = true;
        }
    }

    private void CombineManager_OnPossibleConsume(GameObject consumer, GameObject eaten)
    {
        if (eaten == owner.gameObject)
        {
            canInput = false;
        }
    }

    private void Owner_OnActivate()
    {
        if (collide == null) return;
        collide.enabled = true;
        if(activations == 0)
        {
            originalPosition = transform.position;
        }
        activations++;
    }

    private void OnMouseUp()
    {
        //Debug.Log($"{owner.name} mouse up");
        if(canInput)
        {
            OnMouseUpEvent?.Invoke();
        }
        
    }
    

    private void Update()
    {
        if(isDragging)
        {
            if(Input.GetMouseButtonUp(0))
            {
                ReActivate();
            } else
            {
                MoveOwner();
            }


        }
    }

    void ReActivate()
    {
        isDragging = false;
        owner.transform.position = originalPosition;
        IPurchaseAble purchase = owner;
        purchase.Activate();

        OnMouseDragStopEvent?.Invoke();
    }

    void MoveOwner()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        owner.transform.position = mouseWorld + offset;
        //owner.transform.position += new Vector3(offset.x, offset.y, 0);
    }

    private void OnMouseEnter()
    {
        OnMouseEnterEvent?.Invoke(owner.gameObject);
    }

    private void OnMouseExit()
    {
        OnMouseExitEvent?.Invoke(owner.gameObject);
    }

    public void OnMouseDrag()
    {
        if (CombineManager.instance == null) return;

        if (!isDragging)
        {

            owner.Deactivate(null);
            isDragging = true;
            collide.enabled = false;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = owner.transform.position - mouseWorld;

            OnMouseDragEvent?.Invoke(owner?.gameObject);
        }
        
    }
    
    private void OnDestroy()
    {
        if (owner != null)
        {
            owner.OnActivate -= Owner_OnActivate;
        }
        CombineManager.OnPossibleConsume -= CombineManager_OnPossibleConsume;
        CombineManager.OnNoConsume -= CombineManager_OnNoConsume;
    }

    public Turret GetOwner()
    {
        return owner;
    }

}
