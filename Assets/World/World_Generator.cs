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
        Debug.Log(map.Count);
        foreach (GameObject streetPiece in map)
        {
            ////Debug.Log("Found a streetPiece in map!");
            //Transform streetTransform = streetPiece.transform;
            //Debug.Log("How many children has my object: " + streetTransform.childCount);

            //for(int i = 0; i<streetTransform.childCount; i++)
            //{
            //    Collider subObject = streetTransform.GetChild(i);
            //    if (subObject != null)
            //    {
            //        Debug.Log("Found a SubObject! Type: " + streetTransform.GetType().Name);
            //        //subObject needs to print debug if collided with

            //    }
            //}

            List<Collider> collection = new List<Collider>( streetPiece.GetComponentsInChildren<Collider>() );
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
                            Debug.Log(anchorObject.transform.position);
                        }
                    }
                }
            }

        }
    }
}
