using UnityEngine;
using UnityEditor;

/// <summary>
/// Adiciona botões de teste individual no Inspector do TornoTeste.
/// Só aparece durante o Play Mode.
/// </summary>
[CustomEditor(typeof(TornoTeste))]
public class TornoTesteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TornoTeste t = (TornoTeste)target;

        EditorGUILayout.Space(10);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Entre em Play Mode para usar os botões de teste.", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField("── Testar peças individualmente ──", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Cada botão anima só aquela peça. Observe e reporte se o movimento está correto.", MessageType.None);
        EditorGUILayout.Space(4);

        if (GUILayout.Button("▶  Mandril (EIXO ÁRVORE)", GUILayout.Height(30)))
            t.TestarMandril();

        if (GUILayout.Button("▶  Castanhas (garras do mandril)", GUILayout.Height(30)))
            t.TestarCastanhas();

        EditorGUILayout.Space(4);

        if (GUILayout.Button("▶  Carro Longitudinal", GUILayout.Height(30)))
            t.TestarCarro();

        if (GUILayout.Button("▶  Carro Transversal", GUILayout.Height(30)))
            t.TestarTransversal();

        if (GUILayout.Button("▶  Manivela do Carro", GUILayout.Height(30)))
            t.TestarManivela();

        EditorGUILayout.Space(4);

        if (GUILayout.Button("▶  Cabeçote Móvel (contraponto)", GUILayout.Height(30)))
            t.TestarCabecote();

        EditorGUILayout.Space(4);

        if (GUILayout.Button("▶  Fuso", GUILayout.Height(30)))
            t.TestarFuso();

        if (GUILayout.Button("▶  Torre de Ferramenta", GUILayout.Height(30)))
            t.TestarTorre();
    }
}
