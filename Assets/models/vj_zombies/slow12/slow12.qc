// Created by Crowbar 0.67

$modelname "vj_zombies\slow12.mdl"

$bodygroup "studio"
{
	studio "male_01_reference_b.smd"
}


$surfaceprop "flesh"

$contents "solid"

$illumposition -6.562 2.972 31.757

$eyeposition 0 0 64

$cdmaterials "models\CZombies\"

$attachment "eyes" "ValveBiped.Bip01_Head1" 3.5 -3.98 -0.1 rotate 0 -80.1 -90
$attachment "mouth" "ValveBiped.Bip01_Head1" 0.8 -5.8 -0.15 rotate 0 -80 -90
$attachment "chest" "ValveBiped.Bip01_Spine2" 5 4 0 rotate 0 90 90
$attachment "anim_attachment_head" "ValveBiped.Bip01_Head1" 0 0 0 rotate -90 -90 0

$cbox 0 0 0 0 0 0

$bbox -6.561 -26.368 -2.684 7.857 26.342 73.343

$hboxset "default"
$hbox 1 "ValveBiped.Bip01_Head1" -1.25 -6.5 -3.19 8.25 3.5 3.31
$hbox 4 "ValveBiped.Bip01_L_UpperArm" 0 -2.75 -2.75 12.51 1.75 2.25
$hbox 4 "ValveBiped.Bip01_L_Forearm" 0 -2.19 -2.38 13 1.81 1.62
$hbox 4 "ValveBiped.Bip01_L_Hand" 0.06 -2 -1.5 4.06 1 2.5
$hbox 5 "ValveBiped.Bip01_R_UpperArm" 0 -3 -2.25 12.39 2 2.75
$hbox 5 "ValveBiped.Bip01_R_Forearm" -0.5 -2.2 -1.55 12.5 1.8 2.45
$hbox 5 "ValveBiped.Bip01_R_Hand" 0.06 -2 -2.5 4.06 1 1.5
$hbox 6 "ValveBiped.Bip01_L_Thigh" 0 -3.75 -3.25 17.85 3.75 3.25
$hbox 6 "ValveBiped.Bip01_L_Calf" 0 -3.51 -3.28 15.64 3.49 2.72
$hbox 6 "ValveBiped.Bip01_L_Foot" -1.07 -2 -2.84 5.04 5 2.16
$hbox 6 "ValveBiped.Bip01_L_Toe0" -0.5 -3 -2.19 2.5 0 2.46
$hbox 7 "ValveBiped.Bip01_R_Thigh" 0 -3.75 -3.25 17.85 3.75 3.25
$hbox 7 "ValveBiped.Bip01_R_Calf" 0 -3.51 -2.82 15.64 3.49 3.18
$hbox 7 "ValveBiped.Bip01_R_Foot" -1.06 -2.01 -2.28 5.05 4.99 2.72
$hbox 7 "ValveBiped.Bip01_R_Toe0" -0.5 -3 -2.6 2.5 0 2.1
$hbox 3 "ValveBiped.Bip01_Pelvis" -7.5 -5.5 -6 7.5 8.5 5
$hbox 2 "ValveBiped.Bip01_Spine2" -2.5 -2.5 -7 14.5 7.5 7
$skipboneinbbox

$definebone "ValveBiped.Bip01_Pelvis" "" -0.000005 -0.533615 38.566917 0 0 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine" "ValveBiped.Bip01_Pelvis" 0.000005 3.345127 -2.981901 4.944225 89.999982 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine1" "ValveBiped.Bip01_Spine" 4.01833 0 0 0 -1.675443 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine2" "ValveBiped.Bip01_Spine1" 3.518555 0.000001 0 0 5.748831 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine4" "ValveBiped.Bip01_Spine2" 8.942638 0 0 0 11.12088 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Neck1" "ValveBiped.Bip01_Spine4" 3.30727 0 0 0 22.945701 179.999841 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Head1" "ValveBiped.Bip01_Neck1" 3.593716 0.000004 0 -0.000057 23.295723 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Clavicle" "ValveBiped.Bip01_Spine4" 2.033356 1.000771 -1.93761 73.71205 168.618847 -95.596516 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_UpperArm" "ValveBiped.Bip01_R_Clavicle" 6.028141 0.000008 0 -0.462089 -33.623812 93.935243 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm" 11.692547 0 0 -0.000014 -3.459061 -0.000001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm" 11.481693 0 0.000011 6.108061 2.532072 -89.664285 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Clavicle" "ValveBiped.Bip01_Spine4" 2.033348 1.000766 1.93766 -73.71188 168.619243 91.480562 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_UpperArm" "ValveBiped.Bip01_L_Clavicle" 6.02814 0.000008 0 2.738852 -33.527934 -90.505156 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_L_UpperArm" 11.692554 0 0.000008 -0.000005 -3.459062 0.000001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm" 11.481674 0.000001 -0.000031 -6.108479 2.532072 90.136743 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Thigh" "ValveBiped.Bip01_Pelvis" -3.890452 0.000004 0.000007 2.9705 -90.299349 -89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Calf" "ValveBiped.Bip01_R_Thigh" 17.84816 0 0.000001 0 1.958771 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf" 16.525251 0 0 2.336464 -61.399999 2.212247 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Toe0" "ValveBiped.Bip01_R_Foot" 6.879448 0 0 -0.16908 -33.48211 -4.660039 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Thigh" "ValveBiped.Bip01_Pelvis" 3.890452 -0.000004 -0.000003 2.970156 -89.700792 -89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Calf" "ValveBiped.Bip01_L_Thigh" 17.848158 0 0 0 1.959517 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf" 16.525227 0 0 -2.336637 -61.400334 -1.599412 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Toe0" "ValveBiped.Bip01_L_Foot" 6.87944 0.000001 0.000001 -0.16908 -33.48211 -0.418202 0 0 0 0 0 0


