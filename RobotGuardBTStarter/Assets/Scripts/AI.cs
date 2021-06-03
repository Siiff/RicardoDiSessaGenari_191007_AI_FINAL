using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda; //plugin panda//

//Não consegui comentar no script do Wander.BT//

public class AI : MonoBehaviour
{
	//Referencias//
	public Transform player;
	public Transform bulletSpawn;
	public Slider healthBar;
	public GameObject bulletPrefab;
	NavMeshAgent agent;
	//Variaveis//
	public Vector3 destination; // The movement destination.
	public Vector3 target;      // The position to aim to.
	float health = 100.0f;
	float rotSpeed = 5.0f;
	float visibleRange = 80.0f;
	float shotRange = 40.0f;
    [Header("Droids")]
    public GameObject[] Droids;

    void Start()
	{
		//Atribuindo componentes e stopdistance//
		agent = this.GetComponent<NavMeshAgent>();
		agent.stoppingDistance = shotRange - 5; //for a little buffer
												//Regeneração de vida//
		//InvokeRepeating("UpdateHealth", 5, 1.5f);
	}

	void Update()
	{
		EncontrarDroids();
		//Barra de vida//
		Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
		healthBar.value = (int)health;
		healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);
        if (Input.GetKeyDown(KeyCode.F))
        {
			health -= 40;
        }
	}
/*	//Sistema de vida + regeneração//
	void UpdateHealth()
	{
		if (health < 100)
			health++;
	}*/
	//Sistema de dano//
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "bullet")
		{
			health -= 10;
		}
	}

	//Metodos do Wander.BT, Patrol.BT e Attack.BT//
	//Pegando o destino//
	//Esse é o pickdestinantion do patrol, o outro é aleatório//
	[Task]
	public void PickDestination(int x, int z)
	{
		Vector3 dest = new Vector3(x, 0, z);
		agent.SetDestination(dest);
		Task.current.Succeed();
	}
	//Metodo de Andar Aleatoriamente//
	[Task]
	public void PickRandomDestination()
	{
		Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
		agent.SetDestination(dest);
		Task.current.Succeed();
	}
	//Movendo pra posição//
	[Task]
	public void MoveToDestination()
	{
		if (Task.isInspected)
		{
			Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
		}
		if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
		{
			Task.current.Succeed();
		}
	}

	[Task]
	public void FollowPlayer()
    {
		Vector3 dest = new Vector3(player.transform.position.x,player.transform.position.y,player.transform.position.z);
		agent.SetDestination(dest);
		Task.current.Succeed();
	}
	//Metodo de target no player//
	[Task]
	public void TargetPlayer()
	{
		//setando o player cm alvo//
		target = player.transform.position;
		Task.current.Succeed();
	}
	//Metodo de atirar//   
	[Task]
	public bool Fire()
	{
		//disparando as balas quando o player for visto//
		GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
		bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
		return true;
	}
	//esse Metodo faz o droid olhar ao redor aleatóriamente//
	[Task]
	public void LookAtTarget()
	{
		Vector3 direction = target - this.transform.position;
		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
		if (Task.isInspected)
		{
			Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));
		}
		if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
		{
			Task.current.Succeed();
		}
	}

	//Metodo de Vendo o player//
	[Task]
	bool SeePlayer()
	{
		Vector3 distance = player.transform.position - this.transform.position;
		RaycastHit hit;
		//Se tiver uma parede no caminho, bloqueia o resto do script//
		bool seeWall = false; Debug.DrawRay(this.transform.position, distance, Color.red);
		if (Physics.Raycast(this.transform.position, distance, out hit))
		{
			if (hit.collider.gameObject.tag == "wall")
			{
				seeWall = true;
			}
		}
		if (Task.isInspected)
		{
			Task.current.debugInfo = string.Format("wall={0}", seeWall);
		}
		if (distance.magnitude < visibleRange && !seeWall)
		{
			return true;
		}
		else
			return false;
	}
	//Metodo de virar//
	[Task]

	bool Turn(float angle)
	{
		var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
		target = p;
		return true;
	}

	[Task] //Se a vida for menor que X, fazer Y//
	public bool IsHealthLessThan(float health) 
	{ 
		return this.health < health; 
	}
	[Task] //Morrer//
	public bool Explode() 
	{ 
		Destroy(healthBar.gameObject); 
		Destroy(this.gameObject); 
		return true;
	}
	
	[Task] //MorrerDoPreto//
	public bool ExplodeBlack()
	{//verificando se ele é o ultimo, se for, morre, se não, regenera vida
		if(Droids.Length == 1)
        {
			Debug.LogError("SOMENTE O PRETO TA VIVO");
			bool pretoUnico = true;
            if (pretoUnico)
            {
				Explode();
            }
		}
        else
        {
			health = 70;
        }
		return true;
	}
	//Pegando os droids na cena
	public void EncontrarDroids()
    {
		Droids = GameObject.FindGameObjectsWithTag("droid");
		Debug.Log("" + Droids);
    }

}



