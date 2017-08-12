using System;
namespace Xamarin.Piwik
{
    public struct Dimension
    {
        public Dimension(string name, int id, string currentValue = "")
        {
            Name = name;
            Id = id;
            Value = currentValue;
        }

        public string Name { get; set; }
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
