using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MovementItem : InteractableObject
{
    public RoomManager roomManager;
    public Transform teleportPosition;
    public int destinationRoomIndex;
    public CharacterType[] charactersAbleToOpen; // whitelist of characters that are able to use this item

    void Start()
    {
        type = InteractableType.movement;
    }
    public override void Interact(GameObject character, Player playerScript)
    {
        if (charactersAbleToOpen.Length == 0 || charactersAbleToOpen.Contains(playerScript.currentCharacter))
        {
            TeleportCharacter(character);
        }
        else
        {
            Debug.Log("Character can`t use this item");
        }
    }
    private void TeleportCharacter(GameObject character)
    {
        // hider management
        int roomLeavingIndex = character.transform.parent.GetComponent<Room>().roomPositionIndex;
        roomManager.CloseHider(roomLeavingIndex);
        roomManager.OpenHider(destinationRoomIndex);

        // player is teleported to this room
        GameObject roomEntering = roomManager.activeRooms[destinationRoomIndex].transform.gameObject;
        character.transform.position = teleportPosition.position;
        character.transform.parent = roomEntering.transform;
        roomManager.presentRoom = roomEntering;
    }
}
