using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Cria clips de animação e um AnimatorController para o torno mecânico.
/// Menu: Tools > Criar Animações do Torno
/// </summary>
public class CreateTornoAnimations : Editor
{
    [MenuItem("Tools/Criar Animações do Torno")]
    public static void CreateAnimations()
    {
        // Garante pasta Animations
        if (!AssetDatabase.IsValidFolder("Assets/Animations"))
            AssetDatabase.CreateFolder("Assets", "Animations");

        // ── 1. Placa / Mandril girando ─────────────────────────────────────
        // EIXO ÁRVORE é o eixo principal do torno — gira continuamente
        AnimationClip placaClip = CreateRotationClip(
            "Placa_Girando",
            "EIXO ÁRVORE",    // Á = Á
            "localEulerAngles.x",
            fromDeg: 0f,
            toDeg: 360f,
            duration: 1.5f,
            loop: true
        );

        // ── 2. Carro longitudinal avançando e recuando ─────────────────────
        AnimationClip carroClip = CreatePingPongTranslationClip(
            "Carro_Avancando",
            "CARRO LONGITUDINAL",
            axis: "localPosition.x",
            distance: 0.25f,
            duration: 4f
        );

        // ── 3. Manivela do carro girando (sincronizada com o carro) ─────────
        AnimationClip manivelClip = CreateRotationClip(
            "Manivela_Girando",
            "MANIVELA",
            "localEulerAngles.z",
            fromDeg: 0f,
            toDeg: 360f,
            duration: 4f,
            loop: true
        );

        // ── 4. Torre rotacionando (troca de ferramenta) ────────────────────
        AnimationClip torreClip = CreateRotationClip(
            "Torre_Rotacionando",
            "TORRE",
            "localEulerAngles.y",
            fromDeg: 0f,
            toDeg: 90f,
            duration: 1.0f,
            loop: false
        );

        // ── 5. Carro transversal ────────────────────────────────────────────
        AnimationClip carroTransClip = CreatePingPongTranslationClip(
            "Carro_Transversal",
            "CARRO TRANSVERSAL",
            axis: "localPosition.z",
            distance: 0.1f,
            duration: 3f
        );

        // ── AnimatorController ──────────────────────────────────────────────
        string controllerPath = "Assets/Animations/TornoController.controller";
        AnimatorController ctrl = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Parâmetros
        ctrl.AddParameter("PlacaGirando",    AnimatorControllerParameterType.Bool);
        ctrl.AddParameter("CarroAvancando",  AnimatorControllerParameterType.Bool);
        ctrl.AddParameter("TrocaFerramenta", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine sm = ctrl.layers[0].stateMachine;

        // Estados
        AnimatorState idle         = sm.AddState("Idle");
        AnimatorState placa        = sm.AddState("PlacaGirando");
        AnimatorState carro        = sm.AddState("CarroAvancando");
        AnimatorState manivela     = sm.AddState("ManivelGirando");
        AnimatorState torre        = sm.AddState("TrocaFerramenta");
        AnimatorState carroTrans   = sm.AddState("CarroTransversal");

        placa.motion      = placaClip;
        carro.motion      = carroClip;
        manivela.motion   = manivelClip;
        torre.motion      = torreClip;
        carroTrans.motion = carroTransClip;

        sm.defaultState = idle;

        // Transições básicas
        AddBoolTransition(sm, idle,  placa,  ctrl, "PlacaGirando",    true);
        AddBoolTransition(sm, placa, idle,   ctrl, "PlacaGirando",    false);
        AddBoolTransition(sm, idle,  carro,  ctrl, "CarroAvancando",  true);
        AddBoolTransition(sm, carro, idle,   ctrl, "CarroAvancando",  false);

        AnimatorStateTransition torreTransition = sm.AddAnyStateTransition(torre);
        torreTransition.AddCondition(AnimatorConditionMode.If, 0, "TrocaFerramenta");
        torreTransition.hasExitTime = false;
        torreTransition.duration = 0f;

        // ── Aplica Animator ao objeto 'torno' na cena ───────────────────────
        GameObject tornoGO = GameObject.Find("torno");
        if (tornoGO != null)
        {
            Animator anim = tornoGO.GetComponent<Animator>();
            if (anim == null) anim = tornoGO.AddComponent<Animator>();
            anim.runtimeAnimatorController = ctrl;
            EditorUtility.SetDirty(tornoGO);
            Debug.Log("[CreateTornoAnimations] Animator adicionado ao 'torno' na cena.");
        }
        else
        {
            Debug.LogWarning("[CreateTornoAnimations] Objeto 'torno' não encontrado na cena. Adicione o Animator manualmente.");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[CreateTornoAnimations] Concluído! Clips criados:\n" +
                  "  • Placa_Girando.anim\n" +
                  "  • Carro_Avancando.anim\n" +
                  "  • Manivela_Girando.anim\n" +
                  "  • Torre_Rotacionando.anim\n" +
                  "  • Carro_Transversal.anim\n" +
                  "  • TornoController.controller");

        EditorUtility.DisplayDialog(
            "Animações Criadas",
            "5 clips + TornoController criados em Assets/Animations/\n\n" +
            "Selecione 'torno' na Hierarchy e abra\n" +
            "Window > Animation > Animation para visualizar.",
            "OK"
        );
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    static AnimationClip CreateRotationClip(
        string clipName, string childPath, string property,
        float fromDeg, float toDeg, float duration, bool loop)
    {
        var clip = new AnimationClip { name = clipName };

        if (loop)
        {
            var s = AnimationUtility.GetAnimationClipSettings(clip);
            s.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, s);
        }

        var curve = new AnimationCurve(
            new Keyframe(0f, fromDeg),
            new Keyframe(duration, toDeg)
        );
        // Interpolação linear para rotação contínua
        for (int i = 0; i < curve.length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(curve,  i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
        }

        clip.SetCurve(childPath, typeof(Transform), property, curve);

        string path = $"Assets/Animations/{clipName}.anim";
        AssetDatabase.CreateAsset(clip, path);
        return AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
    }

    static AnimationClip CreatePingPongTranslationClip(
        string clipName, string childPath, string axis,
        float distance, float duration)
    {
        var clip = new AnimationClip { name = clipName };

        var s = AnimationUtility.GetAnimationClipSettings(clip);
        s.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, s);

        float half = duration * 0.5f;
        var curve = new AnimationCurve(
            new Keyframe(0f,        0f),
            new Keyframe(half,      distance),
            new Keyframe(duration,  0f)
        );

        clip.SetCurve(childPath, typeof(Transform), axis, curve);

        string path = $"Assets/Animations/{clipName}.anim";
        AssetDatabase.CreateAsset(clip, path);
        return AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
    }

    static void AddBoolTransition(
        AnimatorStateMachine sm,
        AnimatorState from, AnimatorState to,
        AnimatorController ctrl, string param, bool value)
    {
        var t = from.AddTransition(to);
        t.AddCondition(value ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0, param);
        t.hasExitTime = false;
        t.duration = 0.1f;
    }
}
