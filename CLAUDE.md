# CLAUDE.md

Instruções e memória técnica completa do projeto para uso com Claude Code.

---

## Regras de desenvolvimento

- **Commits**: mensagem direta e objetiva. **Sem `Co-Authored-By`** em nenhum commit.
- Explicações devem ser diretas e práticas — o usuário é iniciante em Unity.
- O projeto é usado exclusivamente com Claude Code.

---

## O que é este repositório

Projeto educacional de **Realidade Virtual** para aprendizado de máquinas industriais. O foco atual é o **torno mecânico**. O objetivo de longo prazo é uma fábrica virtual com múltiplas máquinas.

O usuário é iniciante em Unity — explicações devem ser diretas e práticas. O projeto é usado exclusivamente com Claude Code.

---

## Estado atual (atualizado 2026-06-01)

### O que está pronto

- Modelo 3D (`torno.fbx`, 1,24 MB) importado com **hierarquia completa** — ~40 objetos separados, todos animáveis.
- 13 materiais URP com cores corretas (amarelo, verde, cinza, metal, vermelho, etc.).
- Cena de visualização `TornoMecanico.unity` funcional com câmera e iluminação.
- **5 clips de animação** em `Assets/Animations/`:
  - `Placa_Girando.anim` — EIXO ÁRVORE girando (loop, 1,5 s/volta)
  - `Carro_Avancando.anim` — CARRO LONGITUDINAL translação (loop, 4 s)
  - `Manivela_Girando.anim` — MANIVELA girando (loop, 4 s/volta)
  - `Torre_Rotacionando.anim` — TORRE 90° one-shot (1 s)
  - `Carro_Transversal.anim` — CARRO TRANSVERSAL translação (loop, 3 s)
- `TornoController.controller` com parâmetros `PlacaGirando` (bool), `CarroAvancando` (bool), `TrocaFerramenta` (trigger).
- **`TornoDemo.cs`** — script de demonstração automática com transforms diretos (sem Animator):
  - Mandril gira (EIXO ÁRVORE + CASTANHA 1/2/3 juntos)
  - CASTANHA 1/2/3 fecham/abrem radialmente (prender/soltar peça)
  - CARRO LONGITUDINAL avança/recua (ping-pong) + FUSO sincronizado
  - MANIVELA CARRO gira tipo volante (540° ida+volta)
  - CARRO TRANSVERSAL avança/recua (passada de faceamento)
  - CABEÇOTE MÓVEL.002 avança/recua (contraponto) + MANIVELA CABEÇOTE sincronizada
  - TORRE DE FERRAMENTA rotaciona 90° na troca de ferramenta
  - Sequência realista: cabeçote → castanhas fecham → máquina liga → usinagem → desliga → castanhas abrem → cabeçote recua
- Projeto Unity versionado no Git. Documentação no GitHub Pages (MkDocs).

---

## Roadmap de implementação

### FASE 1 — Animações (atual)

| # | Peça | O que fazer | Status |
|---|---|---|---|
| 1.1 | CASTANHA 1 / 2 / 3 | Garras do mandril abrindo e fechando radialmente | ✅ implementado |
| 1.2 | CABEÇOTE MÓVEL.002 | Translação no barramento (avanço do contraponto) | ✅ implementado |
| 1.3 | FUSO | Rotação contínua sincronizada com avanço do carro | ✅ implementado |
| 1.4 | CARRO TRANSVERSAL | Integrar no TornoDemo | ✅ implementado |
| 1.5 | MANIVELA CABEÇOTE | Girar sincronizada com o avanço do cabeçote | ✅ implementado |
| 1.6 | Eixos em Play Mode | Confirmar/corrigir eixos de todas as peças no Unity | ⚠️ pendente verificação visual |

**Campos ajustáveis no Inspector (sem precisar editar código):**
- `Inverter Manivela` — inverte sentido da manivela do carro
- `Inverter Fuso` — inverte sentido de rotação do fuso
- `Inverter Castanhas` — inverte direção radial das garras
- `Inverter Manivela Cabeçote` — inverte sentido da manivela do contraponto
- `Abertura Max Castanha` — distância de abertura/fechamento das garras
- `Deslocamento Cabeçote` — direção e distância que o cabeçote percorre

### FASE 2 — Interação VR

| # | O que fazer | Status |
|---|---|---|
| 2.1 | Colisores (MeshCollider/BoxCollider) nas peças interativas | ❌ pendente |
| 2.2 | XR Origin com câmera e controladores configurados | ❌ pendente |
| 2.3 | XR Grab Interactable nas manivelas | ❌ pendente |
| 2.4 | QuickOutline nos objetos interativos (hover highlight) | ❌ pendente |

### FASE 3 — Conteúdo didático

| # | O que fazer | Status |
|---|---|---|
| 3.1 | Hotspots informativos (UI panels nas peças ao olhar/clicar) | ❌ pendente |
| 3.2 | Narração/legenda explicando cada peça durante a demo | ❌ pendente |
| 3.3 | Build executável (PC + Quest standalone) | ❌ pendente |

---

## Hierarquia do FBX — peças-chave

