using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DoorController : MonoBehaviour
{
    public int rotation = 0;
    public bool isOpened = false;
    public BoxCollider2D closedDoorCollider;
    public CircleCollider2D doorTrigger;
    public Animator animator;
    public GameplayManager.Direction doorDirection;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //closedDoorCollider = GetComponent<BoxCollider2D>();
        SetDoorState(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            SetDoorState(!isOpened);
        }
    }

    public void SetDoorState(bool ds)
    {
        isOpened = ds;
        animator.SetBool("doorOpen", ds);
        closedDoorCollider.enabled = !ds;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && col.isTrigger)
        {
            GameObject.Find("/GameplayManager").GetComponent<GameplayManager>().ChangeRoom(doorDirection);
        }
    }

}
