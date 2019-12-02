//** wassim ben grira ** © 2016
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float speed,defaultspeed;		
	public float jumpheight;
	public float slidespeed;
	private Rigidbody2D myrig;
	private bool grounded,slide,facingleft,check,change,freeze;
	private bool doublejump,climb,wallcoltest,jump,jumpclimb;
	public Transform end,end2,wallcheck,wallcheck2,end3;
	private Animator anim;
	private float move, countdown,countdown1;
	private PolygonCollider2D[] polycolliders; 
	private bool moveright,moveleft;
	// Use this for initialization

	void Start () {
		myrig = GetComponent<Rigidbody2D> ();
		doublejump = false;
		anim = GetComponent<Animator> ();
		slide = false;
		jump = false;
		facingleft = false;
		polycolliders = GetComponents<PolygonCollider2D>();
		polycolliders [0].enabled = true;
		defaultspeed = speed;
		check = true;
		countdown=0;
		countdown1=0.2f;
		move = speed;
		moveright = moveleft = true;
	}
	
	// Update is called once per frame
	void Update () {
		grounded = (Physics2D.Linecast (transform.position, end.position, 1 << LayerMask.NameToLayer ("ground")) || Physics2D.Linecast (transform.position, end2.position, 1 << LayerMask.NameToLayer ("ground")));
		grounded = grounded || Physics2D.Linecast(transform.position, end3.position, 1 << LayerMask.NameToLayer ("ground"));

		if (grounded || climb || jumpclimb) {
			anim.SetBool ("jump", false);
			anim.SetBool ("doublejump", false);
			jump = false;
		}

		if (grounded)
			doublejump = false;



		if (!climb && !slide ) {
			if(Input.GetKey(KeyCode.LeftArrow) && moveleft ){
				myrig.velocity = new Vector2 (-Mathf.Abs(move) , myrig.velocity.y);
				moveright=true;
				if(!slide && !climb){
				transform.localScale = new Vector3 (-Mathf.Abs (transform.localScale.x), transform.localScale.y, 1f);
				facingleft = true;
				}
			}
			if(Input.GetKey(KeyCode.RightArrow) && moveright){
				myrig.velocity = new Vector2 (Mathf.Abs(move), myrig.velocity.y);
				moveleft=true;
				if(!slide && !climb){
				transform.localScale = new Vector3 (Mathf.Abs (transform.localScale.x), transform.localScale.y, 1f);
				facingleft = false;
				}
			}
		}
		if (grounded)
			   anim.SetFloat ("movespeed", Mathf.Abs(myrig.velocity.x));

		if (!grounded && !jumpclimb)
			anim.SetFloat ("movespeed", 0);
		
		if (Input.GetKeyDown (KeyCode.UpArrow) && grounded && !slide && !climb) {
			anim.SetBool ("jump", true);
			jump = true;
		}

		if (Input.GetKeyDown (KeyCode.UpArrow) && doublejump && !slide && !climb) {
			anim.SetBool ("doublejump", true);
			jump = true;
		}

		if (Input.GetKeyDown (KeyCode.UpArrow) && (grounded || doublejump) && !slide && !climb) {
			if (grounded) {
				myrig.velocity = new Vector2 (myrig.velocity.x, jumpheight);	
			
			} else {
				myrig.velocity = new Vector2 (myrig.velocity.x, jumpheight - 1);
			}
			doublejump = !doublejump;
		}

		if (Input.GetKey(KeyCode.LeftControl) && grounded && (Mathf.Abs (myrig.velocity.x) > 1)) {
			StartCoroutine (slide1 ());
		}

		if (slide) {
			myrig.velocity = new Vector2(0,myrig.velocity.y) ;
			if (!facingleft)
				myrig.velocity = new Vector2 (slidespeed - 0.4f, myrig.velocity.y);
			moveright = moveleft = true;
			if (facingleft)
				myrig.velocity = new Vector2 (-slidespeed + 0.4f, myrig.velocity.y);
			moveright = moveleft = true;
		}

		if (!grounded) {
			if (Physics2D.Linecast (transform.position, wallcheck2.position, 1 << LayerMask.NameToLayer ("ground"))){
				goto outt; // always check for wallcheck point first not wallcheck2 ;
		    }
			wallcoltest = (Physics2D.Linecast (transform.position, wallcheck.position, 1 << LayerMask.NameToLayer ("ground")) || Physics2D.Linecast (transform.position, wallcheck2.position, 1 << LayerMask.NameToLayer ("ground")));
		outt:;
		 }

	
		if (wallcoltest && !grounded) {

			polycolliders [0].enabled = false;
			polycolliders [2].enabled = true;
			countdown1-=Time.deltaTime;
			if(countdown1 > 0)
				myrig.velocity = new Vector2 (0, 0.4f);
			else{
			countdown=countdown+(Time.deltaTime);
				myrig.velocity = new Vector2 (0,-countdown);
			}
			climb = true;
			if (facingleft) {
				transform.localScale = new Vector3 (Mathf.Abs (transform.localScale.x), transform.localScale.y, 1f);
			} else {
				transform.localScale = new Vector3 (-Mathf.Abs (transform.localScale.x), transform.localScale.y, 1f);
			}
		}
		else { 
			if(!slide){
			polycolliders [2].enabled = false;
			polycolliders [0].enabled = true;
			climb = false;
			countdown=0;
			countdown1=0.2f;
			}
		}

		anim.SetBool ("climb", climb);

		if (climb && Input.GetKey (KeyCode.LeftArrow) && Input.GetKeyDown (KeyCode.UpArrow) && !facingleft  ) {
			myrig.AddForce (new Vector2 (-1000f, 550f));
			jumpclimb = true;
			anim.SetBool ("climbjump", true);
			climb = false;
			move = 3;
			moveright = moveleft = true;
			facingleft=true;
		}

		if (climb && Input.GetKey (KeyCode.RightArrow) && Input.GetKeyDown (KeyCode.UpArrow) && facingleft ) {
			myrig.AddForce (new Vector2 (1000f, 550f));
			jumpclimb = true;
			anim.SetBool ("climbjump", true);
			climb = false;
			move = 3;
			moveright = moveleft = true;
			facingleft=false;
		}


		if (grounded && !slide ) {
			move = 2;
			jumpclimb = false;
			anim.SetBool ("climbjump", false);
			polycolliders [0].enabled=true;
		}
		if (climb) {
			jumpclimb = false;
			anim.SetBool ("climbjump", false);
		}

		if (!jumpclimb && !slide && !grounded && !climb && !jump && !doublejump) {
			anim.SetBool ("inair", true);
			polycolliders [0].enabled=false;
			polycolliders [3].enabled=true;
		}
		if (grounded && !slide) {
			anim.SetBool ("inair", false);
			polycolliders [0].enabled=true;
			polycolliders [3].enabled=false;
		}
		if (climb) {
			anim.SetBool ("inair", false);
			polycolliders [2].enabled=true;
			polycolliders [3].enabled=false;
		}


		if(jump || doublejump || climb || jumpclimb)
			moveright = moveleft = true;


}//end update


	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.layer == LayerMask.NameToLayer ("ground") && grounded) {
			//foreach (ContactPoint2D contact in coll.contacts) {
			float angle = Vector2.Angle (coll.contacts [0].normal, transform.right);

			if (angle > 140 && angle < 180){
			moveright = false;
		}
			if(angle >0 && angle <20 ){
			moveleft=false;
			}
		}
	}
	IEnumerator slide1(){
		slide = true;
		polycolliders [0].enabled = false;
		polycolliders [1].enabled = true;
		anim.SetBool ("slide", true);
		yield return new WaitForSeconds (0.7f);
		slide = false;
		polycolliders [1].enabled = false;
		polycolliders [0].enabled = true;
		anim.SetBool ("slide", false);
	}
}
