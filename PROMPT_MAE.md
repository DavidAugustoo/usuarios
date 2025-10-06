# Prompt Mestre para Estrutura de Event Sourcing em Pagamentos

Você é um desenvolvedor responsável por evoluir o microsserviço de Pagamentos. O objetivo é implementar uma arquitetura de _Event Sourcing_ seguindo o padrão utilizado no serviço de Usuários deste repositório. Ao realizar qualquer ajuste, mantenha a aplicação funcional e evite alterações desnecessárias.

## Diretrizes Gerais
1. **Contexto de Pagamentos**: todos os eventos devem refletir ações do domínio de pagamentos (por exemplo: criação de transação, autorização, confirmação, estorno, falha de pagamento ou eventos de autenticação relacionados ao fluxo de pagamento).
2. **Event Sourcing**: persista cada evento na tabela `StoredEvent`, mantendo `AggregateId`, `EventType`, `Data`, `Timestamp` e `Version` conforme configurado em `FCG.Domain.Entities.StoredEvent`.
3. **Publisher**: reutilize o `IEventPublisher`/`EventPublisher` para serializar eventos em JSON e armazená-los via `IEventStoreRepository`.
4. **Eventos de Domínio**: para cada ação relevante, crie uma classe de evento em `FCG.Domain.EventSourcing.Events` estendendo `Event`. Inclua apenas os dados necessários (evite dados sensíveis).
5. **Serviços de Aplicação**: injete `IEventPublisher` nos serviços responsáveis pelo caso de uso (por exemplo, `PagamentoService`). Após concluir a operação e confirmar a transação com o `IUnitOfWork`, publique o evento correspondente.
6. **Repositórios**: mantenha os repositórios responsáveis por manipular os agregados de pagamento. Utilize o `UnitOfWork` existente para garantir consistência entre a operação e o registro do evento.
7. **Configuração**: registre `IEventStoreRepository` e `IEventPublisher` na injeção de dependências (ver `FCG.Infra.Ioc.DependencyInjection`). Certifique-se de que o contexto (`ApplicationDbContext`) conhece o `DbSet<StoredEvent>`.
8. **Projeções/Leituras**: quando for necessário criar _read models_, utilize uma interface de projeção para consumir `StoredEvent` e montar visões específicas (seguindo o padrão de `UsuarioProjection` quando disponível).
9. **Segurança de Dados**: eventos podem ser consultados posteriormente; não serialize chaves privadas, senhas ou tokens. Prefira armazenar identificadores e metadados relevantes.
10. **Testes**: não é obrigatório criar testes automatizados para validar os eventos neste momento; priorize manter a cobertura existente funcionando.

## Fluxo Recomendado ao Implementar um Novo Evento
1. Criar/atualizar a entidade ou serviço responsável pela ação de pagamento.
2. Criar o evento de domínio derivado de `Event` em `FCG.Domain.EventSourcing.Events`.
3. Invocar `IEventPublisher.PublishAsync` após a operação persistir com sucesso.
4. Garantir que o `AggregateId` seja preenchido com o identificador da entidade de pagamento.
5. Validar manualmente que o evento é persistido na tabela `StoredEvent`.

Siga essas diretrizes para garantir que o microsserviço de Pagamentos permaneça alinhado à arquitetura de Event Sourcing adotada neste projeto.
