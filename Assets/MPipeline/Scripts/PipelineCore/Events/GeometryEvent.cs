﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace MPipeline
{
    [CreateAssetMenu(menuName = "GPURP Events/Geometry")]
    [RequireEvent(typeof(PropertySetEvent))]
    public unsafe class GeometryEvent : PipelineEvent
    {
        public const bool useHiZ = false;
        HizDepth hizDepth;
        Material linearDrawerMat;
        Material linearMat;
        Material clusterMat;
        public Material debugMat;
        private PropertySetEvent proper;
        public DecalEvent decal;
        private AOEvents ao;
        private ReflectionEvent reflection;
        private Material downSampleMat;
        private Material motionVecMat;
        private RenderTargetIdentifier[] downSampledGBuffers = new RenderTargetIdentifier[3];
        protected override void Init(PipelineResources resources)
        {
            linearMat = new Material(resources.shaders.linearDepthShader);
            linearDrawerMat = new Material(resources.shaders.linearDrawerShader);
            motionVecMat = new Material(resources.shaders.motionVectorShader);
            if (useHiZ)
            {
                hizDepth.InitHiZ(resources, new Vector2(Screen.width, Screen.height));
                clusterMat = new Material(resources.shaders.clusterRenderShader);
            }
            ao = RenderPipeline.GetEvent<AOEvents>();
            reflection = RenderPipeline.GetEvent<ReflectionEvent>();
            proper = RenderPipeline.GetEvent<PropertySetEvent>();
            decal.Init();
            downSampleMat = new Material(resources.shaders.depthDownSample);
        }
        public override bool CheckProperty()
        {
            
            if (useHiZ)
            {
                return linearMat && linearDrawerMat && hizDepth.Check() && clusterMat && motionVecMat;
            }
            else
                return linearMat && linearDrawerMat && motionVecMat;
        }
        protected override void Dispose()
        {
            DestroyImmediate(downSampleMat);
            DestroyImmediate(linearMat);
            DestroyImmediate(linearDrawerMat);
            
            if (useHiZ)
            {
                hizDepth.DisposeHiZ();
                DestroyImmediate(clusterMat);
            }
            linearMat = null;
        }
        public override void PreRenderFrame(PipelineCamera cam, ref PipelineCommandData data)
        {
            decal.PreRenderFrame(cam, ref data);
        }
        public override void FrameUpdate(PipelineCamera cam, ref PipelineCommandData data)
        {
            CommandBuffer buffer = data.buffer;
            RenderClusterOptions options = new RenderClusterOptions
            {
                command = buffer,
                frustumPlanes = proper.frustumPlanes,
                cullingShader = data.resources.shaders.gpuFrustumCulling,
                terrainCompute = data.resources.shaders.terrainCompute
            };
            
            if (useHiZ)
            {
                HizOcclusionData hizOccData = IPerCameraData.GetProperty(cam, () => new HizOcclusionData());
                SceneController.DrawClusterOccDoubleCheck(ref options, ref proper.cullResults, ref hizDepth, hizOccData, clusterMat, linearMat, cam, ref data);
            }
            else SceneController.DrawCluster(ref options, ref cam.targets, ref data, cam.cam, ref proper.cullResults);
            data.buffer.SetRenderTarget(ShaderIDs._CameraMotionVectorsTexture, cam.targets.depthBuffer);
            SceneController.RenderScene(ref data, cam.cam, ref proper.cullResults, "MotionVector", UnityEngine.Rendering.PerObjectData.None, SortingCriteria.None);
            decal.FrameUpdate(cam, ref data);
            //Generate DownSampled GBuffer
            if ((ao != null && ao.Enabled) || (reflection != null && reflection.Enabled && reflection.ssrEvents.enabled))
            {
                int2 res = int2(cam.cam.pixelWidth, cam.cam.pixelHeight) / 2;
                data.buffer.GetTemporaryRT(ShaderIDs._DownSampledGBuffer1, res.x, res.y, 0, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, 1, false);
                data.buffer.GetTemporaryRT(ShaderIDs._DownSampledGBuffer2, res.x, res.y, 0, FilterMode.Point, RenderTextureFormat.ARGB2101010, RenderTextureReadWrite.Linear, 1, false);
                data.buffer.GetTemporaryRT(ShaderIDs._DownSampledDepthTexture, res.x, res.y, 0, FilterMode.Point, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear, 1, false);
                RenderPipeline.AddTempRtToReleaseList(ShaderIDs._DownSampledGBuffer1);
                RenderPipeline.AddTempRtToReleaseList(ShaderIDs._DownSampledGBuffer2);
                RenderPipeline.AddTempRtToReleaseList(ShaderIDs._DownSampledDepthTexture);
                downSampledGBuffers[0] = ShaderIDs._DownSampledDepthTexture;
                downSampledGBuffers[1] = ShaderIDs._DownSampledGBuffer1;
                downSampledGBuffers[2] = ShaderIDs._DownSampledGBuffer2;
                data.buffer.SetRenderTarget(colors: downSampledGBuffers, depth: downSampledGBuffers[0]);
                data.buffer.DrawMesh(GraphicsUtility.mesh, Matrix4x4.identity, downSampleMat, 0, 0);
                //TODO
            }
            data.buffer.BlitSRTWithDepth(ShaderIDs._CameraMotionVectorsTexture, cam.targets.depthBuffer, motionVecMat, 0);
        }
    }
    public class HizOcclusionData : IPerCameraData
    {
        public RenderTexture historyDepth { get; private set; }
        public Vector3 lastFrameCameraUp;
        public HizOcclusionData()
        {
            historyDepth = new RenderTexture(HizDepth.depthRes.x, HizDepth.depthRes.y, 0, RenderTextureFormat.R16, RenderTextureReadWrite.Linear);
            historyDepth.useMipMap = true;
            historyDepth.autoGenerateMips = false;
            historyDepth.enableRandomWrite = false;
            historyDepth.wrapMode = TextureWrapMode.Clamp;
            historyDepth.filterMode = FilterMode.Point;
            historyDepth.Create();
            lastFrameCameraUp = Vector3.up;
        }
        public override void DisposeProperty()
        {
            Object.DestroyImmediate(historyDepth);
        }
    }
}