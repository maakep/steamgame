using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public class PlayerMovementController : NetworkBehaviour
{
    private PlayerObjectController poController;

    private float Speed = 0.1f;
    private readonly Color[] Colours = new Color[4] { Color.cyan, Color.red, Color.green, Color.black };

    private bool Stunned = true;

    void Start()
    {
        poController = GetComponent<PlayerObjectController>();
        GetComponent<SpriteRenderer>().color = Colours[poController.PlayerIdNumber-1];

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene prev, Scene next)
    {
        if (next.name == "Game")
        {
            Stunned = false;
            transform.position = new Vector3((-4 + (poController.PlayerIdNumber*2)), 0, 0);
        }
    }

    void Update()
    {
        if (!isOwned) return;
        if (Stunned) return;

        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        var dir = new Vector3(x, y  , 0);

        transform.position += dir * Speed;
    }
}
