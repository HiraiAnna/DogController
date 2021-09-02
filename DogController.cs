using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//プレイヤー(犬)にアタッチ walk/run speed,gravityは初期設定 5/10/50
//ジャンプ必要なら付けます！

public class DogController : MonoBehaviour
{
    //-----  フィールド  -----
    Animator _animator;
    DogAttack _dogAttack;
    CharacterController _dogController;
    Vector3 _moveDirection;
    public Vector3 _input; //移動キー入力
    public float _sceneChangeDeley;

    //犬のライフ 仮置き
    int _life = 5;
    
    //死亡判定用
    bool _isDead;

    //キャッシュ
    Transform _transform;
    int _walkAnimeHash;
    int _runAnimeHash;
    int _defendAnimeHash;
    int _hitAnimeHash;
    int _dieAnimeHash;

    [SerializeField]
    private float _walkSpeed = 5.0f;
    [SerializeField]
    private float _runSpeed = 10.0f;
    [SerializeField]
    private float _gravity = 50.0f;



    //-----  プロパティ(外部メソッドで使ってもらうもの)  -----
    public int LifeProperty
    {
        set{
            this._life = value;
        }
        get
        {
            return this._life;
        }
        
    }

    //!!!!!!!!!!!!!!!!!!!!!!消す!!!!!!!!!!!!!!!!!!!!!!!!
    public int Life()
    {
        return _life;
    }

    public bool IsDead
    {
        set
        {
            this._isDead = value;
        }
        get
        {
            return this._isDead;
        }
    }

    //-----  メソッド  -----

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _dogController = GetComponent<CharacterController>();
        _dogAttack = GetComponent<DogAttack>();

        //軽量化用
        _transform = _dogController.transform;

        //アニメーション処理軽量化用
        _walkAnimeHash = Animator.StringToHash("walk");
        _runAnimeHash = Animator.StringToHash("run");
        _defendAnimeHash = Animator.StringToHash("defend");
        _hitAnimeHash = Animator.StringToHash("hit");
        _dieAnimeHash = Animator.StringToHash("die");
    }

    // Update is called once per frame
    void Update()
    {
        _input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (_dogController.isGrounded)
        {
            _moveDirection = Vector3.zero;

            bool _canMove = _input.magnitude > 0.0f 
                && !_animator.GetCurrentAnimatorStateInfo(0).IsTag("NotMove")
                && !_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
                && !_isDead;

            if (_canMove)    //キー入力がされているか+アニメーションの条件
                             //(ダメージ/攻撃モーション/死亡状態では無効)
            {
                _transform.LookAt(_transform.position + _input);  //向き変える

                //左シフトでダッシュ
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _animator.SetBool(_runAnimeHash, _canMove);
                    _moveDirection += _input.normalized * _runSpeed;
                }

                else
                {
                    //歩き
                    _animator.SetBool(_runAnimeHash, false);
                    _animator.SetBool(_walkAnimeHash, _canMove);
                    _moveDirection += _input.normalized * _walkSpeed;
                }


            }
            else
            {
                //アイドル状態にする
                _animator.SetBool(_runAnimeHash, false);
                _animator.SetBool(_walkAnimeHash, false);
            }

            //ジャンプ処理
            //if (Input.GetButton("Jump"))
            //{
            //    _moveDirection.y = _runSpeed;
            //}
        }

        

        //左クリックで攻撃
        if (Input.GetMouseButtonDown(0))
        {
            _dogAttack.Attack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            //右クリックしている間防御モーション ダメージ無効
            _animator.SetBool(_defendAnimeHash, Input.GetMouseButtonDown(1));
        }
        else if(Input.GetMouseButtonUp(1))
        {
            _animator.SetBool(_defendAnimeHash, false);
        }

        //重力
        _moveDirection.y = _moveDirection.y - (_gravity * Time.deltaTime);
        //移動処理
        _dogController.Move(_moveDirection * Time.deltaTime);


        if (_isDead)
        {
            //enabled = false;
            //フェードアウトしてゲームオーバーのシーンへ移行
            FadeManager.Instance.LoadScene("GameOver", _sceneChangeDeley);
        }

    }

    private void OnCollisionEnter(Collision col)
    {
        bool _hitEnemy = col.gameObject.CompareTag("Bat") || col.gameObject.CompareTag("Spider") 
            || col.gameObject.CompareTag("Skeleton") || col.gameObject.CompareTag("Slime")
            || col.gameObject.CompareTag("TurtleShell");
        bool _defendOrHit = _animator.GetCurrentAnimatorStateInfo(0).IsTag("NotMove");

        if (_hitEnemy && _life != 0)
        {
            if (_defendOrHit)
            {
                //防御中,hit中,死亡時は無効
                return;
            }
            _life--;

            //ダメージ
            _animator.SetTrigger(_hitAnimeHash);

        }
        if (_life == 0 && !_isDead)
        {
            //死亡アニメーション
            _animator.SetTrigger(_dieAnimeHash);

            //ゲームオーバー処理へ
            _isDead = true;
        }

        
    }

    



}
