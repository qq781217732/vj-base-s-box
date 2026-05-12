// Created by Crowbar 0.67

$modelname "vj_zombies/slow_main.mdl"

$bodygroup "studio"
{
	studio "zombie_classic_Zombie_reference.smd"
}
$bodygroup "headcrab1"
{
	blank
	studio "zombie_classic_Headcrab_reference.smd"
}

$lod 10
{
	replacemodel "zombie_classic_Zombie_reference.smd" "zombie_classic_Zombie_reference_lod1.smd"
	replacemodel "zombie_classic_Headcrab_reference.smd" "zombie_classic_Headcrab_reference_lod1.smd"
	replacebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3"
	replacebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31"
	replacebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21"
	replacebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11"
	replacebone "ValveBiped.Bip01_R_Finger31" "ValveBiped.Bip01_R_Finger3"
	replacebone "ValveBiped.Bip01_R_Finger32" "ValveBiped.Bip01_R_Finger31"
	replacebone "ValveBiped.Bip01_R_Finger22" "ValveBiped.Bip01_R_Finger21"
	replacebone "ValveBiped.Bip01_R_Finger12" "ValveBiped.Bip01_R_Finger11"
	replacebone "ValveBiped.Bip01_R_Finger01" "ValveBiped.Bip01_R_Finger0"
	replacebone "ValveBiped.Bip01_R_Finger02" "ValveBiped.Bip01_R_Finger01"
	replacebone "ValveBiped.Bip01_L_Finger01" "ValveBiped.Bip01_L_Finger0"
	replacebone "ValveBiped.Bip01_L_Finger02" "ValveBiped.Bip01_L_Finger01"
	nofacial
}
$lod 20
{
	replacemodel "zombie_classic_Zombie_reference.smd" "zombie_classic_Zombie_reference_lod2.smd"
	replacemodel "zombie_classic_Headcrab_reference.smd" "zombie_classic_Headcrab_reference_lod2.smd"
	replacebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3"
	replacebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31"
	replacebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2"
	replacebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21"
	replacebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1"
	replacebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11"
	replacebone "ValveBiped.Bip01_R_Finger31" "ValveBiped.Bip01_R_Finger3"
	replacebone "ValveBiped.Bip01_R_Finger32" "ValveBiped.Bip01_R_Finger31"
	replacebone "ValveBiped.Bip01_R_Finger21" "ValveBiped.Bip01_R_Finger2"
	replacebone "ValveBiped.Bip01_R_Finger22" "ValveBiped.Bip01_R_Finger21"
	replacebone "ValveBiped.Bip01_R_Finger11" "ValveBiped.Bip01_R_Finger1"
	replacebone "ValveBiped.Bip01_R_Finger12" "ValveBiped.Bip01_R_Finger11"
	replacebone "ValveBiped.Bip01_R_Finger01" "ValveBiped.Bip01_R_Finger0"
	replacebone "ValveBiped.Bip01_R_Finger02" "ValveBiped.Bip01_R_Finger01"
	replacebone "ValveBiped.Bip01_L_Finger01" "ValveBiped.Bip01_L_Finger0"
	replacebone "ValveBiped.Bip01_L_Finger02" "ValveBiped.Bip01_L_Finger01"
	nofacial
}
$lod 35
{
	replacemodel "zombie_classic_Zombie_reference.smd" "zombie_classic_Zombie_reference_lod3.smd"
	replacemodel "zombie_classic_Headcrab_reference.smd" "zombie_classic_Headcrab_reference_lod3.smd"
	replacebone "ValveBiped.Bip01_L_Toe0" "ValveBiped.Bip01_L_Foot"
	replacebone "ValveBiped.Bip01_R_Toe0" "ValveBiped.Bip01_R_Foot"
	replacebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3"
	replacebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31"
	replacebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2"
	replacebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21"
	replacebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1"
	replacebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11"
	replacebone "ValveBiped.Bip01_R_Finger3" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger31" "ValveBiped.Bip01_R_Finger3"
	replacebone "ValveBiped.Bip01_R_Finger32" "ValveBiped.Bip01_R_Finger31"
	replacebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger21" "ValveBiped.Bip01_R_Finger2"
	replacebone "ValveBiped.Bip01_R_Finger22" "ValveBiped.Bip01_R_Finger21"
	replacebone "ValveBiped.Bip01_R_Finger1" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger11" "ValveBiped.Bip01_R_Finger1"
	replacebone "ValveBiped.Bip01_R_Finger12" "ValveBiped.Bip01_R_Finger11"
	replacebone "ValveBiped.Bip01_R_Finger0" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger01" "ValveBiped.Bip01_R_Finger0"
	replacebone "ValveBiped.Bip01_R_Finger02" "ValveBiped.Bip01_R_Finger01"
	replacebone "ValveBiped.Bip01_L_Finger0" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger01" "ValveBiped.Bip01_L_Finger0"
	replacebone "ValveBiped.Bip01_L_Finger02" "ValveBiped.Bip01_L_Finger01"
	replacebone "ValveBiped.HC_Head_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_Rear_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ThighL_Bone" "ValveBiped.HC_Rear_Bone"
	replacebone "ValveBiped.HC_CalfL_Bone" "ValveBiped.HC_ThighL_Bone"
	replacebone "ValveBiped.HC_ThighR_Bone" "ValveBiped.HC_Rear_Bone"
	replacebone "ValveBiped.HC_CalfR_Bone" "ValveBiped.HC_ThighR_Bone"
	replacebone "ValveBiped.HC_UpperArmL_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ForeArmL_Bone" "ValveBiped.HC_UpperArmL_Bone"
	replacebone "ValveBiped.HC_UpperArmR_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ForeArmR_Bone" "ValveBiped.HC_UpperArmR_Bone"
	nofacial
}
$lod 60
{
	replacemodel "zombie_classic_Zombie_reference.smd" "zombie_classic_Zombie_reference_lod4.smd"
	replacemodel "zombie_classic_Headcrab_reference.smd" "zombie_classic_Headcrab_reference_lod4.smd"
	replacebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf"
	replacebone "ValveBiped.Bip01_L_Toe0" "ValveBiped.Bip01_L_Foot"
	replacebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf"
	replacebone "ValveBiped.Bip01_R_Toe0" "ValveBiped.Bip01_R_Foot"
	replacebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm"
	replacebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3"
	replacebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31"
	replacebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2"
	replacebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21"
	replacebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1"
	replacebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11"
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
	replacebone "ValveBiped.Bip01_R_Finger0" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger01" "ValveBiped.Bip01_R_Finger0"
	replacebone "ValveBiped.Bip01_R_Finger02" "ValveBiped.Bip01_R_Finger01"
	replacebone "ValveBiped.Bip01_L_Finger0" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger01" "ValveBiped.Bip01_L_Finger0"
	replacebone "ValveBiped.Bip01_L_Finger02" "ValveBiped.Bip01_L_Finger01"
	replacebone "ValveBiped.HC_Body_Bone" "ValveBiped.Bip01_Spine4"
	replacebone "ValveBiped.HC_Head_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_Rear_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ThighL_Bone" "ValveBiped.HC_Rear_Bone"
	replacebone "ValveBiped.HC_CalfL_Bone" "ValveBiped.HC_ThighL_Bone"
	replacebone "ValveBiped.HC_ThighR_Bone" "ValveBiped.HC_Rear_Bone"
	replacebone "ValveBiped.HC_CalfR_Bone" "ValveBiped.HC_ThighR_Bone"
	replacebone "ValveBiped.HC_UpperArmL_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ForeArmL_Bone" "ValveBiped.HC_UpperArmL_Bone"
	replacebone "ValveBiped.HC_UpperArmR_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ForeArmR_Bone" "ValveBiped.HC_UpperArmR_Bone"
	nofacial
}
$lod 90
{
	replacemodel "zombie_classic_Zombie_reference.smd" "zombie_classic_Zombie_reference_lod5.smd"
	replacemodel "zombie_classic_Headcrab_reference.smd" "zombie_classic_Headcrab_reference_lod5.smd"
	replacebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf"
	replacebone "ValveBiped.Bip01_L_Toe0" "ValveBiped.Bip01_L_Foot"
	replacebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf"
	replacebone "ValveBiped.Bip01_R_Toe0" "ValveBiped.Bip01_R_Foot"
	replacebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm"
	replacebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3"
	replacebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31"
	replacebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2"
	replacebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21"
	replacebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1"
	replacebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11"
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
	replacebone "ValveBiped.Bip01_R_Finger0" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger01" "ValveBiped.Bip01_R_Finger0"
	replacebone "ValveBiped.Bip01_R_Finger02" "ValveBiped.Bip01_R_Finger01"
	replacebone "ValveBiped.Bip01_L_Finger0" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger01" "ValveBiped.Bip01_L_Finger0"
	replacebone "ValveBiped.Bip01_L_Finger02" "ValveBiped.Bip01_L_Finger01"
	replacebone "ValveBiped.HC_Body_Bone" "ValveBiped.Bip01_Spine4"
	replacebone "ValveBiped.HC_Head_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_Rear_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ThighL_Bone" "ValveBiped.HC_Rear_Bone"
	replacebone "ValveBiped.HC_CalfL_Bone" "ValveBiped.HC_ThighL_Bone"
	replacebone "ValveBiped.HC_ThighR_Bone" "ValveBiped.HC_Rear_Bone"
	replacebone "ValveBiped.HC_CalfR_Bone" "ValveBiped.HC_ThighR_Bone"
	replacebone "ValveBiped.HC_UpperArmL_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ForeArmL_Bone" "ValveBiped.HC_UpperArmL_Bone"
	replacebone "ValveBiped.HC_UpperArmR_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ForeArmR_Bone" "ValveBiped.HC_UpperArmR_Bone"
	nofacial
}
$shadowlod
{
	replacemodel "zombie_classic_Zombie_reference.smd" "zombie_classic_Zombie_reference_lod6.smd"
	replacemodel "zombie_classic_Headcrab_reference.smd" "zombie_classic_Headcrab_reference_lod6.smd"
	replacebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf"
	replacebone "ValveBiped.Bip01_L_Toe0" "ValveBiped.Bip01_L_Foot"
	replacebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf"
	replacebone "ValveBiped.Bip01_R_Toe0" "ValveBiped.Bip01_R_Foot"
	replacebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm"
	replacebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3"
	replacebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31"
	replacebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2"
	replacebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21"
	replacebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1"
	replacebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11"
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
	replacebone "ValveBiped.Bip01_R_Finger0" "ValveBiped.Bip01_R_Hand"
	replacebone "ValveBiped.Bip01_R_Finger01" "ValveBiped.Bip01_R_Finger0"
	replacebone "ValveBiped.Bip01_R_Finger02" "ValveBiped.Bip01_R_Finger01"
	replacebone "ValveBiped.Bip01_L_Finger0" "ValveBiped.Bip01_L_Hand"
	replacebone "ValveBiped.Bip01_L_Finger01" "ValveBiped.Bip01_L_Finger0"
	replacebone "ValveBiped.Bip01_L_Finger02" "ValveBiped.Bip01_L_Finger01"
	replacebone "ValveBiped.HC_Body_Bone" "ValveBiped.Bip01_Spine4"
	replacebone "ValveBiped.HC_Head_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_Rear_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ThighL_Bone" "ValveBiped.HC_Rear_Bone"
	replacebone "ValveBiped.HC_CalfL_Bone" "ValveBiped.HC_ThighL_Bone"
	replacebone "ValveBiped.HC_ThighR_Bone" "ValveBiped.HC_Rear_Bone"
	replacebone "ValveBiped.HC_CalfR_Bone" "ValveBiped.HC_ThighR_Bone"
	replacebone "ValveBiped.HC_UpperArmL_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ForeArmL_Bone" "ValveBiped.HC_UpperArmL_Bone"
	replacebone "ValveBiped.HC_UpperArmR_Bone" "ValveBiped.HC_Body_Bone"
	replacebone "ValveBiped.HC_ForeArmR_Bone" "ValveBiped.HC_UpperArmR_Bone"
	nofacial
}

