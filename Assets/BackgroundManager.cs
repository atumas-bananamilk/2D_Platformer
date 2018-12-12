using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour {
    public SpriteRenderer background_static;
    public SpriteRenderer background_0;
    public SpriteRenderer background_1;
    public SpriteRenderer background_2;

    public static BackgroundManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void MoveBackgrounds(Vector2 current_player_pos){
        background_static.transform.position = current_player_pos;
        background_0.transform.position = new Vector2(current_player_pos.x / 2.2f, background_0.transform.position.y);
        background_1.transform.position = new Vector2(current_player_pos.x / 2.5f, background_0.transform.position.y);
        background_2.transform.position = new Vector2(current_player_pos.x / 4f, background_0.transform.position.y);
    }
}
