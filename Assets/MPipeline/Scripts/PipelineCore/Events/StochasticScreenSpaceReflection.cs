using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Collections;
using static Unity.Mathematics.math;
using UnityEngine.Rendering.PostProcessing;
using Random = Unity.Mathematics.Random;
namespace MPipeline
{
    [System.Serializable]
    public unsafe class StochasticScreenSpaceReflection
    {
        public bool enabled;

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [Header("Trace Property")]
        private Random rand;

        [Range(0.05f, 5)]
        [SerializeField]
        float Thickness = 0.1f;

        [Range(0f, 1f)]
        [SerializeField]
        float brdfBias = 0.7f;

        [Range(0, 0.5f)]
        [SerializeField]
        float ScreenFade = 0.1f;


        [Range(64, 512)]
        [SerializeField]
        int HiZ_RaySteps = 64;


        [Range(0, 4)]
        [SerializeField]
        int HiZ_MaxLevel = 10;


        [Range(0, 2)]
        [SerializeField]
        int HiZ_StartLevel = 1;


        [Range(0, 2)]
        [SerializeField]
        int HiZ_StopLevel = 0;



        [Header("Filtter Property")]

        [Range(0, 0.99f)]
        [SerializeField]
        float TemporalWeight = 0.98f;


        [Range(0.01f, 5f)]
        [SerializeField]
        float MaximumAllowedTemporalDepthBias = 0.1f;



        [Header("DeBug Property")]


        [SerializeField]
        bool RunTimeDebugMod = true;



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private Material StochasticScreenSpaceReflectionMaterial;
        private ComputeBuffer ssrDatas;
        private PreviousDepthData prevDepthData;
        private PropertySetEvent propertySetEvent;


        /*
        private static int SSR_NumSteps_HiZ_ID = Shader.PropertyToID("_SSR_NumSteps_HiZ");
        private static int SSR_ScreenFade_ID = Shader.PropertyToID("_SSR_ScreenFade");
        private static int SSR_Thickness_ID = Shader.PropertyToID("_SSR_Thickness");
        private static int SSR_TemporalScale_ID = Shader.PropertyToID("_SSR_TemporalScale");
        private static int SSR_TemporalWeight_ID = Shader.PropertyToID("_SSR_TemporalWeight");
        private static int SSR_ScreenSize_ID = Shader.PropertyToID("_SSR_ScreenSize");
        private static int SSR_RayCastSize_ID = Shader.PropertyToID("_SSR_RayCastSize");
        
        private static int SSR_HiZ_MaxLevel_ID = Shader.PropertyToID("_SSR_HiZ_MaxLevel");
        private static int SSR_HiZ_StartLevel_ID = Shader.PropertyToID("_SSR_HiZ_StartLevel");
        private static int SSR_HiZ_StopLevel_ID = Shader.PropertyToID("_SSR_HiZ_StopLevel");
        */
        private static int SSR_HiZ_PrevDepthLevel_ID = Shader.PropertyToID("_SSR_HiZ_PrevDepthLevel");
        private static int SSR_HierarchicalDepth_ID = Shader.PropertyToID("_SSR_HierarchicalDepth_RT");
        private static int SSR_SceneColor_ID = Shader.PropertyToID("_SSR_SceneColor_RT");
        private static int _SSRDatas = Shader.PropertyToID("_SSRDatas");


        private static int SSR_Trace_ID = Shader.PropertyToID("_SSR_RayCastRT");
        private static int SSR_GetSSRColor_ID = Shader.PropertyToID("_SSR_GetSSRColor_RT");
        private static int SSR_TemporalPrev_ID = Shader.PropertyToID("_SSR_TemporalPrev_RT");
        private static int SSR_TemporalCurr_ID = Shader.PropertyToID("_SSR_TemporalCurr_RT");

        /*
        private static int SSR_ProjectionMatrix_ID = Shader.PropertyToID("_SSR_ProjectionMatrix");
        private static int SSR_InverseProjectionMatrix_ID = Shader.PropertyToID("_SSR_InverseProjectionMatrix");
        private static int SSR_WorldToCameraMatrix_ID = Shader.PropertyToID("_SSR_WorldToCameraMatrix");
        private static int SSR_CameraToWorldMatrix_ID = Shader.PropertyToID("_SSR_CameraToWorldMatrix");
        private static int SSR_ProjectToPixelMatrix_ID = Shader.PropertyToID("_SSR_ProjectToPixelMatrix");
        */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private System.Func<PipelineCamera, SSRCameraData> getDataFunc;
        public void Init(PipelineResources res)
        {
            getDataFunc = (c) => new SSRCameraData(new Vector2Int(c.cam.pixelWidth, c.cam.pixelHeight), 2);
            StochasticScreenSpaceReflectionMaterial = new Material(res.shaders.ssrShader);
            ssrDatas = new ComputeBuffer(1, sizeof(SSRData));
            rand = new Random((uint)System.Guid.NewGuid().GetHashCode());
            propertySetEvent = RenderPipeline.GetEvent<PropertySetEvent>();
        }

