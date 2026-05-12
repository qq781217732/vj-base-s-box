// Created by Crowbar 0.67

$modelname "vj_zombies/fast_main.mdl"

$bodygroup "studio"
{
	studio "zombie_fast_fast_Zombiefast_reference.smd"
}
$bodygroup "headcrab1"
{
	blank
	studio "zombie_fast_fast_Headcrab_reference.smd"
}

$lod 8
{
	replacemodel "zombie_fast_fast_Zombiefast_reference.smd" "zombie_fast_fast_Zombiefast_reference_lod1.smd"
	replacemodel "zombie_fast_fast_Headcrab_reference.smd" "zombie_fast_fast_Headcrab_reference_lod1.smd"
	nofacial
}
$lod 20
{
	replacemodel "zombie_fast_fast_Zombiefast_reference.smd" "zombie_fast_fast_Zombiefast_reference_lod2.smd"
	replacemodel "zombie_fast_fast_Headcrab_reference.smd" "zombie_fast_fast_Headcrab_reference_lod2.smd"
	replacebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf"
	replacebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf"
	replacebone "ValveBiped.HC_BoneLB2" "ValveBiped.HC_BoneLB1"
	replacebone "ValveBiped.HC_BoneRB2" "ValveBiped.HC_BoneRB1"
	nofacial
}
$lod 40
{
	replacemodel "zombie_fast_fast_Zombiefast_reference.smd" "zombie_fast_fast_Zombiefast_reference_lod3.smd"
	replacemodel "zombie_fast_fast_Headcrab_reference.smd" "zombie_fast_fast_Headcrab_reference_lod3.smd"
	replacebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf"
	replacebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf"
	replacebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm"
	replacebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3"
	replacebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31"
	replacebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1"
	replacebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11"
	replacebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2"
	replacebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21"
	replacebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm"
	replacebone "ValveBiped.Bip01_R_Finger3" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger31" "ValveBiped.Bip01_R_Finger3"
	replacebone "ValveBiped.Bip01_R_Finger32" "ValveBiped.Bip01_R_Finger31"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger21" "ValveBiped.Bip01_R_Finger2"
	replacebone "ValveBiped.Bip01_R_Finger22" "ValveBiped.Bip01_R_Finger21"
	replacebone "ValveBiped.Bip01_R_Finger1" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger11" "ValveBiped.Bip01_R_Finger1"
	replacebone "ValveBiped.Bip01_R_Finger12" "ValveBiped.Bip01_R_Finger11"
	replacebone "ValveBiped.HC_BoneLB2" "ValveBiped.HC_BoneLB1"
	replacebone "ValveBiped.HC_BoneRB2" "ValveBiped.HC_BoneRB1"
	nofacial
}
$lod 80
{
	replacemodel "zombie_fast_fast_Zombiefast_reference.smd" "zombie_fast_fast_Zombiefast_reference_lod4.smd"
	replacemodel "zombie_fast_fast_Headcrab_reference.smd" "zombie_fast_fast_Headcrab_reference_lod4.smd"
	replacebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf"
	replacebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf"
	replacebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm"
	replacebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3"
	replacebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31"
	replacebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1"
	replacebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11"
	replacebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2"
	replacebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21"
	replacebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm"
	replacebone "ValveBiped.Bip01_R_Finger3" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger31" "ValveBiped.Bip01_R_Finger3"
	replacebone "ValveBiped.Bip01_R_Finger32" "ValveBiped.Bip01_R_Finger31"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger21" "ValveBiped.Bip01_R_Finger2"
	replacebone "ValveBiped.Bip01_R_Finger22" "ValveBiped.Bip01_R_Finger21"
	replacebone "ValveBiped.Bip01_R_Finger1" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger11" "ValveBiped.Bip01_R_Finger1"
	replacebone "ValveBiped.Bip01_R_Finger12" "ValveBiped.Bip01_R_Finger11"
	replacebone "ValveBiped.HC_BodyCube" "ValveBiped.Bip01_Spine4"
	replacebone "ValveBiped.HC_HeadCube" "ValveBiped.HC_BodyCube"
	replacebone "ValveBiped.HC_BoneLF" "ValveBiped.HC_HeadCube"
	replacebone "ValveBiped.HC_BoneLF1" "ValveBiped.HC_BoneLF"
	replacebone "ValveBiped.HC_BoneRF" "ValveBiped.HC_HeadCube"
	replacebone "ValveBiped.HC_BoneRF1" "ValveBiped.HC_BoneRF"
	replacebone "ValveBiped.HC_BoneLB" "ValveBiped.HC_BodyCube"
	replacebone "ValveBiped.HC_BoneLB1" "ValveBiped.HC_BoneLB"
	replacebone "ValveBiped.HC_BoneLB2" "ValveBiped.HC_BoneLB1"
	replacebone "ValveBiped.HC_BoneRB" "ValveBiped.HC_BodyCube"
	replacebone "ValveBiped.HC_BoneRB1" "ValveBiped.HC_BoneRB"
	replacebone "ValveBiped.HC_BoneRB2" "ValveBiped.HC_BoneRB1"
	nofacial
}
$shadowlod
{
	replacemodel "zombie_fast_fast_Zombiefast_reference.smd" "zombie_fast_fast_Zombiefast_reference_lod5.smd"
	replacemodel "zombie_fast_fast_Headcrab_reference.smd" "zombie_fast_fast_Headcrab_reference_lod5.smd"
	replacebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf"
	replacebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf"
	replacebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm"
	replacebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3"
	replacebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31"
	replacebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1"
	replacebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11"
	replacebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2"
	replacebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21"
	replacebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm"
	replacebone "ValveBiped.Bip01_R_Finger3" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger31" "ValveBiped.Bip01_R_Finger3"
	replacebone "ValveBiped.Bip01_R_Finger32" "ValveBiped.Bip01_R_Finger31"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger21" "ValveBiped.Bip01_R_Finger2"
	replacebone "ValveBiped.Bip01_R_Finger22" "ValveBiped.Bip01_R_Finger21"
	replacebone "ValveBiped.Bip01_R_Finger1" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger11" "ValveBiped.Bip01_R_Finger1"
	replacebone "ValveBiped.Bip01_R_Finger12" "ValveBiped.Bip01_R_Finger11"
	replacebone "ValveBiped.HC_BodyCube" "ValveBiped.Bip01_Spine4"
	replacebone "ValveBiped.HC_HeadCube" "ValveBiped.HC_BodyCube"
	replacebone "ValveBiped.HC_BoneLF" "ValveBiped.HC_HeadCube"
	replacebone "ValveBiped.HC_BoneLF1" "ValveBiped.HC_BoneLF"
	replacebone "ValveBiped.HC_BoneRF" "ValveBiped.HC_HeadCube"
	replacebone "ValveBiped.HC_BoneRF1" "ValveBiped.HC_BoneRF"
	replacebone "ValveBiped.HC_BoneLB" "ValveBiped.HC_BodyCube"
	replacebone "ValveBiped.HC_BoneLB1" "ValveBiped.HC_BoneLB"
	replacebone "ValveBiped.HC_BoneLB2" "ValveBiped.HC_BoneLB1"
	replacebone "ValveBiped.HC_BoneRB" "ValveBiped.HC_BodyCube"
	replacebone "ValveBiped.HC_BoneRB1" "ValveBiped.HC_BoneRB"
	replacebone "ValveBiped.HC_BoneRB2" "ValveBiped.HC_BoneRB1"
	nofacial
}

