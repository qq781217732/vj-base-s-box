// Created by Crowbar 0.67

$modelname "vj_zombies/fast4.mdl"

$bodygroup "studio"
{
	studio "corpse1_reference.smd"
}

$lod 10
{
	replacemodel "corpse1_reference.smd" "corpse1_reference_lod1.smd"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.HC_BodyCube" "ValveBiped.Bip01_Spine4"
	nofacial
}
$lod 20
{
	replacemodel "corpse1_reference.smd" "corpse1_reference_lod2.smd"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.HC_BodyCube" "ValveBiped.Bip01_Spine4"
	nofacial
}
$lod 30
{
	replacemodel "corpse1_reference.smd" "corpse1_reference_lod3.smd"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.HC_BodyCube" "ValveBiped.Bip01_Spine4"
	nofacial
}
$lod 40
{
	replacemodel "corpse1_reference.smd" "corpse1_reference_lod4.smd"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.HC_BodyCube" "ValveBiped.Bip01_Spine4"
	nofacial
}
$shadowlod
{
	replacemodel "corpse1_reference.smd" "corpse1_reference_lod5.smd"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.HC_BodyCube" "ValveBiped.Bip01_Spine4"
	nofacial
}

$surfaceprop "flesh"

$contents "solid"

$illumposition 10 7 32

$eyeposition 10 7 32

$cdmaterials "models\zombie_fast\"

$texturegroup "skinfamilies"
{
	{ "corpse1" }
	{ "corpse2" }
	{ "corpse3" }
	{ "corpse4" }
}

$attachment "eyes" "ValveBiped.HC_BodyCube" -4 -8 -7 rotate 0 -90 0
$attachment "head" "ValveBiped.HC_BodyCube" 0 0 0 rotate 0 0 0
$attachment "chest" "ValveBiped.Bip01_Spine4" -4 0 3 rotate -70 0 -180
$attachment "Blood_Left" "ValveBiped.Bip01_L_Finger2" 0 0 0 rotate 0 0 0
$attachment "Blood_Right" "ValveBiped.Bip01_R_Finger2" 0 0 0 rotate 0 0 0

$cbox 0 0 0 0 0 0

$bbox -8.508 -24.802 -2.392 11.686 25.413 73.366

$hboxset "default"
$hbox 2 "ValveBiped.Bip01_Spine1" -8.5 0 -7.12 15.5 9 7.73
$hbox 4 "ValveBiped.Bip01_L_UpperArm" -1 -3.5 -2.5 11 3.5 2.5
$hbox 4 "ValveBiped.Bip01_L_Forearm" -0.5 -1.86 -1.9 10.5 3.11 2.36
$hbox 4 "ValveBiped.Bip01_L_Hand" 0 -1.78 -1 6 1.23 2
$hbox 5 "ValveBiped.Bip01_R_UpperArm" -1 -3.5 -2.5 11 3.5 2.5
$hbox 5 "ValveBiped.Bip01_R_Forearm" -0.5 -1.89 -1.6 10.5 3.09 2.66
$hbox 5 "ValveBiped.Bip01_R_Hand" 0 -1.78 -2.5 6 1.23 0.5
$hbox 6 "ValveBiped.Bip01_L_Thigh" 0 -4 -3.73 17.25 4 4.29
$hbox 6 "ValveBiped.Bip01_L_Calf" 0 -4.87 -3.54 17.25 3.18 2.63
$hbox 6 "ValveBiped.Bip01_L_Foot" -1.5 -2 -2.5 7.5 2 2.5
$hbox 7 "ValveBiped.Bip01_R_Thigh" 0 -4 -3.73 17.25 4 4.29
$hbox 7 "ValveBiped.Bip01_R_Calf" 0 -4.92 -3.12 17.25 3.25 2.65
$hbox 7 "ValveBiped.Bip01_R_Foot" -1.5 -2 -2.5 7.5 2 2.5
$hbox 1 "ValveBiped.Bip01_Spine4" 3.5 -3 -4 14.5 6 4
$skipboneinbbox

