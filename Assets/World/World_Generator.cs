using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class World_Generator : MonoBehaviour
{
    //player to locate
    public GameObject PlayerEntity;
    //collect the spawn catalogue
    public GameObject streetStraight;
    public GameObject streetCurve;
    public GameObject streetTSection;
    public GameObject street4Way;
    public GameObject streetEmpty;

    public int NumberOfTileTypes = 5; //this needs to be adjusted when more tiles are added! both here!!! and in the getRandomTile function!!!


    public enum Connection_Type { closed, open }
    public enum Direction { Bottom, Left, Top, Right}
    public static Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Bottom:
                return Direction.Top;
            case Direction.Left:
                return Direction.Right;
            case Direction.Top:
                return Direction.Bottom;
            case Direction.Right:
                return Direction.Left;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    const int _ChunkSize = 4;

    public class Connection
    {
        private bool _fixed;
        public bool Fixed
        {
            get { return _fixed; }
            set
            {
                if (_fixed) { throw new Exception("Can't edit a fixed Connection!"); }
                _fixed = value;
            }
        }

        private Connection_Type _type;
        public Connection_Type Type
        {
            get { return _type; }
            set
            {
                if (_fixed) { throw new Exception("Can't edit a fixed Connection!"); }
                _type = value;
            }
        }

        public Connection(Connection_Type conType = Connection_Type.closed, bool isFixed = false)
        {
            _type = conType;
            _fixed = isFixed;
        }
    }

    public class Tile
    {
        public GameObject GameObject;
        public Vector3 Position;
        public int Rotation;
        public Tile PreviousTile;

        private Connection[] connections;

        public Connection Connection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Bottom:
                    return connections[0];
                case Direction.Right:
                    return connections[1];
                case Direction.Top:
                    return connections[2];
                case Direction.Left:
                    return connections[3];
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public void ReplaceConnection(Direction direction, Connection connection)
        {
            switch (direction)
            {
                case Direction.Bottom:
                    connections[0] = connection;
                    break;
                case Direction.Right:
                    connections[1] = connection;
                    break;
                case Direction.Top:
                    connections[2] = connection;
                    break;
                case Direction.Left:
                    connections[3] = connection;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public Tile(GameObject StreetObject, Vector3 SpawnPoint, int Rotation, Tile PreviousTile = null) 
        {
            this.Position = SpawnPoint;
            if(Rotation > 3 || Rotation < 0) { throw new ArgumentOutOfRangeException("Rotation has to be 0-3"); }
            this.Rotation = Rotation;
            this.PreviousTile = PreviousTile;
            //generate 4 connections for each tile, have them standart be closed, for now
            this.connections = new Connection[4];
            for (int i = 0; i < 4; i++)
            {
                this.connections[i] = new Connection(Connection_Type.closed);
            }

            //place the StreetObject propperly, depending on the rotation and set open connections
            this.GameObject = StreetObject;
            if (StreetObject.CompareTag("streetStreight"))
            {
                if (this.Rotation == 0 || this.Rotation == 2)
                {
                    this.Connection(Direction.Top).Type = Connection_Type.open;
                    this.Connection(Direction.Bottom).Type = Connection_Type.open;
                    this.GameObject.name += " - Streight down<>up";
                    this.GameObject.transform.SetPositionAndRotation(this.Position, Quaternion.Euler(0, this.Rotation * 90, 0));
                }
                if (this.Rotation == 1 || this.Rotation == 3)
                {
                    this.Connection(Direction.Left).Type = Connection_Type.open;
                    this.Connection(Direction.Right).Type = Connection_Type.open;
                    this.GameObject.name += " - Streight left<>right";
                    this.GameObject.transform.SetPositionAndRotation(this.Position, Quaternion.Euler(0, this.Rotation * 90, 0));
                }
            }
            else if (StreetObject.CompareTag("streetCurve"))
            {
                if (this.Rotation == 0) //bottom to right 
                {
                    this.Connection(Direction.Bottom).Type = Connection_Type.open;
                    this.Connection(Direction.Right).Type = Connection_Type.open;
                    this.GameObject.name += " - Curve bottom->right";
                    this.GameObject.transform.SetPositionAndRotation(new(this.Position.x + 12.5f, this.Position.y, this.Position.z - 12.5f), Quaternion.Euler(0, this.Rotation * 90, 0));
                }
                if (this.Rotation == 1) //bottom to left 
                {
                    this.Connection(Direction.Bottom).Type = Connection_Type.open;
                    this.Connection(Direction.Left).Type = Connection_Type.open;
                    this.GameObject.name += " - Curve left->bottom";
                    this.GameObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z - 12.5f), Quaternion.Euler(0, this.Rotation * 90, 0));
                }
                if (this.Rotation == 2) //top to left 
                {
                    this.Connection(Direction.Top).Type = Connection_Type.open;
                    this.Connection(Direction.Left).Type = Connection_Type.open;
                    this.GameObject.name += " - Curve top->left";
                    this.GameObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, this.Rotation * 90, 0));
                }
                if (this.Rotation == 3) //top to right 
                {
                    this.Connection(Direction.Top).Type = Connection_Type.open;
                    this.Connection(Direction.Right).Type = Connection_Type.open;
                    this.GameObject.name += " - Curve Top->Right";
                    this.GameObject.transform.SetPositionAndRotation(new(this.Position.x + 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, this.Rotation * 90, 0));
                }
            }
            else if (StreetObject.CompareTag("streetTSection"))
            {
                if(this.Rotation == 0) // right -> Top and Bottom
                {
                    this.Connection(Direction.Right).Type = Connection_Type.open;
                    this.Connection(Direction.Top).Type = Connection_Type.open;
                    this.Connection(Direction.Bottom).Type = Connection_Type.open;
                    this.GameObject.name += " - TSection bottom->top<>right";
                    this.GameObject.transform.SetPositionAndRotation(new(this.Position.x + 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, this.Rotation * 90, 0));
                }
                if (this.Rotation == 1) // Bottom -> Left and Right
                {
                    this.Connection(Direction.Bottom).Type = Connection_Type.open;
                    this.Connection(Direction.Right).Type = Connection_Type.open;
                    this.Connection(Direction.Left).Type = Connection_Type.open;
                    this.GameObject.name += "- TSection bottom->left <> right";
                    this.GameObject.transform.SetPositionAndRotation(new(this.Position.x + 12.5f, this.Position.y, this.Position.z - 12.5f), Quaternion.Euler(0, this.Rotation * 90, 0));
                }
                if (this.Rotation == 2) //left -> bottom and Top
                {
                    this.Connection(Direction.Left).Type = Connection_Type.open;
                    this.Connection(Direction.Bottom).Type = Connection_Type.open;
                    this.Connection(Direction.Top).Type = Connection_Type.open;
                    this.GameObject.name += " - TSection left->bottom<>top";
                    this.GameObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z - 12.5f), Quaternion.Euler(0, this.Rotation * 90, 0));
                }
                if (this.Rotation == 3) //top -> right and left
                {
                    this.Connection(Direction.Top).Type = Connection_Type.open;
                    this.Connection(Direction.Left).Type = Connection_Type.open;
                    this.Connection(Direction.Right).Type = Connection_Type.open;
                    this.GameObject.name += " - TSection top->right<>left";
                    this.GameObject.transform.SetPositionAndRotation(new(this.Position.x - 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, this.Rotation * 90, 0));
                }
            } 
            else if (StreetObject.CompareTag("street4Way")) 
            {
                this.Connection(Direction.Top).Type = Connection_Type.open;
                this.Connection(Direction.Left).Type = Connection_Type.open;
                this.Connection(Direction.Right).Type = Connection_Type.open;
                this.Connection(Direction.Bottom).Type = Connection_Type.open;
                this.GameObject.name += " - 4WayStreet";
                this.GameObject.transform.SetPositionAndRotation(new(this.Position.x + 12.5f, this.Position.y, this.Position.z + 12.5f), Quaternion.Euler(0, 0, 0));
            }
            else if (StreetObject.CompareTag("streetEmpty"))
            {
                this.Connection(Direction.Top).Type = Connection_Type.closed;
                this.Connection(Direction.Left).Type = Connection_Type.closed;
                this.Connection(Direction.Right).Type = Connection_Type.closed;
                this.Connection(Direction.Bottom).Type = Connection_Type.closed;
                this.GameObject.name += " - empty Tile";
                this.GameObject.transform.SetPositionAndRotation(new(this.Position.x, this.Position.y, this.Position.z), Quaternion.Euler(0, 0, 0));
            }
            else
            {
                Debug.Log("Init of StreetTile failed!!!");
            }
        }
    }

    public class Chunk 
    {
        private int ChunkID = 0;
        private static int ChunkCounter = 0;
        private Connection[][] borderConnections;
        public Tile[,] ChunkMap;
        public Vector3 Position;
        public GameObject ChunkType = null;

        private int TileCounter = 0;
        private int SoftLockEscape = 10000; //10.000 standart?

        //Basic Constructor with only spawnpoint as consideration/value to pass through
        public Chunk(Vector3 SpawnPoint)
        {
            this.Position = SpawnPoint;
            this.ChunkMap = new Tile[_ChunkSize, _ChunkSize];
            this.borderConnections = new Connection[4][];
            foreach(Direction direction in Enum.GetValues(typeof(Direction)))
            {
                int directionIndex = (int)direction;
                this.borderConnections[directionIndex] = new Connection[_ChunkSize];
                for (int con = 0; con < _ChunkSize; con++)
                {
                    this.borderConnections[directionIndex][con] = new Connection(Connection_Type.closed); //add border connections standart closed
                }
            }
            ChunkID = ChunkCounter;
            ChunkCounter++;
        }

        public Chunk(Vector3 SpawnPoint, GameObject StreetType)
        {
            this.Position = SpawnPoint;
            this.ChunkMap = new Tile[_ChunkSize, _ChunkSize];
            this.borderConnections = new Connection[4][];
            this.ChunkType = StreetType;
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                int directionIndex = (int)direction;
                this.borderConnections[directionIndex] = new Connection[_ChunkSize];
                for (int con = 0; con < _ChunkSize; con++)
                {
                    this.borderConnections[directionIndex][con] = new Connection(Connection_Type.closed); //add border connections standart closed
                }
            }
            ChunkID = ChunkCounter;
            ChunkCounter++;
        }

        public Connection[] Connections(Direction direction)
        {
            Connection[] returnValues = new Connection[_ChunkSize];

            switch (direction)
            {
                case Direction.Left:
                    for (int i = 0; i < _ChunkSize; i++)
                    {
                        returnValues[i] = borderConnections[3][i];
                    }
                    break;
                case Direction.Right:
                    for (int i = 0; i < _ChunkSize; i++)
                    {
                        returnValues[i] = borderConnections[1][i];
                    }
                    break;
                case Direction.Top:
                    for (int i = 0; i < _ChunkSize; i++)
                    {
                        returnValues[i] = borderConnections[2][i];
                    }
                    break;
                case Direction.Bottom:
                    for (int i = 0; i < _ChunkSize; i++)
                    {
                        returnValues[i] = borderConnections[0][i];
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return returnValues;
        }

        public static GameObject GetRandomStreetType(World_Generator instance)
        {
            //System.Random random = new();
            //int randomIndex = random.Next(5); // 0 to 4

            int randomIndex = UnityEngine.Random.Range(0, 5);

            switch (randomIndex)
            {
                case 0:
                    return instance.streetStraight;
                case 1:
                    return instance.streetCurve;
                case 2:
                    return instance.streetTSection;
                case 3:
                    return instance.street4Way;
                case 4:
                    return instance.streetEmpty;
                default:
                    return instance.street4Way; // Default to straight street if something goes wrong
            }
        }

        public bool AddTile(GameObject type, int Rotation, int X, int Y, float Z = 0, bool ForceAddTile = false, Tile PreviousTile = null)
        {
            return AddTile_Updated(type, Rotation, X, Y, Z, ForceAddTile, PreviousTile);
        }

        private bool AddTile_Old(GameObject type, int Rotation, int X, int Y, float Z = 0, bool ForceAddTile = false, Tile PreviousTile = null)
        {
            //check if tile connections are superimposed! 
            //superimposed are either by already existing or by border connections
            //if... then see if the tile is going to fit
            //if not... abbort...
            //if 

            //check if there is already a tile in the location we are trying to spawn a new one into.
            if (this.ChunkMap[X, Y] != null & ForceAddTile == false)
            {
                Debug.Log($"Failed to create a tile at X: {X}, Y: {Y} in Chunk: {ChunkID}! There is already a tile!");
                return false;
            }

            //Spawns the new tile (for now)
            //create a new instance of the street object the tile is based on
            GameObject NewTileObject = Instantiate(type);
            //name the tile
            NewTileObject.name = $"Chunk: {ChunkID} - Street at (X:{X}, Y:{Y}) - Nr.: {this.TileCounter}";
            this.TileCounter++;
            //calculate the location the tile is going to be set to (global location)
            Vector3 SpawnPoint = new(this.Position.x + 25f * X, 0, this.Position.z + 25f * Y);
            //generate tile object
            Tile newTile = new(NewTileObject, SpawnPoint, Rotation, PreviousTile);
            
            //add tile to map
            this.ChunkMap[X, Y] = newTile;

            

            //tile is forced to be generated, skip the fitting checks
            if(ForceAddTile == true) { return true; }
            //now that this tile exists and has properties, try and find out if the tile fits into the location (due to connections)
            if(X == 0) 
            {
                
                //meaning this connection is superimposing the tile, if the tiles connection do not meet the needed connections, the tile it going to get thrown
                if (this.Connections(Direction.Left)[Y].Fixed == true)
                {
                    //Debug.Log($"Fixed at X: 0 Y:{Y}");
                    //check if the tile and the connections required are the same or not
                    if(this.Connections(Direction.Left)[Y].Type != this.ChunkMap[X, Y].Connection(Direction.Left).Type)
                    {
                        this.RemoveTile(X, Y);
                        //Debug.Log($"Failed to add a tile at X: {X}, Y: {Y} in Chunk: {ChunkID}! Tile in Direction {Direction.Left} is fixed!");
                        return false;
                    }
                }
                //border chunk connections get replaced with the directions that are already there
                this.ChunkMap[X, Y].ReplaceConnection(Direction.Left, this.Connections(Direction.Left)[Y]);
            }
            if(X == _ChunkSize-1)
            {
                //meaning this connection is superimposing the tile, if the tiles connection do not meet the needed connections, the tile it going to get thrown
                if (this.Connections(Direction.Right)[Y].Fixed == true)
                {
                    //check if the tile and the connections required are the same or not
                    if (this.Connections(Direction.Right)[Y].Type != this.ChunkMap[X, Y].Connection(Direction.Right).Type)
                    {
                        this.RemoveTile(X, Y);
                        Debug.Log($"Failed to add a tile at X: {X}, Y: {Y} in Chunk: {ChunkID}! Tile in Direction {Direction.Right} is fixed!");
                        return false;
                    }
                }
                this.ChunkMap[X, Y].ReplaceConnection(Direction.Right, this.Connections(Direction.Right)[Y]);
            }
            if(Y == 0)
            {
                //meaning this connection is superimposing the tile, if the tiles connection do not meet the needed connections, the tile it going to get thrown
                if (this.Connections(Direction.Bottom)[X].Fixed == true)
                {
                    //check if the tile and the connections required are the same or not
                    if (this.Connections(Direction.Bottom)[X].Type != this.ChunkMap[X, Y].Connection(Direction.Bottom).Type)
                    {
                        this.RemoveTile(X, Y);
                        Debug.Log($"Failed to add a tile at X: {X}, Y: {Y} in Chunk: {ChunkID}! Tile in Direction {Direction.Bottom} is fixed!");
                        return false;
                    }
                }
                this.ChunkMap[X, Y].ReplaceConnection(Direction.Bottom, this.Connections(Direction.Bottom)[X]);
            }
            if (Y == _ChunkSize - 1)
            {
                //meaning this connection is superimposing the tile, if the tiles connection do not meet the needed connections, the tile it going to get thrown
                if (this.Connections(Direction.Top)[X].Fixed == true)
                {
                    //check if the tile and the connections required are the same or not
                    if (this.Connections(Direction.Top)[X].Type != this.ChunkMap[X, Y].Connection(Direction.Top).Type)
                    {
                        this.RemoveTile(X, Y);
                        Debug.Log($"Failed to add a tile at X: {X}, Y: {Y} in Chunk: {ChunkID}! Tile in Direction {Direction.Top} is fixed!");
                        return false;
                    }
                }
                this.ChunkMap[X, Y].ReplaceConnection(Direction.Top, this.Connections(Direction.Top)[X]);
            }
            return true;
        }

        private bool AddTile_Updated(GameObject type, int Rotation, int X, int Y, float Z = 0, bool ForceAddTile = false, Tile prevTile = null) 
        {
            if (this.ChunkMap[X,Y] != null) { return false; }

            //Spawns the new tile (for now)
            //create a new instance of the street object the tile is based on
            GameObject NewTileObject = Instantiate(type);
            //name the tile
            NewTileObject.name = $"Chunk: {ChunkID} - Street at (X:{X}, Y:{Y}) - Nr.: {this.TileCounter}";
            this.TileCounter++;
            //calculate the location the tile is going to be set to (global location)
            Vector3 SpawnPoint = new(this.Position.x + 25f * X, 0, this.Position.z + 25f * Y);
            //generate tile object
            Tile newTile = new(NewTileObject, SpawnPoint, Rotation, prevTile);
            //add tile to map
            this.ChunkMap[X, Y] = newTile;
            if (ForceAddTile == true) { return true; }
            //now that this tile exists and has properties, try and find out if the tile fits into the location (due to connections)
            //starting with border connections
            if (X == 0)
            {

                //meaning this connection is superimposing the tile, if the tiles connection do not meet the needed connections, the tile it going to get thrown
                if (this.Connections(Direction.Left)[Y].Fixed == true)
                {
                    //Debug.Log($"Fixed at X: 0 Y:{Y}");
                    //check if the tile and the connections required are the same or not
                    if (this.Connections(Direction.Left)[Y].Type != this.ChunkMap[X, Y].Connection(Direction.Left).Type)
                    {
                        this.RemoveTile(X, Y);
                        //Debug.Log($"Failed to add a tile at X: {X}, Y: {Y} in Chunk: {ChunkID}! Tile in Direction {Direction.Left} is fixed!");
                        return false;
                    }
                }
                //border chunk connections get replaced with the directions that are already there
                this.ChunkMap[X, Y].ReplaceConnection(Direction.Left, this.Connections(Direction.Left)[Y]);
            }
            if (X == _ChunkSize - 1)
            {
                //meaning this connection is superimposing the tile, if the tiles connection do not meet the needed connections, the tile it going to get thrown
                if (this.Connections(Direction.Right)[Y].Fixed == true)
                {
                    //check if the tile and the connections required are the same or not
                    if (this.Connections(Direction.Right)[Y].Type != this.ChunkMap[X, Y].Connection(Direction.Right).Type)
                    {
                        this.RemoveTile(X, Y);
                        Debug.Log($"Failed to add a tile at X: {X}, Y: {Y} in Chunk: {ChunkID}! Tile in Direction {Direction.Right} is fixed!");
                        return false;
                    }
                }
                this.ChunkMap[X, Y].ReplaceConnection(Direction.Right, this.Connections(Direction.Right)[Y]);
            }
            if (Y == 0)
            {
                //meaning this connection is superimposing the tile, if the tiles connection do not meet the needed connections, the tile it going to get thrown
                if (this.Connections(Direction.Bottom)[X].Fixed == true)
                {
                    //check if the tile and the connections required are the same or not
                    if (this.Connections(Direction.Bottom)[X].Type != this.ChunkMap[X, Y].Connection(Direction.Bottom).Type)
                    {
                        this.RemoveTile(X, Y);
                        Debug.Log($"Failed to add a tile at X: {X}, Y: {Y} in Chunk: {ChunkID}! Tile in Direction {Direction.Bottom} is fixed!");
                        return false;
                    }
                }
                this.ChunkMap[X, Y].ReplaceConnection(Direction.Bottom, this.Connections(Direction.Bottom)[X]);
            }
            if (Y == _ChunkSize - 1)
            {
                //meaning this connection is superimposing the tile, if the tiles connection do not meet the needed connections, the tile it going to get thrown
                if (this.Connections(Direction.Top)[X].Fixed == true)
                {
                    //check if the tile and the connections required are the same or not
                    if (this.Connections(Direction.Top)[X].Type != this.ChunkMap[X, Y].Connection(Direction.Top).Type)
                    {
                        this.RemoveTile(X, Y);
                        Debug.Log($"Failed to add a tile at X: {X}, Y: {Y} in Chunk: {ChunkID}! Tile in Direction {Direction.Top} is fixed!");
                        return false;
                    }
                }
                this.ChunkMap[X, Y].ReplaceConnection(Direction.Top, this.Connections(Direction.Top)[X]);
            }
            //check if this tile fits internally to another - tile -> tile
                        
            Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
            foreach (Direction Loopdirection in directions)
            {
                int NeighborX, NeighborY;
                (NeighborX, NeighborY) = this.GetNeighborCords(X, Y, Loopdirection);
                if((NeighborX != -1) && (NeighborY != -1))
                {
                    if(this.ChunkMap[NeighborX, NeighborY] == null) { continue; }
                    if (this.ChunkMap[X, Y].Connection(Loopdirection).Type != this.ChunkMap[NeighborX, NeighborY].Connection(GetOppositeDirection(Loopdirection)).Type)
                    {
                        this.RemoveTile(X, Y);
                        return false;
                    }
                }
            }

         

            return true;


            return false;
        }

        public void RemoveTile(int X, int Y)
        {
            if (this.ChunkMap[X, Y] != null)
            {
                Destroy(this.ChunkMap[X, Y].GameObject);
                this.ChunkMap[X, Y] = null;
            }
        }

        public void RotateTile(int X, int Y, int newRotation)
        {
            GameObject Type = this.ChunkMap[X, Y].GameObject;
            float Z = this.ChunkMap[X, Y].GameObject.transform.position.y;

            this.RemoveTile(X, Y);
            this.AddTile(Type, newRotation, X, Y, Z);
        }

        public void DEBUG_FillMap(World_Generator instance, int Rotation = -1, GameObject StreetType = null)
            //debug function to fill map with random or specific Tiles
        {
            
            for (int X = 0; X < _ChunkSize; X++)
            {
                for(int Y = 0; Y < _ChunkSize; Y++)
                {
                    int RandomRotation;
                    GameObject RandomStreetType;
                    if (Rotation == -1) { RandomRotation = UnityEngine.Random.Range(0, 4); }
                    else { RandomRotation = Rotation; }

                    if (StreetType == null) { RandomStreetType = GetRandomStreetType(instance); }
                    else { RandomStreetType = StreetType; }


                    
                    if (this.ChunkMap[X,Y] == null) 
                    { 
                        bool WasAdded = this.AddTile(RandomStreetType, RandomRotation, X, Y);
                        //Debug.Log($"DEBUG_FillMap - was added: {WasAdded}");
                    }
                }
            }
        }

        public void FillOutMapEmpty(World_Generator instance)
        {
            Debug.Log($"Fillout was started! Chunk: {ChunkID}");
            for (int X = 0; X < _ChunkSize; X++)
            {
                for (int Y = 0; Y < _ChunkSize; Y++)
                {
                    //Debug.Log($"Chunk: {ChunkID} X: {X} Y: {Y} Status: {this.ChunkMap[X, Y]}");
                    if (this.ChunkMap[X,Y] == null)
                    {
                        this.AddTile(instance.streetEmpty, 0, X, Y);
                    }
                }
            }
        }

        public bool FillMap(World_Generator instance, bool Origin = false) //todo make sure it does not add an emtpy tile as the continues!!!
        {
            this.SoftLockEscape--;
            if(this.SoftLockEscape < 25) { Debug.Log("SOFTLOCK INCOMMING!"); }
            if(this.SoftLockEscape <= 0) { throw new TimeoutException("SoftlockEsacpe was triggered!"); }
            //when no ChunkType is defined (I am not forcing the chunk to have only 1 scenario block inside) continue with random
            if (this.ChunkType == null) 
            { 
                return this.FillMap_Random(instance, Origin); 
            }
            //else, run new function
            else
            {
                return this.FillMap_Type(instance, Origin);
            }

        }

        private bool FillMap_Random(World_Generator instance, bool Origin = false) //todo make sure it does not add an emtpy tile as the continues!!!
        {   //Fill map with random streetTiles/connections/...

            bool IsEmpty = true;
            //check if Map is EMPTY when this function is called first!!!
            for (int DX = 0; DX < _ChunkSize; DX++)
            {
                for (int DY = 0; DY < _ChunkSize; DY++)
                {
                    if (this.ChunkMap[DX, DY] != null) { IsEmpty = false; break; }
                }
            }

            //if empty chunkmap detected!!! add first tile! and call FillMap new
            if (IsEmpty)
            {
                bool firstStileAdded = this.AddTile(Chunk.GetRandomStreetType(instance), UnityEngine.Random.Range(0, 3),
                    UnityEngine.Random.Range(0, _ChunkSize - 1), UnityEngine.Random.Range(0, _ChunkSize - 1));
                if (firstStileAdded == true) { Debug.Log($"First tile added into Chunk {this.ChunkID}"); this.FillMap(instance, true); }
                else { Debug.Log($"First tile failed to add to Chunk {this.ChunkID} try again!"); this.FillMap(instance, true); }
            }

            //search for an unsatisfied tile in the current map
            int X; int Y;
            (X, Y) = this.GetCordsOfUnsatisfiedTile();
            //there are no more unsatusfied tiles!!! 
            if (X == -1 && Y == -1)
            {
                return true;
            }


            //get the direction in which to move next
            List<Direction> directions = this.FindOpenConnection(X, Y);
            int newSpawnX; int newSpawnY;
            (newSpawnX, newSpawnY) = GetNeighborCords(X, Y, directions[0]);
            //when this check is successful, neighbor is out of bounds, should NOT happen!  
            if (newSpawnX == -1 && newSpawnY == -1)
            {
                //Debug.Log("WARNING!!! Fillmap encountered an ERROR!!!");
                //Debug.Log($"FillMap found an open connection from Chunk: {this.ChunkID} X:{X}, Y:{Y} to Direction: {directions[0]} which would be out of bounds of the chunk!!!");
                throw new IndexOutOfRangeException($"FillMap found an open connection from Chunk: {this.ChunkID} X:{X}, Y:{Y} to Direction: {directions[0]} which would be out of bounds of the chunk!!!");
                //return false;
            }
            if (this.ChunkMap[newSpawnX, newSpawnY] != null)
            {
                return false;
            }

            List<GameObject> triedTiles = new List<GameObject>();

            while (triedTiles.Count < instance.NumberOfTileTypes)
            {
                bool tryagain = false;
                GameObject tileType = Chunk.GetRandomStreetType(instance);
                for (int i = 0; i < triedTiles.Count; i++)
                {
                    if (triedTiles[i].tag == tileType.tag)
                    {
                        //this tile type has already been tried!!!
                        tryagain = true;
                        break;
                    }
                }
                if (tryagain == true) { continue; }

                List<int> triedRotations = new List<int>();
                while (triedRotations.Count < 4)
                {
                    int tryRotation = UnityEngine.Random.Range(0, 4);
                    if (triedRotations.Contains(tryRotation)) { continue; }

                    //try and add a tile
                    bool WasSuccess = this.AddTile(tileType, tryRotation, newSpawnX, newSpawnY);
                    //if the tile not able to be placed, try another rotation
                    if (WasSuccess == false) { triedRotations.Add(tryRotation); this.RemoveTile(newSpawnX, newSpawnY); }
                    //if it was successfully placed... try fill map more
                    else
                    {
                        //Debug.Log("Initiated another FillMap");
                        bool mapStatus = FillMap(instance);
                        //if mapstatus is true, the map has been solved, we can return!
                        if (mapStatus == true)
                        {
                            if (Origin == true) { this.DEBUG_FillMap(instance, 0, instance.streetEmpty); }
                            return true;
                        }
                    }
                }
                triedTiles.Add(tileType);
            }
            //when all types of tiles have been tried and all rotations have been tried, nothing fits backtrack...
            return false;
        }

        private bool FillMap_Type(World_Generator instance, bool Origin = false) 
        {   //Fill map with only one decision block (if even)        
            //Debug.Log($"Chunktype: {this.ChunkType}");
            if ((this.ChunkType == instance.streetStraight) || (this.ChunkType == instance.streetCurve))
            {   //this is a boundery chunk... one with no decision block
                return FillMap_BounderyChunk(instance, Origin);
            }
            else if ((this.ChunkType == instance.street4Way) || (this.ChunkType == instance.streetTSection))
            {   //this is a decision chunk... 
                return FillMap_DecisionChunk(instance, Origin);
            }
            else { throw new InvalidOperationException("Invalid ChunkType value encountered. Possible? - Can't create a new chunk out of empty tile!"); }
        }

        private bool FillMap_DecisionChunk(World_Generator instance, bool Origin = false)
        { //todo check if the connections are considered propperly!
            //Debug.Log("FillMap_DecisionChunk has been evoked!");

            bool IsEmpty = true;
            //check if Map is EMPTY when this function is called first!!!
            for (int DX = 0; DX < _ChunkSize; DX++)
            {
                for (int DY = 0; DY < _ChunkSize; DY++)
                {
                    if (this.ChunkMap[DX, DY] != null) { IsEmpty = false; break; }
                }
            }

            //if empty chunkmap detected!!! add first tile! and call FillMap new
            if (IsEmpty)
            {   //adds the ONLY decision block into the chunk FIRST!!!
                bool firstTileAdded = this.AddTile(this.ChunkType, UnityEngine.Random.Range(0, 3),
                    UnityEngine.Random.Range(0, _ChunkSize - 1), UnityEngine.Random.Range(0, _ChunkSize - 1));
                if (firstTileAdded == true) { Debug.Log($"First tile added into Chunk {this.ChunkID}"); this.FillMap(instance, true); }
                else { Debug.Log($"First tile failed to add to Chunk {this.ChunkID} try again!"); this.FillMap(instance, true); }
            }

            //search for an unsatisfied tile in the current map
            int X; int Y;
            (X, Y) = this.GetCordsOfUnsatisfiedTile();
            //there are no more unsatusfied tiles!!! 
            if (X == -1 && Y == -1)
            {
                return true;
            }

            //get the direction in which to move next
            List<Direction> directions = this.FindOpenConnection(X, Y);
            int newSpawnX; int newSpawnY;
            (newSpawnX, newSpawnY) = GetNeighborCords(X, Y, directions[0]);
            //when this check is successful, neighbor is out of bounds, should NOT happen!  
            if (newSpawnX == -1 && newSpawnY == -1)
            {
                throw new IndexOutOfRangeException($"FillMap found an open connection from Chunk: {this.ChunkID} X:{X}, Y:{Y} to Direction: {directions[0]} which would be out of bounds of the chunk!!!");
            }
            if (this.ChunkMap[newSpawnX, newSpawnY] != null)
            {
                return false;
            }

            List<GameObject> triedTiles = new List<GameObject>();

            while (triedTiles.Count < 2) //check only for t-section or 4way street (eg 2 checks)
            {
                bool tryagain = false;
                GameObject tileType = UnityEngine.Random.Range(0, 2) == 0 ? instance.streetStraight : instance.streetCurve;

                for (int i = 0; i < triedTiles.Count; i++)
                {
                    if (triedTiles[i].tag == tileType.tag)
                    {
                        //this tile type has already been tried!!!
                        tryagain = true;
                        break;
                    }
                }
                if (tryagain == true) { continue; }

                List<int> triedRotations = new List<int>();
                while (triedRotations.Count < 4)
                {
                    int tryRotation = UnityEngine.Random.Range(0, 4);
                    if (triedRotations.Contains(tryRotation)) { continue; }

                    //try and add a tile
                    //Debug.Log($"Try add Tile: X:{newSpawnX} Y:{newSpawnY} Rot:{tryRotation}");
                    bool WasSuccess = this.AddTile(tileType, tryRotation, newSpawnX, newSpawnY);
                    //if the tile not able to be placed, try another rotation
                    if (WasSuccess == false) 
                    {               
                        triedRotations.Add(tryRotation); 
                        this.RemoveTile(newSpawnX, newSpawnY);
                        continue;
                    }   //if it was successfully placed... try fill map more
                    else
                    {
                        triedRotations.Add(tryRotation);
                        bool mapStatus = FillMap(instance);
                        //if mapstatus is true, the map has been solved, we can return!
                        //if(this.SoftLockEscape < 100) { Debug.Log($"{ triedRotations.Count} <-tried rotations, {triedTiles.Count} <- tried tiles" ); }

                        if (mapStatus == true)
                        {
                            if (Origin == true) { this.FillOutMapEmpty(instance); }
                            return true;
                        }
                        this.RemoveTile(newSpawnX, newSpawnY);
                    }
                    //DEBUG:
                    //Debug.Log($"Last tried rotation {tryRotation} with list lengh:{triedRotations.Count} in X{newSpawnX}, Y{newSpawnX}");
                    //string debugStr = "";
                    //foreach (int num in triedRotations) { debugStr += $" {num},"; }
                    //Debug.Log($"Tried rotations: {debugStr}");
                }
                triedTiles.Add(tileType);
            }
            this.RemoveTile(newSpawnX, newSpawnY);
            //when all types of tiles have been tried and all rotations have been tried, nothing fits backtrack...
            //Debug.Log("Backtrack");
            return false;
        }

        private bool FillMap_BounderyChunk(World_Generator instance, bool Origin = false)
        {
            Debug.Log("FillMap_BounderyChunk has been evoked!");
            return false;
        }

        private (int, int) GetCordsOfUnsatisfiedTile()
        {
            for(int X = 0; X < _ChunkSize; X++)
            {
                for (int Y = 0; Y < _ChunkSize; Y++)
                {
                    if (this.ChunkMap[X, Y] == null) { continue; }
                    List<Direction> directions = this.FindOpenConnection(X, Y);
                    if (directions.Count > 0) { return (X, Y); }
                }
            }
            return (-1, -1);
        }

        private (int, int) GetNeighborCords(int X, int Y, Direction direction) 
        {
            // code to calculate the cordinates of the neighboring tile
            int neighborX = X + (direction == Direction.Left ? -1 : (direction == Direction.Right ? 1 : 0));
            int neighborY = Y + (direction == Direction.Bottom ? -1 : (direction == Direction.Top ? 1 : 0));
            //check if the neighbor cordinates are within the chunk
            if (neighborX >= 0 && neighborX < _ChunkSize && neighborY >= 0 && neighborY < _ChunkSize)
            {
                //return neighbor cords
                return (neighborX, neighborY);
            }
            //return invalid cordinates
            return (-1, -1);
        }

        public bool CheckTileConnected(int X, int Y, Direction direction)  
            // function to check if a tile is connected propperly in a given direction...  
            // true, connections are satisfied (eg open connects to open or closed connects to closed)
        {            
            // code to calculate the cordinates of the neighboring tile
            int neighborX = X + (direction == Direction.Left ? -1: (direction == Direction.Right ? 1 : 0)); 
            int neighborY = Y + (direction == Direction.Bottom ? -1: (direction == Direction.Top ? 1 : 0));
            // x is left and right
            // y is up and down
            Direction RevDir = GetOppositeDirection(direction);
            //check if the neighbor cordinates are within the chunk
            if(neighborX >= 0 && neighborX < _ChunkSize && neighborY >= 0 && neighborY < _ChunkSize)
            {
                if(this.ChunkMap[neighborX, neighborY] == null) { return false; } //when tile non existent, return false
                if (this.ChunkMap[neighborX, neighborY].Connection(RevDir).Type == 
                    this.ChunkMap[X, Y].Connection(direction).Type) { return true; }
            }
            //todo add check if tile connection is valid in between chunks
            return false;
        }

        public List<Direction> FindOpenConnection(int X, int Y)
        {
            List<Direction> returnValues = new List<Direction>();

            foreach (Direction direction in Enum.GetValues(typeof(Direction))) 
            {
                if (this.ChunkMap[X, Y].Connection(direction).Type == Connection_Type.open)
                {
                    if (this.CheckTileConnected(X,Y,direction) == false)
                    {
                        returnValues.Add(direction);
                    }
                    
                }
            }

            return returnValues;
        }
    }

    public class WorldMap
    {
        public List<Chunk> Chunks;
        public float DEBUG_OFFSET = 5;
        int SpawnLocX, SpawnLocY, SpawnLocZ;

        public WorldMap(World_Generator instance, int X = 0, int Y = 0, int Z = 0)
        {
            this.SpawnLocX = X;
            this.SpawnLocY = Y;
            this.SpawnLocZ = Z; 

            this.Chunks = new List<Chunk>();                        //new list of chunks representing the map
            Chunk firstChunk = new(new(X, Y, Z));                   //create first chunk
            firstChunk.AddTile(instance.streetStraight, 0, 0, 0);   //first tile is always a street streight to start with
            //firstChunk.ChunkType = instance.streetStraight;
            firstChunk.FillMap(instance, true);                     //fill first chunk so it exists
            Chunks.Add(firstChunk);
        }

        public WorldMap(World_Generator instance, GameObject StreetType, int X = 0, int Y = 0, int Z = 0)
        {
            this.SpawnLocX = X;
            this.SpawnLocY = Y;
            this.SpawnLocZ = Z;

            this.Chunks = new List<Chunk>();                        //new list of chunks representing the map
            Chunk firstChunk = new(new(X, Y, Z));                   //create first chunk

            //set connections so, that only open connections to the right and top are present!
            for(int i = 0; i < firstChunk.Connections(Direction.Right).Length; i++)
            {
                firstChunk.Connections(Direction.Left)[i].Type = Connection_Type.closed;
                firstChunk.Connections(Direction.Left)[i].Fixed = true;
            }
            for (int i = 0; i < firstChunk.Connections(Direction.Bottom).Length; i++)
            {
                firstChunk.Connections(Direction.Bottom)[i].Type = Connection_Type.closed;
                //firstChunk.Connections(Direction.Right)[i].Fixed = true;
            }

            firstChunk.AddTile(StreetType, 0, 0, 0);                
            firstChunk.ChunkType = StreetType;
            firstChunk.FillMap(instance, true);                     //fill first chunk so it exists
            Chunks.Add(firstChunk);
        }

        public bool AddChunk(World_Generator instance, int OriginIndex, Direction direction, GameObject streetType = null)
        {
            if(streetType == null) { return AddChunk_Random(instance, OriginIndex, direction); }
            else if((streetType.tag == instance.streetTSection.tag) || streetType.tag == instance.street4Way.tag)
            {
                //decision chunk function
                return AddChunk_Decision(instance, OriginIndex, direction, streetType);
            }
            else if ((streetType.tag == instance.streetCurve.tag) || streetType.tag == instance.streetStraight.tag)
            {
                //AddChunk_Boundery chunk function
                return AddChunk_Boundery(instance, OriginIndex, direction, streetType);
            }
            else { throw new ArgumentException($"Invalid parameter on add chunk. Found Tag: {streetType.tag}"); }
        }

        private bool AddChunk_Random(World_Generator instance, int OriginIndex, Direction direction)
        {
            if (this.CheckIfFreeSpace(OriginIndex, direction) == false) { return false; } // could not add chunk space occupied
            int ChunkOffset = _ChunkSize * 25 + (int)DEBUG_OFFSET;

            //global cord system x and z plane y hight
            int X = (int)this.Chunks[OriginIndex].Position.x;
            int Y = (int)this.Chunks[OriginIndex].Position.y;
            int Z = (int)this.Chunks[OriginIndex].Position.z;

            // code to calculate the cordinates of the neighboring chunk
            int neighborX = X + (direction == Direction.Left ? -ChunkOffset : (direction == Direction.Right ? ChunkOffset : 0));
            int neighborZ = Z + (direction == Direction.Bottom ? -ChunkOffset : (direction == Direction.Top ? ChunkOffset : 0));

            //if any chunk occupies the same location as caluclated from the origin chunk, the there is no free space in this direction
            Vector3 TargetLocation = new(neighborX, Y, neighborZ);
            for(int i = 0; i < this.Chunks.Count; i++)
            {
                if (this.Chunks[i].Position == TargetLocation) { return false; }
            }            

            Chunk NewChunk = new(TargetLocation);

            //make sure the chunk fits to the chunk initiating its creation
            Connection[] connectionsToNewChunk = GetConnections(this.Chunks[OriginIndex].Position, direction);
            for (int i = 0; i < connectionsToNewChunk.Length; i++)
            {
                NewChunk.Connections(GetOppositeDirection(direction))[i] = connectionsToNewChunk[i];
            }
            //make sure the new chunk checks all around itself to see if there are already existing chunks which have connections
            List<Direction> checkDirection = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();
            checkDirection.Remove(GetOppositeDirection(direction));

            for (int i = 0; i < checkDirection.Count; i++)
            {
                Connection[] connectionsToCheck = GetConnections(NewChunk.Position, checkDirection[i]);
                if (connectionsToCheck != null)
                {
                    for (int y = 0; y < connectionsToCheck.Length; y++)
                    {
                        NewChunk.Connections(checkDirection[i])[y] = connectionsToCheck[y];
                    }
                }
            }
            //add new chunk to the map
            bool wasSuccessful = NewChunk.FillMap(instance, true);
            if (wasSuccessful) { this.Chunks.Add(NewChunk); return true; }
            else { return false; }
        }

        private bool AddChunk_Decision(World_Generator instance, int OriginIndex, Direction direction, GameObject streetType) 
        {
            if (this.CheckIfFreeSpace(OriginIndex, direction) == false) { return false; } // could not add chunk space occupied
            int ChunkOffset = _ChunkSize * 25 + (int)DEBUG_OFFSET;

            //global cord system x and z plane y hight
            int X = (int)this.Chunks[OriginIndex].Position.x;
            int Y = (int)this.Chunks[OriginIndex].Position.y;
            int Z = (int)this.Chunks[OriginIndex].Position.z;

            // code to calculate the cordinates of the neighboring chunk
            int neighborX = X + (direction == Direction.Left ? -ChunkOffset : (direction == Direction.Right ? ChunkOffset : 0));
            int neighborZ = Z + (direction == Direction.Bottom ? -ChunkOffset : (direction == Direction.Top ? ChunkOffset : 0));

            //if any chunk occupies the same location as caluclated from the origin chunk, the there is no free space in this direction
            Vector3 TargetLocation = new(neighborX, Y, neighborZ);
            for (int i = 0; i < this.Chunks.Count; i++)
            {
                if (this.Chunks[i].Position == TargetLocation) { return false; }
            }
            Chunk NewChunk = new(TargetLocation, streetType);

            //make sure the chunk fits to the chunk initiating its creation
            Connection[] connectionsToNewChunk = GetConnections(this.Chunks[OriginIndex].Position, direction);
            for (int i = 0; i < connectionsToNewChunk.Length; i++)
            {
                NewChunk.Connections(GetOppositeDirection(direction))[i] = connectionsToNewChunk[i];
                
            }

            //make sure the new chunk checks all around itself to see if there are already existing chunks which have connections
            List<Direction> checkDirection = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();
            checkDirection.Remove(GetOppositeDirection(direction));

            for (int i = 0; i < checkDirection.Count; i++)
            {
                Connection[] connectionsToCheck = GetConnections(NewChunk.Position, checkDirection[i]);
                if (connectionsToCheck != null)
                {
                    for (int y = 0; y < connectionsToCheck.Length; y++)
                    {
                        NewChunk.Connections(checkDirection[i])[y] = connectionsToCheck[y];
                    }
                }
            }
            //add new chunk to the map
            NewChunk.ChunkType = streetType;
            bool wasSuccessful = NewChunk.FillMap(instance, true);
            if (wasSuccessful) { 
                this.Chunks.Add(NewChunk);
                Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
                foreach (Direction Loopdirection in directions)
                {
                    for (int i = 0; i < NewChunk.Connections(Loopdirection).Length; i++)
                    {
                        NewChunk.Connections(Loopdirection)[i].Fixed = true;
                    }
                }

                return true; 
            }
            else { return false; }
        }

        private bool AddChunk_Boundery(World_Generator instance, int OriginIndex, Direction direction, GameObject streetType) 
        {
            Debug.Log("Not implemented yet!"); //todo
            return false;
        }

        private bool CheckIfFreeSpace(int ChunkIndex, Direction direction) // need alt function for if I only have a vector and not an index f
        {
            int ChunkOffset = _ChunkSize * 25 + (int)DEBUG_OFFSET;

            //global cord system x and z plane y hight
            int X = (int)this.Chunks[ChunkIndex].Position.x;
            int Y = (int)this.Chunks[ChunkIndex].Position.y;
            int Z = (int)this.Chunks[ChunkIndex].Position.z;
                        
            // code to calculate the cordinates of the neighboring chunk
            int neighborX = X + (direction == Direction.Left ? -ChunkOffset : (direction == Direction.Right ? ChunkOffset : 0));
            int neighborZ = Z + (direction == Direction.Bottom ? -ChunkOffset : (direction == Direction.Top ? ChunkOffset : 0));

            //if any chunk occupies the same location as caluclated from the origin chunk, the there is no free space in this direction
            Vector3 TargetLocation = new(neighborX, Y, neighborZ);
            for(int i = 0; i < this.Chunks.Count; i++)
            {
                if (this.Chunks[i].Position == TargetLocation) { return false; }
            }
            //if no chunk occupying the space if been found, return true
            return true; 
        }

        private int GetChunkIndex(Vector3 ChunkLocation)
        {
            for (int i = 0; i < this.Chunks.Count; i++)
            {
                if (this.Chunks[i].Position == ChunkLocation) { return i; }
            }
            return -1;
        }

        private Connection[] GetConnections(Vector3 ChunkLocation, Direction direction)
        {
            int ChunkOffset = _ChunkSize * 25 + (int)DEBUG_OFFSET;

            int ChunkIndex = this.GetChunkIndex(ChunkLocation);
            //first, check if this chunk exists within the list of chunks, if it does, return directions
            if(ChunkIndex != -1) { return this.Chunks[ChunkIndex].Connections(direction); }
            //check if the surrounding chunk exists and if so, get its connection with the missing chunk
            //code to calculate the cordinates of the neighboring chunk
            int neighborX = (int)ChunkLocation.x + (direction == Direction.Left ? -ChunkOffset : (direction == Direction.Right ? ChunkOffset : 0));
            int neighborZ = (int)ChunkLocation.z + (direction == Direction.Bottom ? -ChunkOffset : (direction == Direction.Top ? ChunkOffset : 0));
            ChunkIndex = this.GetChunkIndex(new Vector3(neighborX, ChunkLocation.y, neighborZ));
            if (ChunkIndex != -1) { return this.Chunks[ChunkIndex].Connections(GetOppositeDirection(direction)); }
            //no connections found
            return null;   
        }


    }

    public List<Chunk> Map;
    public WorldMap MyMap;
    

    // Start is called before the first frame update
    void Start()
    {
        //Map = new();
        //Chunk TestChunk = new(new Vector3(0, 0, 0));
        //Chunk TestChunk2 = new(new Vector3(100, 0, 100));
        //Map.Add(TestChunk);
        //Map.Add(TestChunk2);

        //Map[0].AddTile(streetStraight, 0, 0, 0);
        //Map[1].AddTile(streetCurve, 1, 1, 1);

        //Debug.Log(Map[0].FindOpenConnection(0, 0).Count);

        //Map[0].FillMap(this, true);
        //Map[1].FillMap(this, true);

        //for (int i = 0; i < 20; i++)
        //{
        //    Chunk loopTest = new(new Vector3(80 * i, 0, 80));
        //    loopTest.AddTile(Chunk.GetRandomStreetType(this), UnityEngine.Random.Range(0, 4), 1, 1);
        //    loopTest.FillMap(this, true);
        //    Map.Add(loopTest);
        //}


        MyMap = new WorldMap(this, streetTSection ,(int)PlayerEntity.transform.position.x, (int)PlayerEntity.transform.position.y, 0);

        //MyMap.AddChunk(this, 0, Direction.Top, street4Way);

        MyMap.AddChunk(this, 0, Direction.Right, street4Way);

        MyMap.AddChunk(this, 1, Direction.Right, street4Way);
        MyMap.AddChunk(this, 2, Direction.Right, street4Way);


        for (int i = 3; i < 10; i++)
        {
            Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
            foreach (Direction Loopdirection in directions)
            {
                MyMap.AddChunk(this, i, Direction.Right, street4Way);
            }

        }


        //MyMap.AddChunk(this, 1, Direction.Top);

        //MyMap.AddChunk(this, 2, Direction.Top);

        //MyMap.AddChunk(this, 3, Direction.Top);

        //MyMap.AddChunk(this, 0, Direction.Left);

    }

    //private int TestMapIndex = 0;
    //private float elapsedTime = 0f;
    //private float updateInterval = 1f; // 1 second

    void Update()
    {
        //// Update the elapsed time
        //elapsedTime += Time.deltaTime;

        //// Check if one second has passed
        //if (elapsedTime >= updateInterval)
        //{
        //    Debug.Log("try add another chunk");
        //    Debug.Log(MyMap.AddChunk(this, TestMapIndex, Direction.Top));

        //    // Reset the timer
        //    elapsedTime = 0f;
        //}
        //MyMap = new WorldMap(this, streetTSection, (int)PlayerEntity.transform.position.x, (int)PlayerEntity.transform.position.y, 0);
    }




}