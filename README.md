# Realidade Virtual — Máquinas Industriais

Projeto educacional de **Realidade Virtual (RV)** para o aprendizado de máquinas industriais em ambiente seguro e imersivo. O foco inicial é o **torno mecânico**, com planos de expansão para uma fábrica virtual completa.

---

## Objetivo

Permitir que estudantes e operadores explorem, entendam e pratiquem o uso de máquinas industriais em um ambiente virtual — sem riscos físicos, sem necessidade de equipamentos reais e com possibilidade de repetição ilimitada.

---

## Estado atual

| Componente | Status |
|---|---|
| Modelo 3D do torno (`.fbx`) | ✅ Importado e configurado no Unity |
| Materiais URP (13 no total) | ✅ Convertidos e com cores corretas |
| Cena de visualização Unity | ✅ Funcional |
| Pipeline de renderização (URP) | ✅ Configurado |
| Interações VR | 🔄 Em desenvolvimento |
| Animações das peças móveis | ⏳ Aguardando separação no Blender |
| Documentação técnica (MkDocs) | ✅ Publicada |

---

## Estrutura do repositório

```
RealidadeVirtual/
├── docs/                          # Documentação em Markdown (MkDocs)
├── Projetinho/
│   └── TornoREV2/                 # Projeto Unity 6.3 LTS
│       ├── Assets/
│       │   ├── TornoMecanico/     # Modelo FBX + 13 materiais URP
│       │   ├── Scenes/            # Cena TornoMecanico.unity
│       │   └── Editor/            # Scripts auxiliares do Editor
│       ├── Packages/              # Dependências Unity (manifest.json)
│       └── ProjectSettings/       # Configurações do projeto (URP, XR, etc.)
├── mkdocs.yml                     # Configuração do site de documentação
├── requirements.txt               # Dependências Python (MkDocs)
└── codex.md                       # Memória técnica do projeto
```

> **Não estão no Git:** `Library/`, `Temp/`, pastas de assets de terceiros (`Free/`, `ADG_Textures/`), arquivos grandes (`.ova`, `.zip`), e o ambiente Python (`.venv/`).

---

## Projeto Unity

### Versão
```
Unity 6.3 LTS — 6000.3.14f1
```

### Pacotes principais

| Pacote | Versão | Finalidade |
|---|---|---|
| Universal Render Pipeline | 14.x | Renderização moderna (necessário para VR) |
| XR Interaction Toolkit | 2.5.2 | Interações com controladores VR |
| OpenXR | 1.8.2 | Suporte a headsets (Meta Quest, etc.) |
| XR Management | 4.4.0 | Gerenciamento do sistema XR |
| Input System | 1.7.0 | Entrada de dados dos controladores |

### Cena principal

```
Assets/Scenes/TornoMecanico.unity
```

Contém o modelo do torno posicionado com câmera e iluminação direcional. É uma cena de visualização — as interações VR estão em desenvolvimento.

### Scripts do Editor

| Script | Menu Unity | Função |
|---|---|---|
| `CreateTornoScene.cs` | `Tools > Criar Cena do Torno` | Recria a cena do torno do zero |
| `FixTornoMaterials.cs` | `Tools > Corrigir Materiais do Torno` | Reaplica cores URP em todos os materiais |

### Materiais

O modelo possui 13 materiais com shader `Universal Render Pipeline/Lit`:

| Material | Cor | Uso |
|---|---|---|
| `amarelo` | Amarelo vivo | Botões, alavancas |
| `amarelo_bege` | Bege claro | Acabamentos |
| `branco` | Branco quase puro | Superfícies claras |
| `cinza` | Cinza médio | Estrutura geral |
| `metal` | Cinza metálico | Peças metálicas |
| `preto` | Preto | Componentes escuros |
| `seta` | Amarelo | Indicadores de direção |
| `verde freio` | Verde escuro | Freios industriais |
| `verde luz` | Verde vivo | LEDs indicadores |
| `vermelho` | Vermelho | Alertas, paradas de emergência |
| `vidro_` | Azul transparente | Proteções de vidro |
| `Material.001` | Cinza padrão | Geometria genérica |
| `Material.002` | Cinza escuro | Geometria secundária |

---

## Modelo 3D

- **Arquivo:** `torno.fbx` (1.24 MB, formato binário)
- **Localização:** `Assets/TornoMecanico/Torno Mecanico/torno.fbx`
- **Origem:** Exportado do Blender

> **Limitação atual:** o modelo foi exportado como uma malha única (single mesh). Para animar peças individuais (placa, carro, volantes), é necessário separar os objetos no Blender e re-exportar.

---

## Documentação

Gerada com [MkDocs](https://www.mkdocs.org/) usando o tema [Material](https://squidfunk.github.io/mkdocs-material/).

### Configurar ambiente

```powershell
python -m venv .venv
.\.venv\Scripts\pip install -r requirements.txt
```

### Comandos

```powershell
# Servidor local em http://127.0.0.1:8000
.\.venv\Scripts\mkdocs.exe serve

# Build e validação (usado no CI)
.\.venv\Scripts\mkdocs.exe build --strict
```

### Páginas disponíveis

- Início
- Inventário de arquivos
- Projeto Unity
- Modelo 3D
- Máquina Virtual
- Stack tecnológica
- Problemas e Soluções
- Roadmap

---

## Roadmap

### Torno mecânico (em andamento)

- [x] Organizar e recuperar arquivos do projeto
- [x] Importar modelo FBX no Unity
- [x] Extrair e configurar 13 materiais URP
- [x] Configurar pipeline de renderização (URP)
- [x] Versionar projeto no Git
- [ ] Separar peças móveis no Blender (placa, carro, volantes)
- [ ] Adicionar colisores para interação VR
- [ ] Criar animações das peças móveis
- [ ] Implementar hotspots educacionais (placas informativas)
- [ ] Configurar experiência VR completa com OpenXR
- [ ] Gerar build executável

### Fábrica virtual (próxima etapa)

Após o torno, a proposta é criar um cenário de fábrica onde o usuário pode explorar diferentes máquinas industriais:

- Fresadora
- Furadeira de bancada
- Serra industrial
- Prensa
- Esteira transportadora
- Robô industrial
- Painel elétrico / bancada de comandos

---

## Stack tecnológica

**Motor e renderização**
- Unity 6.3 LTS
- Universal Render Pipeline (URP)
- OpenXR / XR Interaction Toolkit

**Modelagem 3D**
- Blender
- FreeCAD / Autodesk Fusion 360

**Linguagens**
- C# (scripts Unity)
- Python (documentação MkDocs)

**Documentação e versionamento**
- MkDocs + Material Theme
- Git + GitHub
- GitHub Pages (deploy automático)
- Git LFS (para modelos grandes futuros)

---

## Deploy da documentação

O GitHub Actions publica automaticamente a documentação no GitHub Pages a cada push na branch `main`.

Configuração em: `.github/workflows/documentation.yml`

---
