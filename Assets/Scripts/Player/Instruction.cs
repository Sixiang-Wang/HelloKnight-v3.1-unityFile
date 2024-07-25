using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour
{
    public bool instructionMove;
    public bool instructionJump;
    public bool instructionAttack;
    public bool instructionAim;

    public bool instrAvailable;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("InstructionMove"))
        {
            instructionMove = true;
        }
        if (collision.CompareTag("InstructionJump"))
        {
            instructionJump = true;
        }
        if (collision.CompareTag("InstructionAttack"))
        {
            instructionAttack = true;
        }
        if (collision.CompareTag("InstructionAim"))
        {
            instructionAim = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("InstructionMove"))
        {
            instructionMove = false;
        }
        if (collision.CompareTag("InstructionJump"))
        {
            instructionJump = false;
        }
        if (collision.CompareTag("InstructionAttack"))
        {
            instructionAttack = false;
        }
        if (collision.CompareTag("InstructionAim"))
        {
            instructionAim= false;
        }
    }
}
