using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World_Generator : MonoBehaviour
{
    //player to locate
    public GameObject player_entity;
    //collect the spawn catalogue
    public GameObject street_straight;
    public GameObject street_curve;
    public GameObject street_tSection;
    public GameObject street_4Way;

    public List<MapChunk> map;
    const int _ChunkSize = 16;

    //type, rotation, locationX, locationY

    public class MapChunk : World_Generator
    {
        //map informations
        public StreetTile[,] ChunkMapArray;
        public Vector3 Position;
        //connection point informations
        public List<int> ConnectionPoints_Left;
        public List<int> ConnectionPoints_Right;
        public List<int> ConnectionPoints_Top;
        public List<int> ConnectionPoints_Bottom;

        public MapChunk(int X, int Y, int Z) //initializer for Class - needs to take in already existing map connections
        {
            this.ChunkMapArray = new StreetTile[_ChunkSize, _ChunkSize];
            //location of the chunks bottom left corner
            this.Position = new Vector3(X, Y, Z);
            //initialize connectionpoint lists
            ConnectionPoints_Left = new List<int>();
            ConnectionPoints_Right = new List<int>();
            ConnectionPoints_Top = new List<int>();
            ConnectionPoints_Bottom = new List<int>();
        }

        public void Force_AddTile(GameObject type, int rotation, int posX, int posY) //place a new spawned pre-defined tile into the chunk 
        {
            GameObject NewTile = Instantiate(type);
            Vector3 spawnPoint = new(this.Position.x + 25f * posX, 0, this.Position.y + 25f * posY);
            StreetTile newStreet = new(NewTile, spawnPoint, rotation);
            this.ChunkMapArray[posX, posY] = newStreet;
        }

        public void GenerateChunkMap()
        {
            //todo - fill the chunk with valid map connections
            for (int x = 0; x < _ChunkSize; x++)
            {
                for (int y = 0; y < _ChunkSize; y++)
                {
                    this.Force_AddTile(street_straight, 0, x, y);
                }
            }
        }

    }

    public class StreetTile
    {
        public GameObject GrafikObject;
        public Vector3 Position;
        public int Rotation;

        public bool Connection_Top = false;
        public bool Connection_Bottom = false;
        public bool Connection_Left = false;
        public bool Connection_Right = false;

        public StreetTile(GameObject StreetObject, Vector3 SpawnPoint, int Rotation)
        {
            this.Position = SpawnPoint;
            this.Rotation = Rotation;

            if (StreetObject.CompareTag("streetStreight"))
            {
                if (Rotation == 0 || Rotation == 2)
                {
                    this.Connection_Top = true;
                    this.Connection_Bottom = true;
                    this.GrafikObject = StreetObject;
                    this.GrafikObject.transform.position = this.Position;
                    this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                    //todo rotation and positions
                }
                if (Rotation == 1 || Rotation == 3)
                {
                    this.Connection_Left = true;
                    this.Connection_Right = true;
                    this.GrafikObject = StreetObject;
                    this.GrafikObject.transform.position = this.Position;
                    this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                    //todo rotation and positions
                }
            }
            else if (StreetObject.CompareTag("streetCurve"))
            {
                if (Rotation == 0) //bottom to right
                {
                    this.Connection_Bottom = true;
                    this.Connection_Right = true;
                    this.GrafikObject = StreetObject;
                    this.GrafikObject.transform.position = this.Position;
                    this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                    //todo rotation and position
                }
                if (Rotation == 1) //right to top
                {
                    this.Connection_Right = true;
                    this.Connection_Top = true;
                    this.GrafikObject = StreetObject;
                    this.GrafikObject.transform.position = this.Position;
                    this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                    //todo rotation and position
                }
                if (Rotation == 2) //top to left
                {
                    this.Connection_Top = true;
                    this.Connection_Left = true;
                    this.GrafikObject = StreetObject;
                    this.GrafikObject.transform.position = this.Position;
                    this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                    //todo rotation and position
                }
                if (Rotation == 3) //left to bottom
                {
                    this.Connection_Left = true;
                    this.Connection_Bottom = true;
                    this.GrafikObject = StreetObject;
                    this.GrafikObject.transform.position = this.Position;
                    this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                    //todo rotation and position
                }
            }
            else if (StreetObject.CompareTag("streetTSection"))
            {
                if (Rotation == 0) // bottom to left and right
                {
                    this.Connection_Bottom = true;
                    this.Connection_Right = true;
                    this.Connection_Left = true;
                    this.GrafikObject = StreetObject;
                    this.GrafikObject.transform.position = this.Position;
                    this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                    //todo rotation and position
                }
                if (Rotation == 1) // right to top and bottom
                {
                    this.Connection_Right = true;
                    this.Connection_Top = true;
                    this.Connection_Bottom = true;
                    this.GrafikObject = StreetObject;
                    this.GrafikObject.transform.position = this.Position;
                    this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                    //todo rotation and position
                }
                if (Rotation == 2) // top to right and left
                {
                    this.Connection_Top = true;
                    this.Connection_Left = true;
                    this.Connection_Right = true;
                    this.GrafikObject = StreetObject;
                    this.GrafikObject.transform.position = this.Position;
                    this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                    //todo rotation and position
                }
                if (Rotation == 3) //left to bottom and top
                {
                    this.Connection_Left = true;
                    this.Connection_Bottom = true;
                    this.Connection_Top = true;
                    this.GrafikObject = StreetObject;
                    this.GrafikObject.transform.position = this.Position;
                    this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                    //todo rotation and position
                }
            }
            else if (StreetObject.CompareTag("street4Way"))
            {
                this.Connection_Left = true;
                this.Connection_Right = true;
                this.Connection_Bottom = true;
                this.Connection_Top = true;
                this.GrafikObject = StreetObject;
                this.GrafikObject.transform.position = this.Position;
                this.GrafikObject.transform.rotation = new(0, Rotation * 90, 0, 0);
                //todo rotation and position
            }
            else
            {
                Debug.Log("Init of StreetTile failed!!!");
            }
        }

        public bool DoesTileFit(StreetTile CompareTile)
        {
            Vector3 myTile = this.GrafikObject.transform.position;
            Vector3 myComp = CompareTile.GrafikObject.transform.position;

            if (myTile.x < myComp.x || myTile.z == myComp.z)
            {
                //tile is to the right
                if (CompareTile.Connection_Right == true && this.Connection_Left == true)
                {
                    //tile fits
                    return true;
                }
            }
            if (myTile.x > myComp.x || myTile.z == myComp.z)
            {
                //tile is to the left
                if (CompareTile.Connection_Left == true && this.Connection_Right == true)
                {
                    //tile fits
                    return true;
                }
            }
            if (myTile.z < myComp.z || myTile.x == myComp.x)
            {
                //tile is to the top
                if (CompareTile.Connection_Top == true && this.Connection_Bottom == true)
                {
                    //tile fits
                    return true;
                }
            }
            if (myTile.z > myComp.z || myTile.x == myComp.x)
            {
                //tile is to the bottom
                if (CompareTile.Connection_Bottom == true && this.Connection_Top == true)
                {
                    //tile fits
                    return true;
                }
            }
            return false;
        }

    }



    // Start is called before the first frame update
    void Start()
    {
        map = new List<MapChunk>();
        MapChunk myTest = new MapChunk(0, 0, 0);
        map.Add(myTest);
        map[0].GenerateChunkMap();

    }

    // Update is called once per frame
    void Update()
    {

    }




}