using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//UIのアイテムパネルにアタッチ ～Panel Colorにはそれぞれアイテムパネル直下の武器を設定してください
//武器持ち替え用に弓のプレハブ、剣を設定してください(剣装備スタートなので弓は非アクティブにしてください)

public class ItemChange : MonoBehaviour
{

    public enum ItemStatus
    {
        Sword,
        Bomb,
        Arrow
    }

    public ItemStatus _itemStatus;
    ItemsPanel _itemsPanel;

    //[SerializeField]
    //GameObject _bomb;
    [SerializeField]
    Image _arrowPanelColor = default;
    [SerializeField]
    Image _bombPanelColor = default;
    [SerializeField]
    Image _swordPanelColor = default;

    //武器持ち替え用
    [SerializeField]
    GameObject _bowPrefab = default;
    [SerializeField]
    GameObject _sword = default;


    // Start is called before the first frame update
    void Start()
    {
        
        //初期設定
        _itemStatus = ItemStatus.Sword;
        _itemsPanel = GetComponent<ItemsPanel>();
        
    }

    //// Update is called once per frame
    void Update()
    {
        //武器切り替え用 ストックがない場合は持ち替え不可
        if (Input.GetKeyDown("1"))
        {
            _itemStatus = ItemStatus.Sword;
            Debug.Log("アイテム切り替え : " + _itemStatus);

            
        }
        else if (Input.GetKeyDown("2") && _itemsPanel.GetArrowPossession() != 0)
        {
            _itemStatus = ItemStatus.Arrow;
            Debug.Log("アイテム切り替え : " + _itemStatus);

            
        }
        else if (Input.GetKeyDown("3") && _itemsPanel.GetBombPossession() != 0)
        {
            _itemStatus = ItemStatus.Bomb;
            Debug.Log("アイテム切り替え : " + _itemStatus);

            
        }
    }

    private void LateUpdate()
    {
        

        switch (_itemStatus)
        {
            

            case ItemStatus.Bomb:
                //パネル色変え
                _bombPanelColor.color = Color.yellow;
                _swordPanelColor.color = Color.white;
                _arrowPanelColor.color = Color.white;

                //武器持ち替え
                _sword.SetActive(false);
                _bowPrefab.SetActive(false);
                break;

            case ItemStatus.Arrow:
                //パネル色変え
                _arrowPanelColor.color = Color.yellow;
                _swordPanelColor.color = Color.white;
                _bombPanelColor.color = Color.white;

                //武器持ち替え
                _sword.SetActive(false);
                _bowPrefab.SetActive(true);
                break;

            default:
                //パネル色変え
                _swordPanelColor.color = Color.yellow;
                _bombPanelColor.color = Color.white;
                _arrowPanelColor.color = Color.white;

                //武器持ち替え
                _sword.SetActive(true);
                _bowPrefab.SetActive(false);
                break;
        }
    }

    

    
}