$ikchain "rhand" "ValveBiped.Bip01_R_Hand" knee 0.64 0.769 0
$ikchain "lhand" "ValveBiped.Bip01_L_Hand" knee 0.607 0.795 0
$ikchain "rfoot" "ValveBiped.Bip01_R_Foot" knee 0.545 -0.838 0
$ikchain "lfoot" "ValveBiped.Bip01_L_Foot" knee 0.518 -0.855 0

$sequence "ragdoll" {
	"slow12_anims\ragdoll.smd"
	activity "ACT_DIERAGDOLL" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 103 103 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 103 103 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 103 103 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 103 103 target 3
}

$includemodel "vj_zombies/slow_main.mdl"

$collisionjoints "slow12_physics.smd"
{
	$mass 90
	$inertia 2
	$damping 0.1
	$rotdamping 0.4
	$rootbone "valvebiped.bip01_pelvis"


	$jointconstrain "ValveBiped.Bip01_L_Thigh" x limit -12 12 0.002
	$jointconstrain "ValveBiped.Bip01_L_Thigh" y limit -73 6 0.002
	$jointconstrain "ValveBiped.Bip01_L_Thigh" z limit -93 30 0.002

	$jointconstrain "ValveBiped.Bip01_L_Calf" x limit 0 0 0.002
	$jointconstrain "ValveBiped.Bip01_L_Calf" y limit 0 0 0.002
	$jointconstrain "ValveBiped.Bip01_L_Calf" z limit -8 126 0.002

	$jointconstrain "ValveBiped.Bip01_R_Thigh" x limit -12 12 0.002
	$jointconstrain "ValveBiped.Bip01_R_Thigh" y limit -8 75 0.002
	$jointconstrain "ValveBiped.Bip01_R_Thigh" z limit -97 32 0.002

	$jointconstrain "ValveBiped.Bip01_R_Calf" x limit 0 0 0.002
	$jointconstrain "ValveBiped.Bip01_R_Calf" y limit 0 0 0.002
	$jointconstrain "ValveBiped.Bip01_R_Calf" z limit -12 126 0.002

	$jointconstrain "ValveBiped.Bip01_R_Foot" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Foot" y limit -25 6 0.002
	$jointconstrain "ValveBiped.Bip01_R_Foot" z limit -15 35 0.002

	$jointconstrain "ValveBiped.Bip01_Spine2" x limit -44 44 0.002
	$jointconstrain "ValveBiped.Bip01_Spine2" y limit -39 39 0.002
	$jointconstrain "ValveBiped.Bip01_Spine2" z limit -32 50 0.002

	$jointconstrain "ValveBiped.Bip01_L_UpperArm" x limit -30 30 0.002
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" y limit -95 84 0.002
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" z limit -86 26 0.002

	$jointconstrain "ValveBiped.Bip01_L_Forearm" x limit 0 0 0.002
	$jointconstrain "ValveBiped.Bip01_L_Forearm" y limit 0 0 0.002
	$jointconstrain "ValveBiped.Bip01_L_Forearm" z limit -149 4 0.002

	$jointconstrain "ValveBiped.Bip01_R_UpperArm" x limit -39 39 0.002
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" y limit -79 95 0.002
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" z limit -93 28 0.002

	$jointconstrain "ValveBiped.Bip01_R_Forearm" x limit 0 0 0.002
	$jointconstrain "ValveBiped.Bip01_R_Forearm" y limit 0 0 0.002
	$jointconstrain "ValveBiped.Bip01_R_Forearm" z limit -149 4 0.002

	$jointconstrain "ValveBiped.Bip01_R_Hand" x limit -55 55 0.002
	$jointconstrain "ValveBiped.Bip01_R_Hand" y limit -55 55 0.002
	$jointconstrain "ValveBiped.Bip01_R_Hand" z limit -55 55 0.002

	$jointconstrain "ValveBiped.Bip01_Head1" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_Head1" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_Head1" z limit 0 0 0

	$jointconstrain "ValveBiped.Bip01_L_Hand" x limit -55 55 0.002
	$jointconstrain "ValveBiped.Bip01_L_Hand" y limit -55 55 0.002
	$jointconstrain "ValveBiped.Bip01_L_Hand" z limit -55 55 0.002

	$jointconstrain "ValveBiped.Bip01_L_Foot" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_Foot" y limit -19 19 0.002
	$jointconstrain "ValveBiped.Bip01_L_Foot" z limit -15 35 0.002
}
