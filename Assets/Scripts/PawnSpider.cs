using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSpider : Pawn
{
    [HideInInspector]
    public Rigidbody rb;
    private Animation anim;
    [SerializeField]
    private WebTarget _currentTarget;
    private WebTarget currentTarget {
        get {
            return _currentTarget;
        }
        set {
            // If the current target is already set, untarget it  (visuals)
            if (_currentTarget != null) {
                _currentTarget.SetUntargeted();
            }

            // Save the new target (as long as we aren't setting this to null
            _currentTarget = value;

            // If not null, Set it as targeted (visuals)
            if (value != null) {
                _currentTarget.SetTargeted();
            }
        }
    }
    [SerializeField]
    private List<WebTarget> targetsInRange;
    [SerializeField]
    private Transform centerOfButtTransform;
    public float webRadius;
    public float moveSpeed;
    public float turnSpeed;
    public float webbingPerSecond = 100;
    public Transform webStartingPoint;

    // Coroutines
    private IEnumerator webShootingCoroutine;

    public void Start()
    {
        // Get the rigidbody component, if it has one, if not, add one
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Get the animator component
        anim = GetComponentInChildren<Animation>();

        // Get all the targets in range of the web attack
        GetAllTargetsInRange();

        // Save our coroutines for web shooting
        webShootingCoroutine = SpinWebUpdate();

}

public void Update()
    {
        // TODO: This is heavy, when we optimize, only target humans when cycling targets or target is null and we get a new human
        TargetFirstHuman();

    }

    public bool IsAttacking()
    {
        // Check if we are attacking
        if (anim.IsPlaying("attack1") || anim.IsPlaying("attack2start") || anim.IsPlaying("attack2end") || anim.IsPlaying ("webLoop")) {
            return true;
        }

        // Otherwise
        return false;

    }

    public override void Move(Vector3 direction)
    {
        if (!IsAttacking()) {
            Vector3 moveVector = direction;
            moveVector.Normalize();
            moveVector *= moveSpeed;

            // Play an animation by setting the animation directly
            if (moveVector.magnitude < 0.1f) {
                anim.Play("idle");
            }
            else if (moveVector.magnitude < 3.0f) {
                anim.Play("walk");
            }
            else {
                anim.Play("run");
            }

            // Simplest movement -- just go there
            // Later we might want to limit this to forward/back, no strafe, no flying
            moveVector *= Time.deltaTime;
            rb.MovePosition(rb.position + moveVector);
        }
    }

    public override void Rotate(float directionWithMagnitude)
    {
       transform.Rotate(0, directionWithMagnitude * turnSpeed * Time.deltaTime, 0);
    }
    
    public void TargetFirstHuman ()
    {
        // Target our first human (if they exist)
        if (targetsInRange.Count > 0) {
            currentTarget = targetsInRange[0];
        } else {
            currentTarget = null;
        }
    }

    public void GetAllTargetsInRange()
    {
        // This is slow....  we can optimize by only doing this at start, and then 
        //       keep up to date via onTriggerEnter and OnTriggerExit
        Collider[] allObjectsInSphere = Physics.OverlapSphere(centerOfButtTransform.position, webRadius);
        foreach (Collider collider in allObjectsInSphere) {
            WebTarget webTarget = collider.GetComponent<WebTarget>();
            if (webTarget != null) {
                if (!targetsInRange.Contains(webTarget)) {
                    targetsInRange.Add(webTarget);
                }
            }
        }
    }

    // Use Triggers to keep list of targets up to date
    public void OnTriggerEnter(Collider other)
    {
        WebTarget webTarget = other.GetComponent<WebTarget>();
        if (webTarget != null) {
            if (!targetsInRange.Contains(webTarget)) {
                targetsInRange.Add(webTarget);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        WebTarget webTarget = other.GetComponent<WebTarget>();
        if (webTarget != null) {
            targetsInRange.Remove(webTarget);
        }
    }


    // Attacking
    public override void StartAttack()
    {
        anim.Play("attack1");
    }
    public override void EndAttack()
    {
        // DO NOTHING TO END MAIN ATTACK
    }

    // Death
    public override void Die()
    {
        anim.Blend("death2");
    }

    // Webbing
    public override void StartAlternateAttack()
    {
        anim.Play("attack2start");
        if (currentTarget != null) {
            StartSpinWeb();
        } else {
            EndAlternateAttack();
        }
    }

    public void StartSpinWeb ()
    {
        // Play loop
        anim.CrossFadeQueued("webLoop", 0.2f);

        // Attach a line
        _currentTarget.AttachWebLine();

        // Start the Coroutine
        StartCoroutine(webShootingCoroutine);

    }
       
    public IEnumerator SpinWebUpdate()
    {
        // This runs when we are spinning a web on an enemy - we break out, or stop the coroutine when it is time to stop
        while (true) {

            print("Spinning web on:" + _currentTarget.gameObject.name);

            // If webloop is not playing
            if (!anim.IsPlaying("webLoop")) {
                // Play web loop
                anim.CrossFade("webLoop", 0.2f);
            }

            // Increase web amount by a given amount
            _currentTarget.amountWebbed += webbingPerSecond * Time.deltaTime;
          
            // If at total, Stop animation and end attack
            if (_currentTarget.amountWebbed >= _currentTarget.maxAmountWebbed) {
                anim.Stop();
                StartDragTarget();
                EndAlternateAttack();
                // Stop our coroutine!
                yield break;
            }

            // Otherwise, come back and do this again next frame draw!
            yield return null;
        }
    }

    public void StartDragTarget()
    {
        // Turn on the web line
        _currentTarget.AttachWebLine();
    }


    public void StopDragTarget()
    {
        // Turn off the web line
        _currentTarget.DetachWebLine();
    }



    public override void EndAlternateAttack()
    {
        // Stop our coroutine of web shooting
        if (webShootingCoroutine != null) {
            StopCoroutine(webShootingCoroutine);
        }

        // If we are still playing the Spin animation, we stopped early, so release the human
        if (anim.IsPlaying("webLoop")) {
            // Release the Human!
            _currentTarget.amountWebbed = 0;

            // Stop Dragging
            StopDragTarget();
        }

        // End the attack
        anim.CrossFade("idle",0.2f);
    }

}
