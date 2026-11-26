using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyMovement : MonoBehaviour
{ 
    private Rigidbody2D rig;
private Animator anim;

public float speed;

public Transform rightcol;
 public Transform leftcol;

 public Transform headpoint;
 private bool colliding;

    public LayerMask layer;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rig.velocity = new Vector2(speed, rig.velocity.y);

        colliding = Physics2D.Linecast(rightcol.position, leftcol.position,layer);


        if (colliding) 
        {
         
            speed = -speed;

        }
    }
}
