using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    [SerializeField] private Player player;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private PlayerManager()
    {

    }

    public static PlayerManager getInstance()
    {
        return instance;
    }

    public Player getPlayer()
    {
        return player;
    }
}
