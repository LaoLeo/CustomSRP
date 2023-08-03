using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    const string bufferName = "Render Camera";

    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    ScriptableRenderContext context;
    Camera camera;

    Lighting lighting = new Lighting();

    public void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing)
    {
        this.context = context;
        this.camera = camera;

        //设置缓冲区的名字
        PrepareBuffer();

        //sceneview中不用剔除几何体
        //将几何体绘制到SceneView中
        PrepareForSceneWindow();

        if (!Cull())
        {
            return;
        }

        Setup();
        lighting.Setup(context, cullingResults);
        //绘制几何体
        DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
        //绘制SRP不支持的着色器类型
        DrawUnsupportedShaders();
        //绘制Gizmos
        DrawGizmos();

        Submit();
 
    }

    CullingResults cullingResults;
    /// <summary>
    /// 剔除
    /// </summary>
    /// <returns></returns>
    private bool Cull()
    {
        ScriptableCullingParameters parameters;
        if (camera.TryGetCullingParameters(out parameters))
        {
            cullingResults = context.Cull(ref parameters);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 将shader的VP矩阵设置相机属性上
    /// </summary>
    private void Setup()
    {
        context.SetupCameraProperties(camera);
        //得到相机的clear flags
        CameraClearFlags flags = camera.clearFlags;
        //设置相机清除状态
        buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, flags == CameraClearFlags.Color, flags == CameraClearFlags.Color ? camera.backgroundColor.linear : Color.clear);
        buffer.BeginSample(SampleName);
        ExcuteBuffer();
    }

    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    static ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");
    /// <summary>
    /// 绘制可见物
    /// </summary>
    void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
    {
        //设置绘制顺序和指定渲染相机
        var sortingSetting = new SortingSettings(camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };
        //设置渲染的Shader Pass和排序模式
        var drawingSetting = new DrawingSettings(unlitShaderTagId, sortingSetting){
            // 设置渲染时批处理的使用状态
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing
        };
        //渲染CustomLit表示的pass块
        drawingSetting.SetShaderPassName(1, litShaderTagId);
        //只绘制RenderQueue为opaque不透明的物体
        var filteringSetting = new FilteringSettings(RenderQueueRange.opaque);

        //1.绘制不透明物体
        context.DrawRenderers(cullingResults, ref drawingSetting, ref filteringSetting);

        //2.绘制天空盒
        context.DrawSkybox(camera);

        sortingSetting.criteria = SortingCriteria.CommonTransparent;
        drawingSetting.sortingSettings = sortingSetting;
        //只绘制RenderQueue为Transparent透明的物体
        filteringSetting.renderQueueRange = RenderQueueRange.transparent;
        //3.绘制透明物体
        context.DrawRenderers(cullingResults, ref drawingSetting, ref filteringSetting);

    }

    /// <summary>
    /// 提交缓冲区渲染命令
    /// </summary>
    void Submit()
    {
        buffer.EndSample(SampleName);
        ExcuteBuffer();
        context.Submit();
    }

    private void ExcuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

}