$surfaceprop "zombieflesh"

$contents "solid"

$illumposition -2.887 -4.81 33.483

$eyeposition 0 0 64

$mostlyopaque

$cdmaterials "models\zombie_classic\"
$cdmaterials "models\headcrab_classic\"

$attachment "headcrab" "ValveBiped.HC_Rear_Bone" 0 0 0 rotate 0 0 0
$attachment "eyes" "ValveBiped.HC_Body_Bone" 0 0 0 rotate 0 0 -50
$attachment "head" "ValveBiped.HC_Body_Bone" 0 0 0 rotate 0 0 -50
$attachment "chest" "ValveBiped.Bip01_Spine4" -3 6 0 rotate 0 60 90
$attachment "maw" "ValveBiped.Bip01_Spine2" 0 8 0 rotate 0 90 90
$attachment "Blood_Left" "ValveBiped.Bip01_L_Finger2" 0 0 0 rotate 0 0 0
$attachment "Blood_Right" "ValveBiped.Bip01_R_Finger2" 0 0 0 rotate 0 0 0

$cbox 0 0 0 0 0 0

$bbox -13 -13 0 13 13 72

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

$definebone "ValveBiped.Bip01_Pelvis" "" -0.000005 1.515989 38.353996 0 -0.000057 90.000037 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Thigh" "ValveBiped.Bip01_Pelvis" 3.890453 0 -0.000004 -6.298009 -90.530886 -89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Calf" "ValveBiped.Bip01_L_Thigh" 17.593172 0 0 0 16.008157 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf" 17.699579 0 0 -2.794963 -66.211956 -0.906613 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Toe0" "ValveBiped.Bip01_L_Foot" 6.879458 0 0 2.831042 -33.495514 -3.654004 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Thigh" "ValveBiped.Bip01_Pelvis" -3.890452 0 0.000007 -6.255897 -90.309895 -89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Calf" "ValveBiped.Bip01_R_Thigh" 17.593161 0 0 0 16.011994 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf" 17.699577 0 0 2.33062 -66.226587 2.220957 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Toe0" "ValveBiped.Bip01_R_Foot" 6.879457 0 0 -3.168915 -33.46874 -1.473704 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine" "ValveBiped.Bip01_Pelvis" 0.000002 3.345116 -2.981992 4.944339 89.999982 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine1" "ValveBiped.Bip01_Spine" 4.126713 0 0 0 5.074516 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine2" "ValveBiped.Bip01_Spine1" 3.613476 0 0 0 7.43745 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine4" "ValveBiped.Bip01_Spine2" 9.183792 0 0 0 20.151327 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Clavicle" "ValveBiped.Bip01_Spine4" 1.673229 1.247213 2.595464 -73.711914 168.587701 91.510847 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_UpperArm" "ValveBiped.Bip01_L_Clavicle" 6.028144 0.000008 0 -12.912918 -32.801758 -101.833749 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_L_UpperArm" 11.692547 0 0.000004 -0.000012 0.000001 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm" 12.07555 0 -0.000031 -7.742161 1.798745 -178.061242 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand" 5.564224 -0.557981 -0.100456 -51.159992 -75.906112 -14.492082 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3" 5.508575 0.000002 0 -0.128418 -78.099976 0.000005 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31" 3.800001 0.000004 0 4.526082 -66.592999 0.000015 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand" 5.744635 0.55798 0.100456 -61.3694 -58.717675 8.796251 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2" 6.052544 -0.000001 -0.000002 -23.542214 -84.10794 0.000013 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21" 3.325016 0 -0.000002 -2.203884 -59.043441 0.000007 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand" 5.613007 1.658157 0.708443 -70.499471 -87.097022 76.889598 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1" 5.643143 0.000002 0.000002 -51.519625 -88.553471 0.000027 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11" 3.079119 0 0 -7.424499 -67.370814 0.000007 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Clavicle" "ValveBiped.Bip01_Spine4" 1.673225 1.247219 -2.595471 73.711914 168.587141 -95.626931 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_UpperArm" "ValveBiped.Bip01_R_Clavicle" 6.028143 0 -0.000004 15.946169 -31.150751 105.625228 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm" 11.69256 0 0 -0.000017 -0.000003 0.000002 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm" 12.075578 -0.000001 0 3.975621 5.609829 -177.6521 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger3" "ValveBiped.Bip01_R_Hand" 5.943747 -0.602343 0.006435 74.957945 -83.501535 28.170693 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger31" "ValveBiped.Bip01_R_Finger3" 5.043457 0.000002 0.000001 -13.470011 -76.728515 -0.000006 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger32" "ValveBiped.Bip01_R_Finger31" 3.384895 -0.000004 0.000002 -9.064312 -97.579559 0.000001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand" 6.213902 0.602343 -0.006447 74.259155 -34.667564 -19.527019 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger21" "ValveBiped.Bip01_R_Finger2" 5.536304 0 0.000002 65.687206 -54.304512 0.000033 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger22" "ValveBiped.Bip01_R_Finger21" 3.312716 0.000008 0 29.106206 -76.658198 -0.000005 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger1" "ValveBiped.Bip01_R_Hand" 5.899515 1.775719 -0.435097 84.408188 -51.263032 -93.162148 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger11" "ValveBiped.Bip01_R_Finger1" 5.285244 -0.000003 0.000005 68.230578 26.411795 -0.000016 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger12" "ValveBiped.Bip01_R_Finger11" 3.098848 0 -0.000004 49.308976 76.043693 0.000013 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger0" "ValveBiped.Bip01_R_Hand" 4.015268 1.836178 -0.578133 61.681943 75.212349 147.088766 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger01" "ValveBiped.Bip01_R_Finger0" 2.400002 0 0 -42.749994 0.000026 0.000005 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger02" "ValveBiped.Bip01_R_Finger01" 1.400002 -0.000002 0.000001 -52.874991 0.000006 -0.000024 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger0" "ValveBiped.Bip01_L_Hand" 3.420517 1.944043 0.26754 -56.416164 68.84942 -156.975902 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger01" "ValveBiped.Bip01_L_Finger0" 2.52499 -0.000006 0.000002 38.249975 -0.000024 0.00001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger02" "ValveBiped.Bip01_L_Finger01" 1.581997 -0.000002 0 48.374992 -0.000013 -0.000007 0 0 0 0 0 0
$definebone "ValveBiped.HC_Body_Bone" "ValveBiped.Bip01_Spine4" 7.856869 0.357134 2.41031 -9.085851 72.317298 154.900057 0 0 0 0 0 0
$definebone "ValveBiped.HC_Head_Bone" "ValveBiped.HC_Body_Bone" 3.738598 -2.965122 -0.136398 -4.50001 -4.500013 12.374974 0 0 0 0 0 0
$definebone "ValveBiped.HC_Rear_Bone" "ValveBiped.HC_Body_Bone" -7.842794 -1.678413 -0.934004 0.000002 20.249981 29.250012 0 0 0 0 0 0
$definebone "ValveBiped.HC_ThighL_Bone" "ValveBiped.HC_Rear_Bone" -0.450933 -0.675705 -6.164913 -30.718465 -70.093389 -172.984649 0 0 0 0 0 0
$definebone "ValveBiped.HC_CalfL_Bone" "ValveBiped.HC_ThighL_Bone" 6.098808 0 0.000001 -0.000007 -0.000012 0 0 0 0 0 0 0
$definebone "ValveBiped.HC_ThighR_Bone" "ValveBiped.HC_Rear_Bone" -0.450914 -0.675713 6.164913 -7.079962 -90.538645 174.066278 0 0 0 0 0 0
$definebone "ValveBiped.HC_CalfR_Bone" "ValveBiped.HC_ThighR_Bone" 6.098812 -0.000001 0 0.000005 -130.088644 0.000001 0 0 0 0 0 0
$definebone "ValveBiped.HC_UpperArmL_Bone" "ValveBiped.HC_Body_Bone" 2.630714 -3.155087 -5.016714 17.056888 -28.79616 1.880336 0 0 0 0 0 0
$definebone "ValveBiped.HC_ForeArmL_Bone" "ValveBiped.HC_UpperArmL_Bone" 8.255674 0.000004 0 0.000003 -121.038766 0.000001 0 0 0 0 0 0
$definebone "ValveBiped.HC_UpperArmR_Bone" "ValveBiped.HC_Body_Bone" 2.630415 -3.15509 5.016869 -4.143291 -63.369628 -131.616994 0 0 0 0 0 0
$definebone "ValveBiped.HC_ForeArmR_Bone" "ValveBiped.HC_UpperArmR_Bone" 8.255676 0 0 0.000017 110.674589 -0.000003 0 0 0 0 0 0


