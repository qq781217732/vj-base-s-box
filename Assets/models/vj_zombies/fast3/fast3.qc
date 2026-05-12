// Created by Crowbar 0.67

$modelname "vj_zombies\fast3.mdl"

$bodygroup "studio"
{
	studio "Snork_ref_fast.smd"
}


$surfaceprop "flesh"

$contents "solid"

$illumposition 2.231 -4.406 26.532

$eyeposition 0 0 64

$cdmaterials "models\czombies\"

$attachment "chest" "ValveBiped.Bip01_Spine4" -4 0 3 rotate -70 0 180

$cbox 0 0 0 0 0 0

$bbox -13.396 -26.075 -2 12.196 26.751 74.318

$hboxset "default"
$hbox 0 "ValveBiped.Bip01_L_UpperArm" -4.65 -4.15 -2.87 14.95 3.12 2.13
$hbox 0 "ValveBiped.Bip01_L_Forearm" -2.5 -2.61 -1.73 12.71 1.55 2.13
$hbox 0 "ValveBiped.Bip01_L_Hand" -2.46 -1.3 -4.35 6.74 1.97 2.31
$hbox 0 "ValveBiped.Bip01_R_UpperArm" -3.37 -3.01 -2.85 14.89 3.63 2.16
$hbox 0 "ValveBiped.Bip01_R_Forearm" -2.26 -1.52 -1.69 12.99 2.87 2.16
$hbox 0 "ValveBiped.Bip01_R_Hand" -2.35 -1.44 -2.36 6.91 1.8 4.27
$hbox 0 "ValveBiped.Bip01_L_Thigh" -2 -5.33 -5.61 18 6.27 3.51
$hbox 0 "ValveBiped.Bip01_L_Calf" -3.88 -6.17 -4.63 22.02 4.23 4.24
$hbox 0 "ValveBiped.Bip01_R_Thigh" -2 -6.27 -5.61 18 5.33 3.51
$hbox 0 "ValveBiped.Bip01_R_Calf" -3.88 -4.23 -4.63 22.02 6.17 4.24
$hbox 0 "ValveBiped.Bip01_Spine2" -2 -8 -4 18 8 8

$definebone "ValveBiped.Bip01" "" -0.006277 1.522119 38.353989 0 -4.499953 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Pelvis" "ValveBiped.Bip01" 0 0 0 0 0 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Thigh" "ValveBiped.Bip01_Pelvis" 3.890449 0 0 -3.134709 -83.19307 -89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Calf" "ValveBiped.Bip01_L_Thigh" 17.591017 0 -0.000001 -0.000002 8.817995 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf" 17.699585 -0.000003 0.000001 1.263143 -62.103126 -7.021998 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Toe0" "ValveBiped.Bip01_L_Foot" 6.879451 0.000003 0.000003 -0.16908 -33.482106 -0.418202 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Thigh" "ValveBiped.Bip01_Pelvis" -3.890451 0 0 -3.134652 -96.806955 -90.000037 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Calf" "ValveBiped.Bip01_R_Thigh" 17.591002 0 0.000003 0 8.817992 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf" 17.699574 0 -0.000001 -1.263143 -62.103126 7.634775 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Toe0" "ValveBiped.Bip01_R_Foot" 6.879457 -0.000001 0.000003 -0.168965 -33.4821 -4.660095 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine" "ValveBiped.Bip01" 0 3.345131 -2.9819 3.030546 89.999982 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine1" "ValveBiped.Bip01_Spine" 4.126621 0 0 0 -6.30013 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine2" "ValveBiped.Bip01_Spine1" 3.612877 -0.000001 0 0 9.000307 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine4" "ValveBiped.Bip01_Spine2" 9.183975 0 0 0 9.938354 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Head1" "ValveBiped.Bip01_Spine4" 4.149971 -1.375574 0 0 -30.564717 179.999964 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Clavicle" "ValveBiped.Bip01_Spine4" 1.663036 1.210508 2.595497 -73.711477 169.261363 90.864342 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_UpperArm" "ValveBiped.Bip01_L_Clavicle" 6.028148 0.000011 0 -3.23343 -33.943001 -89.993623 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_L_UpperArm" 12.692995 0 0.000027 0.000003 -43.708484 -0.000002 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm" 11.575965 0.000015 -0.000008 -0.152231 2.188072 66.004339 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Clavicle" "ValveBiped.Bip01_Spine4" 1.663048 1.210514 -2.595443 73.455024 168.779616 -95.441819 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_UpperArm" "ValveBiped.Bip01_R_Clavicle" 6.028148 -0.000004 0 3.847069 -33.255044 93.511751 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm" 12.692997 0 -0.000004 0.000003 -46.506639 -0.00001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm" 11.599604 0.000006 -0.000008 -6.102456 1.63843 -66.743338 0 0 0 0 0 0