$surfaceprop "flesh"

$contents "solid"

$illumposition 2.144 -2.626 26.526

$eyeposition 0 0 64

$cdmaterials "models\zombie_fast\"
$cdmaterials "models\headcrab\"

$attachment "headcrab" "ValveBiped.HC_BodyCube" 0 -1.5 0 rotate 0 0 70
$attachment "head" "ValveBiped.HC_BodyCube" 0 0 0 rotate 0 0 0
$attachment "chest" "ValveBiped.Bip01_Spine4" -4 0 3 rotate -70 0 -180
$attachment "Blood_Left" "ValveBiped.Bip01_L_Finger2" 0 0 0 rotate 0 0 0
$attachment "Blood_Right" "ValveBiped.Bip01_R_Finger2" 0 0 0 rotate 0 0 0
$attachment "Cable1a" "ValveBiped.Bip01_L_Forearm" 0 0 0 rotate 0 0 0
$attachment "Cable1b" "ValveBiped.Bip01_L_Hand" 0 0 0 rotate 0 0 0
$attachment "Cable2a" "ValveBiped.Bip01_R_Forearm" 0 0 0 rotate 0 0 0
$attachment "Cable2b" "ValveBiped.Bip01_R_Hand" 0 0 0 rotate 0 0 0

$cbox 0 0 0 0 0 0

$bbox -13 -13 0 13 13 72

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
$hbox 1 "ValveBiped.HC_BodyCube" -6.25 -5.95 -5.48 6.03 8.21 4.94
$hbox 0 "ValveBiped.Bip01_Spine2" -2 -8 -4 18 8 8
$hbox 0 "ValveBiped.Bip01" -8 -4.5 -6 8 10.5 6

