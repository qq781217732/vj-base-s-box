// Created by Crowbar 0.67

$modelname "vj_zombies/zombine_anims.mdl"

$surfaceprop "alienflesh"

$contents "solid"

$illumposition -4.087 -1.175 28.104

$cdmaterials "models\zombie_soldier\"

$cbox 0 0 0 0 0 0

$bbox -17.62 -23.464 -2.754 19.969 15.289 58.962

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

$definebone "ValveBiped.Bip01_Pelvis" "" -0.000005 1.516003 38.353996 0 0 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Thigh" "ValveBiped.Bip01_Pelvis" 3.890453 0 -0.000004 -6.298009 -90.530886 -89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Calf" "ValveBiped.Bip01_L_Thigh" 17.59317 0 0 0 16.008153 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Foot" "ValveBiped.Bip01_L_Calf" 17.699581 0 0 -2.794963 -66.211908 -0.906668 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Toe0" "ValveBiped.Bip01_L_Foot" 6.879457 0.000001 0 2.831042 -33.495514 -3.653923 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Thigh" "ValveBiped.Bip01_Pelvis" -3.890452 0 0.000007 -6.255897 -90.309895 -89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Calf" "ValveBiped.Bip01_R_Thigh" 17.593157 0 0 0 16.011995 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Foot" "ValveBiped.Bip01_R_Calf" 17.699577 0 0 2.330621 -66.226587 2.220957 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Toe0" "ValveBiped.Bip01_R_Foot" 6.879459 -0.000001 0 -3.168914 -33.468733 -1.473705 0 0 0 0 0 0
$definebone "ValveBiped.Cod_Bone" "ValveBiped.Bip01_Pelvis" 0.003197 1.916843 7.635199 0 0 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine" "ValveBiped.Bip01_Pelvis" 0.000002 3.345119 -2.981992 4.944283 89.999982 89.999982 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine1" "ValveBiped.Bip01_Spine" 4.126717 0 0 0 5.074513 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine2" "ValveBiped.Bip01_Spine1" 3.613476 0 0 0 7.43745 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_Spine4" "ValveBiped.Bip01_Spine2" 9.183792 0 0 0 20.151326 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Clavicle" "ValveBiped.Bip01_Spine4" 1.673225 1.247211 2.595464 -73.711907 168.587715 91.510847 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_UpperArm" "ValveBiped.Bip01_L_Clavicle" 6.028143 0.000004 -0.000004 -12.912923 -32.801782 -101.833749 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Forearm" "ValveBiped.Bip01_L_UpperArm" 11.692562 0.000001 -0.000004 0.00001 0.000004 0.000001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Hand" "ValveBiped.Bip01_L_Forearm" 12.075584 0 0 -7.742144 1.798748 -178.061242 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger0" "ValveBiped.Bip01_L_Hand" 1.782818 1.657451 0.069283 -48.21646 72.036776 -69.320628 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger01" "ValveBiped.Bip01_L_Finger0" 2.070885 -0.000004 -0.000004 0.000014 -50.40001 -0.000001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger02" "ValveBiped.Bip01_L_Finger01" 1.180882 0 0 0.00001 -29.700012 -0.00001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger3" "ValveBiped.Bip01_L_Hand" 4.942484 -0.87483 0.248177 -17.957532 -6.592456 -88.934684 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger31" "ValveBiped.Bip01_L_Finger3" 1.980927 -0.000004 0 -0.100784 -39.849953 -0.079475 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger32" "ValveBiped.Bip01_L_Finger31" 1.731651 0 0 4.52614 -26.09301 -0.000004 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger2" "ValveBiped.Bip01_L_Hand" 5.040239 0.509705 0.13031 -9.293718 3.242993 -91.830807 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger21" "ValveBiped.Bip01_L_Finger2" 2.156433 0 0 0.000001 -58.950017 -0.000003 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger22" "ValveBiped.Bip01_L_Finger21" 2.243126 0 0 -0.000003 -38.474975 -0.000004 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger1" "ValveBiped.Bip01_L_Hand" 5.046516 1.425083 0.048111 -13.478381 13.535091 -102.595718 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger11" "ValveBiped.Bip01_L_Finger1" 2.069261 0 0 0.000004 -48.150003 -0.000008 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_L_Finger12" "ValveBiped.Bip01_L_Finger11" 2.441578 0.000002 -0.000001 -0.000004 -48.149992 -0.000008 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Clavicle" "ValveBiped.Bip01_Spine4" 1.673229 1.247223 -2.595471 73.711914 168.587141 -95.626931 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_UpperArm" "ValveBiped.Bip01_R_Clavicle" 6.028142 0 0 15.946166 -31.150765 105.625228 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Forearm" "ValveBiped.Bip01_R_UpperArm" 11.692566 0 0.000011 -0.000029 0.000001 0.000006 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Hand" "ValveBiped.Bip01_R_Forearm" 12.07559 0 0.000011 2.648412 2.781881 -87.398179 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger0" "ValveBiped.Bip01_R_Hand" 1.819733 -0.264271 -1.112211 39.081055 -74.649152 -87.270126 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger01" "ValveBiped.Bip01_R_Finger0" 2.070873 0 -0.000002 -0.000031 -50.40001 0.000008 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger02" "ValveBiped.Bip01_R_Finger01" 1.180885 0 0.000002 -0.000022 -29.700022 0.00002 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger3" "ValveBiped.Bip01_R_Hand" 4.958588 -0.225746 1.352891 -7.206952 -18.07077 1.00548 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger31" "ValveBiped.Bip01_R_Finger3" 1.980934 0 -0.000001 0.100782 -39.84996 0.079467 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger32" "ValveBiped.Bip01_R_Finger31" 1.731651 0 0 -4.526141 -26.093005 0.000001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger2" "ValveBiped.Bip01_R_Hand" 5.056065 -0.114651 -0.09717 2.26983 -9.311739 -2.856771 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger21" "ValveBiped.Bip01_R_Finger2" 2.156433 0 0 -0.000004 -58.950004 -0.000001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger22" "ValveBiped.Bip01_R_Finger21" 2.243122 0.000003 0 -0.000004 -38.474958 0.000001 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger1" "ValveBiped.Bip01_R_Hand" 5.062344 -0.036644 -1.012916 13.092598 -13.908178 3.272962 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger11" "ValveBiped.Bip01_R_Finger1" 2.069266 0 0.000001 -0.000012 -48.149982 0 0 0 0 0 0 0
$definebone "ValveBiped.Bip01_R_Finger12" "ValveBiped.Bip01_R_Finger11" 2.441574 0.000002 0 -0.000009 -48.149985 0.00001 0 0 0 0 0 0
$definebone "ValveBiped.HC_Body_Bone" "ValveBiped.Bip01_Spine4" 7.856865 0.35714 2.41031 -9.085843 72.317298 154.900057 0 0 0 0 0 0
$definebone "ValveBiped.HC_Head_Bone" "ValveBiped.HC_Body_Bone" 3.7386 -2.965115 -0.136395 -4.500008 -4.500015 12.374989 0 0 0 0 0 0
$definebone "ValveBiped.HC_FangL_Bone1" "ValveBiped.HC_Head_Bone" 1.884797 0.225918 -3.244101 -7.359746 -99.621499 -49.057735 0 0 0 0 0 0
$definebone "ValveBiped.HC_FangL_Bone2" "ValveBiped.HC_FangL_Bone1" 3.698879 0.000002 0 0.000012 -38.081925 -0.000001 0 0 0 0 0 0
$definebone "ValveBiped.HC_FangR_Bone1" "ValveBiped.HC_Head_Bone" 1.88467 0.22591 3.247673 7.360111 -99.621198 -130.939945 0 0 0 0 0 0
$definebone "ValveBiped.HC_FangR_Bone2" "ValveBiped.HC_FangR_Bone1" 3.698872 0 0 0.000004 38.081921 -0.000017 0 0 0 0 0 0
$definebone "ValveBiped.HC_HoleControlL" "ValveBiped.HC_Body_Bone" -1.923262 -4.27964 -3.138432 -0.000004 -0.00001 0.000015 0 0 0 0 0 0
$definebone "ValveBiped.HC_HoleControlR" "ValveBiped.HC_Body_Bone" -0.320878 -5.869141 3.247988 -0.000004 -0.00001 0.000015 0 0 0 0 0 0
$definebone "ValveBiped.HC_Rear_Bone" "ValveBiped.HC_Body_Bone" -7.842797 -1.67841 -0.934004 -0.000009 20.249976 29.250029 0 0 0 0 0 0
$definebone "ValveBiped.HC_ThighL_Bone" "ValveBiped.HC_Rear_Bone" -0.450934 -0.675697 -6.164909 -30.718445 -70.093409 -172.984649 0 0 0 0 0 0
$definebone "ValveBiped.HC_CalfL_Bone" "ValveBiped.HC_ThighL_Bone" 6.098812 0 0.000002 -0.000024 -0.000012 -0.000001 0 0 0 0 0 0
$definebone "ValveBiped.HC_ThighR_Bone" "ValveBiped.HC_Rear_Bone" -0.450914 -0.675701 6.164912 -7.079969 -90.538645 174.066278 0 0 0 0 0 0
$definebone "ValveBiped.HC_CalfR_Bone" "ValveBiped.HC_ThighR_Bone" 6.098816 -0.000003 0 0.000014 -130.088657 -0.000017 0 0 0 0 0 0
$definebone "ValveBiped.HC_UpperArmL_Bone" "ValveBiped.HC_Body_Bone" 2.630718 -3.15509 -5.01671 17.056879 -28.79617 1.880352 0 0 0 0 0 0
$definebone "ValveBiped.HC_ForeArmL_Bone" "ValveBiped.HC_UpperArmL_Bone" 8.255686 0 0.000002 0.000027 -121.038779 0.000004 0 0 0 0 0 0
$definebone "ValveBiped.HC_UpperArmR_Bone" "ValveBiped.HC_Body_Bone" 2.630419 -3.155079 5.01687 -4.143269 -63.369628 -131.616994 0 0 0 0 0 0
$definebone "ValveBiped.HC_ForeArmR_Bone" "ValveBiped.HC_UpperArmR_Bone" 8.25568 0.000004 0.000005 0.000026 110.674582 -0.000021 0 0 0 0 0 0


