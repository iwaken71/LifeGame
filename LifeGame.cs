using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LifeGame : MonoBehaviour {
	
	public int Size = 30; //ライフゲームの大きさ
	int N; 
	public float interval = 0.01f; //シミュレーションスピード
	GameObject cell; // cellのオブジェクト
	int [,] board; // 0...死 1...生
	Renderer[,] _renderer; //描画担当
	bool game = false; //シミュレーション中かどうか
	float timer = 0; //タイマー変数
	Camera _camera; //カメラ
	Color deathColor,aliveColor; //生きている時の色、死んでる時の色
	
	void Awake(){
		deathColor = Color.black;
		aliveColor = Color.green;
		cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//cell.transform.localScale = Vector3.one * 0.90f;
		cell.GetComponent<Renderer> ().material.color = deathColor;
		cell.tag = "Cell";
		_camera = Camera.main;
		if (Size != null) {
			N = Size;
		} else { 
			N = 30;
		}
	}
	
	void Start () {
		board = new int[N,N];
		_renderer = new Renderer[N, N];
		//初期化
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				board [i, j] = 0;
				GameObject obj = Instantiate (cell, new Vector3 (i, 0, j), Quaternion.identity) as GameObject;
				_renderer [i, j] = obj.gameObject.GetComponent<Renderer> ();
			}
		}
		GameObject cameraObj = GameObject.FindGameObjectWithTag ("MainCamera");
		cameraObj.transform.position = new Vector3 (N/2,N,N/2);
		cameraObj.transform.eulerAngles = new Vector3 (90,0,0); //カメラ位置調整
		CreateText (); //Text生成
	}
	void Update () {
		if (game) {
			timer += Time.deltaTime;
			if (timer > interval) {
				Action (); // シミュレーション
				Draw (); //描画
				timer = 0;
			}
		} else {
			if(Input.GetMouseButton (0)){
				Click ();
			}
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			StartButton ();
		}if (Input.GetKeyDown (KeyCode.Z)) {
			SetLife ((int)(N/2),(int)(N/2));
			Draw ();
		}if (Input.GetKeyDown (KeyCode.C)) {
			Clear();
			Draw ();
		}
	}
	void Click(){
		Vector3 center = new Vector3 (Screen.width/2,Screen.height/2,0);
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 10000)) {
			if (hit.collider.tag == "Cell") {
				int x = (int)hit.collider.transform.position.x;
				int z = (int)hit.collider.transform.position.z;
				// 情報を更新
				if (board [x, z] == 0) {
					board [x, z] = 1;
				}else if(board [x, z] == 1){
					board [x, z] = 1;
				}
			}
		}
		Draw(); //描画
	}
	//描画メソッド
	void Draw(){
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				if (board [i, j] == 0) {
					_renderer [i, j].material.color = deathColor;
				}else if (board [i, j] == 1) {
					_renderer [i, j].material.color = aliveColor;
				}
			}
		}
	}
	public void StartButton(){
		game = !game;
	}
	// リセット
	public void Clear(){
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				board [i, j] = 0;
				_renderer [i, j].material.color = deathColor;
			}
		}
	}
	
	// 一回一回のシミュレーション
	void Action(){
		int[,] board2 = new int[N,N];
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				board2 [i, j] = board [i, j];
			}
		}
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				if (board [i, j] == 0) {
					int count = 0;
					for (int x = i - 1; x <= i + 1; x++) {
						for (int z = j - 1; z <= j + 1; z++) {
							if (x >= 0 && x < N && z >= 0 && z < N) {
								if (board [x, z] == 1) {
									count++;
								}
							}
						}
					}
					if (count == 3) {
						board2 [i, j] = 1;
					}
				} else if (board [i, j] == 1) {
					int count = 0;
					for (int x = i - 1; x <= i + 1; x++) {
						for (int z = j - 1; z <= j + 1; z++) {
							if (x >= 0 && x < N && z >= 0 && z < N) {
								if (board [x, z] == 1) {
									if (!(i == x && j == z)) {
										count++;
									}
								}
							}
						}
					}
					if (count <= 1) {
						board2 [i, j] = 0;
					} else if (count >= 4) {
						board2 [i, j] = 0;
					}
				}
			}
		}
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				board [i, j] = board2 [i, j];
			}
		}
	}
	void CreateText(){
		GameObject guiObj = new GameObject("GUIText");  //Hierarchy に表示される名前
		guiObj.AddComponent<GUIText>();
		guiObj.GetComponent<GUIText>().transform.Translate(0.5f, 0, 0);
		guiObj.GetComponent<GUIText>().anchor = TextAnchor.LowerCenter;
		guiObj.GetComponent<GUIText>().alignment = TextAlignment.Center;
		guiObj.GetComponent<GUIText>().fontSize = 18;
		guiObj.GetComponent<GUIText>().fontStyle = FontStyle.Normal;
		guiObj.GetComponent<GUIText>().text = "Space Key is Start or Stop";
		
		GameObject guiObj2 = new GameObject("GUIText2");  //Hierarchy に表示される名前
		guiObj2.AddComponent<GUIText>();
		guiObj2.GetComponent<GUIText>().transform.Translate(0.5f, 0.5f, 0);
		guiObj2.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
		guiObj2.GetComponent<GUIText>().alignment = TextAlignment.Center;
		guiObj2.GetComponent<GUIText>().fontSize = 36;
		guiObj2.GetComponent<GUIText>().fontStyle = FontStyle.Normal;
		guiObj2.GetComponent<GUIText>().text = "Click Here!";
		Destroy (guiObj2,3);
	}
	
	//特定パターンを生成する
	void SetLife(int x,int y){
		int[,] board2 = new int[N,N];
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				board2 [i, j] = board [i, j];
			}
		}
		
		for (int i = 0 ; i < 5; i++) {
			for (int j = 0; j < 5; j++) {
				if (i+x >= 0 && i +x< N && j+y >= 0 && j+y < N) {
					if (i == 0) {
						if (j == 1 || j == 2) {
							board2 [i + x, j + y] = 0;
						} else {
							board2 [i + x, j + y] = 1;
						}
					} else if (i == 1) {
						if (j == 1 || j == 4) {
							board2 [i + x, j + y] = 1;
						} else {
							board2 [i + x, j + y] = 0;
						}
					}else if (i == 2) {
						if (j == 3 || j == 2) {
							board2 [i + x, j + y] = 0;
						} else {
							board2 [i + x, j + y] = 1;
						}
					}else if (i == 3) {
						if (j == 2) {
							board2 [i + x, j + y] = 1;
						} else {
							board2 [i + x, j + y] = 0;
						}
					}else if (i == 4) {
						if (j == 3) {
							board2 [i + x, j + y] = 0;
						} else {
							board2 [i + x, j + y] = 1;
						}
					}
				} else {
					return;
				}
			}
		}
		for (int i = 0; i < N; i++) {
			for (int j = 0; j < N; j++) {
				board [i, j] = board2 [i, j];
			}
		}
	}
}