$definebone "ValveBiped.Bip01" "" 0 1.516078 38.353989 0 0 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Pelvis" "ValveBiped.Bip01" 0 0 0 0 0 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Thigh" "ValveBiped.Bip01_Pelvis" 3.890447 0 0 -3.134709 -83.193056 -90.000003 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Calf" "ValveBiped.Bip01_L_Thigh" 17.591 0 -0.000001 0.000002 8.817994 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf" 17.699585 0 0 1.263166 -62.10314 -7.021982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Thigh" "ValveBiped.Bip01_Pelvis" -3.890447 0 0 -3.134709 -96.806969 -90.000023 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Calf" "ValveBiped.Bip01_R_Thigh" 17.591005 0 0 0 8.817993 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf" 17.699581 0 0.000002 -1.263143 -62.103133 7.634777 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine" "ValveBiped.Bip01" 0 3.345131 -2.981898 3.030546 89.999982 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine1" "ValveBiped.Bip01_Spine" 4.126621 0 0 0 -6.300129 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine2" "ValveBiped.Bip01_Spine1" 3.612873 0 0 0 9.000305 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine4" "ValveBiped.Bip01_Spine2" 9.183983 0 0 0 9.938357 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Neck1" "ValveBiped.Bip01_Spine4" 3.138092 0 0 0 -53.66133 179.999964 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Head1" "ValveBiped.Bip01_Spine4" 4.149979 -1.375576 0 0.000014 -30.564717 179.999964 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Clavicle" "ValveBiped.Bip01_Spine4" 1.663036 1.21051 2.595493 -73.711845 169.261336 90.864274 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_UpperArm" "ValveBiped.Bip01_L_Clavicle" 6.028142 0.000004 0 -3.23343 -33.943008 -89.993623 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_L_UpperArm" 12.693008 0 0 0.000005 -43.708491 -0.000009 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm" 11.575974 -0.000004 -0.000031 -0.152225 2.188071 66.004325 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand" 5.562234 -0.288269 -0.510578 11.370639 -0.288842 -175.421033 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3" 5.158504 0 0 -0.325442 0.504048 -4.026864 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31" 3.326 -0.000004 0 -0.42342 0.564384 0.509086 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand" 5.591166 -0.279018 1.848877 -6.139754 0.268022 -175.484608 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1" 5.293 0 -0.000004 -0.3581 0.481408 -4.215535 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11" 3.079117 0 0 -0.392479 0.453791 0.399831 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand" 5.73974 -0.095432 0.607273 1.464944 -1.052536 -175.509634 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2" 5.6525 -0.000004 0.000002 -0.347915 0.488858 -3.88151 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21" 3.325017 0.000004 0 -0.380231 0.46411 0.535048 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Clavicle" "ValveBiped.Bip01_Spine4" 1.663048 1.210516 -2.595442 73.455003 168.779602 -95.441805 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_UpperArm" "ValveBiped.Bip01_R_Clavicle" 6.028137 0.000008 0 3.847067 -33.255071 93.511751 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm" 12.693005 0 0.000004 -0.000015 -46.506639 0.000019 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm" 11.599607 0.000004 0.000008 -6.102471 1.638433 -66.743324 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger3" "ValveBiped.Bip01_R_Hand" 5.928188 0.37891 0.637283 -10.707787 6.136648 168.772608 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger31" "ValveBiped.Bip01_R_Finger3" 5.074006 0 0 0.000011 0.000014 -0.000009 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger32" "ValveBiped.Bip01_R_Finger31" 3.326001 0.000004 0 -0.000049 0.000017 -0.000009 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand" 6.18965 0.568798 -0.554356 -0.783931 4.738863 168.977514 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger21" "ValveBiped.Bip01_R_Finger2" 5.511005 0 0 0.000006 0.710715 -0.000006 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger22" "ValveBiped.Bip01_R_Finger21" 3.325018 0 -0.000004 0.000008 -0.566295 -0.000005 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger1" "ValveBiped.Bip01_R_Hand" 5.900461 0.299198 -1.780357 7.365082 5.871197 168.939183 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger11" "ValveBiped.Bip01_R_Finger1" 5.248507 0 -0.000002 0.000003 0.678289 -0.000005 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger12" "ValveBiped.Bip01_R_Finger11" 3.151 0 -0.000002 -0.000001 -0.573798 -0.000007 0 0 0 0 0 0
$definebone "ValveBiped.HC_BodyCube" "ValveBiped.Bip01_Spine4" 7.600437 -0.90844 0.636362 -89.999982 170.392033 0 0 0 0 0 0 0
$definebone "ValveBiped.HC_HeadCube" "ValveBiped.HC_BodyCube" 0.051795 -3.221541 -2.149796 -0.227178 -179.024162 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.HC_BoneLF" "ValveBiped.HC_HeadCube" -3.92942 0 0 34.419307 -144.015809 26.145389 0 0 0 0 0 0
$definebone "ValveBiped.HC_BoneLF1" "ValveBiped.HC_BoneLF" 13.106236 0.000004 -0.000003 -0.00002 116.133679 0.000005 0 0 0 0 0 0
$definebone "ValveBiped.HC_BoneRF" "ValveBiped.HC_HeadCube" 3.929418 0 0 30.834425 -33.423049 -23.567308 0 0 0 0 0 0
$definebone "ValveBiped.HC_BoneRF1" "ValveBiped.HC_BoneRF" 13.106233 0.000004 -0.000004 -0.000019 -118.266296 -0.000005 0 0 0 0 0 0
$definebone "ValveBiped.HC_BoneLB" "ValveBiped.HC_BodyCube" 3.007646 4.344523 0.542816 -15.51381 40.457263 75.722387 0 0 0 0 0 0
$definebone "ValveBiped.HC_BoneLB1" "ValveBiped.HC_BoneLB" 10.074757 -0.000008 -0.000004 0.000006 -93.416436 -0.000022 0 0 0 0 0 0
$definebone "ValveBiped.HC_BoneLB2" "ValveBiped.HC_BoneLB1" 10.292011 0 -0.000001 -0.000005 -24.396712 -0.00002 0 0 0 0 0 0
$definebone "ValveBiped.HC_BoneRB" "ValveBiped.HC_BodyCube" -2.753414 4.323164 0.558395 -12.847951 130.549176 110.68284 0 0 0 0 0 0
$definebone "ValveBiped.HC_BoneRB1" "ValveBiped.HC_BoneRB" 10.074759 0.000008 0.000008 -0.000007 -91.912742 0.00001 0 0 0 0 0 0
$definebone "ValveBiped.HC_BoneRB2" "ValveBiped.HC_BoneRB1" 10.292 -0.000002 0.000002 0 -23.266273 0.000015 0 0 0 0 0 0


