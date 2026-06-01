using System.Collections;
using UnityEngine;

/// <summary>
/// Testa cada peça do torno individualmente.
/// Adicione este componente no objeto "torno" e use os botões no Inspector durante o Play.
/// </summary>
public class TornoTeste : MonoBehaviour
{
    [Header("Referências (preenchidas automaticamente)")]
    public TornoDemo demo;

    [Header("Duração de cada teste (segundos)")]
    public float duracaoTeste = 3f;

    Coroutine _testAtivo;

    void Start()
    {
        if (demo == null)
            demo = GetComponent<TornoDemo>();
    }

    // ── Chamadas pelos botões do Editor ─────────────────────────────────────

    public void TestarMandril()      => Rodar(TesteMandril());
    public void TestarCastanhas()    => Rodar(TesteCastanhas());
    public void TestarCarro()        => Rodar(TesteCarro());
    public void TestarTransversal()  => Rodar(TesteTransversal());
    public void TestarCabecote()     => Rodar(TesteCabecote());
    public void TestarFuso()         => Rodar(TesteFuso());
    public void TestarManivela()     => Rodar(TesteManivela());
    public void TestarTorre()        => Rodar(TesteTorre());

    void Rodar(IEnumerator co)
    {
        if (_testAtivo != null) StopCoroutine(_testAtivo);
        _testAtivo = StartCoroutine(co);
    }

    // ── Testes individuais ───────────────────────────────────────────────────

    IEnumerator TesteMandril()
    {
        var t = FindDeep("EIXO ÁRVORE");
        if (t == null) { Aviso("EIXO ÁRVORE não encontrado"); yield break; }
        Debug.Log("[Teste] Mandril girando por " + duracaoTeste + "s...");
        float elapsed = 0f;
        while (elapsed < duracaoTeste)
        {
            t.Rotate(demo.eixoPlaca, demo.rpmMandril * Time.deltaTime, Space.Self);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Debug.Log("[Teste] Mandril: OK");
    }

    IEnumerator TesteCastanhas()
    {
        var nomes = new[] { "CASTANHA 1", "CASTANHA 2", "CASTANHA 3" };
        var castanhas = new Transform[3];
        var origPos   = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            castanhas[i] = FindDeep(nomes[i]);
            if (castanhas[i] == null) { Aviso(nomes[i] + " não encontrado"); yield break; }
            origPos[i] = castanhas[i].localPosition;
        }
        Debug.Log("[Teste] Castanhas: fechando...");
        yield return AnimarCastanhas(castanhas, origPos, fechar: true);
        yield return new WaitForSeconds(1f);
        Debug.Log("[Teste] Castanhas: abrindo...");
        yield return AnimarCastanhas(castanhas, origPos, fechar: false);
        Debug.Log("[Teste] Castanhas: OK — verifique se fecharam para DENTRO do mandril");
    }

    IEnumerator TesteCarro()
    {
        var t = FindDeep("CARRO LONGITUDINAL");
        if (t == null) { Aviso("CARRO LONGITUDINAL não encontrado"); yield break; }
        Vector3 orig = t.localPosition;
        Debug.Log("[Teste] Carro longitudinal avançando...");
        yield return Mover(t, orig, orig + demo.deslocamentoCarro, duracaoTeste / 2f);
        yield return Mover(t, orig + demo.deslocamentoCarro, orig, duracaoTeste / 2f);
        Debug.Log("[Teste] Carro longitudinal: OK");
    }

    IEnumerator TesteTransversal()
    {
        var t = FindDeep("CARRO TRANSVERSAL");
        if (t == null) { Aviso("CARRO TRANSVERSAL não encontrado"); yield break; }
        Vector3 orig = t.localPosition;
        Debug.Log("[Teste] Carro transversal avançando...");
        yield return Mover(t, orig, orig + demo.deslocamentoTransversal, duracaoTeste / 2f);
        yield return Mover(t, orig + demo.deslocamentoTransversal, orig, duracaoTeste / 2f);
        Debug.Log("[Teste] Carro transversal: OK");
    }

    IEnumerator TesteCabecote()
    {
        var t = FindDeep("CABEÇOTE MÓVEL.002");
        if (t == null) { Aviso("CABEÇOTE MÓVEL.002 não encontrado"); yield break; }
        Vector3 orig = t.localPosition;
        Debug.Log("[Teste] Cabeçote avançando...");
        yield return Mover(t, orig, orig + demo.deslocamentoCabecote, duracaoTeste / 2f);
        yield return new WaitForSeconds(0.5f);
        yield return Mover(t, orig + demo.deslocamentoCabecote, orig, duracaoTeste / 2f);
        Debug.Log("[Teste] Cabeçote: OK — verifique se avançou em direção ao mandril");
    }