$definebone "ValveBiped.Bip01" "" -0.169411 -0.220975 38.353985 0 0 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Pelvis" "ValveBiped.Bip01" 0 0 0 0 0 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Thigh" "ValveBiped.Bip01_Pelvis" 3.890447 0 0 -3.134709 -90.106437 -89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Calf" "ValveBiped.Bip01_L_Thigh" 17.591007 0 0 0 8.817993 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf" 17.699589 0 0.000001 1.263143 -62.103133 -7.021999 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Thigh" "ValveBiped.Bip01_Pelvis" -3.890447 0 0 -3.155966 -88.601801 -90.451014 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Calf" "ValveBiped.Bip01_R_Thigh" 17.590996 0 0 2.113126 8.809569 1.391543 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf" 17.699585 -0.000001 0 -1.263143 -62.103113 7.634778 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine" "ValveBiped.Bip01" 0 3.345131 -2.981898 -8.149294 89.999982 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine1" "ValveBiped.Bip01_Spine" 4.126617 -0.000001 0 0 -6.300129 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine2" "ValveBiped.Bip01_Spine1" 3.612877 0 0 0 2.932571 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine4" "ValveBiped.Bip01_Spine2" 9.183975 0 0 0 9.938356 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Clavicle" "ValveBiped.Bip01_Spine4" 1.66304 1.21051 2.595493 -73.711764 169.261363 90.864342 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_UpperArm" "ValveBiped.Bip01_L_Clavicle" 6.028139 0.000011 0 -3.23343 -33.943001 -89.993623 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_L_UpperArm" 12.692989 0 0.000008 1.018491 -0.583844 -38.960495 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm" 11.575962 -0.000002 -0.000023 8.777083 9.381557 66.065469 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand" 5.739731 -0.095432 0.607273 1.464944 -1.052526 -175.509647 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Clavicle" "ValveBiped.Bip01_Spine4" 1.663052 1.210515 -2.595442 73.455024 168.779616 -95.441819 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_UpperArm" "ValveBiped.Bip01_R_Clavicle" 6.02814 0.000004 -0.000001 3.847066 -33.255037 93.511751 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm" 12.693001 0 0.000004 -0.618452 2.246969 41.8933 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm" 11.599609 -0.000002 0 -12.021349 7.046981 -67.220666 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand" 6.18964 0.568802 -0.554356 -0.783925 4.738871 168.977514 0 0 0 0 0 0
$definebone "ValveBiped.HC_BodyCube" "ValveBiped.Bip01_Spine4" 7.600449 -0.908442 0.636362 -89.999982 170.392033 0 0 0 0 0 0 0


$poseparameter "move_yaw" -180 180 loop 0

$ikchain "rfoot" "ValveBiped.Bip01_R_Foot" knee 0.774 -0.633 0
$ikchain "lfoot" "ValveBiped.Bip01_L_Foot" knee 0 0 0
$ikchain "rhand" "ValveBiped.Bip01_R_Hand" knee 0.759 0.651 0
$ikchain "lhand" "ValveBiped.Bip01_L_Hand" knee 0.726 0.688 0

$sequence "ragdoll" {
	"fast4_anims\ragdoll.smd"
	activity "ACT_DIERAGDOLL" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 3 3 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 3 3 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 3 3 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 3 3 target 3
}

$includemodel "vj_zombies/fast_main.mdl"

$collisionjoints "fast4_physics.smd"
{
	$mass 90
	$inertia 10
	$damping 0.01
	$rotdamping 1.5
	$rootbone "valvebiped.bip01"


	$jointconstrain "ValveBiped.Bip01_Spine2" x limit -48 48 0.04
	$jointconstrain "ValveBiped.Bip01_Spine2" y limit -25 25 0.04
	$jointconstrain "ValveBiped.Bip01_Spine2" z limit -25 50 0.04

	$jointconstrain "ValveBiped.Bip01_L_UpperArm" x limit -30 30 0.04
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" y limit -95 84 0.04
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" z limit -86 26 0.04

	$jointconstrain "ValveBiped.Bip01_L_Forearm" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Forearm" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Forearm" z limit -149 4 0.04

	$jointconstrain "ValveBiped.Bip01_R_UpperArm" x limit -39 39 0.04
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" y limit -79 95 0.04
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" z limit -93 23 0.04

	$jointconstrain "ValveBiped.Bip01_R_Thigh" x limit -12 12 0.04
	$jointconstrain "ValveBiped.Bip01_R_Thigh" y limit -8 75 0.04
	$jointconstrain "ValveBiped.Bip01_R_Thigh" z limit -97 32 0.04

	$jointconstrain "ValveBiped.Bip01_R_Calf" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Calf" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Calf" z limit -12 126 0.04

	$jointconstrain "ValveBiped.Bip01_R_Foot" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Foot" y limit -25 6 0.04
	$jointconstrain "ValveBiped.Bip01_R_Foot" z limit -15 35 0.04

	$jointconstrain "ValveBiped.Bip01_L_Thigh" x limit -12 12 0.04
	$jointconstrain "ValveBiped.Bip01_L_Thigh" y limit -73 6 0.04
	$jointconstrain "ValveBiped.Bip01_L_Thigh" z limit -93 30 0.04

	$jointconstrain "ValveBiped.Bip01_L_Calf" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Calf" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Calf" z limit -8 126 0.04

	$jointconstrain "ValveBiped.Bip01_L_Foot" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_Foot" y limit -19 19 0.04
	$jointconstrain "ValveBiped.Bip01_L_Foot" z limit -15 35 0.04

	$jointconstrain "ValveBiped.Bip01_R_Forearm" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Forearm" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Forearm" z limit -149 4 0.04
}
