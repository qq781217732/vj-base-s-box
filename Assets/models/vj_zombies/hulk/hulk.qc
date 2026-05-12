// Created by Crowbar 0.67

$modelname "vj_zombies/hulk.mdl"

$bodygroup "studio"
{
	studio "Zombie_poison_reference.smd"
}

$lod 10
{
	replacemodel "Zombie_poison_reference.smd" "Zombie_poison_reference_lod1.smd"
	nofacial
}
$lod 20
{
	replacemodel "Zombie_poison_reference.smd" "Zombie_poison_reference_lod2.smd"
	nofacial
}
$lod 35
{
	replacemodel "Zombie_poison_reference.smd" "Zombie_poison_reference_lod3.smd"
	nofacial
}
$lod 60
{
	replacemodel "Zombie_poison_reference.smd" "Zombie_poison_reference_lod4.smd"
	nofacial
}
$shadowlod
{
	replacemodel "Zombie_poison_reference.smd" "Zombie_poison_reference_lod5.smd"
	nofacial
}

$surfaceprop "flesh"

$contents "solid"

$illumposition 16.252 -0.773 29.87

$eyeposition 0 0 64

$cdmaterials "models\zombie_poison\"

$texturegroup "skinfamilies"
{
	{ "PoisonZombie_sheet"  }
	{ "PoisonZombie_sheet2" }
	{ "PoisonZombie_sheet3" }
	{ "PoisonZombie_sheet4" }
}

$attachment "eyes" "ValveBiped.Bip01_Spine4" 10.5 0 0 rotate -20 0 180
$attachment "head" "ValveBiped.Bip01_Spine4" 15 0 0 rotate 0 0 180
$attachment "chest" "ValveBiped.Bip01_Spine4" -9 0 6 rotate 0 0 0
$attachment "Blood_Left" "ValveBiped.Bip01_L_Finger2" 0 0 0 rotate 0 0 0
$attachment "Blood_Right" "ValveBiped.Bip01_R_Hand" 9.29 -0.99 0.65 rotate 1.68 23.4 11.7

$cbox 0 0 0 0 0 0

$bbox -19.652 -31.255 -8.499 31.825 31.827 77.397

$hboxset "default"
$hbox 1 "ValveBiped.Bip01_Spine4" -1.5 4.5 4.5 10.5 -4.5 -6
$hbox 0 "ValveBiped.Bip01_Spine2" -15 -15 -4.5 18.224998 9 12
$hbox 4 "ValveBiped.Bip01_L_UpperArm" 0 -5.37 -6.075 22.17 5.115 5.295
$hbox 4 "ValveBiped.Bip01_L_Forearm" -7.35 -6.15 -3.705 19.305 3.255 5.295
$hbox 4 "ValveBiped.Bip01_L_Hand" 0 -1.77 -5.205 9.045 2.565 3.525
$hbox 5 "ValveBiped.Bip01_R_UpperArm" -0.315 -4.305 -4.98 22.17 6 4.74
$hbox 5 "ValveBiped.Bip01_R_Forearm" -3.36 -3.21 -3.705 20.969999 5.235 5.22
$hbox 5 "ValveBiped.Bip01_R_Hand" -0.735 -2.865 -3.09 9.78 2.385 5.085
$hbox 6 "ValveBiped.Bip01_L_Thigh" -10.275 -9.09 -10.62 32.34 10.725 7.755
$hbox 6 "ValveBiped.Bip01_L_Calf" -4.32 -10.38 -6.915 32.264999 5.85 6.21
$hbox 6 "ValveBiped.Bip01_L_Foot" -5.88 -6.3 -5.175 14.190001 6.135 5.115
$hbox 7 "ValveBiped.Bip01_R_Thigh" -9.24 -9 -8.595 32.52 8.445 9.72
$hbox 7 "ValveBiped.Bip01_R_Calf" -6.36 -6.255 -7.53 32.939999 9.165 6.27
$hbox 7 "ValveBiped.Bip01_R_Foot" -1.65 -6.405 -4.08 14.07 6.03 5.205
$skipboneinbbox

