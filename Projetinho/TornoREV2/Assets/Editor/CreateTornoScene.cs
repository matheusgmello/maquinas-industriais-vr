using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CreateTornoScene
{
    private const string ModelPath = "Assets/TornoMecanico/Torno Mecanico/torno.fbx";
    private const string ScenePath = "Assets/Scenes/TornoMecanico.unity";

    [MenuItem("Tools/Criar Cena do Torno")]
    public static void CreateScene()
    {
        AssetDatabase.Refresh();

        var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        if (model == null)
        {
            EditorUtility.DisplayDialog(
                "Torno nao encontrado",
                "Nao encontrei o modelo em:\n" + ModelPath,
                "OK");
            return;
        }

        Directory.CreateDirectory("Assets/Scenes");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var torno = PrefabUtility.InstantiatePrefab(model) as GameObject;
        if (torno == null)
        {
            torno = Object.Instantiate(model);
        }

        torno.name = "Torno Mecanico";
        torno.transform.position = Vector3.zero;
        torno.transform.rotation = Quaternion.identity;
        torno.transform.localScale = Vector3.one;

        var bounds = CalculateBounds(torno);
        var maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        if (maxSize > 0f)
        {
            var scale = 2.5f / maxSize;
            torno.transform.localScale = Vector3.one * scale;
            bounds = CalculateBounds(torno);
        }

        var lightObject = new GameObject("Directional Light");
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 2.5f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        var cameraObject = new GameObject("Main Camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.Skybox;
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 100f;

        PositionCamera(cameraObject.transform, bounds);

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorSceneManager.OpenScene(ScenePath);
        Selection.activeGameObject = torno;
        EditorGUIUtility.PingObject(torno);
    }

    [MenuItem("Tools/Ajustar Camera Frente do Torno")]
    public static void AdjustCamera()
    {
        var torno = GameObject.Find("Torno Mecanico");
        var camera = Camera.main;
        if (torno == null || camera == null)
        {
            EditorUtility.DisplayDialog(
                "Cena incompleta",
                "Abra a cena do torno ou use Tools > Criar Cena do Torno primeiro.",
                "OK");
            return;
        }

        PositionCamera(camera.transform, CalculateBounds(torno));
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        Selection.activeGameObject = camera.gameObject;
        EditorGUIUtility.PingObject(camera.gameObject);
    }

    private static Bounds CalculateBounds(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(root.transform.position, Vector3.one);
        }

        var bounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }

    private static void PositionCamera(Transform cameraTransform, Bounds bounds)
    {
        var center = bounds.center;
        var distance = Mathf.Max(bounds.size.magnitude * 1.4f, 4f);
        cameraTransform.position = center + new Vector3(0f, bounds.extents.y * 0.6f + 0.8f, distance);
        cameraTransform.LookAt(center);
    }
}