$sectionframes 30 119

$poseparameter "move_yaw" -180 180 loop 360

$ikchain "rhand" "ValveBiped.Bip01_R_Hand" knee 0.707 0.707 0
$ikchain "lhand" "ValveBiped.Bip01_L_Hand" knee 0.707 0.707 0
$ikchain "rfoot" "ValveBiped.Bip01_R_Foot" knee 0.707 -0.707 0
$ikchain "lfoot" "ValveBiped.Bip01_L_Foot" knee 0.707 -0.707 0

$ikautoplaylock "rfoot" 1 0.1
$ikautoplaylock "lfoot" 1 0.1

$weightlist "weights_flinch_headinDelta" {
	 "ValveBiped.Bip01_Pelvis" 1
	 "ValveBiped.Bip01_L_Thigh" 0
	 "ValveBiped.Bip01_L_Calf" 0
	 "ValveBiped.Bip01_L_Foot" 0
	 "ValveBiped.Bip01_L_Toe0" 0
	 "ValveBiped.Bip01_R_Thigh" 0
	 "ValveBiped.Bip01_R_Calf" 0
	 "ValveBiped.Bip01_R_Foot" 0
	 "ValveBiped.Bip01_R_Toe0" 0
	 "ValveBiped.Bip01_Spine" 1
	 "ValveBiped.Bip01_Spine1" 1
	 "ValveBiped.Bip01_Spine2" 1
	 "ValveBiped.Bip01_Spine4" 1
	 "ValveBiped.Bip01_L_Clavicle" 1
	 "ValveBiped.Bip01_L_UpperArm" 1
	 "ValveBiped.Bip01_L_Forearm" 1
	 "ValveBiped.Bip01_L_Hand" 1
	 "ValveBiped.Bip01_L_Finger3" 1
	 "ValveBiped.Bip01_L_Finger31" 1
	 "ValveBiped.Bip01_L_Finger32" 1
	 "ValveBiped.Bip01_L_Finger2" 1
	 "ValveBiped.Bip01_L_Finger21" 1
	 "ValveBiped.Bip01_L_Finger22" 1
	 "ValveBiped.Bip01_L_Finger1" 1
	 "ValveBiped.Bip01_L_Finger11" 1
	 "ValveBiped.Bip01_L_Finger12" 1
	 "ValveBiped.Bip01_R_Clavicle" 1
	 "ValveBiped.Bip01_R_UpperArm" 1
	 "ValveBiped.Bip01_R_Forearm" 1
	 "ValveBiped.Bip01_R_Hand" 1
	 "ValveBiped.Bip01_R_Finger3" 1
	 "ValveBiped.Bip01_R_Finger31" 1
	 "ValveBiped.Bip01_R_Finger32" 1
	 "ValveBiped.Bip01_R_Finger2" 1
	 "ValveBiped.Bip01_R_Finger21" 1
	 "ValveBiped.Bip01_R_Finger22" 1
	 "ValveBiped.Bip01_R_Finger1" 1
	 "ValveBiped.Bip01_R_Finger11" 1
	 "ValveBiped.Bip01_R_Finger12" 1
	 "ValveBiped.Bip01_R_Finger0" 1
	 "ValveBiped.Bip01_R_Finger01" 1
	 "ValveBiped.Bip01_R_Finger02" 1
	 "ValveBiped.Bip01_L_Finger0" 1
	 "ValveBiped.Bip01_L_Finger01" 1
	 "ValveBiped.Bip01_L_Finger02" 1
	 "ValveBiped.HC_Body_Bone" 1
	 "ValveBiped.HC_Head_Bone" 1
	 "ValveBiped.HC_Rear_Bone" 1
	 "ValveBiped.HC_ThighL_Bone" 1
	 "ValveBiped.HC_CalfL_Bone" 1
	 "ValveBiped.HC_ThighR_Bone" 1
	 "ValveBiped.HC_CalfR_Bone" 1
	 "ValveBiped.HC_UpperArmL_Bone" 1
	 "ValveBiped.HC_ForeArmL_Bone" 1
	 "ValveBiped.HC_UpperArmR_Bone" 1
	 "ValveBiped.HC_ForeArmR_Bone" 1
}