$poseparameter "move_yaw" -180 180 loop 0

$ikchain "rfoot" "ValveBiped.Bip01_R_Foot" knee 0.774 -0.633 0
$ikchain "lfoot" "ValveBiped.Bip01_L_Foot" knee 0 0 0
$ikchain "rhand" "ValveBiped.Bip01_R_Hand" knee 0.759 0.651 0
$ikchain "lhand" "ValveBiped.Bip01_L_Hand" knee 0.726 0.688 0

$sequence "ragdoll" {
	"fast3_anims\ragdoll.smd"
	activity "ACT_DIERAGDOLL" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 5 5 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 5 5 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 5 5 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 5 5 target 3
}

$includemodel "vj_zombies/fast_main.mdl"

$collisionjoints "fast3_physics.smd"
{
	$mass 95
	$inertia 2
	$damping 0.01
	$rotdamping 0.4
	$rootbone "valvebiped.bip01_pelvis"


	$jointconstrain "ValveBiped.Bip01_Spine" x limit -32 32 0
	$jointconstrain "ValveBiped.Bip01_Spine" y limit -39 39 0
	$jointconstrain "ValveBiped.Bip01_Spine" z limit -15 40 0

	$jointconstrain "ValveBiped.Bip01_Spine2" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_Spine2" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_Spine2" z limit -41 41 0

	$jointconstrain "ValveBiped.Bip01_L_UpperArm" x limit -30 61 0
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" y limit -90 48 0
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" z limit -106 68 0

	$jointmassbias "ValveBiped.Bip01_L_Forearm" 2
	$jointconstrain "ValveBiped.Bip01_L_Forearm" x limit 1 1 0
	$jointconstrain "ValveBiped.Bip01_L_Forearm" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_Forearm" z limit -102 44 0

	$jointmassbias "ValveBiped.Bip01_L_Hand" 2
	$jointconstrain "ValveBiped.Bip01_L_Hand" x limit -39 39 0
	$jointconstrain "ValveBiped.Bip01_L_Hand" y limit -19 19 0
	$jointconstrain "ValveBiped.Bip01_L_Hand" z limit -64 26 0

	$jointconstrain "ValveBiped.Bip01_R_UpperArm" x limit -41 37 0
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" y limit -44 90 0
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" z limit -106 68 0

	$jointmassbias "ValveBiped.Bip01_R_Forearm" 2
	$jointconstrain "ValveBiped.Bip01_R_Forearm" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Forearm" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Forearm" z limit -41 93 0

	$jointconstrain "ValveBiped.Bip01_R_Thigh" x limit -28 30 0
	$jointconstrain "ValveBiped.Bip01_R_Thigh" y limit -37 26 0
	$jointconstrain "ValveBiped.Bip01_R_Thigh" z limit -82 44 0

	$jointmassbias "ValveBiped.Bip01_R_Calf" 2
	$jointconstrain "ValveBiped.Bip01_R_Calf" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Calf" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Calf" z limit -8 130 0

	$jointmassbias "ValveBiped.Bip01_R_Foot" 2
	$jointconstrain "ValveBiped.Bip01_R_Foot" x limit -30 30 0
	$jointconstrain "ValveBiped.Bip01_R_Foot" y limit -28 28 0
	$jointconstrain "ValveBiped.Bip01_R_Foot" z limit -30 26 0

	$jointconstrain "ValveBiped.Bip01_L_Thigh" x limit -37 30 0
	$jointconstrain "ValveBiped.Bip01_L_Thigh" y limit -30 19 0
	$jointconstrain "ValveBiped.Bip01_L_Thigh" z limit -75 39 0

	$jointmassbias "ValveBiped.Bip01_L_Calf" 2
	$jointconstrain "ValveBiped.Bip01_L_Calf" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_Calf" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_Calf" z limit -8 130 0

	$jointmassbias "ValveBiped.Bip01_L_Foot" 2
	$jointconstrain "ValveBiped.Bip01_L_Foot" x limit -10 10 0
	$jointconstrain "ValveBiped.Bip01_L_Foot" y limit -17 17 0
	$jointconstrain "ValveBiped.Bip01_L_Foot" z limit -21 39 0

	$jointmassbias "ValveBiped.Bip01_R_Hand" 2
	$jointconstrain "ValveBiped.Bip01_R_Hand" x limit -58 58 0
	$jointconstrain "ValveBiped.Bip01_R_Hand" y limit -28 28 0
	$jointconstrain "ValveBiped.Bip01_R_Hand" z limit -35 46 0
}