| Peça Unity | Função no torno | Animação atual |
|---|---|---|
| `EIXO ÁRVORE` | Eixo principal / mandril | `Placa_Girando` (rot. X) |
| `CARRO LONGITUDINAL` | Carro que avança pelo barramento | `Carro_Avancando` (transl. X) |
| `CARRO TRANSVERSAL` | Avanço transversal | `Carro_Transversal` (transl. Z) |
| `CARRO PRINCIPAL` | Estrutura do carro | — |
| `MANIVELA` / `.001`–`.003` | Volantes manuais (×4) | `Manivela_Girando` (rot. Z) |
| `TORRE` | Porta-ferramentas rotativo | `Torre_Rotacionando` (rot. Y) |
| `CASTANHA 1/2/3` | Garras do mandril | pendente |
| `CABEÇOTE MÓVEL` / `.002` | Contraponto | pendente |
| `FUSO` | Fuso de avanço automático | pendente |
| `PROTEÇÃO` (×6) | Proteções de segurança | — |

---

## Projeto Unity

**Localização:** `Projetinho/TornoREV2`  
**Versão:** Unity 6.3 LTS (6000.3.14f1)

### Pacotes obrigatórios

| Pacote | Versão |
|---|---|
| XR Interaction Toolkit | 2.5.2 |
| OpenXR | 1.8.2 |
| XR Management | 4.4.0 |
| Input System | 1.7.0 |
| Universal Render Pipeline | 14.0.9 |

### Caminhos importantes

```
Assets/Scenes/TornoMecanico.unity          ← cena principal
Assets/TornoMecanico/Torno Mecanico/torno.fbx  ← modelo FBX
Assets/TornoMecanico/Materials/            ← 13 materiais URP
Assets/Animations/                         ← 5 clips + TornoController
Assets/Editor/CreateTornoScene.cs          ← recria a cena
Assets/Editor/CreateTornoAnimations.cs     ← gera os clips de animação
Assets/Editor/FixTornoMaterials.cs         ← corrige materiais
Assets/QuickOutline/                       ← plugin de highlight (para VR)
```

### Menus Tools disponíveis no Unity

| Menu | Função |
|---|---|
| `Tools > Criar Cena do Torno` | Recria a cena com o modelo posicionado |
| `Tools > Criar Animações do Torno` | Gera os 5 clips e o AnimatorController |
| `Tools > Corrigir Materiais do Torno` | Reaplica cores URP nos 13 materiais |
| `Tools > Ajustar Camera Frente do Torno` | Reposiciona câmera |

---

## Decisões técnicas

- **URP obrigatório**: necessário para XR/VR. Todos os materiais usam `Universal Render Pipeline/Lit`.
- **FBX como binário no Git**: `.gitattributes` define `*.fbx binary`.
- **Sem Git LFS por enquanto**: FBX é pequeno (1,24 MB). Ativar se modelos maiores chegarem.
- **Sem texturas PNG**: o FBX referencia `bege.png`, `cinza.png`, `logo.png`, `metal.png`, `preto.png`, `seta.png`, `textura_painel2.png` — arquivos não estão no repo. Modelo usa cores sólidas por enquanto. Solicitar ao modelista.
- **QuickOutline instalado**: em `Assets/QuickOutline/`, pronto para highlights VR.

---

## Documentação (MkDocs)

```powershell
# Criar ambiente
python -m venv .venv
.\.venv\Scripts\pip install -r requirements.txt

# Servidor local em http://127.0.0.1:8000
.\.venv\Scripts\mkdocs.exe serve

# Validação estrita (usado no CI)
.\.venv\Scripts\mkdocs.exe build --strict
```

O `site/` gerado é ignorado no Git. O GitHub Actions (`.github/workflows/documentation.yml`) faz deploy automático no GitHub Pages a cada push na `main`.

Páginas em `docs/`: `index.md`, `inventario.md`, `unity.md`, `modelo-3d.md`, `maquina-virtual.md`, `stack.md`, `problemas-e-solucoes.md`, `roadmap.md`.

Ao adicionar páginas novas, registrar no `nav:` do `mkdocs.yml`.

---

## O que NÃO está no Git (e por quê)

| Pasta/Arquivo | Motivo |
|---|---|
| `Projetinho/TornoREV2/Library/` | Cache pesado gerado pelo Unity |
| `Projetinho/TornoREV2/Temp/` | Cache temporário |
| `Projetinho/TornoREV2/Assets/Free/` | Assets de terceiros (Asset Store) |
| `Projetinho/TornoREV2/Assets/ADG_Textures/` | Texturas de terceiros |
| `Projetinho/TornoREV2/Assets/_Recovery/` | Cenas de recuperação automática |
| `Torno/` | Arquivo compactado do modelo original |
| `Modelos/` | Modelos auxiliares |
| `DPADI0191-002.ova` | VM VirtualBox (~40 GB, Ubuntu 64-bit) |
| `*.zip`, `*.rar`, `*.7z` | Backups originais |
| `.venv/`, `site/` | Gerados localmente |
| `Backups/` | Backups locais do projeto |

---

## Máquina virtual (encerrado)

O arquivo `DPADI0191-002.ova` foi investigado (2026-05-29) — era uma VM de laboratório de redes de uma disciplina acadêmica, sem conteúdo relacionado ao projeto. Arquivo pode ser excluído.

---

## Fábrica virtual (futuro)

Cenário com múltiplas máquinas: fresadora, furadeira, serra, prensa, esteira, robô industrial, painel elétrico.