$weightlist "weights_flinch_head" {
	 "ValveBiped.Bip01_Pelvis" 0
	 "ValveBiped.Bip01_L_Thigh" 0
	 "ValveBiped.Bip01_L_Calf" 0
	 "ValveBiped.Bip01_L_Foot" 0
	 "ValveBiped.Bip01_L_Toe0" 0
	 "ValveBiped.Bip01_R_Thigh" 0
	 "ValveBiped.Bip01_R_Calf" 0
	 "ValveBiped.Bip01_R_Foot" 0
	 "ValveBiped.Bip01_R_Toe0" 0
	 "ValveBiped.Bip01_Spine" 0
	 "ValveBiped.Bip01_Spine1" 0
	 "ValveBiped.Bip01_Spine2" 0
	 "ValveBiped.Bip01_Spine4" 0
	 "ValveBiped.Bip01_L_Clavicle" 0
	 "ValveBiped.Bip01_L_UpperArm" 0
	 "ValveBiped.Bip01_L_Forearm" 0
	 "ValveBiped.Bip01_L_Hand" 0
	 "ValveBiped.Bip01_L_Finger3" 0
	 "ValveBiped.Bip01_L_Finger31" 0
	 "ValveBiped.Bip01_L_Finger32" 0
	 "ValveBiped.Bip01_L_Finger2" 0
	 "ValveBiped.Bip01_L_Finger21" 0
	 "ValveBiped.Bip01_L_Finger22" 0
	 "ValveBiped.Bip01_L_Finger1" 0
	 "ValveBiped.Bip01_L_Finger11" 0
	 "ValveBiped.Bip01_L_Finger12" 0
	 "ValveBiped.Bip01_R_Clavicle" 0
	 "ValveBiped.Bip01_R_UpperArm" 0
	 "ValveBiped.Bip01_R_Forearm" 0
	 "ValveBiped.Bip01_R_Hand" 0
	 "ValveBiped.Bip01_R_Finger3" 0
	 "ValveBiped.Bip01_R_Finger31" 0
	 "ValveBiped.Bip01_R_Finger32" 0
	 "ValveBiped.Bip01_R_Finger2" 0
	 "ValveBiped.Bip01_R_Finger21" 0
	 "ValveBiped.Bip01_R_Finger22" 0
	 "ValveBiped.Bip01_R_Finger1" 0
	 "ValveBiped.Bip01_R_Finger11" 0
	 "ValveBiped.Bip01_R_Finger12" 0
	 "ValveBiped.Bip01_R_Finger0" 0
	 "ValveBiped.Bip01_R_Finger01" 0
	 "ValveBiped.Bip01_R_Finger02" 0
	 "ValveBiped.Bip01_L_Finger0" 0
	 "ValveBiped.Bip01_L_Finger01" 0
	 "ValveBiped.Bip01_L_Finger02" 0
	 "ValveBiped.HC_Body_Bone" 0
	 "ValveBiped.HC_Head_Bone" 0
	 "ValveBiped.HC_Rear_Bone" 0
	 "ValveBiped.HC_ThighL_Bone" 0
	 "ValveBiped.HC_CalfL_Bone" 0
	 "ValveBiped.HC_ThighR_Bone" 0
	 "ValveBiped.HC_CalfR_Bone" 0
	 "ValveBiped.HC_UpperArmL_Bone" 0
	 "ValveBiped.HC_ForeArmL_Bone" 0
	 "ValveBiped.HC_UpperArmR_Bone" 0
	 "ValveBiped.HC_ForeArmR_Bone" 0
}

$animation "@flinch_headinDelta_corrective_animation" "slow_main_anims\@flinch_headinDelta_corrective_animation.smd" {
}

$animation "@flinch_headCoreDelta_corrective_animation" "slow_main_anims\@flinch_headCoreDelta_corrective_animation.smd" {
}

$animation "@flinch_headoutDelta_corrective_animation" "slow_main_anims\@flinch_headoutDelta_corrective_animation.smd" {
}

$animation "@flinch_chestinDelta_corrective_animation" "slow_main_anims\@flinch_chestinDelta_corrective_animation.smd" {
}

$animation "@flinch_chestCoreDelta_corrective_animation" "slow_main_anims\@flinch_chestCoreDelta_corrective_animation.smd" {
}

$animation "@flinch_chestoutDelta_corrective_animation" "slow_main_anims\@flinch_chestoutDelta_corrective_animation.smd" {
}

$animation "@flinch_leftArminDelta_corrective_animation" "slow_main_anims\@flinch_leftArminDelta_corrective_animation.smd" {
}

$animation "@flinch_leftArmCoreDelta_corrective_animation" "slow_main_anims\@flinch_leftArmCoreDelta_corrective_animation.smd" {
}

$animation "@flinch_leftArmoutDelta_corrective_animation" "slow_main_anims\@flinch_leftArmoutDelta_corrective_animation.smd" {
}

$animation "@flinch_rightArminDelta_corrective_animation" "slow_main_anims\@flinch_rightArminDelta_corrective_animation.smd" {
}

$animation "@flinch_rightArmCoreDelta_corrective_animation" "slow_main_anims\@flinch_rightArmCoreDelta_corrective_animation.smd" {
}

$animation "@flinch_rightArmoutDelta_corrective_animation" "slow_main_anims\@flinch_rightArmoutDelta_corrective_animation.smd" {
}

$animation "a_walk1" "slow_main_anims\a_walk1.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 5 fakeorigin 4.79 5.9 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 44 5 15 46 target 3
	ikrule "lfoot" footstep contact 28 fakeorigin 3.99 7.84 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 21 28 38 44 target 3
	ikrule "rfoot" footstep contact 16 fakeorigin 9.02 -7.74 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 12 16 28 33 target 2
	ikrule "rfoot" footstep contact 39 fakeorigin 11.74 -6.89 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 33 39 8 46 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 46 46 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 46 46 target 1
	walkframe 46 LX LY
}

$animation "a_walk2" "slow_main_anims\a_walk2.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 7 fakeorigin 14.12 6.89 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 2 7 23 27 target 3
	ikrule "lfoot" footstep contact 31 fakeorigin 9.51 7.75 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 27 31 43 46 target 3
	ikrule "rfoot" footstep contact 21 fakeorigin 3.79 -5.78 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 15 21 31 37 target 2
	ikrule "rfoot" footstep contact 43 fakeorigin 4.49 -7.85 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 37 43 10 46 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 46 46 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 46 46 target 1
	walkframe 46 LX LY
}

$animation "a_walk3" "slow_main_anims\a_walk3.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 7 fakeorigin 5.42 5.76 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 7 21 26 target 3
	ikrule "lfoot" footstep contact 32 fakeorigin 4.3 7.83 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 26 32 43 50 target 3
	ikrule "rfoot" footstep contact 18 fakeorigin 10.97 -7.75 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 14 18 31 35 target 2
	ikrule "rfoot" footstep contact 40 fakeorigin 18.88 -6.9 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 35 40 10 50 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 1
	walkframe 50 LX LY
}

$animation "a_Walk4" "slow_main_anims\a_Walk4.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 3 fakeorigin 7.21 8.6 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 71 3 10 72 target 3
	ikrule "lfoot" footstep contact 22 fakeorigin 7.93 11.18 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 16 22 29 36 target 3
	ikrule "lfoot" footstep contact 43 fakeorigin 7.1 8.66 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 36 43 50 54 target 3
	ikrule "lfoot" footstep contact 58 fakeorigin 6.98 10.37 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 54 58 67 71 target 3
	ikrule "rfoot" footstep contact 16 fakeorigin 2.76 -6.7 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 12 16 20 24 target 2
	ikrule "rfoot" footstep contact 29 fakeorigin 12.64 -5.69 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 24 29 41 45 target 2
	ikrule "rfoot" footstep contact 50 fakeorigin 10.57 -6.71 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 45 50 55 61 target 2
	ikrule "rfoot" footstep contact 68 fakeorigin 3.2 -5.17 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 61 68 9 72 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 72 72 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 72 72 target 1
	walkframe 72 LX LY
}

