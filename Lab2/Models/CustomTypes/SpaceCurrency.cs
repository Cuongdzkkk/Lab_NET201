using System;
using System.Globalization;

namespace Lab2.Models.CustomTypes
{
    /// <summary>
    /// Represents a monetary value in the Nexus Galaxy.
    /// Immutable Value Object.
    /// </summary>
    public record SpaceCurrency : IComparable<SpaceCurrency>
    {
        public decimal Amount { get; }
        public string Code { get; } // e.g., "NXS", "CRD", "SOL"

        // Default currency for the Nexus Lab
        public static readonly string DefaultCode = "NXS";

        /// <summary>
        /// Creates a new SpaceCurrency.
        /// </summary>
        /// <param name="amount">The monetary value (must be non-negative).</param>
        /// <param name="code">The 3-letter currency code (default: NXS).</param>
        /// <exception cref="ArgumentException">Thrown if validation fails.</exception>
        public SpaceCurrency(decimal amount, string code = "NXS")
        {
            if (amount < 0)
                throw new ArgumentException("Currency amount cannot be negative in the Nexus protocol.", nameof(amount));

            if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
                throw new ArgumentException("Currency code must be exactly 3 characters (e.g., NXS).", nameof(code));

            Amount = amount;
            Code = code.ToUpperInvariant();
        }

        // === OPERATORS ===

        public static SpaceCurrency operator +(SpaceCurrency a, SpaceCurrency b)
        {
            EnsureSameCurrency(a, b);
            return new SpaceCurrency(a.Amount + b.Amount, a.Code);
        }

        public static SpaceCurrency operator -(SpaceCurrency a, SpaceCurrency b)
        {
            EnsureSameCurrency(a, b);
            // Result can be negative? Assuming no debt in this context or checking validation.
            // If validation enforces non-negative, we check:
            if (a.Amount - b.Amount < 0)
                 throw new InvalidOperationException("Insufficient funds for transaction.");

            return new SpaceCurrency(a.Amount - b.Amount, a.Code);
        }

        public static SpaceCurrency operator *(SpaceCurrency a, decimal multiplier)
        {
            if (multiplier < 0) throw new ArgumentException("Multiplier cannot be negative.");
            return new SpaceCurrency(a.Amount * multiplier, a.Code);
        }

        public static bool operator >(SpaceCurrency a, SpaceCurrency b)
        {
            EnsureSameCurrency(a, b);
            return a.Amount > b.Amount;
        }

        public static bool operator <(SpaceCurrency a, SpaceCurrency b)
        {
            EnsureSameCurrency(a, b);
            return a.Amount < b.Amount;
        }

        // === METHODS ===

        /// <summary>
        /// Returns a formatted string suitable for the HUD Terminal.
        /// Format: "[ CODE : AMOUNT ]"
        /// </summary>
        public override string ToString()
        {
            return $"[ {Code} : {Amount.ToString("N2", CultureInfo.InvariantCulture)} ]";
        }

        public string ToCompactString()
        {
            if (Amount >= 1_000_000)
                return $"{Code} {(Amount / 1_000_000):F1}M";
            if (Amount >= 1_000)
                return $"{Code} {(Amount / 1_000):F1}K";
            
            return ToString();
        }

        public int CompareTo(SpaceCurrency? other)
        {
            if (other is null) return 1;
            EnsureSameCurrency(this, other);
            return Amount.CompareTo(other.Amount);
        }

        // === HELPERS ===

        private static void EnsureSameCurrency(SpaceCurrency a, SpaceCurrency b)
        {
            if (a.Code != b.Code)
                throw new InvalidOperationException($"Currency mismatch: {a.Code} vs {b.Code}. Exchange protocol required.");
        }

        // Factory for zero
        public static SpaceCurrency Zero(string code = "NXS") => new SpaceCurrency(0, code);
    }
}
