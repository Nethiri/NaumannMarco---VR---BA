using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class World_Generator : MonoBehaviour
{
    //player to locate
    public GameObject player_entity;
    //collect the spawn catalogue
    public GameObject street_straight;
    public GameObject street_curve;
    public GameObject street_tSection;
    public GameObject street_4Way;
    public GameObject street_empty;

    public List<MapChunk> map;
    const int _ChunkSize = 3;

    //type, rotation, locationX, locationY

    public class MapChunk
    {
        //map informations
        public StreetTile[,] ChunkMapArray;
        public Vector3 Position;
        //connection point informations
        public List<int> ConnectionPoints_Left;
        bool external_left = false;
        public List<int> ConnectionPoints_Right;
        bool external_right = false;
        public List<int> ConnectionPoints_Top;
        bool external_top = false;
        public List<int> ConnectionPoints_Bottom;
        bool external_bottom = false;

        public MapChunk(int X, int Y, int Z) //initializer for Class - needs to take in already existing map connections
        {
            this.ChunkMapArray = new StreetTile[_ChunkSize, _ChunkSize];
            //location of the chunks bottom left corner
            this.Position = new Vector3(X, Y, Z);            
        }

        public void Force_AddTile(GameObject type, int rotation, int posX, int posY) //place a new spawned pre-defined tile into the chunk 
        {
            GameObject NewTile = Instantiate(type);
            NewTile.name = "Street at " + "X:" + posX + "Y:" + posY;
            Vector3 spawnPoint = new(this.Position.x + 25f * posX, 0, this.Position.y + 25f * posY);
            StreetTile newStreet = new(NewTile, spawnPoint, rotation, posX, posY);
            this.ChunkMapArray[posX, posY] = newStreet;
        }

        

        public void SetConnectionPoints(int side, List<int> ConnectionLocations)
        {
            //side 0 = bottom
            //side 1 = right
            //side 2 = top
            //side 3 = left
            if (side == 0) { this.ConnectionPoints_Bottom = ConnectionLocations; this.external_bottom = true; }
            if (side == 1) { this.ConnectionPoints_Right = ConnectionLocations; this.external_right = true; }
            if (side == 2) { this.ConnectionPoints_Top = ConnectionLocations; this.external_top = true; }
            if (side == 3) { this.ConnectionPoints_Left = ConnectionLocations; this.external_left = true; }
        }

        public List<int> GetConnectionPoints(int side)
        {
            //todo
            return new List<int>();
        }

        public bool RemoveTile(int x, int y)
        {
            if (ChunkMapArray[x, y].DestroyObject() == true)
            {
                Debug.Log("Tile: X-" + x + " Y-" + y + " has been removed!");
                ChunkMapArray[x, y] = null;
                return true;
            }
            return false;
        }

        public void GenerateChunkMap(World_Generator instance, GameObject ChunkType = null)
        {
            Debug.Log(this.ChunkMapArray.GetLength(0));
        }

        private GameObject GetRandomStreetType(World_Generator instance)
        {
            System.Random random = new();
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

        private bool CheckMapFilled()
        {
            bool full = true;
            for (int x = 0; x < _ChunkSize; x++)
            {
                for (int y = 0; y < _ChunkSize; y++)
                {
                    if (this.ChunkMapArray[x, y] != null)
                    {
                        full = false;
                        break;
                    }
                }
            }
            return full;
        }

        private List<int> GetTileConnections(int posX, int posY)
        {
            //0 - bottom || 4 - bottom optional 
            //1 - right  || 5 - right optional
            //2 - top    || 6 - top optional
            //3 - left   || 7 - left optional
            List<int> connectionList = new();

            if(posX + 1 == this.ChunkMapArray.GetLength(0)) { connectionList.Add(5); }
            if(posX - 1 < 0) { connectionList.Add(7); }
            if(posY + 1 == this.ChunkMapArray.GetLength(1)) { connectionList.Add(6); }
            if(posY - 1 < 0) { connectionList.Add(4); }

            if (this.ChunkMapArray[posX, posY + 1].Connection_Bottom) { connectionList.Add(2); }
            if (this.ChunkMapArray[posX, posY - 1].Connection_Top) { connectionList.Add(0); }
            if (this.ChunkMapArray[posX + 1, posY].Connection_Top) { connectionList.Add(3); }
            if (this.ChunkMapArray[posX - 1, posY].Connection_Top) { connectionList.Add(1); }

            return connectionList;
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
        //own location in map array
        public int self_x;
        public int self_y;
        //prevois tile
        public int previous_x;
        public int previous_y;



        public StreetTile(GameObject StreetObject, Vector3 SpawnPoint, int Rotation, int curX, int curY, int prevX = -1, int prevY = -1)
        {
            this.Position = SpawnPoint;
            this.Rotation = Rotation;
            this.self_x = curX;
            this.self_y = curY;

            if(prevX != -1 && prevY != -1)
            {
                this.previous_x = prevX;
                this.previous_y = prevY;
            }

            if (StreetObject.CompareTag("streetStreight"))
            {
                if (Rotation == 0 || Rotation == 2)
                {
                    this.Connection_Top = true;
                    this.Connection_Bottom = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name += " - Streight down<>up";
                    GrafikObject.transform.SetPositionAndRotation(this.Position, Quaternion.Euler(0, Rotation * 90, 0));
                    
                    //todo rotation and positions
                }
                if (Rotation == 1 || Rotation == 3)
                {
                    this.Connection_Left = true;
                    this.Connection_Right = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name += " - Streight left<>right";
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
                    GrafikObject.name += " - Curve bottom->right";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x + 12.5f, this.Position.y, this.Position.z - 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 1) //right to top
                {
                    this.Connection_Left = true;
                    this.Connection_Bottom = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name += " - Curve left->bottom";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z - 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 2) //top to left
                {
                    this.Connection_Top = true;
                    this.Connection_Left = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name += " - Curve top->left";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 3) //left to bottom
                {
                    this.Connection_Right = true;
                    this.Connection_Top = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name += " - Curve right->top";
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
                    GrafikObject.name += " - TSection right->top<>bottom";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x + 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 1) // right to top and bottom
                {
                    this.Connection_Bottom = true;
                    this.Connection_Right = true;
                    this.Connection_Left = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name += " - TSection bottom->left<>right";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x+12.5f, this.Position.y, this.Position.z-12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 2) // top to right and left
                {
                    this.Connection_Left = true;
                    this.Connection_Bottom = true;
                    this.Connection_Top = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name += " - TSection left->bottom<>top";
                    GrafikObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z - 12.5f), Quaternion.Euler(0, Rotation * 90, 0));
                    //todo rotation and position
                }
                if (Rotation == 3) //left to bottom and top
                {
                    this.Connection_Top = true;
                    this.Connection_Left = true;
                    this.Connection_Right = true;
                    this.GrafikObject = StreetObject;
                    GrafikObject.name += " - TSection top->right<>left";
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
                GrafikObject.name += " - 4WayStreet";
                GrafikObject.transform.SetPositionAndRotation(new(this.Position.x+12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, 0, 0));
                //todo rotation and position
            }
            else
            {
                Debug.Log("Init of StreetTile failed!!!");
            }
        }

        public bool DoesTileConnect(StreetTile CompareTile = null, int side = -1) //checks if the two tiles in question connect to one another or if an external connection exists
        {
            if(CompareTile != null)
            {
                if (this.self_x < CompareTile.self_x && this.self_y == CompareTile.self_y) 
                {
                    if (this.Connection_Right == true && CompareTile.Connection_Left == true) { return true; } 
                }
                if (this.self_x > CompareTile.self_x && this.self_y == CompareTile.self_y)
                {
                    if (this.Connection_Left == true && CompareTile.Connection_Right == true) { return true; }
                }
                if (this.self_y < CompareTile.self_y && this.self_x == CompareTile.self_x)
                {
                    if (this.Connection_Top == true && CompareTile.Connection_Bottom == true) { return true; }
                }
                if (this.self_y > CompareTile.self_y && this.self_x == CompareTile.self_x)
                {
                    if (this.Connection_Bottom == true && CompareTile.Connection_Top == true) { return true; }
                }
                return false;
            }
            else if(side != -1)
            {
                //side 0 = bottom
                //side 1 = right
                //side 2 = top
                //side 3 = left
                if (this.Connection_Bottom == true && side == 0) { return true; }
                if (this.Connection_Right == true && side == 1) { return true; }
                if (this.Connection_Top == true && side == 2) { return true; }
                if (this.Connection_Left == true && side == 3) { return true; }
                return false;
            }
            else { return false; }
        }

        public bool DoesTileFit(StreetTile CompareTile) //checks if both tiles are next to one another without having a connection expectation from either
        {
            Vector3 myTile = this.GrafikObject.transform.position;
            Vector3 myComp = CompareTile.GrafikObject.transform.position;

            if (myTile.x < myComp.x || myTile.z == myComp.z)
            {
                //tile is to the right
                if (CompareTile.Connection_Right == false && this.Connection_Left == false)
                {
                    //tile fits
                    return true;
                }
            }
            if (myTile.x > myComp.x || myTile.z == myComp.z)
            {
                //tile is to the left
                if (CompareTile.Connection_Left == false && this.Connection_Right == false)
                {
                    //tile fits
                    return true;
                }
            }
            if (myTile.z < myComp.z || myTile.x == myComp.x)
            {
                //tile is to the top
                if (CompareTile.Connection_Top == false && this.Connection_Bottom == false)
                {
                    //tile fits
                    return true;
                }
            }
            if (myTile.z > myComp.z || myTile.x == myComp.x)
            {
                //tile is to the bottom
                if (CompareTile.Connection_Bottom == false && this.Connection_Top == false)
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
        //MapChunk myTest2 = new(110, 0, 0);
        //MapChunk myTest3 = new(-110, 0, 0);
        //MapChunk myTest4 = new(0, 110, 0);
        //MapChunk myTest5 = new(0, -110, 0);

        map.Add(myTest);
        //map.Add(myTest2);
        //map.Add(myTest3);
        //map.Add(myTest4);
        //map.Add(myTest5);
        //map[0].Force_AddTile(street_straight, 0, 0, 0);
        map[0].GenerateChunkMap(this);

        //map[1].GenerateChunkMap(this);
        //map[2].GenerateChunkMap(this);
        //map[3].GenerateChunkMap(this);
        //map[4].GenerateChunkMap(this);

    }

    // Update is called once per frame
    void Update()
    {
  
    }




}