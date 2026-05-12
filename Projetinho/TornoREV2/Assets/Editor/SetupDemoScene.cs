using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

/// <summary>
/// Configura o cenário para demonstração com fundo escuro e iluminação dramática.
/// Menu: Tools > Configurar Cenário Demo (Escuro)
/// </summary>
public class SetupDemoScene : Editor
{
    [MenuItem("Tools/Configurar Cenário Demo (Escuro)")]
    public static void SetupDarkScene()
    {
        // ── Câmera: fundo preto sólido ──────────────────────────────────
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.07f); // preto azulado
            EditorUtility.SetDirty(cam.gameObject);
        }

        // ── Skybox e luz ambiente ────────────────────────────────────────
        RenderSettings.skybox = null;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.08f, 0.08f, 0.10f); // quase preto
        RenderSettings.fog = false;

        // ── Luz principal (direcional existente) ─────────────────────────
        Light dirLight = Object.FindFirstObjectByType<Light>();
        if (dirLight != null && dirLight.type == LightType.Directional)
        {
            dirLight.color = new Color(1f, 0.95f, 0.85f);    // branco quente
            dirLight.intensity = 1.4f;
            dirLight.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
            EditorUtility.SetDirty(dirLight.gameObject);
        }

        // ── Luz de preenchimento (fill) — azul fria do lado oposto ──────
        GameObject fillGO = GameObject.Find("_FillLight");
        if (fillGO == null) fillGO = new GameObject("_FillLight");
        fillGO.transform.rotation = Quaternion.Euler(20f, 150f, 0f);
        Light fill = fillGO.GetComponent<Light>();
        if (fill == null) fill = fillGO.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.color = new Color(0.4f, 0.55f, 0.9f);  // azul industrial
        fill.intensity = 0.45f;
        fill.shadows = LightShadows.None;
        EditorUtility.SetDirty(fillGO);

        // ── Luz de borda (rim) — sutil por trás para destacar silhueta ──
        GameObject rimGO = GameObject.Find("_RimLight");
        if (rimGO == null) rimGO = new GameObject("_RimLight");
        rimGO.transform.rotation = Quaternion.Euler(-15f, 200f, 0f);
        Light rim = rimGO.GetComponent<Light>();
        if (rim == null) rim = rimGO.AddComponent<Light>();
        rim.type = LightType.Directional;
        rim.color = new Color(0.9f, 0.7f, 0.3f);   // âmbar para profundidade
        rim.intensity = 0.25f;
        rim.shadows = LightShadows.None;
        EditorUtility.SetDirty(rimGO);

        // ── Piso escuro (plano simples) ──────────────────────────────────
        GameObject floor = GameObject.Find("_DemoFloor");
        if (floor == null)
        {
            floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "_DemoFloor";
        }
        floor.transform.position = new Vector3(0f, -0.01f, 0f);
        floor.transform.localScale = new Vector3(3f, 1f, 3f);

        // Material escuro para o piso
        Renderer rend = floor.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = "DemoFloorMat";
            mat.color = new Color(0.06f, 0.06f, 0.08f);
            mat.SetFloat("_Smoothness", 0.3f);
            mat.SetFloat("_Metallic", 0.1f);
            rend.sharedMaterial = mat;
            AssetDatabase.CreateAsset(mat, "Assets/Materials/DemoFloorMat.mat");
        }

        // Desativa colisão do piso (não precisamos para a demo)
        Collider col = floor.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        EditorUtility.SetDirty(floor);

        // ── Salvar cena ──────────────────────────────────────────────────
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        AssetDatabase.SaveAssets();

        Debug.Log("[SetupDemoScene] Cenário demo configurado: fundo escuro + 3 luzes + piso.");
        EditorUtility.DisplayDialog(
            "Cenário Demo Configurado",
            "Fundo preto + luz principal quente + fill azul + rim âmbar + piso escuro.\n\n" +
            "Pressione Ctrl+S para salvar, depois Play para rodar a demo.",
            "OK");
    }
}
