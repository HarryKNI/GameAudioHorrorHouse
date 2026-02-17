using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace RealisticHorrorGameSystem
{
    public class MonitorScript : MonoBehaviour
    {
        public Camera sourceCamera;
        public Animation monitor_animation;
        public RawImage targetRawImage;
        public GameObject Screen;
        private RenderTexture rt;
        public bool matchAspect = true;
        [Header("RT Settings")]
        public int width = 1024;
        public int height = 576;
        public RenderTextureFormat format = RenderTextureFormat.ARGBHalf;
        public bool useMipMaps = false;
        public int antiAliasing = 1;
        public bool isOn = false;
        public AudioSource audioSource;
        public Material grayscaleMaterial;

        void OnEnable()
        {
            CreateRT();
            Bind();
        }

        void OnDisable()
        {
            Unbind();
            ReleaseRT();
        }

        void CreateRT()
        {
            if (rt != null) return;

            rt = new RenderTexture(width, height, 0, format)
            {
                useMipMap = useMipMaps,
                autoGenerateMips = false,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                antiAliasing = Mathf.Max(1, antiAliasing),
                name = "Canvas_Live_RT"
            };
            rt.Create();
        }

        void Bind()
        {
            if (sourceCamera != null) sourceCamera.targetTexture = rt;
            if (targetRawImage != null)
            {
                targetRawImage.texture = rt;
                if (grayscaleMaterial != null)
                    targetRawImage.material = grayscaleMaterial;
            }

            if (matchAspect && targetRawImage != null)
            {
                var fitter = targetRawImage.GetComponent<AspectRatioFitter>();
                if (fitter == null) fitter = targetRawImage.gameObject.AddComponent<AspectRatioFitter>();
                fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                fitter.aspectRatio = (float)width / height;
            }
        }

        void Unbind()
        {
            if (sourceCamera != null) sourceCamera.targetTexture = null;
            if (targetRawImage != null) targetRawImage.texture = null;
        }

        public void Interact()
        {
            if(!isOn)
            {
                if (sourceCamera != null) sourceCamera.gameObject.SetActive(true);
                if (sourceCamera != null) sourceCamera.enabled = true;
                if (targetRawImage != null) targetRawImage.enabled = true;
                isOn = true;
                Screen.SetActive(true);
                monitor_animation.Play("Custom_Animation_CameraSwitch_On");
            }
            else
            {
                if (sourceCamera != null) sourceCamera.gameObject.SetActive(false);
                if (sourceCamera != null) sourceCamera.enabled = false;
                if (targetRawImage != null) targetRawImage.enabled = false;
                isOn = false;
                Screen.SetActive(false);
                monitor_animation.Play("Custom_Animation_CameraSwitch_Off");
            }
            audioSource.Play();
        }

        public void RecreateRT(int newW, int newH)
        {
            width = newW; height = newH;
            Unbind();
            ReleaseRT();
            CreateRT();
            Bind();
        }

        void ReleaseRT()
        {
            if (rt != null)
            {
                rt.Release();
                Destroy(rt);
                rt = null;
            }
        }
    }
}
