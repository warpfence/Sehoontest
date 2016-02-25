using UnityEngine;
using System.Collections;

public class skidMarks : MonoBehaviour {
	public AudioClip skidSound;
	AudioSource skidAudio = null;

	public AudioClip bumpSound;
	AudioSource bumpAudio = null;
	public float bumpMinForce = 4000;				
	public float bumpMaxForse = 18000;
	float bumpMinVolume = 0.2f;
	float bumpMaxVolume = 0.6f;

	public car car1;
	
	public float minToSlip=150;
	public bool rear;
	public float skidAt=5.5f;
	public float markWidth=0.2f;
	public Material skidMaterial;
	public GameObject skidSmoke;
	public float currentfrictionValue;
	int skidding;
	Vector3[] lastPos=new Vector3[2];
	WheelHit hit;

	float force;
	float stress;

	void Start () {
		skidAudio = gameObject.AddComponent<AudioSource>();
		skidAudio.loop = true;
		skidAudio.clip = skidSound;
		skidAudio.volume = 0.25f;
		skidAudio.spatialBlend=0.8f;

		bumpAudio = gameObject.AddComponent<AudioSource>();
		bumpAudio.loop = false;
		bumpAudio.clip = bumpSound;
		bumpAudio.spatialBlend=0.8f;

	}
	

	void FixedUpdate () {
			transform.GetComponent<WheelCollider>().GetGroundHit(out hit);
			currentfrictionValue=Mathf.Abs(hit.sidewaysSlip);

			stress=force-hit.force;
			force=hit.force;

			BumpPlay (stress);

			float rpm = transform.GetComponent<WheelCollider>().rpm;

			if((skidAt<= currentfrictionValue) || (rpm<minToSlip && Input.GetAxis("Vertical")>0&& rear) && hit.collider ){
				SkidMesh();
				if(!skidAudio.isPlaying)
					skidAudio.Play();
			}
			else{
				skidding=0;
				skidSmoke.GetComponent<ParticleSystem>().enableEmission=false;
				skidAudio.Stop();
			}
		if(transform.GetComponentInParent<Rigidbody>().velocity.magnitude>5){
			skidSmoke.GetComponent<ParticleSystem>().enableEmission=true;
		}
		else{
			skidSmoke.GetComponent<ParticleSystem>().enableEmission=false;
		}
	}

	void SkidMesh(){

		GameObject mark=new GameObject("Mark");
		MeshFilter filter = mark.AddComponent<MeshFilter>();
		mark.AddComponent<MeshRenderer>();
		Mesh markMesh=new Mesh();
		Vector3[] vertices=new Vector3[4];
		int[] triangles=new int[6];
		if(skidding==0){

			vertices[0]=hit.point + Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)*new Vector3(markWidth,0.01f,0);
			vertices[0]=hit.point + Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)*new Vector3(-markWidth,0.01f,0);
			vertices[2]=hit.point + Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)*new Vector3(-markWidth,0.01f,0);
			vertices[3]=hit.point +Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)*new Vector3(markWidth,0.01f,0);
			lastPos[0]=vertices[2];
			lastPos[1]=vertices[3];
			skidding=1;
		}
		else{
			vertices[1]=lastPos[0];
			vertices[0]=lastPos[1];
			vertices[2]=hit.point + Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)*new Vector3(-markWidth,0.01f,0);
			vertices[3]=hit.point +Quaternion.Euler(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z)*new Vector3(markWidth,0.01f,0);
			lastPos[0]=vertices[2];
			lastPos[1]=vertices[3];
		}

		triangles[0]=0;
		triangles[1]=1;
		triangles[2]=2;
		triangles[3]=2;
		triangles[4]=3;
		triangles[5]=0;
		markMesh.vertices=vertices;
		markMesh.triangles=triangles;
		markMesh.RecalculateNormals();
		Vector2[] uvm=new Vector2[4];
		uvm[0]=new Vector2(1,0);
		uvm[1]=new Vector2(0,0);
		uvm[2]=new Vector2(0,1);
		uvm[3]=new Vector2(1,1);
		markMesh.uv=uvm;
		filter.mesh=markMesh;
		mark.GetComponent<Renderer>().material=skidMaterial;
		mark.AddComponent<destroyAfter>();
	}

	void BumpPlay(float SuspensionStress){
		float bumpRatio=Mathf.InverseLerp(bumpMinForce,bumpMaxForse,SuspensionStress);
		if(bumpRatio>0){
			bumpAudio.volume=Mathf.Lerp (bumpMinVolume,bumpMaxVolume,bumpRatio);
			bumpAudio.pitch=bumpRatio;
			bumpAudio.PlayOneShot(bumpSound);
		} 
	}
}
