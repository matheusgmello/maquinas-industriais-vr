# Inventário dos Arquivos

Esta página descreve os principais arquivos e pastas encontrados.

## Raiz do material

| Item | Tipo | Descrição |
| --- | --- | --- |
| `DPADI0191-002.ova` | Máquina virtual | Appliance exportado do VirtualBox com Ubuntu 64-bit. |
| `Projetinho/` | Projeto Unity | Pasta com o projeto `TornoREV2`, assets Unity e cena de visualização criada. |
| `Torno/` | Arquivo compactado | Contém o modelo `torno.fbx`. |
| `Modelos/` | Arquivos compactados | Modelos auxiliares recebidos separadamente. |
| `Projetinho-20260426T135109Z-3-003.zip` | Backup/compactado | Arquivo compactado original do projeto Unity. |

## Modelo do torno

O arquivo principal do torno estava dentro de:

```text
Torno/Torno Mecanico-20260427T192644Z-3-001.zip
```

Ele foi extraído para o projeto Unity em:

```text
Projetinho/TornoREV2/Assets/TornoMecanico/Torno Mecanico/torno.fbx
```

## Cena criada para visualização

A cena criada para visualizar o torno fica em:

```text
Projetinho/TornoREV2/Assets/Scenes/TornoMecanico.unity
```

Ela contém:

- o modelo do torno;
- uma câmera;
- uma luz direcional.

## O que não foi encontrado

Não foi encontrada uma cena pronta do torno com:

- interação de botões;
- animação das partes;
- simulação mecânica;
- controle por teclado/mouse;
- experiência VR funcional dedicada ao torno.
