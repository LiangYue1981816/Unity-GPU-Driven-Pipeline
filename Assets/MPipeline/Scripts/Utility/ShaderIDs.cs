﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class ShaderIDs
{
    public static readonly int _Count = Shader.PropertyToID("_Count");
    public static readonly int planes = Shader.PropertyToID("planes");
    public static readonly int _DirShadowMap = Shader.PropertyToID("_DirShadowMap");
    public static readonly int _Color = Shader.PropertyToID("_Color");
    public static readonly int _CubeShadowMap = Shader.PropertyToID("_CubeShadowMap");
    public static readonly int _InvVP = Shader.PropertyToID("_InvVP");
    public static readonly int _InvNonJitterVP = Shader.PropertyToID("_InvNonJitterVP");
    public static readonly int _LastVp = Shader.PropertyToID("_LastVp");
    public static readonly int _InvLastVp = Shader.PropertyToID("_InvLastVp");
    public static readonly int _ShadowMapVP = Shader.PropertyToID("_ShadowMapVP");
    public static readonly int _ShadowMapVPs = Shader.PropertyToID("_ShadowMapVPs");
    public static readonly int _ShadowCamPoses = Shader.PropertyToID("_ShadowCamPoses");
    public static readonly int _ShadowDisableDistance = Shader.PropertyToID("_ShadowDisableDistance");
    public static readonly int _DirLightFinalColor = Shader.PropertyToID("_DirLightFinalColor");
    public static readonly int _DirLightPos = Shader.PropertyToID("_DirLightPos");
    public static readonly int _LightPos = Shader.PropertyToID("_LightPos");
    public static readonly int _MainTex = Shader.PropertyToID("_MainTex");
    public static readonly int _SoftParam = Shader.PropertyToID("_SoftParam");
    public static readonly int _OffsetIndex = Shader.PropertyToID("_OffsetIndex");
    public static readonly int _ShadowOffset = Shader.PropertyToID("_ShadowOffset");
    public static readonly int _IndexBuffer = Shader.PropertyToID("_IndexBuffer");
    public static readonly int clusterBuffer = Shader.PropertyToID("clusterBuffer");
    public static readonly int instanceCountBuffer = Shader.PropertyToID("instanceCountBuffer");
    public static readonly int resultBuffer = Shader.PropertyToID("resultBuffer");
    public static readonly int verticesBuffer = Shader.PropertyToID("verticesBuffer");
    public static readonly int dispatchBuffer = Shader.PropertyToID("dispatchBuffer");
    public static readonly int reCheckResult = Shader.PropertyToID("reCheckResult");
    public static readonly int reCheckCount = Shader.PropertyToID("reCheckCount");
    public static readonly int allPoints = Shader.PropertyToID("allPoints");
    public static readonly int _DeltaTime = Shader.PropertyToID("_DeltaTime");
    public static readonly int _LastFrameModel = Shader.PropertyToID("_LastFrameModel");
    public static readonly int _RainTexture = Shader.PropertyToID("_RainTexture");
    public static readonly int _LastFrameDepthTexture = Shader.PropertyToID("_LastFrameDepthTexture");
    public static readonly int _CameraNormals = Shader.PropertyToID("_CameraNormals");

    public static readonly int _Jitter = Shader.PropertyToID("_Jitter");
    public static readonly int _LastJitter = Shader.PropertyToID("_LastJitter");
    public static readonly int _Sharpness = Shader.PropertyToID("_Sharpness");
    public static readonly int _FinalBlendParameters = Shader.PropertyToID("_FinalBlendParameters");
    public static readonly int _HistoryTex = Shader.PropertyToID("_HistoryTex");
    public static readonly int _TextureSize = Shader.PropertyToID("_TextureSize");
    public static readonly int _TextureBuffer = Shader.PropertyToID("_TextureBuffer");
    public static readonly int _IndirectIntensity = Shader.PropertyToID("_IndirectIntensity");

    public static readonly int _ShadowMapResolution = Shader.PropertyToID("_ShadowMapResolution");
    public static readonly int _LightDir = Shader.PropertyToID("_LightDir");
    public static readonly int _WorldPoses = Shader.PropertyToID("_WorldPoses");
    public static readonly int _PreviousLevel = Shader.PropertyToID("_PreviousLevel");
    public static readonly int _HizDepthTex = Shader.PropertyToID("_HizDepthTex");
    public static readonly int _CameraUpVector = Shader.PropertyToID("_CameraUpVector");
    public static readonly int _VP = Shader.PropertyToID("_VP");
    public static readonly int _Depth = Shader.PropertyToID("_Depth");
    public static readonly int _LastDepth = Shader.PropertyToID("_LastDepth");
    public static readonly int _NonJitterVP = Shader.PropertyToID("_NonJitterVP");
    public static readonly int _NonJitterTextureVP = Shader.PropertyToID("_NonJitterTextureVP");
    public static readonly int _Lut3D = Shader.PropertyToID("_Lut3D");
    public static readonly int _Lut3D_Params = Shader.PropertyToID("_Lut3D_Params");
    public static readonly int _PostExposure = Shader.PropertyToID("_PostExposure");
    public static readonly int _TemporalClipBounding = Shader.PropertyToID("_TemporalClipBounding");

    public static readonly int _LightIntensity = Shader.PropertyToID("_LightIntensity");
    public static readonly int _LightColor = Shader.PropertyToID("_LightColor");
    public static readonly int lightPositionBuffer = Shader.PropertyToID("lightPositionBuffer");
    public static readonly int _LightRadius = Shader.PropertyToID("_LightRadius");

    public static readonly int _ModelBones = Shader.PropertyToID("_ModelBones");
    public static readonly int _TimeVar = Shader.PropertyToID("_TimeVar");
    public static readonly int _AnimTex = Shader.PropertyToID("_AnimTex");
    public static readonly int objBuffer = Shader.PropertyToID("objBuffer");
    public static readonly int bonesBuffer = Shader.PropertyToID("bonesBuffer");
    public static readonly int bindBuffer = Shader.PropertyToID("bindBuffer");

    public static readonly int _PropertiesBuffer = Shader.PropertyToID("_PropertiesBuffer");
    public static readonly int _TempPropBuffer = Shader.PropertyToID("_TempPropBuffer");
    public static readonly int _CameraForward = Shader.PropertyToID("_CameraForward");
    public static readonly int _CameraNearPos = Shader.PropertyToID("_CameraNearPos");
    public static readonly int _CameraFarPos = Shader.PropertyToID("_CameraFarPos");
    public static readonly int _XYPlaneTexture = Shader.PropertyToID("_XYPlaneTexture");
    public static readonly int _ZPlaneTexture = Shader.PropertyToID("_ZPlaneTexture");
    public static readonly int _PointLightTexture = Shader.PropertyToID("_PointLightTexture");
    public static readonly int _AllPointLight = Shader.PropertyToID("_AllPointLight");
    public static readonly int _AllSpotLight = Shader.PropertyToID("_AllSpotLight");
    public static readonly int _PointLightIndexBuffer = Shader.PropertyToID("_PointLightIndexBuffer");
    public static readonly int _SpotLightIndexBuffer = Shader.PropertyToID("_SpotLightIndexBuffer");

    public static readonly int heightMapBuffer = Shader.PropertyToID("heightMapBuffer");
    public static readonly int triangleBuffer = Shader.PropertyToID("triangleBuffer");
    public static readonly int _MeshSize = Shader.PropertyToID("_MeshSize");
    public static readonly int _LightFlag = Shader.PropertyToID("_LightFlag");
    public static readonly int _CubeShadowMapArray = Shader.PropertyToID("_CubeShadowMapArray");
    public static readonly int _SpotMapArray = Shader.PropertyToID("_SpotMapArray");
    public static readonly int _TemporalWeight = Shader.PropertyToID("_TemporalWeight");
    public static readonly int _MaxDistance = Shader.PropertyToID("_MaxDistance");
    public static readonly int _VolumeTex = Shader.PropertyToID("_VolumeTex");
    public static readonly int _RandomSeed = Shader.PropertyToID("_RandomSeed");
    public static readonly int _LastVolume = Shader.PropertyToID("_LastVolume");
    public static readonly int _NearFarClip = Shader.PropertyToID("_NearFarClip");
    public static readonly int _CameraClipDistance = Shader.PropertyToID("_CameraClipDistance");
    public static readonly int _Screen_TexelSize = Shader.PropertyToID("_Screen_TexelSize");
    public static readonly int _PointLightCount = Shader.PropertyToID("_PointLightCount");
    public static readonly int _SpotLightCount = Shader.PropertyToID("_SpotLightCount");
    public static readonly int _AllFogVolume = Shader.PropertyToID("_AllFogVolume");
    public static readonly int _FogVolumeCount = Shader.PropertyToID("_FogVolumeCount");
    public static readonly int _SceneOffset = Shader.PropertyToID("_SceneOffset");
    public static readonly int _BackupMap = Shader.PropertyToID("_BackupMap");
    public static readonly int _CameraMotionVectorsTexture = Shader.PropertyToID("_CameraMotionVectorsTexture");
    public static readonly int _CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");
    public static readonly int _DepthBufferTexture = Shader.PropertyToID("_DepthBufferTexture");
    public static readonly int _LightMap = Shader.PropertyToID("_LightMap");
    public static readonly int _ReflectionIndices = Shader.PropertyToID("_ReflectionIndices");
    public static readonly int _ReflectionData = Shader.PropertyToID("_ReflectionData");
    public static readonly int _ReflectionTextures = Shader.PropertyToID("_ReflectionTextures");

    public static readonly int _AOROTexture = Shader.PropertyToID("_AOROTexture");
    public static readonly int _DownSampledDepthTexture = Shader.PropertyToID("_DownSampledDepthTexture");
    public static readonly int _DownSampledGBuffer1 = Shader.PropertyToID("_DownSampledGBuffer1");
    public static readonly int _DownSampledGBuffer2 = Shader.PropertyToID("_DownSampledGBuffer2");
    public static readonly int _Coeff = Shader.PropertyToID("_Coeff");
    public static readonly int _Tex3DSize = Shader.PropertyToID("_Tex3DSize");
    public static readonly int _WorldToLocalMatrix = Shader.PropertyToID("_WorldToLocalMatrix");

    public static readonly int _DecalCountBuffer = Shader.PropertyToID("_DecalCountBuffer");
    public static readonly int _DecalBuffer = Shader.PropertyToID("_DecalBuffer");
    public static readonly int _DecalAtlas = Shader.PropertyToID("_DecalAtlas");
    public static readonly int _DecalNormalAtlas = Shader.PropertyToID("_DecalNormalAtlas");

    public static readonly int _AreaLightBuffer = Shader.PropertyToID("_AreaLightBuffer");
    public static readonly int _AreaLightCount = Shader.PropertyToID("_AreaLightCount");
    public static readonly int _VolumetricNoise = Shader.PropertyToID("_VolumetricNoise");
    public static readonly int _DecalNormal = Shader.PropertyToID("_DecalNormal");
    public static readonly int _DecalAlbedo = Shader.PropertyToID("_DecalAlbedo");
    public static readonly int _BackupAlbedoMap = Shader.PropertyToID("_BackupAlbedoMap");
    public static readonly int _BackupNormalMap = Shader.PropertyToID("_BackupNormalMap");
    public static readonly int _OpaqueScale = Shader.PropertyToID("_OpaqueScale");
    public static readonly int _GrabTexture = Shader.PropertyToID("_GrabTexture");

    public static readonly int _TileSize = Shader.PropertyToID("_TileSize");
    public static readonly int _DepthBoundTexture = Shader.PropertyToID("_DepthBoundTexture");
    public static readonly int _PointLightTile = Shader.PropertyToID("_PointLightTile");
    public static readonly int _SpotLightTile = Shader.PropertyToID("_SpotLightTile");
    public static readonly int _CameraPos = Shader.PropertyToID("_CameraPos");
    public static readonly int _TargetDepthTexture = Shader.PropertyToID("_TargetDepthTexture");
}
