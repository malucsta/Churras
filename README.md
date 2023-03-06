# Churras

#### Requisitos Técnicos

Na [wiki](https://github.com/trinca137/trinca-challenge/wiki/Comece-por-aqui) estão as informações relevantes sobre o desafio.

#### Implementação

Pro Churras ficar mais que perfeito, fiz algumas alterações para organizar tanto a estrutura da aplicação quanto às próprias regras. Dentre design centrado no domínio refletido nas pastas e divisão em camadas separadas por *concerns*, agora também é possível não só organizar a lista de cada churrasco, mas ter a lista pelo id dos convidados. Dessa forma, através do novo enpoint de `api/churras/{bbqId}/lista-de-compras/estimativa` podemos saber quanto cada pessoa terá que contribuir, com base no preço da carne e dos vegetais do rolê

E mais! Além da validação das requests para garantir que tudo seja passado certinho, temos validação, também, de regras de negócio. Ninguém merece marcar dois churrascos ao mesmo tempo e causar aquela 🥧 de # climão, não é? Sempre fica melhor com todo mundo junto! :)

E, claro, para saber quando cada um desses B.Os rolam, tem todo um tratamento de erros com códigos internos de erro e objetos fortemente tipados com mensagens personalizadas que são logados quando rolam. Um pouquinho de observabilidade sempre é bom. 


#### Sobre as branches

Existem duas branches no projeto: a main e a feature-infra-layer. A main contém tudo que a feature-infra-layer, com exceção de uma nova camada para separar o domínio da implementação do banco de dados, isolando-o em seus *concerns*. A razão para estarem separadas é explicada a seguir. 

Na User Story de Criar um Novo Churrasco, a especificação é a seguinte:

"A interface "IEventStore" está sendo utilizada direto na camada de _api_. Aliás, ela nem deveria ser acessível por esta camada. Encapsule-a para que ela seja acessível apenas dentro de seu _assembly_ de origem (_Domain_)"

A criação de uma nova camada que acessa o EventStore não no domínio quebraria um tanto essa regra, então fica como extra apenas para seguir a lógica da separação. 


### Observação

A regra do aceite e rejeição do convite impõem sete repetições, entretanto, existem apenas 5 ids distintos. Respeitando a requisição, a regra continua como 7 na entidade BBQ. 
