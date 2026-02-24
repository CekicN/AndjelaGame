using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; 
    private Animator animator; 
    private CharacterController controller; 
    void Start() 
    { 
        animator = GetComponent<Animator>(); 
        controller = GetComponent<CharacterController>(); 
    } 
    
    void Update() { 
        float h = Input.GetAxis("Horizontal"); 
        float v = Input.GetAxis("Vertical"); 
        Vector3 move = new Vector3(h, 0, v); 
        controller.Move(move * moveSpeed * Time.deltaTime); 

        animator.SetFloat("Speed", move.magnitude); 
        
        if (Input.GetButtonDown("Jump"))
            animator.SetTrigger("Jump");
    }
}
