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
    [Tooltip("Eixo LOCAL de rotação do mandril.")]
    public Vector3 eixoPlaca = Vector3.right;

    [Tooltip("Graus por segundo do mandril (240 = 1 volta em 1,5 s).")]
    public float rpmMandril = 240f;

    [Header("Castanhas (garras do mandril)")]
    [Tooltip("Distância radial que cada castanha percorre ao fechar (unidades Unity).")]
    public float aberturaMaxCastanha = 0.04f;

    [Tooltip("Duração da abertura/fechamento das castanhas em segundos.")]
    public float duracaoCastanha = 0.6f;

    [Tooltip("Inverte a direção radial das castanhas. Ative se elas abrirem quando deveriam fechar.")]
    public bool inverterCastanhas = false;

    [Header("Carro Longitudinal")]
    [Tooltip("Deslocamento local do CARRO LONGITUDINAL durante a usinagem.")]
    public Vector3 deslocamentoCarro = new Vector3(0.5f, 0f, 0f);

    [Header("Carro Transversal")]
    [Tooltip("Deslocamento local do CARRO TRANSVERSAL durante a usinagem.")]
    public Vector3 deslocamentoTransversal = new Vector3(0f, 0f, 0.12f);

    [Tooltip("Duração do avanço/recuo do carro transversal em segundos.")]
    public float duracaoTransversal = 2.5f;

    [Header("Cabeçote Móvel (Contraponto)")]
    [Tooltip("Deslocamento local do CABEÇOTE MÓVEL ao avançar para fixar a peça.")]
    public Vector3 deslocamentoCabecote = new Vector3(-0.3f, 0f, 0f);

    [Tooltip("Duração do avanço/recuo do cabeçote em segundos.")]
    public float duracaoCabecote = 1.5f;

    [Tooltip("Eixo LOCAL da manivela do cabeçote.")]
    public Vector3 eixoManivelaC = Vector3.right;

    [Tooltip("Amplitude de rotação da manivela do cabeçote (graus).")]
    public float amplitudeManivelaCabecote = 360f;

    [Tooltip("Inverte a direção da manivela do cabeçote.")]
    public bool inverterManivelaCabecote = false;

    [Header("Fuso")]
    [Tooltip("Eixo LOCAL de rotação do fuso.")]
    public Vector3 eixoFuso = Vector3.right;

    [Tooltip("Graus por segundo do fuso (sincronizado com o avanço do carro).")]
    public float rpmFuso = 120f;

    [Tooltip("Inverte o sentido de rotação do fuso.")]
    public bool inverterFuso = false;

    [Header("Torre de Ferramenta")]
    [Tooltip("Ângulo de rotação da torre na troca de ferramenta.")]
    public Vector3 rotacaoTorre = new Vector3(0f, 90f, 0f);

    [Header("Manivela do Carro (estilo volante)")]
    [Tooltip("Eixo LOCAL de rotação da manivela.")]
    public Vector3 eixoManivela = Vector3.up;

    [Tooltip("Amplitude total de rotação por passada (graus). 540 = 1,5 voltas.")]
    public float amplitudeManivela = 540f;

    [Tooltip("Inverter sentido de rotação da manivela.")]
    public bool inverterManivela = true;

    // ── Referências ─────────────────────────────────────────────────────────
    readonly string[] _nomesPlaca = { "EIXO ÁRVORE", "CASTANHA 1", "CASTANHA 2", "CASTANHA 3" };
    Transform[] _pecasPlaca;

    readonly string[] _nomesCastanhas = { "CASTANHA 1", "CASTANHA 2", "CASTANHA 3" };
    Transform[] _castanhas;
    Vector3[]   _castanhasOrigPos;   // posições locais originais de cada castanha

    Transform _carroLong;
    Transform _carroTransversal;
    Transform _cabecote;
    Transform _manivelaCabecote;
    Transform _fuso;
    Transform _manivela;
    Vector3   _manivelaCenter;
    Vector3   _manivelaCCenter;
    Transform _torre;

    Vector3 _carroOrigPos;
    Vector3 _transversalOrigPos;
    Vector3 _cabecoteOrigPos;

    bool _placaGirando;
    bool _fusoGirando;

    // ────────────────────────────────────────────────────────────────────────

    void Start()
    {
        var anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;

        // Peças da placa
        var pecasEncontradas = new List<Transform>();
        foreach (string nome in _nomesPlaca)
        {
            Transform t = FindDeep(nome);
            if (t != null) pecasEncontradas.Add(t);
            else Debug.LogWarning($"[TornoDemo] '{nome}' não encontrado.");
        }
        _pecasPlaca = pecasEncontradas.ToArray();

        // Castanhas (referência separada para animação radial)
        var castanhasEncontradas = new List<Transform>();
        foreach (string nome in _nomesCastanhas)
        {
            Transform t = FindDeep(nome);
            if (t != null) castanhasEncontradas.Add(t);
        }
        _castanhas = castanhasEncontradas.ToArray();
        _castanhasOrigPos = new Vector3[_castanhas.Length];
        for (int i = 0; i < _castanhas.Length; i++)
            _castanhasOrigPos[i] = _castanhas[i].localPosition;

        // Demais peças
        _carroLong        = FindDeep("CARRO LONGITUDINAL");
        _carroTransversal = FindDeep("CARRO TRANSVERSAL");
        _cabecote         = FindDeep("CABEÇOTE MÓVEL.002");
        _manivelaCabecote = FindDeep("MANIVELA CABEÇOTE");
        _fuso             = FindDeep("FUSO");
        _manivela         = FindDeep("MANIVELA CARRO");
        _torre            = FindDeep("TORRE DE FERRAMENTA");

        if (_carroLong        == null) Debug.LogWarning("[TornoDemo] 'CARRO LONGITUDINAL' não encontrado!");
        if (_carroTransversal == null) Debug.LogWarning("[TornoDemo] 'CARRO TRANSVERSAL' não encontrado — ok se não houver.");
        if (_cabecote         == null) Debug.LogWarning("[TornoDemo] 'CABEÇOTE MÓVEL.002' não encontrado — ok se não houver.");
        if (_manivelaCabecote == null) Debug.LogWarning("[TornoDemo] 'MANIVELA CABEÇOTE' não encontrada — ok se não houver.");
        if (_fuso             == null) Debug.LogWarning("[TornoDemo] 'FUSO' não encontrado — ok se não houver.");
        if (_manivela         == null) Debug.LogWarning("[TornoDemo] 'MANIVELA CARRO' não encontrada — ok se não houver.");
        if (_torre            == null) Debug.LogWarning("[TornoDemo] 'TORRE DE FERRAMENTA' não encontrada — ok se não houver.");

        // Centro visual da manivela do carro
        if (_manivela != null)
        {
            var rends = _manivela.GetComponentsInChildren<Renderer>();
            if (rends.Length > 0)
            {
                Bounds world = rends[0].bounds;
                for (int i = 1; i < rends.Length; i++) world.Encapsulate(rends[i].bounds);
                _manivelaCenter = _manivela.InverseTransformPoint(world.center);
            }
        }

        // Centro visual da manivela do cabeçote
        if (_manivelaCabecote != null)
        {
            var rends = _manivelaCabecote.GetComponentsInChildren<Renderer>();
            if (rends.Length > 0)
            {
                Bounds world = rends[0].bounds;
                for (int i = 1; i < rends.Length; i++) world.Encapsulate(rends[i].bounds);
                _manivelaCCenter = _manivelaCabecote.InverseTransformPoint(world.center);
            }
        }

        if (_carroLong        != null) _carroOrigPos       = _carroLong.localPosition;
        if (_carroTransversal != null) _transversalOrigPos = _carroTransversal.localPosition;
        if (_cabecote         != null) _cabecoteOrigPos    = _cabecote.localPosition;

        Debug.Log($"[TornoDemo] Castanhas: {_castanhas.Length} | Peças placa: {_pecasPlaca.Length}");
        StartCoroutine(RunDemo());
    }

    void Update()
    {
        float dt = Time.deltaTime * velocidade;

        if (_placaGirando && _pecasPlaca != null)
            foreach (var p in _pecasPlaca)
                p.Rotate(eixoPlaca, rpmMandril * dt, Space.Self);

        if (_fusoGirando && _fuso != null)
        {
            float sentidoFuso = inverterFuso ? -1f : 1f;
            _fuso.Rotate(eixoFuso, rpmFuso * sentidoFuso * dt, Space.Self);
        }
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
        yield return Espera(1f);

        // ── 1. Cabeçote avança para fixar a peça ────────────────────────────
        Debug.Log("[TornoDemo] Cabeçote avançando...");
        Coroutine manivelaCo1 = StartCoroutine(
            RotateManivelaVolante(_manivelaCabecote, _manivelaCCenter, eixoManivelaC,
                amplitudeManivelaCabecote, inverterManivelaCabecote, duracaoCabecote));
        yield return StartCoroutine(MoveSmooth(_cabecote, _cabecoteOrigPos,
            _cabecoteOrigPos + deslocamentoCabecote, duracaoCabecote));
        if (manivelaCo1 != null) yield return manivelaCo1;
        yield return Espera(0.5f);

        // ── 2. Castanhas fecham (prendem a peça) ────────────────────────────
        Debug.Log("[TornoDemo] Castanhas fechando...");
        yield return StartCoroutine(AnimarCastanhas(fechar: true));
        yield return Espera(0.4f);

        // ── 3. Mandril e fuso ligam ─────────────────────────────────────────
        Debug.Log("[TornoDemo] Ligando máquina...");
        _placaGirando = true;
        yield return Espera(1.5f);

        // ── 4. Carro longitudinal avança — 1ª passada ───────────────────────
        Debug.Log("[TornoDemo] 1ª passada (longitudinal)...");
        _fusoGirando = true;
        Coroutine manivCo1 = StartCoroutine(RotateManivelaVolante(amplitudeManivela, 4f));
        yield return StartCoroutine(
            MovePingPong(_carroLong, _carroOrigPos, deslocamentoCarro, 4f));
        if (manivCo1 != null) yield return manivCo1;
        _fusoGirando = false;
        yield return Espera(0.8f);

        // ── 5. Carro transversal avança — passada de faceamento ─────────────
        Debug.Log("[TornoDemo] Passada transversal...");
        yield return StartCoroutine(
            MovePingPong(_carroTransversal, _transversalOrigPos, deslocamentoTransversal, duracaoTransversal));
        yield return Espera(0.5f);

        // ── 6. Troca de ferramenta ───────────────────────────────────────────
        Debug.Log("[TornoDemo] Trocando ferramenta...");
        yield return StartCoroutine(RotateBy(_torre, rotacaoTorre, 1.2f));
        yield return Espera(0.5f);

        // ── 7. 2ª passada longitudinal ───────────────────────────────────────
        Debug.Log("[TornoDemo] 2ª passada (longitudinal)...");
        Vector3 pos2 = _carroLong != null ? _carroLong.localPosition : _carroOrigPos;
        _fusoGirando = true;
        Coroutine manivCo2 = StartCoroutine(RotateManivelaVolante(amplitudeManivela * 0.7f, 3f));
        yield return StartCoroutine(
            MovePingPong(_carroLong, pos2, deslocamentoCarro * 0.7f, 3f));
        if (manivCo2 != null) yield return manivCo2;
        _fusoGirando = false;
        yield return Espera(0.8f);

        // ── 8. Desliga ───────────────────────────────────────────────────────
        Debug.Log("[TornoDemo] Desligando máquina...");
        _placaGirando = false;
        yield return Espera(1.5f);

        // ── 9. Castanhas abrem (soltam a peça) ──────────────────────────────
        Debug.Log("[TornoDemo] Castanhas abrindo...");
        yield return StartCoroutine(AnimarCastanhas(fechar: false));
        yield return Espera(0.4f);

        // ── 10. Cabeçote recua ───────────────────────────────────────────────
        Debug.Log("[TornoDemo] Cabeçote recuando...");
        Coroutine manivelaCo2 = StartCoroutine(
            RotateManivelaVolante(_manivelaCabecote, _manivelaCCenter, eixoManivelaC,
                amplitudeManivelaCabecote, !inverterManivelaCabecote, duracaoCabecote));
        yield return StartCoroutine(MoveSmooth(_cabecote,
            _cabecoteOrigPos + deslocamentoCabecote, _cabecoteOrigPos, duracaoCabecote));
        if (manivelaCo2 != null) yield return manivelaCo2;
        yield return Espera(0.5f);
    }

    // ── Animação das castanhas ──────────────────────────────────────────────

    /// <summary>
    /// Anima as 3 castanhas radialmente a partir de sua posição local original.
    /// fechar=true → move em direção ao centro. fechar=false → abre.
    /// Use inverterCastanhas no Inspector se a direção estiver errada.
    /// </summary>
    IEnumerator AnimarCastanhas(bool fechar)
    {
        if (_castanhas == null || _castanhas.Length == 0)
        { yield return Espera(duracaoCastanha); yield break; }

        float durReal = duracaoCastanha / velocidade;
        float elapsed = 0f;

        // inverterCastanhas troca o sinal do deslocamento
        float sinal = inverterCastanhas ? 1f : -1f;

        while (elapsed < durReal)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / durReal));

            for (int i = 0; i < _castanhas.Length; i++)
            {
                if (_castanhas[i] == null) continue;
                Vector3 orig       = _castanhasOrigPos[i];
                Vector3 dir        = orig.normalized;
                Vector3 posAberta  = orig;
                Vector3 posFechada = orig + dir * (aberturaMaxCastanha * sinal);

                _castanhas[i].localPosition = fechar
                    ? Vector3.Lerp(posAberta,  posFechada, t)
                    : Vector3.Lerp(posFechada, posAberta,  t);
            }
            yield return null;
        }
    }

    // ── Helpers de animação ─────────────────────────────────────────────────

    IEnumerator MoveSmooth(Transform t, Vector3 origem, Vector3 destino, float duracao)
    {
        if (t == null) { yield return Espera(duracao); yield break; }
        float durReal = duracao / velocidade;
        float elapsed = 0f;
        while (elapsed < durReal)
        {
            elapsed += Time.deltaTime;
            t.localPosition = Vector3.Lerp(origem, destino,
                Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / durReal)));
            yield return null;
        }
        t.localPosition = destino;
    }

    IEnumerator MovePingPong(Transform t, Vector3 origem, Vector3 deslocamento, float duracao)
    {
        if (t == null) { yield return Espera(duracao); yield break; }
        float durReal = duracao / velocidade;
        float meio    = durReal * 0.5f;
        float elapsed = 0f;
        Vector3 alvo  = origem + deslocamento;

        while (elapsed < meio)
        {
            elapsed += Time.deltaTime;
            t.localPosition = Vector3.Lerp(origem, alvo, Mathf.Clamp01(elapsed / meio));
            yield return null;
        }
        t.localPosition = alvo;

        elapsed = 0f;
        while (elapsed < meio)
        {
            elapsed += Time.deltaTime;
            t.localPosition = Vector3.Lerp(alvo, origem, Mathf.Clamp01(elapsed / meio));
            yield return null;
        }
        t.localPosition = origem;
    }

    // Atalho para a manivela do carro (mantém assinatura original das chamadas)
    IEnumerator RotateManivelaVolante(float amplitudeGraus, float duracaoTotal)
        => RotateManivelaVolante(_manivela, _manivelaCenter, eixoManivela,
                                 amplitudeGraus, inverterManivela, duracaoTotal);

    // Versão genérica — usada pelo carro e pelo cabeçote
    IEnumerator RotateManivelaVolante(Transform manivela, Vector3 center, Vector3 eixo,
                                      float amplitudeGraus, bool inverter, float duracaoTotal)
    {
        if (manivela == null) { yield return Espera(duracaoTotal); yield break; }

        float sentido        = inverter ? -1f : 1f;
        float durReal        = duracaoTotal / velocidade;
        float meio           = durReal * 0.5f;
        float anguloAtual    = 0f;
        float anguloAnterior = 0f;

        float elapsed = 0f;
        while (elapsed < meio)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / meio);
            anguloAtual = Mathf.Lerp(0f, amplitudeGraus * sentido, t);
            float delta = anguloAtual - anguloAnterior;
            Vector3 worldCenter = manivela.TransformPoint(center);
            Vector3 worldAxis   = manivela.TransformDirection(eixo).normalized;
            manivela.RotateAround(worldCenter, worldAxis, delta);
            anguloAnterior = anguloAtual;
            yield return null;
        }

        elapsed = 0f;
        float anguloInicialVolta = anguloAtual;
        while (elapsed < meio)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / meio);
            anguloAtual = Mathf.Lerp(anguloInicialVolta, 0f, t);
            float delta = anguloAtual - anguloAnterior;
            Vector3 worldCenter = manivela.TransformPoint(center);
            Vector3 worldAxis   = manivela.TransformDirection(eixo).normalized;
            manivela.RotateAround(worldCenter, worldAxis, delta);
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
            t.localRotation = Quaternion.Lerp(from, to,
                Mathf.Clamp01(elapsed / durReal));
            yield return null;
        }
        t.localRotation = to;
    }

    Transform FindDeep(string nomePeca)
    {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>(true))
            if (t.name == nomePeca)
                return t;
        return null;
    }

    WaitForSeconds Espera(float segundos) => new WaitForSeconds(segundos / velocidade);
}
