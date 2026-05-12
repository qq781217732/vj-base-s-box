// Created by Crowbar 0.67

$modelname "vj_zombies\draggy.mdl"

$bodygroup "Body"
{
	studio "charple04_reference.smd"
}

$lod 10
{
	replacemodel "charple04_reference.smd" "charple04_reference_lod1.smd"
	nofacial
}
$lod 18
{
	replacemodel "charple04_reference.smd" "charple04_reference_lod2.smd"
	nofacial
}
$lod 35
{
	replacemodel "charple04_reference.smd" "charple04_reference_lod3.smd"
	nofacial
}
$lod 55
{
	replacemodel "charple04_reference.smd" "charple04_reference_lod4.smd"
	nofacial
}
$shadowlod
{
	replacemodel "charple04_reference.smd" "charple04_reference_lod5.smd"
	nofacial
}

$surfaceprop "flesh"

$contents "solid"

$illumposition 0 -8 52

$eyeposition 0 -8 52

$cdmaterials "models\Charple\"

$texturegroup "skinfamilies"
{
	{ "Charple4_sheet"  }
	{ "Charple4_sheet2" }
	{ "Charple4_sheet3" }
	{ "Charple4_sheet4" }
}

$attachment "eyes" "Charple4.Head" 3 4 0 rotate 0 90 90
$attachment "mouth" "Charple4.Head" 1 5 0 rotate 0 0 0

$cbox 0 0 0 0 0 0

$bbox -21.488 -16.551 17.281 18.837 15.825 69.696

$hboxset "default"
$hbox 0 "Charple4.Pelvis" -9.644 -4.917 -6.116 9.312 7.752 5.794
$hbox 0 "Charple4.Lumbar" -6.346 -3.236 -7.395 9.346 8.133 6.683
$hbox 0 "Charple4.Spine" -8.129 -3.423 -9.467 13.412 7.477 9.518
$hbox 0 "Charple4.Humerus_R" -2.937 -4.825 -1.695 13.079 4.332 7.702
$hbox 0 "Charple4.Humerus_L" -3.636 -4.825 -7.581 16.212 4.825 1.695
$hbox 0 "Charple4.Head" -3.788 -4.202 -2.934 7.714 5.356 2.934
$hbox 0 "Charple4.Femur_R" -5.65 -6.164 -3.169 20.412001 3.837 7.324
$hbox 0 "Charple4.Tibia_R" -6.296 -1.197 -1.217 13.573 2.992 1.027
$hbox 0 "Charple4.Femur_L" -5.455 -5.53 -7.324 20.000999 3.837 3.137

$definebone "Charple4.Pelvis" "" 0 1.99717 41.908962 -74.196031 89.999982 89.999982 0 0 0 0 0 0
$definebone "Charple4.Lumbar" "Charple4.Pelvis" 5.695656 -0.000002 0 0 19.304556 0 0 0 0 0 0 0
$definebone "Charple4.Spine" "Charple4.Lumbar" 4.946404 0 0 0 7.480996 0 0 0 0 0 0 0
$definebone "Charple4.Humerus_R" "Charple4.Spine" 6.47958 0.277505 -4.762589 85.536467 -131.363485 59.256672 0 0 0 0 0 0
$definebone "Charple4.Humerus_L" "Charple4.Spine" 6.479584 0.277504 4.762589 -85.536467 -131.363485 -59.256672 0 0 0 0 0 0
$definebone "Charple4.Head" "Charple4.Spine" 10.804882 -0.033414 0 0 -0.952657 0 0 0 0 0 0 0
$definebone "Charple4.Femur_R" "Charple4.Pelvis" -3.534504 -0.652041 -2.454911 7.244193 -168.588822 0 0 0 0 0 0 0
$definebone "Charple4.Tibia_R" "Charple4.Femur_R" 18.59152 0.000001 0.000001 -0.000001 10.39546 0 0 0 0 0 0 0
$definebone "Charple4.Femur_L" "Charple4.Pelvis" -3.534508 -0.652041 2.454911 -7.24419 -168.588781 0 0 0 0 0 0 0


$sequence "ragdoll" {
	"draggy_anims\ragdoll.smd"
	activity "ACT_DIERAGDOLL" 1
	fadein 0.2
	fadeout 0.2
	fps 30
}

$sequence "idle2" {
	"draggy_anims\idle2.smd"
	activity "ACT_IDLE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
}

$sequence "walk" {
	"draggy_anims\walk.smd"
	activity "ACT_WALK" 1
	fadein 0.2
	fadeout 0.2
	fps 45
	loop
	walkframe 29 LX
}

$sequence "attack1" {
	"draggy_anims\attack1.smd"
	activity "ACT_MELEE_ATTACK1" 1
	{ event 1100 1 "melee" }
	fadein 0.2
	fadeout 0.2
	fps 15
}

$collisionjoints "draggy_physics.smd"
{
	$mass 23
	$inertia 10
	$damping 0.01
	$rotdamping 1.5
	$rootbone "charple4.pelvis"


	$jointconstrain "Charple4.Femur_R" x limit -15 15 0.04
	$jointconstrain "Charple4.Femur_R" y limit -17 46 0.04
	$jointconstrain "Charple4.Femur_R" z limit -64 66 0.04

	$jointconstrain "Charple4.Tibia_R" x limit 0 0 0.04
	$jointconstrain "Charple4.Tibia_R" y limit 0 0 0.04
	$jointconstrain "Charple4.Tibia_R" z limit 0 115 0.04

	$jointconstrain "Charple4.Lumbar" x limit -23 23 0.04
	$jointconstrain "Charple4.Lumbar" y limit -21 21 0.04
	$jointconstrain "Charple4.Lumbar" z limit -21 37 0.04

	$jointconstrain "Charple4.Spine" x limit -30 30 0.04
	$jointconstrain "Charple4.Spine" y limit -28 28 0.04
	$jointconstrain "Charple4.Spine" z limit -32 28 0.04

	$jointconstrain "Charple4.Humerus_R" x limit -41 41 0.04
	$jointconstrain "Charple4.Humerus_R" y limit -84 61 0.04
	$jointconstrain "Charple4.Humerus_R" z limit -91 21 0.04

	$jointconstrain "Charple4.Head" x limit -39 39 0.04
	$jointconstrain "Charple4.Head" y limit -23 23 0.04
	$jointconstrain "Charple4.Head" z limit -15 35 0.04

	$jointconstrain "Charple4.Humerus_L" x limit -41 41 0.04
	$jointconstrain "Charple4.Humerus_L" y limit -61 84 0.04
	$jointconstrain "Charple4.Humerus_L" z limit -91 21 0.04

	$jointconstrain "Charple4.Femur_L" x limit -15 15 0.04
	$jointconstrain "Charple4.Femur_L" y limit -46 17 0.04
	$jointconstrain "Charple4.Femur_L" z limit -64 66 0.04
}
