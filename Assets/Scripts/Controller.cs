using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public Pawn pawn;

    // Start is called before the first frame update
    public abstract void Start();
    public abstract void Update();

    public abstract void MakeDecisions();
}