$sectionframes 30 119

$poseparameter "move_yaw" -180 180 loop 360

$ikchain "rhand" "ValveBiped.Bip01_R_Hand" knee 0.707 0.707 0
$ikchain "lhand" "ValveBiped.Bip01_L_Hand" knee 0.707 0.707 0
$ikchain "rfoot" "ValveBiped.Bip01_R_Foot" knee 0.707 -0.707 0
$ikchain "lfoot" "ValveBiped.Bip01_L_Foot" knee 0.707 -0.707 0

$ikautoplaylock "rfoot" 1 0.1
$ikautoplaylock "lfoot" 1 0.1

$weightlist "weights_flinch_grenade_backinDelta" {
	 "ValveBiped.Bip01_Pelvis" 1
	 "ValveBiped.Bip01_L_Thigh" 0
	 "ValveBiped.Bip01_L_Calf" 0
	 "ValveBiped.Bip01_L_Foot" 0
	 "ValveBiped.Bip01_L_Toe0" 0
	 "ValveBiped.Bip01_R_Thigh" 0
	 "ValveBiped.Bip01_R_Calf" 0
	 "ValveBiped.Bip01_R_Foot" 0
	 "ValveBiped.Bip01_R_Toe0" 0
	 "ValveBiped.Cod_Bone" 1
	 "ValveBiped.Bip01_Spine" 1
	 "ValveBiped.Bip01_Spine1" 1
	 "ValveBiped.Bip01_Spine2" 1
	 "ValveBiped.Bip01_Spine4" 1
	 "ValveBiped.Bip01_L_Clavicle" 1
	 "ValveBiped.Bip01_L_UpperArm" 1
	 "ValveBiped.Bip01_L_Forearm" 1
	 "ValveBiped.Bip01_L_Hand" 1
	 "ValveBiped.Bip01_L_Finger0" 1
	 "ValveBiped.Bip01_L_Finger01" 1
	 "ValveBiped.Bip01_L_Finger02" 1
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
	 "ValveBiped.Bip01_R_Finger0" 1
	 "ValveBiped.Bip01_R_Finger01" 1
	 "ValveBiped.Bip01_R_Finger02" 1
	 "ValveBiped.Bip01_R_Finger3" 1
	 "ValveBiped.Bip01_R_Finger31" 1
	 "ValveBiped.Bip01_R_Finger32" 1
	 "ValveBiped.Bip01_R_Finger2" 1
	 "ValveBiped.Bip01_R_Finger21" 1
	 "ValveBiped.Bip01_R_Finger22" 1
	 "ValveBiped.Bip01_R_Finger1" 1
	 "ValveBiped.Bip01_R_Finger11" 1
	 "ValveBiped.Bip01_R_Finger12" 1
	 "ValveBiped.HC_Body_Bone" 1
	 "ValveBiped.HC_Head_Bone" 1
	 "ValveBiped.HC_FangL_Bone1" 1
	 "ValveBiped.HC_FangL_Bone2" 1
	 "ValveBiped.HC_FangR_Bone1" 1
	 "ValveBiped.HC_FangR_Bone2" 1
	 "ValveBiped.HC_HoleControlL" 1
	 "ValveBiped.HC_HoleControlR" 1
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

$weightlist "weights_flinch_grenade_back" {
	 "ValveBiped.Bip01_Pelvis" 0
	 "ValveBiped.Bip01_L_Thigh" 0
	 "ValveBiped.Bip01_L_Calf" 0
	 "ValveBiped.Bip01_L_Foot" 0
	 "ValveBiped.Bip01_L_Toe0" 0
	 "ValveBiped.Bip01_R_Thigh" 0
	 "ValveBiped.Bip01_R_Calf" 0
	 "ValveBiped.Bip01_R_Foot" 0
	 "ValveBiped.Bip01_R_Toe0" 0
	 "ValveBiped.Cod_Bone" 0
	 "ValveBiped.Bip01_Spine" 0
	 "ValveBiped.Bip01_Spine1" 0
	 "ValveBiped.Bip01_Spine2" 0
	 "ValveBiped.Bip01_Spine4" 0
	 "ValveBiped.Bip01_L_Clavicle" 0
	 "ValveBiped.Bip01_L_UpperArm" 0
	 "ValveBiped.Bip01_L_Forearm" 0
	 "ValveBiped.Bip01_L_Hand" 0
	 "ValveBiped.Bip01_L_Finger0" 0
	 "ValveBiped.Bip01_L_Finger01" 0
	 "ValveBiped.Bip01_L_Finger02" 0
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
	 "ValveBiped.Bip01_R_Finger0" 0
	 "ValveBiped.Bip01_R_Finger01" 0
	 "ValveBiped.Bip01_R_Finger02" 0
	 "ValveBiped.Bip01_R_Finger3" 0
	 "ValveBiped.Bip01_R_Finger31" 0
	 "ValveBiped.Bip01_R_Finger32" 0
	 "ValveBiped.Bip01_R_Finger2" 0
	 "ValveBiped.Bip01_R_Finger21" 0
	 "ValveBiped.Bip01_R_Finger22" 0
	 "ValveBiped.Bip01_R_Finger1" 0
	 "ValveBiped.Bip01_R_Finger11" 0
	 "ValveBiped.Bip01_R_Finger12" 0
	 "ValveBiped.HC_Body_Bone" 0
	 "ValveBiped.HC_Head_Bone" 0
	 "ValveBiped.HC_FangL_Bone1" 0
	 "ValveBiped.HC_FangL_Bone2" 0
	 "ValveBiped.HC_FangR_Bone1" 0
	 "ValveBiped.HC_FangR_Bone2" 0
	 "ValveBiped.HC_HoleControlL" 0
	 "ValveBiped.HC_HoleControlR" 0
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

$weightlist "weights_layer_upperBodyGrenade" {
	 "ValveBiped.Bip01_Pelvis" 0
	 "ValveBiped.Bip01_L_Thigh" 0
	 "ValveBiped.Bip01_L_Calf" 0
	 "ValveBiped.Bip01_L_Foot" 0
	 "ValveBiped.Bip01_L_Toe0" 0
	 "ValveBiped.Bip01_R_Thigh" 0
	 "ValveBiped.Bip01_R_Calf" 0
	 "ValveBiped.Bip01_R_Foot" 0
	 "ValveBiped.Bip01_R_Toe0" 0
	 "ValveBiped.Cod_Bone" 0
	 "ValveBiped.Bip01_Spine" 1
	 "ValveBiped.Bip01_Spine1" 1
	 "ValveBiped.Bip01_Spine2" 1
	 "ValveBiped.Bip01_Spine4" 1
	 "ValveBiped.Bip01_L_Clavicle" 1
	 "ValveBiped.Bip01_L_UpperArm" 1
	 "ValveBiped.Bip01_L_Forearm" 1
	 "ValveBiped.Bip01_L_Hand" 1
	 "ValveBiped.Bip01_L_Finger0" 1
	 "ValveBiped.Bip01_L_Finger01" 1
	 "ValveBiped.Bip01_L_Finger02" 1
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
	 "ValveBiped.Bip01_R_Finger0" 1
	 "ValveBiped.Bip01_R_Finger01" 1
	 "ValveBiped.Bip01_R_Finger02" 1
	 "ValveBiped.Bip01_R_Finger3" 1
	 "ValveBiped.Bip01_R_Finger31" 1
	 "ValveBiped.Bip01_R_Finger32" 1
	 "ValveBiped.Bip01_R_Finger2" 1
	 "ValveBiped.Bip01_R_Finger21" 1
	 "ValveBiped.Bip01_R_Finger22" 1
	 "ValveBiped.Bip01_R_Finger1" 1
	 "ValveBiped.Bip01_R_Finger11" 1
	 "ValveBiped.Bip01_R_Finger12" 1
	 "ValveBiped.HC_Body_Bone" 1
	 "ValveBiped.HC_Head_Bone" 1
	 "ValveBiped.HC_FangL_Bone1" 1
	 "ValveBiped.HC_FangL_Bone2" 1
	 "ValveBiped.HC_FangR_Bone1" 1
	 "ValveBiped.HC_FangR_Bone2" 1
	 "ValveBiped.HC_HoleControlL" 1
	 "ValveBiped.HC_HoleControlR" 1
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

$animation "@flinch_grenade_backinDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_backinDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_backCoreDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_backCoreDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_backoutDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_backoutDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_frontinDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_frontinDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_frontCoreDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_frontCoreDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_frontoutDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_frontoutDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_eastinDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_eastinDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_eastCoreDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_eastCoreDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_eastoutDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_eastoutDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_westinDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_westinDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_westCoreDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_westCoreDelta_corrective_animation.smd" {
}

$animation "@flinch_grenade_westoutDelta_corrective_animation" "zombine_anims_anims\@flinch_grenade_westoutDelta_corrective_animation.smd" {
}

$animation "@flinch1inDelta_corrective_animation" "zombine_anims_anims\@flinch1inDelta_corrective_animation.smd" {
}

$animation "@flinch1CoreDelta_corrective_animation" "zombine_anims_anims\@flinch1CoreDelta_corrective_animation.smd" {
}

$animation "@flinch1outDelta_corrective_animation" "zombine_anims_anims\@flinch1outDelta_corrective_animation.smd" {
}

$animation "@flinch2inDelta_corrective_animation" "zombine_anims_anims\@flinch2inDelta_corrective_animation.smd" {
}

$animation "@flinch2CoreDelta_corrective_animation" "zombine_anims_anims\@flinch2CoreDelta_corrective_animation.smd" {
}

$animation "@flinch2outDelta_corrective_animation" "zombine_anims_anims\@flinch2outDelta_corrective_animation.smd" {
}

$animation "@flinch3inDelta_corrective_animation" "zombine_anims_anims\@flinch3inDelta_corrective_animation.smd" {
}

$animation "@flinch3CoreDelta_corrective_animation" "zombine_anims_anims\@flinch3CoreDelta_corrective_animation.smd" {
}

$animation "@flinch3outDelta_corrective_animation" "zombine_anims_anims\@flinch3outDelta_corrective_animation.smd" {
}

$animation "a_WalkN" "zombine_anims_anims\a_WalkN.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 7 fakeorigin 12.25 10.71 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 7 26 32 target 3
	ikrule "rfoot" footstep contact 24 fakeorigin 4.52 -9.9 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 16 24 9 32 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 1
	walkframe 32 LX LY
}

$animation "a_WalkS" "zombine_anims_anims\a_WalkS.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 8 fakeorigin -8.9 2.79 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 1 8 26 32 target 3
	ikrule "rfoot" footstep contact 23 fakeorigin -10.83 -8.36 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 15 23 8 32 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 1
	walkframe 32 LX LY
}

$animation "a_WalkSE" "zombine_anims_anims\a_WalkSE.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 5 fakeorigin -3.03 -2.85 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 30 5 24 32 target 3
	ikrule "rfoot" footstep contact 22 fakeorigin -10.91 -16.09 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 15 22 8 32 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 1
	walkframe 32 LX LY
}

