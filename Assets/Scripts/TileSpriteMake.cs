using UnityEngine;
using System.IO; // 파일을 저장하기 위해 필요합니다.

public class TilemapBaker : MonoBehaviour
{
    // Inspector 창에서 베이킹에 사용할 RenderTexture를 연결해줍니다.
    public RenderTexture bakeTexture;

    // 이 함수를 호출하면 베이킹을 시작합니다.
    // [ContextMenu("Bake Tilemap to PNG")]를 사용하면 Inspector 창에서
    // 스크립트 컴포넌트를 우클릭하여 이 함수를 바로 실행할 수 있습니다.
    [ContextMenu("Bake Tilemap to PNG")]
    public void SaveTilemapAsPNG()
    {
        if (bakeTexture == null)
        {
            Debug.LogError("Bake Texture가 할당되지 않았습니다!");
            return;
        }

        // RenderTexture를 활성화하고 픽셀 데이터를 읽을 준비를 합니다.
        RenderTexture.active = bakeTexture;

        // RenderTexture의 크기와 동일한 새로운 Texture2D를 생성합니다.
        Texture2D texture2D = new Texture2D(bakeTexture.width, bakeTexture.height, TextureFormat.ARGB32, false);

        // 현재 활성화된 RenderTexture(bakeTexture)의 픽셀을 Texture2D로 읽어옵니다.
        texture2D.ReadPixels(new Rect(0, 0, bakeTexture.width, bakeTexture.height), 0, 0);
        texture2D.Apply();

        // 활성화된 RenderTexture를 원래대로 되돌립니다.
        RenderTexture.active = null;

        // Texture2D의 픽셀 데이터를 PNG 형식의 바이트 배열로 인코딩합니다.
        byte[] bytes = texture2D.EncodeToPNG();

        // 생성된 Texture2D 객체는 더 이상 필요 없으므로 메모리에서 제거합니다.
        DestroyImmediate(texture2D);

        // 파일 경로를 지정하고 PNG 파일을 저장합니다.
        // Application.dataPath는 Assets 폴더를 가리킵니다.
        string path = Path.Combine(Application.dataPath, "BakedTilemap.png");
        File.WriteAllBytes(path, bytes);

        Debug.Log($"타일맵이 {path} 경로에 저장되었습니다!");

        // 에디터에서 파일이 바로 보이도록 AssetDatabase를 새로고침합니다.
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}