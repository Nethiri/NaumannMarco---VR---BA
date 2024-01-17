using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public List<Chunk> Map;
    const int _ChunkSize = 3;

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
            NewTileObject.name = $"Chunk: {ChunkID} - Street at (X:{X}, Y:{Y})";
            //calculate the location the tile is going to be set to (global location)
            Vector3 SpawnPoint = new(this.Position.x + 25f * X, Z, this.Position.y + 25f * Y);
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
                    //check if the tile and the connections required are the same or not
                    if(this.Connections(Direction.Left)[Y].Type != this.ChunkMap[X, Y].Connection(Direction.Left).Type)
                    {
                        this.RemoveTile(X, Y);
                        Debug.Log($"Failed to add a tile at X: {X}, Y: {Y} in Chunk: {ChunkID}! Tile in Direction {Direction.Left} is fixed!");
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
                        bool WasAdded = AddTile(RandomStreetType, RandomRotation, X, Y);
                        Debug.Log($"DEBUG_FillMap - was added: {WasAdded}");
                    }
                }
            }
        }

        public bool FillMap(World_Generator instance, bool Origin = false)
        {
            //search for an unsatisfied tile in the current map
            int X; int Y;
            (X, Y) = GetCordsOfUnsatisfiedTile();
            //there are no more unsatusfied tiles!!!
            if(X == -1 && Y == -1) { return true; }
            
            //get the direction in which to move next
            List<Direction> directions = this.FindOpenConnection(X, Y);
            int newSpawnX; int newSpawnY;
            (newSpawnX, newSpawnY) = GetNeighborCords(X, Y, directions[0]);
            //when this check is successful, neighbor is out of bounds, should NOT happen!  
            if (newSpawnX == -1 && newSpawnY == -1) 
            {
                Debug.Log("WARNING!!! Fillmap encountered an ERROR!!!");
                Debug.Log($"FillMap found an open connection from Chunk: {this.ChunkID} X:{X}, Y:{Y} to Direction: {directions[0]} which would be out of bounds of the chunk!!!");
                return false; 
            }


            List<GameObject> triedTiles = new List<GameObject>();
            
            while(triedTiles.Count < instance.NumberOfTileTypes)
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
                while(triedRotations.Count < 4)
                {
                    int tryRotation = UnityEngine.Random.Range(0, 4);
                    if(triedRotations.Contains(tryRotation)) { continue; }

                    //try and add a tile
                    bool WasSuccess = this.AddTile(tileType, tryRotation, newSpawnX, newSpawnY);
                    //if the tile not able to be placed, try another rotation
                    if (WasSuccess == false) { triedRotations.Add(tryRotation); this.RemoveTile(newSpawnX, newSpawnY); }
                    //if it was successfully placed... try fill map more
                    else
                    {
                        Debug.Log("Initiated another FillMap");
                        bool mapStatus = FillMap(instance);
                        //if mapstatus is true, the map has been solved, we can return!
                        if(mapStatus == true) {
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
        public float DEBUG_OFFSET = 0;  

        public WorldMap(World_Generator instance)
        {
            Chunks = new List<Chunk>();
            Chunk firstChunk = new(new(0, 0, 0));
            firstChunk.AddTile(instance.streetStraight, 0, 0, 0);
            firstChunk.FillMap(instance, true);
            Chunks.Add(firstChunk);
        }

        public bool AddChunk(int OriginIndex, Direction direction)
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
            Chunk NewChunk = new(TargetLocation);
            
            //todo chunk needs to fit to origin / surrounding chunks



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

        private Connection[] getConnections(Vector3 ChunkLocation, Direction direction)
        {
            int ChunkIndex = this.GetChunkIndex(ChunkLocation);
            //first, check if this chunk exists within the list of chunks

            if(ChunkIndex != -1) { return this.Chunks[ChunkIndex].Connections(direction); }
            //todo what to do if I dont have an index and I try to get the connections from a theoretical chunk eg one that I want to spawn or that has yet to spawn


            return new Connection[this.Chunks.Count];   
        }


    }


    // Start is called before the first frame update
    void Start()
    {
        Map = new();
        Chunk TestChunk = new(new Vector3(0, 0, 0));
        Chunk TestChunk2 = new(new Vector3(-80, 0, 0));
        Map.Add(TestChunk);
        Map.Add(TestChunk2);
        
        Map[0].AddTile(streetStraight, 0, 0, 0);
        Map[1].AddTile(streetCurve, 1, 1, 1);

        Debug.Log(Map[0].FindOpenConnection(0, 0).Count);

        Map[0].FillMap(this, true);
        Map[1].FillMap(this, true);

        for(int i = 0; i < 20; i++)
        {
            Chunk loopTest = new(new Vector3(80 * i, 0, 80));
            loopTest.AddTile(Chunk.GetRandomStreetType(this), UnityEngine.Random.Range(0,4), 1,1);
            loopTest.FillMap(this, true);
            Map.Add(loopTest);    
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Map[0].RotateTile(1, 1, UnityEngine.Random.Range(0, 4));
        //Map[0].RotateTile(0, 1, UnityEngine.Random.Range(0, 4));
        //Map[0].RotateTile(2, 2, UnityEngine.Random.Range(0, 4));



    }




}