$poseparameter "move_yaw" -180 180 loop 0
$poseparameter "lean_lateral" -1 1 loop 0
$poseparameter "lean_vertical" -1 1 loop 0

$ikchain "rfoot" "ValveBiped.Bip01_R_Foot" knee 0.806 -0.592 0
$ikchain "lfoot" "ValveBiped.Bip01_L_Foot" knee 0.779 -0.627 0
$ikchain "rhand" "ValveBiped.Bip01_R_Hand" knee 0.75 0.661 0
$ikchain "lhand" "ValveBiped.Bip01_L_Hand" knee 0.779 0.628 0
$ikchain "zombie_right_foot" "ValveBiped.Bip01_R_Foot" knee 0.806 -0.592 0
$ikchain "zombie_left_foot" "ValveBiped.Bip01_L_Foot" knee 0.779 -0.627 0
$ikchain "zombie_right_hand" "ValveBiped.Bip01_R_Hand" knee 0.75 0.661 0
$ikchain "zombie_left_hand" "ValveBiped.Bip01_L_Hand" knee 0.779 0.628 0

$animation "vehicle_idle_lean_left_corrective_animation" "fast_main_anims\vehicle_idle_lean_left_corrective_animation.smd" {
}

$animation "vehicle_idle_lean_center_corrective_animation" "fast_main_anims\vehicle_idle_lean_center_corrective_animation.smd" {
}

$animation "vehicle_idle_lean_right_corrective_animation" "fast_main_anims\vehicle_idle_lean_right_corrective_animation.smd" {
}

$animation "vehicle_idle_lean_up_corrective_animation" "fast_main_anims\vehicle_idle_lean_up_corrective_animation.smd" {
}

$animation "vehicle_idle_lean_down_corrective_animation" "fast_main_anims\vehicle_idle_lean_down_corrective_animation.smd" {
}

$animation "climblanding" "fast_main_anims\climblanding.smd" {
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 7
}

$animation "a_Walks" "fast_main_anims\a_Walks.smd" {
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
	walkframe 20 LX LY
}

$animation "a_WalksE" "fast_main_anims\a_WalksE.smd" {
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
	walkframe 20 LX LY
}

$animation "a_WalkE" "fast_main_anims\a_WalkE.smd" {
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
	walkframe 20 LX LY
}

$animation "a_WalkNE" "fast_main_anims\a_WalkNE.smd" {
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
	walkframe 20 LX LY
}

$animation "a_WalkN" "fast_main_anims\a_WalkN.smd" {
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
	walkframe 20 LX LY
}

$animation "a_WalkNW" "fast_main_anims\a_WalkNW.smd" {
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
	walkframe 20 LX LY
}

$animation "a_WalkW" "fast_main_anims\a_WalkW.smd" {
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
	walkframe 20 LX LY
}

$animation "a_WalkSW" "fast_main_anims\a_WalkSW.smd" {
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
	walkframe 20 LX LY
}

$animation "vehicle_idle_lean_neutral" "fast_main_anims\vehicle_idle_lean_neutral.smd" {
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 7
}

$animation "vehicle_idle_lean_center" "fast_main_anims\vehicle_idle_lean_center.smd" {
	fps 30
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "vehicle_idle_lean_center_corrective_animation" 0
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 7
}

$animation "vehicle_idle_lean_right" "fast_main_anims\vehicle_idle_lean_right.smd" {
	fps 30
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "vehicle_idle_lean_right_corrective_animation" 0
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 7
}

$animation "vehicle_idle_lean_left" "fast_main_anims\vehicle_idle_lean_left.smd" {
	fps 30
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "vehicle_idle_lean_left_corrective_animation" 0
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 7
}

$animation "vehicle_idle_lean_up" "fast_main_anims\vehicle_idle_lean_up.smd" {
	fps 30
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "vehicle_idle_lean_up_corrective_animation" 0
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 7
}

$animation "vehicle_idle_lean_down" "fast_main_anims\vehicle_idle_lean_down.smd" {
	fps 30
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "vehicle_idle_lean_down_corrective_animation" 0
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 7
}

$sequence "idle" {
	"fast_main_anims\idle.smd"
	activity "ACT_IDLE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 21 21 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 21 21 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 21 21 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 21 21 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 21 21 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 21 21 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 21 21 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 21 21 target 7
}

