# Roadmap

Esta é uma proposta de evolução para transformar o material em um projeto de realidade virtual mais completo.

## Frente atual - Torno mecânico

### Fase 1 - Organização ✅ Concluída

- [x] Inventário dos arquivos recebidos.
- [x] Separação de arquivos brutos, projeto Unity e documentação.
- [x] Configuração do Git com `.gitignore` e `.gitattributes` corretos.
- [x] Versionamento do projeto Unity no repositório.
- [x] Documentação técnica publicada com MkDocs + GitHub Pages.

### Fase 2 - Visualização ✅ Concluída

- [x] Importar modelo FBX no Unity com URP (Universal Render Pipeline).
- [x] Configurar 13 materiais URP com cores corretas.
- [x] Criar cena de visualização (`TornoMecanico.unity`).
- [x] Verificar hierarquia separada — cada peça é um GameObject independente.
- [x] Confirmar viabilidade de animações (peças separadas = animável).

### Fase 3 - Animações 🔄 Em andamento

- [x] Criar script editor `CreateTornoAnimations.cs`.
- [x] Criar `Assets/Animations/` com 5 clips de demonstração:
  - [x] `Placa_Girando` — EIXO ÁRVORE girando continuamente (loop).
  - [x] `Carro_Avancando` — CARRO LONGITUDINAL avançando e recuando (loop).
  - [x] `Manivela_Girando` — MANIVELA girando (loop).
  - [x] `Torre_Rotacionando` — TORRE rotacionando 90° (troca de ferramenta).
  - [x] `Carro_Transversal` — CARRO TRANSVERSAL avançando e recuando (loop).
- [x] Criar `TornoController.controller` (Animator Controller) com estados e transições.
- [x] Adicionar componente `Animator` ao objeto `torno` na cena.
- [ ] Ajustar eixos de rotação/translação após inspeção visual em Play Mode.
- [ ] Adicionar animações das CASTANHA 1/2/3 (garras do mandril abrindo/fechando).
- [ ] Sincronizar animação da manivela com o movimento do carro.
- [ ] Adicionar sons de máquina (motor, movimento mecânico).

### Fase 4 - Interação VR ⏳ Próxima etapa

- [ ] Adicionar colisores (Colliders) nas peças interativas.
- [ ] Configurar XR Origin com câmera e controladores.
- [ ] Implementar agarrar (XR Grab Interactable) nas manivelas.
- [ ] Criar estados: desligado → ligado → operando.
- [ ] Adicionar feedback visual (highlight com QuickOutline já instalado).
- [ ] Testar com simulador XR no editor.

### Fase 5 - Conteúdo didático ⏳ Futuro

- [ ] Criar objetivos de aprendizagem.
- [ ] Adicionar etiquetas/hotspots informativos nas partes do torno.
- [ ] Criar roteiro de operação guiada.
- [ ] Adicionar checklist de segurança e procedimentos.

### Fase 6 - Entrega ⏳ Futuro

- [ ] Gerar build executável (PC e/ou Meta Quest).
- [ ] Documentar requisitos de hardware.
- [ ] Criar vídeo ou imagens demonstrativas.

## Próximo passo - Fábrica virtual

Depois da etapa do torno mecânico, a proposta é evoluir para uma fábrica virtual: um cenário intuitivo onde os usuários possam explorar, testar e aprender sobre diferentes máquinas industriais em um ambiente seguro.

Essa frente amplia o projeto de um único equipamento para uma experiência educacional mais completa, simulando um espaço industrial com máquinas, áreas de circulação, sinalização, instruções e interações guiadas.

### Objetivos

- Criar um ambiente virtual com aparência de fábrica ou laboratório industrial.
- Permitir que o usuário navegue pelo cenário e escolha máquinas para estudar.
- Adicionar outras máquinas industriais além do torno mecânico.
- Criar interações simples e didáticas para cada máquina.
- Usar etiquetas, painéis e feedback visual para orientar o aprendizado.
- Manter a experiência segura, intuitiva e adequada para usuários iniciantes.

### Possíveis máquinas futuras

- Fresadora.
- Furadeira de bancada.
- Serra industrial.
- Prensa.
- Esteira transportadora.
- Robô industrial.
- Painel elétrico ou bancada de comandos.

### Entregas esperadas

- Cenário base da fábrica virtual.
- Sistema de navegação do usuário.
- Área dedicada ao torno mecânico.
- Estrutura reutilizável para adicionar novas máquinas.
- Documentação de uso e expansão do ambiente.
