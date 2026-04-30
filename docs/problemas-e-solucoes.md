# Problemas e Soluções

## O Unity abriu com muitos erros

### Causa provável

O projeto foi aberto como se fosse novo e o `Packages/manifest.json` foi resetado, removendo pacotes usados pelos assets XR.

### Solução aplicada

Foram recolocados pacotes como:

- Input System;
- XR Interaction Toolkit;
- OpenXR;
- XR Management;
- URP;
- Visual Scripting;
- Mathematics.

Depois disso, o projeto compilou em modo batch sem erros C#.

## Aviso sobre Input System

### Sintoma

O Unity mostra aviso dizendo que o novo Input System está instalado, mas não está ativado.

### Solução

Clique em `Yes`. O editor reinicia e habilita o backend necessário.

## Cena vazia ao abrir

### Sintoma

A Hierarchy mostra apenas `Main Camera` e `Directional Light`.

### Causa

O Unity abriu uma cena vazia chamada `Untitled`.

### Solução

Abrir a cena:

```text
Assets/Scenes/TornoMecanico.unity
```

Ou usar:

```text
Tools > Criar Cena do Torno
```

## Game View mostrando o torno de costas

### Causa

A câmera estava posicionada atrás do modelo.

### Solução

Use:

```text
Tools > Ajustar Camera Frente do Torno
```

## Game View pixelada

### Causa

O controle `Scale` da aba Game estava ampliando a imagem renderizada.

### Solução

Reduzir o `Scale` para `1x` ou selecionar uma resolução maior.

## Modelo sem cores

### Causa

O FBX não trouxe materiais/texturas completas.

### Soluções possíveis

- aplicar materiais manualmente no Unity;
- corrigir materiais no Blender e exportar novamente;
- criar uma paleta simples de materiais para o torno no Unity.