$sequence "idle_angry" {
	"fast_main_anims\idle_angry.smd"
	activity "ACT_IDLE_ANGRY" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 7
}

$sequence "idle_onfire" {
	"fast_main_anims\idle_onfire.smd"
	activity "ACT_IDLE_ON_FIRE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 60 60 target 7
}

$sequence "Run" {
	"fast_main_anims\Run.smd"
	activity "ACT_RUN" 1
	{ event 1100 5 "step" }
	{ event 1100 9 "step" }
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 7 usesequence 
	walkframe 14 LX
}

$sequence "Leap" {
	"fast_main_anims\Leap.smd"
	activity "ACT_RANGE_ATTACK1" 1
	{ event 1100 8 "leap" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 7 usesequence 
	walkframe 1 LX LY LZ
}

$sequence "LeapStrike" {
	"fast_main_anims\LeapStrike.smd"
	activity "ACT_GLIDE" 1
	fadein 0.2
	fadeout 0.2
	fps 26
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 7 usesequence 
	walkframe 11 LX LY LZ
}

$sequence "LandRight" {
	"fast_main_anims\LandRight.smd"
	activity "ACT_FASTZOMBIE_LAND_RIGHT" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 7 usesequence 
	walkframe 24 LX LY
}

$sequence "LandLeft" {
	"fast_main_anims\LandLeft.smd"
	activity "ACT_FASTZOMBIE_LAND_LEFT" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 39 39 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 39 39 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 39 39 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 39 39 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 39 39 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 39 39 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 39 39 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 39 39 target 7 usesequence 
	walkframe 30 LX LY
}

$sequence "Melee" {
	"fast_main_anims\Melee.smd"
	activity "ACT_MELEE_ATTACK1" 1
	{ event 1100 3 "melee" }
	{ event 1100 7 "melee" }
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 7 usesequence 
	walkframe 14 LX LY LZ
}

$sequence "climbmount" {
	"fast_main_anims\climbmount.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 7 usesequence 
	walkframe 21 LX LY LZ
}

$sequence "climbloop" {
	"fast_main_anims\climbloop.smd"
	activity "ACT_CLIMB_UP" 1
	{ event 1100 5 "step_climb" }
	{ event 1100 12 "step_climb" }
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 7 usesequence 
	walkframe 14 LZ
}

$sequence "climbdismount" {
	"fast_main_anims\climbdismount.smd"
	activity "ACT_CLIMB_DISMOUNT" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 44 fakeorigin 11.67 4.09 0 fakerotate 0 0 0 floor 0 height 18 radius 0 range 40 44 52 52 target 1 usesequence 
	ikrule "rfoot" footstep contact 47 fakeorigin 20.41 -4.46 0 fakerotate 0 0 0 floor 0 height 18 radius 0 range 43 47 52 52 target 0 usesequence 
	ikrule "lhand" footstep contact 33 fakeorigin 26.59 10.63 0 fakerotate 0 0 0 floor 0 height 18 radius 0 range 18 23 33 38 target 3 usesequence 
	ikrule "rhand" footstep contact 33 fakeorigin 22.19 -11.99 0 fakerotate 0 0 0 floor 0 height 18 radius 0 range 12 16 33 38 target 2 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 7 usesequence 
	walkframe 33 LZ
	walkframe 52 LX
}

$sequence "JumpNavMove" {
	"fast_main_anims\JumpNavMove.smd"
	activity "ACT_JUMP" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 52 52 target 7 usesequence 
	walkframe 43
}

$sequence "leap_start" {
	"fast_main_anims\leap_start.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 13 13 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 13 13 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 13 13 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 13 13 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 13 13 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 13 13 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 13 13 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 13 13 target 7 usesequence 
	walkframe 4
}

$sequence "leap_loop" {
	"fast_main_anims\leap_loop.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 7 usesequence 
	walkframe 30
}

$sequence "leap_land" {
	"fast_main_anims\leap_land.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 7 usesequence 
	walkframe 9
}

$sequence "BR2_Roar" {
	"fast_main_anims\BR2_Roar.smd"
	activity "ACT_FASTZOMBIE_FRENZY" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 7 usesequence 
	walkframe 40 LX LY
}

$sequence "BR2_Attack" {
	"fast_main_anims\BR2_Attack.smd"
	activity "ACT_FASTZOMBIE_BIG_SLASH" 1
	{ event 1100 18 "melee" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 34 34 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 34 34 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 34 34 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 34 34 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 34 34 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 34 34 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 34 34 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 34 34 target 7 usesequence 
	walkframe 25 LX LY
}

$sequence "walk_all" {
	"a_Walks"
	"a_WalksE"
	"a_WalkE"
	"a_WalkNE"
	"a_WalkN"
	"a_WalkNW"
	"a_WalkW"
	"a_WalkSW"
	"a_Walks"
	activity "ACT_WALK" 1
	blend "move_yaw" -180 180
	blendwidth 9
	{ event 1100 7 "step" }
	{ event 1100 16 "step" }
	fadein 0.2
	fadeout 0.2
}

$sequence "vehicle_lean_all" {
	"vehicle_idle_lean_left"
	"vehicle_idle_lean_center"
	"vehicle_idle_lean_right"
	blend "lean_lateral" -1 1
	blendwidth 3
	delta
	fadein 0.2
	fadeout 0.2
	iklock "zombie_right_foot" 1 0
	iklock "zombie_left_foot" 1 0
	iklock "zombie_right_hand" 1 0
	iklock "zombie_left_hand" 1 0
}

$sequence "vehicle_lean_vertical" {
	"vehicle_idle_lean_up"
	"vehicle_idle_lean_center"
	"vehicle_idle_lean_down"
	blend "lean_vertical" -1 1
	blendwidth 3
	delta
	fadein 0.2
	fadeout 0.2
	iklock "zombie_right_foot" 1 0
	iklock "zombie_left_foot" 1 0
	iklock "zombie_right_hand" 1 0
	iklock "zombie_left_hand" 1 0
}

$sequence "vehicle_idle" {
	"fast_main_anims\vehicle_idle.smd"
	activity "ACT_PASSENGER_IDLE" 1
	fadein 0.2
	fadeout 0.2
	addlayer "vehicle_lean_all"
	addlayer "vehicle_lean_vertical"
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 7
}

$sequence "vehicle_idle_right" {
	"fast_main_anims\vehicle_idle_right.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 7
}

$sequence "vehicle_idle_left" {
	"fast_main_anims\vehicle_idle_left.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 7
}

$sequence "buggy_jumpdown_preidle" {
	"fast_main_anims\buggy_jumpdown_preidle.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 40 40 target 7 usesequence 
	walkframe 3 LX LY LZ
	walkframe 6 LX LY LZ
	walkframe 10 LX LY LZ
	walkframe 17 LX LY LZ
	walkframe 23 LX LY LZ
}

$sequence "buggy_preidle" {
	"fast_main_anims\buggy_preidle.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
}

$sequence "buggy_preidle_leap" {
	"fast_main_anims\buggy_preidle_leap.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 15 15 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 15 15 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 15 15 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 15 15 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 15 15 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 15 15 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 15 15 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 15 15 target 7 usesequence 
	walkframe 6 LX LY LZ
	walkframe 15 LX LY LZ
}

$sequence "buggy_postidle_leap" {
	"fast_main_anims\buggy_postidle_leap.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 7
}

$sequence "buggy_enter1" {
	"fast_main_anims\buggy_enter1.smd"
	{ event AE_PASSENGER_PHYSICS_PUSH 8 "150000" }
	fadein 0.2
	fadeout 0.2

keyvalues
{
	entryexit_blend 
	{
		"origin"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
		"angles"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
	}
}
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
}

$sequence "buggy_enter_right" {
	"fast_main_anims\buggy_enter_right.smd"
	{ event AE_PASSENGER_PHYSICS_PUSH 6 "300000" }
	fadein 0.2
	fadeout 0.2

keyvalues
{
	entryexit_blend 
	{
		"origin"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
		"angles"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
	}
}
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 7 usesequence 
	walkframe 15 LX LY LZ LZR
}

$sequence "buggy_enter_left" {
	"fast_main_anims\buggy_enter_left.smd"
	{ event AE_PASSENGER_PHYSICS_PUSH 6 "400000" }
	fadein 0.2
	fadeout 0.2

keyvalues
{
	entryexit_blend 
	{
		"origin"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
		"angles"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
	}
}
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 7 usesequence 
	walkframe 15 LX LY LZ LZR
}

$sequence "buggy_enter_front_left" {
	"fast_main_anims\buggy_enter_front_left.smd"
	{ event 1008 45 "NPC_FastZombie.CarScream" }
	{ event AE_PASSENGER_PHYSICS_PUSH 8 "300000" }
	fadein 0.2
	fadeout 0.2

keyvalues
{
	entryexit_blend 
	{
		"origin"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
		"angles"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
	}
}
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 7 usesequence 
	walkframe 77 LX LY LZ
}

$sequence "buggy_enter_front_right" {
	"fast_main_anims\buggy_enter_front_right.smd"
	{ event AE_PASSENGER_PHYSICS_PUSH 8 "300000" }
	fadein 0.2
	fadeout 0.2

keyvalues
{
	entryexit_blend 
	{
		"origin"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
		"angles"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
	}
}
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 7 usesequence 
	walkframe 24 LX LY LZ
}

$sequence "buggy_enter_back_left" {
	"fast_main_anims\buggy_enter_back_left.smd"
	{ event AE_PASSENGER_PHYSICS_PUSH 6 "400000" }
	fadein 0.2
	fadeout 0.2

keyvalues
{
	entryexit_blend 
	{
		"origin"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
		"angles"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
	}
}
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 7 usesequence 
	walkframe 40 LX LY LZ LZR
}

$sequence "buggy_enter_back_right" {
	"fast_main_anims\buggy_enter_back_right.smd"
	{ event AE_PASSENGER_PHYSICS_PUSH 6 "300000" }
	fadein 0.2
	fadeout 0.2

keyvalues
{
	entryexit_blend 
	{
		"origin"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
		"angles"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
	}
}
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 49 49 target 7 usesequence 
	walkframe 40 LX LY LZ LZR
}

$sequence "buggy_dismount" {
	"fast_main_anims\buggy_dismount.smd"
	fadein 0.2
	fadeout 0.2

keyvalues
{
	entryexit_blend 
	{
		"origin"		 
		{
			"startframe"			"0"
			"endframe"			"1"
		}
		"angles"		 
		{
			"startframe"			"0"
			"endframe"			"1"
		}
	}
}
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 12 12 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 12 12 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 12 12 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 12 12 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 12 12 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 12 12 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 12 12 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 12 12 target 7 usesequence 
	walkframe 3 LX LY LZ
}

$sequence "vehicle_bump" {
	"fast_main_anims\vehicle_bump.smd"
	activity "ACT_PASSENGER_FLINCH" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
}

$sequence "vehicle_melee1" {
	"fast_main_anims\vehicle_melee1.smd"
	activity "ACT_PASSENGER_MELEE_ATTACK1" 1
	{ event AE_ZOMBIE_ATTACK_LEFT 8 "" }
	fadein 0.2
	fadeout 0.2
	addlayer "vehicle_lean_all"
	addlayer "vehicle_lean_vertical"
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 7
}

$sequence "vehicle_melee2" {
	"fast_main_anims\vehicle_melee2.smd"
	activity "ACT_PASSENGER_MELEE_ATTACK1" 1
	{ event AE_ZOMBIE_ATTACK_LEFT 8 "" }
	fadein 0.2
	fadeout 0.2
	addlayer "vehicle_lean_all"
	addlayer "vehicle_lean_vertical"
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 25 25 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 25 25 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 25 25 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 25 25 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 25 25 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 25 25 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 25 25 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 25 25 target 7
}

$sequence "vehicle_pass_melee1" {
	"fast_main_anims\vehicle_pass_melee1.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 7
}

$sequence "vehicle_driver_melee1" {
	"fast_main_anims\vehicle_driver_melee1.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 24 24 target 7
}

$sequence "vehicle_roar" {
	"fast_main_anims\vehicle_roar.smd"
	activity "ACT_PASSENGER_THREATEN" 1
	{ event 1008 6 "NPC_FastZombie.CarScream" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 7
}

$sequence "vehicle_leap" {
	"fast_main_anims\vehicle_leap.smd"
	activity "ACT_PASSENGER_RANGE_ATTACK1" 1
	{ event AE_FASTZOMBIE_VEHICLE_LEAP 8 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 7 usesequence 
	walkframe 1 LX LY LZ
}

$sequence "vehicle_leap_loop" {
	"fast_main_anims\vehicle_leap_loop.smd"
	activity "ACT_PASSENGER_ZOMBIE_LEAP_LOOP" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 0 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 1 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 2 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 3 usesequence 
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 4 usesequence 
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 5 usesequence 
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 6 usesequence 
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 11 11 target 7 usesequence 
	walkframe 11 LX LY LZ
}

$sequence "ragdoll" {
	"fast_main_anims\ragdoll.smd"
	activity "ACT_DIERAGDOLL" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 7
}

$sequence "buggy_fast_dynamic01" {
	"fast_main_anims\buggy_fast_dynamic01.smd"
	{ event 1008 4 "NPC_FastZombie.CarEnter1" }
	{ event 1008 21 "NPC_FastZombie.CarEnter2" }
	{ event 1008 30 "NPC_FastZombie.CarEnter3" }
	{ event 1008 45 "NPC_FastZombie.CarEnter4" }
	{ event AE_SV_PLAYSOUND 60 "NPC_FastZombie.CarScream" }
	{ event AE_PASSENGER_PHYSICS_PUSH 8 "300000" }
	{ event AE_FASTZOMBIE_VEHICLE_SS_DIE 95 "" }
	fadein 0.2
	fadeout 0.2

keyvalues
{
	entryexit_blend 
	{
		"origin"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
		"angles"		 
		{
			"startframe"			"0"
			"endframe"			"5"
		}
	}
}
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 100 100 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 100 100 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 100 100 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 100 100 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 100 100 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 100 100 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 100 100 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 100 100 target 7
}

$sequence "slump_a" {
	"fast_main_anims\slump_a.smd"
	activity "ACT_RANGE_ATTACK2" -1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 7
}

$sequence "slumprise_a" {
	"fast_main_anims\slumprise_a.smd"
	activity "ACT_SPECIAL_ATTACK1" 1
	{ event 1100 1 "sound_alert" }
	{ event 1100 16 "get_up" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 7
}

$sequence "slump_b" {
	"fast_main_anims\slump_b.smd"
	activity "ACT_IDLE_STIMULATED" -1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 7
}

$sequence "slumprise_b" {
	"fast_main_anims\slumprise_b.smd"
	activity "ACT_SPECIAL_ATTACK2" 1
	{ event 1100 1 "sound_alert" }
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 10 "get_up" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 7
}

$sequence "slumprise_c" {
	"fast_main_anims\slumprise_c.smd"
	activity "ACT_IDLE_STEALTH" -1
	{ event 1100 1 "sound_alert" }
	{ event 1100 20 "sound_attack_scream" }
	{ event 1100 10 "get_up" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 0
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 1
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 2
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 3
	ikrule "zombie_right_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 4
	ikrule "zombie_left_foot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 5
	ikrule "zombie_right_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 6
	ikrule "zombie_left_hand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 7
}

$collisionjoints "fast_main_physics.smd"
{
	$mass 95
	$inertia 2
	$damping 0.01
	$rotdamping 0.4
	$rootbone "valvebiped.bip01_pelvis"


	$jointconstrain "ValveBiped.Bip01_Spine" x limit -32 32 0.008
	$jointconstrain "ValveBiped.Bip01_Spine" y limit -39 39 0.008
	$jointconstrain "ValveBiped.Bip01_Spine" z limit -15 40 0.008

	$jointconstrain "ValveBiped.Bip01_Spine2" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_Spine2" y limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_Spine2" z limit -41 41 0.008

	$jointconstrain "ValveBiped.Bip01_L_UpperArm" x limit -30 61 0.008
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" y limit -90 48 0.008
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" z limit -106 68 0.008

	$jointmassbias "ValveBiped.Bip01_L_Forearm" 2
	$jointconstrain "ValveBiped.Bip01_L_Forearm" x limit 1 1 0.008
	$jointconstrain "ValveBiped.Bip01_L_Forearm" y limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_L_Forearm" z limit -102 44 0.008

	$jointmassbias "ValveBiped.Bip01_L_Hand" 2
	$jointconstrain "ValveBiped.Bip01_L_Hand" x limit -39 39 0.008
	$jointconstrain "ValveBiped.Bip01_L_Hand" y limit -19 19 0.008
	$jointconstrain "ValveBiped.Bip01_L_Hand" z limit -64 26 0.008

	$jointconstrain "ValveBiped.Bip01_R_UpperArm" x limit -41 37 0.008
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" y limit -44 90 0.008
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" z limit -106 68 0.008

	$jointmassbias "ValveBiped.Bip01_R_Forearm" 2
	$jointconstrain "ValveBiped.Bip01_R_Forearm" x limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_R_Forearm" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Forearm" z limit -41 93 0.008

	$jointconstrain "ValveBiped.Bip01_R_Thigh" x limit -28 30 0.008
	$jointconstrain "ValveBiped.Bip01_R_Thigh" y limit -37 26 0.008
	$jointconstrain "ValveBiped.Bip01_R_Thigh" z limit -82 44 0.008

	$jointmassbias "ValveBiped.Bip01_R_Calf" 2
	$jointconstrain "ValveBiped.Bip01_R_Calf" x limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_R_Calf" y limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_R_Calf" z limit -8 130 0.008

	$jointmassbias "ValveBiped.Bip01_R_Foot" 2
	$jointconstrain "ValveBiped.Bip01_R_Foot" x limit -30 30 0.008
	$jointconstrain "ValveBiped.Bip01_R_Foot" y limit -28 28 0.008
	$jointconstrain "ValveBiped.Bip01_R_Foot" z limit -30 26 0.008

	$jointconstrain "ValveBiped.Bip01_L_Thigh" x limit -37 30 0.008
	$jointconstrain "ValveBiped.Bip01_L_Thigh" y limit -30 19 0.008
	$jointconstrain "ValveBiped.Bip01_L_Thigh" z limit -75 39 0.008

	$jointmassbias "ValveBiped.Bip01_L_Calf" 2
	$jointconstrain "ValveBiped.Bip01_L_Calf" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_Calf" y limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_Calf" z limit -8 130 0.008

	$jointmassbias "ValveBiped.Bip01_L_Foot" 2
	$jointconstrain "ValveBiped.Bip01_L_Foot" x limit -10 10 0.008
	$jointconstrain "ValveBiped.Bip01_L_Foot" y limit -17 17 0.008
	$jointconstrain "ValveBiped.Bip01_L_Foot" z limit -21 39 0.008

	$jointmassbias "ValveBiped.Bip01_R_Hand" 2
	$jointconstrain "ValveBiped.Bip01_R_Hand" x limit -58 58 0.008
	$jointconstrain "ValveBiped.Bip01_R_Hand" y limit -28 28 0.008
	$jointconstrain "ValveBiped.Bip01_R_Hand" z limit -35 46 0.008
}

$keyvalues
{
	Cables 
	{
		"Cable"		 
		{
			"StartAttachment"			"Cable1a"
			"EndAttachment"			"Cable1b"
			"Width"			".3"
			"Material"			"cable/cable_lit"
			"NumSegments"			"5"
			"Length"			"115"
		}
		"Cable"		 
		{
			"StartAttachment"			"Cable2a"
			"EndAttachment"			"Cable2b"
			"Width"			".3"
			"Material"			"cable/cable_lit"
			"NumSegments"			"5"
			"Length"			"115"
		}
	}
}
