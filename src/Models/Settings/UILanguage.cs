namespace PartyYomi.Models.Settings
{
    public class UILanguage
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
