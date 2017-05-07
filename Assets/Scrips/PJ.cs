using UnityEngine;
using System.Collections;

public class PJ : MonoBehaviour {

    public bool attack, walk, firstJump,shoot, hit,liana,balance;
    int life,energy,tries,initalLife,puntsPassatsLiana;
    AnimatorStateInfo state;
    BoxCollider2D col;
    SpriteRenderer render;
    Rigidbody2D rigidBody;
    public GameObject gm;
    public GameObject bullet;
    public Texture heart;
    public Texture pjTexture;
    GameObject savePosition;
    GameObject[] puntsLiana;


    // Use this for initialization
    void Start () {
        attack = false;
        firstJump = true;
        puntsPassatsLiana = 0;
        col = this.GetComponent<BoxCollider2D>();
        render = this.GetComponent<SpriteRenderer>();
        rigidBody = this.GetComponent<Rigidbody2D>();
        initalLife = 5;
        life = initalLife;
        tries = 3;
        energy = 0;
        shoot = true;
        savePosition = new GameObject("SavePoint");
        savePosition.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log(balance);
        //Si cau el buit o es queda sensa vides
        if (life == 0)
        {
            Mort();
        }
        else
        {
            #region INPUTS

            //Si ataca amb el latig
            if (Input.GetAxis("Fire1") > 0 && !balance)
            {
                attack = true;
            }

            //Si dispara el superAtac
            else if (Input.GetAxis("Fire2") > 0 && !balance)
            {
                this.GetComponent<Animator>().SetBool("SuperAttack", true);
            }

            //Si esta aprop d'un lloc on pot utilitzar les lianes i apreta la tecla
            else if (Input.GetAxis("Fire3") > 0 && liana && !balance)
            {
                if(this.transform.position.x<puntsLiana[1].transform.position.x)
                {
                    this.transform.position = puntsLiana[0].transform.position;
                    render.flipX = false;
                }
                else
                {
                    this.transform.position = puntsLiana[2].transform.position;
                    render.flipX = true;
                }
                puntsLiana[3].transform.position = puntsLiana[1].transform.position;
                puntsPassatsLiana++;
                balance = true;
            }

            //Si camina cap a la dreta i no ha rebut cap impacte
            else if (Input.GetAxis("Horizontal") > 0 && !hit && !balance)
            {
                //Esta caminant
                walk = true;

                //En cas de que faci un canvi de sentit, fem un gir a l'sprite.
                if (render.flipX) render.flipX = false;

                //Movem el pj
                this.transform.position = new Vector3(this.transform.position.x + 0.05f, this.transform.position.y, this.transform.position.z);
            }

            //Si camina cap a l'esquerra i no ha rebut cap impacte
            else if (Input.GetAxis("Horizontal") < 0 && !hit && !balance)
            {
                //Esta caminant
                walk = true;

                //Si ha fet un canvi de sentit, fem el gir a l'sprite
                if (!render.flipX) render.flipX = true;

                //Movem el pj
                this.transform.position = new Vector3(this.transform.position.x - 0.05f, this.transform.position.y, this.transform.position.z);
            }

            //Si no esta fent res
            else
            {
                attack = false;
                walk = false;
                //balance = false;
                this.GetComponent<Animator>().SetBool("SuperAttack", false);
            }

            //Si salta
            if (Input.GetAxis("Jump") != 0 && firstJump)
            {
                //Li donem una força
                rigidBody.AddForce(new Vector2(0.0f, 7.0f), ForceMode2D.Impulse);

                //Indiquem que ja esta saltant, per impedir que torni a saltar
                firstJump = false;
            }
            #endregion

            #region ANIMATIONS

            //Adquirim els components del personatge per tenir acces a ells,
            //d'una forma mes senzilla.
            //Agafem l'esta de la animacio
            state = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

            //Li donem les dimensions standers, de les colisio, del pj
            col.offset = new Vector2(0, col.offset.y);
            col.size = new Vector2(0.8f, col.size.y);

            //Si s'esta fent l'animacio d'atac
            if (state.IsName("Attack"))
            {
                //Indiquem al Animator que canvii l'estat de la booleana
                this.GetComponent<Animator>().SetBool("Attack", false);
                attack = false;
            }

            //Quant esta a amb el latig estirat
            else if (state.IsName("Attack2"))
            {
                //Redimensionem la colisio, perque sigui l'adequada.
                col.offset = new Vector2(1, col.offset.y);
                col.size = new Vector2(4, col.size.y);
            }

            //Si fa el superatack i no hem disparat.
            else if (state.IsName("SuperAttack") && !shoot)
            {
                //Indiquem que hem disparat
                shoot = true;

                //Restem energia
                energy -= 10;

                //Creem una instancia de l'objecte (el hadouken)
                GameObject b = Instantiate<GameObject>(bullet);

                //Si mira cap a la dreta
                if (!render.flipX)
                {
                    //El posem a la dreta del personatge
                    b.transform.position = new Vector3(this.transform.position.x + 1.2f, this.transform.position.y,
                    this.transform.position.z);
                }

                //Si mira cap a l'esquerra
                else
                {
                    //El posem a l'esquerra del personatge
                    b.transform.position = new Vector3(this.transform.position.x - 0.2f, this.transform.position.y,
                    this.transform.position.z);
                }

                //Creem un objecte de la classe bullet.
                Bullet p = new Bullet();

                //Li passem el GameObject
                p.obj = b;

                //Li donem el temps de vida inicial
                p.time = 0;

                //Indiquem en quina direccio anirar
                p.dreta = !render.flipX;

                //Si ha sigut dispart per l'enemic
                p.enemyBullet = false;

                //Li passem el Game Manager que es l'encarregat de gestionar els projectils
                gm.GetComponent<GameManager>().bulletsList.Add(p);

                //Treiem l'animacio de superAtac
                this.GetComponent<Animator>().SetBool("SuperAttack", false);

            }

            //Si esta quiet
            else if (state.IsName("Idle"))
            {
                //Si te energia, pot disparar
                if (energy >= 10) shoot = false;

                //Si hem apretat per atacar, passem a l'estat attack
                if (attack)
                {
                    this.GetComponent<Animator>().SetBool("Attack", true);
                }

                //Si hem apretat per mourens, passem a l'estat de walk
                else if (walk)
                {
                    this.GetComponent<Animator>().SetBool("Walk", true);
                }

                //Sino continuem amb idle
                else
                {
                    this.GetComponent<Animator>().SetBool("Walk", false);
                    this.GetComponent<Animator>().SetBool("Attack", false);
                }
            }

            //Si estem caminant
            else if (state.IsName("Walk"))
            {
                //I deixem de caminar, passem a idle
                if (!walk)
                {
                    this.GetComponent<Animator>().SetBool("Walk", false);
                }
                //Si ataquem, passem a atacar
                else if (attack)
                {
                    this.GetComponent<Animator>().SetBool("Walk", false);
                    this.GetComponent<Animator>().SetBool("Attack", true);
                }
            }
            #endregion

            if(balance)
            {
                Vector3 v = puntsLiana[3].transform.position- this.transform.position;

                if (v.x > 0) v.x = -0.05f;
                else if (v.x < 0) v.x = 0.05f;

                if (v.y > 0) v.y = -0.05f;
                else if (v.y < 0) v.y = 0.05f;

                this.transform.position += v;
                if (!render.flipX && this.transform.position == puntsLiana[1].transform.position)
                {
                    puntsLiana[3].transform.position = puntsLiana[2].transform.position;
                    puntsPassatsLiana++;
                }
                else if (render.flipX && this.transform.position == puntsLiana[1].transform.position)
                {
                    puntsLiana[3].transform.position = puntsLiana[0].transform.position;
                    puntsPassatsLiana++;
                }

                /*balance = !(puntsPassatsLiana >= 2 && (puntsLiana[3].transform.position == puntsLiana[0].transform.position
                    || puntsLiana[3].transform.position == puntsLiana[2].transform.position));*/

                Debug.Log(puntsPassatsLiana);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Si colisiona amb un enemic i esta amb el latig estes
        //Matem l'enemic i ens dona energia
        if (state.IsName("Attack2") && col.gameObject.tag=="Enemy")
        {
            Destroy(col.gameObject);
            energy += 10;
        }

        //Si ens colisiona una bala
        if(col.gameObject.tag=="Bullet")
        {
            //Ens restem vida
            life--;
            //Accedim al GameManager i mirem totes les bales
            //que hi ha a la llista
            GameManager g=gm.GetComponent<GameManager>();
            foreach(Bullet b in g.bulletsList)
            {
                //Si trobem la bala que ens a colisionat i es una disparada de l'enemic
                if(b.enemyBullet && b.obj.transform.position==col.gameObject.transform.position)
                {
                    //Li indiquem que ha arribat el seu temps de vida maxim.
                    b.time = b.maxTime;
                }
            }

            //Si encara li queden vides
            if (life > 0)
            {
                //Si mirem cap a la esquerra, fara un saltet cap a la dreta
                if (render.flipX) rigidBody.AddForce(new Vector2(2.0f, 5.0f), ForceMode2D.Impulse);

                //Si mira cap a la dreta, fara un saltet cap a l'esquerra
                else rigidBody.AddForce(new Vector2(-2.0f, 5.0f), ForceMode2D.Impulse);

                //Indiquem que l'han tocat i per tant no pot saltar
                hit = true;
                firstJump = false;
            }
        }

        //Si toca terra, pot contiunar.
        else if(col.gameObject.tag=="Terrain")
        {
            firstJump = true;
            hit = false;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        //No pot utilitzarla fora de l'area estipulada
        if(col.tag=="Liana")
        {
            liana = false;
            balance = false;
            int p = puntsLiana.Length;
            for(int i=0;i<p;i++)
            {
                Destroy(puntsLiana[i]);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        //Si entre a l'area en concret, mort
        if (col.tag == "Mort")
        {
            Mort();
        }
        //Si esta aprop d'un objecte liana, podra fer el balanceig
        else if(col.tag=="Liana")
        {
            liana = true;
            puntsLiana = new GameObject[4];
            puntsLiana[0] = new GameObject();
            puntsLiana[1] = new GameObject();
            puntsLiana[2] = new GameObject();
            puntsLiana[3] = new GameObject();
            puntsLiana[0].transform.position = col.transform.GetChild(0).position;
            puntsLiana[1].transform.position = col.transform.GetChild(1).position;
            puntsLiana[2].transform.position = col.transform.GetChild(2).position;
        }
    }

    void Mort()
    {
        //Tornem a l'ultim punt de guardat
        //Li restem un intent i li posem totes les vides.
        this.transform.position = savePosition.transform.position;
        tries--;
        life = initalLife;
    }

    void OnGUI()
    {
        //Creem un Gui style per donarli un toc mes personal
        //Canviem el size de la font i el color, que es blanc
        GUIStyle style = new GUIStyle();
        style.fontSize = 25;
        style.normal.textColor = Color.white;

        //Definim els rectangles on estaran el text i les imatges
        int w=Screen.width / 12;
        int h = Screen.height / 12;
        Rect lifeRect = new Rect(0, h, w, h);
        Rect energyRect = new Rect(Screen.width-150, 0, 150, 50);
        Rect heartRect = new Rect(0, 0, w, h);

        //Per cada nombre de intents que em queden, afegim un pj.
        for (int i = 0; i < tries; i++)
        {
            lifeRect.x = w/3 * i;
            GUI.Label(lifeRect, pjTexture);
        }
        //GUI.Label(lifeRect, "Tries:  "+this.tries.ToString(),style);
        GUI.Label(energyRect, "Energy:  " + this.energy.ToString()+"%", style);

        //Per cada nombre de vides que em queden, afegim un cor.
        for (int i =0; i<life; i++)
        {
            heartRect.x = w/3 * i;
            GUI.Label(heartRect, heart);
        }
    }
}
