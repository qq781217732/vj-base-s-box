// Created by Crowbar 0.67

$modelname "vj_zombies\burnzie.mdl"

$bodygroup "Body"
{
	studio "bz_reference.smd"
}

$lod 10
{
	replacemodel "bz_reference.smd" "bz_reference_lod1.smd"
	replacebone "Bip01 L Finger12" "Bip01 L Finger11"
	replacebone "Bip01 L Finger32" "Bip01 L Finger31"
	replacebone "Bip01 R Finger12" "Bip01 R Finger11"
	nofacial
}
$lod 18
{
	replacemodel "bz_reference.smd" "bz_reference_lod2.smd"
	replacebone "Bip01 L Finger12" "Bip01 L Finger11"
	replacebone "Bip01 L Finger32" "Bip01 L Finger31"
	replacebone "Bip01 R Finger12" "Bip01 R Finger11"
	nofacial
}
$lod 35
{
	replacemodel "bz_reference.smd" "bz_reference_lod3.smd"
	replacebone "Bip01 L Finger12" "Bip01 L Finger11"
	replacebone "Bip01 L Finger32" "Bip01 L Finger31"
	replacebone "Bip01 R Finger12" "Bip01 R Finger11"
	nofacial
}
$lod 55
{
	replacemodel "bz_reference.smd" "bz_reference_lod4.smd"
	replacebone "Bip01 L Finger12" "Bip01 L Finger11"
	replacebone "Bip01 L Finger32" "Bip01 L Finger31"
	replacebone "Bip01 R Finger12" "Bip01 R Finger11"
	replacebone "Bip01 L Toe0" "Bip01 L Foot"
	nofacial
}
$shadowlod
{
	replacemodel "bz_reference.smd" "bz_reference_lod5.smd"
	replacebone "Bip01 L Finger12" "Bip01 L Finger11"
	replacebone "Bip01 L Finger32" "Bip01 L Finger31"
	replacebone "Bip01 R Finger12" "Bip01 R Finger11"
	replacebone "Bip01 L Toe0" "Bip01 L Foot"
	nofacial
}

$surfaceprop "flesh"

$contents "solid"

$illumposition 0 -13 60

$eyeposition 0 -13 60

$cdmaterials "models\Charple\"

$attachment "eyes" "Bip01 Head" 4 4 0 rotate 0 90 90

$cbox 0 0 0 0 0 0

$bbox -33.037 -6.855 -4.553 32.767 8 70.31

$hboxset "default"
$hbox 0 "Bip01 Pelvis" -6.382 -4.088 -5.404 5.696 5.345 5.469
$hbox 0 "Bip01 Spine1" -4.132 -1.062 -5.569 5.806 6.862 5.51
$hbox 0 "Bip01 Spine2" -4.354 -2.567 -6.689 11.014 6.692 6.414
$hbox 0 "Bip01 R UpperArm" -1.48 -3.724 -1.308 12.18 3.243 3.201
$hbox 0 "Bip01 R Forearm" -0.12 -1.47 -1.205 10.621 1.215 1.549
$hbox 0 "Bip01 R Hand" -0.651 -2.366 -1.574 5.591 2.198 1.252
$hbox 0 "Bip01 L UpperArm" -1.778 -3.389 -4.213 13.553 2.385 1.313
$hbox 0 "Bip01 L Forearm" -1.47 -1.378 -1.272 10.857 1.207 1.171
$hbox 0 "Bip01 L Hand" -0.651 -2.747 -1.181 5.591 2.198 1.574
$hbox 0 "Bip01 Head" -2.601 -3.817 -2.642 7.333 4.867 2.5
$hbox 0 "Bip01 R Thigh" -2.753 -3.42 -2.971 20.895 3.518 2.899
$hbox 0 "Bip01 R Calf" -2.114 -3.11 -1.93 18.853001 3.004 1.214
$hbox 0 "Bip01 R Foot" -1.675 -2.713 -1.741 8.511 3.05 1.962
$hbox 0 "Bip01 L Thigh" -2.857 -3.42 -3.796 18.934 3.518 2.963
$hbox 0 "Bip01 L Calf" -2.118 -3.159 -1.747 18.750999 2.546 1.411
$hbox 0 "Bip01 R Foot" -1.619 -2.673 -1.961 8.58 2.749 1.852

