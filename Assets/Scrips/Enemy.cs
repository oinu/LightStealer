using UnityEngine;
using System.Collections;


public class Enemy : MonoBehaviour {
    public GameObject start;
    public GameObject end;
    public bool right=true;
    public GameObject bullet;
    public GameObject gm;
    bool walk,shoot;
    BoxCollider2D campVisio;
    AnimatorStateInfo state;

    // Use this for initialization
    void Start () {

        walk = true;
        shoot = false;
        campVisio = this.transform.GetChild(0).GetComponent<BoxCollider2D>();
       
	
	}
	
	// Update is called once per frame
	void Update () {
        // Si camina
        if(walk && !state.IsName("EnemyCharge") && !state.IsName("EnemyAttack"))
        {
            //Activem animacio caminar
            this.GetComponent<Animator>().SetBool("Walk", true);

            //Si va cap a la dreta
            if (right)
            {
                //Girem la textura
                this.GetComponent<SpriteRenderer>().flipX = false;

                //Canviem el trigger de visio
                campVisio.offset = new Vector2(1.2f, 0.0f);

                //Movem l'enemic.
                this.transform.position = new Vector3(this.transform.position.x + 0.1f,
                    this.transform.position.y, this.transform.position.z);

                // Calcula la distancia que hi ha entre ell i el punt final.
                float d = end.transform.position.x - this.transform.position.x;

                //Si aquesta distancia es mes gran que 0.5 vol dir que encara no ha arribat.
                right = d > 0.5;
            }

            // Si va cap a l'esquerra
            else
            {
                //Girem la textura
                this.GetComponent<SpriteRenderer>().flipX = true;

                //Canviem el trigger de visio
                campVisio.offset = new Vector2(-1.0f, 0.0f);

                //Movem l'enemic.
                this.transform.position = new Vector3(this.transform.position.x - 0.1f,
                    this.transform.position.y, this.transform.position.z);

                // Calcula la distancia que hi ha entre ell i el punt inicial.
                float d = this.transform.position.x - start.transform.position.x;

                //Si aquesta distancia es mes gran que 0.5 vol dir que encara no ha arribat.
                right = d < 0.5;
            }
        }

        //Si no caminem
        else
        {
            //Desactivem l'animacio de caminar.
            this.GetComponent<Animator>().SetBool("Walk", false);
        }

        //Comprovem en quina animacio esta
        state = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

        //Si esta atacan i no ha disparat
        if (state.IsName("EnemyAttack") && !shoot)
        {
            //Disparta
            shoot = true;

            //Crea una instancia del Prefab bullet.
            GameObject b = Instantiate<GameObject>(bullet);

            //El Col·loquem on toca
            if(right)
            {
                b.transform.position = new Vector3(this.transform.GetChild(0).position.x + 1.2f, this.transform.GetChild(0).position.y,
                this.transform.GetChild(0).position.z);
            }

            else
            {
                b.transform.position = new Vector3(this.transform.GetChild(0).position.x - 0.2f, this.transform.GetChild(0).position.y,
                this.transform.GetChild(0).position.z);
            }
            

            //Creem un objecte de tipus Bullet
            Bullet p = new Bullet();

            //Li passem l'objecte
            p.obj = b;

            //Li donem un valor al temps.
            p.time = 0;

            p.dreta = right;

            //L'afegim a la llista de municio.
            gm.GetComponent<GameManager>().bulletsList.Add(p);


            
        }

        //Si esta carregant
        else if(state.IsName("EnemyCharge"))
        {
            //Pot disparar
            shoot = false;
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        //Si el jugador entre dintre el camp de visio
        if(col.name=="PJ")
        {
            //Canviem l'estat a atacar.
            this.GetComponent<Animator>().SetBool("Attack", true);
            walk = false;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        //Si el jugador surt del camp de visio
        if (col.name == "PJ")
        {
            //Deixa d'atacar.
            this.GetComponent<Animator>().SetBool("Attack", false);
            walk = true;
        }
    }
}
