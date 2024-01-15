using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiapOrders.Domain.Entities.Orders
{
    public class Order
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public decimal Total { get => (Items?.Sum(x => x.Total)).GetValueOrDefault(0); }
        public required IEnumerable<Item> Items { get; set; }

        public override string? ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<h1>Fiap Tech Challenge 2</h1>");
            sb.AppendLine($"<p>Olá {Name},</p>");
            sb.AppendLine("<p>Confirmamos o seu pagamento</p>");
            sb.AppendLine("<p>Resumo do pedido</p>");
            sb.AppendLine("<table>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th>Descrição</th>");
            sb.AppendLine("<th>Quantidade</th>");
            sb.AppendLine("<th>Preço Un</th>");
            sb.AppendLine("<th>Total</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");
            foreach(var item in Items)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{item.Description}</td>");
                sb.AppendLine($"<td>{item.Quantity}</td>");
                sb.AppendLine($"<td>R$ {item.Price.ToString("0.00")}</td>");
                sb.AppendLine($"<td>R$ {item.Total.ToString("0.00")}</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody>");
            sb.AppendLine("<tfooter>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td></td>");
            sb.AppendLine("<td></td>");
            sb.AppendLine("<td>Total:</td>");
            sb.AppendLine($"<td>R$ {Total.ToString("0.00")}</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</tfooter>");
            sb.AppendLine("</table>");
            return sb.ToString();
        }
    }
}
