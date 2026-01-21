using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{

    [Header("Zombie Animation")]
    public Animator anim; // <--- ADD THIS LINE
    
    [SerializeField] private int HP = 100;
    private Animator animator;
    private NavMeshAgent navAgent;
    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }
    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        if (HP <= 0)
        {
            anim.SetBool("Walking", false);
            anim.SetBool("Running", false);
            anim.SetBool("Attacking", false);
            anim.SetBool("Died", true);
            animator.SetTrigger("DEATH");
            Destroy(gameObject);   
        }
        else
        {
            animator.SetTrigger("DAMAGE");
        }
    }
    private void Update()
    {
        if(navAgent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    }}