$definebone "ValveBiped.Bip01" "" -3.606493 0.942271 36.701271 0 -6.867759 112.499978 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine" "ValveBiped.Bip01" 0.000005 3.345131 -2.981986 -24.86952 100.440186 7.185695 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine1" "ValveBiped.Bip01_Spine" 4.126701 0 -0.000002 -17.999983 0.391157 -3.150008 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine2" "ValveBiped.Bip01_Spine1" 3.612999 0 0.000004 -17.099978 -11.602854 1.350003 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine4" "ValveBiped.Bip01_Spine2" 9.183993 0 0 -8.138226 -1.37699 -0.405137 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Clavicle" "ValveBiped.Bip01_Spine4" 1.667105 -2.621269 1.204559 -9.170313 -104.12689 9.618751 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_UpperArm" "ValveBiped.Bip01_L_Clavicle" 6.028137 0 0.000011 -49.152237 29.436114 -137.856003 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_L_UpperArm" 12.693001 -0.000006 0 -0.000001 -72.241135 -0.000017 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm" 11.575994 0.000006 0.000004 34.496349 20.094977 -78.677612 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand" 5.591135 0.133425 -1.865045 9.699966 -3.450005 11.700027 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1" 5.293002 0 0.000004 -0.358121 46.381448 -4.215564 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand" 5.739708 0.047594 -0.612881 -1.24552 20.064806 -8.376824 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2" 5.6525 -0.000001 0.000001 -0.347918 31.088794 -3.881573 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand" 5.562208 0.327349 0.486442 -20.312559 44.389492 -18.900009 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3" 5.158503 0.000002 0 -0.325434 24.804016 -4.026855 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31" 3.325998 -0.000001 0 -0.423424 80.664397 0.509081 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Clavicle" "ValveBiped.Bip01_Spine4" 1.659056 2.569642 1.216358 2.098459 91.936593 -9.486456 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_UpperArm" "ValveBiped.Bip01_R_Clavicle" 6.028138 0 -0.000004 -42.844079 -10.490278 95.35513 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm" 12.693 0.000004 -0.000001 0.000007 55.794104 0.000003 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm" 11.598286 -0.138271 0.107486 2.543252 7.504018 -36.137534 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger3" "ValveBiped.Bip01_R_Hand" 5.928186 -0.249671 -0.698117 8.982894 29.349985 22.500006 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Pelvis" "ValveBiped.Bip01" 0 0.000008 0.000002 0.000001 8.516902 -0.000002 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Thigh" "ValveBiped.Bip01_Pelvis" 3.890452 0 -0.00009 -50.60982 -86.776003 -99.569985 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Calf" "ValveBiped.Bip01_L_Thigh" 17.590996 -0.000002 0 0.000001 31.493087 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf" 17.69957 0 0 -22.330803 -49.202473 1.465739 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Toe0" "ValveBiped.Bip01_L_Foot" 6.879455 0 0 -0.169253 -33.486331 -4.918207 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Thigh" "ValveBiped.Bip01_Pelvis" -3.890443 -0.000004 -0.000088 -40.555785 -95.988145 67.054795 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Calf" "ValveBiped.Bip01_R_Thigh" 17.590994 0.000001 0 -0.000004 -41.711839 -0.000005 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf" 17.699574 0 0.000002 9.053541 79.705371 3.183748 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Toe0" "ValveBiped.Bip01_R_Foot" 6.879455 0 -0.000001 0.216646 35.689538 -5.550646 0 0 0 0 0 0


$sequence "refpose" {
	"hulk_anims\refpose.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "Idle01_fix" {
	"hulk_anims\Idle01_fix.smd"
	activity "ACT_IDLE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
}

$sequence "FireWalk" {
	"hulk_anims\FireWalk.smd"
	activity "ACT_WALK_ON_FIRE" 1
	{ event 1100 40 "step" }
	{ event 1100 11 "step" }
	fadein 0.2
	fadeout 0.2
	fps 60
	loop
	walkframe 41 LX LY
}

$sequence "walk_fix" {
	"hulk_anims\walk_fix.smd"
	activity "ACT_WALK" 1
	{ event 1100 21 "step" }
	{ event 1100 40 "step" }
	fadein 0.2
	fadeout 0.2
	fps 60
	loop
	walkframe 40 LX LY
}

$sequence "Run" {
	"hulk_anims\Run.smd"
	activity "ACT_RUN" 1
	{ event 1100 9 "step" }
	{ event 1100 17 "step" }
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	walkframe 17 LX LY
}

$sequence "melee_01" {
	"hulk_anims\melee_01.smd"
	activity "ACT_MELEE_ATTACK1" 1
	{ event 1100 5 "sound_attack" }
	{ event 1100 29 "melee" }
	fadein 0.2
	fadeout 0.2
	fps 40
	walkframe 34 LX LY LZ
}

