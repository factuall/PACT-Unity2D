using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int rotation = 0;
    public bool isOpened = false;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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

    }

}