        public void Dispose()
        {
            Object.DestroyImmediate(StochasticScreenSpaceReflectionMaterial);
            ssrDatas.Dispose();
        }

        public void PreRender(PipelineCamera cam)
        {
            prevDepthData = IPerCameraData.GetProperty(cam, (cc) => new PreviousDepthData(new Vector2Int(cc.cam.pixelWidth, cc.cam.pixelHeight)));
            prevDepthData.targetObject = this;
        }

        public void Render(ref PipelineCommandData data, PipelineCamera cam, PropertySetEvent proper)
        {
            SSRCameraData cameraData = IPerCameraData.GetProperty(cam, getDataFunc);
            SSR_UpdateVariable(cameraData, cam.cam, ref data, proper);
            RenderScreenSpaceReflection(data.buffer, cameraData, cam);
        }


        ////////////////////////////////////////////////////////////////SSR Function////////////////////////////////////////////////////////////////
        public bool MaterialEnabled()
        {
            return StochasticScreenSpaceReflectionMaterial;
        }
        private struct SSRData
        {
            public float brdfBias;
            public float ScreenFade;
            public float Thickness;
            public int HiZ_RaySteps;
            public int HiZ_MaxLevel;
            public int HiZ_StartLevel;
            public int HiZ_StopLevel;
            public float MaximumBiasAllowed;
            public float TemporalWeight;
            public float2 ScreenSize;
            public float2 rayCastSize;
            public float4x4 viewProjection;
            public float4x4 projection;
            public float4x4 inverseProj;
            public float4x4 worldToCamera;
            public float4x4 cameraToWorld;
            public float4x4 projToPixelMatrix;
            public float4x4 inverseLastVP;
        }

