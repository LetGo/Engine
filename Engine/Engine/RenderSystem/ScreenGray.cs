//*************************************************************************
//	创建日期:	2016-9-29 9:46
//	文件名称:	ScreenGray.cs
//  创 建 人:   	Chengxue.Zhao
//	版权所有:	中青宝
//	说    明:	死亡屏幕灰化效果
//*************************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    class ScreenGray :MonoBehaviour
    {
        private Shader m_curShader;
        private float m_fGrayScaleAmount = 1.0f;
        private Material m_curMaterial;
        public Material material
        {
            get
            {
                if (m_curMaterial == null)
                {
                    m_curMaterial = new Material(m_curShader);
                    m_curMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_curMaterial;
            }
        }
        void Start()
        {
            if (!SystemInfo.supportsImageEffects)
            {
                enabled = false;
                return;
            }

            m_curShader = Resources.Load<Shader>("Shaders/Gray");
            if (m_curShader != null && m_curShader.isSupported == false)
            {
                enabled = false;
            }
        }

        void OnDisable()
        {
            if (m_curMaterial != null)
            {
                DestroyImmediate(m_curMaterial);
            }
        }

        void OnRenderImage(RenderTexture sourceTex,RenderTexture destTex)
        {
            if (m_curShader != null)
            {
                Graphics.Blit(sourceTex, destTex, material);
            }
            else
            {
                Graphics.Blit(sourceTex, destTex);
            }
        }
    }
}
