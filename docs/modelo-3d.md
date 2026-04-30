# Modelo 3D

O modelo principal é:

```text
torno.fbx
```

Ele foi exportado a partir do Blender, conforme os metadados encontrados no arquivo FBX.

## Estado visual

O modelo aparece majoritariamente branco/cinza no Unity. Isso indica que ele veio sem materiais/texturas completas ou que os materiais não foram associados corretamente durante a importação.

Isso pode acontecer quando:

- o FBX é exportado sem materiais;
- as texturas não são enviadas junto;
- os materiais existem, mas estão sem imagens associadas;
- o projeto usa URP e materiais antigos não foram convertidos.

## Possíveis melhorias

Para melhorar a visualização:

- criar materiais no Unity e aplicar nas partes do torno;
- abrir o modelo no Blender, configurar cores/materiais e exportar novamente;
- organizar partes do modelo para facilitar interação futura;
- separar peças que devem girar ou se mover.

## Observação sobre versionamento

O `torno.fbx` atual é relativamente pequeno e pode ser versionado. Para modelos maiores, usaremos Git LFS ou manteremos os arquivos brutos fora do repositório.