        private void SSR_UpdateVariable(SSRCameraData cameraData, Camera RenderCamera, ref PipelineCommandData data, PropertySetEvent proper)
        {
            Vector2Int CameraSize = new Vector2Int(RenderCamera.pixelWidth, RenderCamera.pixelHeight);
            CommandBuffer buffer = data.buffer;
            cameraData.UpdateCameraSize(CameraSize, 2);
            buffer.SetGlobalMatrix(ShaderIDs._VP, proper.VP);
            Matrix4x4 proj = GL.GetGPUProjectionMatrix(RenderCamera.projectionMatrix, false);
            Matrix4x4 warpToScreenSpaceMatrix = Matrix4x4.identity;
            Vector2 HalfCameraSize = new Vector2(CameraSize.x, CameraSize.y) / 2;
            warpToScreenSpaceMatrix.m00 = HalfCameraSize.x; warpToScreenSpaceMatrix.m03 = HalfCameraSize.x;
            warpToScreenSpaceMatrix.m11 = HalfCameraSize.y; warpToScreenSpaceMatrix.m13 = HalfCameraSize.y;

            Matrix4x4 SSR_ProjectToPixelMatrix = warpToScreenSpaceMatrix * proj;
            NativeArray<SSRData> dataArr = new NativeArray<SSRData>(1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            SSRData* ptr = dataArr.Ptr();
            ptr->ScreenFade = ScreenFade;
            ptr->Thickness = Thickness;
            ptr->HiZ_RaySteps = HiZ_RaySteps;
            ptr->HiZ_MaxLevel = HiZ_MaxLevel;
            ptr->HiZ_StartLevel = HiZ_StartLevel;
            ptr->HiZ_StopLevel = HiZ_StopLevel;
            ptr->MaximumBiasAllowed = MaximumAllowedTemporalDepthBias;
            ptr->TemporalWeight = TemporalWeight;
            ptr->ScreenSize = float2(CameraSize.x, CameraSize.y);
            ptr->rayCastSize = float2(CameraSize.x, CameraSize.y) / 2;
            ptr->projection = proj;
            ptr->inverseProj = proj.inverse;
            ptr->viewProjection = proper.VP;
            ptr->worldToCamera = RenderCamera.worldToCameraMatrix;
            ptr->cameraToWorld = RenderCamera.cameraToWorldMatrix;
            ptr->projToPixelMatrix = SSR_ProjectToPixelMatrix;
            ptr->brdfBias = brdfBias;
            ptr->inverseLastVP = propertySetEvent.inverseLastViewProjection;
            ssrDatas.SetData(dataArr);
            buffer.SetGlobalBuffer(_SSRDatas, ssrDatas);
            dataArr.Dispose();
        }

        private void RenderScreenSpaceReflection(CommandBuffer ScreenSpaceReflectionBuffer, SSRCameraData camData, PipelineCamera cam)
        {
            Vector2Int resolution = new Vector2Int(cam.cam.pixelWidth, cam.cam.pixelHeight);
            
            //////Gte HiZ_DEPTHrt//////
            ScreenSpaceReflectionBuffer.CopyTexture(ShaderIDs._CameraDepthTexture, 0, 0, camData.SSR_HierarchicalDepth_RT, 0, 0);
            for (int i = 1; i < 9; ++i)
            {
                ScreenSpaceReflectionBuffer.SetGlobalInt(SSR_HiZ_PrevDepthLevel_ID, i - 1);
                ScreenSpaceReflectionBuffer.SetRenderTarget(camData.SSR_HierarchicalDepth_BackUp_RT, i);
                ScreenSpaceReflectionBuffer.DrawMesh(GraphicsUtility.mesh, Matrix4x4.identity, StochasticScreenSpaceReflectionMaterial, 0, 0);
                ScreenSpaceReflectionBuffer.CopyTexture(camData.SSR_HierarchicalDepth_BackUp_RT, 0, i, camData.SSR_HierarchicalDepth_RT, 0, i);
            }
            ScreenSpaceReflectionBuffer.SetGlobalTexture(SSR_HierarchicalDepth_ID, camData.SSR_HierarchicalDepth_RT);

            //////Set SceneColorRT//////
            ScreenSpaceReflectionBuffer.CopyTexture(cam.targets.renderTargetIdentifier, 0, 0, camData.SSR_SceneColor_RT, 0, 0);
            ScreenSpaceReflectionBuffer.SetGlobalTexture(SSR_SceneColor_ID, camData.SSR_SceneColor_RT);
            ScreenSpaceReflectionBuffer.GenerateMips(camData.SSR_SceneColor_RT);
            ScreenSpaceReflectionBuffer.GetTemporaryRT(SSR_Trace_ID, resolution.x / 2, resolution.y / 2, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            ScreenSpaceReflectionBuffer.GetTemporaryRT(SSR_GetSSRColor_ID, resolution.x, resolution.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            ScreenSpaceReflectionBuffer.SetRenderTarget(SSR_GetSSRColor_ID);
            ScreenSpaceReflectionBuffer.ClearRenderTarget(false, true, new Color(0, 0, 0, 0));
            //////RayCasting//////
            for (int i = 0; i < 2; ++i)
            {
                ScreenSpaceReflectionBuffer.SetGlobalVector(ShaderIDs._RandomSeed, (float4)(rand.NextDouble4()));
                ScreenSpaceReflectionBuffer.BlitSRT(SSR_Trace_ID, StochasticScreenSpaceReflectionMaterial, 1);
                //////GetSSRColor//////
                ScreenSpaceReflectionBuffer.BlitSRT(SSR_GetSSRColor_ID, StochasticScreenSpaceReflectionMaterial, 2);
            }
            //////Temporal filter//////
            ScreenSpaceReflectionBuffer.GetTemporaryRT(SSR_TemporalCurr_ID, resolution.x, resolution.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            ScreenSpaceReflectionBuffer.SetGlobalTexture(SSR_TemporalPrev_ID, camData.SSR_TemporalPrev_RT);
            ScreenSpaceReflectionBuffer.SetGlobalTexture(ShaderIDs._LastFrameDepthTexture, prevDepthData.SSR_PrevDepth_RT);
            ScreenSpaceReflectionBuffer.BlitSRT(SSR_TemporalCurr_ID, StochasticScreenSpaceReflectionMaterial, 3);
            ScreenSpaceReflectionBuffer.CopyTexture(SSR_TemporalCurr_ID, 0, 0, camData.SSR_TemporalPrev_RT, 0, 0);
            if (prevDepthData.targetObject == this)
            {
                prevDepthData.UpdateCameraSize(resolution);
                ScreenSpaceReflectionBuffer.CopyTexture(ShaderIDs._CameraDepthTexture, 0, 0, prevDepthData.SSR_PrevDepth_RT, 0, 0);
            }
            ScreenSpaceReflectionBuffer.ReleaseTemporaryRT(SSR_Trace_ID);
            ScreenSpaceReflectionBuffer.ReleaseTemporaryRT(SSR_GetSSRColor_ID);
            ScreenSpaceReflectionBuffer.ReleaseTemporaryRT(SSR_TemporalCurr_ID);
        }
    }
    public class PreviousDepthData : IPerCameraData
    {
        public object targetObject;
        public Vector2 CameraSize { get; private set; }
        public RenderTexture SSR_PrevDepth_RT;
        public PreviousDepthData(Vector2Int currentSize)
        {
            CameraSize = currentSize;
            SSR_PrevDepth_RT = new RenderTexture(currentSize.x, currentSize.y, 0, RenderTextureFormat.RFloat);
            SSR_PrevDepth_RT.filterMode = FilterMode.Bilinear;
            SSR_PrevDepth_RT.Create();
        }
        public bool UpdateCameraSize(Vector2Int currentSize)
        {
            if (CameraSize == currentSize) return false;
            CameraSize = currentSize;
            SSRCameraData.ChangeSet(SSR_PrevDepth_RT, currentSize.x, currentSize.y, 0, RenderTextureFormat.RFloat);
            return true;
        }
        public override void DisposeProperty()
        {
            SSRCameraData.CheckAndRelease(ref SSR_PrevDepth_RT);
        }
    }
    public class SSRCameraData : IPerCameraData
    {
        public Vector2 CameraSize { get; private set; }
        public int RayCastingResolution { get; private set; }
        public RenderTexture SSR_TemporalPrev_RT, SSR_SceneColor_RT, SSR_HierarchicalDepth_RT, SSR_HierarchicalDepth_BackUp_RT;
        public static void CheckAndRelease(ref RenderTexture targetRT)
        {
            if (targetRT && targetRT.IsCreated())
            {
                Object.DestroyImmediate(targetRT);
                targetRT = null;
            }
        }

        public SSRCameraData(Vector2Int currentSize, int targetResolution)
        {
            CameraSize = currentSize;
            RayCastingResolution = targetResolution;

            SSR_SceneColor_RT = new RenderTexture(currentSize.x, currentSize.y, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            SSR_SceneColor_RT.filterMode = FilterMode.Trilinear;
            SSR_SceneColor_RT.useMipMap = true;
            SSR_SceneColor_RT.autoGenerateMips = false;

            SSR_HierarchicalDepth_RT = new RenderTexture(currentSize.x, currentSize.y, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            SSR_HierarchicalDepth_RT.filterMode = FilterMode.Point;
            SSR_HierarchicalDepth_RT.useMipMap = true;
            SSR_HierarchicalDepth_RT.autoGenerateMips = false;

            SSR_HierarchicalDepth_BackUp_RT = new RenderTexture(currentSize.x, currentSize.y, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            SSR_HierarchicalDepth_BackUp_RT.filterMode = FilterMode.Point;
            SSR_HierarchicalDepth_BackUp_RT.useMipMap = true;
            SSR_HierarchicalDepth_BackUp_RT.autoGenerateMips = false;

            SSR_TemporalPrev_RT = new RenderTexture(currentSize.x, currentSize.y, 0, RenderTextureFormat.ARGBHalf);
            SSR_TemporalPrev_RT.filterMode = FilterMode.Bilinear;
            SSR_SceneColor_RT.Create();
            SSR_HierarchicalDepth_RT.Create();
            SSR_HierarchicalDepth_BackUp_RT.Create();
            SSR_TemporalPrev_RT.Create();

        }

        public static void ChangeSet(RenderTexture targetRT, int width, int height, int depth, RenderTextureFormat format)
        {
            targetRT.Release();
            targetRT.width = width;
            targetRT.height = height;
            targetRT.depth = depth;
            targetRT.format = format;
            targetRT.Create();
        }

        public bool UpdateCameraSize(Vector2Int currentSize, int targetResolution)
        {
            if (CameraSize == currentSize && RayCastingResolution == targetResolution) return false;
            CameraSize = currentSize;
            RayCastingResolution = targetResolution;
            ChangeSet(SSR_SceneColor_RT, currentSize.x, currentSize.y, 0, RenderTextureFormat.DefaultHDR);
            ChangeSet(SSR_HierarchicalDepth_RT, currentSize.x, currentSize.y, 0, RenderTextureFormat.RFloat);
            ChangeSet(SSR_HierarchicalDepth_BackUp_RT, currentSize.x, currentSize.y, 0, RenderTextureFormat.RFloat);
            ChangeSet(SSR_TemporalPrev_RT, currentSize.x, currentSize.y, 0, RenderTextureFormat.ARGBHalf);
            return true;
        }

        public override void DisposeProperty()
        {
            CheckAndRelease(ref SSR_SceneColor_RT);
            CheckAndRelease(ref SSR_HierarchicalDepth_RT);
            CheckAndRelease(ref SSR_HierarchicalDepth_BackUp_RT);
            CheckAndRelease(ref SSR_TemporalPrev_RT);
           
        }
    }
}