using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebTarget : MonoBehaviour
{

    [Header("Webbing Settings")]
    [SerializeField]
    private float _amountWebbed;
    public float amountWebbed {
        get { return _amountWebbed; }
        set {
            _amountWebbed = value;
            UpdateVisualsForWebbing();
        }
    }
    public float maxAmountWebbed = 100;

    [Header("Webbing Visuals")]
    public Renderer webbingVisuals;
    public LineRenderer webLine;
    public Vector3 webPointOffset;
    public GameObject targetVisuals;
    public SpringJoint webLineSpringJoint;

    public void Start()
    {
        // Start untargeted
        SetUntargeted();
        // Set our webbing amount based on our amount webbed at start
        UpdateVisualsForWebbing();

        // Set starting data for webline
        webLine.startWidth = 0.35f;
        webLine.endWidth = 0.25f;
        webLine.startColor = Color.grey;
        webLine.endColor = Color.grey;

        // Start with webline off
        webLine.enabled = false;
    }

    public void Update()
    {
        // If the web is on
        if (webLine.enabled) {
            // Move the endpoints of the webline
            UpdateWebline();
        }
    }

    public void AttachWebLine()
    {
        // Turn webline on
        webLine.enabled = true;

        // Destroy any old springjoint
        if (webLineSpringJoint != null) {
            Destroy(webLineSpringJoint);
        }

        // Create a springjoint
        webLineSpringJoint = gameObject.AddComponent<SpringJoint>();
        webLineSpringJoint.connectedBody = GameManager.instance.playerPawn.rb;
        webLineSpringJoint.enableCollision = true;
        webLineSpringJoint.spring = 10;
        webLineSpringJoint.anchor = webPointOffset;
        webLineSpringJoint.connectedAnchor = new Vector3(0, 0, -1);
    }

    public void DetachWebLine()
    {
        // Turn off visuals
        webLine.enabled = false;

        // Destroy the joint
        Destroy(webLineSpringJoint);
    }

    public void UpdateWebline()
    {
        // Connect the end points of the webline to match
        var points = new Vector3[2];
        points[0] = GameManager.instance.playerPawn.webStartingPoint.position;
        points[1] = transform.position + webPointOffset;
        webLine.SetPositions(points);
    }

    public void UpdateVisualsForWebbing()
    {
        //Change apperance of web-sphere based on webbing amount.
        Material webbingMaterial = webbingVisuals.material;
        webbingMaterial.SetFloat("_Cutoff", 1-(amountWebbed/maxAmountWebbed));
    }


    public void SetTargeted()
    {
        targetVisuals.SetActive(true);
    }

    public void SetUntargeted()
    {
        targetVisuals.SetActive(false);
    }


}
