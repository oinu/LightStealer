using UnityEngine;
using System.Collections;

public class PJ : MonoBehaviour {

    bool attack, walk, firstJump,jump;
    int life,energy;
    AnimatorStateInfo state;
    BoxCollider2D col;
    SpriteRenderer render;
    Rigidbody2D rigidBody;
    public GameObject gm;


    // Use this for initialization
    void Start () {
        attack = false;
        firstJump = true;
        jump = true;
        col = this.GetComponent<BoxCollider2D>();
        render = this.GetComponent<SpriteRenderer>();
        rigidBody = this.GetComponent<Rigidbody2D>();
        life = 100;
        energy = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        #region INPUTS
        if (Input.GetAxis("Fire1") > 0)
        {
            attack = true;
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            walk = true;
            render.flipX = false;
            this.transform.position = new Vector3(this.transform.position.x + 0.05f, this.transform.position.y, this.transform.position.z);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            walk = true;
            render.flipX = true;
            this.transform.position = new Vector3(this.transform.position.x - 0.05f, this.transform.position.y, this.transform.position.z);
        }
        else
        {
            attack = false;
            walk = false;
        }

        if(Input.GetAxis("Jump")!=0)
        {
            if (jump)
            {
                if (firstJump)
                {
                    rigidBody.AddForce(new Vector2(0.0f, 10.0f), ForceMode2D.Impulse);
                    rigidBody.gravityScale = 1f;
                    firstJump = false;
                }
                else
                {
                    rigidBody.gravityScale = 0.5f;
                    rigidBody.AddForce(new Vector2(0.0f, 5f), ForceMode2D.Impulse);
                    
                }
                jump = false;
            }
        }
        else if(Input.GetAxis("Jump") == 0)
        {
            jump = true;
        }
        #endregion

        #region ANIMATIONS
        state = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        col.offset = new Vector2(0, col.offset.y);
        col.size = new Vector2(0.8f, col.size.y);

        if (state.IsName("Attack"))
        {
            this.GetComponent<Animator>().SetBool("Attack", false);
            attack = false;
        }
        else if(state.IsName("Attack2"))
        {
            col.offset= new Vector2(1, col.offset.y);
            col.size = new Vector2(4, col.size.y);
        }

        else if (state.IsName("Idle"))
        {
            if (attack)
            {
                this.GetComponent<Animator>().SetBool("Attack", true);
            }
            else if(walk)
            {
                this.GetComponent<Animator>().SetBool("Walk", true);
            }
            else
            {
                this.GetComponent<Animator>().SetBool("Walk", false);
                this.GetComponent<Animator>().SetBool("Attack", false);
            }
        }

        else if(state.IsName("Walk"))
        {
            if (!walk)
            {
                this.GetComponent<Animator>().SetBool("Walk", false);
            }
            else if (attack)
            {
                this.GetComponent<Animator>().SetBool("Walk", false);
                this.GetComponent<Animator>().SetBool("Attack", true);
            }
        }
        #endregion
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (state.IsName("Attack2") && col.gameObject.tag=="Enemy")
        {
            Destroy(col.gameObject);
            energy += 10;
        }
        if(col.gameObject.tag=="Bullet")
        {
            life--;
            GameManager g=gm.GetComponent<GameManager>();
            foreach(Bullet b in g.bulletsList)
            {
                if(b.obj.transform.position==col.gameObject.transform.position)
                {
                    b.time = b.maxTime;
                }
            }

        }
        else if(col.gameObject.tag=="Terrain")
        {
            firstJump = true;
        }
    }
}
