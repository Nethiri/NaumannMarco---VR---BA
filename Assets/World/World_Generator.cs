using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public class MapChunk
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
            NewTile.name = "Street at " + "X:" + posX + "Y:" + posY;
            Vector3 spawnPoint = new(this.Position.x + 25f * posX, 0, this.Position.y + 25f * posY);
            StreetTile newStreet = new(NewTile, spawnPoint, rotation);
            this.ChunkMapArray[posX, posY] = newStreet;
        }

        public bool RemoveTile(int x, int y)
        {
            if (ChunkMapArray[x, y].DestroyObject() == true)
            {
                ChunkMapArray[x, y] = null;
                return true;
            }
            return false;
        }

        public void GenerateChunkMap(World_Generator instance)
        {
            //todo - fill the chunk with valid map connections
            //for (int x = 0; x < _ChunkSize; x++)
            //{
            //    for (int y = 0; y < _ChunkSize; y++)
            //    {
            //        this.Force_AddTile(instance.street_straight, 1, x, y);
            //        if (x != 0 && y != 0)
            //        {
            //            Debug.Log("Tile: x:" + x + " y:" + y + "and Tile: x:" + x + " y:" + (y-1));
            //            if (ChunkMapArray[x, y].DoesTileFit(ChunkMapArray[x, y - 1]))
            //            {
            //                Debug.Log("Tile fits!");
            //            }
            //            else
            //            {
            //                Debug.Log("Tile doesnt fit!");
            //            }
            //        }
            //    }
            //}
            System.Random random = new System.Random(); // Random number generator

            for (int x = 0; x < _ChunkSize; x++)
            {
                for (int y = 0; y < _ChunkSize; y++)
                {
                    // Check if there is already a street tile in this position
                    if (ChunkMapArray[x, y] == null)
                    {
                        // Generate a random street type
                        GameObject streetType = GetRandomStreetType(instance);

                        // Generate a random rotation (0 to 3)
                        int rotation = random.Next(4);

                        // Add the street tile to the map
                        Force_AddTile(streetType, rotation, x, y);
                    }
                }
            }

        }

        private GameObject GetRandomStreetType(World_Generator instance)
        {
            System.Random random = new System.Random();
            int randomIndex = random.Next(4); // 0 to 3

            switch (randomIndex)
            {
                case 0:
                    return instance.street_straight;
                case 1:
                    return instance.street_curve;
                case 2:
                    return instance.street_tSection;
                case 3:
                    return instance.street_4Way;
                default:
                    return instance.street_4Way; // Default to straight street if something goes wrong
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
                    GrafikObject.name = GrafikObject.name + " - Streight down<>up";
                    GrafikObject.transform.SetPositionAndRotation(this.Position, Quaternion.Euler(0, Rotation * 90, 0));
                    
                    //todo rotation and positions
                }
                if (Rotation == 1 || Rotation == 3)
                {
                    this.Connection_Left = true;
                    this.Connection_Right = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name = GrafikObject.name + " - Streight left<>right";
                    GrafikObject.transform.SetPositionAndRotation(this.Position, Quaternion.Euler(0, Rotation * 90, 0));
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
                    GrafikObject.name = GrafikObject.name + " - Curve bottom->right";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x + 12.5f, this.Position.y, this.Position.z - 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 1) //right to top
                {
                    this.Connection_Left = true;
                    this.Connection_Bottom = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name = GrafikObject.name + " - Curve left->bottom";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z - 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 2) //top to left
                {
                    this.Connection_Top = true;
                    this.Connection_Left = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name = GrafikObject.name + " - Curve top->left";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 3) //left to bottom
                {
                    this.Connection_Right = true;
                    this.Connection_Top = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name = GrafikObject.name + " - Curve right->top";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x + 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
            }
            else if (StreetObject.CompareTag("streetTSection"))
            {
                if (Rotation == 0) // bottom to left and right
                {
                    this.Connection_Right = true;
                    this.Connection_Top = true;
                    this.Connection_Bottom = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name = GrafikObject.name + " - TSection right->top<>bottom";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x + 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 1) // right to top and bottom
                {
                    this.Connection_Bottom = true;
                    this.Connection_Right = true;
                    this.Connection_Left = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name = GrafikObject.name + " - TSection bottom->left<>right";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x+12.5f, this.Position.y, this.Position.z-12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 2) // top to right and left
                {
                    this.Connection_Left = true;
                    this.Connection_Bottom = true;
                    this.Connection_Top = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name = GrafikObject.name + " - TSection left->bottom<>top";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z - 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 3) //left to bottom and top
                {
                    this.Connection_Top = true;
                    this.Connection_Left = true;
                    this.Connection_Right = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name = GrafikObject.name + " - TSection top->right<>left";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
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
                GrafikObject.transform.SetPositionAndRotation(new(this.Position.x+12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, 0, 0));
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

        public bool DestroyObject()
        {
            GrafikObject.SetActive(false);
            Destroy(GrafikObject);
            if(GrafikObject.IsDestroyed())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        map = new List<MapChunk>();
        MapChunk myTest = new(0, 0, 0);
        MapChunk myTest2 = new(410, 0, 0);
        MapChunk myTest3 = new(-410, 0, 0);
        MapChunk myTest4 = new(0, 410, 0);
        MapChunk myTest5 = new(0, -410, 0);

        map.Add(myTest);
        map.Add(myTest2);
        map.Add(myTest3);
        map.Add(myTest4);
        map.Add(myTest5);
        map[0].Force_AddTile(street_straight, 0, 0, 0);
        map[0].GenerateChunkMap(this);
        map[1].GenerateChunkMap(this);
        map[2].GenerateChunkMap(this);
        map[3].GenerateChunkMap(this);
        map[4].GenerateChunkMap(this);

    }

    // Update is called once per frame
    void Update()
    {
  
    }




}