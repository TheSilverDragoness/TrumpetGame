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
        private bool canDamagePlayer = true;
        private float damageDelay;
        private marker marker;
        [SerializeField]
        private Compass compass;
        private CapsuleCollider capsuleCollider;

        private void Start()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            player = GameObject.FindWithTag("Player").transform;
            bones = GetComponentsInChildren<Rigidbody>();
            foreach (var bone in bones)
            {
                bone.isKinematic = true;
            }
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            controller = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        }

        public void SetUpEnemy(Compass _compass)
        {
            compass = _compass;
            marker = GetComponent<marker>();
            if (compass != null && marker != null) 
            {
                Debug.LogWarning("Marker added");
                compass.AddQuestMarker(marker);
            }
            else
            {
                Debug.LogError("No Marker");
            }
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
            if (dead)
            {
                return;
            }
            if (noteStr != note)
            {
                return;
            }
            dead = true;
            foreach (var bone in bones)
            {
                bone.isKinematic = false;
            }
            animator.enabled = false;
            controller.UpdateEnemyCounter();
            compass.RemoveMarker(marker);
            capsuleCollider.enabled = false;
            
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