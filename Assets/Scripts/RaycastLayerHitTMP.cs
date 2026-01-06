using UnityEngine;
using TMPro;

public class RaycastLayerHitTMP : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 10f;
    public LayerMask layerMask; // Assign layers in inspector

    [Header("UI")]
    public TMP_Text hitText; // Drag your TextMeshProUGUI here

    public Transform head;

    private string lastHitObjectName = "None";

    void Update()
    {
        Ray ray = new Ray(head.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, layerMask))
        {
            lastHitObjectName = hit.collider.gameObject.name;
        }

        // Update TMP text
        if (hitText != null)
        {
            hitText.text = "Last Hit: " + lastHitObjectName;
        }

        // Optional: Draw the ray in Scene view
        Debug.DrawRay(head.position, transform.forward * rayDistance, Color.red);
    }
}
