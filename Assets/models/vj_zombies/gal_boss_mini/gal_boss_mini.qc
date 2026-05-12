// Created by Crowbar 0.67

$modelname "vj_zombies/gal_boss_mini.mdl"

$bodygroup "studio"
{
	studio "monkmini2_reference.smd"
}

$lod 15
{
	replacemodel "monkmini2_reference.smd" "monkmini2_reference_lod1.smd"
	replacebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm"
	replacebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.HC_Body_Bone" "ValveBiped.Bip01_Spine4"
	nofacial
}
$shadowlod
{
	replacemodel "monkmini2_reference.smd" "monkmini2_reference_lod2.smd"
	replacebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm"
	replacebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.HC_Body_Bone" "ValveBiped.Bip01_Spine4"
	nofacial
}

$surfaceprop "zombieflesh"

$contents "solid"

$illumposition 6.562 -2.972 31.757

$eyeposition 0 0 64

$cdmaterials "models\zombie_gal\2\"

$attachment "headcrab" "ValveBiped.HC_Body_Bone" -4.49 -1.32 -0.02 rotate 0 20.25 29.25
$attachment "eyes" "ValveBiped.HC_Body_Bone" 0 0 0 rotate 0 0 -50
$attachment "head" "ValveBiped.HC_Body_Bone" 0 0 0 rotate 0 0 -50
$attachment "chest" "ValveBiped.Bip01_Spine4" -3 6 0 rotate 0 60 90
$attachment "maw" "ValveBiped.Bip01_Spine2" 0 8 0 rotate 0 90 90
$attachment "Blood_Left" "ValveBiped.Bip01_L_Finger2" 0 0 0 rotate 0 0 0
$attachment "Blood_Right" "ValveBiped.Bip01_R_Finger2" 0 0 0 rotate 0 0 0

$cbox 0 0 0 0 0 0

$bbox -12.999 -18.614 -2.395 14.599 12.845 70.624

$hboxset "default"
$hbox 2 "ValveBiped.Bip01_Spine1" -8.5 -3.84 -7.12 15.5 9.84 7.73
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
$hbox 1 "ValveBiped.HC_Body_Bone" -6.59 -5 -5.76 8.5 5 6.24

$definebone "ValveBiped.Bip01_Pelvis" "" 0 -0.533612 38.566914 0 0 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine" "ValveBiped.Bip01_Pelvis" 0 3.345131 -2.981898 4.766608 89.999982 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine1" "ValveBiped.Bip01_Spine" 3.995838 0 0 0 -1.651035 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine2" "ValveBiped.Bip01_Spine1" 3.520298 0 0 0 5.57528 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine4" "ValveBiped.Bip01_Spine2" 8.921474 0 0 0 10.680678 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Neck1" "ValveBiped.Bip01_Spine4" 3.306362 0.000002 0 0 23.268327 179.999841 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Head1" "ValveBiped.Bip01_Neck1" 3.602009 -0.000015 0 -0.000057 23.206683 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Clavicle" "ValveBiped.Bip01_Spine4" 2.059532 0.977924 -1.937661 73.711859 169.030953 -95.200871 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_UpperArm" "ValveBiped.Bip01_R_Clavicle" 6.028132 0.000004 0 -0.680903 -33.620537 93.606062 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm" 11.692554 0 -0.000011 -0.000003 -3.45906 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm" 11.481705 0 -0.000027 2.217403 2.063909 -89.381017 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand" 3.881344 0.211365 -0.402026 -2.04907 -29.776746 -5.025814 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Clavicle" "ValveBiped.Bip01_Spine4" 2.059528 0.977924 1.937661 -73.711518 169.030953 91.085258 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_UpperArm" "ValveBiped.Bip01_L_Clavicle" 6.028143 -0.000004 0 2.957035 -33.511551 -90.175245 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_L_UpperArm" 11.692562 0 0 -0.000007 -3.459061 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm" 11.481684 0.000001 -0.000034 -2.21787 2.063909 89.856514 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand" 3.881371 0.214684 0.400258 1.812896 -29.791052 5.02524 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Thigh" "ValveBiped.Bip01_Pelvis" -3.890447 0 0 2.970156 -90.299178 -89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Calf" "ValveBiped.Bip01_R_Thigh" 17.848164 0 0 0 1.959516 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf" 16.525215 -0.000002 0 2.33658 -61.400389 2.212133 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Toe0" "ValveBiped.Bip01_R_Foot" 6.879427 0.000005 0 -0.168964 -33.482106 -4.660095 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Thigh" "ValveBiped.Bip01_Pelvis" 3.890447 0 0 2.970156 -89.700792 -89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Calf" "ValveBiped.Bip01_L_Thigh" 17.848133 0 0 0 1.959517 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf" 16.525208 0 0 -2.336579 -61.400341 -1.599412 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Toe0" "ValveBiped.Bip01_L_Foot" 6.879432 0.000005 -0.000001 -0.16908 -33.48211 -0.418202 0 0 0 0 0 0
$definebone "ValveBiped.HC_Body_Bone" "ValveBiped.Bip01_Spine4" 7.858723 0.357154 2.413891 -9.085848 72.317298 154.900057 0 0 0 0 0 0


