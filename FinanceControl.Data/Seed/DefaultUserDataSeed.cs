namespace FinanceControl.Data.Seed
{
    public static class DefaultUserDataSeed
    {
        /// <summary>
        /// Returns the default categories and subcategories to create for a new user.
        /// Structure: (CategoryName, TransactionType, SubCategories[])
        /// TransactionType: "Expense" | "Income"
        /// </summary>
        public static IEnumerable<(string CategoryName, string Type, string[] SubCategories)> GetDefaultCategories()
        {
            return
            [
                // DESPESAS
                ("Moradia", "Expense",
                [
                    "Aluguel / Financiamento",
                    "Condomínio",
                    "Água e Esgoto",
                    "Energia Elétrica",
                    "Gás",
                    "Internet",
                    "Manutenção / Reparos",
                    "Seguro Residencial"
                ]),
                ("Alimentação", "Expense",
                [
                    "Mercado / Supermercado",
                    "Feira / Hortifruti",
                    "Restaurante",
                    "Delivery",
                    "Padaria / Café",
                    "Bar"
                ]),
                ("Transporte", "Expense",
                [
                    "Combustível",
                    "Transporte Público",
                    "Uber / Táxi",
                    "Estacionamento / Pedágio",
                    "Manutenção do Veículo",
                    "Seguro do Veículo",
                    "IPVA / Licenciamento"
                ]),
                ("Saúde", "Expense",
                [
                    "Plano de Saúde",
                    "Consulta Médica",
                    "Exames",
                    "Farmácia / Medicamentos",
                    "Dentista",
                    "Academia / Esportes"
                ]),
                ("Educação", "Expense",
                [
                    "Mensalidade Escolar / Faculdade",
                    "Cursos e Capacitações",
                    "Livros / Material Escolar",
                    "Idiomas"
                ]),
                ("Lazer e Entretenimento", "Expense",
                [
                    "Cinema / Teatro / Shows",
                    "Assinaturas (streaming, música)",
                    "Viagem / Hospedagem",
                    "Jogos",
                    "Hobbies"
                ]),
                ("Vestuário", "Expense",
                [
                    "Roupas",
                    "Calçados",
                    "Acessórios"
                ]),
                ("Cuidados Pessoais", "Expense",
                [
                    "Salão / Barbearia",
                    "Produtos de Higiene",
                    "Cosméticos / Beleza"
                ]),
                ("Pets", "Expense",
                [
                    "Ração",
                    "Veterinário",
                    "Petshop / Higiene"
                ]),
                ("Financeiro", "Expense",
                [
                    "Empréstimo / Financiamento",
                    "Tarifas Bancárias",
                    "Seguros"
                ]),
                ("Impostos e Taxas", "Expense",
                [
                    "IPTU",
                    "Imposto de Renda",
                    "Outras Taxas"
                ]),
                ("Presentes e Doações", "Expense",
                [
                    "Presente",
                    "Doação"
                ]),

                // RECEITAS
                ("Trabalho", "Income",
                [
                    "Salário",
                    "Freelance / Bico",
                    "13º Salário",
                    "Férias",
                    "Bônus / PLR",
                    "Adiantamento"
                ]),
                ("Investimentos", "Income",
                [
                    "Rendimentos / Juros",
                    "Dividendos"
                ]),
                ("Outros", "Income",
                [
                    "Venda de Itens",
                    "Aluguel Recebido",
                    "Reembolso",
                    "Benefícios (VA/VR/VT)",
                    "Presente / Doação Recebida"
                ]),
            ];
        }
    }
}
