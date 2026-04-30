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

## Game View pixelada

Se a aba Game parecer pixelada, confira o valor `Scale`. Se estiver em `3x`, `4x` ou parecido, reduza para `1x` ou selecione uma resolução maior como `Full HD (1920x1080)`.
