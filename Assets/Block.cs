using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    public SpriteRenderer block_crack;
    public Sprite crack_image_0;
    public Sprite crack_image_1;
    public Sprite crack_image_2;
    public ParticleSystem particle_system;

    private Vector2 move_target;
    private float move_speed = 24f;

    public enum BLOCK_TYPE{
        WOOD, BRICK, METAL, CHEST, UNKNOWN
    }
    public BLOCK_TYPE block_type;
    public float health = 100f;
    public float health_max_wood = 100f;
    public float health_max_brick = 200f;
    public float health_max_metal = 300f;
    public bool unopened = true;
    ParticleSystem.MainModule particle_system_main;

	private void Start()
    {
        particle_system_main = particle_system.main;
        // move_target - straight up from current block position
        move_target = new Vector2(transform.position.x, 1000);
	}

    private void OnCollisionEnter2D(Collision2D c){
        if (c.gameObject.tag == TagManager.PLAYER){
            Physics2D.IgnoreLayerCollision(LayerManager.CHEST, LayerManager.PLAYER, true);
            Physics2D.IgnoreLayerCollision(LayerManager.UNKNOWN, LayerManager.PLAYER, true);
        }
        if (c.gameObject.tag == TagManager.PICK_UP_ITEM){
            Physics2D.IgnoreLayerCollision(LayerManager.CHEST, LayerManager.PICK_UP_ITEM, true);
        }
    }

	public void SetBlockType(int cell_id){
        if (MapManager.TILES_WOOD.Contains(cell_id)){
            block_type = BLOCK_TYPE.WOOD;
            health = health_max_wood;
        }
        else if (MapManager.TILES_BRICK.Contains(cell_id)){
            block_type = BLOCK_TYPE.BRICK;
            health = health_max_brick;
        }
        else if (MapManager.TILES_METAL.Contains(cell_id)){
            block_type = BLOCK_TYPE.METAL;
            health = health_max_metal;
        }
        else if (MapManager.TILES_CHEST.Contains(cell_id)){
            block_type = BLOCK_TYPE.CHEST;
            gameObject.tag = TagManager.CHEST;
            gameObject.layer = LayerManager.CHEST;
        }
        else{
            block_type = BLOCK_TYPE.UNKNOWN;
            gameObject.tag = TagManager.UNKNOWN;
            gameObject.layer = LayerManager.UNKNOWN;
        }
        block_crack.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = MapManager.tile_sprites[cell_id];
    }

	public void Update()
	{
        UpdateCrack();
	}

    private void UpdateCrack(){
        if (block_type == BLOCK_TYPE.WOOD){
            if (health >= health_max_wood * 0.66 && health < health_max_wood)
            {
                block_crack.enabled = true;
                block_crack.sprite = crack_image_0;
            }
            else if (health >= health_max_wood * 0.33 && health < health_max_wood * 0.66)
            {
                block_crack.enabled = true;
                block_crack.sprite = crack_image_1;
            }
            else if (health > 0 && health < health_max_wood * 0.33)
            {
                block_crack.enabled = true;
                block_crack.sprite = crack_image_2;
            }
            else if (health <= 0){
                GetComponent<BoxCollider2D>().enabled = false;
                transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                transform.position = Vector2.MoveTowards(transform.position, move_target, move_speed * Time.deltaTime);

                if (transform.position.y > 100){
                    Destroy(gameObject);
                }
            }
        }
    }

    public void KeepOpeningChest(){
        particle_system_main.startColor = Color.yellow;
        if (!particle_system.isPlaying){
            particle_system.Play();
        }
    }

    public void OpenChest(){
        unopened = false;
        StopOpeningChest();
        // play chest animation
        GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().Play("chest_open");
    }

    public void StopOpeningChest(){
        particle_system.Stop();
        GetComponent<Animator>().enabled = false;
    }

    public void Dig(){
        particle_system_main.startColor = RGBToColour(82, 48, 35);
        if (!particle_system.isPlaying){
            particle_system.Play();
        }
    }

    public void StopDig(){
        particle_system.Stop();
    }

    private Color RGBToColour(int r, int g, int b){
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }
}