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
        //streight piece part programm!!!! - move / todo
        Debug.Log(map.Count);
        foreach (GameObject streetPiece in map)
        {
            List<Collider> collection = new ( streetPiece.GetComponentsInChildren<Collider>() );
            Debug.Log("Number of colliders:" + collection.Count);
            foreach(Collider col in collection)
            {
                foreach (Collider playerCollider in player_entity.GetComponentsInChildren<Collider>())
                {
                    if (col.bounds.Intersects(playerCollider.bounds))
                    {
                        Debug.Log("Collision detected in: " + col.name);
                        // Do something when a collision is detected
                        if(col.name == "Sensor_Z_Pos")
                        {
                            Debug.Log("Do something cool!");
                            GameObject anchorObject = streetPiece.transform.Find("Anchor_Z_Pos").gameObject;
                            //Add new object to anchor
                            Debug.Log(anchorObject.transform.position);
                            Map_addPartToAnchor(street_streight, anchorObject.transform.position, new Vector3(x:0,y:anchorObject.transform.rotation.y,z:0), 0);
                        }
                    }
                }
            }

        }
    }

    void Map_addPartToAnchor(GameObject street_Part, Vector3 anchorOrigin, Vector3 partDirection, int partOrientation) 
    {
        //Function to spawn a new part of the map on a certain anchor
        Vector3 spawnPoint = anchorOrigin + Vector3.Cross(partDirection, new Vector3(Offset_small, Offset_small, Offset_small)); 
        Quaternion spawnOrientation = new (x: 0, y: partOrientation, z: 0, w: 0);
        GameObject newStreet = Instantiate(street_Part, spawnPoint, spawnOrientation);
        map.Add(newStreet);
    }
}
