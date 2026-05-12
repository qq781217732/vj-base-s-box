// Created by Crowbar 0.67

$modelname "vj_zombies/stalker.mdl"

$bodygroup "studio"
{
	studio "stalker_old_reference.smd"
}


$surfaceprop "flesh"

$contents "solid"

$illumposition 3.074 -0.067 31.571

$eyeposition 0 0 73

$cdmaterials "models\stalker\"

$attachment "Laser" "ValveBiped.Bip01_Head1" 3 0 5 rotate 0 0 0

$cbox 0 0 0 0 0 0

$bbox -11.515 -12.667 -0.081 17.667 12.533 63.227

$hboxset "default"
$hbox 0 "ValveBiped.Bip01_Pelvis" -6.82 -4.318 -3.85 6.82 10.452 4.979
$hbox 0 "ValveBiped.Bip01_L_Thigh" -4.755 -3.271 -3.838 22.237 4.074 2.604
$hbox 0 "ValveBiped.Bip01_L_Calf" -6.03 -1.229 -1.887 22.802 3.151 2.729
$hbox 0 "ValveBiped.Bip01_R_Thigh" -4.481 -4.074 -4.324 22.237 3.271 2.604
$hbox 0 "ValveBiped.Bip01_R_Calf" -9.272 -3.151 -1.887 22.802 1.229 2.729
$hbox 0 "ValveBiped.Bip01_Spine" -5.016 -6.538 -3.201 7.614 6.538 8.867
$hbox 0 "ValveBiped.Bip01_Spine1" -2.005 -5.667 0 4.314 6.375 7.802
$hbox 0 "ValveBiped.Bip01_Spine2" -7.665 -8.415 -2.218 11.995 7.202 8.856
$hbox 0 "ValveBiped.Bip01_Spine4" -1.117 -4.488 -1.851 5.711 4.488 1.117
$hbox 0 "ValveBiped.Bip01_Neck1" -4.511 -5.052 -2.429 5.175 5.052 5.068
$hbox 0 "ValveBiped.Bip01_Head1" -4.75 -3.115 -2.621 6.069 3.107 7.029
$hbox 0 "ValveBiped.Bip01_L_Clavicle" -2.1 -3.762 -2.862 8.936 3.162 4.983
$hbox 0 "ValveBiped.Bip01_L_UpperArm" -3.51 -5.694 -3.324 15.478 1.571 1.875
$hbox 0 "ValveBiped.Bip01_L_Forearm" -3.626 -1.832 -0.45 8.363 0.768 1.937
$hbox 0 "ValveBiped.Bip01_R_Clavicle" -1.99 -3.162 -2.862 8.936 4.971 4.983
$hbox 0 "ValveBiped.Bip01_R_UpperArm" -3.666 -1.651 -4.668 15.478 5.694 1.875
$hbox 0 "ValveBiped.Bip01_R_Forearm" -3.626 -0.768 -0.45 8.363 1.832 1.937

$definebone "ValveBiped.Bip01" "" 0 0.940159 41.290905 0 0 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Pelvis" "ValveBiped.Bip01" 0 0 0 0 0 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Thigh" "ValveBiped.Bip01_Pelvis" 3.890447 0 0 -7.292892 -87.90937 -112.407271 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Calf" "ValveBiped.Bip01_L_Thigh" 18.266357 0 0 0.000001 10.483236 -0.000007 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Thigh" "ValveBiped.Bip01_Pelvis" -3.890447 0 0 -7.292893 -92.090655 112.407271 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Calf" "ValveBiped.Bip01_R_Thigh" 18.266352 0 0 0 -10.483236 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine" "ValveBiped.Bip01" 0 4.295589 -1.508128 17.297138 89.999982 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine1" "ValveBiped.Bip01_Spine" 3.736862 0 -0.000001 -5.01997 0 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine2" "ValveBiped.Bip01_Spine1" 4.313999 0 0 -15.85512 0 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine4" "ValveBiped.Bip01_Spine2" 7.293003 0 0 -13.800262 0 0 0 0 0 0 0 0
$definebone "ValveBiped.Eff" "ValveBiped.Bip01_Spine4" 3.816998 0 -0.000002 0 0 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Neck1" "ValveBiped.Eff" -0.022125 0.000373 0.000002 4.243269 0.022632 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Head1" "ValveBiped.Bip01_Neck1" 4.645 0 0 13.499973 0 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Clavicle" "ValveBiped.Bip01_Spine4" 2.761646 -1.937661 1.117149 -1.736635 -109.45889 -0.000002 0 0 0 0 0 0
$definebone "ValveBiped.LShoulderEff" "ValveBiped.Bip01_L_Clavicle" 5.958607 0.000004 0.000002 0 0 -0.000004 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_UpperArm" "ValveBiped.LShoulderEff" 0 0 0.000002 -11.766036 -41.063534 -78.269528 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_L_UpperArm" 10.499996 0 -0.000002 -0.000009 -4.883494 0.000003 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Clavicle" "ValveBiped.Bip01_Spine4" 2.761646 1.937661 1.117149 -1.736635 109.458884 0 0 0 0 0 0 0
$definebone "ValveBiped.RShoulderEff" "ValveBiped.Bip01_R_Clavicle" 5.958608 0 -0.000002 0.000001 -0.000005 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_UpperArm" "ValveBiped.RShoulderEff" 0.000001 0 -0.000002 -11.766033 41.063531 78.269528 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm" 10.499992 0 0.000002 -0.000003 4.883489 -0.000005 0 0 0 0 0 0


