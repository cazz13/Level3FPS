using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.ProBuilder.MeshOperations;

public class EnemyController : MonoBehaviour
{

    [Header("Enemy Data")]
    [SerializeField] private int currentLife;
    [SerializeField] private int maxLife;
    [SerializeField] private int enemyScorePoint;

    [Header ("patrol")]

    [SerializeField] private GameObject patrolPointsContainer;
    private List<Transform> patrolPoints = new List<Transform>();
    private int destinationPoint = 0;
    private bool isChasing;



    private NavMeshAgent agent;
    //Player
    private Transform playerTransform;

    private WeaponController controller;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        controller = GetComponent<WeaponController>();

        //Take all the children of patrolPointContainer and add them in the patrolPoints array
        //patrolPoints = new Transform[patrolPointsContainer.transform.childCount];
        foreach (Transform child in patrolPointsContainer.transform)    
            patrolPoints.Add(child);

        //for (int i = 0;i< patrolPointsContainer.transform.childCount; i++)
        //{

        //    patrolPoints[i] = patrolPointsContainer.transform.GetChild(i);

        //} 
        GotoNextPatrolPoint();
    }

    private void Update()
    {
        //search player with Ray Cast
        SearchPlayer();


        //TODO Choose destination point
        if (!isChasing && !agent.pathPending && agent.remainingDistance < 3f)
        {
            GotoNextPatrolPoint();
        }
    }


    private void GotoNextPatrolPoint()
    {
        //restart the stopping distance to 0 to posibility the Patrol
        agent.stoppingDistance = 0f;


        //set the agent to the currently destination Point
        agent.SetDestination(patrolPoints[destinationPoint].position);


        if (agent.remainingDistance <= 0.5)
        {
            //choose nest destinationPoint in the List
            //cycling to the start if necessary
            destinationPoint = (destinationPoint + 1) % patrolPoints.Count;
        }
        

    }

    /// <summary>
    /// Enemy search and go towards player
    /// </summary>
    private void SearchPlayer()
    {
        NavMeshHit hit;

        //If no obstacle between enemy ond player
        if (!agent.Raycast(playerTransform.position, out hit))
        {
            if (hit.distance <= 10f)
            {
                    agent.SetDestination(playerTransform.position);
                    agent.stoppingDistance = 3f;
                    isChasing = true; //Chase player
         
                
                transform.LookAt(playerTransform.position);

                if (hit.distance < 5)
                {
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                }

                //TODO
                if(hit.distance <= 7f)
                {
                    if (controller.CanShoot())
                        controller.Shoot();
                }
            }
            else
            {
                isChasing = false;                
            }
        }
        else
        {
            agent.isStopped = false;
            isChasing = false;         

        }
    }
    

    /// <summary>
    /// Handle when the enemy receive a bullet
    /// </summary>
    /// <param name="quantity"></param>
    public void DamageEnemy(int quantity)
    {
        currentLife -= quantity;
        if (currentLife < 0)
        {

            Destroy(gameObject);
        }
        
        
    }


}
