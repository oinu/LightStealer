using UnityEngine;
using System.Collections;

class Bullet
{
    public GameObject obj;
    public float time;
    public float maxTime = 5.0f;
    public bool dreta;
    public bool enemyBullet;
    public void Time(float i)
    {
        time += i;
    }
    public void Move()
    {
        if (dreta)
        {
            obj.transform.position = new Vector3(obj.transform.position.x + 0.1f, obj.transform.position.y,
               obj.transform.position.z);
            obj.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            obj.transform.position = new Vector3(obj.transform.position.x - 0.1f, obj.transform.position.y,
               obj.transform.position.z);
            obj.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
};

public class GameManager : MonoBehaviour {
    int[] rm;
    int length;
    public ArrayList bulletsList;
    public GameObject pj;
    public Camera c;

    // Use this for initialization
    void Start () {
        bulletsList = new ArrayList();
        
    }
	
	// Update is called once per frame
	void Update () {

        CameraUpdate();
        //Llista per borrar aquelles bales que ja an passat el seu temps de vida.
        rm = new int[bulletsList.Count];
        length = 0;

        //Per cada bala de la llista.
        foreach (Bullet b in bulletsList)
        {
            //La movem a l'espai
            b.Move();

            //L'incrementem el temps
            b.Time(Time.deltaTime);

            //Si ha expirat el seu temps de vida
            if (b.time >= b.maxTime)
            {
                //L'afegim a la llista de borrat.
                rm[length] = bulletsList.IndexOf(b);
                length++;
            }
        }

        //Borrem els elements que ja han passat el seu temps de vida
        for (int i = 0; i < length; i++)
        {
            //Obtenim l'index del les bales, guardats a l'array.
            int index = rm[i];

            //Fem el cast per poder eliminar l'objecte de l'escena.
            Bullet b = (Bullet)bulletsList[index];

            //Destruim l'objecte.
            Destroy(b.obj);

            //L'eliminem de la llista de bales.
            bulletsList.RemoveAt(index);
        }

        // "Borrem" la llista de borrats.
        rm = null;
    }

    void CameraUpdate()
    {
        if (pj.transform.position.y < 0.0f)
        {
            c.transform.position = new Vector3(pj.transform.position.x + 3.0f, 0.0f, c.transform.position.z);
        }
        else
        {
            c.transform.position = new Vector3(pj.transform.position.x + 3.0f, pj.transform.position.y, c.transform.position.z);
        }
    }
}
