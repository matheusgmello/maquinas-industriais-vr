using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demonstração automática do torno mecânico com manipulação direta de transforms.
/// Não depende do AnimatorController — evita conflitos entre estados simultâneos.
///
/// Ajuste os campos no Inspector se algum movimento parecer na direção errada.
/// </summary>
public class TornoDemo : MonoBehaviour
{
    [Header("Ciclos")]
    [Tooltip("Número de ciclos completos. 0 = loop infinito.")]
    public int ciclos = 1;

    [Tooltip("Velocidade geral. 1 = normal, 2 = dobro.")]
    [Range(0.25f, 3f)]
    public float velocidade = 1f;

    [Header("Placa / Mandril")]
    [Tooltip("Eixo LOCAL de rotação do mandril. Troque para Y ou Z se não girar corretamente.")]
    public Vector3 eixoPlaca = Vector3.right;

    [Tooltip("Graus por segundo do mandril (240 = 1 volta em 1,5 s).")]
    public float rpmMandril = 240f;

    [Header("Carro Longitudinal")]
    [Tooltip("Deslocamento local do CARRO LONGITUDINAL durante a usinagem.")]
    public Vector3 deslocamentoCarro = new Vector3(0.5f, 0f, 0f);

    [Header("Torre de Ferramenta")]
    [Tooltip("Ângulo de rotação da torre na troca de ferramenta.")]
    public Vector3 rotacaoTorre = new Vector3(0f, 90f, 0f);

    // ── Referências ─────────────────────────────────────────────────────────
    // Peças que giram juntas formando a placa/mandril
    readonly string[] _nomesPlaca = { "EIXO ÁRVORE", "CASTANHA 1", "CASTANHA 2", "CASTANHA 3" };
    Transform[] _pecasPlaca;

    Transform _carroLong;
    Transform _manivela;   // MANIVELA CARRO
    Transform _torre;      // TORRE DE FERRAMENTA

    Vector3 _carroOrigPos;
    bool    _placaGirando;
    bool    _carroAvancando;

    // ────────────────────────────────────────────────────────────────────────

    void Start()
    {
        // Desabilita o Animator para não sobrescrever as transforms diretas
        var anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;

        // Busca as peças da placa (EIXO ÁRVORE + 3 castanhas)
        var pecasEncontradas = new List<Transform>();
        foreach (string nome in _nomesPlaca)
        {
            Transform t = FindDeep(nome);
            if (t != null)
                pecasEncontradas.Add(t);
            else
                Debug.LogWarning($"[TornoDemo] '{nome}' não encontrado — será ignorado.");
        }
        _pecasPlaca = pecasEncontradas.ToArray();

        _carroLong = FindDeep("CARRO LONGITUDINAL");
        _manivela  = FindDeep("MANIVELA CARRO");
        _torre     = FindDeep("TORRE DE FERRAMENTA");

        if (_carroLong == null) Debug.LogWarning("[TornoDemo] 'CARRO LONGITUDINAL' não encontrado!");
        if (_manivela  == null) Debug.LogWarning("[TornoDemo] 'MANIVELA CARRO' não encontrada — ok se não houver.");
        if (_torre     == null) Debug.LogWarning("[TornoDemo] 'TORRE DE FERRAMENTA' não encontrada — ok se não houver.");

        if (_carroLong != null)
            _carroOrigPos = _carroLong.localPosition;

        Debug.Log($"[TornoDemo] Peças da placa encontradas: {_pecasPlaca.Length}/{_nomesPlaca.Length}");
        StartCoroutine(RunDemo());
    }

    void Update()
    {
        float dt = Time.deltaTime * velocidade;

        // Rotação contínua de TODAS as peças do mandril
        if (_placaGirando && _pecasPlaca != null)
            foreach (var p in _pecasPlaca)
                p.Rotate(eixoPlaca, rpmMandril * dt, Space.Self);

        // Rotação da manivela do carro em sincronia com o avanço
        if (_carroAvancando && _manivela != null)
            _manivela.Rotate(Vector3.forward, rpmMandril * 0.5f * dt, Space.Self);
    }

