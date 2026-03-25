namespace FinanceControl.Shared.Helpers
{
    public static class PaymentMethodsByCountry
    {
        private static readonly Dictionary<string, List<string>> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["BR"] = ["DINHEIRO", "CARTAO_DEBITO", "CARTAO_CREDITO", "PIX", "BOLETO", "TRANSFERENCIA", "CHEQUE"],
            ["US"] = ["CASH", "DEBIT_CARD", "CREDIT_CARD", "ACH", "ZELLE", "CHECK", "WIRE_TRANSFER", "PAYPAL"],
            ["PT"] = ["DINHEIRO", "CARTAO_DEBITO", "CARTAO_CREDITO", "TRANSFERENCIA", "MULTIBANCO", "MBWAY"],
            ["AR"] = ["EFECTIVO", "TARJETA_DEBITO", "TARJETA_CREDITO", "TRANSFERENCIA", "MERCADOPAGO"],
            ["MX"] = ["EFECTIVO", "TARJETA_DEBITO", "TARJETA_CREDITO", "TRANSFERENCIA", "SPEI", "OXXO_PAY"],
            ["GB"] = ["CASH", "DEBIT_CARD", "CREDIT_CARD", "BANK_TRANSFER", "FASTER_PAYMENTS", "CHEQUE"],
            ["DE"] = ["BARGELD", "EC_KARTE", "KREDITKARTE", "UBERWEISUNG", "LASTSCHRIFT", "PAYPAL"],
            ["FR"] = ["ESPECES", "CARTE_BANCAIRE", "VIREMENT", "CHEQUE", "PRELEVEMENT"],
            ["IT"] = ["CONTANTI", "CARTA_DEBITO", "CARTA_CREDITO", "BONIFICO", "SATISPAY"],
            ["ES"] = ["EFECTIVO", "TARJETA_DEBITO", "TARJETA_CREDITO", "TRANSFERENCIA", "BIZUM"],
        };

        private static readonly List<string> _default =
        [
            "CASH", "DEBIT_CARD", "CREDIT_CARD", "BANK_TRANSFER"
        ];

        public static List<string> GetForCountry(string? countryCode)
        {
            if (countryCode is not null && _map.TryGetValue(countryCode, out var methods))
                return methods;

            return _default;
        }

        public static bool IsValid(string paymentMethod, string? countryCode)
        {
            var methods = GetForCountry(countryCode);
            return methods.Contains(paymentMethod, StringComparer.OrdinalIgnoreCase);
        }
    }
}