$sequence "FlinchC" {
	"hulk_anims\FlinchC.smd"
	activity "ACT_BIG_FLINCH" 1
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "FlinchR" {
	"hulk_anims\FlinchR.smd"
	activity "ACT_FLINCH_RIGHTARM" 1
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "FlinchL" {
	"hulk_anims\FlinchL.smd"
	activity "ACT_FLINCH_LEFTARM" 1
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "ragdoll" {
	"hulk_anims\ragdoll.smd"
	activity "ACT_DIERAGDOLL" 1
	fadein 0.2
	fadeout 0.2
	fps 30
}

$collisionjoints "hulk_physics.smd"
{
	$mass 115
	$inertia 10
	$damping 0.01
	$rotdamping 1.5
	$rootbone "valvebiped.bip01_pelvis"


	$jointconstrain "ValveBiped.Bip01_R_Thigh" x limit -28 30 0.04
	$jointconstrain "ValveBiped.Bip01_R_Thigh" y limit -46 10 0.04
	$jointconstrain "ValveBiped.Bip01_R_Thigh" z limit -59 59 0.04

	$jointconstrain "ValveBiped.Bip01_R_Calf" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Calf" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Calf" z limit -97 37 0.04

	$jointconstrain "ValveBiped.Bip01_L_Thigh" x limit -37 30 0.04
	$jointconstrain "ValveBiped.Bip01_L_Thigh" y limit -64 19 0.04
	$jointconstrain "ValveBiped.Bip01_L_Thigh" z limit -50 61 0.04

	$jointconstrain "ValveBiped.Bip01_L_Calf" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Calf" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Calf" z limit -35 111 0.04

	$jointconstrain "ValveBiped.Bip01_L_Foot" x limit -10 10 0.04
	$jointconstrain "ValveBiped.Bip01_L_Foot" y limit -17 17 0.04
	$jointconstrain "ValveBiped.Bip01_L_Foot" z limit -30 32 0.04

	$jointconstrain "ValveBiped.Bip01_Spine" x limit -32 32 0.04
	$jointconstrain "ValveBiped.Bip01_Spine" y limit -32 30 0.04
	$jointconstrain "ValveBiped.Bip01_Spine" z limit -46 32 0.04

	$jointconstrain "ValveBiped.Bip01_Spine2" x limit -17 17 0.04
	$jointconstrain "ValveBiped.Bip01_Spine2" y limit -3 3 0.04
	$jointconstrain "ValveBiped.Bip01_Spine2" z limit -26 26 0.04

	$jointconstrain "ValveBiped.Bip01_R_UpperArm" x limit -41 77 0.04
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" y limit -48 70 0.04
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" z limit -61 77 0.04

	$jointconstrain "ValveBiped.Bip01_L_UpperArm" x limit -61 50 0.04
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" y limit -79 48 0.04
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" z limit -79 88 0.04

	$jointconstrain "ValveBiped.Bip01_L_Forearm" x limit 1 1 0.04
	$jointconstrain "ValveBiped.Bip01_L_Forearm" y limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_L_Forearm" z limit -73 77 0.04

	$jointmassbias "ValveBiped.Bip01_L_Hand" 0.2
	$jointconstrain "ValveBiped.Bip01_L_Hand" x limit -39 59 0.04
	$jointconstrain "ValveBiped.Bip01_L_Hand" y limit -19 19 0.04
	$jointconstrain "ValveBiped.Bip01_L_Hand" z limit -79 35 0.04

	$jointconstrain "ValveBiped.Bip01_R_Forearm" x limit 0 0 0.04
	$jointconstrain "ValveBiped.Bip01_R_Forearm" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Forearm" z limit -57 88 0.04

	$jointmassbias "ValveBiped.Bip01_R_Hand" 0.2
	$jointconstrain "ValveBiped.Bip01_R_Hand" x limit -44 12 0.04
	$jointconstrain "ValveBiped.Bip01_R_Hand" y limit -28 28 0.04
	$jointconstrain "ValveBiped.Bip01_R_Hand" z limit -55 59 0.04

	$jointconstrain "ValveBiped.Bip01_R_Foot" x limit -30 30 0.04
	$jointconstrain "ValveBiped.Bip01_R_Foot" y limit -28 28 0.04
	$jointconstrain "ValveBiped.Bip01_R_Foot" z limit -30 26 0.04
}
