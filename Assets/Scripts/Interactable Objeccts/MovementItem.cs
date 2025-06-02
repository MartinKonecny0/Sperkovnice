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
        // player is using door in this room and leaving it
        GameObject roomLeaving = character.transform.parent.gameObject;
        // player is teleported to this room
        GameObject roomEntering = roomManager.activeRooms[destinationRoomIndex].transform.gameObject;
        character.transform.position = teleportPosition.position;
        character.transform.parent = roomEntering.transform;
        roomManager.presentRoom = roomEntering;
        roomManager.SwapHiders(roomLeaving, roomEntering);
    }
}
