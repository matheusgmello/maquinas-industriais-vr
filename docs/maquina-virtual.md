# Máquina Virtual

O arquivo:

```text
DPADI0191-002.ova
```

é uma máquina virtual exportada do VirtualBox.

## Conteúdo identificado

O appliance contém:

- `DPADI0191.ovf`
- `DPADI0191-disk001.vmdk`
- `DPADI0191.mf`

Configuração identificada:

- sistema: Ubuntu 64-bit;
- CPU: 1 vCPU;
- memória: 4096 MB;
- disco virtual: 40 GB;
- origem: VirtualBox.

## Como abrir

No VirtualBox:

```text
Arquivo > Importar Appliance
```

Selecione:

```text
DPADI0191-002.ova
```

## Por que alguém usaria VM?

Possíveis motivos:

- padronizar o ambiente entre computadores do laboratório;
- evitar instalar Unity e dependências em todos os PCs;
- preservar uma configuração pronta;
- isolar alterações do sistema principal;
- armazenar arquivos que não vieram separados.

## Observação de desempenho

Para Unity 3D ou VR, rodar dentro de VM geralmente não melhora desempenho. Em máquinas fracas, pode piorar por limitações de aceleração gráfica.

A VM ainda pode ser útil para verificar se existe algum arquivo original que não apareceu na pasta do Windows.
