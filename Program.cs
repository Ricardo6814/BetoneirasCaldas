using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

class Aluguel
{
    public string Cliente { get; set; }
    public string Equipamento { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public int DiaDoMes { get; set; }
    public bool Pago { get; set; }
    public decimal ValorTotal => Quantidade * ValorUnitario;
}

class Divida
{
    public string Credor { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public bool Paga { get; set; }
}

class Program
{
    static void Main()
    {
        List<Aluguel> alugueis = new List<Aluguel>();
        List<Divida> dividas = new List<Divida>();
        decimal totalBruto = 0;

        Console.WriteLine("=== Sistema de Registro de Aluguéis ===");

        while (true)
        {
            Console.Write("\nNome do cliente (ou 'sair' para finalizar): ");
            string cliente = Console.ReadLine();

            if (cliente.ToLower() == "sair")
                break;

            Console.Write("Nome do equipamento: ");
            string equipamento = Console.ReadLine();

            Console.Write("Quantidade (dias ou unidades): ");
            if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
            {
                Console.WriteLine("Quantidade inválida!");
                continue;
            }

            Console.Write("Valor unitário do aluguel: ");
            if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorUnitario))
            {
                Console.WriteLine("Valor inválido!");
                continue;
            }

            Console.Write("Dia do mês do aluguel (1 a 31): ");
            if (!int.TryParse(Console.ReadLine(), out int diaDoMes) || diaDoMes < 1 || diaDoMes > 31)
            {
                Console.WriteLine("Dia inválido!");
                continue;
            }

            Console.Write("Pagamento realizado? (s/n): ");
            string pagoStr = Console.ReadLine().Trim().ToLower();
            bool pago = pagoStr == "s";

            var aluguel = new Aluguel
            {
                Cliente = cliente,
                Equipamento = equipamento,
                Quantidade = quantidade,
                ValorUnitario = valorUnitario,
                DiaDoMes = diaDoMes,
                Pago = pago
            };

            alugueis.Add(aluguel);
            totalBruto += aluguel.ValorTotal;
        }

        Console.WriteLine("\n=== Registro de Dívidas da Empresa ===");

        while (true)
        {
            Console.Write("\nNome do credor (ou 'sair' para finalizar): ");
            string credor = Console.ReadLine();

            if (credor.ToLower() == "sair")
                break;

            Console.Write("Descrição da dívida: ");
            string descricao = Console.ReadLine();

            Console.Write("Valor da dívida: ");
            if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                Console.WriteLine("Valor inválido!");
                continue;
            }

            Console.Write("Dívida paga? (s/n): ");
            string pagaStr = Console.ReadLine().Trim().ToLower();
            bool paga = pagaStr == "s";

            var divida = new Divida
            {
                Credor = credor,
                Descricao = descricao,
                Valor = valor,
                Paga = paga
            };

            dividas.Add(divida);
        }

        // Cálculos de aluguéis
        decimal parteJoaci = totalBruto * 0.35m;
        decimal parteHumberto = totalBruto - parteJoaci;

        // Cálculo de dívidas
        decimal totalDividas = 0, totalPagas = 0, totalPendentes = 0;
        foreach (var d in dividas)
        {
            totalDividas += d.Valor;
            if (d.Paga) totalPagas += d.Valor;
            else totalPendentes += d.Valor;
        }

        // Cálculo das entradas (apenas aluguéis pagos)
        decimal totalEntradas = 0;
        foreach (var a in alugueis)
        {
            if (a.Pago) totalEntradas += a.ValorTotal;
        }

        // Fluxo de caixa
        decimal saldoFinal = totalEntradas - totalPagas;

        // Lucros reais
        decimal lucroLiquido = totalEntradas - totalPagas;
        decimal lucroJoaci = lucroLiquido * 0.35m;
        decimal lucroHumberto = lucroLiquido - lucroJoaci;

        string caminho = "relatorio.txt";
        using (StreamWriter sw = new StreamWriter(caminho))
        {
            sw.WriteLine("=== RELATÓRIO DE ALUGUÉIS ===\n");

            int contador = 1;
            foreach (var aluguel in alugueis)
            {
                sw.WriteLine($"Aluguel #{contador++}");
                sw.WriteLine($"Cliente:         {aluguel.Cliente}");
                sw.WriteLine($"Equipamento:     {aluguel.Equipamento}");
                sw.WriteLine($"Quantidade:      {aluguel.Quantidade}");
                sw.WriteLine($"Valor Unitário:  R${aluguel.ValorUnitario:F2}");
                sw.WriteLine($"Total:           R${aluguel.ValorTotal:F2}");
                sw.WriteLine($"Dia do Mês:      {aluguel.DiaDoMes}");
                sw.WriteLine($"Pagamento:       {(aluguel.Pago ? "Pago" : "Não Pago")}");
                sw.WriteLine(new string('-', 40));
            }

            sw.WriteLine("\n=== RESUMO FINAL DOS ALUGUÉIS ===");
            sw.WriteLine($"Total Bruto:          R${totalBruto:F2}");
            sw.WriteLine($"Parte de Joaci (35%): R${parteJoaci:F2}");
            sw.WriteLine($"Parte de Humberto:    R${parteHumberto:F2}");

            sw.WriteLine("\n=== DÍVIDAS REGISTRADAS ===");

            int numero = 1;
            foreach (var d in dividas)
            {
                sw.WriteLine($"Dívida #{numero++}");
                sw.WriteLine($"Credor:     {d.Credor}");
                sw.WriteLine($"Descrição:  {d.Descricao}");
                sw.WriteLine($"Valor:      R${d.Valor:F2}");
                sw.WriteLine($"Situação:   {(d.Paga ? "Paga" : "Não Paga")}");
                sw.WriteLine(new string('-', 40));
            }

            sw.WriteLine("\n=== RESUMO DAS DÍVIDAS ===");
            sw.WriteLine($"Total de Dívidas:     R${totalDividas:F2}");
            sw.WriteLine($"Total Já Pago:        R${totalPagas:F2}");
            sw.WriteLine($"Total Pendente:       R${totalPendentes:F2}");

            sw.WriteLine("\n=== FLUXO DE CAIXA ===");
            sw.WriteLine($"Entradas (Aluguéis Pagos): R${totalEntradas:F2}");
            sw.WriteLine($"Saídas (Dívidas Pagas):     R${totalPagas:F2}");
            sw.WriteLine($"Saldo Final:                R${saldoFinal:F2}");

            sw.WriteLine("\n=== LUCRO LÍQUIDO POR SÓCIO ===");
            sw.WriteLine($"Lucro Líquido Total:       R${lucroLiquido:F2}");
            sw.WriteLine($"Lucro de Joaci (35%):      R${lucroJoaci:F2}");
            sw.WriteLine($"Lucro de Humberto (65%):   R${lucroHumberto:F2}");
        }

        // Console outputs finais
        Console.WriteLine("\nRelatório gerado com sucesso em 'relatorio.txt'!");
        Console.WriteLine($"\nSaldo final do caixa:      R${saldoFinal:F2}");
        Console.WriteLine($"Lucro líquido da empresa:  R${lucroLiquido:F2}");
        Console.WriteLine($"Lucro de Joaci (35%):      R${lucroJoaci:F2}");
        Console.WriteLine($"Lucro de Humberto (65%):   R${lucroHumberto:F2}");
    }
}
