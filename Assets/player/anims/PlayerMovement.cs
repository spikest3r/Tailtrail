using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    

    // properties
    public bool AllowedToMove { private set; get; } // used in dialog control etc.
    [SerializeField] float Speed = 2f;

    public void SetAllowedToMove(bool state)
    {
        if (!state) TriggerAnim(true, currentDir); // stop movement anim if flag is false
        AllowedToMove = state;
    }

    // system
    Animator anim;
    public Direction currentDir = Direction.DOWN; // default dir
    bool move = false;
    Dictionary<KeyControl, Direction> keyBinds;
    Vector2[] vectorBinds;
    [SerializeField] LayerMask ObstacleMask;
    

    void TriggerAnim(bool idle, Direction dir)
    {
        if (dir == Direction.UNSPECIFIED) return;
        currentDir = dir;
        anim.SetBool("Idle", idle);
        anim.SetInteger("Direction", (int)dir);
        move = !idle;
        anim.SetTrigger("Change");
    }

    private void Awake()
    {
        AllowedToMove = true; // default
        anim = GetComponent<Animator>();

        Keyboard c = Keyboard.current;
        keyBinds = new()
        {
            { c.aKey, Direction.LEFT },
            { c.leftArrowKey, Direction.LEFT },
            { c.dKey, Direction.RIGHT },
            { c.rightArrowKey, Direction.RIGHT },
            { c.wKey, Direction.UP },
            { c.upArrowKey, Direction.UP },
            { c.sKey, Direction.DOWN },
            { c.downArrowKey, Direction.DOWN },
        };
        vectorBinds = new Vector2[] { Vector2.down, Vector2.up, Vector2.left, Vector2.right };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!AllowedToMove) return;
        foreach(var pair in keyBinds)
        {
            var key = pair.Key;
            var dir = pair.Value;

            if (key.wasPressedThisFrame)
            {
                TriggerAnim(false, dir);
            }

            if (key.wasReleasedThisFrame)
            {
                if (!move || dir != currentDir) continue; // if not moving then skip this
                TriggerAnim(true, dir);
            }
        }
    }

    private void FixedUpdate()
    {
        if(move && AllowedToMove)
        {
            Vector2 direction = vectorBinds[(int)currentDir];
            RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, .2f, ObstacleMask);
            Debug.DrawRay(transform.position, direction);
            if (ray.collider != null) return; // dont modify position if ray != null
            transform.position += Speed * Time.deltaTime * (Vector3)direction;
        }
    }

    public IEnumerator ArtificialMove(float dirOffset, Direction dir = Direction.UNSPECIFIED, float customSpeed = -1f)
    {
        Direction movingDir = dir == Direction.UNSPECIFIED ? currentDir : dir;
        float movingSpeed = customSpeed <= 0 ? Speed : customSpeed;
        Vector3 vectorDir = vectorBinds[(int)movingDir];
        Vector3 target = transform.position + vectorDir * dirOffset;
        TriggerAnim(false, movingDir);
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, movingSpeed * Time.deltaTime);
            yield return null;
        }
        TriggerAnim(true, movingDir);
    }

    public Vector2 GetVectorDirection()
    {
        return vectorBinds[(int)currentDir];
    }
}