$definebone "Bip01" "" 0 0 37.641029 0.024179 -90.165047 0 0 0 0 0 0 0
$definebone "Bip01 Pelvis" "Bip01" 0 0 0.000004 -89.834521 -90.000556 0.000859 0 0 0 0 0 0
$definebone "Bip01 Spine" "Bip01 Pelvis" 3.861622 -0.004609 0.011265 -0.079584 -5.746079 -1.285603 0 0 0 0 0 0
$definebone "Bip01 Spine1" "Bip01 Spine" 5.821072 -0.004634 0.000015 -0.00338 -0.827465 -0.184034 0 0 0 0 0 0
$definebone "Bip01 Spine2" "Bip01 Spine1" 5.821003 -0.004633 0.00003 -0.009454 -1.65493 -0.368011 0 0 0 0 0 0
$definebone "Bip01 Neck" "Bip01 Spine2" 5.821896 -0.003104 0 -0.000057 13.634733 0 0 0 0 0 0 0
$definebone "Bip01 Head" "Bip01 Neck" 4.010231 0 0 -0.742324 -3.857267 -0.480769 0 0 0 0 0 0
$definebone "Bip01 L Clavicle" "Bip01 Neck" -0.536713 0.140761 1.635261 -81.339671 -128.888934 -65.043714 0 0 0 0 0 0
$definebone "Bip01 L UpperArm" "Bip01 L Clavicle" 5.228383 0 0 -3.314904 0.243794 8.351714 0 0 0 0 0 0
$definebone "Bip01 L Forearm" "Bip01 L UpperArm" 10.075052 0 0 0 -19.319566 -0.000003 0 0 0 0 0 0
$definebone "Bip01 L Hand" "Bip01 L Forearm" 10.075051 0 0 -8.29202 8.099618 -34.374197 0 0 0 0 0 0
$definebone "Bip01 L Finger0" "Bip01 L Hand" 2.444336 0.326687 -1.924511 35.686393 13.468759 62.11096 0 0 0 0 0 0
$definebone "Bip01 L Finger01" "Bip01 L Finger0" 1.049482 0.000002 0.000004 0.000007 13.368994 0.000014 0 0 0 0 0 0
$definebone "Bip01 L Finger1" "Bip01 L Hand" 3.485641 -0.001001 -1.25938 -8.715275 -45.989033 -10.202384 0 0 0 0 0 0
$definebone "Bip01 L Finger11" "Bip01 L Finger1" 1.25938 0 0.000004 -0.000015 -0.000005 -0.000011 0 0 0 0 0 0
$definebone "Bip01 L Finger12" "Bip01 L Finger11" 1.25938 0.000004 0 -0.000014 -0.000005 -0.00001 0 0 0 0 0 0
$definebone "Bip01 L Finger2" "Bip01 L Hand" 3.485641 -0.000328 -0.419792 -8.714703 -44.934449 -5.575802 0 0 0 0 0 0
$definebone "Bip01 L Finger21" "Bip01 L Finger2" 1.46928 0.000002 0 -0.000018 -0.000005 -0.000006 0 0 0 0 0 0
$definebone "Bip01 L Finger22" "Bip01 L Finger21" 1.46928 0.000002 0.000004 -0.000021 -0.000005 -0.000007 0 0 0 0 0 0
$definebone "Bip01 L Finger3" "Bip01 L Hand" 3.485645 0.000334 0.419796 -15.887326 -40.485953 -5.732967 0 0 0 0 0 0
$definebone "Bip01 L Finger31" "Bip01 L Finger3" 1.25938 0 0.000004 -0.000011 -0.000009 -0.000008 0 0 0 0 0 0
$definebone "Bip01 L Finger32" "Bip01 L Finger31" 1.259384 -0.000002 0 -0.000014 -0.000007 -0.000008 0 0 0 0 0 0
$definebone "Bip01 L Finger4" "Bip01 L Hand" 3.485645 0.001003 1.25938 -12.88806 0.428797 -5.668288 0 0 0 0 0 0
$definebone "Bip01 L Finger41" "Bip01 L Finger4" 1.049492 0 0 -0.000009 -0.000006 -0.000015 0 0 0 0 0 0
$definebone "Bip01 L Finger42" "Bip01 L Finger41" 1.049488 0 0.000004 -0.000009 -0.000007 -0.000016 0 0 0 0 0 0
$definebone "Bip01 R Clavicle" "Bip01 Neck" -0.53672 0.14077 -1.63526 81.597244 -94.627968 98.956778 0 0 0 0 0 0
$definebone "Bip01 R UpperArm" "Bip01 R Clavicle" 5.228383 -0.000001 -0.000004 2.732781 -6.846845 -28.322336 0 0 0 0 0 0
$definebone "Bip01 R Forearm" "Bip01 R UpperArm" 10.075047 -0.000002 0 0 -7.765813 0.000003 0 0 0 0 0 0
$definebone "Bip01 R Hand" "Bip01 R Forearm" 10.075048 -0.000002 0.000008 10.625794 6.05353 27.619892 0 0 0 0 0 0
$definebone "Bip01 R Finger0" "Bip01 R Hand" 2.444336 0.326687 1.924511 -35.686393 13.468753 -62.111029 0 0 0 0 0 0
$definebone "Bip01 R Finger01" "Bip01 R Finger0" 1.049486 0.000004 0 -0.000009 13.368995 0.00001 0 0 0 0 0 0
$definebone "Bip01 R Finger1" "Bip01 R Hand" 3.485638 -0.001003 1.25938 8.715265 -45.989037 10.202371 0 0 0 0 0 0
$definebone "Bip01 R Finger11" "Bip01 R Finger1" 1.259377 -0.000002 -0.000004 0.000002 -0.000005 -0.000003 0 0 0 0 0 0
$definebone "Bip01 R Finger12" "Bip01 R Finger11" 1.25938 0.000002 0 0.000004 -0.000009 -0.000004 0 0 0 0 0 0
$definebone "Bip01 R Finger2" "Bip01 R Hand" 3.485638 -0.000334 0.419796 8.714691 -44.934462 5.575793 0 0 0 0 0 0
$definebone "Bip01 R Finger21" "Bip01 R Finger2" 1.46928 0.000002 -0.000004 0.000001 -0.000022 -0.000004 0 0 0 0 0 0
$definebone "Bip01 R Finger22" "Bip01 R Finger21" 1.469284 0 0 0.000001 -0.00002 -0.000003 0 0 0 0 0 0
$definebone "Bip01 R Finger3" "Bip01 R Hand" 3.485641 0.000334 -0.419792 15.887321 -40.485957 5.732957 0 0 0 0 0 0
$definebone "Bip01 R Finger31" "Bip01 R Finger3" 1.25938 0 -0.000004 0 -0.000014 -0.000002 0 0 0 0 0 0
$definebone "Bip01 R Finger32" "Bip01 R Finger31" 1.259384 0 0.000004 0 -0.000012 -0.000003 0 0 0 0 0 0
$definebone "Bip01 R Finger4" "Bip01 R Hand" 3.485638 0.001003 -1.259384 12.888059 0.428799 5.668273 0 0 0 0 0 0
$definebone "Bip01 R Finger41" "Bip01 R Finger4" 1.049484 0 -0.000004 0.000009 -0.000003 0.000001 0 0 0 0 0 0
$definebone "Bip01 R Finger42" "Bip01 R Finger41" 1.049488 0 0 0.000009 -0.000003 0.000001 0 0 0 0 0 0
$definebone "Bip01 L Thigh" "Bip01 Spine" -3.838036 -0.457154 3.343002 -0.088214 -178.34169 176.603448 0 0 0 0 0 0
$definebone "Bip01 L Calf" "Bip01 L Thigh" 16.791765 0 0 0 -11.946457 0 0 0 0 0 0 0
$definebone "Bip01 L Foot" "Bip01 L Calf" 16.791767 0.000001 0 -0.702847 4.462824 0.021085 0 0 0 0 0 0
$definebone "Bip01 L Toe0" "Bip01 L Foot" 3.862103 6.588043 0 0 89.999982 0 0 0 0 0 0 0
$definebone "Bip01 R Thigh" "Bip01 Spine" -3.847347 -0.306467 -3.372 1.792769 -178.26291 -165.515455 0 0 0 0 0 0
$definebone "Bip01 R Calf" "Bip01 R Thigh" 16.791767 0 0 0 -12.134558 0.000002 0 0 0 0 0 0
$definebone "Bip01 R Foot" "Bip01 R Calf" 16.791767 0 0 -3.730128 5.808934 0.004814 0 0 0 0 0 0
$definebone "Bip01 R Toe0" "Bip01 R Foot" 3.862103 6.588047 0 0 89.999982 0 0 0 0 0 0 0


