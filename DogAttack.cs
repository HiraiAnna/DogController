using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//プレイヤーにアタッチする
//プレイヤーのキャラクターコントローラー、爆弾/矢/弓のプレハブ、剣を設定してください
//犬の攻撃部分
//当たり判定→攻撃アニメーション中に敵がコライダーに入る
//敵のDogAnimatorに犬ごとドラッグ&ドロップして設定してください

public class DogAttack : MonoBehaviour
{
    Animator _animator;

    //アイテム何装備してるかスクリプト
    ItemChange _itemStatusScript;

    //アイテムパネル ストック数取得用
    ItemsPanel _itemPanel = default;

    int _attack01AnimeHash;
    int _attack02AnimeHash;

    Transform _transform;

    GameObject _itemPanelObject = default;


    [SerializeField]
    CharacterController _dogController = default;

    [SerializeField]
    GameObject _bombPrefab = default;
    [SerializeField]
    GameObject _arrowPrefab = default;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();

        _transform = _dogController.transform;

        //Find関数のタグ検索
        _itemPanelObject = GameObject.FindWithTag("ItemPanel");
        
        _itemPanel = _itemPanelObject.GetComponent<ItemsPanel>();
        _itemStatusScript = _itemPanelObject.GetComponent<ItemChange>();
 
        _attack01AnimeHash = Animator.StringToHash("attack01");
        _attack02AnimeHash = Animator.StringToHash("attack02");
    }

    //左クリックで各種攻撃、残数がない場合は自動で剣攻撃
    public void Attack()
    {
        bool _bomb = _itemStatusScript._itemStatus == ItemChange.ItemStatus.Bomb
            && _bombPrefab != null;
            //&& _itemPanel.GetBombPossession() != 0;

        bool _arrow = _itemStatusScript._itemStatus == ItemChange.ItemStatus.Arrow
            && _arrowPrefab != null;
            

        if (_bomb)
        {
            Vector3 _TransformInstantiate = new Vector3(_transform.position.x, 2.0f, _transform.position.z);

            //爆弾生成
            if (Input.GetMouseButtonDown(0))
            {
                
                //いぬの現在地の頭上に生成 できれば前に出したい
                Instantiate(_bombPrefab, _TransformInstantiate, Quaternion.identity);

                //UI 爆弾/矢のストック更新
                _itemPanel.SetBombPossession(_itemPanel.GetBombPossession() - 1);

            }

            //ストックが0になったら自動で剣に持ち替える
            if (_itemPanel.GetBombPossession() == 0)
            {
                _itemStatusScript._itemStatus = ItemChange.ItemStatus.Sword;
                
            }
        }
        else if (_arrow)
        {
            Vector3 _TransformInstantiateArrow = new Vector3(_transform.position.x, 1.0f, _transform.position.z);

            //クリックで矢を射る
            if (Input.GetMouseButtonDown(0))
            {
                //前方に飛ぶ 残数を減らす
                //オブジェクトの向きをいぬの正面に合わせて変更
                Instantiate(_arrowPrefab, 
                    _TransformInstantiateArrow, 
                    _transform.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f));
                //ストック減らす
                _itemPanel.SetArrowPossession(_itemPanel.GetArrowPossession() - 1);

            }

            //ストックが0になったら自動で剣に持ち替える
            if (_itemPanel.GetArrowPossession() == 0)
            {
                _itemStatusScript._itemStatus = ItemChange.ItemStatus.Sword;
            }
        }
        else
        {
            //コライダー付けた剣を振る
            //攻撃アニメーション

            _animator.SetTrigger(_attack02AnimeHash);
            
        }
    }
}
