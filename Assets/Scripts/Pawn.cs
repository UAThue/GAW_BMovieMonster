using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    public Controller controller;
    public abstract void Move(Vector3 direction);
    public abstract void Rotate(float direction);
    public abstract void StartAttack();
    public abstract void EndAttack();
    public abstract void Die();
    public abstract void StartAlternateAttack();
    public abstract void EndAlternateAttack();
}
