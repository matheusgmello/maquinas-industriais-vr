using System.Collections;
using UnityEngine;

/// <summary>
/// Demonstração automática do torno mecânico.
/// Anexe este script ao objeto 'torno' na cena e pressione Play.
/// A sequência simula uma operação real: liga → usina → troca ferramenta → desliga.
/// </summary>
public class TornoDemo : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Quantas vezes repetir o ciclo completo. 0 = infinito.")]
    public int ciclos = 1;

    [Tooltip("Velocidade geral da demonstração. 1 = normal, 0.5 = metade da velocidade.")]
    [Range(0.25f, 2f)]
    public float velocidade = 1f;

    Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("[TornoDemo] Animator não encontrado no objeto. Adicione o Animator ao 'torno'.");
            return;
        }
        StartCoroutine(RunDemo());
    }

    IEnumerator RunDemo()
    {
        int ciclosFeitos = 0;

        while (ciclos == 0 || ciclosFeitos < ciclos)
        {
            yield return StartCoroutine(SequenciaDemonstracao());
            ciclosFeitos++;

            if (ciclos == 0 || ciclosFeitos < ciclos)
                yield return Espera(2f); // pausa entre ciclos
        }

        Debug.Log("[TornoDemo] Demonstração concluída.");
    }

    IEnumerator SequenciaDemonstracao()
    {
        // ── 1. Pausa inicial (máquina parada) ─────────────────────────────
        yield return Espera(1f);

        // ── 2. Ligar: placa começa a girar ────────────────────────────────
        Debug.Log("[TornoDemo] Ligando máquina...");
        _animator.SetBool("PlacaGirando", true);
        yield return Espera(1.5f);

        // ── 3. Carro avança (usinagem) ────────────────────────────────────
        Debug.Log("[TornoDemo] Carro avançando...");
        _animator.SetBool("CarroAvancando", true);
        yield return Espera(4f);

        // ── 4. Carro para ─────────────────────────────────────────────────
        _animator.SetBool("CarroAvancando", false);
        yield return Espera(1f);

        // ── 5. Troca de ferramenta (torre rotaciona) ──────────────────────
        Debug.Log("[TornoDemo] Trocando ferramenta...");
        _animator.SetTrigger("TrocaFerramenta");
        yield return Espera(1.5f);

        // ── 6. Segunda passada de usinagem ────────────────────────────────
        Debug.Log("[TornoDemo] Segunda passada...");
        _animator.SetBool("CarroAvancando", true);
        yield return Espera(3f);

        // ── 7. Carro recua ────────────────────────────────────────────────
        _animator.SetBool("CarroAvancando", false);
        yield return Espera(1f);

        // ── 8. Desligar: placa para ───────────────────────────────────────
        Debug.Log("[TornoDemo] Desligando máquina...");
        _animator.SetBool("PlacaGirando", false);
        yield return Espera(1f);
    }

    // Aplica a velocidade configurada pelo usuário
    WaitForSeconds Espera(float segundos) => new WaitForSeconds(segundos / velocidade);
}