    IEnumerator TesteFuso()
    {
        var t = FindDeep("FUSO");
        if (t == null) { Aviso("FUSO não encontrado"); yield break; }
        Debug.Log("[Teste] Fuso girando por " + duracaoTeste + "s...");
        float elapsed = 0f;
        float sentido = demo.inverterFuso ? -1f : 1f;
        while (elapsed < duracaoTeste)
        {
            t.Rotate(demo.eixoFuso, demo.rpmFuso * sentido * Time.deltaTime, Space.Self);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Debug.Log("[Teste] Fuso: OK");
    }

    IEnumerator TesteManivela()
    {
        var t = FindDeep("MANIVELA CARRO");
        if (t == null) { Aviso("MANIVELA CARRO não encontrado"); yield break; }
        var rends = t.GetComponentsInChildren<Renderer>();
        Vector3 center = Vector3.zero;
        if (rends.Length > 0)
        {
            Bounds b = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
            center = t.InverseTransformPoint(b.center);
        }
        Debug.Log("[Teste] Manivela girando (ida e volta)...");
        float sentido = demo.inverterManivela ? -1f : 1f;
        float half = duracaoTeste / 2f;
        yield return GirarVolante(t, center, demo.eixoManivela, demo.amplitudeManivela * sentido, half);
        yield return GirarVolante(t, center, demo.eixoManivela, 0f, half, demo.amplitudeManivela * sentido);
        Debug.Log("[Teste] Manivela: OK");
    }

    IEnumerator TesteTorre()
    {
        var t = FindDeep("TORRE DE FERRAMENTA");
        if (t == null) { Aviso("TORRE DE FERRAMENTA não encontrado"); yield break; }
        Quaternion orig = t.localRotation;
        Quaternion dest = orig * Quaternion.Euler(demo.rotacaoTorre);
        Debug.Log("[Teste] Torre rotacionando 90°...");
        float elapsed = 0f;
        while (elapsed < 1.2f)
        {
            elapsed += Time.deltaTime;
            t.localRotation = Quaternion.Lerp(orig, dest, Mathf.Clamp01(elapsed / 1.2f));
            yield return null;
        }
        t.localRotation = dest;
        yield return new WaitForSeconds(0.5f);
        elapsed = 0f;
        while (elapsed < 1.2f)
        {
            elapsed += Time.deltaTime;
            t.localRotation = Quaternion.Lerp(dest, orig, Mathf.Clamp01(elapsed / 1.2f));
            yield return null;
        }
        t.localRotation = orig;
        Debug.Log("[Teste] Torre: OK");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    IEnumerator AnimarCastanhas(Transform[] cs, Vector3[] origs, bool fechar)
    {
        float dur = demo.duracaoCastanha;
        float elapsed = 0f;
        float sinal = demo.inverterCastanhas ? 1f : -1f;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / dur));
            for (int i = 0; i < cs.Length; i++)
            {
                if (cs[i] == null) continue;
                Vector3 dir       = origs[i].normalized;
                Vector3 posAberta = origs[i];
                Vector3 posFech   = origs[i] + dir * (demo.aberturaMaxCastanha * sinal);
                cs[i].localPosition = fechar
                    ? Vector3.Lerp(posAberta, posFech, t)
                    : Vector3.Lerp(posFech, posAberta, t);
            }
            yield return null;
        }
    }

    IEnumerator Mover(Transform t, Vector3 de, Vector3 para, float dur)
    {
        float elapsed = 0f;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            t.localPosition = Vector3.Lerp(de, para, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / dur)));
            yield return null;
        }
        t.localPosition = para;
    }

    IEnumerator GirarVolante(Transform t, Vector3 center, Vector3 eixo, float angDest, float dur, float angOrig = 0f)
    {
        float elapsed = 0f;
        float prev = angOrig;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float ang = Mathf.Lerp(angOrig, angDest, Mathf.Clamp01(elapsed / dur));
            float delta = ang - prev;
            Vector3 wc = t.TransformPoint(center);
            Vector3 wa = t.TransformDirection(eixo).normalized;
            t.RotateAround(wc, wa, delta);
            prev = ang;
            yield return null;
        }
    }

    Transform FindDeep(string nome)
    {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>(true))
            if (t.name == nome) return t;
        return null;
    }

    void Aviso(string msg) => Debug.LogWarning("[Teste] " + msg);
}
