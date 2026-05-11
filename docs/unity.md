# Projeto Unity

O projeto Unity fica em:

```text
Projetinho/TornoREV2
```

## Versão usada

O projeto foi aberto e validado com:

```text
Unity 6.3 LTS - 6000.3.14f1
```

## Pacotes importantes

O projeto usa ou precisou recuperar os seguintes pacotes:

- Input System
- XR Interaction Toolkit
- OpenXR
- XR Management
- XR Core Utilities
- Universal Render Pipeline
- Visual Scripting
- Mathematics

## Cena do torno

A cena principal de visualização fica em:

```text
Assets/Scenes/TornoMecanico.unity
```

Ela foi criada para permitir que o modelo do torno fosse visto no Unity. Ao clicar em Play, o esperado atualmente é apenas visualizar o modelo pela câmera. Ainda não existe lógica de simulação.

## Menu auxiliar criado

Foi adicionado um script editor em:

```text
Assets/Editor/CreateTornoScene.cs
```

Ele adiciona os menus:

```text
Tools > Criar Cena do Torno
Tools > Ajustar Camera Frente do Torno
```

Use esses comandos se a cena precisar ser recriada ou se a câmera ficar apontando para o lado errado.

## Aviso do Input System

Se aparecer um aviso dizendo que o projeto usa o novo Input System, clique em `Yes`. O Unity vai reiniciar e ativar o backend correto.

Também é possível conferir manualmente:

```text
Edit > Project Settings > Player > Other Settings > Active Input Handling
```

Valores aceitáveis:

- `Input System Package (New)`
- `Both`

## Animações

As animações do torno ficam em `Assets/Animations/` e foram geradas pelo script editor `CreateTornoAnimations.cs`.

### Clips disponíveis

| Clip | Peça animada | Tipo | Duração |
|---|---|---|---|
| `Placa_Girando` | EIXO ÁRVORE | Rotação contínua (loop) | 1,5 s/volta |
| `Carro_Avancando` | CARRO LONGITUDINAL | Translação ida/volta (loop) | 4 s |
| `Manivela_Girando` | MANIVELA | Rotação contínua (loop) | 4 s/volta |
| `Torre_Rotacionando` | TORRE | Rotação 90° (one-shot) | 1 s |
| `Carro_Transversal` | CARRO TRANSVERSAL | Translação ida/volta (loop) | 3 s |

### Animator Controller

O `TornoController.controller` gerencia as transições entre estados. Parâmetros disponíveis:

| Parâmetro | Tipo | Função |
|---|---|---|
| `PlacaGirando` | Bool | Liga/desliga a animação da placa |
| `CarroAvancando` | Bool | Liga/desliga o avanço do carro |
| `TrocaFerramenta` | Trigger | Dispara a rotação da torre |

O componente `Animator` já está configurado no objeto `torno` na cena `TornoMecanico`.

### Como visualizar as animações

1. Selecione o objeto `torno` na Hierarchy.
2. Abra `Window > Animation > Animation`.
3. No dropdown de clips, escolha um dos 5 clips.
4. Clique em Play (▶) no painel de animação para ver a prévia.

### Como recriar as animações

Se precisar recriar ou atualizar os clips:

```text
Tools > Criar Animações do Torno
```

O script deleta e recria todos os clips em `Assets/Animations/`.

## Scripts do Editor

| Script | Menu | Função |
|---|---|---|
| `CreateTornoScene.cs` | `Tools > Criar Cena do Torno` | Recria a cena com o modelo posicionado |
| `CreateTornoAnimations.cs` | `Tools > Criar Animações do Torno` | Gera os 5 clips e o AnimatorController |
| `FixTornoMaterials.cs` | `Tools > Corrigir Materiais do Torno` | Reaplica cores URP em todos os materiais |

## Game View pixelada

Se a aba Game parecer pixelada, confira o valor `Scale`. Se estiver em `3x`, `4x` ou parecido, reduza para `1x` ou selecione uma resolução maior como `Full HD (1920x1080)`.