$animation "a_WalkS" "slow_main_anims\a_WalkS.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 6 fakeorigin -6.98 5.93 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 44 6 15 45 target 3
	ikrule "lfoot" footstep contact 27 fakeorigin -6.63 5.9 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 21 27 37 44 target 3
	ikrule "rfoot" footstep contact 21 fakeorigin -11.53 -9.61 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 13 21 28 32 target 2
	ikrule "rfoot" footstep contact 36 fakeorigin -14.81 -5.02 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 32 36 4 45 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1
	walkframe 45 LX LY
}

$animation "a_WalkSE" "slow_main_anims\a_WalkSE.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 6 fakeorigin -8.62 0.49 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 44 6 15 45 target 3
	ikrule "lfoot" footstep contact 27 fakeorigin -3.48 -2.69 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 21 27 37 44 target 3
	ikrule "rfoot" footstep contact 20 fakeorigin -4.07 -18.7 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 12 20 26 32 target 2
	ikrule "rfoot" footstep contact 37 fakeorigin -7.29 -18.43 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 32 37 4 45 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1
	walkframe 45 LX LY
}

$animation "a_WalkE" "slow_main_anims\a_WalkE.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 6 fakeorigin 4.19 0.83 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 43 6 14 45 target 3
	ikrule "lfoot" footstep contact 27 fakeorigin 6.36 -4.94 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 21 27 35 43 target 3
	ikrule "rfoot" footstep contact 16 fakeorigin -5.91 -18.92 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 10 16 26 32 target 2
	ikrule "rfoot" footstep contact 38 fakeorigin -6.19 -17.44 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 32 38 4 45 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1
	walkframe 45 LX LY
}

$animation "a_WalkNE" "slow_main_anims\a_WalkNE.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 7 fakeorigin 2.26 1.98 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 44 7 15 45 target 3
	ikrule "lfoot" footstep contact 28 fakeorigin 4.64 -1.67 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 22 28 35 44 target 3
	ikrule "rfoot" footstep contact 17 fakeorigin 4.12 -18.89 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 12 17 26 32 target 2
	ikrule "rfoot" footstep contact 38 fakeorigin 13.53 -14.06 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 32 38 6 45 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1
	walkframe 45 LX LY
}

$animation "a_WalkNW" "slow_main_anims\a_WalkNW.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 7 fakeorigin 7.81 9.75 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 44 7 15 45 target 3
	ikrule "lfoot" footstep contact 28 fakeorigin 6.01 8.38 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 22 28 35 44 target 3
	ikrule "rfoot" footstep contact 17 fakeorigin 6.13 -0.42 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 11 17 26 32 target 2
	ikrule "rfoot" footstep contact 38 fakeorigin 15.67 -3.12 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 32 38 4 45 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1
	walkframe 45 LX LY
}

$animation "a_WalkW" "slow_main_anims\a_WalkW.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 7 fakeorigin 4.21 13.25 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 44 7 15 45 target 3
	ikrule "lfoot" footstep contact 28 fakeorigin 3.11 11.47 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 22 28 35 44 target 3
	ikrule "rfoot" footstep contact 17 fakeorigin -4.94 4.14 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 11 17 26 32 target 2
	ikrule "rfoot" footstep contact 38 fakeorigin -2.76 3.11 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 32 38 4 45 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1
	walkframe 45 LX LY
}

$animation "a_WalkSW" "slow_main_anims\a_WalkSW.smd" {
	fps 24
	loop
	ikrule "lfoot" footstep contact 7 fakeorigin -4.36 14.1 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 44 7 15 45 target 3
	ikrule "lfoot" footstep contact 28 fakeorigin 2.79 10.14 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 22 28 35 44 target 3
	ikrule "rfoot" footstep contact 17 fakeorigin -16.93 -0.81 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 11 17 26 32 target 2
	ikrule "rfoot" footstep contact 38 fakeorigin -12.26 -2.48 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 32 38 4 45 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1
	walkframe 45 LX LY
}

$sequence "Idle01" {
	"slow_main_anims\Idle01.smd"
	activity "ACT_IDLE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "lfoot" footstep contact 0 fakeorigin -3.64 8.01 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 53 53 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.2 -11.82 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 53 53 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 53 53 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 53 53 target 1
}

$sequence "walk" {
	"a_WalkS"
	"a_WalkSE"
	"a_WalkE"
	"a_WalkNE"
	"a_walk1"
	"a_WalkNW"
	"a_WalkW"
	"a_WalkSW"
	"a_WalkS"
	activity "ACT_WALK" 1
	blend "move_yaw" -180 180
	blendwidth 9
	{ event 1100 5 "step" }
	{ event 1100 15 "step" }
	{ event 1100 20 "scuff" }
	{ event 1100 28 "step" }
	{ event 1100 38 "step" }
	{ event 1100 40 "scuff" }
	fadein 0.2
	fadeout 0.2
}

$sequence "walk2" {
	"a_WalkS"
	"a_WalkSE"
	"a_WalkE"
	"a_WalkNE"
	"a_walk2"
	"a_WalkNW"
	"a_WalkW"
	"a_WalkSW"
	"a_WalkS"
	activity "ACT_WALK" 1
	blend "move_yaw" -180 180
	blendwidth 9
	{ event 1100 7 "step" }
	{ event 1100 10 "scuff" }
	{ event 1100 21 "step" }
	{ event 1100 31 "step" }
	{ event 1100 34 "scuff" }
	{ event 1100 44 "step" }
	fadein 0.2
	fadeout 0.2
}

$sequence "walk3" {
	"a_WalkS"
	"a_WalkSE"
	"a_WalkE"
	"a_WalkNE"
	"a_walk3"
	"a_WalkNW"
	"a_WalkW"
	"a_WalkSW"
	"a_WalkS"
	activity "ACT_WALK" 1
	blend "move_yaw" -180 180
	blendwidth 9
	{ event 1100 7 "step" }
	{ event 1100 18 "step" }
	{ event 1100 21 "scuff" }
	{ event 1100 32 "step" }
	{ event 1100 40 "step" }
	{ event 1100 44 "scuff" }
	fadein 0.2
	fadeout 0.2
}

$sequence "walk4" {
	"a_Walk4"
	activity "ACT_WALK" 1
	{ event 1100 4 "step" }
	{ event 1100 16 "step" }
	{ event 1100 22 "step" }
	{ event 1100 28 "step" }
	{ event 1100 43 "step" }
	{ event 1100 49 "step" }
	{ event 1100 58 "step" }
	{ event 1100 67 "step" }
	fadein 0.2
	fadeout 0.2
}

$sequence "FireWalk" {
	"slow_main_anims\FireWalk.smd"
	activity "ACT_WALK_ON_FIRE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "lfoot" footstep contact 2 fakeorigin 3.04 9.12 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 25 2 18 30 target 3 usesequence 
	ikrule "rfoot" footstep contact 17 fakeorigin 1.91 -7.14 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 10 17 4 30 target 2 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1 usesequence 
	walkframe 30 LX LY
}

$sequence "FireIdle" {
	"slow_main_anims\FireIdle.smd"
	activity "ACT_IDLE_ON_FIRE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "lfoot" footstep contact 0 fakeorigin -3.91 1.69 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 30 30 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.47 -18.15 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 30 30 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1
}

$sequence "attackA" {
	"slow_main_anims\attackA.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee" }
	{ event 1100 10 "AE_NPC_ATTACK_BROADCAST" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.64 7.81 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 22 34 target 3 usesequence 
	ikrule "lfoot" footstep contact 46 fakeorigin -3.06 8.13 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 34 46 56 56 target 3 usesequence 
	ikrule "rfoot" footstep contact 0 fakeorigin -1.2 -12.03 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 22 29 target 2 usesequence 
	ikrule "rfoot" footstep contact 36 fakeorigin 3.35 -10.14 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 29 36 56 56 target 2 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 1 usesequence 
	walkframe 47 LX LY
}

