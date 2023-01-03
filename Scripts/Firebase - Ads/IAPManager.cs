using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour
{
    //Google play product ids
    [SerializeField]
    private string Gold1000 = "com.fortrexcolorduels.1000gold"
                , Gold10000 = "com.fortrexcolorduels.10000gold"
                , Gold50000 = "com.fortrexcolorduels.50000gold";

    [SerializeField]
    UserSO saveData;
    [SerializeField]
    Text Gold;

    private bool isLoaded;

    //Operation after payment completed
    public void OnPurchaseCompleted(Product product)
    {
        if (product.definition.id == Gold1000)
        {
            GoldPurchase(1000);
        }
        else if (product.definition.id == Gold10000)
        {
            GoldPurchase(10000);
        }
        else if (product.definition.id == Gold50000)
        {
            GoldPurchase(50000);
        }

    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase failed: " + product.definition.id + " Reason: " + reason);
    }

    private void GoldPurchase(int amount)
    {
        //Loading before buy phase for perventing cheat
        FirebaseManager.instance.LoadSO(saveData).ContinueWith(task =>
        {
            isLoaded = true;
        });

        StartCoroutine(WaitForBoolThenSave(amount));

        //Directly referred to text component because eventmanager function trigger encounters error with test purchase.
        Gold.text = saveData.Gold.ToString();
    }

    private IEnumerator WaitForBoolThenSave(int amount)
    {
        //Waiting for load ends
        yield return new WaitUntil(() => isLoaded == true);
        saveData.Gold += amount;
        //Saving new amount
        FirebaseManager.instance.SaveSO(saveData);
        isLoaded = false;
        //Store page selected for refreshing gold
        EventManager.SelectMenu(3);
    }

}
