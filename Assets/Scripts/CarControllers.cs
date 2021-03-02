using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CarControllers : MonoBehaviourPunCallbacks {

    public Rigidbody rb;

    private GameObject WheelMeshes, WheelColliders;

    private WheelFrictionCurve Curve;

    public float Vitesse;
    public float VitesseMax;
    public float VitesseMaxBoost;
    public float Acceleration;
    public float Deceleration;
    public float Freinage;
    public float DirectionSensor;
    public float Derapage = 10000;
    public float JumpFirstForce;
    public float JumpSecondForce;
    public float JumpSecondTorque1;
    public float JumpSecondTorque2;
    public float MaxAngleRoue = 30f;
    public float Anglederotation = 0f;
    public float MinAngleRoue;

    public float stiffnessFrontwayCourse;
    public float stiffnessSidewayCourse;
    public float stiffnessFrontwayFreinage;
    public float stiffnessSidewayFreinage;
    public float stiffnessFrontway;
    public float stiffnessSideway;

    public float Slip1Freinage;
    public float Slip2Freinage;
    public float Slip1;
    public float Slip2;

    public float Couple = 0f;
    public float Frein = 0f;
    public bool IsBraking;
    public bool IsJumpingFirst;
    public bool IsJumpingSecond;
    public bool JumpInit;

    public Vector3 Centre_de_gravite1;
    public Vector3 Centre_de_gravite2;
    public float MassGrounded;
    public float MassNotGrounded;
    public Vector3 RotationAerials;

    public ParticleSystem Particle0;
    public ParticleSystem Particle1;
    public ParticleSystem Particle2;
    public ParticleSystem Particle3;

    private WheelCollider[] Wheel_Colliders = new WheelCollider[4];
    private GameObject[] Wheel_Meshes = new GameObject[4];
    private ParticleSystem[] Wheel_Particle = new ParticleSystem[4];
    private ParticleSystem[] LoopParticle = new ParticleSystem[4];

    public bool IsBoosting;
    public GameObject Boost;
    public float BoostForce;
    public float SmokeSensor;
    public float stiffnessSideWayBoosting;
    public float BoostPadStiffnessSideway;
    public float BoostPadForce;
    public float x;
    public GameObject Sheel;
    public GameObject Front;




    void Start() {
        VehicleInitialisation();
    }

    void FixedUpdate() {
        VehicleInput();
        VehicleMove();
        VehicleDirection();
        VehicleJump();
        VehicleSettings();
        VehiculeEffect();
    }

    private void VehicleInput() {
        if (Input.GetKeyDown(KeyCode.Space))
            IsBraking = true;
        else if (Input.GetKey(KeyCode.Space))
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, x / Vitesse);
            stiffnessSidewayCourse = BoostPadStiffnessSideway;
        }
        if (Input.GetKeyUp(KeyCode.Space) || !Input.GetKey(KeyCode.Space))
            IsBraking = false;
        if (CheckWheelTag("Boost Pad"))
        {
            stiffnessSidewayCourse = BoostPadStiffnessSideway;
            rb.AddForceAtPosition(transform.forward * BoostPadForce, Boost.transform.position, ForceMode.Acceleration);
            IsBoosting = true;
        }
        if (Input.GetKey(KeyCode.X))
        {
            stiffnessSidewayCourse = stiffnessSideWayBoosting;
            rb.AddForceAtPosition(transform.forward * BoostForce, Boost.transform.position, ForceMode.Acceleration);
            IsBoosting = true;
        }
        if ((Input.GetKeyUp(KeyCode.X) || !Input.GetKey(KeyCode.X)) && !CheckWheelTag("Boost Pad"))
        {
            stiffnessSidewayCourse = 1.1f;
            IsBoosting = false;
        }
        if (Input.GetKeyUp(KeyCode.Z) && IsGrounded() && !IsJumpingFirst && !IsJumpingSecond) {
            IsJumpingFirst = true; }
        if (JumpInit && ((Input.GetKey(KeyCode.Z)) || (Input.GetKey(KeyCode.Q)) || (Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.D))) && !IsJumpingFirst && !IsJumpingSecond && (!Wheel_Colliders[0].isGrounded && !Wheel_Colliders[1].isGrounded && !Wheel_Colliders[2].isGrounded && !Wheel_Colliders[3].isGrounded)) {
            IsJumpingSecond = true; }
        else if (IsGrounded() && !IsJumpingFirst) {
            IsJumpingSecond = false;
            JumpInit = true; }
        if (Input.GetKeyDown(KeyCode.C)){
            GameObject SheelGameObject = PhotonNetwork.Instantiate(Sheel.name, Front.transform.position, transform.rotation, 0);
            Rigidbody ShellRB = SheelGameObject.transform.GetComponent<Rigidbody>();
            ShellRB.velocity = transform.TransformVector(new Vector3(0, 0, 35));
        }
    }

  private void VehicleInitialisation(){

        rb = GetComponent<Rigidbody>();

        WheelColliders = GameObject.Find("Wheel_Colliders");
        WheelMeshes = GameObject.Find("Wheel_Meshes");

        LoopParticle = new[] {Particle0, Particle1, Particle2, Particle3};

        for(int i=0; i<4; i++){
          Wheel_Colliders[i] = WheelColliders.transform.Find(i.ToString()).gameObject.GetComponent<WheelCollider>();
        }
        for(int i=0; i<4; i++){
          Wheel_Meshes[i] = WheelMeshes.transform.Find(i.ToString()).gameObject;
        }
        for(int i=0; i<4; i++){
          Wheel_Particle[i] = LoopParticle[i];
        }  
        rb.centerOfMass = Centre_de_gravite1;  
  }

  private void VehicleMove(){
   Vitesse = rb.velocity.magnitude;
    if (IsBraking){
      SetWheelCollidersVar(0f, Freinage, stiffnessFrontwayFreinage, stiffnessSidewayFreinage, Slip1Freinage, Slip2Freinage);}
    else if (Input.GetAxis("Vertical")!=0&&Vitesse<VitesseMax){
        SetWheelCollidersVar(Acceleration * Mathf.Clamp( Input.GetAxis("Vertical"), -1, 1 ), 0f, stiffnessFrontwayCourse, stiffnessSidewayCourse, 0.5f, 0.75f);}
    else if (Input.GetAxis("Vertical")==0){
        SetWheelCollidersVar(0f, Deceleration * Vitesse , stiffnessFrontwayCourse, stiffnessSidewayCourse, 0.5f, 0.75f);}
    else{
        SetWheelCollidersVar(0f , 0f, stiffnessFrontwayCourse, stiffnessSidewayCourse, 0.5f, 0.75f);}
    SetTorque(Couple, Frein);
    SetWheelColliders(Curve, stiffnessFrontway, stiffnessSideway, true); 
    if (rb.velocity.magnitude>VitesseMaxBoost && Boost)
            rb.velocity = rb.velocity.normalized * VitesseMaxBoost;
        for (int i=0; i<4; i++)
    {
            Debug.Log(WheelTag(Wheel_Colliders[i].gameObject));
    }
    }

  private void VehicleDirection(){
    if(IsBraking){ 
      Anglederotation = ((((MaxAngleRoue - DirectionSensor)/ VitesseMaxBoost) * Vitesse) + DirectionSensor) * Input.GetAxis("Horizontal");}
    else{
      Anglederotation = ((-((DirectionSensor)/ VitesseMaxBoost) * Vitesse) + DirectionSensor + MinAngleRoue) * Input.GetAxis("Horizontal");}
    
    Wheel_Colliders[0].steerAngle = Anglederotation;
    Wheel_Colliders[1].steerAngle = Anglederotation;

    /*Wheel_Meshes[0].transform.Rotate(0 ,Wheel_Colliders[0].rpm / 60 * 360 * Time.deltaTime, 0);
    Wheel_Meshes[1].transform.Rotate(0 ,-Wheel_Colliders[1].rpm / 60 * 360 * Time.deltaTime, 0);
    Wheel_Meshes[2].transform.Rotate(Wheel_Colliders[2].rpm / 60 * 360 * Time.deltaTime,0,0);
    Wheel_Meshes[3].transform.Rotate(-Wheel_Colliders[3].rpm / 60 * 360 * Time.deltaTime,0,0);
    */
    Wheel_Meshes[0].transform.localEulerAngles = new Vector3(Wheel_Meshes[0].transform.localEulerAngles.x, Wheel_Colliders[0].steerAngle - Wheel_Meshes[0].transform.localEulerAngles.z - 90, Wheel_Meshes[0].transform.localEulerAngles.z);
    Wheel_Meshes[1].transform.localEulerAngles = new Vector3(Wheel_Meshes[1].transform.localEulerAngles.x, Wheel_Colliders[1].steerAngle - Wheel_Meshes[1].transform.localEulerAngles.z + 90, Wheel_Meshes[1].transform.localEulerAngles.z);

    if(Input.GetKey(KeyCode.LeftShift)&&!IsSomeWheelGrounded()){
      gameObject.transform.Rotate(0, 0, Input.GetAxis("Horizontal")*RotationAerials.z);}
    else if(!Input.GetKey(KeyCode.LeftShift)&&!IsSomeWheelGrounded()){
      gameObject.transform.Rotate(Input.GetAxis("Vertical")*RotationAerials.x, Input.GetAxis("Horizontal")*RotationAerials.y, 0);}
  }

  private void SetWheelCollidersVar(float couple, float frein, float stiffnessfrontway, float stiffnesssideway, float slip1, float slip2){
      Couple = couple;
      Frein = frein;
      stiffnessFrontway = stiffnessfrontway;
      stiffnessSideway = stiffnesssideway;
      Slip1 = slip1;
      Slip2 = slip2;
  }

  private void VehicleJump(){
   if(!IsJumpingFirst&&!IsJumpingSecond&&!IsSomeWheelGrounded()&&!(!IsJumpingFirst&&IsJumpingSecond&&JumpInit)&&JumpInit){
     rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;}
   else{rb.constraints = RigidbodyConstraints.None;}
   if(IsJumpingFirst&&!IsJumpingSecond&&IsGrounded()){
     rb.velocity = new Vector3 (rb.velocity.x, JumpFirstForce*transform.up.y*Time.fixedDeltaTime, rb.velocity.z);
     IsJumpingFirst = false;}
  if(!IsJumpingFirst&&IsJumpingSecond&&JumpInit){
    if(Input.GetKeyDown(KeyCode.Z)){
    rb.velocity = rb.velocity + JumpSecondForce*transform.forward*Time.fixedDeltaTime;
    rb.AddTorque(JumpSecondForce*transform.right*JumpSecondTorque1);}
    if(Input.GetKeyDown(KeyCode.Q)){
    rb.velocity = rb.velocity - JumpSecondForce*transform.right*Time.fixedDeltaTime;
    rb.AddTorque(JumpSecondForce*transform.forward*JumpSecondTorque2);}
    if(Input.GetKeyDown(KeyCode.D)){
    rb.velocity = rb.velocity + JumpSecondForce*transform.right*Time.fixedDeltaTime;
    rb.AddTorque(-JumpSecondForce*transform.forward*JumpSecondTorque2);}
    if(Input.GetKeyDown(KeyCode.S)){
    rb.velocity = rb.velocity - JumpSecondForce*transform.forward*Time.fixedDeltaTime;
    rb.AddTorque(-JumpSecondForce*transform.right*JumpSecondTorque1);}
     IsJumpingSecond = false;
     JumpInit = false;}}

  private void SetWheelColliders(WheelFrictionCurve WheelFrictionCurve, float x, float y, bool state){
    float x2, y2;
    for(int i=0; i<4; i++){
      if(i<2 && state){
        x2 = 1f;
        y2 = 1f;}
      else{
        x2 = x;
        y2 = y;}  
      WheelFrictionCurve = Wheel_Colliders[i].forwardFriction;
      WheelFrictionCurve.stiffness = x2;
      WheelFrictionCurve.asymptoteValue = Slip1;
      Wheel_Colliders[i].forwardFriction = WheelFrictionCurve;
      WheelFrictionCurve = Wheel_Colliders[i].sidewaysFriction;
      WheelFrictionCurve.stiffness = y2;
      WheelFrictionCurve.asymptoteValue = Slip2;
      Wheel_Colliders[i].sidewaysFriction = WheelFrictionCurve;}
  }

  private void SetTorque(float x, float y){
    for(int i=0; i<4; i++){
      Wheel_Colliders[i].motorTorque = x;
      Wheel_Colliders[i].brakeTorque = y;}
  }

  private void VehicleSettings(){
    if(IsSomeWheelGrounded()){
      rb.mass = MassGrounded;
      rb.centerOfMass = Centre_de_gravite1;}
    else{
      rb.mass = MassNotGrounded;
      rb.centerOfMass = Centre_de_gravite2;} 
  }
  
  private void VehiculeEffect(){
        if (IsDrifting() && IsGrounded())
        {
            PlayParticules();
        }
        else
        {
            StopParticules();
        }
        /*for (int i = 0; i < 4; i++)
        {
            Vector3 pos;
            Quaternion quat;
            Wheel_Colliders[i].GetWorldPose(out pos, out quat);
            Wheel_Meshes[i].transform.position = pos;
            Wheel_Meshes[i].transform.rotation = quat;
        }*/
  }


    private string WheelTag(GameObject Wheel){
        RaycastHit hit;
        if(Physics.Raycast(Wheel.transform.position, Vector3.down, out hit))
            return hit.collider.tag;
        return "";
  }
    private bool IsDrifting(){
    return (float.IsNaN(Mathf.Acos(Vector3.Dot(rb.velocity, transform.right * SmokeSensor)) * Mathf.Rad2Deg));
  }

  public bool IsGrounded(){
    return (Wheel_Colliders[0].isGrounded && Wheel_Colliders[1].isGrounded &&Wheel_Colliders[2].isGrounded && Wheel_Colliders[3].isGrounded);
  }

  private bool IsSomeWheelGrounded(){
    return !(!Wheel_Colliders[0].isGrounded && !Wheel_Colliders[1].isGrounded &&!Wheel_Colliders[2].isGrounded && !Wheel_Colliders[3].isGrounded);
  }

  private void PlayParticules(){
    for(int i=0; i<4; i++){
      Wheel_Particle[i].Play();}
  }

  private void StopParticules(){
    for(int i=0; i<4; i++){
      Wheel_Particle[i].Stop();}
  }
  private bool CheckWheelTag(string tag)
    {
        return WheelTag(Wheel_Colliders[0].gameObject) == tag || WheelTag(Wheel_Colliders[1].gameObject) == tag || WheelTag(Wheel_Colliders[2].gameObject) == tag || WheelTag(Wheel_Colliders[3].gameObject) == tag;
    }
}