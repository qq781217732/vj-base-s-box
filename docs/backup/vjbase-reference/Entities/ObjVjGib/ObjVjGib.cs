using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox;

public partial class ObjVjGib : BaseProjectile
{
    [Property] public bool IsStinky = false;
    [Property] public object BloodType = VJ.BLOOD_COLOR_RED;
    [Property] public string CollisionDecal = "Default";
    [Property] public int CollisionDecalChance = 3;
    [Property] public object CollisionSound = {"physics/flesh/flesh_squishy_impact_hard1.wav", "physics/flesh/flesh_squishy_impact_hard2.wav", "physics/flesh/flesh_squishy_impact_hard3.wav", "physics/flesh/flesh_squishy_impact_hard4.wav"};
    [Property] public int CollisionSoundLevel = 60;
    [Property] public object CollisionSoundPitch = VJ.SET(90, 100);
    [Property] public int NextStinkyTime = 0;
    [Property] public string Type = "anim";
    [Property] public string PrintName = "VJ Base Gib";
    [Property] public string Author = "DrVrej";
    [Property] public string Contact = "http://steamcommunity.com/groups/vrejgaming";
    [Property] public string Category = "VJ Base";
    [Property] public bool IsVJBaseCorpse = true;
    [Property] public bool IsVJBaseCorpse_Gib = true;

    public virtual void OnInitialize()
    {
        PhysicsInit(MOVETYPE_VPHYSICS);
        // SetMoveType removed: MOVETYPE_VPHYSICS  // Use MOVETYPE_NONE for testing, makes the entity freeze!
        // SetSolid removed: MOVETYPE_VPHYSICS
        if (vj_npc_gib_collision.GetInt() == 0) Collider.CollisionGroup = COLLISION_GROUP_DEBRIS 

        // Physics Functions
        var physObj = Rigidbody;
        if (physObj.IsValid())
        physObj.Wake();

        // Stinky system
        if (stinkyMatTypes[physObj.GetMaterial()])
        this.IsStinky = true;



        var hp = OBBMaxs():Distance(OBBMins());
        MaxHealth = hp;
        Health = hp;

        if (this.CollisionDecal == "Default")
        this.CollisionDecal = defDecals[this.BloodType] || false;


        // Used to correct the blood data (Ex: Eating system uses this!)
        var bloodData = this.BloodData;
        if (bloodData)
        bloodData.Decal = this.CollisionDecal;
        else;
        this.BloodData = {Decal = this.CollisionDecal}


        if (vj_npc_snd_gib.GetInt() == 0) this.CollisionSound = false 
        if (vj_npc_gib_vfx.GetInt() == 0) this.CollisionDecal = false 
    }

    public virtual void Think()
    {
        var selfData = funcGetTable(self);
        var curTime = Time.Now;

        // Stinky gib! yuck!
        if (selfData.IsStinky && selfData.NextStinkyTime < curTime)
        // SOUND_MEAT = Do NOT use this because we would need to call "GetLoudestSoundHint" twice for each sound type!
        sound.EmitHint(SOUND_CARCASS, Transform.Position, 400, 0.15, self);
        selfData.NextStinkyTime = curTime + 2;

    }

    public virtual void PhysicsCollide(data, phys)
    {
        var selfData = funcGetTable(self);

        // Collision Sound
        var velSpeed = phys.GetVelocity():Length();
        var collideSD = Game.Random.FromList(selfData.CollisionSound);
        if (collideSD && velSpeed > 18)
        Sound.Play(collideSD, selfData.CollisionSoundLevel, Game.Random.NextInt(selfData.CollisionSoundPitch.a, selfData.CollisionSoundPitch.b, Transform.Position));


        // Collision Decal
        var collideDecal = Game.Random.FromList(selfData.CollisionDecal);
        if (collideDecal && velSpeed > 18 && !data.Entity && Game.Random.NextInt(1, selfData.CollisionDecalChance) == 1)
        var myPos = Transform.Position;
        LocalTransform.Position = myPos + Transform.Up * 4  // Because the entity is too close to the ground;
        var tr = util.TraceLine({
        start = myPos,;
        endpos = myPos - (data.HitNormal * -30),;
        filter = self;
        });
        Decals.Place(collideDecal, tr.HitPos + tr.HitNormal, tr.HitPos - tr.HitNormal);

    }

    public virtual void OnTakeDamage(dmginfo)
    {
        Rigidbody.AddVelocity(dmginfo.GetDamageForce() * 0.1);
    }

    public virtual void OnDraw()
    {
    }

}