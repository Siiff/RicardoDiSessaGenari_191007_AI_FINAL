using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drive : MonoBehaviour {
    //Script de movimentação de disparos do Personagem//


    //Variaveis//
	public float speed = 20.0F;
    public float rotationSpeed = 120.0F;
    //Referencias//
    [Header("Balas")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    [Header("Spawn")]
    public GameObject Spawn;
    //Vida//
    [Header("Health")]
    public Slider healthBar;
    public float health = 100.0f;

    private Rigidbody rb;
    Vector3 x, z;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        InvokeRepeating("UpdateHealth", 5, 0.5f);
    }

    private void FixedUpdate()
    { //Mov pelo rigidbody//
        x = transform.right * Input.GetAxisRaw("Horizontal");
        z = transform.forward * Input.GetAxisRaw("Vertical");

        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal"), 0) * Time.deltaTime * rotationSpeed);
        Vector3 movement = (x + z).normalized * speed;
        rb.AddForce(movement);

        //Correr//
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 50;
        }
        else
        {
            speed = 20;
        }
    }
    void Update() {
        //Respawn do player//
        Respawn();

        //Vida//
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);

        //Movimentação pelo translate//
        /*//Pegando os inputs e multiplicando por deltatime//
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);*/

        //Disparo//
        if(Input.GetKeyDown("space"))
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            health -= 10;
        }
    }
    //Regen de vida//
    void UpdateHealth()
    {
        if (health < 100)
            health++;
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "bullet")
        {
            Debug.Log("Tomei tiro");
            health -= 10;
        }
    }

    //Metodo de respawn, vida abaixo de 0, reseta posição e vida//
    void Respawn()
    {
        if(health <= 0)
        {
            Debug.Log("MORRI");
            transform.position = new Vector3 (Spawn.transform.position.x, Spawn.transform.position.y, Spawn.transform.position.z);
            Debug.Log("RESPAWN");
            health = 100f;
        }
    }
}
