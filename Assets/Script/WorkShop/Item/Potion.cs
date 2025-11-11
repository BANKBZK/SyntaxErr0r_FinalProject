using UnityEngine;

public class Potion : Item
{
    public int AmountHealth = 20;
    public AudioClip drinkSound; //Here
    public override void OnCollect(Player player)
    {
        base.OnCollect(player);
        player.Heal(AmountHealth);
        SoundManager.instance.PlaySFX(drinkSound); //Here
        Destroy(gameObject);        
    }
}
