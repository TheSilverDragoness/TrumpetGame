using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private string note;

        [SerializeField]
        private float damageAmount;

        [SerializeField]
        private float damageCoolDown;

        private Transform player;

        private Rigidbody[] bones;
             
        private Animator animator;
        private NavMeshAgent agent;
        [HideInInspector]
        public bool dead = false;
        private GameController controller;
        private bool canDamagePlayer;
        private float damageDelay;

        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            bones = GetComponentsInChildren<Rigidbody>();
            foreach (var bone in bones)
            {
                bone.isKinematic = true;
            }
            canDamagePlayer = true;
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            controller = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        }

        private void Update()
        {
            if (!dead)
            {
                ChasePlayer();
            }
            if (!canDamagePlayer)
            {
                damageDelay -= Time.deltaTime;
                if (damageDelay <= 0)
                {
                    canDamagePlayer = true;
                }
            }
        }

        private void ChasePlayer()
        {
            Vector3 direction = player.position - transform.position;
            Vector3 targetPos = transform.position + direction;
            agent.SetDestination(targetPos);
        }

        public void Die(string noteStr)
        {
            if (!dead)
            {
                if (noteStr == note)
                {
                    dead = true;
                    foreach (var bone in bones)
                    {
                        bone.isKinematic = false;
                    }
                    animator.enabled = false;
                    controller.UpdateEnemyCounter();
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.transform.root.tag == "Player" && !dead && canDamagePlayer)
            {
                Debug.Log("Collision with player");
                other.gameObject.GetComponent<Health>().TakeDamage(damageAmount);
                damageDelay = damageCoolDown;
                canDamagePlayer = false;
            }
        }
    }
}