$sequence "attackB" {
	"slow_main_anims\attackB.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee" }
	{ event 1100 10 "AE_NPC_ATTACK_BROADCAST" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.64 7.81 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 56 56 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.2 -12.03 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 56 56 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 1
}

$sequence "attackC" {
	"slow_main_anims\attackC.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee" }
	{ event 1100 10 "AE_NPC_ATTACK_BROADCAST" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -4.1 7.8 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 22 28 target 3 usesequence 
	ikrule "lfoot" footstep contact 34 fakeorigin 2.04 6.42 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 28 34 56 56 target 3 usesequence 
	ikrule "rfoot" footstep contact 0 fakeorigin -1.52 -12.02 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 22 34 target 2 usesequence 
	ikrule "rfoot" footstep contact 46 fakeorigin -1.43 -13.36 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 34 46 56 56 target 2 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 1 usesequence 
	walkframe 47 LX LY
}

$sequence "attackD" {
	"slow_main_anims\attackD.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee" }
	{ event 1100 10 "AE_NPC_ATTACK_BROADCAST" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.7 7.74 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 56 56 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.17 -12.17 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 56 56 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 1
}

$sequence "attackE" {
	"slow_main_anims\attackE.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee_heavy" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.7 7.74 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 41 41 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.17 -12.17 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 6 15 target 2
	ikrule "rfoot" footstep contact 23 fakeorigin 10.69 -15.06 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 15 23 32 35 target 2
	ikrule "rfoot" footstep contact 38 fakeorigin -1.17 -12.18 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 35 38 41 41 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 1
}

$sequence "attackF" {
	"slow_main_anims\attackF.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee_heavy" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.98 7.69 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 41 41 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.03 -12.13 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 41 41 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 1
}

