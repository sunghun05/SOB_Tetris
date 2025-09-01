using UnityEngine;
using System.IO; // ������ �����ϱ� ���� �ʿ��մϴ�.

public class TilemapBaker : MonoBehaviour
{
    // Inspector â���� ����ŷ�� ����� RenderTexture�� �������ݴϴ�.
    public RenderTexture bakeTexture;

    // �� �Լ��� ȣ���ϸ� ����ŷ�� �����մϴ�.
    // [ContextMenu("Bake Tilemap to PNG")]�� ����ϸ� Inspector â����
    // ��ũ��Ʈ ������Ʈ�� ��Ŭ���Ͽ� �� �Լ��� �ٷ� ������ �� �ֽ��ϴ�.
    [ContextMenu("Bake Tilemap to PNG")]
    public void SaveTilemapAsPNG()
    {
        if (bakeTexture == null)
        {
            Debug.LogError("Bake Texture�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        // RenderTexture�� Ȱ��ȭ�ϰ� �ȼ� �����͸� ���� �غ� �մϴ�.
        RenderTexture.active = bakeTexture;

        // RenderTexture�� ũ��� ������ ���ο� Texture2D�� �����մϴ�.
        Texture2D texture2D = new Texture2D(bakeTexture.width, bakeTexture.height, TextureFormat.ARGB32, false);

        // ���� Ȱ��ȭ�� RenderTexture(bakeTexture)�� �ȼ��� Texture2D�� �о�ɴϴ�.
        texture2D.ReadPixels(new Rect(0, 0, bakeTexture.width, bakeTexture.height), 0, 0);
        texture2D.Apply();

        // Ȱ��ȭ�� RenderTexture�� ������� �ǵ����ϴ�.
        RenderTexture.active = null;

        // Texture2D�� �ȼ� �����͸� PNG ������ ����Ʈ �迭�� ���ڵ��մϴ�.
        byte[] bytes = texture2D.EncodeToPNG();

        // ������ Texture2D ��ü�� �� �̻� �ʿ� �����Ƿ� �޸𸮿��� �����մϴ�.
        DestroyImmediate(texture2D);

        // ���� ��θ� �����ϰ� PNG ������ �����մϴ�.
        // Application.dataPath�� Assets ������ ����ŵ�ϴ�.
        string path = Path.Combine(Application.dataPath, "BakedTilemap.png");
        File.WriteAllBytes(path, bytes);

        Debug.Log($"Ÿ�ϸ��� {path} ��ο� ����Ǿ����ϴ�!");

        // �����Ϳ��� ������ �ٷ� ���̵��� AssetDatabase�� ���ΰ�ħ�մϴ�.
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}