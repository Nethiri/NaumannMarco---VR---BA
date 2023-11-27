using System.Collections;
using System.Collections.Generic;
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

    private List<GameObject> map; //9x9 field

    // Start is called before the first frame update
    void Start()
    {
        //sets the first street (streight) under the player
        GameObject newStreet = Instantiate(street_streight, new Vector3(x:player_entity.transform.position.x, y: 0, z: player_entity.transform.position.z), street_streight.transform.rotation) ;
        map.Add(newStreet);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject part in map)
        {
            Debug.Log();
        }   
    }
}
