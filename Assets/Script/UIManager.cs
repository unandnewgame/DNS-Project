using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Image[] turretImage;
    public TextMeshProUGUI[] turretName;
    public TextMeshProUGUI[] turretLv;
    public TextMeshProUGUI[] turretCost;

    public TextMeshProUGUI playerMoney;

    public Gate gate;
    public PlayerInventory playerInventory;
    public GameObject gateWarning;

    public BuildableObject turret;

    void Start()
    {
        turretImage[0].sprite = turret.turretSprite;
        turretName[0].text = turret.objectName;
        turretLv[0].text = "Lv : " + turret.lv.ToString();
        turretCost[0].text = "$ " + turret.turretCost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        playerMoney.text = "$ " + playerInventory.money.ToString();

        if(gate.isUnderAttack == true)
        {
            gateWarning.SetActive(true);
        }
        else
        {
            gateWarning.SetActive(false);
        }
    }
}