    // ── Sequência principal ─────────────────────────────────────────────────

    IEnumerator RunDemo()
    {
        int feitos = 0;
        while (ciclos == 0 || feitos < ciclos)
        {
            yield return StartCoroutine(Sequencia());
            feitos++;
            if (ciclos == 0 || feitos < ciclos)
                yield return Espera(2f);
        }
        Debug.Log("[TornoDemo] Demonstração concluída.");
    }

    IEnumerator Sequencia()
    {
        // ── Pausa inicial ────────────────────────────────────────────────────
        yield return Espera(1f);

        // ── 1. Liga — mandril começa a girar ────────────────────────────────
        Debug.Log("[TornoDemo] Ligando máquina...");
        _placaGirando = true;
        yield return Espera(1.5f);

        // ── 2. Carro avança — primeira passada de usinagem ──────────────────
        Debug.Log("[TornoDemo] Carro avançando (1ª passada)...");
        _carroAvancando = true;
        yield return StartCoroutine(
            MovePingPong(_carroLong, _carroOrigPos, deslocamentoCarro, 4f));
        _carroAvancando = false;
        yield return Espera(0.8f);

        // ── 3. Troca de ferramenta — torre rotaciona ─────────────────────────
        Debug.Log("[TornoDemo] Trocando ferramenta...");
        yield return StartCoroutine(RotateBy(_torre, rotacaoTorre, 1.2f));
        yield return Espera(0.5f);

        // ── 4. Segunda passada de usinagem ───────────────────────────────────
        Debug.Log("[TornoDemo] Carro avançando (2ª passada)...");
        Vector3 pos2 = _carroLong != null ? _carroLong.localPosition : _carroOrigPos;
        _carroAvancando = true;
        yield return StartCoroutine(
            MovePingPong(_carroLong, pos2, deslocamentoCarro * 0.7f, 3f));
        _carroAvancando = false;
        yield return Espera(0.8f);

        // ── 5. Desliga ───────────────────────────────────────────────────────
        Debug.Log("[TornoDemo] Desligando máquina...");
        _placaGirando = false;
        yield return Espera(1f);
    }

    // ── Helpers de animação ─────────────────────────────────────────────────

    IEnumerator MovePingPong(Transform t, Vector3 origem, Vector3 deslocamento, float duracao)
    {
        if (t == null) { yield return Espera(duracao); yield break; }

        float durReal = duracao / velocidade;
        float meio    = durReal * 0.5f;
        float elapsed = 0f;
        Vector3 alvo  = origem + deslocamento;

        // Ida
        while (elapsed < meio)
        {
            elapsed += Time.deltaTime;
            t.localPosition = Vector3.Lerp(origem, alvo, Mathf.Clamp01(elapsed / meio));
            yield return null;
        }
        t.localPosition = alvo;

        // Volta
        elapsed = 0f;
        while (elapsed < meio)
        {
            elapsed += Time.deltaTime;
            t.localPosition = Vector3.Lerp(alvo, origem, Mathf.Clamp01(elapsed / meio));
            yield return null;
        }
        t.localPosition = origem;
    }

    IEnumerator RotateBy(Transform t, Vector3 eulerDelta, float duracao)
    {
        if (t == null) { yield return Espera(duracao); yield break; }

        float      durReal = duracao / velocidade;
        float      elapsed = 0f;
        Quaternion from    = t.localRotation;
        Quaternion to      = from * Quaternion.Euler(eulerDelta);

        while (elapsed < durReal)
        {
            elapsed += Time.deltaTime;
            t.localRotation = Quaternion.Lerp(from, to, Mathf.Clamp01(elapsed / durReal));
            yield return null;
        }
        t.localRotation = to;
    }

    /// Busca recursiva por nome exato na hierarquia deste GameObject.
    Transform FindDeep(string nomePeca)
    {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>(true))
            if (t.name == nomePeca)
                return t;
        return null;
    }

    WaitForSeconds Espera(float segundos) => new WaitForSeconds(segundos / velocidade);
}
