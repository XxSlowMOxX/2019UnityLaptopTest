using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerTeamDict : MonoBehaviour
{
    public Dictionary<Player, playerTeam> teamDictionary = new Dictionary<Player, playerTeam>();
}
