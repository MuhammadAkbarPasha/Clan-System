using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGGamesCore.Matchmaker
{
    //NOTE: This is a sample implementation. Feel free to add more fields or inherit from this class.
    //This was primarily made with Murasaki 7's Arena Matchmaking System in mind since that implementation
    //uses party attack levels as another way to determine if a player is elligible to fight with you, alongside the 
    //rank of the opponent.
    public class PlayerMatchamkingPartyData 
    {
        public int partyAttackLevel;
    }
}