$sectionframes 30 125

$sequence "idle1" {
	"stalker_anims\idle1.smd"
	activity "ACT_IDLE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
}

$sequence "idle2" {
	"stalker_anims\idle2.smd"
	activity "ACT_IDLE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
}

$sequence "walk" {
	"stalker_anims\walk.smd"
	activity "ACT_WALK" 1
	{ event 1100 10 "step" }
	{ event 1100 19 "step" }
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	walkframe 19 LX LY
}

$sequence "run" {
	"stalker_anims\run.smd"
	activity "ACT_RUN" 1
	{ event 1100 12 "step" }
	{ event 1100 21 "step" }
	fadein 0.2
	fadeout 0.2
	fps 60
	loop
	walkframe 21 LX LY
}

$sequence "podconvulse" {
	"stalker_anims\podconvulse.smd"
	{ event 1100 101 "Metal_Barrel.ImpactHard" }
	{ event 1100 118 "Metal_Barrel.ImpactHard" }
	{ event 1100 90 "Metal_Barrel.ImpactHard" }
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "working" {
	"stalker_anims\working.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "ragdoll" {
	"stalker_anims\ragdoll.smd"
	activity "ACT_DIERAGDOLL" 1
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "rangeattack" {
	"stalker_anims\rangeattack.smd"
	activity "ACT_RANGE_ATTACK1" 1
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "work" {
	"stalker_anims\work.smd"
	activity "ACT_STALKER_WORK" 1
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "jumpback" {
	"stalker_anims\jumpback.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "attack1" {
	"stalker_anims\attack1.smd"
	activity "ACT_MELEE_ATTACK1" 1
	{ event 1100 11 "melee" }
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "attack2" {
	"stalker_anims\attack2.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "attack3" {
	"stalker_anims\attack3.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
}

$collisionjoints "stalker_physics.smd"
{
	$mass 90
	$inertia 5
	$damping 0.01
	$rotdamping 1.5
	$rootbone "valvebiped.bip01_pelvis"


	$jointconstrain "ValveBiped.Bip01_Spine2" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_Spine2" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_Spine2" z limit 0 0 0

	$jointrotdamping "ValveBiped.Bip01_Head1" 5
	$jointconstrain "ValveBiped.Bip01_Head1" x limit -20 20 0.2
	$jointconstrain "ValveBiped.Bip01_Head1" y limit -25 25 0.2
	$jointconstrain "ValveBiped.Bip01_Head1" z limit -13 30 0.2

	$jointmassbias "ValveBiped.Bip01_R_UpperArm" 2
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" y limit -45 76 0.2
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" z limit -75 75 0.2

	$jointmassbias "ValveBiped.Bip01_L_UpperArm" 2
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" y limit -76 30 0.2
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" z limit -75 75 0.2

	$jointmassbias "ValveBiped.Bip01_L_Thigh" 2
	$jointconstrain "ValveBiped.Bip01_L_Thigh" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_Thigh" y limit -20 40 0.2
	$jointconstrain "ValveBiped.Bip01_L_Thigh" z limit -360 360 0.2

	$jointmassbias "ValveBiped.Bip01_R_Thigh" 2
	$jointconstrain "ValveBiped.Bip01_R_Thigh" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Thigh" y limit -40 20 0.2
	$jointconstrain "ValveBiped.Bip01_R_Thigh" z limit -37 63 0.2
}
