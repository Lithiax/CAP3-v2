using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseParallax : MonoBehaviour
{
    Vector2 startPos;
    
    private float length, xPos, yPos, offset;
    private Camera cam;

    public float parallaxEffect;

    //Object nearer from player faster - Object farther from player slower
    //origin + (travel * parallax)

    /* Explanation
     * type name  - description - asset name - sprite layer - parallax effect
     * background - farthest    - highest    - lowest       - highest
     * midground  - farther     - higher     - lower        - higher
     * ground     - player      - normal     - normal       - normal
     * foreground - nearest     - lowest     - highest      - lowest / negative
    */



    /* Example
     * type name  - description - asset name - sprite layer - parallax effect
     * background - farthest    - 5          - 0            - 1
     * midground  - farther     - 4          - 1            - 0.7
     * midground  - farther     - 3          - 2            - 0.5
     * midground  - farther     - 2          - 3            - 0.3
     * ground     - player      - 1          - 4            - 0
     * foreground - nearest     - 0          - 5            - -0.3
    */

    private void Awake()
    {
        if (cam == null)
        {
            if (Camera.main != null)
            {
                cam = Camera.main;
            }
        }
     
       
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        xPos = transform.position.x;
        yPos = transform.position.y;
        //length = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // Update is called once per frame
    void Update()
    {
        //Null check
        if (cam == null) return;
        Vector2 pz = new Vector3(0, 0, 0);

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            pz = cam.ScreenToViewportPoint(Input.mousePosition)*0.1f;
        }
        else
        {
            if (SystemInfo.supportsAccelerometer == true)
            {

                pz = cam.ScreenToViewportPoint(Input.acceleration) * 2000;
            }
        }
        float posX = Mathf.Lerp(transform.position.x, startPos.x + (pz.x * (100*(1 - parallaxEffect))), 2f * Time.deltaTime);
        float posY = Mathf.Lerp(transform.position.y, startPos.y + (pz.y * (100*(1 - parallaxEffect))), 2f * Time.deltaTime);
        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