$animation "a_WalkE" "zombine_anims_anims\a_WalkE.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 4 fakeorigin 5.08 -5.16 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 30 4 25 32 target 3
	ikrule "rfoot" footstep contact 22 fakeorigin -5.71 -21.09 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 14 22 7 32 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 1
	walkframe 32 LX LY
}

$animation "a_WalkNE" "zombine_anims_anims\a_WalkNE.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 6 fakeorigin 11.04 -5.25 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 6 27 32 target 3
	ikrule "rfoot" footstep contact 24 fakeorigin 3.27 -17.42 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 16 24 9 32 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 1
	walkframe 32 LX LY
}

$animation "a_WalkNW" "zombine_anims_anims\a_WalkNW.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 8 fakeorigin 4.82 8.14 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 2 8 28 32 target 3
	ikrule "rfoot" footstep contact 25 fakeorigin 3.23 -9.29 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 17 25 9 32 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 1
	walkframe 32 LX LY
}

$animation "a_WalkW" "zombine_anims_anims\a_WalkW.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 10 fakeorigin 2.31 7.2 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 3 10 28 32 target 3
	ikrule "rfoot" footstep contact 25 fakeorigin -7.18 -5.12 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 16 25 7 32 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 1
	walkframe 32 LX LY
}

$animation "a_WalkSW" "zombine_anims_anims\a_WalkSW.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 8 fakeorigin -3.49 8.7 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 1 8 27 32 target 3
	ikrule "rfoot" footstep contact 24 fakeorigin -6.01 -7.9 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 17 24 10 32 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 32 32 target 1
	walkframe 32 LX LY
}

