namespace CountriesBackend.Models{
    public class Flags
    {
        public string Svg { get; set; }
        public string Png { get; set; }
    }

    public class Currency
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }

    public class Language
    {
        public string Name { get; set; }
    }


    public class Country
    {
        public string Name { get; set; }

        public string? Capital { get; set; }

        public int? Population { get; set; }

        public string? Alpha3Code {get; set; }

        public Flags? Flags { get; set; }

        public List<Currency>? Currencies { get; set; }

        public List<Language>? Languages { get; set; }
    }


}