$sequence "ragdoll" {
	"burnzie_anims\ragdoll.smd"
	activity "ACT_DIERAGDOLL" 1
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "idle1" {
	"burnzie_anims\idle1.smd"
	activity "ACT_IDLE" 3
	fadein 0.2
	fadeout 0.2
	fps 15
	loop
}

$sequence "idle2" {
	"burnzie_anims\idle2.smd"
	activity "ACT_IDLE" 3
	fadein 0.2
	fadeout 0.2
	fps 15
	loop
}

$sequence "idle3" {
	"burnzie_anims\idle3.smd"
	activity "ACT_IDLE" 6
	fadein 0.2
	fadeout 0.2
	fps 15
	loop
}

$sequence "walk" {
	"burnzie_anims\walk.smd"
	activity "ACT_WALK" 1
	{ event 1100 7 "step" }
	{ event 1100 15 "step" }
	{ event 1100 21 "step" }
	{ event 1100 29 "step" }
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	walkframe 29 LX LY
}

$sequence "walk2" {
	"burnzie_anims\walk2.smd"
	activity "ACT_WALK" 1
	{ event 1100 7 "step" }
	{ event 1100 14 "step" }
	{ event 1100 20 "step" }
	{ event 1100 29 "step" }
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	walkframe 29 LX LY
}

$sequence "Walk_OnFire" {
	"burnzie_anims\Walk_OnFire.smd"
	activity "ACT_WALK_ON_FIRE" 1
	{ event 1100 3 "step" }
	{ event 1100 5 "step" }
	{ event 1100 9 "step" }
	fadein 0.2
	fadeout 0.2
	fps 24
	loop
	walkframe 10 LX LY
}

$sequence "Idle_OnFire" {
	"burnzie_anims\Idle_OnFire.smd"
	activity "ACT_IDLE_ON_FIRE" 1
	fadein 0.2
	fadeout 0.2
	fps 16
	loop
}

$sequence "attackleft" {
	"burnzie_anims\attackleft.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 1 "sound_attack" }
	{ event 1100 16 "melee" }
	fadein 0.2
	fadeout 0.2
	fps 25
}

$sequence "attackleft2" {
	"burnzie_anims\attackleft2.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 1 "sound_attack" }
	{ event 1100 17 "melee" }
	fadein 0.2
	fadeout 0.2
	fps 25
}

$sequence "attackright" {
	"burnzie_anims\attackright.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 1 "sound_attack" }
	{ event 1100 13 "melee" }
	fadein 0.2
	fadeout 0.2
	fps 25
}

$sequence "attackright2" {
	"burnzie_anims\attackright2.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 1 "sound_attack" }
	{ event 1100 14 "melee" }
	fadein 0.2
	fadeout 0.2
	fps 25
}

$collisionjoints "burnzie_physics.smd"
{
	$mass 30
	$inertia 10
	$damping 0.5
	$rotdamping 1.5
	$rootbone "Bip01 Pelvis"


	$jointconstrain "Bip01 R Thigh" x limit -15 15 0.008
	$jointconstrain "Bip01 R Thigh" y limit -20 27 0.008
	$jointconstrain "Bip01 R Thigh" z limit -82 30 0.008

	$jointconstrain "Bip01 R Calf" x limit 0 0 0
	$jointconstrain "Bip01 R Calf" y limit 0 0 0
	$jointconstrain "Bip01 R Calf" z limit 3 93 0.008

	$jointconstrain "Bip01 L Thigh" x limit -50 -15 0.008
	$jointconstrain "Bip01 L Thigh" y limit -35 35 0.008
	$jointconstrain "Bip01 L Thigh" z limit -61 61 0.008

	$jointconstrain "Bip01 L Calf" x limit -1 -1 0.008
	$jointconstrain "Bip01 L Calf" y limit 0 0 0.008
	$jointconstrain "Bip01 L Calf" z limit 3 106 0.008

	$jointconstrain "Bip01 L Foot" x limit 0 0 0
	$jointconstrain "Bip01 L Foot" y limit 0 0 0
	$jointconstrain "Bip01 L Foot" z limit 0 0 0

	$jointconstrain "Bip01 Spine1" x limit -15 15 0.008
	$jointconstrain "Bip01 Spine1" y limit -3 3 0.008
	$jointconstrain "Bip01 Spine1" z limit -8 8 0.008

	$jointconstrain "Bip01 Spine2" x limit -15 15 0.008
	$jointconstrain "Bip01 Spine2" y limit -10 10 0.008
	$jointconstrain "Bip01 Spine2" z limit -22 22 0.008

	$jointconstrain "Bip01 R UpperArm" x limit -48 48 0.008
	$jointconstrain "Bip01 R UpperArm" y limit -64 64 0.008
	$jointconstrain "Bip01 R UpperArm" z limit -53 53 0.008

	$jointconstrain "Bip01 L UpperArm" x limit -32 32 0.008
	$jointconstrain "Bip01 L UpperArm" y limit -43 43 0.008
	$jointconstrain "Bip01 L UpperArm" z limit -50 50 0.008

	$jointconstrain "Bip01 L Forearm" x limit -91 95 0.008
	$jointconstrain "Bip01 L Forearm" y limit 0 0 0
	$jointconstrain "Bip01 L Forearm" z limit -155 -21 0.008

	$jointconstrain "Bip01 L Hand" x limit -59 59 0.008
	$jointconstrain "Bip01 L Hand" y limit -68 68 0.008
	$jointconstrain "Bip01 L Hand" z limit -48 48 0.008

	$jointconstrain "Bip01 R Forearm" x limit -91 84 0.008
	$jointconstrain "Bip01 R Forearm" y limit 0 0 0
	$jointconstrain "Bip01 R Forearm" z limit -153 6 0.008

	$jointconstrain "Bip01 R Hand" x limit -61 48 0.008
	$jointconstrain "Bip01 R Hand" y limit -84 84 0.008
	$jointconstrain "Bip01 R Hand" z limit -66 66 0.008

	$jointconstrain "Bip01 Head" x limit -30 30 0.008
	$jointconstrain "Bip01 Head" y limit -35 35 0.008
	$jointconstrain "Bip01 Head" z limit -19 19 0.008

	$jointconstrain "Bip01 R Foot" x limit -3 3 0.008
	$jointconstrain "Bip01 R Foot" y limit -10 10 0.008
	$jointconstrain "Bip01 R Foot" z limit -39 30 0.008
}
