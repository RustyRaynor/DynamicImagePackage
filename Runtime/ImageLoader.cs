using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Raynor.DynamicImages 
{
    public class ImageLoader : MonoBehaviour
    {
        public delegate void OnImageLoadedHandler();
        public OnImageLoadedHandler OnImageLoaded;

        [SerializeField] List<Image> images;

        Queue<Image> loadingImages;

        private void Start()
        {
            loadingImages = new Queue<Image>();
            OnImageLoaded += NextImage;

            for (int i = 0; i < images.Count; i++)
            {
                loadingImages.Enqueue(images[i]);
            }

            NextImage();
        }

        void NextImage()
        {
            if (loadingImages.Count > 0)
            {
                StartCoroutine(GetImage(loadingImages.Dequeue()));
            }
        }

        IEnumerator GetImage(Image image)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://picsum.photos/200/300");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                if (image.sprite != null)
                {
                    Destroy(image.sprite.texture);
                }

                yield return new WaitForSeconds(0.1f);

                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }

            loadingImages.Enqueue(image);
            OnImageLoaded();
        }
    }
}