pragma solidity ^0.4.18;

contract Renting {

  mapping (bytes32 => bool) public roomBooked;
  bytes32[] public roomNumber;

  function Renting(bytes32[] rooms) public {
    roomNumber = rooms;
  }

  function bookRoom(bytes32 room) public {
    require(isvalidRoom(room));
    roomBooked[room] = true;
  }

  function unbookRoom(bytes32 room) public {
    require(isvalidRoom(room));
    roomBooked[room] = false;
  }
  
  function isRoomEmpty(bytes32 room) view public returns (bool) {
      return roomBooked[room];
  }
  
  function isvalidRoom(bytes32 room) view public returns (bool) {
    for(uint i = 0; i < roomNumber.length; i++) {
      if (roomNumber[i] == room){
        return true;
      }
    }
    return false;
  }
}
