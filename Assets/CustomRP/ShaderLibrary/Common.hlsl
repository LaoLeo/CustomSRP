//公共方法库
#ifndef CUSTOM_COMMON_INCLUDED
#define CUSTOM_COMMON_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "UnityInput.hlsl"

// //顶点从模型空间转换到世界空间
// float3 TransformObjectToWorld(float3 positionOS)
// {
//     return mul(unity_ObjectToWorld, float4(positionOS, 1.0)).xyz;
// }

// //从世界空间转换到裁剪空间
// float3 TransformWorldToHClip(float3 positionWS)
// {
//     return mul(unity_MatrixVP, float4(positionWS, 1.0));
// }

#define UNITY_MATRIX_M unity_ObjectToWorld
#define UNITY_MATRIX_I_M unity_WorldToObject
#define UNITY_MATRIX_V unity_MatrixV
#define UNITY_MATRIX_VP unity_MatrixVP
#define UNITY_MATRIX_P glstate_matrix_projection

// GPU Instancing库，需要里面定义好的宏和方法
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

//该文件包含了转换方法
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

#endif