$poseparameter "move_yaw" -180 180 loop 0

$ikchain "rhand" "ValveBiped.Bip01_R_Hand" knee 0.64 0.769 0
$ikchain "lhand" "ValveBiped.Bip01_L_Hand" knee 0.607 0.795 0
$ikchain "rfoot" "ValveBiped.Bip01_R_Foot" knee 0.545 -0.838 0
$ikchain "lfoot" "ValveBiped.Bip01_L_Foot" knee 0.518 -0.855 0

$sequence "ragdoll" {
	"gal_boss_mini_anims\ragdoll.smd"
	activity "ACT_DIERAGDOLL" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 101 101 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 101 101 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 101 101 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 101 101 target 3
}

$includemodel "vj_zombies/slow_main.mdl"

$collisionjoints "gal_boss_mini_physics.smd"
{
	$mass 100
	$inertia 10
	$damping 0.1
	$rotdamping 3
	$rootbone "valvebiped.bip01_pelvis"


	$jointconstrain "ValveBiped.Bip01_L_Thigh" x limit -12 12 0.04
	$jointconstrain "ValveBiped.Bip01_L_Thigh" y limit -73 6 0.04
	$jointconstrain "ValveBiped.Bip01_L_Thigh" z limit -93 30 0.04

	$jointconstrain "ValveBiped.Bip01_L_Calf" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Calf" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Calf" z limit -8 126 0.04

	$jointconstrain "ValveBiped.Bip01_R_Thigh" x limit -12 12 0.04
	$jointconstrain "ValveBiped.Bip01_R_Thigh" y limit -8 75 0.04
	$jointconstrain "ValveBiped.Bip01_R_Thigh" z limit -97 32 0.04

	$jointconstrain "ValveBiped.Bip01_R_Calf" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Calf" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Calf" z limit -12 126 0.04

	$jointconstrain "ValveBiped.Bip01_R_Foot" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Foot" y limit -25 6 0.04
	$jointconstrain "ValveBiped.Bip01_R_Foot" z limit -15 35 0.04

	$jointconstrain "ValveBiped.Bip01_Spine2" x limit -44 44 0.04
	$jointconstrain "ValveBiped.Bip01_Spine2" y limit -39 39 0.04
	$jointconstrain "ValveBiped.Bip01_Spine2" z limit -32 50 0.04

	$jointconstrain "ValveBiped.Bip01_L_UpperArm" x limit -30 30 0.04
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" y limit -95 84 0.04
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" z limit -86 26 0.04

	$jointconstrain "ValveBiped.Bip01_L_Forearm" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Forearm" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Forearm" z limit -149 4 0.04

	$jointconstrain "ValveBiped.Bip01_R_UpperArm" x limit -39 39 0.04
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" y limit -79 95 0.04
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" z limit -93 28 0.04

	$jointconstrain "ValveBiped.Bip01_R_Forearm" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Forearm" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Forearm" z limit -149 4 0.04

	$jointconstrain "ValveBiped.Bip01_R_Hand" x limit -55 55 0.04
	$jointconstrain "ValveBiped.Bip01_R_Hand" y limit -55 55 0.04
	$jointconstrain "ValveBiped.Bip01_R_Hand" z limit -55 55 0.04

	$jointconstrain "ValveBiped.Bip01_Head1" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_Head1" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_Head1" z limit 0 0 0

	$jointconstrain "ValveBiped.Bip01_L_Hand" x limit -55 55 0.04
	$jointconstrain "ValveBiped.Bip01_L_Hand" y limit -55 55 0.04
	$jointconstrain "ValveBiped.Bip01_L_Hand" z limit -55 55 0.04

	$jointconstrain "ValveBiped.Bip01_L_Foot" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_Foot" y limit -19 19 0.04
	$jointconstrain "ValveBiped.Bip01_L_Foot" z limit -15 35 0.04
}
