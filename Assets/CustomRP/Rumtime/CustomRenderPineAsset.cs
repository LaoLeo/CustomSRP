using UnityEngine;
using UnityEngine.Rendering;


[CreateAssetMenu(menuName = "Rendering/CreateCustomRenderPipeline")]
public class CustomRenderPineAsset : RenderPipelineAsset
{
    [SerializeField]
    //定义合批状态字段
    bool useDynamicBatching = true, useGPUInstancing = true, useSRPBatcher = true;
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline(useDynamicBatching, useGPUInstancing, useSRPBatcher);
    }
}
