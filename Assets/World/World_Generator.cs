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
    public int size;

    private List<GameObject> map;
    const int _ChunkSize = 16;

    //type, rotation, locationX, locationY

    private class MapChunk
    {
        public GameObject[,] ChunkMapArray;
        public Vector3 Position;
        
        public MapChunk(int X, int Y, int Z) //initializer for Class - needs to take in already existing map connections
        {
            //todo - place "pre existing" street tiles into the map
            this.ChunkMapArray = new GameObject[_ChunkSize, _ChunkSize];
            //location of the chunks bottom left corner
            

        }

        public void addTile(GameObject type, int rotation, int posX, int posY)
        {
            Vector3 spawnPoint = new Vector3(0, 0, 0);                  //todo
            Quaternion spawnOrientation = new Quaternion(0, 0, 0, 0);   //todo
            GameObject newStreet = Instantiate(type, spawnPoint, spawnOrientation);
            this.ChunkMapArray[posX, posY] = type;
        }

        private void GenerateMap() { }

    }











    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    

}

