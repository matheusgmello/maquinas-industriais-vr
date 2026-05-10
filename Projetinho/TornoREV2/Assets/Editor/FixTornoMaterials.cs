using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Editor script para corrigir as cores dos materiais do torno após conversão para URP.
/// Menu: Tools > Corrigir Materiais do Torno
/// </summary>
public class FixTornoMaterials : Editor
{
    // Mapeamento nome do material → cor RGBA
    private static readonly Dictionary<string, Color> MaterialColors = new Dictionary<string, Color>
    {
        { "amarelo",      new Color(0.95f, 0.75f, 0.05f) },  // amarelo vivo
        { "amarelo_bege", new Color(0.85f, 0.78f, 0.55f) },  // bege/amarelo claro
        { "branco",       new Color(0.92f, 0.92f, 0.92f) },  // branco quase puro
        { "cinza",        new Color(0.45f, 0.45f, 0.45f) },  // cinza médio
        { "metal",        new Color(0.55f, 0.55f, 0.58f) },  // cinza metálico
        { "preto",        new Color(0.08f, 0.08f, 0.08f) },  // preto
        { "seta",         new Color(0.85f, 0.75f, 0.05f) },  // amarelo para setas
        { "verde freio",  new Color(0.05f, 0.30f, 0.05f) },  // verde escuro industrial
        { "verde_freio",  new Color(0.05f, 0.30f, 0.05f) },
        { "verde luz",    new Color(0.10f, 0.80f, 0.10f) },  // verde vivo (LED)
        { "verde_luz",    new Color(0.10f, 0.80f, 0.10f) },
        { "vermelho",     new Color(0.80f, 0.05f, 0.05f) },  // vermelho industrial
        { "vidro_",       new Color(0.70f, 0.85f, 0.90f, 0.3f) }, // azul-transparente
        { "vidro",        new Color(0.70f, 0.85f, 0.90f, 0.3f) },
        { "Material.001", new Color(0.50f, 0.50f, 0.50f) },  // cinza padrão
        { "Material.002", new Color(0.40f, 0.40f, 0.42f) },  // cinza escuro
    };

    [MenuItem("Tools/Corrigir Materiais do Torno")]
    public static void FixMaterials()
    {
        string materialsPath = "Assets/TornoMecanico/Materials";
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { materialsPath });

        int fixed_count = 0;
        int skipped = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

            if (mat == null) continue;

            string matName = mat.name.ToLower().Trim();
            Color targetColor = Color.white;
            bool found = false;

            // Busca exata primeiro
            foreach (var entry in MaterialColors)
            {
                if (entry.Key.ToLower() == matName)
                {
                    targetColor = entry.Value;
                    found = true;
                    break;
                }
            }

            // Busca parcial se não encontrou exato
            if (!found)
            {
                foreach (var entry in MaterialColors)
                {
                    if (matName.Contains(entry.Key.ToLower()) || entry.Key.ToLower().Contains(matName))
                    {
                        targetColor = entry.Value;
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                Debug.LogWarning($"[FixTornoMaterials] Sem cor definida para: '{mat.name}' — pulando.");
                skipped++;
                continue;
            }

            // Define a cor no shader URP/Lit
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", targetColor);

            if (mat.HasProperty("_BaseMap"))
                mat.SetColor("_BaseMap", targetColor);

            // Configura metallic/smoothness conforme o tipo
            bool isMetal = matName.Contains("metal");
            bool isGlass = matName.Contains("vidro");

            if (mat.HasProperty("_Metallic"))
                mat.SetFloat("_Metallic", isMetal ? 0.7f : 0.0f);

            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", isMetal ? 0.6f : (isGlass ? 0.9f : 0.3f));

            // Transparência para vidro
            if (isGlass)
            {
                mat.SetFloat("_Surface", 1); // 0=Opaque, 1=Transparent
                mat.renderQueue = 3000;
                mat.SetOverrideTag("RenderType", "Transparent");
            }

            EditorUtility.SetDirty(mat);
            fixed_count++;
            Debug.Log($"[FixTornoMaterials] ✓ {mat.name} → {ColorUtility.ToHtmlStringRGB(targetColor)}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[FixTornoMaterials] Concluído: {fixed_count} materiais corrigidos, {skipped} pulados.");
        EditorUtility.DisplayDialog(
            "Materiais do Torno Corrigidos",
            $"{fixed_count} materiais corrigidos com sucesso!\n{skipped} pulados (sem cor mapeada).",
            "OK"
        );
    }
}
