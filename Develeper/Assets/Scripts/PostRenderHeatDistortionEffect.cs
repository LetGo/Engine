using UnityEngine;
public class PostRenderHeatDistortionEffect : PostRenderBase
{
    public float m_fHeatTime = 0.5f;
    public float m_fHeatForce = 0.0f;
    //public Texture curTexture;  
    public Texture texture;

    private float m_fNowTime;
    private int m_nCount;
    private bool m_bHeating = false;

    private void Awake()
    {
        string path = "map/prefab/commontexture/Distortion2.unity3d";
//         Engine.ITexture mapTex = null;
//         Engine.RareEngine.Instance().GetRenderSystem().CreateTexture(ref path, ref mapTex, null, null, Engine.TaskPriority.TaskPriority_Immediate);
// 
//         texture = mapTex.GetTexture();

        shader = Shader.Find("HeatDistortion");
    }

    private void OnEnable()
    {
        m_fNowTime = Time.time;

        float fTime = Random.Range(20, 60);
        this.Invoke("StartHeatDistortion", fTime);
        this.Invoke("EndHeatDistortion", fTime + 8);
    }

    private void OnDisable()
    {
        this.CancelInvoke();
    }

    private void StartHeatDistortion()
    {
        this.InvokeRepeating("AddHeatForce", 0.01f, 0.5f);
    }

    private void EndHeatDistortion()
    {
        //methodName方法名
        //time多少秒后执行
        //repeatRate重复执行间隔
        this.InvokeRepeating("SubHeatForce", 0.01f, 0.5f);
    }

    private void NextStart()
    {
        float fTime = Random.Range(20, 60);
        this.Invoke("StartHeatDistortion", fTime);
        this.Invoke("EndHeatDistortion", fTime + 8);
    }
    private void SubHeatForce()
    {
        m_fHeatForce -= 0.001f;
        if (m_fHeatForce < 0f)
            m_fHeatForce = 0f;

        m_nCount += 1;
        if (m_nCount == 10)
        {
            m_nCount = 0;
            this.CancelInvoke("SubHeatForce");

            NextStart();
            m_bHeating = false;
        }
    }
    private void AddHeatForce()
    {
        m_bHeating = true;
        m_fHeatForce += 0.001f;

        m_nCount += 1;
        if (m_nCount == 10)
        {
            m_nCount = 0;
            this.CancelInvoke("AddHeatForce");
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //material.SetTexture("_MainTex", source);  
        if (m_bHeating)
        {
            material.SetTexture("_NoiseTex", texture);
            material.SetFloat("_HeatTime", m_fHeatTime);
            material.SetFloat("_HeatForce", m_fHeatForce);
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
