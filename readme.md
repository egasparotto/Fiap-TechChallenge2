# FIAP ORDERS

Projeto criado para o Tech Challenge da fase 2 do curso de pós graduação em **Arquitetura de Sistemas .NET com Azure**.
Turma **2023/2**

## Sobre o projeto
Fiap Orders é um serviço de processamento de pedidos. Desenvolvido em .NET 7.0 para ser utilizado no serviço serverless da microsoft (Azure Functions).

O projeto também está hospedado no azure através da url https://fiaptechchallenge2.azurewebsites.net

## Processo
 ![Processo do serivço](https://raw.githubusercontent.com/egasparotto/Fiap-TechChallenge2/main/processo.png)


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