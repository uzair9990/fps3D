using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
  PATROL,
  CHASE,
  ATTACK
}

public class EnemyController : MonoBehaviour
{
  private EnemyAnimator enemy_Anim;
  private NavMeshAgent navAgent;
  private EnemyState enemy_State;
  public float walk_Speed = 0.5f;
  public float run_Speed = 4f;
  public float chaseDistance = 7f;
  private float current_chaseDistance;
  public float attack_Distance = 1.8f;
  public float chase_After_Attack_Distance = 2f;
  public float patrol_Radius_Min = 20f, patrol_Radius_Max = 60f;
  public float patrol_For_This_Time = 15f;
  private float patrol_Timer;
  public float wait_Before_Attack = 2f;
  private float attack_Timer;
  private Transform target;
  public GameObject attack_Point;

  void Awake()
  {
    enemy_Anim = GetComponent<EnemyAnimator>();
    navAgent = GetComponent<NavMeshAgent>();
    target = GameObject.FindWithTag("Character").transform;
  }

  void Start()
  {
    enemy_State = EnemyState.PATROL;
    patrol_Timer = patrol_For_This_Time;
    attack_Timer = wait_Before_Attack;
    current_chaseDistance = chaseDistance;
  }

  void Update()
  {
    if (enemy_State == EnemyState.PATROL)
      Patrol();
    if (enemy_State == EnemyState.CHASE)
      Chase();
    if (enemy_State == EnemyState.ATTACK)
      Attack();
  }

  void Patrol()
  {
    navAgent.isStopped = false;
    navAgent.speed = walk_Speed;
    patrol_Timer += Time.deltaTime;

    if (patrol_Timer > patrol_For_This_Time)
    {
      SetNewRandomDestination();
      patrol_Timer = 0f;
    }

    if (navAgent.velocity.sqrMagnitude > 0)
    {
      enemy_Anim.walk(true);
    }
    else
    {
      enemy_Anim.walk(false);
    }

    if (Vector3.Distance(transform.position, target.position) <= chaseDistance)
    {
      enemy_Anim.walk(false);
      enemy_State = EnemyState.CHASE;
    }
  }

  void Chase()
  {
    navAgent.isStopped = false;
    navAgent.speed = run_Speed;
    navAgent.SetDestination(target.position);

    if (navAgent.velocity.sqrMagnitude > 0)
    {
      enemy_Anim.run(true);
    }
    else
    {
      enemy_Anim.run(false);
    }

    if (Vector3.Distance(transform.position, target.position) <= attack_Distance)
    {
      enemy_Anim.run(false);
      enemy_Anim.walk(false);
      enemy_State = EnemyState.ATTACK;

      if (chaseDistance != current_chaseDistance)
      {
        chaseDistance = current_chaseDistance;
      }
    }
    else if (Vector3.Distance(transform.position, target.position) > chaseDistance)
    {
      enemy_Anim.run(false);
      enemy_State = EnemyState.PATROL;
      patrol_Timer = patrol_For_This_Time;

      if (chaseDistance != current_chaseDistance)
      {
        chaseDistance = current_chaseDistance;
      }
    }
  }

  void Attack()
  {
    navAgent.velocity = Vector3.zero;
    navAgent.isStopped = true;
    attack_Timer += Time.deltaTime;

    if (attack_Timer > wait_Before_Attack)
    {
      enemy_Anim.attack();
      attack_Timer = 0f;
    }

    if (Vector3.Distance(transform.position, target.position) > attack_Distance + chase_After_Attack_Distance)
    {
      enemy_State = EnemyState.CHASE;
    }
  }

  void SetNewRandomDestination()
  {
    float rand_Radius = Random.Range(patrol_Radius_Min, patrol_Radius_Max);
    Vector3 randDir = Random.insideUnitSphere * rand_Radius;
    randDir += transform.position;

    NavMeshHit navHit;
    NavMesh.SamplePosition(randDir, out navHit, rand_Radius, -1);
    navAgent.SetDestination(navHit.position);
  }

  public void showAttackPoint()
  {
    attack_Point.SetActive(true);
  }

  public void hideAttackPoint()
  {
    if (attack_Point.activeInHierarchy)
      attack_Point.SetActive(false);
  }

  public EnemyState getEnemyState
  {
    get; set;
  }
}
