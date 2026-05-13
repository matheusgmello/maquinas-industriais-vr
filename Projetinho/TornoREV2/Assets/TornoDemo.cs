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

    [Header("Manivela do Carro (estilo volante)")]
    [Tooltip("Eixo LOCAL de rotação da manivela. Se girar no plano errado: tente X (1,0,0), Y (0,1,0) ou Z (0,0,1).")]
    public Vector3 eixoManivela = Vector3.up;

    [Tooltip("Amplitude total de rotação por passada (graus). 540 = 1,5 voltas, 720 = 2 voltas (como volante de carro).")]
    public float amplitudeManivela = 540f;

    [Tooltip("Inverter sentido de rotação (se estiver girando ao contrário).")]
    public bool inverterManivela = true;

    // ── Referências ─────────────────────────────────────────────────────────
    // Peças que giram juntas formando a placa/mandril
    readonly string[] _nomesPlaca = { "EIXO ÁRVORE", "CASTANHA 1", "CASTANHA 2", "CASTANHA 3" };
    Transform[] _pecasPlaca;

    Transform _carroLong;
    Transform _manivela;        // MANIVELA CARRO
    Vector3   _manivelaCenter;  // centro real da mesh (bounds) em espaço local
    Transform _torre;           // TORRE DE FERRAMENTA

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

        // Calcula o centro VISUAL real da manivela considerando TODOS os Renderers
        // (próprio + filhos). Necessário porque o pivot do FBX costuma estar
        // deslocado — sem isso, a peça orbita em vez de girar no lugar.
        if (_manivela != null)
        {
            var rends = _manivela.GetComponentsInChildren<Renderer>();
            if (rends.Length > 0)
            {
                Bounds world = rends[0].bounds;
                for (int i = 1; i < rends.Length; i++) world.Encapsulate(rends[i].bounds);
                // Converte o centro world em local (relativo ao transform da manivela)
                _manivelaCenter = _manivela.InverseTransformPoint(world.center);
            }
            else
            {
                _manivelaCenter = Vector3.zero;
            }
            Debug.Log($"[TornoDemo] Centro VISUAL local da MANIVELA CARRO: {_manivelaCenter} (renderers: {rends.Length})");
        }

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

        // (A manivela agora é controlada pela coroutina RotateManivelaVolante,
        //  sincronizada com o avanço do carro — estilo volante de carro.)
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
        // Inicia a manivela em paralelo (rotação tipo volante, sincronizada com o carro)
        Coroutine manivCo1 = StartCoroutine(RotateManivelaVolante(amplitudeManivela, 4f));
        yield return StartCoroutine(
            MovePingPong(_carroLong, _carroOrigPos, deslocamentoCarro, 4f));
        if (manivCo1 != null) yield return manivCo1;
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
        Coroutine manivCo2 = StartCoroutine(RotateManivelaVolante(amplitudeManivela * 0.7f, 3f));
        yield return StartCoroutine(
            MovePingPong(_carroLong, pos2, deslocamentoCarro * 0.7f, 3f));
        if (manivCo2 != null) yield return manivCo2;
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

    /// <summary>
    /// Rotação tipo VOLANTE DE CARRO:
    /// - Vai de 0 → +amplitude durante a IDA do carro (ex: 540° para a direita)
    /// - Volta de +amplitude → 0 durante a VOLTA do carro (gira de volta ao centro)
    /// Total: 1 ida e 1 volta como o motorista virando o volante e desfazendo.
    /// </summary>
    IEnumerator RotateManivelaVolante(float amplitudeGraus, float duracaoTotal)
    {
        if (_manivela == null) { yield return Espera(duracaoTotal); yield break; }

        float sentido       = inverterManivela ? -1f : 1f;
        float durReal       = duracaoTotal / velocidade;
        float meio          = durReal * 0.5f;
        float anguloAtual   = 0f;   // ângulo já acumulado em RotateAround
        float anguloAnterior = 0f;

        // ── IDA: 0° → +amplitude ────────────────────────────────────────────
        float elapsed = 0f;
        while (elapsed < meio)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / meio);
            anguloAtual = Mathf.Lerp(0f, amplitudeGraus * sentido, t);
            float delta = anguloAtual - anguloAnterior;
            // Recalcula center+axis a cada frame (manivela acompanha o carro em movimento)
            Vector3 worldCenter = _manivela.TransformPoint(_manivelaCenter);
            Vector3 worldAxis   = _manivela.TransformDirection(eixoManivela).normalized;
            _manivela.RotateAround(worldCenter, worldAxis, delta);
            anguloAnterior = anguloAtual;
            yield return null;
        }

        // ── VOLTA: +amplitude → 0° ──────────────────────────────────────────
        elapsed = 0f;
        float anguloInicialVolta = anguloAtual;
        while (elapsed < meio)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / meio);
            anguloAtual = Mathf.Lerp(anguloInicialVolta, 0f, t);
            float delta = anguloAtual - anguloAnterior;
            Vector3 worldCenter = _manivela.TransformPoint(_manivelaCenter);
            Vector3 worldAxis   = _manivela.TransformDirection(eixoManivela).normalized;
            _manivela.RotateAround(worldCenter, worldAxis, delta);
            anguloAnterior = anguloAtual;
            yield return null;
        }
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
