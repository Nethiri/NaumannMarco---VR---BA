/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class World_Generator : MonoBehaviour
{
    //player to locate
    public GameObject player_entity;
    //collect the spawn catalogue
    public GameObject street_streight;
    public GameObject street_curve;
    public GameObject street_tSection;
    public GameObject street_4Way;

    private List<GameObject> map;
    private readonly float Offset_small = 12.5f;
    private readonly float Offset_large = 25f;

    // Start is called before the first frame update
    void Start()
    {
        //initialize map container!
        map = new List<GameObject>();
        //sets the first street (streight) under the player
        GameObject newStreet = Instantiate(street_streight, new Vector3(x:player_entity.transform.position.x, y: 0, z: player_entity.transform.position.z), street_streight.transform.rotation) ;
        map.Add(newStreet);
    }

    // Update is called once per frame
    void Update()
    {
        Map_FindCollision();
    }

    void Map_FindCollision()
    {
        foreach(GameObject street in map)
        {
            foreach(Collider col in street.GetComponentsInChildren<Collider>())
            {
                if(col.bounds.Intersects(player_entity.GetComponentInChildren<Collider>().bounds))
                {
                    //if part of street_streight
                    if(street.CompareTag("streetStreight"))
                    {
                        //todo streight street function
                        if (Handle_streetStreight(street, col)) { return; };
                    }
                    else if(street.CompareTag("streetCurve"))
                    {
                        //todo curve street function
                    }
                    else if (street.CompareTag("streetTSection"))
                    {
                        //todo TSection street function
                    }
                    else if (street.CompareTag("street4Way"))
                    {
                        //todo 4Way street function
                    }
                    else
                    {
                        Debug.Log("Collision was detected in unknown tag!");
                    }
                }
            }
        } 
    }

    void Map_addPartToAnchor(GameObject street_Part, Vector3 anchorOrigin, float partOffset, Quaternion partOrientation) 
    {
        //Function to spawn a new part of the map on a certain anchor

        Vector3 spawnPoint = anchorOrigin + (partOrientation * Vector3.forward * partOffset);
        foreach(GameObject street in map) 
        { 
            if (street.transform.position == spawnPoint) 
            {
                //no new object needs to be spawned
                return;
            } 
        }
        GameObject newStreet = Instantiate(street_Part, spawnPoint, partOrientation);
        map.Add(newStreet);
    }

    bool Handle_streetStreight(GameObject street, Collider col) 
    {
        if(col.name == "Sensor_Z_Pos")
        {
            GameObject anchorObject = street.transform.Find("Anchor_Z_Pos").gameObject;
            Map_addPartToAnchor(street_streight, anchorObject.transform.position, Offset_small, anchorObject.transform.rotation);
            return true;
        }
        else if(col.name == "Sensor_Z_Neg")
        {
            GameObject anchorObject = street.transform.Find("Anchor_Z_Neg").gameObject;
            Map_addPartToAnchor(street_streight, anchorObject.transform.position, -Offset_small, anchorObject.transform.rotation);
            return true;
        }
        else
        {
            Debug.Log("Colum: " + col.name + " sensoren not found");
            return false;
        }
    }

    bool Handle_streetCurve(GameObject street, Collider col)
    {
        if (col.name == "Sensor_Z_Pos")
        {
            GameObject anchorObject = street.transform.Find("Anchor_Z_Pos").gameObject;
            Map_addPartToAnchor(street_streight, anchorObject.transform.position, Offset_small, anchorObject.transform.rotation);
            return true;
        }
        else if (col.name == "Sensor_X_Neg")
        {
            GameObject anchorObject = street.transform.Find("Anchor_X_Neg").gameObject;
            Map_addPartToAnchor(street_streight, anchorObject.transform.position, -Offset_small, anchorObject.transform.rotation);
            return true;
        }
        else
        {
            Debug.Log("Colum: " + col.name + " sensoren not found");
            return false;
        }
    }


}
*/