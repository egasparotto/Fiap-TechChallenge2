# FIAP ORDERS

[![Deploy no Azure](https://github.com/egasparotto/Fiap-TechChallenge2/actions/workflows/FiapTechChallenge2.yml/badge.svg)](https://github.com/egasparotto/Fiap-TechChallenge2/actions/workflows/FiapTechChallenge2.yml)

Projeto criado para o Tech Challenge da fase 2 do curso de pós graduação em **Arquitetura de Sistemas .NET com Azure**.
Turma **2023/2**

## Sobre o projeto
Fiap Orders é um serviço de processamento de pedidos. Desenvolvido em .NET 7.0 para ser utilizado no serviço serverless da microsoft (Azure Functions).

O projeto também está hospedado no azure através da url https://fiaptechchallenge2.azurewebsites.net

## Processo
 ![Processo do serivço](https://raw.githubusercontent.com/egasparotto/Fiap-TechChallenge2/main/processo.png)


## Continuous Deployment

Sempre que é efetuado um novo push no projeto, é gerada uma nova versão e atualizado no Azure Functions.

A Pipeline de deploy é executada através do github actions - [Ver Processo](https://github.com/egasparotto/Fiap-TechChallenge2/actions)

## Documentação da API

### Inserir novo Pedido

Para inserir um novo pedido devem ser informados o nome, e-mail do cliente e os itens do pedido.
O link de confirmação de pagamento e a mensagem de confirmação são enviados para o cliente.

Também é enviado um email para o cliente caso o pagamento não seja confirmado a tempo (5 Minutos)

O Retorno do método contém os dados do pedido, e também a url de confirmação de pagamento.

**URL:**  `https://fiaptechchallenge2.azurewebsites.net/api/NewOrder`
**Método:** `POST`

#### Parâmetros

```json
{
	"name":  "Cliente Teste",
	"email":  "teste@email.com",
	"items":  [
		{
			"description":  "Caneta",
			"quantity":  1,
			"price":  1.90
		},
		{
			"description":  "Carne",
			"quantity":  0.590,
			"price":  53.00
		}
	]
}
```

#### Response
```json
{
	"order":  {
		"Name":  "Cliente Teste",
		"Email":  "teste@email.com",
		"Total":  33.170,
		"Items":  [
			{
				"Description":  "Caneta",
				"Quantity":  1,
				"Price":  1.90,
				"Total":  1.90
			},
			{
				"Description":  "Carne",
				"Quantity":  0.590,
				"Price":  53,
				"Total":  31.270
			}
		]
	},
	"aprrovePaymentUrl": "https://fiaptechchallenge2.azurewebsites.net/api/AprovePayment/4314da8575834148a6b5212d42669d5e"
}
```

### Confirmar Pagamento

Utilizado para confirmar pagamento.
Através do Id do Pedido, é localizada o pedido e é efetuado a confirmação do pagamento.

Após a confirmação é enviado um email para o cliente, confirmando o pagamento.

**URL:**  `https://fiaptechchallenge2.azurewebsites.net/api/AprovePayment/{ID_DO_PEDIDO}`
**Método:** `GET`

#### Response
```json
{
	"success": true
}
```


# Documentação do WorkFlow -Build e Deploy no Azure

Este repositório utiliza GitHub Actions para automatizar o processo de compilação e implantação de uma Azure Function App. O fluxo de trabalho é dividido em duas etapas principais: `build` e `deploy`.

### Detalhes do Fluxo de Trabalho

#### Gatilho do Fluxo de Trabalho

O fluxo de trabalho é acionado automaticamente quando há um push no ramo `main`.

```yaml
on:
  push:
    branches:
    - main
```

#### Variáveis de Ambiente
Variáveis de ambiente são configuradas para armazenar informações como nome da Function App, caminho do pacote, versão do .NET Core, etc.

```yaml
env:
  AZURE_FUNCTIONAPP_NAME: FiapTechChallenge2
  AZURE_FUNCTIONAPP_PACKAGE_PATH: FiapOrders/published
  CONFIGURATION: Release
  DOTNET_CORE_VERSION: 7.0.x
  WORKING_DIRECTORY: FiapOrders
  DOTNET_CORE_VERSION_INPROC: 7.0.x
```

#### JOB de Build
O job de build é responsável por compilar o projeto, restaurar dependências e gerar o pacote da Function App.

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET SDK
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: ${{ env.DOTNET_CORE_VERSION }}
      - name: Setup .NET Core (for inproc extensions)
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: ${{ env.DOTNET_CORE_VERSION_INPROC }}
      - name: Restore
        run: dotnet restore "${{ env.WORKING_DIRECTORY }}"
      - name: Build
        run: dotnet build "${{ env.WORKING_DIRECTORY }}" --configuration ${{ env.CONFIGURATION }} --no-restore
      - name: Publish
        run: dotnet publish "${{ env.WORKING_DIRECTORY }}" --configuration ${{ env.CONFIGURATION }} --no-build --output "${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}"
      - name: Publish Artifacts
        uses: actions/upload-artifact@v3
        with:
          name: functionapp
          path: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}

```

#### JOB de Deploy
O job de deploy faz o download do artefato gerado pelo trabalho de build e o utiliza para implantar a Function App no Azure.

```yaml
  deploy:
    runs-on: ubuntu-latest
    needs: build
    steps:
      - name: Download artifact from build job
        uses: actions/download-artifact@v3
        with:
          name: functionapp
          path: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}
      - name: Deploy to Azure Function App
        uses: Azure/functions-action@v1
        with:
          app-name: ${{ env.AZURE_FUNCTIONAPP_NAME }}
          publish-profile: ${{ secrets.FiapTechChallenge2_23F8 }}
          package: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}

```

### Observações Importantes
Certifique-se de configurar corretamente as variáveis de ambiente.

* Certifique-se de configurar corretamente as variáveis de ambiente.
* As credenciais sensíveis, como o perfil de publicação, devem ser configuradas como segredos no GitHub para proteger informações confidenciais.
* Antes de executar este fluxo de trabalho, certifique-se de ter uma Azure Function App criada e pronta para receber a implantação.