$sequence "swatleftmid" {
	"slow_main_anims\swatleftmid.smd"
	activity "ACT_ZOM_SWATLEFTMID" 1
	{ event 1100 1 "AE_ZOMBIE_STARTSWAT" }
	{ event 1100 9 "AE_ZOMBIE_SWATITEM" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.7 7.74 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 36 36 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.17 -12.17 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 36 36 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 1
}

$sequence "swatrightmid" {
	"slow_main_anims\swatrightmid.smd"
	activity "ACT_ZOM_SWATRIGHTMID" 1
	{ event 1100 1 "AE_ZOMBIE_STARTSWAT" }
	{ event 1100 9 "AE_ZOMBIE_SWATITEM" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.64 7.81 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 36 36 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.2 -12.03 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 36 36 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 1
}

$sequence "swatleftlow" {
	"slow_main_anims\swatleftlow.smd"
	activity "ACT_ZOM_SWATLEFTLOW" 1
	{ event 1100 1 "AE_ZOMBIE_STARTSWAT" }
	{ event 1100 15 "AE_ZOMBIE_SWATITEM" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -4.1 7.8 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 17 22 target 3 usesequence 
	ikrule "lfoot" footstep contact 26 fakeorigin 0.03 5.06 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 22 26 45 45 target 3 usesequence 
	ikrule "rfoot" footstep contact 0 fakeorigin -1.52 -12.02 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 17 28 target 2 usesequence 
	ikrule "rfoot" footstep contact 39 fakeorigin -1.4 -13.47 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 28 39 45 45 target 2 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1 usesequence 
	walkframe 36 LX LY
}

$sequence "swatrightlow" {
	"slow_main_anims\swatrightlow.smd"
	activity "ACT_ZOM_SWATRIGHTLOW" 1
	{ event 1100 1 "AE_ZOMBIE_STARTSWAT" }
	{ event 1100 18 "AE_ZOMBIE_SWATITEM" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.64 7.81 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 15 27 target 3 usesequence 
	ikrule "lfoot" footstep contact 38 fakeorigin -3.44 8.66 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 27 38 47 47 target 3 usesequence 
	ikrule "rfoot" footstep contact 0 fakeorigin -1.2 -12.03 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 23 26 target 2 usesequence 
	ikrule "rfoot" footstep contact 29 fakeorigin 2.12 -9.56 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 26 29 47 47 target 2 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 47 47 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 47 47 target 1 usesequence 
	walkframe 38 LX LY
}

$sequence "releasecrab" {
	"slow_main_anims\releasecrab.smd"
	activity "ACT_ZOM_RELEASECRAB" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.64 7.81 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 30 30 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.2 -12.03 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 30 30 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1
}

$sequence "physflinch1" {
	"slow_main_anims\physflinch1.smd"
	activity "ACT_FLINCH_PHYSICS" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.39 7.51 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 18 18 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.12 -11.62 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 18 18 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 18 18 target 1
}

$sequence "physflinch2" {
	"slow_main_anims\physflinch2.smd"
	activity "ACT_FLINCH_PHYSICS" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.65 8.02 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 20 20 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.21 -11.82 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 20 20 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
}

$sequence "physflinch3" {
	"slow_main_anims\physflinch3.smd"
	activity "ACT_FLINCH_PHYSICS" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -3.65 8.02 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 20 20 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -1.21 -11.82 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 20 20 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
}

$sequence "smashfall64" {
	"slow_main_anims\smashfall64.smd"
	{ event 1100 10 "AE_NPC_BODYDROP_HEAVY" }
	{ event 1100 27 "AE_NPC_BODYDROP_HEAVY" }
	{ event 1100 37 "AE_NPC_BODYDROP_HEAVY" }
	{ event 1100 75 "get_up" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 120 120 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 120 120 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 120 120 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 120 120 target 3
}

$sequence "slump_a" {
	"slow_main_anims\slump_a.smd"
	activity "ACT_IDLE_ANGRY" -1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 3
}

$sequence "slumprise_a" {
	"slow_main_anims\slumprise_a.smd"
	activity "ACT_SPECIAL_ATTACK1" 5
	{ event 1100 1 "sound_alert" }
	{ event 1100 15 "step" }
	{ event 1100 50 "step" }
	{ event 1100 80 "get_up" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 127 127 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 127 127 target 1 usesequence 
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 127 127 target 2 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 127 127 target 3 usesequence 
	walkframe 68
	walkframe 127 LX LY
}

$sequence "slumprise_a_attack" {
	"slow_main_anims\slumprise_a_attack.smd"
	activity "ACT_SPECIAL_ATTACK1" 1
	{ event 1100 1 "sound_alert" }
	{ event 1100 15 "step" }
	{ event 1100 50 "step" }
	{ event 1100 80 "get_up" }
	{ event 1100 16 "sound_attack_scream" }
	{ event 1100 36 "melee" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 127 127 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 127 127 target 1 usesequence 
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 127 127 target 2 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 127 127 target 3 usesequence 
	walkframe 68
	walkframe 127 LX LY
}

$sequence "slumprise_a2" {
	"slow_main_anims\slumprise_a2.smd"
	activity "ACT_SPECIAL_ATTACK1" 3
	{ event 1100 1 "sound_alert" }
	{ event 1100 16 "step" }
	{ event 1100 22 "step" }
	{ event 1100 24 "get_up" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 68 68 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 68 68 target 1 usesequence 
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 68 68 target 2 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 68 68 target 3 usesequence 
	walkframe 68 LX LY
}

$sequence "slump_b" {
	"slow_main_anims\slump_b.smd"
	activity "ACT_IDLE_STIMULATED" -1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 14 14 target 3
}

$sequence "slumprise_b" {
	"slow_main_anims\slumprise_b.smd"
	activity "ACT_SPECIAL_ATTACK2" -1
	{ event 1100 1 "sound_alert" }
	{ event 1100 13 "step" }
	{ event 1100 27 "step" }
	{ event 1100 57 "get_up" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 101 101 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 101 101 target 1 usesequence 
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 101 101 target 2 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 101 101 target 3 usesequence 
	walkframe 101 LX LY
}

$sequence "Breakthrough" {
	"slow_main_anims\Breakthrough.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 63 63 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 63 63 target 1 usesequence 
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 63 63 target 2 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 63 63 target 3 usesequence 
	walkframe 54 LX LY
}

$sequence "canal5await" {
	"slow_main_anims\canal5await.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 3
}

$sequence "canal5aattack" {
	"slow_main_anims\canal5aattack.smd"
	fadein 0.2
	fadeout 0.2
	snap
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 3
}

$sequence "flinch_headinDelta" {
	"slow_main_anims\flinch_headinDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_headinDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_headinFrame" {
	"slow_main_anims\flinch_headinFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_headCoreDelta" {
	"slow_main_anims\flinch_headCoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_headCoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_headOutFrame" {
	"slow_main_anims\flinch_headOutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_headoutDelta" {
	"slow_main_anims\flinch_headoutDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_headoutDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_head" {
	"slow_main_anims\flinch_head.smd"
	activity "ACT_GESTURE_FLINCH_HEAD" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch_headinFrame" 0 2 2 3 xfade spline
	blendlayer "flinch_headOutFrame" 2 3 3 34 spline
	blendlayer "flinch_headinDelta" 0 0 2 2
	blendlayer "flinch_headCoreDelta" 2 2 3 3
	blendlayer "flinch_headoutDelta" 3 3 35 34
	fps 30
	weightlist "weights_flinch_head"
}

$sequence "flinch_chestinDelta" {
	"slow_main_anims\flinch_chestinDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_chestinDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 5 5 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 5 5 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_chestinFrame" {
	"slow_main_anims\flinch_chestinFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_chestCoreDelta" {
	"slow_main_anims\flinch_chestCoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_chestCoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 13 13 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 13 13 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_chestOutFrame" {
	"slow_main_anims\flinch_chestOutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_chestoutDelta" {
	"slow_main_anims\flinch_chestoutDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_chestoutDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 7 7 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 7 7 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_chest" {
	"slow_main_anims\flinch_chest.smd"
	activity "ACT_GESTURE_FLINCH_CHEST" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch_chestinFrame" 0 5 5 18 xfade spline
	blendlayer "flinch_chestOutFrame" 5 18 18 24 spline
	blendlayer "flinch_chestinDelta" 0 0 5 5
	blendlayer "flinch_chestCoreDelta" 5 5 18 18
	blendlayer "flinch_chestoutDelta" 18 18 25 24
	fps 30
	weightlist "weights_flinch_head"
}

$sequence "flinch_leftArminDelta" {
	"slow_main_anims\flinch_leftArminDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_leftArminDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_leftArminFrame" {
	"slow_main_anims\flinch_leftArminFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_leftArmCoreDelta" {
	"slow_main_anims\flinch_leftArmCoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_leftArmCoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_leftArmOutFrame" {
	"slow_main_anims\flinch_leftArmOutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_leftArmoutDelta" {
	"slow_main_anims\flinch_leftArmoutDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_leftArmoutDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 27 27 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 27 27 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_leftArm" {
	"slow_main_anims\flinch_leftArm.smd"
	activity "ACT_GESTURE_FLINCH_LEFTARM" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch_leftArminFrame" 0 1 1 3 xfade spline
	blendlayer "flinch_leftArmOutFrame" 1 3 3 29 spline
	blendlayer "flinch_leftArminDelta" 0 0 1 1
	blendlayer "flinch_leftArmCoreDelta" 1 1 3 3
	blendlayer "flinch_leftArmoutDelta" 3 3 30 29
	fps 30
	weightlist "weights_flinch_head"
}

$sequence "flinch_rightArminDelta" {
	"slow_main_anims\flinch_rightArminDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_rightArminDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_rightArminFrame" {
	"slow_main_anims\flinch_rightArminFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_rightArmCoreDelta" {
	"slow_main_anims\flinch_rightArmCoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_rightArmCoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_rightArmOutFrame" {
	"slow_main_anims\flinch_rightArmOutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_rightArmoutDelta" {
	"slow_main_anims\flinch_rightArmoutDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_rightArmoutDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 27 27 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 27 27 target 1
	weightlist "weights_flinch_headinDelta"
}

$sequence "flinch_rightArm" {
	"slow_main_anims\flinch_rightArm.smd"
	activity "ACT_GESTURE_FLINCH_RIGHTARM" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch_rightArminFrame" 0 1 1 3 xfade spline
	blendlayer "flinch_rightArmOutFrame" 1 3 3 39 spline
	blendlayer "flinch_rightArminDelta" 0 0 1 1
	blendlayer "flinch_rightArmCoreDelta" 1 1 3 3
	blendlayer "flinch_rightArmoutDelta" 3 3 40 39
	fps 30
	weightlist "weights_flinch_head"
}

$sequence "flinch_leftLeg" {
	"slow_main_anims\flinch_leftLeg.smd"
	activity "ACT_FLINCH_LEFTLEG" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 85 85 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 85 85 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 85 85 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 85 85 target 3
}

$sequence "flinch_rightLeg" {
	"slow_main_anims\flinch_rightLeg.smd"
	activity "ACT_FLINCH_RIGHTLEG" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 58 58 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 58 58 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 58 58 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 58 58 target 3
}

$sequence "alyx_zombie_fight1" {
	"slow_main_anims\alyx_zombie_fight1.smd"
	activity "ACT_DI_ALYX_ZOMBIE_MELEE" 1
	{ event AE_CL_PLAYSOUND 10 "Zombie.Alert" }
	{ event AE_CL_PLAYSOUND 36 "Zombie.Pain" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 75 75 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 75 75 target 1 usesequence 
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 75 75 target 2 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 75 75 target 3 usesequence 
	walkframe 66 LX LY
}

$sequence "alyx_zombie_fight2" {
	"slow_main_anims\alyx_zombie_fight2.smd"
	activity "ACT_DI_ALYX_ZOMBIE_MELEE" 1
	{ event 1100 4 "sound_alert" }
	{ event AE_CL_PLAYSOUND 38 "Zombie.Pain" }
	{ event AE_ZOMBIE_POPHEADCRAB 76 "ValveBiped.Bip01_R_hand 1000" }
	{ event AE_NPC_SET_INTERACTION_CANTDIE 50 "0" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 3
}

$sequence "alyx_zombie_shotgun_fight1" {
	"slow_main_anims\alyx_zombie_shotgun_fight1.smd"
	activity "ACT_DI_ALYX_ZOMBIE_SHOTGUN64" 1
	{ event 1100 4 "sound_alert" }
	{ event AE_CL_PLAYSOUND 23 "Zombie.Pain" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 64 64 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 64 64 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 64 64 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 64 64 target 3
}

$sequence "alyx_zombie_shotgun_fight2" {
	"slow_main_anims\alyx_zombie_shotgun_fight2.smd"
	activity "ACT_DI_ALYX_ZOMBIE_SHOTGUN64" 1
	{ event 1100 4 "sound_alert" }
	{ event AE_CL_PLAYSOUND 23 "Zombie.Pain" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 64 64 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 64 64 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 64 64 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 64 64 target 3
}

$sequence "alyx_zombie_shotgun_fight3" {
	"slow_main_anims\alyx_zombie_shotgun_fight3.smd"
	activity "ACT_DI_ALYX_ZOMBIE_SHOTGUN26" 1
	{ event 1100 4 "sound_alert" }
	{ event AE_CL_PLAYSOUND 17 "Zombie.Pain" }
	{ event AE_ZOMBIE_POPHEADCRAB 17 "ValveBiped.Bip01_R_hand 800" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 3
}

$sequence "alyx_zombie_shotgun_fight4" {
	"slow_main_anims\alyx_zombie_shotgun_fight4.smd"
	activity "ACT_DI_ALYX_ZOMBIE_SHOTGUN26" 1
	{ event 1100 4 "sound_alert" }
	{ event AE_CL_PLAYSOUND 15 "Zombie.Die" }
	{ event AE_CL_PLAYSOUND 45 "Zombie.Pain" }
	{ event AE_NPC_HURT_INTERACTION_PARTNER 48 "ME 200" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 70 70 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 70 70 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 70 70 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 70 70 target 3
}

$sequence "alyx_zombie_kick1" {
	"slow_main_anims\alyx_zombie_kick1.smd"
	activity "ACT_DI_ALYX_ZOMBIE_MELEE" 1
	{ event AE_ZOMBIE_POPHEADCRAB 22 "ValveBiped.Bip01_R_Foot 1500" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 3
}

$sequence "alyx_zombie_grab" {
	"slow_main_anims\alyx_zombie_grab.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 38 38 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 38 38 target 1 usesequence 
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 38 38 target 2 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 38 38 target 3 usesequence 
	walkframe 29 LX LY
}

$sequence "alyx_zombie_grab_PI" {
	"slow_main_anims\alyx_zombie_grab_PI.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 43 43 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 43 43 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 43 43 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 43 43 target 3
}

$sequence "alyx_zombie_grab_exit" {
	"slow_main_anims\alyx_zombie_grab_exit.smd"
	{ event AE_NPC_HURT_INTERACTION_PARTNER 20 "ME 100" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 27 27 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 27 27 target 1 usesequence 
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 27 27 target 2 usesequence 
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 27 27 target 3 usesequence 
	walkframe 18 LX LY
}

$sequence "alyx_zombie_elbow_45L" {
	"slow_main_anims\alyx_zombie_elbow_45L.smd"
	{ event AE_NPC_HURT_INTERACTION_PARTNER 43 "ME 100" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 3
}

$sequence "vort_zombie_attack02" {
	"slow_main_anims\vort_zombie_attack02.smd"
	{ event AE_ZOMBIE_POPHEADCRAB 78 "ValveBiped.Bip01_R_Foot 2500" }
	{ event AE_NPC_HURT_INTERACTION_PARTNER 80 "ME 200" }
	{ event AE_CL_PLAYSOUND 20 "NPC_FastZombie.Pain" }
	{ event AE_CL_PLAYSOUND 51 "Flesh.ImpactHard" }
	{ event AE_CL_PLAYSOUND 78 "GrenadeSpit.Hit" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 119 119 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 119 119 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 119 119 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 119 119 target 3
}

$sequence "vort_zombie_attack" {
	"slow_main_anims\vort_zombie_attack.smd"
	{ event 1100 4 "sound_alert" }
	{ event AE_CL_PLAYSOUND 12 "Zombie.AttackMiss" }
	{ event AE_CL_PLAYSOUND 16 "Zombie.AttackMiss" }
	{ event AE_CL_PLAYSOUND 19 "Zombie.Idle" }
	{ event AE_CL_PLAYSOUND 26 "Zombie.AttackMiss" }
	{ event AE_CL_PLAYSOUND 37 "Zombie.Pain" }
	{ event AE_CL_PLAYSOUND 60 "Flesh_Bloody.ImpactHard" }
	{ event AE_ZOMBIE_POPHEADCRAB 62 "ValveBiped.cleaver_attachment 1800" }
	{ event AE_NPC_HURT_INTERACTION_PARTNER 73 "ME 200" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 97 97 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 97 97 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 97 97 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 97 97 target 3
}

$sequence "Tantrum" {
	"slow_main_anims\Tantrum.smd"
	activity "ACT_ZOMBIE_TANTRUM" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 3
}

$sequence "WallPound" {
	"slow_main_anims\WallPound.smd"
	activity "ACT_ZOMBIE_WALLPOUND" 1
	{ event 1100 14 "wallpound" }
	{ event 1100 35 "wallpound" }
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 53 53 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 53 53 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 53 53 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 53 53 target 3
}

$sequence "ragdoll" {
	"slow_main_anims\ragdoll.smd"
	activity "ACT_DIERAGDOLL" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 3
}

$collisionjoints "slow_main_physics.smd"
{
	$mass 100
	$inertia 10
	$damping 0.1
	$rotdamping 3
	$rootbone "valvebiped.bip01_pelvis"


	$jointconstrain "ValveBiped.Bip01_Spine2" x limit -44 44 0.008
	$jointconstrain "ValveBiped.Bip01_Spine2" y limit -39 39 0.008
	$jointconstrain "ValveBiped.Bip01_Spine2" z limit -32 50 0.008

	$jointconstrain "ValveBiped.Bip01_R_Clavicle" x limit -30 30 0.008
	$jointconstrain "ValveBiped.Bip01_R_Clavicle" y limit -46 46 0.008
	$jointconstrain "ValveBiped.Bip01_R_Clavicle" z limit -26 48 0.008

	$jointconstrain "ValveBiped.Bip01_R_UpperArm" x limit -39 39 0.008
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" y limit -79 95 0.008
	$jointconstrain "ValveBiped.Bip01_R_UpperArm" z limit -93 28 0.008

	$jointconstrain "ValveBiped.Bip01_L_Clavicle" x limit -30 30 0.008
	$jointconstrain "ValveBiped.Bip01_L_Clavicle" y limit -46 46 0.008
	$jointconstrain "ValveBiped.Bip01_L_Clavicle" z limit -26 48 0.008

	$jointconstrain "ValveBiped.Bip01_L_UpperArm" x limit -30 30 0.008
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" y limit -95 84 0.008
	$jointconstrain "ValveBiped.Bip01_L_UpperArm" z limit -86 26 0.008

	$jointconstrain "ValveBiped.Bip01_L_Forearm" x limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_L_Forearm" y limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_L_Forearm" z limit -149 4 0.008

	$jointconstrain "ValveBiped.Bip01_L_Hand" x limit -55 55 0.008
	$jointconstrain "ValveBiped.Bip01_L_Hand" y limit -55 55 0.008
	$jointconstrain "ValveBiped.Bip01_L_Hand" z limit -55 55 0.008

	$jointconstrain "ValveBiped.Bip01_R_Forearm" x limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_R_Forearm" y limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_R_Forearm" z limit -149 4 0.008

	$jointconstrain "ValveBiped.Bip01_R_Thigh" x limit -12 12 0.008
	$jointconstrain "ValveBiped.Bip01_R_Thigh" y limit -8 75 0.008
	$jointconstrain "ValveBiped.Bip01_R_Thigh" z limit -97 32 0.008

	$jointconstrain "ValveBiped.Bip01_R_Calf" x limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_R_Calf" y limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_R_Calf" z limit -12 126 0.008

	$jointconstrain "ValveBiped.Bip01_R_Foot" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_R_Foot" y limit -25 6 0.008
	$jointconstrain "ValveBiped.Bip01_R_Foot" z limit -15 35 0.008

	$jointconstrain "ValveBiped.Bip01_L_Thigh" x limit -12 12 0.008
	$jointconstrain "ValveBiped.Bip01_L_Thigh" y limit -73 6 0.008
	$jointconstrain "ValveBiped.Bip01_L_Thigh" z limit -93 30 0.008

	$jointconstrain "ValveBiped.Bip01_L_Calf" x limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_L_Calf" y limit 0 0 0.008
	$jointconstrain "ValveBiped.Bip01_L_Calf" z limit -8 126 0.008

	$jointconstrain "ValveBiped.Bip01_L_Foot" x limit 0 0 0
	$jointconstrain "ValveBiped.Bip01_L_Foot" y limit -19 19 0.008
	$jointconstrain "ValveBiped.Bip01_L_Foot" z limit -15 35 0.008

	$jointconstrain "ValveBiped.Bip01_R_Hand" x limit -55 55 0.008
	$jointconstrain "ValveBiped.Bip01_R_Hand" y limit -55 55 0.008
	$jointconstrain "ValveBiped.Bip01_R_Hand" z limit -55 55 0.008

	$jointcollide "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_Thigh"
	$jointcollide "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_L_Thigh"
	$jointcollide "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_R_Thigh"
	$jointcollide "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_L_Thigh"
	$jointcollide "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_Pelvis"
	$jointcollide "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_Pelvis"
	$jointcollide "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_L_Calf"
	$jointcollide "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_R_Calf"
	$jointcollide "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_R_Foot"
	$jointcollide "ValveBiped.Bip01_R_Calf" "ValveBiped.Bip01_L_Calf"
	$jointcollide "ValveBiped.Bip01_R_Thigh" "ValveBiped.Bip01_L_Thigh"
	$jointcollide "ValveBiped.Bip01_R_UpperArm" "ValveBiped.Bip01_Spine2"
	$jointcollide "ValveBiped.Bip01_L_UpperArm" "ValveBiped.Bip01_Spine2"
	$jointcollide "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_Spine2"
	$jointcollide "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_Spine2"
}
