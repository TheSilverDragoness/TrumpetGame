using UnityEngine;
using UnityEngine.AI;
using DootEmUp.UI;

namespace DootEmUp.Gameplay.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private string note;

        [SerializeField]
        private int damageAmount;

        [SerializeField]
        private float damageCoolDown;

        private Transform player;

        private Rigidbody[] bones;
             
        private Animator animator;
        private NavMeshAgent agent;
        [HideInInspector]
        public bool dead = false;
        private bool canDamagePlayer = true;
        private float damageDelay;
        private marker marker;
        [SerializeField]
        private UI.Compass compass;
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
        }

        public void SetUpEnemy(UI.Compass _compass)
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
            GameManager.instance.UpdateEnemyCounter();
            compass.RemoveMarker(marker);
            capsuleCollider.enabled = false;
            
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageableObject) && !dead && canDamagePlayer)
            {
                Debug.Log("Collision with player");
                damageableObject.TakeDamage(damageAmount);
                damageDelay = damageCoolDown;
                canDamagePlayer = false;
            }
        }
    }
}