$animation "a_run_n" "zombine_anims_anims\a_run_n.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 7 fakeorigin 5.18 8.95 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 18 7 10 20 target 3
	ikrule "rfoot" footstep contact 18 fakeorigin 8.41 -6.56 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 10 18 2 20 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	walkframe 20 LX LY
}

$animation "a_run_ne" "zombine_anims_anims\a_run_ne.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 7 fakeorigin 5.8 -3.07 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 18 7 10 20 target 3
	ikrule "rfoot" footstep contact 18 fakeorigin 2.24 -16.48 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 10 18 2 20 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	walkframe 20 LX LY
}

$animation "a_run_nw" "zombine_anims_anims\a_run_nw.smd" {
	fps 30
	loop
	ikrule "lfoot" footstep contact 8 fakeorigin 7.51 3.72 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 19 8 11 20 target 3
	ikrule "rfoot" footstep contact 18 fakeorigin 5.43 -7.87 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 10 18 2 20 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	walkframe 20 LX LY
}

$sequence "Idle01" {
	"zombine_anims_anims\Idle01.smd"
	activity "ACT_IDLE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "lfoot" footstep contact 0 fakeorigin 7.34 10.86 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 48 48 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -14.35 -7.01 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 48 48 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 48 48 target 1
}

$sequence "pullGrenade" {
	"zombine_anims_anims\pullGrenade.smd"
	activity "ACT_SLAM_DETONATOR_DRAW" 1
	{ event 1100 14 "grenade" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 35 35 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 35 35 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 35 35 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 35 35 target 3
}

$sequence "Idle_Grenade" {
	"zombine_anims_anims\Idle_Grenade.smd"
	activity "ACT_HANDGRENADE_THROW1" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "lfoot" footstep contact 0 fakeorigin 7.34 10.86 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 29 29 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -7.87 -6.71 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 29 29 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 29 29 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 29 29 target 1
}

$sequence "flinch_grenade_backinDelta" {
	"zombine_anims_anims\flinch_grenade_backinDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_backinDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_backinFrame" {
	"zombine_anims_anims\flinch_grenade_backinFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_backCoreDelta" {
	"zombine_anims_anims\flinch_grenade_backCoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_backCoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_backOutFrame" {
	"zombine_anims_anims\flinch_grenade_backOutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_backoutDelta" {
	"zombine_anims_anims\flinch_grenade_backoutDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_backoutDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 17 17 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 17 17 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_back" {
	"zombine_anims_anims\flinch_grenade_back.smd"
	activity "ACT_ZOMBINE_GRENADE_FLINCH_BACK" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch_grenade_backinFrame" 0 1 1 3 xfade spline
	blendlayer "flinch_grenade_backOutFrame" 1 3 3 19 spline
	blendlayer "flinch_grenade_backinDelta" 0 0 1 1
	blendlayer "flinch_grenade_backCoreDelta" 1 1 3 3
	blendlayer "flinch_grenade_backoutDelta" 3 3 20 19
	fps 30
	weightlist "weights_flinch_grenade_back"
}

$sequence "flinch_grenade_frontinDelta" {
	"zombine_anims_anims\flinch_grenade_frontinDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_frontinDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_frontinFrame" {
	"zombine_anims_anims\flinch_grenade_frontinFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_frontCoreDelta" {
	"zombine_anims_anims\flinch_grenade_frontCoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_frontCoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_frontOutFrame" {
	"zombine_anims_anims\flinch_grenade_frontOutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_frontoutDelta" {
	"zombine_anims_anims\flinch_grenade_frontoutDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_frontoutDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 17 17 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 17 17 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_front" {
	"zombine_anims_anims\flinch_grenade_front.smd"
	activity "ACT_ZOMBINE_GRENADE_FLINCH_FRONT" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch_grenade_frontinFrame" 0 1 1 3 xfade spline
	blendlayer "flinch_grenade_frontOutFrame" 1 3 3 19 spline
	blendlayer "flinch_grenade_frontinDelta" 0 0 1 1
	blendlayer "flinch_grenade_frontCoreDelta" 1 1 3 3
	blendlayer "flinch_grenade_frontoutDelta" 3 3 20 19
	fps 30
	weightlist "weights_flinch_grenade_back"
}

$sequence "flinch_grenade_eastinDelta" {
	"zombine_anims_anims\flinch_grenade_eastinDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_eastinDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_eastinFrame" {
	"zombine_anims_anims\flinch_grenade_eastinFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_eastCoreDelta" {
	"zombine_anims_anims\flinch_grenade_eastCoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_eastCoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_eastOutFrame" {
	"zombine_anims_anims\flinch_grenade_eastOutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_eastoutDelta" {
	"zombine_anims_anims\flinch_grenade_eastoutDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_eastoutDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 17 17 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 17 17 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_east" {
	"zombine_anims_anims\flinch_grenade_east.smd"
	activity "ACT_ZOMBINE_GRENADE_FLINCH_EAST" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch_grenade_eastinFrame" 0 1 1 3 xfade spline
	blendlayer "flinch_grenade_eastOutFrame" 1 3 3 19 spline
	blendlayer "flinch_grenade_eastinDelta" 0 0 1 1
	blendlayer "flinch_grenade_eastCoreDelta" 1 1 3 3
	blendlayer "flinch_grenade_eastoutDelta" 3 3 20 19
	fps 30
	weightlist "weights_flinch_grenade_back"
}

$sequence "flinch_grenade_westinDelta" {
	"zombine_anims_anims\flinch_grenade_westinDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_westinDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 1 1 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_westinFrame" {
	"zombine_anims_anims\flinch_grenade_westinFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_westCoreDelta" {
	"zombine_anims_anims\flinch_grenade_westCoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_westCoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_westOutFrame" {
	"zombine_anims_anims\flinch_grenade_westOutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_westoutDelta" {
	"zombine_anims_anims\flinch_grenade_westoutDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch_grenade_westoutDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 17 17 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 17 17 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch_grenade_west" {
	"zombine_anims_anims\flinch_grenade_west.smd"
	activity "ACT_ZOMBINE_GRENADE_FLINCH_WEST" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch_grenade_westinFrame" 0 1 1 3 xfade spline
	blendlayer "flinch_grenade_westOutFrame" 1 3 3 19 spline
	blendlayer "flinch_grenade_westinDelta" 0 0 1 1
	blendlayer "flinch_grenade_westCoreDelta" 1 1 3 3
	blendlayer "flinch_grenade_westoutDelta" 3 3 20 19
	fps 30
	weightlist "weights_flinch_grenade_back"
}

$sequence "layer_upperBodyGrenade" {
	"zombine_anims_anims\layer_upperBodyGrenade.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	loop
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 29 29 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 29 29 target 1
	weightlist "weights_layer_upperBodyGrenade"
}

$sequence "FastAttack" {
	"zombine_anims_anims\FastAttack.smd"
	activity "ACT_MELEE_ATTACK2" 1
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 9 "melee" }
	{ event AE_NPC_ATTACK_BROADCAST 3 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin 7.35 10.86 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 31 31 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -14.35 -7.01 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 31 31 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 31 31 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 31 31 target 1
}

$sequence "layer_walk_upperBodyGrenade" {
	"zombine_anims_anims\layer_walk_upperBodyGrenade.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	loop
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 33 33 target 1
	weightlist "weights_layer_upperBodyGrenade"
}

$sequence "walk_All" {
	"a_WalkS"
	"a_WalkSE"
	"a_WalkE"
	"a_WalkNE"
	"a_WalkN"
	"a_WalkNW"
	"a_WalkW"
	"a_WalkSW"
	"a_WalkS"
	activity "ACT_WALK" 1
	blend "move_yaw" -180 180
	blendwidth 9
	{ event 1100 14 "step" }
	{ event 1100 28 "step" }
	fadein 0.2
	fadeout 0.2
}

$sequence "walk_All_Grenade" {
	"a_WalkS"
	"a_WalkSE"
	"a_WalkE"
	"a_WalkNE"
	"a_WalkN"
	"a_WalkNW"
	"a_WalkW"
	"a_WalkSW"
	"a_WalkS"
	activity "ACT_HANDGRENADE_THROW2" 1
	blend "move_yaw" -180 180
	blendwidth 9
	{ event 1100 14 "step" }
	{ event 1100 28 "step" }
	fadein 0.2
	fadeout 0.2

keyvalues
{
	interior Idle_Grenade
}
	addlayer "layer_walk_upperBodyGrenade"
}

$sequence "layer_Run_upperBodyGrenade" {
	"zombine_anims_anims\layer_Run_upperBodyGrenade.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	loop
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 20 20 target 1
	weightlist "weights_layer_upperBodyGrenade"
}

$sequence "Run_All" {
	"a_WalkS"
	"a_WalkSE"
	"a_WalkE"
	"a_run_ne"
	"a_run_n"
	"a_run_nw"
	"a_WalkW"
	"a_WalkSW"
	"a_WalkS"
	activity "ACT_RUN" 1
	blend "move_yaw" -180 180
	blendwidth 9
	{ event 1100 15 "step" }
	{ event 1100 7 "step" }
	fadein 0.2
	fadeout 0.2
}

$sequence "Run_All_grenade" {
	"a_WalkS"
	"a_WalkSE"
	"a_WalkE"
	"a_run_ne"
	"a_run_n"
	"a_run_nw"
	"a_WalkW"
	"a_WalkSW"
	"a_WalkS"
	activity "ACT_HANDGRENADE_THROW3" 1
	blend "move_yaw" -180 180
	blendwidth 9
	{ event 1100 15 "step" }
	{ event 1100 7 "step" }
	fadein 0.2
	fadeout 0.2

keyvalues
{
	interior Idle_Grenade
}
	addlayer "layer_Run_upperBodyGrenade"
}

$sequence "Zombine_Intro_Pose" {
	"zombine_anims_anims\Zombine_Intro_Pose.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 44 44 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 44 44 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 44 44 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 44 44 target 3
}

$sequence "FireWalk" {
	"zombine_anims_anims\FireWalk.smd"
	activity "ACT_WALK_ON_FIRE" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "lfoot" footstep contact 2 fakeorigin -0.23 14.49 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 25 2 18 30 target 3 usesequence 
	ikrule "rfoot" footstep contact 17 fakeorigin -1.36 -1.76 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 10 17 4 30 target 2 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1 usesequence 
	walkframe 30 LX LY
}

$sequence "attackA" {
	"zombine_anims_anims\attackA.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee" }
	{ event AE_NPC_ATTACK_BROADCAST 10 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -6.92 13.19 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 22 34 target 3 usesequence 
	ikrule "lfoot" footstep contact 46 fakeorigin -6.33 13.52 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 34 46 56 56 target 3 usesequence 
	ikrule "rfoot" footstep contact 0 fakeorigin -4.47 -6.65 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 22 29 target 2 usesequence 
	ikrule "rfoot" footstep contact 36 fakeorigin 0.08 -4.76 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 29 36 56 56 target 2 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 1 usesequence 
	walkframe 47 LX LY
}

$sequence "attackB" {
	"zombine_anims_anims\attackB.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee" }
	{ event AE_NPC_ATTACK_BROADCAST 10 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -6.92 13.19 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 56 56 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -4.47 -6.65 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 56 56 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 1
}

$sequence "attackC" {
	"zombine_anims_anims\attackC.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee" }
	{ event AE_NPC_ATTACK_BROADCAST 10 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -7.37 13.18 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 22 28 target 3 usesequence 
	ikrule "lfoot" footstep contact 34 fakeorigin -1.22 11.79 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 28 34 56 56 target 3 usesequence 
	ikrule "rfoot" footstep contact 0 fakeorigin -4.79 -6.64 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 22 34 target 2 usesequence 
	ikrule "rfoot" footstep contact 46 fakeorigin -4.69 -7.99 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 34 46 56 56 target 2 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 1 usesequence 
	walkframe 47 LX LY
}

$sequence "attackD" {
	"zombine_anims_anims\attackD.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee" }
	{ event AE_NPC_ATTACK_BROADCAST 10 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -6.97 13.12 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 56 56 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -4.44 -6.79 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 56 56 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 56 56 target 1
}

$sequence "attackE" {
	"zombine_anims_anims\attackE.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee_heavy" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -6.97 13.12 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 41 41 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -4.44 -6.79 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 6 15 target 2
	ikrule "rfoot" footstep contact 23 fakeorigin 7.43 -9.68 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 15 23 32 35 target 2
	ikrule "rfoot" footstep contact 38 fakeorigin -4.44 -6.8 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 35 38 41 41 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 1
}

$sequence "attackF" {
	"zombine_anims_anims\attackF.smd"
	activity "ACT_MELEE_ATTACK1" 2
	{ event 1100 5 "sound_attack_scream" }
	{ event 1100 25 "melee_heavy" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -7.25 13.07 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 41 41 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -4.3 -6.75 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 41 41 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 1
}

$sequence "swatleftmid" {
	"zombine_anims_anims\swatleftmid.smd"
	activity "ACT_ZOM_SWATLEFTMID" 1
	{ event AE_ZOMBIE_STARTSWAT 1 "" }
	{ event AE_ZOMBIE_SWATITEM 9 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -6.97 13.12 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 36 36 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -4.44 -6.79 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 36 36 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 1
}

$sequence "swatrightmid" {
	"zombine_anims_anims\swatrightmid.smd"
	activity "ACT_ZOM_SWATRIGHTMID" 1
	{ event AE_ZOMBIE_STARTSWAT 1 "" }
	{ event AE_ZOMBIE_SWATITEM 9 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -6.92 13.19 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 36 36 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -4.47 -6.65 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 36 36 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 36 36 target 1
}

$sequence "swatleftlow" {
	"zombine_anims_anims\swatleftlow.smd"
	activity "ACT_ZOM_SWATLEFTLOW" 1
	{ event AE_ZOMBIE_STARTSWAT 1 "" }
	{ event AE_ZOMBIE_SWATITEM 15 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -7.37 13.18 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 17 22 target 3 usesequence 
	ikrule "lfoot" footstep contact 26 fakeorigin -3.24 10.44 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 22 26 45 45 target 3 usesequence 
	ikrule "rfoot" footstep contact 0 fakeorigin -4.79 -6.64 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 17 28 target 2 usesequence 
	ikrule "rfoot" footstep contact 39 fakeorigin -4.67 -8.09 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 28 39 45 45 target 2 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1 usesequence 
	walkframe 36 LX LY
}

$sequence "swatrightlow" {
	"zombine_anims_anims\swatrightlow.smd"
	activity "ACT_ZOM_SWATRIGHTLOW" 1
	{ event AE_ZOMBIE_STARTSWAT 1 "" }
	{ event AE_ZOMBIE_SWATITEM 18 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -6.92 13.19 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 15 27 target 3 usesequence 
	ikrule "lfoot" footstep contact 38 fakeorigin -6.71 14.03 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 27 38 47 47 target 3 usesequence 
	ikrule "rfoot" footstep contact 0 fakeorigin -4.47 -6.65 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 0 0 23 26 target 2 usesequence 
	ikrule "rfoot" footstep contact 29 fakeorigin -1.15 -4.18 0 fakerotate 0 0 0 floor 0 height 18 radius 2.5 range 26 29 47 47 target 2 usesequence 
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 47 47 target 0 usesequence 
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 47 47 target 1 usesequence 
	walkframe 38 LX LY
}

$sequence "releasecrab" {
	"zombine_anims_anims\releasecrab.smd"
	activity "ACT_ZOM_RELEASECRAB" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "lfoot" footstep contact 0 fakeorigin -6.92 13.19 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 30 30 target 3
	ikrule "rfoot" footstep contact 0 fakeorigin -4.47 -6.65 0 fakerotate 0 0 0 floor 0 height 20 radius 2.5 range 0 0 30 30 target 2
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1
}

$sequence "physflinch1" {
	"zombine_anims_anims\physflinch1.smd"
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
	"zombine_anims_anims\physflinch2.smd"
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
	"zombine_anims_anims\physflinch3.smd"
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
	"zombine_anims_anims\smashfall64.smd"
	{ event 1100 10 "bodydrop_heavy" }
	{ event 1100 27 "bodydrop_heavy" }
	{ event 1100 37 "bodydrop_heavy" }
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
	"zombine_anims_anims\slump_a.smd"
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
	"zombine_anims_anims\slumprise_a.smd"
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
	"zombine_anims_anims\slumprise_a_attack.smd"
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
	"zombine_anims_anims\slumprise_a2.smd"
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
	"zombine_anims_anims\slump_b.smd"
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
	"zombine_anims_anims\slumprise_b.smd"
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
	"zombine_anims_anims\Breakthrough.smd"
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
	"zombine_anims_anims\canal5await.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 3
}

$sequence "canal5aattack" {
	"zombine_anims_anims\canal5aattack.smd"
	fadein 0.2
	fadeout 0.2
	snap
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 3
}

$sequence "flinch1inDelta" {
	"zombine_anims_anims\flinch1inDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch1inDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 8 8 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 8 8 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch1inFrame" {
	"zombine_anims_anims\flinch1inFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch1CoreDelta" {
	"zombine_anims_anims\flinch1CoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch1CoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 4 4 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 4 4 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch1OutFrame" {
	"zombine_anims_anims\flinch1OutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch1outDelta" {
	"zombine_anims_anims\flinch1outDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch1outDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 6 6 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 6 6 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch1" {
	"zombine_anims_anims\flinch1.smd"
	activity "ACT_GESTURE_FLINCH_HEAD" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch1inFrame" 0 8 8 12 xfade spline
	blendlayer "flinch1OutFrame" 8 12 12 17 spline
	blendlayer "flinch1inDelta" 0 0 8 8
	blendlayer "flinch1CoreDelta" 8 8 12 12
	blendlayer "flinch1outDelta" 12 12 18 17
	fps 30
	weightlist "weights_flinch_grenade_back"
}

$sequence "flinch2inDelta" {
	"zombine_anims_anims\flinch2inDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch2inDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 7 7 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 7 7 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch2inFrame" {
	"zombine_anims_anims\flinch2inFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch2CoreDelta" {
	"zombine_anims_anims\flinch2CoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch2CoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 5 5 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 5 5 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch2OutFrame" {
	"zombine_anims_anims\flinch2OutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch2outDelta" {
	"zombine_anims_anims\flinch2outDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch2outDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 8 8 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 8 8 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch2" {
	"zombine_anims_anims\flinch2.smd"
	activity "ACT_GESTURE_FLINCH_HEAD" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch2inFrame" 0 7 7 12 xfade spline
	blendlayer "flinch2OutFrame" 7 12 12 19 spline
	blendlayer "flinch2inDelta" 0 0 7 7
	blendlayer "flinch2CoreDelta" 7 7 12 12
	blendlayer "flinch2outDelta" 12 12 20 19
	fps 30
	weightlist "weights_flinch_grenade_back"
}

$sequence "flinch3inDelta" {
	"zombine_anims_anims\flinch3inDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch3inDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 10 10 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch3inFrame" {
	"zombine_anims_anims\flinch3inFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch3CoreDelta" {
	"zombine_anims_anims\flinch3CoreDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch3CoreDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 2 2 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch3OutFrame" {
	"zombine_anims_anims\flinch3OutFrame.smd"
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 0 0 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch3outDelta" {
	"zombine_anims_anims\flinch3outDelta.smd"
	delta
	// Crowbar writes this subtract option to prevent jigglebone and poseparameter problems when delta sequences are recompiled.
	subtract "@flinch3outDelta_corrective_animation" 0
	fadein 0.2
	fadeout 0.2
	hidden
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 8 8 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 8 8 target 1
	weightlist "weights_flinch_grenade_backinDelta"
}

$sequence "flinch3" {
	"zombine_anims_anims\flinch3.smd"
	activity "ACT_GESTURE_FLINCH_HEAD" 1
	fadein 0.2
	fadeout 0.2
	iklock "rfoot" 1 0
	iklock "lfoot" 1 0
	blendlayer "flinch3inFrame" 0 10 10 12 xfade spline
	blendlayer "flinch3OutFrame" 10 12 12 19 spline
	blendlayer "flinch3inDelta" 0 0 10 10
	blendlayer "flinch3CoreDelta" 10 10 12 12
	blendlayer "flinch3outDelta" 12 12 20 19
	fps 30
	weightlist "weights_flinch_grenade_back"
}

$sequence "flinch_head" {
	"zombine_anims_anims\flinch_head.smd"
	activity "ACT_FLINCH_HEAD" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 35 35 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 35 35 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 35 35 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 35 35 target 3
}

$sequence "flinch_leftArm" {
	"zombine_anims_anims\flinch_leftArm.smd"
	activity "ACT_FLINCH_LEFTARM" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 3
}

$sequence "flinch_rightArm" {
	"zombine_anims_anims\flinch_rightArm.smd"
	activity "ACT_FLINCH_RIGHTARM" 1
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 30 30 target 3
}

$sequence "flinch_leftLeg" {
	"zombine_anims_anims\flinch_leftLeg.smd"
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
	"zombine_anims_anims\flinch_rightLeg.smd"
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
	"zombine_anims_anims\alyx_zombie_fight1.smd"
	activity "ACT_DI_ALYX_ZOMBIE_MELEE" 1
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
	"zombine_anims_anims\alyx_zombie_fight2.smd"
	activity "ACT_DI_ALYX_ZOMBIE_MELEE" 1
	{ event 1100 4 "sound_alert" }
	{ event AE_CL_PLAYSOUND 22 "Zombie.Pain" }
	{ event AE_ZOMBIE_POPHEADCRAB 76 "ValveBiped.Bip01_R_hand 1000" }
	{ event AE_NPC_RAGDOLL 81 "" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 86 86 target 3
}

$sequence "alyx_zombie_kick1" {
	"zombine_anims_anims\alyx_zombie_kick1.smd"
	activity "ACT_DI_ALYX_ZOMBIE_MELEE" 1
	{ event AE_ZOMBIE_POPHEADCRAB 24 "ValveBiped.Bip01_R_Foot 1500" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 50 50 target 3
}

$sequence "alyx_zombie_grab" {
	"zombine_anims_anims\alyx_zombie_grab.smd"
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
	"zombine_anims_anims\alyx_zombie_grab_PI.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
	loop
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 41 41 target 3
}

$sequence "alyx_zombie_grab_exit" {
	"zombine_anims_anims\alyx_zombie_grab_exit.smd"
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
	"zombine_anims_anims\alyx_zombie_elbow_45L.smd"
	{ event AE_NPC_HURT_INTERACTION_PARTNER 43 "ME 100" }
	fadein 0.2
	fadeout 0.2
	fps 30
	ikrule "rhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 0
	ikrule "lhand" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 1
	ikrule "rfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 2
	ikrule "lfoot" release contact 0 fakeorigin 0 0 0 fakerotate 0 0 0 floor 0 height 0 radius 0 range 0 0 45 45 target 3
}

$sequence "Tantrum" {
	"zombine_anims_anims\Tantrum.smd"
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
	"zombine_anims_anims\WallPound.smd"
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

$sequence "vort_zombie_attack02" {
	"zombine_anims_anims\vort_zombie_attack02.smd"
	{ event AE_ZOMBIE_POPHEADCRAB 76 "ValveBiped.Bip01_R_Foot 2500" }
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
	"zombine_anims_anims\vort_zombie_attack.smd"
	{ event AE_ZOMBIE_ALERTSOUND 4 "" }
	{ event AE_CL_PLAYSOUND 12 "Zombie.AttackMiss" }
	{ event AE_CL_PLAYSOUND 16 "Zombie.AttackMiss" }
	{ event AE_CL_PLAYSOUND 19 "Zombine.Charge" }
	{ event AE_CL_PLAYSOUND 26 "Zombine.ScuffRight" }
	{ event AE_CL_PLAYSOUND 37 "Zombine.Pain" }
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
