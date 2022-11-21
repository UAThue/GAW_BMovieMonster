using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlayer : Controller
{
    public override void Start()
    {
        // Add myself to the list of players
        if (GameManager.instance != null) {          
            GameManager.instance.players.Add(this);
        }
    }

    public override void Update()
    {
        MakeDecisions();
    }

    public override void MakeDecisions()
    {
        // Get the direction to move from the input devices
        Vector3 moveVector = new Vector3(0, 0, Input.GetAxis("Vertical"));
        // Change move vector so it is LOCAL (Forward/Backward, not North/South)
        moveVector = pawn.transform.TransformDirection(moveVector);
        // Move that direction
        pawn.Move(moveVector);
        pawn.Rotate(Input.GetAxis("Horizontal"));

        // If Fire button is pressed
        if (Input.GetButtonDown("Fire1")) {
            pawn.StartAttack();
        }
        if (Input.GetButtonUp("Fire1")) {
            pawn.EndAttack();
        }


        if (Input.GetButtonDown("Fire2")) {
            pawn.StartAlternateAttack();
        }
        if (Input.GetButtonUp("Fire2")) {
            pawn.EndAlternateAttack();
        }

    }

    public void OnDestroy()
    {
        // Remove myself from the list of players
        if (GameManager.instance != null) {
            GameManager.instance.players.Remove(this);
        }

    }

}
