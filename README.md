# `alelo-cli` gerenciando os *vales* na linha de comando

O objetivo da ferramenta é facilitar o consumo e gestão de extratos e cartões, baseado nos fluxos correspondentes ao da aplicação.

> **Nota**: Projeto em desenvolvimento, ainda não temos nenhuma *release* produtiva.

## Recursos

A aplicação visa fornecer uma API simples e eficiente para orquestrar múltiplas contas do aplicativo Meu Alelo. Toda documentação da ferramenta pode ser consultada através da *flag* `--help`.

Por conta da gestão de múltiplas contas, a hierarquia de sessão é baseada sob um *profile*, nominado de acordo com a parametrização ao consumir `profile --create <profile name>`. Para facilitar a utilização, o último *profile* criado, sempre será o padrão, quando a variável de ambiente `ALELO_DEFAULT_PROFILE` estiver vazia.

É possível listar cartões, extratos, *profiles* e ações especificas de cada comando raiz, como por exemplo redefinição de senha de vales e consulta de estabelecimentos disponíveis de acordo com as coordenadas parametrizadas.

```
Alelo.Console:
  Meu Alelo as a command line interface, but better

Usage:
  Alelo.Console [options] [command]

Options:
  -s, --statement    List last transactions for the default card (123)
  -v, --verbose      Increase the application verbose
  -e, --env          Show application environment variables
  --version          Show version information
  -?, -h, --help     Show help and usage information

Commands:
  profile    Select default, create, delete and list user profiles for Meu Alelo
  card       Select default and list user cards
```

## Variáveis de Ambiente

Para consultar as variáveis de ambiente da aplicação, basta consumir a opção `--env` ou `-e` na raiz. Atualmente, três variáveis são consumidas para parametrização de valores padrões para diretório raiz da aplicação, *profile* e cartão, sendo elas:

- `ALELO_HOME`: Parametriza a raiz do diretório utilizada pelo `alelo-cli`, caso vazia, o valor padrão `$USER/.alelo` para Linux e macOS ou `%userprofile% \Documents\.alelo` dependendo do idioma do sistema;
- `ALELO_DEFAULT_PROFILE`: Seleciona o *profile* padrão, caso mais de um exista dentro de `ALELO_HOME`, e caso vazia, o *profile* criado mais recente é selecionado como *default*;
- `ALELO_DEFAULT_CARD`: Seleciona o cartão padrão para consultas de linha de comando, caso vazio, o primeiro cartão correspondente ao *profile default* será utilizado.

Sendo todas essas opcionais, caso não instanciadas, os valores padrões são utilizados.

## Ambiente de Desenvolvimento

Para colaborar com o projeto - ainda não temos nenhum *template* de *issue* ou padrões de PRs - basta respeitar a versão do *dotnet core SDK* utilizada (`3.1.101` ou superior, desde que seja uma *release* estável e oficial) e o editor de texto ou IDE da sua preferência. 

Enquanto não tivermos nenhuma *release* produtiva oficial, todos os *commits* serão enviados diretamente para *master*.

## Licença

O projeto é licenciado sob *Do What The Fuck You Want To Public License* (WTFPL), e não possui nenhum vinculo com a Alelo ou qualquer uma de suas marcas.