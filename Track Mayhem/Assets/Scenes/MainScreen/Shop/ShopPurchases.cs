using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopPurchases : MonoBehaviour
{
    private int[] rubiesOffers = new int[]
    {
        500, 2500, 8000
    };
    private int[] tokensOffers = new int[]
   {
        1000, 10000, 40000
   };
    private int[] tokensPrice = new int[]
   {
        500, 2000, 5000
   };
    private int[] chestsPrice = new int[]
   {
        500, 2000, 5000
   };
    private int[] chestIds = new int[]
   {
        2,3,4
   };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void rubiesPurchase(int id)
    {
        if (true) //add real money
        {
            PublicData.gameData.rubies += rubiesOffers[id];
        }
    }

    public void tokensPurchase(int id)
    {
        if (PublicData.gameData.rubies >= tokensPrice[id])
        {
            PublicData.gameData.rubies -= tokensPrice[id];
            PublicData.gameData.tokens += tokensOffers[id];
        }
    }

    public void chestPurchase(int id)
    {
        if (PublicData.gameData.rubies >= tokensPrice[id])
        {
            PublicData.gameData.rubies -= tokensPrice[id];
            PublicData.currentBoxOpening = chestIds[id];
            SceneManager.LoadScene("ChestOpening");
        }
    }
}
