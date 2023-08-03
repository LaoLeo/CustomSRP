using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    const string bufferName = "Lighting";

    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    const int maxDirLightCount = 4;
    // static int dirLightColorId = Shader.PropertyToID("_DirectionalLightColor");
    // static int dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection");

    static int dirLightCountId = Shader.PropertyToID("_DirectionalLightCount");
    static int dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors");
    static int dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");
    //存储可见光的颜色和方向
    static Vector4[] dirLightColors = new Vector4[maxDirLightCount];
    static Vector4[] dirLightDirections = new Vector4[maxDirLightCount];
    CullingResults cullingResults;

    public void Setup(ScriptableRenderContext context, CullingResults cullingResults)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        //发送光源数据
        // SetupDirectionalLight();
        SetupLights();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    void SetupDirectionalLight(int index, ref VisibleLight visibleLight)
    {
        // Light light = RenderSettings.sun;
        // //灯光颜色需乘上光强作为最终颜色
        // buffer.SetGlobalVector(dirLightColorId, light.color.linear * light.intensity);
        // buffer.SetGlobalVector(dirLightDirectionId, -light.transform.forward);
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
    }

    //发送多个光源数据
    void SetupLights()
    {
        //得到所有可见光
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;

        int dirLightCount = 0;
        for (int i = 0; i< visibleLights.Length; i++)
        {
            VisibleLight visibleLight = visibleLights[i];
            //如果是方向光，我们才进行数据存储
            if (visibleLight.lightType == LightType.Directional)
            {
                //VisibleLight结构很大，我们改为传递引用而不是传递值，这样不会生成副本
                SetupDirectionalLight(dirLightCount++, ref visibleLight);
                // 最大方向光数限制
                if (dirLightCount >= maxDirLightCount)
                {
                    break;
                }
            }
        }

        buffer.SetGlobalInt(dirLightCountId, dirLightCount);
        buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
    }

}