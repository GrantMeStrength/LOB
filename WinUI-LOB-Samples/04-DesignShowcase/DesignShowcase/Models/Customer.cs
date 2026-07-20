namespace DesignShowcase.Models;

/// <summary>
/// A sample line-of-business customer record shown in the Customers list.
/// </summary>
public sealed class Customer
{
    public Customer(string name, string company, string status)
    {
        Name = name;
        Company = company;
        Status = status;
    }

    public string Name { get; }

    public string Company { get; }

    public string Status { get; }

    /// <summary>Initials used for the avatar (derived from <see cref="Name"/>).</summary>
    public string Initials
    {
        get
        {
            var parts = Name.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return "?";
            }

            return parts.Length == 1
                ? parts[0][..1].ToUpperInvariant()
                : (parts[0][..1] + parts[^1][..1]).ToUpperInvariant();
        }
    }
}
