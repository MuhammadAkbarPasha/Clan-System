using System.Collections;
using System.Collections.Generic;
using System;

//Data class taken from Murasaki7
[Serializable]
public class ArenaPlayer
{
    public string avatarUrl;
    public string playFabId;
    public string name;
    public int mmrWeekly;
    public int mmrMonthly;
    public int position;
    public int winStreak;
	public int level;
   // public PartyObject party;

    public ArenaPlayer(string playFabId, string name, int mmrWeekly, int mmrMonthly, int position, int winStreak, int winStreakHistorical, int level)
    {
        this.playFabId = playFabId;
        this.name = name;
        this.mmrWeekly = mmrWeekly;
        this.mmrMonthly = mmrMonthly;
        this.position = position;
        this.winStreak = winStreak;
		this.level = level;
    }

    public void ClearData()
    {
        playFabId = "";
        name = "";
        mmrWeekly = -1;
        mmrMonthly = -1;
        position = -1;
        winStreak = 0;
		level = 0;
    }

    /*
    public void ClearParty()
    {
        party.slot1Saveable = null;
        party.slot2Saveable = null;
        party.slot3Saveable = null;
        party.slot4Saveable = null;
        party.slot5Saveable = null;
    }
    */
}
