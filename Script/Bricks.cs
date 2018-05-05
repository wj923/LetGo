using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Bricks : MonoBehaviour {
    public Vector3 location;
    public Quaternion rotation;
    public string color; 
    public int type;

    public bool movement = false;
    private bool checkCollision = false;
    private bool moveUp = false;
    private BoxCollider sideCol = null;
    private List<BoxCollider> bottomCol = new List<BoxCollider>();
    private Vector3 lastMousePos;
    private Vector3 lastPos;

    private List<Bricks> missionCol = new List<Bricks>();
    private bool match = false;

    Camera mainCam;
    public Camera subCam;

    void Start()
    {
        mainCam = Camera.main;

        location = this.transform.position;
        rotation = this.transform.rotation;

        color = this.transform.GetComponent<Renderer>().material.name;
        string[] tmp = color.Split(' '); // ex) Material Format : Red (Instance) ...
        color = tmp[0];

        tmp = name.Split('('); // ex) Name Format : 2x2(Clone) or 2x2 ...
        string typeName = tmp[0];
        type = getBrickType(typeName);
        
        BrickManager.brickList.Add(this);
    }

    private int getBrickType(string name)
    {
        switch (name)
        {
            case "1x1": return 0;
            case "2x1": return 1;
            case "2x2": return 2;
            case "2x2_3": return 3;
            case "4x1": return 4;
            case "4x1_2": return 5;
            case "4x2": return 6;
            case "4x2_2": return 7;
            case "4x4_2": return 8;
            default: return -1; // Matching Fail
        }
    }

    private void FixedUpdate()  // Handling About Moving Bricks
    {
        if(missionCol.Count != 0 && !match)
        {
            foreach(Bricks goal in missionCol)
            {
                if(goal.location == this.transform.position)
                {
                    if(goal.type == this.type)
                    {
                        if (goal.color.Equals(this.color))
                        {
                            match = true;
                            MissionManager.matchCount++;
                            // To-do : Rotation...
                        }
                    }
                }
            }
        }        

        if (movement && !match)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                float posX, posY, posZ;
                posX = (7.9f) * Mathf.Round(hit.point.x / 7.9f);
                posZ = (7.9f) * Mathf.Round(hit.point.z / 7.9f);

                if (sideCol == null && hit.transform.tag.Equals("Field"))
                {
                    posY = 0;
                }
                else if(sideCol == null)
                {
                    posY = hit.point.y + (this.GetComponent<BoxCollider>().size.y / 2) + 1;
                }
                else
                {
                    posY = sideCol.transform.position.y + sideCol.size.y;
                }

                if (this.transform.position != new Vector3(posX, posY, posZ))
                {
                    subCam.transform.position -= this.transform.position;
                    this.transform.position = new Vector3(posX, posY, posZ);
                    subCam.transform.position += this.transform.position;
                    subCam.transform.LookAt(this.transform);
                }
            }
        }
    }

    public void saveChange()
    {
        location = this.transform.position;
        rotation = this.transform.rotation;
        string[] tmp = this.transform.GetComponent<Renderer>().material.name.Split(' ');
        color = tmp[0];
    }

    void OnDestroy()
    {
        BrickManager.brickList.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        BoxCollider otherCol = other.GetComponent<BoxCollider>();
        if(other.tag == "Brick")
        {
            if(this.transform.position.y <= other.transform.position.y) // Collision on side (Bricks move up)
            {
                if(sideCol == null || sideCol.transform.position.y < other.transform.position.y)
                    sideCol = otherCol;
            }
            else if(this.transform.position.y > other.transform.position.y)
            {
                if(this.GetComponent<BoxCollider>().size.y < 9) // Half Bricks
                {
                    if (sideCol == null || sideCol.transform.position.y < other.transform.position.y)
                        sideCol = otherCol;
                }
                else // Another Bricks, Collision on bottom
                {
                    bottomCol.Add(otherCol);
                }
            }
        }
        else if(other.tag == "Mission")
        {
            missionCol.Add(other.GetComponent<Bricks>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        BoxCollider otherCol = other.GetComponent<BoxCollider>();
        if(other.tag == "Brick")
        {
            if(sideCol == otherCol)
            {
                sideCol = null;
            }
            else if(bottomCol.Contains(otherCol))
            {
                bottomCol.Remove(otherCol);
            }
        }
        else if(other.tag == "Mission")
        {
            missionCol.Remove(other.GetComponent<Bricks>());
        }
    }
}
