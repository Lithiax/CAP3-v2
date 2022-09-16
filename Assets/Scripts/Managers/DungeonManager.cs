using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardinalDirection
{
    North,
    East,
    South,
    West,
    Center,
    None
}

[System.Serializable]
public class RoomTemplate
{
    public CardinalDirection direction;
    public List<Room> prefabs = new List<Room>();
}
public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance;
    [SerializeField] private int amountOfRooms;
    //lets say size is 20:40, half them below so it becomes 10:20
    public float verticalGridSize = 10; //10
    public float horizontalGridSize = 20; // 20

    public List<Room> rooms = new List<Room>();

    public List<RoomTemplate> roomTemplates = new List<RoomTemplate>();

    public void OnEnable()
    {
        //Spawn starter room
        NewSpawnNewRoom();
        StartCoroutine(TrySpawn());

    }

    IEnumerator TrySpawn()
    {
        yield return new WaitForSeconds(3f);
        NewSpawnNewRoom();
        yield return new WaitForSeconds(3f);
        NewSpawnNewRoom();
        yield return new WaitForSeconds(3f);
        NewSpawnNewRoom();
        yield return new WaitForSeconds(3f);
        NewSpawnNewRoom();
        yield return new WaitForSeconds(3f);
        NewSpawnNewRoom();
        yield return new WaitForSeconds(3f);
        NewSpawnNewRoom();
        yield return new WaitForSeconds(1f);
        NewSpawnNewRoom();
    }
    public void NewSpawnNewRoom()
    {
        //PROBLEMS
        //IF HAVENT FOUND ANY/LIST IS EMPTY/COUNT IS 0 WHAT TO DO, WHAT STEP TO GO BACK


        //Check for room(X)s in list that has directions/passageways that are unoccupied and make list(XL) to add it

        List<Room> potentialRoomBasis = new List<Room>();
        if (rooms.Count > 0)
        {
            foreach (Room room in rooms)
            {
                Debug.Log("passa");
                if (GetSurroundingRoomsThatAreNotOccupied(room).Count > 0)
                {
                    Debug.Log("test");

                    potentialRoomBasis.Add(room);
                }
            }

            //Choose which room(X) to serve as basis for spawning a new tile from rooms (OL) list
            int chosenRoomBasis = -1;

            //Choose direction/passageway from list(XL) to spawn new room (Y)
            chosenRoomBasis = Random.Range(0, potentialRoomBasis.Count);

            Debug.Log(potentialRoomBasis.Count);
            //Choose direction/passageway from list(XL) to spawn new room (Y)
            List<Passageway> newRoomPassageways = GetSurroundingRoomsThatAreNotOccupied(potentialRoomBasis[chosenRoomBasis]);
            int chosenNewRoomPotential = -1;
            if (newRoomPassageways.Count > 0)
            {
                foreach (Passageway test in newRoomPassageways)
                {
                    Debug.Log("ADDED TO UNOCCUPIED" + test.name.ToString());
                }

                chosenNewRoomPotential = Random.Range(0, newRoomPassageways.Count);
                Debug.Log(potentialRoomBasis[chosenRoomBasis].gameObject.name + " - " + newRoomPassageways.Count + " CHOSEN : " + chosenNewRoomPotential);
            }
            else
            {
                Debug.Log("Chosen room from list is already surrounded by rooms that does not connect to it");
                //Choose which room(X) to serve as basis for spawning a new tile from rooms (OL) list
            }

            Vector2 newRoomPosition = (Vector2)potentialRoomBasis[chosenRoomBasis].transform.position;
            Debug.Log(newRoomPosition);
            if (newRoomPassageways[chosenNewRoomPotential].cardinalDirection == CardinalDirection.North)
            {
                newRoomPosition += new Vector2(0, verticalGridSize);

            }
            else if (newRoomPassageways[chosenNewRoomPotential].cardinalDirection == CardinalDirection.South) //Vertical
            {
                newRoomPosition += new Vector2(0, -verticalGridSize);
            }
            else if (newRoomPassageways[chosenNewRoomPotential].cardinalDirection == CardinalDirection.East) //Horizontal
            {
                newRoomPosition += new Vector2(horizontalGridSize, 0);
            }
            else if (newRoomPassageways[chosenNewRoomPotential].cardinalDirection == CardinalDirection.West) //Horizontal
            {
                newRoomPosition += new Vector2(-horizontalGridSize, 0);
            }
            Debug.Log(newRoomPosition);

            //Check unspawned new room (Y)'s surrounding rooms (Z) if they have direction/passageway at the place of new room (Y) and make bool/direction list (ZL) (NEED TO OPTIMIZE)
            List<Passageway> surroundingPassageWaysLinking = new List<Passageway>();
            Vector2 detectorSize = new Vector2(5, 5);
            //North
            AddSpawnableRooms(newRoomPosition + new Vector2(0, verticalGridSize), detectorSize, CardinalDirection.South, surroundingPassageWaysLinking);
            //East
            AddSpawnableRooms(newRoomPosition + new Vector2(horizontalGridSize, 0), detectorSize, CardinalDirection.West, surroundingPassageWaysLinking);
            //South
            AddSpawnableRooms(newRoomPosition + new Vector2(0, -verticalGridSize), detectorSize, CardinalDirection.North, surroundingPassageWaysLinking);
            //West
            AddSpawnableRooms(newRoomPosition + new Vector2(-horizontalGridSize, 0), detectorSize, CardinalDirection.East, surroundingPassageWaysLinking);

            //Make list of room prefabs (YL) that has all the bools/directions in the bool/direction list (ZL)
            List<Room> prefabThatCanBeSpawned = new List<Room>();

            CardinalDirection cdTemp = newRoomPassageways[chosenNewRoomPotential].cardinalDirection;
            int chosenRoomTemplate = -1;
            if (roomTemplates.Count > 0)
            {
                for (int i = 0; i < roomTemplates.Count; i++)
                {
                    if (roomTemplates[i].direction == cdTemp)
                    {
                        chosenRoomTemplate = i;
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("Cant choose room template, roomTemplates is empty, Please make new roomTemplate in DungeonManager Inspector");
            }


            //TEMP CONVERT PASSAGE WAY TO CARDINAL DIRECTION
            List<CardinalDirection> cdListTemp = new List<CardinalDirection>();
            foreach (Passageway currentSurroundingPassageWaysLinking in surroundingPassageWaysLinking)
            {
                cdListTemp.Add(currentSurroundingPassageWaysLinking.cardinalDirection);
            }

            foreach (Room currentRoomPrefab in roomTemplates[chosenRoomTemplate].prefabs)
            {
                if (HasPassagewaysForRequiredDirections(currentRoomPrefab,cdListTemp))
                {
                    prefabThatCanBeSpawned.Add(currentRoomPrefab);
                }
            }

            //Choose what room prefab to spawn for new room (Y) from list of room prefabs (YL)
            int chosenRoomPrefab = -1;
            if (prefabThatCanBeSpawned.Count > 0)
            {
                //Choose direction/passageway from list(XL) to spawn new room (Y)
                chosenRoomPrefab = Random.Range(0, prefabThatCanBeSpawned.Count);
            }
            else
            {
                Debug.Log("Cant choose room prefab, list is empty. Please make a room prefab that connects to the following directions: " + cdListTemp.ToString());


            }
            Debug.Log(newRoomPosition);

            //INSTATIATING NEW ROOM
            InstantiateNewRoom(prefabThatCanBeSpawned[chosenRoomPrefab], newRoomPosition, surroundingPassageWaysLinking);

        }
        else
        {
            //Create new room because there are no room in rooms yet
            int chosenRoomPrefab = Random.Range(0, roomTemplates[4].prefabs.Count);

            //INSTATIATING NEW ROOM

            Room newRoom = Instantiate(roomTemplates[4].prefabs[chosenRoomPrefab]);
            rooms.Add(newRoom);
            newRoom.transform.position = new Vector2(0, 0);
        }


    }

    public void SpawnNewRoom()
    {
        //PROBLEMS
        //IF HAVENT FOUND ANY/LIST IS EMPTY/COUNT IS 0 WHAT TO DO, WHAT STEP TO GO BACK

        //Choose which room(X) to serve as basis for spawning a new tile from rooms (OL) list
        int chosenRoom = -1;
        if (rooms.Count > 0)
        {
            chosenRoom = Random.Range(0, rooms.Count);
        }
        else
        {
            Debug.Log("There is no room in list yet");
        }

        //Check if room(X) has directions/passageways that are unoccupied and make list(XL) to add it
        List<Passageway> newRoomPotential = GetSurroundingRoomsThatAreNotOccupied(rooms[chosenRoom]);

        int chosenNewRoomPotential = -1;
        if (newRoomPotential.Count > 0)
        {
            //Choose direction/passageway from list(XL) to spawn new room (Y)
            chosenNewRoomPotential = Random.Range(0, rooms.Count);
        }
        else
        {
            Debug.Log("Chosen room from list is already surrounded by rooms");
            //Choose which room(X) to serve as basis for spawning a new tile from rooms (OL) list
        }

        Vector2 newRoomPosition = (Vector2)rooms[chosenRoom].transform.position;
        if (newRoomPotential[chosenNewRoomPotential].cardinalDirection == CardinalDirection.North ||
            newRoomPotential[chosenNewRoomPotential].cardinalDirection == CardinalDirection.South) //Vertical
        {
            newRoomPosition += new Vector2(0, verticalGridSize);
        }
        else if (newRoomPotential[chosenNewRoomPotential].cardinalDirection == CardinalDirection.East ||
                newRoomPotential[chosenNewRoomPotential].cardinalDirection == CardinalDirection.West) //Horizontal
        {
            newRoomPosition += new Vector2(horizontalGridSize, 0);
        }

        //Check unspawned new room (Y)'s surrounding rooms (Z) if they have direction/passageway at the place of new room (Y) and make bool/direction list (ZL) (NEED TO OPTIMIZE)

        List<Passageway> surroundingPassageWaysLinking = new List<Passageway>();

        Vector2 detectorSize = new Vector2(5, 5);
        //North

        CheckIfRoomSpawnable(newRoomPosition + new Vector2(0, verticalGridSize), detectorSize, CardinalDirection.South, surroundingPassageWaysLinking);
        //East
        CheckIfRoomSpawnable(newRoomPosition + new Vector2(horizontalGridSize, 0), detectorSize, CardinalDirection.West, surroundingPassageWaysLinking);
        //South
        CheckIfRoomSpawnable(newRoomPosition + new Vector2(0, -verticalGridSize), detectorSize, CardinalDirection.North, surroundingPassageWaysLinking);
        //West
        CheckIfRoomSpawnable(newRoomPosition + new Vector2(-horizontalGridSize, 0), detectorSize, CardinalDirection.East, surroundingPassageWaysLinking);
        //Make list of room prefabs (YL) that has all the bools/directions in the bool/direction list (ZL)
        List<Room> prefabThatCanBeSpawned = new List<Room>();

        CardinalDirection cdTemp = newRoomPotential[chosenNewRoomPotential].cardinalDirection;
        int chosenRoomTemplate = -1;
        if (roomTemplates.Count > 0)
        {
            for (int i = 0; i < roomTemplates.Count; i++)
            {
                if (roomTemplates[i].direction == cdTemp)
                {
                    chosenRoomTemplate = i;
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Cant choose room template, roomTemplates is empty");
        }


        //TEMP CONVERT PASSAGE WAY TO CARDINAL DIRECTION
        List<CardinalDirection> cdListTemp = new List<CardinalDirection>();
        foreach (Passageway currentSurroundingPassageWaysLinking in surroundingPassageWaysLinking)
        {
            cdListTemp.Add(currentSurroundingPassageWaysLinking.cardinalDirection);
        }

        foreach (Room currentRoomPrefab in roomTemplates[chosenRoomTemplate].prefabs)
        {
            if (HasPassagewaysForRequiredDirections(currentRoomPrefab,cdListTemp))
            {
                prefabThatCanBeSpawned.Add(currentRoomPrefab);
            }
        }

        //Choose what room prefab to spawn for new room (Y) from list of room prefabs (YL)
        int chosenRoomPrefab = -1;
        if (prefabThatCanBeSpawned.Count > 0)
        {
            //Choose direction/passageway from list(XL) to spawn new room (Y)
            chosenRoomPrefab = Random.Range(0, prefabThatCanBeSpawned.Count);
        }
        else
        {
            Debug.Log("Cant choose room prefab, list is empty");

        }
    }

    public void InstantiateNewRoom(Room p_roomToSpawn, Vector2 p_newRoomPosition, List<Passageway> p_surroundingPassageWaysLinking)
    {
        Room newRoom = Instantiate(p_roomToSpawn);
        rooms.Add(newRoom);
        newRoom.transform.position = p_newRoomPosition;

        //LINK ALL PASSAGEWAYS
        LinkAllPassageways(newRoom,p_surroundingPassageWaysLinking);
    }

    public void LinkAllPassageways(Room p_targetRoom, List<Passageway> p_surroundingPassageWaysLinking)
    {
        foreach (Passageway cp in p_surroundingPassageWaysLinking)
        {
            Debug.Log(cp.gameObject.name);
            //CardinalDirection surroundingRoomPassageway = CardinalDirection.None;
            CardinalDirection newRoomPassageway = CardinalDirection.None;
            if (cp.cardinalDirection == CardinalDirection.North)
            {
                newRoomPassageway = CardinalDirection.South;
            }
            else if (cp.cardinalDirection == CardinalDirection.East)
            {
                newRoomPassageway = CardinalDirection.West;
            }
            else if (cp.cardinalDirection == CardinalDirection.South)
            {
                newRoomPassageway = CardinalDirection.North;
            }
            else if (cp.cardinalDirection == CardinalDirection.West)
            {
                newRoomPassageway = CardinalDirection.East;
            }


            foreach (PassagewayData cpr in GetPassageways(p_targetRoom))
            {
                if (cpr.passageway.cardinalDirection == newRoomPassageway)
                {
                    cp.connectedToPassageway = cpr.passageway;
              
                    cpr.passageway.connectedToPassageway = cp;
               
                }
            }
        }
    }
    public void AddSpawnableRooms(Vector2 p_VectorCheckForRoom, Vector2 p_detectorSize, CardinalDirection p_direction, List<Passageway> p_surroundingPassageWaysLinking)
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(p_VectorCheckForRoom, p_detectorSize, 0f);
        List<Room> roomsSurroundingNewRoom = new List<Room>();
        foreach (Collider2D currentHitCollider in hitColliders)
        {
            Debug.Log(currentHitCollider.gameObject.name.ToString());
            if (currentHitCollider.gameObject.name == "Ground")//Specific tile layer
            {
                if (currentHitCollider.gameObject.transform.parent.GetComponent<Room>())
                {
                    Room roomFound = currentHitCollider.gameObject.transform.parent.GetComponent<Room>();
                    roomsSurroundingNewRoom.Add(roomFound);
                    Debug.Log("T");
                    Passageway passagewayRoomSurroundingNewRoom = GetPassagewayMatchingDirection(roomFound, CardinalDirection.West);
                    if (passagewayRoomSurroundingNewRoom != null) //if this room (tr) is north of newroom, I need this room's(tr) south passage way
                    {
                        Debug.Log("R");
                        if (passagewayRoomSurroundingNewRoom.connectedToPassageway == null)
                        {
                            //If it hits this condition, it means passageway exists, else there's no passageway wanting to connect to here
                            p_surroundingPassageWaysLinking.Add(passagewayRoomSurroundingNewRoom);
                        }
                    }

                }
            }
        }
    }
    public void CheckIfRoomSpawnable(Vector2 p_VectorCheckForRoom, Vector2 p_detectorSize, CardinalDirection p_direction, List<Passageway> p_surroundingPassageWaysLinking)
    {
        List<Room> roomsSurroundingNewRoom = new List<Room>();

        Collider[] hitColliders = Physics.OverlapBox(p_VectorCheckForRoom, p_detectorSize, Quaternion.identity);
        foreach (Collider currentHitCollider in hitColliders)
        {
            if (currentHitCollider.TryGetComponent<Room>(out Room roomFound))//gameObject.GetComponent<Room>())
            {
                roomsSurroundingNewRoom.Add(roomFound);
                if (GetPassagewayMatchingDirection(roomFound, p_direction) != null) //if this room (tr) is north of newroom, I need this room's(tr) East passage way
                {
                    //If it hits this condition, it means passageway exists, else there's no passageway wanting to connect to here
                    p_surroundingPassageWaysLinking.Add(GetPassagewayMatchingDirection(roomFound, p_direction));
                }
            }
        }

    }

    public CardinalDirection RandomizeDirection()
    {
        return (CardinalDirection)Random.Range(0, System.Enum.GetValues(typeof(CardinalDirection)).Length - 2); // 2 because there is no direction and center direction

    }

    public RoomTemplate MatchDirection(CardinalDirection p_direction)
    {
        foreach (RoomTemplate currentRoomTemplate in roomTemplates) // Loop through directions in list
        {
            if (currentRoomTemplate.direction == p_direction) //Find list that matches direction
            {
                return currentRoomTemplate;
            }
        }
        return null;
    }

    public int ChooseRoom(CardinalDirection p_direction)
    {
        RoomTemplate currentRoomTemplate = MatchDirection(p_direction);
        if (currentRoomTemplate.prefabs.Count > 0)
        {

            return Random.Range(0, currentRoomTemplate.prefabs.Count);
        }
        else
        {

            return -1;

        }


    }

    public void GenerateDungeon()
    {

    }

    #region Static Functions
    public List<PassagewayData> GetPassageways(Room p_room)
    {
        return p_room.passagewayDatas;
    }

    public bool HasPassagewaysForRequiredDirections(Room p_room,List<CardinalDirection> p_dir)
    {
        //Check if all directions needed is present, if one of them is missing, this is not the right room
        foreach (CardinalDirection currentPassageway in p_dir)
        {
            for (int i = 0; i < p_room.passagewayDatas.Count;)
            {
                if (currentPassageway == p_room.passagewayDatas[i].passageway.cardinalDirection) //Passageway has the direction
                {
                    break; //move on to the next cardinal direction
                }
                i++;
                if (i >= p_room.passagewayDatas.Count) //If loop reaches end and wasnt able to find any passageways with the same direction of list
                {
                    return false;
                }

            }

        }
        return true;
    }
    public List<Passageway> GetSurroundingRoomsThatAreNotOccupied(Room p_room)
    {
        List<Passageway> unoccupied = new List<Passageway>();
        foreach (PassagewayData currentPassagewayData in p_room.passagewayDatas) // Loop through directions in list
        {
            if (currentPassagewayData.passageway.connectedToPassageway == null) //If it isnt connected already
            {
                Debug.Log("ADDED");
                unoccupied.Add(currentPassagewayData.passageway);

            }
        }

        return unoccupied;//It isnt connected already
    }
    public Passageway GetPassagewayMatchingDirection(Room p_room, CardinalDirection p_direction)
    {
        foreach (PassagewayData currentPassageway in p_room.passagewayDatas) // Loop through directions in list
        {
            if (currentPassageway.passageway.cardinalDirection == p_direction) //Find list that matches direction
            {
                return currentPassageway.passageway;
            }
        }
        return null;
    }
    #endregion
}
