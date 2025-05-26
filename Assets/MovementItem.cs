using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MovementItem : InteractableObject
{
    public RoomManager roomManager;
    public Transform teleportPosition;
    public int destinationRoomIndex; //TODO: could be calculated based on the teleportPosition

    public override void Interact(GameObject character, Player playerScript)
    {
        TeleportCharacter(character);
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
        roomManager.UpdateHiders(roomLeaving, roomEntering);
    }
}
