using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CharacterPhotoURP : MonoBehaviour
{
    [field: SerializeField] public Camera PhotoCamera {get; private set;}  
    [field: SerializeField] public RawImage PhotoImage {get; private set;}
    [field: SerializeField] public int Width {get; private set;} = 256;
    [field: SerializeField] public int Height {get; private set;} = 256;

    private RenderTexture _rt;
    private InterfaceBase _interface;

    [Inject]
    private void Construct(InterfaceBase inter)
    {
        _interface = inter;
    }
    
    private void Start()
    {
        _rt = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
        _rt.Create();

        PhotoCamera.targetTexture = _rt;

        if (PhotoImage != null)
        {
            PhotoImage.texture = _rt; 
            PhotoCamera.clearFlags = CameraClearFlags.SolidColor;
            PhotoCamera.backgroundColor = new Color(0, 0, 0, 0); 
        }
    }

    private Texture2D CaptureOnce(Transform cameraTransform)
    {
        PhotoCamera.transform.SetPositionAndRotation(cameraTransform.position, cameraTransform.rotation);
        PhotoCamera.Render();
        RenderTexture.active = _rt;
        var tex = new Texture2D(_rt.width, _rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        PhotoCamera.enabled = false;
        return tex;
    }

    public void CreatePhoto(Transform cameraTransform)
    {
        var texture = CaptureOnce(cameraTransform);
        var sprite = Sprite.Create(texture, new Rect(0,0,texture.width,texture.height), new Vector2(0.5f,0.5f));
        _interface.Photo.sprite = sprite;
    }

    private void OnDestroy()
    {
        if (_rt != null)
        {
            PhotoCamera.targetTexture = null;
            _rt.Release();
            Destroy(_rt);
        }
    }
}